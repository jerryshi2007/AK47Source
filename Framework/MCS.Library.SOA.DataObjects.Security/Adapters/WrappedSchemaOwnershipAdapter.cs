using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MCS.Library.Data.Builder;
using MCS.Library.SOA.DataObjects.Schemas.SchemaProperties;

namespace MCS.Library.SOA.DataObjects.Security.Adapters
{
	public sealed class WrappedSchemaOwnershipAdapter
	{
		WrappedSchemaOwnershipAdapter()
		{
		}

		/// <summary>
		/// <see cref="WrappedSchemaOwnershipAdapter"/>的实例，此字段为只读
		/// </summary>
		public static readonly WrappedSchemaOwnershipAdapter Instance = new WrappedSchemaOwnershipAdapter();

		/// <summary>
		/// 获取一个或多个终端节点的父机构
		/// </summary>
		/// <param name="terminalIds">终结点的ID</param>
		/// <returns>键值对的枚举器，表示对象ID，和容器ID的关系。</returns>
		public IEnumerable<System.Collections.Generic.KeyValuePair<string, string>> GetOwnerships(string[] terminalIds)
		{
			return this.GetOwnerships(DateTime.MinValue, terminalIds);
		}

		/// <summary>
		/// 获取一个或多个终端节点的父机构
		/// </summary>
		/// <param name="timePoint">时间点</param>
		/// <param name="terminalIds">终结点ID的数组</param>
		/// <returns>表示对象ID，和容器ID的关系。</returns>
		public IEnumerable<System.Collections.Generic.KeyValuePair<string, string>> GetOwnerships(DateTime timePoint, string[] terminalIds)
		{
			string sql = @"SELECT R.ObjectID,R.ParentID FROM 
SC.SchemaRelationObjectsSnapshot R
WHERE ";
			InSqlClauseBuilder inBuilder = new InSqlClauseBuilder("R.ObjectID");
			inBuilder.AppendItem(terminalIds);

			WhereSqlClauseBuilder whereBuilder = new WhereSqlClauseBuilder();

			var timeCondition = VersionStrategyQuerySqlBuilder.Instance.TimePointToBuilder(timePoint, "R.");

			whereBuilder.AppendItem("R.Status", (int)SchemaObjectStatus.Normal);
			whereBuilder.AppendItem("R.ParentSchemaType", "Organizations");

			ConnectiveSqlClauseCollection connectiveBuilder = new ConnectiveSqlClauseCollection(inBuilder, whereBuilder, timeCondition);

			sql += connectiveBuilder.ToSqlString(TSqlBuilder.Instance) + " ORDER BY R.ObjectID ASC";
			var ds = DbHelper.RunSqlReturnDS(sql, this.GetConnectionName());
			foreach (System.Data.DataRow row in ds.Tables[0].Rows)
			{
				yield return new System.Collections.Generic.KeyValuePair<string, string>(row["ObjectID"] as string, row["ParentID"] as string);
			}
		}

		/// <summary>
		/// 获取组织和父组织ID
		/// </summary>
		/// <param name="orgIds">组织的ID</param>
		/// <returns></returns>
		public IEnumerable<SCWrappedSchemaOwnership> GetDirectParents(params string[] orgIds)
		{
			return this.GetDirectParents(DateTime.MinValue, orgIds);
		}

		/// <summary>
		/// 获取组织和父组织ID
		/// </summary>
		/// <param name="timePoint">时间点 或 <see cref="DateTime.MinValue"/>表示取当前时间</param>
		/// <param name="orgIds">组织的ID</param>
		/// <returns></returns>
		public IEnumerable<SCWrappedSchemaOwnership> GetDirectParents(DateTime timePoint, params string[] orgIds)
		{
			string sql = @"SELECT O.*,R.ParentID AS QCHID FROM 
SC.SchemaOrganizationSnapshot O
INNER JOIN SC.SchemaRelationObjectsSnapshot R
ON O.ID = R.ObjectID
WHERE ";
			InSqlClauseBuilder inBuilder = new InSqlClauseBuilder("R.ObjectID");
			inBuilder.AppendItem(orgIds);

			WhereSqlClauseBuilder whereBuilder = new WhereSqlClauseBuilder();

			var timeCondition = VersionStrategyQuerySqlBuilder.Instance.TimePointToBuilder(timePoint, "O.");

			var timeCondition2 = VersionStrategyQuerySqlBuilder.Instance.TimePointToBuilder(timePoint, "R.");
			whereBuilder.AppendItem("O.Status", (int)SchemaObjectStatus.Normal);
			whereBuilder.AppendItem("R.Status", (int)SchemaObjectStatus.Normal);
			whereBuilder.AppendItem("R.ParentSchemaType", "Organizations");

			ConnectiveSqlClauseCollection connectiveBuilder = new ConnectiveSqlClauseCollection(inBuilder, whereBuilder, timeCondition, timeCondition2);

			sql += connectiveBuilder.ToSqlString(TSqlBuilder.Instance) + " ORDER BY R.ParentID ASC";
			var ds = DbHelper.RunSqlReturnDS(sql, this.GetConnectionName());
			SCSimpleObject lastObj = null;
			foreach (System.Data.DataRow row in ds.Tables[0].Rows)
			{
				SCSimpleObject obj;
				if (lastObj != null && lastObj.ID == (string)row["ID"])
				{
					obj = lastObj;
				}
				else
				{
					obj = new SCSimpleObject();
					MCS.Library.Data.Mapping.ORMapping.DataRowToObject(row, obj);
					lastObj = obj;
				}

				yield return new SCWrappedSchemaOwnership(obj, (string)row["QCHID"]);
			}
		}

		private string GetConnectionName()
		{
			return SCConnectionDefine.DBConnectionName;
		}
	}
}
