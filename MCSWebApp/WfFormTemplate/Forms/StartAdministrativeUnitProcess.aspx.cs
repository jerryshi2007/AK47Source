using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using MCS.Library.Core;
using MCS.Library.SOA.DataObjects;
using MCS.Library.SOA.DataObjects.Workflow;
using MCS.Web.Library.MVC;
using WfFormTemplate.DataObjects;
using MCS.Library.Principal;
using MCS.Web.Library;

namespace WfFormTemplate.Forms
{
	public partial class StartAdministrativeUnitProcess : System.Web.UI.Page
	{
		protected void Page_Load(object sender, EventArgs e)
		{

		}

		protected void btnOK_Click(object sender, EventArgs e)
		{
			PrepareProcess();
		}

		protected void initializeDataBtn_Click(object sender, EventArgs e)
		{
			try
			{
				AUHelper.InitializeAdministrativeUnitData();

				WebUtility.ShowClientMessage("初始化完成", string.Empty, "提示");
			}
			catch (System.Exception ex)
			{
				WebUtility.ShowClientError(ex);
			}
		}

		/// <summary>
		/// 准备流程
		/// </summary>
		private void PrepareProcess()
		{
			string returnUrl = Request.QueryString.GetValue("ru", string.Empty);

			WfProcessDescriptor processDesp = new WfProcessDescriptor();

			processDesp.Key = UuidHelper.NewUuidString();
			processDesp.Name = "自由流程";
			processDesp.ApplicationName = "秘书服务";
			processDesp.ProgramName = "部门通知";
			processDesp.Url = returnUrl;
			processDesp.DefaultTaskTitle = "${Subject}$";

			WfActivityDescriptor initDesp = new WfActivityDescriptor("Initial", WfActivityType.InitialActivity);
			initDesp.Name = "起草";
			initDesp.CodeName = "Initial Activity";
			initDesp.Properties.SetValue("AutoSendUserTask", false);

			processDesp.Activities.Add(initDesp);

			processDesp.Activities.Add(PrepareAURelativeActivityDescriptor(processDesp.FindNotUsedActivityKey()));

			WfActivityDescriptor completedActDesp = new WfActivityDescriptor("Completed", WfActivityType.CompletedActivity);
			completedActDesp.Name = "完成";
			completedActDesp.CodeName = "Completed Activity";

			processDesp.Activities.Add(completedActDesp);

			for (int i = 0; i < processDesp.Activities.Count - 1; i++)
			{
				processDesp.Activities[i].ToTransitions.AddForwardTransition(processDesp.Activities[i + 1]);
			}

			WfProcessStartupParams startupParams = new WfProcessStartupParams();

			startupParams.ApplicationRuntimeParameters["AdministrativeUnit"] = "Group";
			startupParams.ApplicationRuntimeParameters["Amount"] = "";
			startupParams.ApplicationRuntimeParameters["CostCenter"] = "1001";
			startupParams.ProcessDescriptor = processDesp;
			startupParams.Creator = DeluxeIdentity.CurrentUser;
			startupParams.Assignees.Add(DeluxeIdentity.CurrentUser);
			startupParams.DefaultTaskTitle = "${Subject}$";
			startupParams.ResourceID = UuidHelper.NewUuidString();
			startupParams.Department = DeluxeIdentity.CurrentUser.Parent;

			string relativeParams = Request.QueryString["relativeParams"];

			if (relativeParams.IsNotEmpty())
				startupParams.RelativeParams.CopyFrom(UriHelper.GetUriParamsCollection(relativeParams));

			WfStartWorkflowExecutor executor = new WfStartWorkflowExecutor(WfClientContext.Current.OriginalActivity, startupParams);

			executor.AfterModifyWorkflow += new ExecutorEventHandler(executor_AfterModifyWorkflow);
			executor.Execute();

			IWfProcess process = WfClientContext.Current.OriginalActivity.Process;

			returnUrl = UriHelper.RemoveUriParams(returnUrl, "relativeParams");

			HttpContext.Current.Response.Redirect(string.Format("{0}?resourceID={1}&activityID={2}",
				returnUrl, process.ResourceID, process.CurrentActivity.ID));
		}

		/// <summary>
		/// 准备管理单元相关的模板
		/// </summary>
		/// <returns></returns>
		private IWfActivityDescriptor PrepareAURelativeActivityDescriptor(string key)
		{
			WfActivityDescriptor templateActDesp = new WfActivityDescriptor(key, WfActivityType.NormalActivity);
			templateActDesp.Name = "审批";
			templateActDesp.CodeName = key;
			templateActDesp.Properties.SetValue("IsDynamic", true);

			WrappedAUSchemaRole role = new WrappedAUSchemaRole(AUHelper.AUSchemaActivityMatrixRoleID) { Name = "实物管理员", Description = "实物管理员" };

			WfAURoleResourceDescriptor roleDesp = new WfAURoleResourceDescriptor(role);

			templateActDesp.Resources.Add(roleDesp);

			return templateActDesp;
		}

		private void executor_AfterModifyWorkflow(WfExecutorDataContext dataContext)
		{
			WfClientContext.Current.OriginalActivity = dataContext.CurrentProcess.CurrentActivity;
		}
	}
}