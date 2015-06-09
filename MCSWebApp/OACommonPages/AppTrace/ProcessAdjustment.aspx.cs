using MCS.Library.Core;
using MCS.Library.Data.Builder;
using MCS.Library.Data.Mapping;
using MCS.Library.Globalization;
using MCS.Library.OGUPermission;
using MCS.Library.Passport;
using MCS.Library.Principal;
using MCS.Library.SOA.DataObjects;
using MCS.Library.SOA.DataObjects.Workflow;
using MCS.Library.SOA.DataObjects.Workflow.Conditions;
using MCS.Web.Library;
using MCS.Web.Library.MVC;
using MCS.Web.Library.Script;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

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
            
            BindProcessStatus(processStatusSelector);

            bindingControl.Data = QueryCondition;
        }

        private static void BindProcessStatus(ListControl control)
        {
            EnumItemDescriptionList statusDesp = EnumItemDescriptionAttribute.GetDescriptionList(typeof(WfProcessStatus));

            List<EnumItemDescription> list = statusDesp.ToList();

            control.BindData(list, "Name", "Description");
            control.Items.Insert(0, new ListItem(Translator.Translate("Workflow", "全部"), string.Empty));
        }

        protected void QueryBtnClick(object sender, EventArgs e)
        {
            bindingControl.CollectData();

            whereCondition.Value = this.QueryCondition.ToSqlBuilder().ToSqlString(TSqlBuilder.Instance);

            ExecQuery();
        }

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

        private WfProcessQueryCondition QueryCondition
        {
            get
            {
                WfProcessQueryCondition result = (WfProcessQueryCondition)ViewState["QueryCondition"];

                if (null == result)
                {
                    result = new WfProcessQueryCondition();
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