using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using CIIC.HSR.TSP.DataAccess;
using System.Data.SqlClient;
using CIIC.HSR.TSP.WF.Persistence.Contract;
using CIIC.HSR.TSP.WF.Bizlet.Common;
using CIIC.HSR.TSP.WF.BizObject;
using CIIC.HSR.TSP.Utility;

namespace CIIC.HSR.TSP.WF.Persistence.Impl
{
    public partial class USER_TASKRepository
    {
        private static readonly string SqlQueryTaskUnProcessed = @"SELECT U.TASK_GUID,U.APPLICATION_NAME,U.PROGRAM_NAME,U.TASK_LEVEL,U.TASK_TITLE,U.RESOURCE_ID,U.PROCESS_ID
          ,U.ACTIVITY_ID ,U.URL,U.DATA,U.EMERGENCY,U.PURPOSE,U.STATUS,U.TASK_START_TIME,U.EXPIRE_TIME,U.SOURCE_ID
          ,U.SOURCE_NAME,U.SEND_TO_USER,U.SEND_TO_USER_NAME ,U.READ_TIME,U.CATEGORY_GUID ,U.TOP_FLAG,U.DRAFT_DEPARTMENT_NAME
          ,U.DELIVER_TIME,U.DRAFT_USER_ID,U.DRAFT_USER_NAME ,U.TenantCode ,U.TaskType,U.DepartmentCode ,U.DepartmentName,P.ProcessKey PROCESS_KEY
		  ,P.Status PROCESS_STATUS,P.ProcessName PROCESS_NAME,P.Created
        FROM TSPWF_USER_TASK U
		JOIN  TSPWF_Process P
		ON U.PROCESS_ID=P.ProcessId
        WHERE U.SEND_TO_USER=@USERID
        AND   U.TenantCode=@TenantCode ";

        private static readonly string SqlQueryTaskProcessed = @"SELECT U.TASK_GUID,U.APPLICATION_NAME,U.PROGRAM_NAME,U.TASK_LEVEL,U.TASK_TITLE,U.RESOURCE_ID,U.PROCESS_ID
          ,U.ACTIVITY_ID ,U.URL,U.DATA,U.EMERGENCY,U.PURPOSE,U.STATUS,U.TASK_START_TIME,U.EXPIRE_TIME,U.SOURCE_ID
          ,U.SOURCE_NAME,U.SEND_TO_USER,U.SEND_TO_USER_NAME ,U.READ_TIME,U.CATEGORY_GUID ,U.TOP_FLAG,U.DRAFT_DEPARTMENT_NAME
          ,U.DELIVER_TIME,U.DRAFT_USER_ID,U.DRAFT_USER_NAME ,U.TenantCode ,U.TaskType,U.DepartmentCode ,U.DepartmentName
          ,P.ProcessKey PROCESS_KEY,P.Status PROCESS_STATUS,P.ProcessName PROCESS_NAME,P.Created
        FROM TSPWF_USER_ACCOMPLISHED_TASK U 
		JOIN TSPWF_Process P
		ON U.PROCESS_ID=P.ProcessId
        WHERE U.SEND_TO_USER=@USERID
        AND   U.TenantCode=@TenantCode";

        private static readonly string SqlSetUserTasksAccomplished = @"INSERT INTO TSPWF_USER_ACCOMPLISHED_TASK
         (
			 TASK_GUID,APPLICATION_NAME,PROGRAM_NAME,TASK_LEVEL,TASK_TITLE,RESOURCE_ID,PROCESS_ID,ACTIVITY_ID,URL,DATA,EMERGENCY
			  ,PURPOSE,STATUS,TASK_START_TIME,EXPIRE_TIME,SOURCE_ID,SOURCE_NAME,SEND_TO_USER,SEND_TO_USER_NAME,READ_TIME,CATEGORY_GUID
			  ,TOP_FLAG,DRAFT_DEPARTMENT_NAME,DELIVER_TIME,DRAFT_USER_ID,DRAFT_USER_NAME,TenantCode,TaskType,DepartmentCode,DepartmentName
          )
        SELECT 
			TASK_GUID,APPLICATION_NAME,PROGRAM_NAME,TASK_LEVEL,TASK_TITLE,RESOURCE_ID,PROCESS_ID,ACTIVITY_ID,URL,DATA,EMERGENCY
		    ,PURPOSE,STATUS,TASK_START_TIME,EXPIRE_TIME,SOURCE_ID,SOURCE_NAME,SEND_TO_USER,SEND_TO_USER_NAME,READ_TIME,CATEGORY_GUID
			,TOP_FLAG,DRAFT_DEPARTMENT_NAME,DELIVER_TIME,DRAFT_USER_ID,DRAFT_USER_NAME,TenantCode,@TaskType,DepartmentCode,DepartmentName
         FROM  TSPWF_USER_TASK
         WHERE TASK_GUID IN ";

        private static readonly string SqlDeleteUserAccomplishedTasks = @"DELETE  TSPWF_USER_ACCOMPLISHED_TASK WHERE TASK_GUID  IN ";

        private static readonly string SqlDeleteUserTasks = @"DELETE TSPWF_USER_TASK WHERE TASK_GUID  IN ";

        private static readonly string SqlDeleteSyncProcess = @"DELETE  TSPWF_Process WHERE ProcessId  IN ";

        private static readonly string SqlInsertSyncProcess = @" INSERT INTO  TSPWF_Process
          VALUES(@ProcessId,@ProcessKey,@ProcessName ,@Status,@CreatorId,@CreatorName,@Created,@TenantCode)";

        private static readonly string SqlQueryTaskUnProcessedTopCnt = @"
        DECLARE @TaskUnProcessedCnt int
        SELECT @TaskUnProcessedCnt=COUNT(1)
                FROM TSPWF_USER_TASK U
		        JOIN  TSPWF_Process P
		        ON U.PROCESS_ID=P.ProcessId
                WHERE U.SEND_TO_USER=@USERID
                AND   U.TenantCode=@TenantCode  ";

        private  string SqlQueryTaskUnProcessedTop = @"  U.TASK_TITLE,U.URL,P.Created,@TaskUnProcessedCnt  TotalCnt
        FROM TSPWF_USER_TASK U
		JOIN  TSPWF_Process P
		ON U.PROCESS_ID=P.ProcessId
        WHERE U.SEND_TO_USER=@USERID
        AND   U.TenantCode=@TenantCode ";


        private static readonly string SqlDeleteUserAccomplishedTasksByProcessId = @"DELETE  TSPWF_USER_ACCOMPLISHED_TASK WHERE PROCESS_ID=@PROCESS_ID  AND TenantCode=@TenantCode AND SEND_TO_USER=@SEND_TO_USER ";

        private static readonly string SqlDeleteUserAccomplishedTasksByResourseId = @"DELETE  TSPWF_USER_ACCOMPLISHED_TASK WHERE RESOURCE_ID=@RESOURCE_ID  AND TenantCode=@TenantCode AND SEND_TO_USER=@SEND_TO_USER ";

        /// <summary>
        /// 查询代办
        /// </summary>
        /// <param name="userId">用户Id</param>
        /// <param name="status">任务状态条件</param>
        /// <param name="tenantCode">租户编码</param>
        /// <returns>代办</returns>
        public PagedCollection<BizObject.USER_TASKBO_PROCESS> QueryTask(string tenantCode, string sendToUserId, UserTaskQueryCondition condition, int pageIndex, int pageSize, int? totalCount = default(int?))
        {
            string strQuerySql;
            List<SqlParameter> parms;
            StringBuilder queryBuilder = new StringBuilder();
            if ((int)condition.TaskType == (int)TaskStatus.Unprocessed)
            {
                queryBuilder = GetUserTaskSqlWhere(tenantCode, sendToUserId, condition, out parms, SqlQueryTaskUnProcessed);
                strQuerySql = queryBuilder.ToString();
            }
            else if ((int)condition.TaskType == (int)TaskStatus.Processed)
            {
                queryBuilder = GetUserTaskSqlWhere(tenantCode, sendToUserId, condition, out parms, SqlQueryTaskProcessed);
                strQuerySql = queryBuilder.ToString();
            }
            else
            {
                StringBuilder queryBuilderProcessed = GetUserTaskSqlWhere(tenantCode, sendToUserId, condition, out parms, SqlQueryTaskProcessed);
                StringBuilder queryBuilderUnProcessed = GetUserTaskSqlWhere(tenantCode, sendToUserId, condition, out parms, SqlQueryTaskUnProcessed);
                strQuerySql = queryBuilderProcessed.ToString() + " UNION ALL  " + queryBuilderUnProcessed.ToString();
            }

            List<SortDirection> sorts = new List<SortDirection>();
            sorts.Add(new SortDirection("U.TASK_START_TIME", Direction.Descending));
            PagedCollection<BizObject.USER_TASKBO_PROCESS> result = base.UnitOfWork.SqlQueryPaged<BizObject.USER_TASKBO_PROCESS>(strQuerySql, parms.ToArray(), sorts, pageIndex, pageSize, totalCount);

            return result;
        
        }

        /// <summary>
        ///  查询代办
        /// </summary>
        /// <param name="tenantCode">租户编码</param>
        /// <param name="sendToUserId">Id</param>
        /// <param name="condition">查询条件</param>
        /// <param name="topIndex">件数</param>
        /// <returns>待办信息</returns>
        public List<USER_TASKBO_TOPUNPROCESS> QueryTaskUnProcessedTop(string tenantCode, string sendToUserId, UserTaskQueryCondition condition, int topIndex)
        {
            string strQuerySqlCnt;
            string strQuerySqlTop;
            List<SqlParameter> parms;
            StringBuilder queryBuilderCnt = new StringBuilder();
            StringBuilder queryBuilderTop = new StringBuilder();
            if ((int)condition.TaskType == (int)TaskStatus.Unprocessed)
            {

                queryBuilderCnt = GetUserTaskSqlWhere(tenantCode, sendToUserId, condition, out parms, SqlQueryTaskUnProcessedTopCnt);
                strQuerySqlCnt = queryBuilderCnt.ToString();

                SqlQueryTaskUnProcessedTop = "   Select Top  " + topIndex + SqlQueryTaskUnProcessedTop;
                queryBuilderTop = GetUserTaskSqlWhere(tenantCode, sendToUserId, condition, out parms, SqlQueryTaskUnProcessedTop);
                strQuerySqlTop = queryBuilderTop.ToString();

                string strQuerySql = strQuerySqlCnt + strQuerySqlTop + " Order By P.Created Desc ";

                List<BizObject.USER_TASKBO_TOPUNPROCESS> result = base.UnitOfWork.SqlQuery<BizObject.USER_TASKBO_TOPUNPROCESS>(strQuerySql, parms.ToArray()).ToList();

                return result;
            }

            return null;
        
        }

        private static StringBuilder GetUserTaskSqlWhere(string tenantCode, string sendToUserId, UserTaskQueryCondition condition, out List<SqlParameter> parms, string strSql)
        {
             parms = new List<SqlParameter> { 
                new SqlParameter("@USERID", sendToUserId),
                new SqlParameter("@TenantCode", tenantCode)
            };

             StringBuilder sb = new StringBuilder(strSql);
            // 分类
            if (!string.IsNullOrEmpty(condition.ApplicationName))
            {
                sb.Append(" AND U.APPLICATION_NAME LIKE @APPLICATION_NAME");
                parms.Add(new SqlParameter("@APPLICATION_NAME", "%" + SecurityHelper.EscapeLikeString(condition.ApplicationName) + "%"));

            }
            // 分类
            if (!string.IsNullOrEmpty(condition.ProgramName))
            {
                sb.Append(" AND U.PROGRAM_NAME LIKE @PROGRAM_NAME");
                parms.Add(new SqlParameter("@PROGRAM_NAME", "%" + SecurityHelper.EscapeLikeString(condition.ProgramName) + "%"));
            }

            // 标题
            if (!string.IsNullOrEmpty(condition.TaskTitle))
            {
                sb.Append(" AND U.TASK_TITLE LIKE @TASK_TITLE");
                parms.Add(new SqlParameter("@TASK_TITLE", "%" + SecurityHelper.EscapeLikeString(condition.TaskTitle) + "%"));
            }
            // 部门
            if (!string.IsNullOrEmpty(condition.DepartmentCode))
            {
                sb.Append(" AND U.DepartmentCode=@DepartmentCode ");
                parms.Add(new SqlParameter("@DepartmentCode", condition.DepartmentCode));
            }
            // 提交时间From
            if (condition.CreatedFrom != null)
            {
                sb.Append(" AND P.Created>=@CREATE_FROM ");
                parms.Add(new SqlParameter("@CREATE_FROM", condition.CreatedFrom));
            }
            // 提交时间TO
            if (condition.CreatedTo != null)
            {
                sb.Append(" AND P.Created<=@CREATE_TO ");
                parms.Add(new SqlParameter("@CREATE_TO", condition.CreatedTo));
            }
            //创建人
            if (!string.IsNullOrEmpty(condition.CreatorUserId))
            {
                sb.Append(" AND P.CreatorId=@CreatorId ");
                parms.Add(new SqlParameter("@CreatorId", condition.CreatorUserId));
            }

            //流程状态
            if (condition.ProcessStatus != null)
            {
                if ((int)condition.ProcessStatus == (int)WfProcessStatus.NotRunning)
                {
                    sb.Append(" AND P.Status=@ProcessStatus ");
                    parms.Add(new SqlParameter("@ProcessStatus", WfProcessStatusDetail.NotRunning.ToString()));
                }
                else if ((int)condition.ProcessStatus == (int)WfProcessStatus.Running)
                {
                    sb.Append(" AND P.Status IN( '" + WfProcessStatusDetail.Running.ToString() + "','"
                                                     + WfProcessStatusDetail.Paused.ToString() + "','"
                                                     + WfProcessStatusDetail.Maintaining.ToString() + "')");

                }
                else if ((int)condition.ProcessStatus == (int)WfProcessStatus.Completed)
                {
                    sb.Append(" AND P.Status IN( '" + WfProcessStatusDetail.Completed.ToString() + "','"
                                                     + WfProcessStatusDetail.Aborted.ToString() + "')");

                }

                sb.Append("  AND U.STATUS='1'  ");

            }

            return sb;
        }

        /// <summary>
        /// 将任务由代办转为已办
        /// </summary>
        /// <param name="tasks">任务</param>
        /// <param name="data">额外数据</param>
        public void SetUserTasksAccomplished(List<CIIC.HSR.TSP.WF.BizObject.USER_TASKBO> tasks, Dictionary<string, string> data,string tenantCode)
        {
            using (var trans = this.UnitOfWork.BeginTransaction())
            {
                SqlParameter[] parms = new SqlParameter[] { 
                            new SqlParameter("@TaskType", TaskStatus.Processed)
                        };

             
                string strTaskGuids = ConverToInParameter(tasks);

                //删除已办
                foreach (var item in tasks)
                {
                    List<SqlParameter> parmsDel = new List<SqlParameter>();
                    parmsDel.Add(new SqlParameter("@TenantCode", tenantCode));
                    parmsDel.Add(new SqlParameter("@SEND_TO_USER", item.SEND_TO_USER));

                    if (!string.IsNullOrEmpty(item.PROCESS_ID))
                    {
                        parmsDel.Add(new SqlParameter("@PROCESS_ID", item.PROCESS_ID));
                        this.UnitOfWork.ExecuteSqlCommand(SqlDeleteUserAccomplishedTasksByProcessId, parmsDel.ToArray());

                    }
                    else if (!string.IsNullOrEmpty(item.RESOURCE_ID))
                    {
                        parmsDel.Add(new SqlParameter("@RESOURCE_ID", item.RESOURCE_ID));
                        this.UnitOfWork.ExecuteSqlCommand(SqlDeleteUserAccomplishedTasksByResourseId, parmsDel.ToArray());
                    }

                }

                this.UnitOfWork.ExecuteSqlCommand(SqlSetUserTasksAccomplished + strTaskGuids, parms);
                this.UnitOfWork.ExecuteSqlCommand(SqlDeleteUserTasks + strTaskGuids);
                trans.Commit();
            }
        }

        /// <summary>
        /// 删除代办
        /// </summary>
        /// <param name="tasks">任务</param>
        /// <param name="data">流程参数</param>
        public void DeleteUserAccomplishedTasks(List<CIIC.HSR.TSP.WF.BizObject.USER_TASKBO> tasks, Dictionary<string, string> data)
        {
            using (var trans = this.UnitOfWork.BeginTransaction())
            {
                string strTaskGuids = ConverToInParameter(tasks);
                this.UnitOfWork.ExecuteSqlCommand(SqlDeleteUserAccomplishedTasks + strTaskGuids);
                trans.Commit();
            }
        }

        /// <summary>
        /// 删除已办
        /// </summary>
        /// <param name="tasks">任务</param>
        /// <param name="data">流程参数</param>
        public void DeleteUserTasks(List<CIIC.HSR.TSP.WF.BizObject.USER_TASKBO> tasks, Dictionary<string, string> context)
        {
            using (var trans = this.UnitOfWork.BeginTransaction())
            {
                string strTaskGuids = ConverToInParameter(tasks);
                this.UnitOfWork.ExecuteSqlCommand(SqlDeleteUserTasks + strTaskGuids);
                trans.Commit();
            }
        }

        /// <summary>
        /// 同步流程状态
        /// </summary>
        /// <param name="json">被序列化的流程数据</param>
        public void SyncProcess(List<ProcessBO> process)
        {
            using (var trans = this.UnitOfWork.BeginTransaction())
            {
                string strRetIds = "";
                foreach (var processItem in process)
                {
                    strRetIds = strRetIds + "'" + processItem.ProcessId + "',";
                }
                strRetIds = "( " + strRetIds.Substring(0, strRetIds.Length - 1) + " )";

                this.UnitOfWork.ExecuteSqlCommand(SqlDeleteSyncProcess + strRetIds);

                foreach (var processItem in process)
                {
                    var createdDateTime = Convert.ToDateTime(processItem.Created).ToLocalTime();
     
                   SqlParameter[] parms = new SqlParameter[] { 
                            new SqlParameter("@ProcessId", processItem.ProcessId),
                            new SqlParameter("@ProcessKey", processItem.ProcessKey),
                            new SqlParameter("@ProcessName", processItem.ProcessName),
                            new SqlParameter("@Status", processItem.Status),
                            new SqlParameter("@CreatorId", processItem.CreatorId),
                            new SqlParameter("@CreatorName", processItem.CreatorName),
                            new SqlParameter("@Created", createdDateTime),
                            new SqlParameter("@TenantCode",processItem.TenantCode)
                        };
                   this.UnitOfWork.ExecuteSqlCommand(SqlInsertSyncProcess, parms);
                }
                trans.Commit();
            }
        }
        private static string ConverToInParameter(List<CIIC.HSR.TSP.WF.BizObject.USER_TASKBO> tasks)
        {
                string strRetIds = "";
                foreach (var userTaskBo in tasks)
                {
                    strRetIds = strRetIds + "'" + userTaskBo.TASK_GUID + "',";
                }
                strRetIds = "( " + strRetIds.Substring(0, strRetIds.Length - 1) + " )";

                return strRetIds;
        }
    }
}
