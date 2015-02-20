using System;
using System.Web.UI.WebControls;
using MCS.Library.Core;
using MCS.Library.Data.Builder;
using MCS.Library.Data.Mapping;
using MCS.Library.SOA.DataObjects;
using MCS.Library.SOA.DataObjects.Workflow;
using PC = MCS.Library.SOA.DataObjects.Security;
using PermissionCenter.WebControls;
using System.Linq;

namespace PermissionCenter.Dialogs
{
	public partial class ADReverseSync : System.Web.UI.Page
	{
		public static readonly string JobID = "CEEE7F7F-3F81-46E6-AC08-6141B1C42AF0";

		protected override void OnPreRender(EventArgs e)
		{
			base.OnPreRender(e);
			this.Response.CacheControl = "no-cache";
		}

		public override void ProcessRequest(System.Web.HttpContext context)
		{
			if (Util.SuperVisiorMode == false)
			{
				context.Response.Redirect("~/dialogs/Profile.aspx");
			}
			else
			{
				base.ProcessRequest(context);
			}
		}

		protected void btnSync_Click(object sender, EventArgs e)
		{
			InvokeWebServiceJob job = InvokeWebServiceJobAdapter.Instance.LoadSingleDataByJobID(JobID);
			if (job == null)
			{
				string url = ResourceUriSettings.GetConfig().Paths["pcServiceAdSync"].Uri.ToString();
				string methodName = "StartADReverseSynchronize";
				WfServiceOperationParameterCollection parameters = new WfServiceOperationParameterCollection();
				parameters.Add(new WfServiceOperationParameter() { Name = "taskID", Type = WfSvcOperationParameterType.RuntimeParameter, Value = "taskID" }); //特殊用法
				string name = "AD逆同步-用户发起";

				job = Util.CreateImmediateJob(JobID, name, "AD→PC", url, methodName, parameters);
				InvokeWebServiceJobAdapter.Instance.Update(job);
			}

			//检查是否有任务已经进入执行序列
			if (SysTaskAdapter.Instance.Load(w => { w.AppendItem("CATEGORY", "AD→PC"); }).Any())
			{
				BannerNotice notic = (BannerNotice)Master.FindControl("notice");
				notic.Text = ("检测到已经存在一个未执行或执行中的同步任务，请等待任务结束或者终止任务后重试。");
			}
			else
			{
				SysTask task = job.ToSysTask();
				task.Url = this.ResolveUrl("~/lists/ADReverseLog.aspx?id=" + task.TaskID);
				task.Source = MCS.Library.Principal.DeluxeIdentity.CurrentUser;
				SysTaskAdapter.Instance.Update(task);
			}
		}

		protected string GetIframeUrl()
		{
			return "/MCSWebApp/WorkflowDesigner/PlanScheduleDialog/TaskMonitor.aspx?category=" + Server.UrlEncode("AD→PC") + "&cansearch=no&nohead=yes";
		}
	}
}