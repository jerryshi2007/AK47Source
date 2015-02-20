using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using MCS.Library.Core;
using MCS.Library.Data.Builder;
using MCS.Library.Data.Mapping;
using MCS.Library.OGUPermission;
using MCS.Library.Passport;
using MCS.Library.SOA.DataObjects;
using MCS.Library.SOA.DataObjects.Workflow;
using MCS.Web.Library;
using MCS.Web.Library.Script;
using MCS.Library.Principal;
using MCS.Web.Library.MVC;

namespace MCS.OA.CommonPages.AppTrace
{
    public partial class ProcessAdjustment : System.Web.UI.Page
    {
        private static bool MostSupervisor
        {
            get
            {
                return RolesDefineConfig.GetConfig().IsCurrentUserInRoles("ProcessAdmin");
            }
        }

        private static bool NormalSupervisior
        {
            get
            {
                return RolesDefineConfig.GetConfig().IsCurrentUserInRoles("ProcessAdmin", "AdminFormQuery", "WorkflowQueryAdmin");
            }
        }

        private class ChangeAssigneesParam
        {
            public string ProcessID = string.Empty;
            public IUser OriginalUser = null;
            public OguDataCollection<IUser> Users = null;
        }

        protected void Page_Load(object sender, EventArgs e)
        {
            //RolesDefineConfig.GetConfig().IsCurrentUserInRoles("ProcessAdmin", "AdminFormQuery", "WorkflowQueryAdmin").FalseThrow("您没有权限执行此页面");

            Response.Cache.SetNoStore();
            bindingControl.Data = QueryCondition;
        }

        protected void QueryBtnClick(object sender, EventArgs e)
        {
            bindingControl.CollectData();

            WhereSqlClauseBuilder builder = ConditionMapping.GetWhereSqlClauseBuilder(QueryCondition,
                new AdjustConditionValueDelegate(AdjustQueryConditionValue));

            whereCondition.Value = builder.ToSqlString(TSqlBuilder.Instance);

            string subSql = GetCurrentUsersSubSqlClause();

            this.FormatWhereSqlClause(subSql);

            string selectType = GetCurrentSelectTypeSqlClause();

            this.FormatWhereSqlClause(selectType);

            ExecQuery();
        }

        private static object AdjustQueryConditionValue(string propertyName, object data, ref bool ignored)
        {
            object result = data;
            switch (propertyName)
            {
                case "ApplicationName":
                    result = string.Format("%{0}%", (string)data);
                    break;
                case "ProcessName":
                    result = string.Format("%{0}%", (string)data);
                    break;
                case "EndStartTime":
                    result = ((DateTime)data).AddDays(1);
                    break;
                case "DepartmentName":
                    result = string.Format("%{0}%", (string)data);
                    break;
            }

            return result;
        }

        private void FormatWhereSqlClause(string wheresql)
        {
            if (wheresql.IsNotEmpty())
            {
                if (this.whereCondition.Value.IsNotEmpty())
                    this.whereCondition.Value += " AND ";

                this.whereCondition.Value += wheresql;
            }
        }

        /// <summary>
        /// 根据类型得到相应的SQL
        /// </summary>
        /// <returns></returns>
        private string GetCurrentSelectTypeSqlClause()
        {
            StringBuilder sqlResult = new StringBuilder();
            string sqlCondition = string.Empty;

            switch (this.QueryCondition.ProcessSelectType)
            {
                case ProcessFilterType.CurrentActivityError:	//当前环节人员异常
                    sqlCondition = "IU.ACTIVITY_ID = CURRENT_ACTIVITY_ID";
                    break;
                case ProcessFilterType.ExsitedActivitiesError:	//当前环节和后续环节人员异常
                    sqlCondition = "1 = 1";
                    break;
            }

            if (sqlCondition.IsNotEmpty())
                sqlResult.AppendFormat("EXISTS(SELECT * FROM WF.INVALID_ASSIGNEES AS IU WHERE IU.PROCESS_ID = INSTANCE_ID AND {0})", sqlCondition);

            return sqlResult.ToString();
        }

        /// <summary>
        /// 得到当前用户的查询子句
        /// </summary>
        /// <returns></returns>
        private string GetCurrentUsersSubSqlClause()
        {
            string result = string.Empty;

            ConnectiveSqlClauseCollection resultBuilder = new ConnectiveSqlClauseCollection();

            InSqlClauseBuilder inBuilder = new InSqlClauseBuilder("USER_ID");

            QueryCondition.CurrentAssignees.ForEach(u => inBuilder.AppendItem(u.ID));

            WhereSqlClauseBuilder wBuilder = new WhereSqlClauseBuilder();

            if (QueryCondition.AssigneesUserName.IsNotEmpty())
                wBuilder.AppendItem("USER_NAME", TSqlBuilder.Instance.EscapeLikeString(QueryCondition.AssigneesUserName) + "%", "LIKE");

            resultBuilder.Add(inBuilder);
            resultBuilder.Add(wBuilder);

            if (resultBuilder.IsEmpty == false)
            {
                string processCondition = string.Empty;

                switch (this.QueryCondition.AssigneesSelectType)
                {
                    case AssigneesFilterType.CurrentActivity:
                        processCondition = "CA.ACTIVITY_ID = CURRENT_ACTIVITY_ID";
                        break;
                    case AssigneesFilterType.AllActivities:
                        processCondition = "CA.PROCESS_ID = INSTANCE_ID";
                        break;
                }

                result = string.Format("EXISTS(SELECT USER_ID FROM WF.PROCESS_CURRENT_ASSIGNEES CA (NOLOCK) WHERE {0} AND {1})",
                    processCondition, resultBuilder.ToSqlString(TSqlBuilder.Instance));
            }

            return result;
        }

        ///// <summary>
        ///// 得到当前用户的查询子句
        ///// </summary>
        ///// <returns></returns>
        //private string GetCurrentUsersSubSqlClause()
        //{
        //    string result = string.Empty;

        //    string sqlwhere = string.Empty;

        //    if (QueryCondition.CurrentAssignees.Count > 0)
        //    {
        //        InSqlClauseBuilder builder = new InSqlClauseBuilder("USER_ID");

        //        QueryCondition.CurrentAssignees.ForEach(u => builder.AppendItem(u.ID));

        //        sqlwhere = builder.ToSqlStringWithInOperator(TSqlBuilder.Instance);
        //    }

        //    if (QueryCondition.AssigneesUserName.IsNotEmpty())
        //    {
        //        if (sqlwhere.IsNotEmpty())
        //            sqlwhere = string.Format(" {0} AND USER_NAME LIKE N'{1}%' ", sqlwhere, TSqlBuilder.Instance.EscapeLikeString(QueryCondition.AssigneesUserName));
        //        else
        //            sqlwhere = string.Format(" USER_NAME LIKE N'{0}%' ", TSqlBuilder.Instance.EscapeLikeString(QueryCondition.AssigneesUserName));
        //    }

        //    if (sqlwhere.IsNotEmpty())
        //    {
        //        if (this.QueryCondition.ProcessSelectType == ProcessFilterType.CurrentActivityError || this.QueryCondition.AssigneesSelectType == AssigneesFilterType.CurrentActivity)
        //            result = string.Format("EXISTS(SELECT USER_ID FROM WF.PROCESS_CURRENT_ASSIGNEES CA (NOLOCK) WHERE CA.ACTIVITY_ID = CURRENT_ACTIVITY_ID AND {0})", sqlwhere);

        //        if (this.QueryCondition.ProcessSelectType == ProcessFilterType.ExsitedActivitiesError || this.QueryCondition.AssigneesSelectType == AssigneesFilterType.AllActivities)
        //            result = string.Format("EXISTS(SELECT USER_ID FROM WF.PROCESS_CURRENT_ASSIGNEES CA (NOLOCK) WHERE CA.PROCESS_ID = INSTANCE_ID AND {0})", sqlwhere);
        //    }

        //    return result;
        //}

        protected void objectDataSource_Selecting(object sender, ObjectDataSourceSelectingEventArgs e)
        {
            e.InputParameters["totalCount"] = LastQueryRowCount;

            if (e.ExecutingSelectCount == false)
            {
                string where = e.InputParameters["where"] as string;

                if (MostSupervisor == false || NormalSupervisior == false)
                {
                    //检查是否具有分类授权
                    WfApplicationAuthCollection authInfo = WfApplicationAuthAdapter.Instance.GetUserApplicationAuthInfo(DeluxeIdentity.Current.User);
                    var builder = authInfo.GetApplicationAndProgramBuilder("APPLICATION_NAME", "PROGRAM_NAME");
                    if (builder.IsEmpty == false)
                    {
                        if (string.IsNullOrEmpty(where) == false)
                            where += " AND ";

                        where += "(" + builder.ToSqlString(TSqlBuilder.Instance) + ")";

                        e.InputParameters["where"] = where;
                    }
                    else
                    {
                        e.Cancel = true; //没有定义任何权限，不能查询
                    }
                }
            }
        }

        protected void objectDataSource_Selected(object sender, ObjectDataSourceStatusEventArgs e)
        {
            LastQueryRowCount = (int)e.OutputParameters["totalCount"];
        }

        protected void RefreshButton_Click(object sender, EventArgs e)
        {
            ExecQuery();
        }

        private void ExecQuery()
        {
            LastQueryRowCount = -1;
            this.dataGrid.SelectedKeys.Clear();
            this.dataGrid.PageIndex = 0;
        }

        private ProcessQueryCondition QueryCondition
        {
            get
            {
                ProcessQueryCondition result = (ProcessQueryCondition)ViewState["QueryCondition"];

                if (null == result)
                {
                    result = new ProcessQueryCondition();
                    ViewState["QueryCondition"] = result;
                }

                return result;
            }
        }

        private int LastQueryRowCount
        {
            get
            {
                return WebControlUtility.GetViewStateValue(ViewState, "LastQueryRowCount", -1);
            }
            set
            {
                WebControlUtility.SetViewStateValue(ViewState, "LastQueryRowCount", value);
            }
        }

        protected void cancelProcess_ExecuteStep(object data)
        {
            IWfProcess process = WfRuntime.GetProcessByProcessID((string)data);

            CheckPermission(process);

            WfCancelProcessExecutor executor = new WfCancelProcessExecutor(null, process);
            executor.Execute();
        }

        protected void changeAssignees_ExecuteStep(object data)
        {
            ChangeAssigneesParam cap = JSONSerializerExecute.Deserialize<ChangeAssigneesParam>(data);

            IWfProcess process = WfRuntime.GetProcessByProcessID(cap.ProcessID);

            CheckPermission(process);

            WfReplaceAssigneesExecutor executor = new WfReplaceAssigneesExecutor(null, process.CurrentActivity, null, cap.Users);

            executor.Execute();
        }

        private static bool HasPermission(IWfProcess process)
        {
            bool result = MostSupervisor;

            if (result == false)
                result = WfClientContext.IsProcessAdmin(DeluxeIdentity.CurrentUser, process);

            return result;
        }

        private static void CheckPermission(IWfProcess process)
        {
            if (HasPermission(process) == false)
                throw new OperationDeniedException(string.Format("您没有操作流程{0}的权限", process.ID));
        }

        protected void Regen_ExecuteStep(object data)
        {
            string processID = (string)data;

            try
            {
                IWfProcess process = WfRuntime.GetProcessByProcessID(processID);

                CheckPermission(process);

                WfPersistQueue pq = WfPersistQueue.FromProcess(process);

                WfPersistQueueAdapter.Instance.DoQueueOperation(pq);
            }
            catch (System.Exception ex)
            {
                string message = string.Format("生成流程{0}的数据异常: {1}", processID, ex.Message);

                throw new SystemSupportException(message, ex);
            }
        }

        protected void RegenProcesses_Error(Exception ex, object data, ref bool isThrow)
        {
            isThrow = false;
        }

        protected void changeCandidates_ExecuteStep(object data)
        {
            ChangeAssigneesParam cap = JSONSerializerExecute.Deserialize<ChangeAssigneesParam>(data);

            IWfProcess process = WfRuntime.GetProcessByProcessID(cap.ProcessID);

            CheckPermission(process);

            foreach (IWfActivity activity in process.Activities)
            {
                if (activity.Status != WfActivityStatus.Completed && activity.Status != WfActivityStatus.Aborted)
                {
                    if (activity.Candidates.Contains(cap.OriginalUser.ID))
                    {
                        WfReplaceAssigneesExecutor executor = new WfReplaceAssigneesExecutor(null, activity, cap.OriginalUser, cap.Users);

                        executor.ExecuteNotPersist();
                    }
                }
            }

            WfRuntime.PersistWorkflows();
        }
    }
}