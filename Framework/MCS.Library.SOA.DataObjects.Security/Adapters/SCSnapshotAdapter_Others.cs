using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Transactions;
using MCS.Library.Core;
using MCS.Library.Data;
using MCS.Library.Data.Builder;
using MCS.Library.Data.Mapping;
using MCS.Library.SOA.DataObjects.Security.Actions;
using MCS.Library.SOA.DataObjects.Security.Configuration;
using MCS.Library.SOA.DataObjects.Schemas.SchemaProperties;

namespace MCS.Library.SOA.DataObjects.Security.Adapters
{
	public partial class SCSnapshotAdapter
	{
		#region Parents Info
		/// <summary>
		/// 根据ID载入所有父级关系
		/// </summary>
		/// <param name="ids">子级ID</param>
		/// <returns>返回父对象的集合，结果中不包含自己</returns>
		public Dictionary<string, SCSimpleObjectCollection> LoadAllParentsInfo(params string[] ids)
		{
			return LoadAllParentsInfo(ids, false, DateTime.MinValue);
		}

		/// <summary>
		/// 根据ID载入所有父级关系
		/// </summary>
		/// <param name="ids">子级ID</param>
		/// <param name="includingSelf">结果中是否包含自己</param>
		/// <returns></returns>
		public Dictionary<string, SCSimpleObjectCollection> LoadAllParentsInfo(bool includingSelf, params string[] ids)
		{
			return LoadAllParentsInfo(ids, includingSelf, DateTime.MinValue);
		}

		/// <summary>
		/// 根据ID和时间点载入所有父级关系
		/// </summary>
		/// <param name="ids">子级ID</param>
		/// <param name="includingSelf">结果中是否包含自己</param>
		/// <param name="timePoint"></param>
		/// <returns></returns>
		public Dictionary<string, SCSimpleObjectCollection> LoadAllParentsInfo(string[] ids, bool includingSelf, DateTime timePoint)
		{
			ids.NullCheck("ids");

			Dictionary<string, SCSimpleObjectCollection> result = new Dictionary<string, SCSimpleObjectCollection>(ids.Length, StringComparer.OrdinalIgnoreCase);

			if (ids.Length > 0)
			{
				string sqlTemplate = ResourceHelper.LoadStringFromResource(Assembly.GetExecutingAssembly(),
					"MCS.Library.SOA.DataObjects.Security.Adapters.Templates.LoadAllParentInfo.sql");

				string timeString = TSqlBuilder.Instance.DBCurrentTimeFunction;

				if (timePoint != DateTime.MinValue)
					timeString = TSqlBuilder.Instance.FormatDateTime(timePoint);

				InSqlClauseBuilder inBuilder = new InSqlClauseBuilder("ID");

				inBuilder.AppendItem(ids);

				string sql = string.Format(sqlTemplate, timeString, inBuilder.ToSqlStringWithInOperator(TSqlBuilder.Instance));

				DataTable table = DbHelper.RunSqlReturnDS(sql, this.GetConnectionName()).Tables[0];
				DataView view = new DataView(table);

				view.Sort = "ID";

				foreach (string id in ids)
				{
					if (result.ContainsKey(id) == false)
					{
						SCSimpleObjectCollection tempParents = new SCSimpleObjectCollection();

						FillAllParents(id, view, includingSelf, tempParents);

						SCSimpleObjectCollection parents = new SCSimpleObjectCollection();

						//转换次序
						for (int i = tempParents.Count - 1; i >= 0; i--)
							parents.Add(tempParents[i]);

						result.Add(id, parents);
					}
				}
			}

			return result;
		}
		#endregion Parents Info

		#region 拼音
		/// <summary>
		/// 得到字符串的拼音
		/// </summary>
		/// <param name="data"></param>
		/// <returns></returns>
		public List<string> GetPinYin(string data)
		{
			data = data.Trim();
			StringBuilder select = new StringBuilder(64);
			StringBuilder from = new StringBuilder(64);
			WhereSqlClauseBuilder wBuilder = new WhereSqlClauseBuilder();
			OrderBySqlClauseBuilder orderByBuilder = new OrderBySqlClauseBuilder();

			for (int i = 0; i < data.Length; i++)
			{
				char ch = data[i];

				if (ch < ' ' || ch > 'z')
				{
					string strAlias = "P" + i.ToString();

					select.AppendWithSplitChars(strAlias + ".PinYin", ", ");
					from.AppendWithSplitChars("SC.PinYin " + strAlias, ", ");
					wBuilder.AppendItem(strAlias + ".HZ", ch.ToString());
					orderByBuilder.AppendItem(strAlias + ".Weight", FieldSortDirection.Ascending);
				}
			}

			List<string> result = new List<string>();

			if (select.Length > 0)
			{
				string sql = string.Format("SELECT {0} FROM {1} WHERE {2} ORDER BY {3}",
					select.ToString(), from.ToString(),
					wBuilder.ToSqlString(TSqlBuilder.Instance),
					orderByBuilder.ToSqlString(TSqlBuilder.Instance));

				DataTable table = DbHelper.RunSqlReturnDS(sql, this.GetConnectionName()).Tables[0];

				foreach (DataRow row in table.Rows)
					result.Add(DataRowToPinYin(data, row));
			}
			else
				result.Add(DataRowToPinYin(data, null));

			return result;
		}
		#endregion 拼音

		/// <summary>
		/// 获取连接的名称
		/// </summary>
		/// <returns>表示连接名称的字符串</returns>
		protected string GetConnectionName()
		{
			return SCConnectionDefine.DBConnectionName;
		}

		#region Private
		private static void FillAllParents(string id, DataView view, bool includingSelf, SCSimpleObjectCollection parents)
		{
			int index = view.Find(id);

			if (index >= 0)
			{
				DataRow row = view[index].Row;

				if (includingSelf)
					parents.Add(MapDataRowToSimpleObject(row));

				FillAllParentsRecursively(row["ParentID"].ToString(), view, parents);
			}
		}

		private static void FillAllParentsRecursively(string id, DataView view, SCSimpleObjectCollection parents)
		{
			int index = view.Find(id);

			if (index >= 0)
			{
				DataRow row = view[index].Row;

				parents.Add(MapDataRowToSimpleObject(row));

				FillAllParentsRecursively(row["ParentID"].ToString(), view, parents);
			}
		}

		private static SCSimpleObject MapDataRowToSimpleObject(DataRow row)
		{
			SCSimpleObject data = new SCSimpleObject();

			ORMapping.DataRowToObject(row, data);

			if (data.DisplayName.IsNullOrEmpty())
				data.DisplayName = data.Name;

			return data;
		}

		private static string DataRowToPinYin(string data, DataRow row)
		{
			int nIndex = 0;
			StringBuilder strB = new StringBuilder(16);

			for (int i = 0; i < data.Length; i++)
			{
				char ch = data[i];

				if ((ch < ' ' || ch > 'z') && row != null)
					strB.Append(row[nIndex++].ToString());
				else
					strB.Append(ch);
			}

			return strB.ToString();
		}

		/// <summary>
		/// 修饰一下FullPath，去掉Root的部分
		/// </summary>
		/// <param name="fullPaths">一个<see cref="string"/>的数组，每个字符串表示一个全路径</param>
		private static void MarkupFullPaths(string[] fullPaths)
		{
			for (int i = 0; i < fullPaths.Length; i++)
			{
				string path = fullPaths[i].Trim();

				string prefix = SCOrganization.RootOrganizationName + "\\";

				if (path.IndexOf(prefix) == 0)
					path = path.Substring(prefix.Length);

				fullPaths[i] = path;
			}
		}

		/// <summary>
		/// 根据TimePoint和状态来生成Builder
		/// </summary>
		/// <param name="includingDeleted"></param>
		/// <param name="timePoint"></param>
		/// <param name="tablePrefixes"></param>
		/// <returns></returns>
		private IConnectiveSqlClause CreateStatusAndTimePointBuilder(bool includingDeleted, DateTime timePoint, params string[] tablePrefixes)
		{
			ConnectiveSqlClauseCollection result = new ConnectiveSqlClauseCollection();

			foreach (string tablePrefix in tablePrefixes)
			{
				IConnectiveSqlClause timePointBuilder = VersionStrategyQuerySqlBuilder.Instance.TimePointToBuilder(timePoint, tablePrefix);

				result.Add(timePointBuilder);

				WhereSqlClauseBuilder statusBuilder = new WhereSqlClauseBuilder();

				if (includingDeleted == false)
					statusBuilder.AppendItem(tablePrefix + "Status", (int)SchemaObjectStatus.Normal);

				result.Add(statusBuilder);
			}

			return result;
		}
		#endregion
	}
}
