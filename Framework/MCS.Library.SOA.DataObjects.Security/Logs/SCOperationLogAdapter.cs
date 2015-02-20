using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MCS.Library.Core;
using MCS.Library.Data.Builder;
using MCS.Library.Data.Mapping;
using MCS.Library.SOA.DataObjects.Schemas.Actions;
using MCS.Library.SOA.DataObjects.Security.Actions;
using MCS.Library.SOA.DataObjects.Security.Adapters;
using MCS.Library.SOA.DataObjects.Schemas.Adapters;
using MCS.Library.Data;

namespace MCS.Library.SOA.DataObjects.Security.Logs
{
	public class SCOperationLogAdapter
	{
		/// <summary>
		/// 表示<see cref="SCOperationLogAdapter"/>的实例，此字段为只读
		/// </summary>
		public static readonly SCOperationLogAdapter Instance = new SCOperationLogAdapter();

		private SCOperationLogAdapter()
		{
		}

		public SCOperationLogCollection LoadByResourceID(string resourceID)
		{
			resourceID.CheckStringIsNullOrEmpty("resourceID");

			WhereSqlClauseBuilder builder = new WhereSqlClauseBuilder();

			builder.AppendItem("ResourceID", resourceID);

			return Load(builder);
		}

		public SCOperationLog Load(int id)
		{
			InSqlClauseBuilder builder = new InSqlClauseBuilder("ID");

			builder.AppendItem(id);

			return Load(builder).FirstOrDefault();
		}

		public SCOperationLogCollection Load(IConnectiveSqlClause sqlClause)
		{
			SCOperationLogCollection result = null;

			VersionedObjectAdapterHelper.Instance.FillData(GetMappingInfo().TableName, sqlClause, this.GetConnectionName(),
				(view) =>
				{
					result = new SCOperationLogCollection();

					ORMapping.DataViewToCollection(result, view);
				});

			return result;
		}

		/// <summary>
		/// 注意，这个仅供首页显示使用。属性不全
		/// </summary>
		/// <param name="count"></param>
		/// <returns></returns>
		public SCOperationLogCollection LoadRecentSummaryLog(int count)
		{

			string sql = "SELECT TOP " + count + " Subject,CreateTime FROM SC.OperationLog ORDER BY ID DESC";

			SCOperationLogCollection result = new SCOperationLogCollection();

			using (DbContext context = DbContext.GetContext(this.GetConnectionName()))
			{
				Database db = DatabaseFactory.Create(context);

				using (var dr = db.ExecuteReader(System.Data.CommandType.Text, sql))
				{
					while (dr.Read())
					{
						result.Add(new SCOperationLog() { Subject = dr["Subject"].ToString(), CreateTime = (DateTime)dr["CreateTime"] });
					}
				}
			}

			return result;
		}

		public void Insert(SCOperationLog log)
		{
			if (log != null)
			{
				log.CreateTime = SCActionContext.Current.TimePoint;
				StringBuilder strB = new StringBuilder(256);

				strB.Append(ORMapping.GetInsertSql(log, this.GetMappingInfo(), TSqlBuilder.Instance));
				strB.Append(TSqlBuilder.Instance.DBStatementSeperator);
				strB.Append("SELECT SCOPE_IDENTITY()");

				Decimal newID = (Decimal)DbHelper.RunSqlReturnScalar(strB.ToString(), this.GetConnectionName());

				log.ID = Decimal.ToInt32(newID);
			}
		}

		protected virtual ORMappingItemCollection GetMappingInfo()
		{
			return ORMapping.GetMappingInfo<SCOperationLog>();
		}

		protected virtual string GetConnectionName()
		{
			return SCConnectionDefine.DBConnectionName;
		}
	}
}
