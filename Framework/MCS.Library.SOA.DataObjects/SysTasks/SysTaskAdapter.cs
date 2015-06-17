using System;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Transactions;
using MCS.Library.Core;
using MCS.Library.Data;
using MCS.Library.Data.Builder;
using MCS.Library.Data.DataObjects;
using MCS.Library.Data.Mapping;
using MCS.Library.OGUPermission;
using MCS.Library.SOA.DataObjects.Workflow;

namespace MCS.Library.SOA.DataObjects
{
    /// <summary>
    /// 系统任务的后台适配器
    /// </summary>
    public class SysTaskAdapter : SysTaskAdapterBase<SysTask, SysTaskCollection>
    {
        public static readonly SysTaskAdapter Instance = new SysTaskAdapter();

        protected SysTaskAdapter()
        {
        }

        /// <summary>
        /// 移动到已经完成系统任务中
        /// </summary>
        /// <param name="taskID">被移动的任务的ID</param>
        /// <param name="status">重置任务的状态</param>
        public SysAccomplishedTask MoveToCompletedSysTask(string taskID, SysTaskStatus status, string statusText)
        {
            SysTask task = this.Load(taskID);

            (task != null).FalseThrow<ArgumentException>("ID为 {0} 的任务不存在", taskID);

            return MoveToCompletedSysTask(task, status, statusText);
        }

        /// <summary>
        /// 移动到已经完成系统任务中
        /// </summary>
        /// <param name="taskID">被移动的任务的ID</param>
        /// <param name="status">重置任务的状态</param>
        public SysAccomplishedTask MoveToCompletedSysTask(SysTask task, SysTaskStatus status, string statusText)
        {
            SysAccomplishedTask result = null;

            PerformanceMonitorHelper.GetDefaultMonitor().WriteExecutionDuration(string.Format("MoveToAccomplished({0})", this.GetType().FullName), () =>
            {
                Dictionary<string, object> context = new Dictionary<string, object>();
                ORMappingItemCollection mappings = GetMappingInfo(context);

                WhereSqlClauseBuilder builder = ORMapping.GetWhereSqlClauseBuilderByPrimaryKey(task, mappings);

                ORMappingItemCollection mappingsCompleted = ORMapping.GetMappingInfo<SysAccomplishedTask>();

                SysAccomplishedTask taskCompleted = new SysAccomplishedTask(task);

                taskCompleted.Status = status;
                taskCompleted.StatusText = statusText;

                StringBuilder sql = new StringBuilder();

                sql.AppendFormat("DELETE FROM {0} WHERE {1}", mappingsCompleted.TableName, builder.ToSqlString(TSqlBuilder.Instance));

                sql.Append(TSqlBuilder.Instance.DBStatementSeperator);

                sql.Append(GetMoveSysTaskSql(taskCompleted, context));

                sql.Append(TSqlBuilder.Instance.DBStatementSeperator);

                sql.Append("IF @@ROWCOUNT = 0");
                sql.Append(TSqlBuilder.Instance.DBStatementSeperator);
                sql.Append("\t" + ORMapping.GetInsertSql(taskCompleted, mappingsCompleted, TSqlBuilder.Instance));

                sql.Append(TSqlBuilder.Instance.DBStatementSeperator);

                sql.AppendFormat("DELETE FROM {0} WHERE {1}", mappings.TableName, builder.ToSqlString(TSqlBuilder.Instance));

                using (TransactionScope scope = TransactionScopeFactory.Create())
                {
                    DbHelper.RunSql(sql.ToString(), this.GetConnectionName());

                    scope.Complete();
                }

                result = taskCompleted;
            });

            return result;
        }

        private string GetMoveSysTaskSql(SysAccomplishedTask taskCompleted, Dictionary<string, object> context)
        {
            ORMappingItemCollection mappings = GetMappingInfo(context);
            ORMappingItemCollection mappingsCompleted = ORMapping.GetMappingInfo<SysAccomplishedTask>();

            InsertSqlClauseBuilder insertBuilder = ORMapping.GetInsertSqlClauseBuilder(taskCompleted, "EndTime", "Status", "StatusText");

            string[] fields = insertBuilder.GetAllDataFields();

            string affectedFields = string.Join(",", fields);

            string sql = string.Format("INSERT INTO {0}({1}, END_TIME, STATUS, STATUS_TEXT) SELECT {2}, GETDATE() AS END_TIME, {3} AS STATUS, {4} AS STATUS_TEXT FROM {5} WHERE TASK_GUID = {6}",
                mappingsCompleted.TableName,
                affectedFields,
                affectedFields,
                TSqlBuilder.Instance.CheckUnicodeQuotationMark(taskCompleted.Status.ToString()),
                TSqlBuilder.Instance.CheckUnicodeQuotationMark(taskCompleted.StatusText),
                mappings.TableName,
                TSqlBuilder.Instance.CheckUnicodeQuotationMark(taskCompleted.TaskID));

            return sql;
        }

        /// <summary>
        /// 获取未运行的系统任务（调用WF.SYS_NOT_RUNNING_TASK）。注意事务
        /// </summary>
        /// <param name="batchCount">取多少个，小于0表示全部</param>
        /// <param name="action">读取完成后，迭代每一个Task执行操作</param>
        /// <returns></returns>
        public SysTaskCollection FetchNotRuningSysTasks(int batchCount, Action<SysTask> action)
        {
            using (TransactionScope scope = TransactionScopeFactory.Create())
            {
                SysTaskCollection result = InnerQueryBatch(batchCount);

                if (action != null)
                    result.ForEach(t => action(t));

                scope.Complete();

                return result;
            }
        }

        protected override void BeforeInnerUpdate(SysTask data, Dictionary<string, object> context)
        {
            base.BeforeInnerUpdate(data, context);

            if (data.Data.IsNullOrEmpty())
                data.FillData(null);
        }

        protected override void AfterLoad(SysTaskCollection data)
        {
            data.ForEach(t => t.AfterLoad());
        }

        private SysTaskCollection InnerQueryBatch(int batchCount)
        {
            string batchClause = batchCount >= 0 ? "TOP " + batchCount : string.Empty;

            WhereSqlClauseBuilder builder = new WhereSqlClauseBuilder();

            builder.AppendItem("1", 1);
            builder.AppendTenantCode();

            string sql = string.Format("SELECT {0} * FROM WF.SYS_NOT_RUNNING_TASK WITH(UPDLOCK READPAST) WHERE {1} ORDER BY SORT_ID ASC",
                batchClause, builder.ToSqlString(TSqlBuilder.Instance));

            return this.QueryData(sql);
        }
    }
}
