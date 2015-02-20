using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MCS.Library.Data.Builder;
using MCS.Library.Core;
using MCS.Library.SOA.DataObjects.Security.Adapters;
using MCS.Library.SOA.DataObjects.Security.Actions;
using MCS.Library.Data.Mapping;
using MCS.Library.SOA.DataObjects.Security.AUObjects.Actions;
using MCS.Library.SOA.DataObjects.Schemas.Actions;
using MCS.Library.SOA.DataObjects.Schemas.Adapters;
using MCS.Library.Data;

namespace MCS.Library.SOA.DataObjects.Security.AUObjects.Logs
{
	public class AUOperationLogAdapter
	{
		/// <summary>
		/// 表示<see cref="AUOperationLogAdapter"/>的实例，此字段为只读
		/// </summary>
		public static readonly AUOperationLogAdapter Instance = new AUOperationLogAdapter();

		private AUOperationLogAdapter()
		{
		}

		public AUOperationLogCollection LoadByResourceID(string resourceID)
		{
			resourceID.CheckStringIsNullOrEmpty("resourceID");

			WhereSqlClauseBuilder builder = new WhereSqlClauseBuilder();

			builder.AppendItem("ResourceID", resourceID);

			return Load(builder);
		}

		public AUOperationLog Load(int id)
		{
			InSqlClauseBuilder builder = new InSqlClauseBuilder("ID");

			builder.AppendItem(id);

			return Load(builder).FirstOrDefault();
		}

		public AUOperationLogCollection Load(IConnectiveSqlClause sqlClause)
		{
			AUOperationLogCollection result = null;

			AUCommon.DoDbAction(() =>
			{
				VersionedObjectAdapterHelper.Instance.FillData(GetMappingInfo().TableName, sqlClause, this.GetConnectionName(),
					(view) =>
					{
						result = new AUOperationLogCollection();

						ORMapping.DataViewToCollection(result, view);
					});
			});

			return result;
		}

		/// <summary>
		/// 插入一个新的<see cref="AUOperationLog"/>到数据库中，操作完毕通过其ID属性获取新插入的ID。
		/// </summary>
		/// <param name="log"></param>
		public void Insert(AUOperationLog log)
		{
			log.NullCheck("log");

			log.CreateTime = SCActionContext.Current.TimePoint;
			StringBuilder strB = new StringBuilder(256);

			strB.Append(ORMapping.GetInsertSql(log, this.GetMappingInfo(), TSqlBuilder.Instance));
			strB.Append(TSqlBuilder.Instance.DBStatementSeperator);
			strB.Append("SELECT SCOPE_IDENTITY()");

			AUCommon.DoDbAction(() =>
			{
				Decimal newID = (Decimal)DbHelper.RunSqlReturnScalar(strB.ToString(), this.GetConnectionName());

				log.ID = Decimal.ToInt32(newID);
			});
		}

		/// <summary>
		/// 注意，这个仅供首页显示使用。属性不全
		/// </summary>
		/// <param name="count"></param>
		/// <returns></returns>
		public AUOperationLogCollection LoadRecentSummaryLog(int count)
		{

			string sql = "SELECT TOP " + count + " Subject,CreateTime FROM SC.OperationLog ORDER BY ID DESC";

			AUOperationLogCollection result = new AUOperationLogCollection();

			using (DbContext context = DbContext.GetContext(this.GetConnectionName()))
			{
				Database db = DatabaseFactory.Create(context);

				using (var dr = db.ExecuteReader(System.Data.CommandType.Text, sql))
				{
					while (dr.Read())
					{
						result.Add(new AUOperationLog() { Subject = dr["Subject"].ToString(), CreateTime = (DateTime)dr["CreateTime"] });
					}
				}
			}

			return result;
		}

		protected virtual ORMappingItemCollection GetMappingInfo()
		{
			return ORMapping.GetMappingInfo<AUOperationLog>();
		}

		protected virtual string GetConnectionName()
		{
			return AUCommon.DBConnectionName;
		}
	}
}
