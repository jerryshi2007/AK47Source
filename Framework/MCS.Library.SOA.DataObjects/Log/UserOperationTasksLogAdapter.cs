using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using MCS.Library.Data.Builder;
using MCS.Library.Data;
using MCS.Library.Data.Mapping;

namespace MCS.Library.SOA.DataObjects
{
	public sealed class UserOperationTasksLogAdapter : UpdatableAndLoadableAdapterBase<UserOperationTasksLog, UserOperationTasksLogCollection>
	{
		public static readonly UserOperationTasksLogAdapter Instance = new UserOperationTasksLogAdapter();

		private UserOperationTasksLogAdapter()
		{
		}

		public int AddOperationTasksLogs(UserOperationTasksLogCollection userTaskslogs)
		{
			StringBuilder sqlStrB = new StringBuilder();

			ORMappingItemCollection mappings = ORMapping.GetMappingInfo<UserOperationTasksLog>();

			foreach (UserOperationTasksLog data in userTaskslogs)
			{
				sqlStrB.AppendFormat("{0} ; \n", ORMapping.GetInsertSql(data, mappings, TSqlBuilder.Instance));
			}

			return DbHelper.RunSql(sqlStrB.ToString());
		}

	    /*
		public DataTable GetTopOperationTasksLogs(WhereSqlClauseBuilder where, int count)
		{
			//内联，由于数据量太大。采用客户瑞处理。
			//string top5sql = string.Format("SELECT LOG_ID, SEND_TO_USER_NAME FROM  WF.USER_OPERATION_TASKS_LOG AS ULOG WHERE ((SELECT COUNT(*) FROM WF.USER_OPERATION_TASKS_LOG WHERE(ULOG.LOG_ID = LOG_ID)) < {1}) AND {0}", where.ToSqlString(TSqlBuilder.Instance), count++);
			DataSet ds = null;
			using (DbContext context = DbContext.GetContext(this.GetConnectionName()))
			{
				Database db = DatabaseFactory.Create(context);
				ds = db.ExecuteDataSet(CommandType.Text, top5sql, "resutlTable");
			}

			if (ds != null)
				return ds.Tables["resutlTable"];
			else
				return new DataTable();
		} */
	}
}
