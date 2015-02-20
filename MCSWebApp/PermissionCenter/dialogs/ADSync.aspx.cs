using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using MCS.Library.Data.Mapping;
using MCS.Library.SOA.DataObjects.Workflow;
using MCS.Library.SOA.DataObjects;
using MCS.Library.Data.Builder;
using MCS.Library.Core;
using PC = MCS.Library.SOA.DataObjects.Security;
using PermissionCenter.WebControls;

namespace PermissionCenter.Dialogs
{
	public partial class ADSync : System.Web.UI.Page
	{
		public static readonly string jobId = "9E765462-D8BA-4712-9FF8-8FA5FE054A52";

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

		protected override void OnPreRender(EventArgs e)
		{
			base.OnPreRender(e);
			this.Response.CacheControl = "no-cache";
		}

		protected string GetIframeUrl()
		{
			return "/MCSWebApp/WorkflowDesigner/PlanScheduleDialog/TaskMonitor.aspx?category=" + Server.UrlEncode("PC→AD") + "&cansearch=no&nohead=yes";
		}

		protected void btnSync_Click(object sender, EventArgs e)
		{
			InvokeWebServiceJob job = InvokeWebServiceJobAdapter.Instance.LoadSingleDataByJobID(jobId);
			if (job == null)
			{
				string url = ResourceUriSettings.GetConfig().Paths["pcServiceAdSync"].Uri.ToString();
				string methodName = "StartSynchronize";
				WfServiceOperationParameterCollection parameters = new WfServiceOperationParameterCollection();
				parameters.Add(new WfServiceOperationParameter() { Name = "startPath", Type = WfSvcOperationParameterType.String, Value = "" });
				parameters.Add(new WfServiceOperationParameter() { Name = "taskID", Type = WfSvcOperationParameterType.RuntimeParameter, Value = "taskID" }); //特殊用法
				string name = "AD同步-用户发起";
				job = Util.CreateImmediateJob(jobId, name, "PC→AD", url, methodName, parameters);
				InvokeWebServiceJobAdapter.Instance.Update(job);
			}

			//检查是否有任务已经进入执行序列
			if (SysTaskAdapter.Instance.Load(w => { w.AppendItem("CATEGORY", "PC→AD"); }).Any())
			{
				BannerNotice notic = (BannerNotice)Master.FindControl("notice");
				notic.Text = ("检测到已经存在一个未执行或执行中的同步任务，请等待任务结束或者终止任务后重试。");
			}
			else
			{
				SysTask task = job.ToSysTask();
				task.Source = MCS.Library.Principal.DeluxeIdentity.CurrentUser;
				task.Url = this.ResolveUrl("~/lists/ADLog.aspx?id=" + task.TaskID);

				SysTaskAdapter.Instance.Update(task);
			}
		}
	}
}