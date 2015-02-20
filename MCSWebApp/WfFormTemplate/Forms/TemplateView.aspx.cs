using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using MCS.Library.Core;
using MCS.Library.OGUPermission;
using MCS.Library.SOA.DataObjects;
using MCS.Library.SOA.DataObjects.Workflow;
using MCS.Web.Library.MVC;
using MCS.Web.Library.Script;
using WfFormTemplate.DataObjects;
using MCS.Library.SOA.DataObjects.Workflow.Tasks;

namespace WfFormTemplate.Forms
{
    public partial class TemplateView : ViewBase<TemplateCommandState>
    {
        protected override void CommandStateInitialized()
        {
            bindingControl.Data = this.ViewData.Data;

            attachmentsControl.DefaultResourceID = ViewData.Data.ID;
            attachmentsControl.RelativePath = string.Format(@"TestForm\{0}\{1}", DateTime.Now.Year, DateTime.Now.Month);
        }

        protected void moveToControl_AfterCreateExecutor(WfExecutorBase executor)
        {
            executor.PrepareApplicationData += new ExecutorEventHandler(executor_PrepareApplicationData);
            executor.SaveApplicationData += new ExecutorEventHandler(executor_SaveApplicationData);

            executor.BeforeExecute += new ExecutorEventHandler(executor_BeforeExecute);
        }

        void executor_BeforeExecute(WfExecutorDataContext dataContext)
        {
            WfRuntime.ProcessContext.PrepareBranchProcessParams += new WfPrepareBranchProcessParamsHandler(ProcessContext_PrepareBranchProcessParams);
        }

        void ProcessContext_PrepareBranchProcessParams(IWfBranchProcessGroup group, WfBranchProcessStartupParamsCollection branchParams)
        {

        }

        private void executor_PrepareApplicationData(WfExecutorDataContext dataContext)
        {
            bindingControl.CollectData();
            this.ViewData.Data.Loaded = true;

            WfClientContext.Current.CurrentActivity.Process.ApprovalRootProcess.Context["appData"] =
                SerializationHelper.SerializeObjectToString(this.ViewData, SerializationFormatterType.Binary);
        }

        private void executor_SaveApplicationData(WfExecutorDataContext dataContext)
        {
            //if (dataContext.OperationType == WfControlOperationType.Return)
            //    throw new ApplicationException("调试终止");

            if (WfClientContext.Current.OriginalActivity.Process.IsApprovalRootProcess)
                AppCommonInfoAdapter.Instance.Update(ViewData.ToAppCommonInfo(ViewData.Data.Subject), "Creator", "DraftDepartmentName");

            if (this.opinionListView.CurrentOpinion != null && dataContext.OperationType != WfControlOperationType.CancelProcess)
                GenericOpinionAdapter.Instance.Update(opinionListView.CurrentOpinion);

            MaterialAdapter.Instance.SaveDeltaMaterials(attachmentsControl.DeltaMaterials);

            //如果进入到了维护模式
            if (dataContext.CurrentProcess.Status == WfProcessStatus.Maintaining)
            {
                foreach (IWfBranchProcessTemplateDescriptor template in WfClientContext.Current.CurrentActivity.Descriptor.BranchProcessTemplates)
                {
                    DispatchStartBranchProcessTask.SendTask(WfClientContext.Current.CurrentActivity.ID, template, true);
                }
            }
        }

        protected void moveToControl_ProcessChanged(IWfProcess process)
        {
            if (this.ViewData.Data.Amount >= 5000)
            {
                process.ApplicationRuntimeParameters["TempApprover"] =
                    (IUser)OguUser.CreateWrapperObject(OguMechanismFactory.GetMechanism().GetObjects<IUser>(SearchOUIDType.LogOnName, "quym").First());
            }
            else
            {
                process.ApplicationRuntimeParameters["TempApprover"] =
                    (IUser)OguUser.CreateWrapperObject(OguMechanismFactory.GetMechanism().GetObjects<IUser>(SearchOUIDType.LogOnName, "fanhy").First());
            }
        }
    }
}