using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using MCS.Library.Core;
using MCS.Library.OGUPermission;
using MCS.Library.Principal;
using MCS.Library.SOA.DataObjects.Workflow;
using MCS.Web.Library.MVC;

namespace WfFormTemplate.Forms
{
	public partial class SelectProcessUsers : System.Web.UI.Page
	{
		protected void Page_Load(object sender, EventArgs e)
		{

		}

		protected void btnOK_Click(object sender, EventArgs e)
		{
			string returnUrl = Request.QueryString.GetValue("ru", string.Empty);

			WfProcessDescriptor processDesc = new WfProcessDescriptor();

			processDesc.Key = UuidHelper.NewUuidString();
			processDesc.Name = "自由流程";
			processDesc.ApplicationName = "秘书服务";
			processDesc.ProgramName = "部门通知";
			processDesc.Url = returnUrl;
			processDesc.DefaultTaskTitle = "${Subject}$";

			WfActivityDescriptor initAct = new WfActivityDescriptor("Initial", WfActivityType.InitialActivity);
			initAct.Name = "起草";
			initAct.CodeName = "Initial Activity";
			initAct.Properties.SetValue("AutoSendUserTask", false);
			initAct.Properties.TrySetValue("AllowWithdraw", true);

			processDesc.Activities.Add(initAct);

			foreach (IUser user in processUsers.SelectedOuUserData)
			{
				string key = processDesc.FindNotUsedActivityKey();
				WfActivityDescriptor normalAct = new WfActivityDescriptor(key, WfActivityType.NormalActivity);
				normalAct.Name = user.DisplayName;
				normalAct.CodeName = key;
				normalAct.Properties.SetValue("AutoAppendSecretary", true);
				//normalAct.ActivityType = WfActivityType.NormalActivity;

				WfUserResourceDescriptor userResourceDesc = new WfUserResourceDescriptor(user);
				normalAct.Resources.Add(userResourceDesc);

				processDesc.Activities.Add(normalAct);
			}

			WfActivityDescriptor completedAct = new WfActivityDescriptor("Completed", WfActivityType.CompletedActivity);
			completedAct.Name = "完成";
			completedAct.CodeName = "Completed Activity";

			processDesc.Activities.Add(completedAct);

			for (int j = 0; j < processDesc.Activities.Count - 1; j++)
			{
				processDesc.Activities[j].ToTransitions.AddForwardTransition(processDesc.Activities[j + 1]);
			}

			WfProcessStartupParams startupParams = new WfProcessStartupParams();
			startupParams.ProcessDescriptor = processDesc;
			startupParams.Creator = DeluxeIdentity.CurrentUser;
			startupParams.Assignees.Add(DeluxeIdentity.CurrentUser);
			startupParams.DefaultTaskTitle = "${Subject}$";
			startupParams.RuntimeProcessName = "${Subject}$";
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

			Response.Redirect(string.Format("{0}?resourceID={1}&activityID={2}",
				returnUrl, process.ResourceID, process.CurrentActivity.ID));
		}

		private void executor_AfterModifyWorkflow(WfExecutorDataContext dataContext)
		{
			WfClientContext.Current.OriginalActivity = dataContext.CurrentProcess.CurrentActivity;
		}
	}
}