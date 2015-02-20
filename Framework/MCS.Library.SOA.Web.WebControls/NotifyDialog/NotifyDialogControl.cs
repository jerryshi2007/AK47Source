using System;
using System.IO;
using System.Web;
using System.Text;
using System.Web.UI;
using System.Reflection;
using System.ComponentModel;
using System.Web.UI.WebControls;
using System.Collections.Generic;
using MCS.Library.SOA.DataObjects;
using MCS.Web.Library;
using MCS.Library.Core;
using System.Web.UI.HtmlControls;
using MCS.Library.Globalization;
using MCS.Library.SOA.DataObjects.Workflow;
using System.Collections.Specialized;

[assembly: WebResource("MCS.Web.WebControls.NotifyDialog.notifyDialogLogo.gif", "image/gif")]

namespace MCS.Web.WebControls
{
	/// <summary>
	/// 通知对话框的控件
	/// </summary>
	[ToolboxData("<{0}:NotifyDialogControl runat=server></{0}:NotifyDialogControl>")]
	public class NotifyDialogControl : Control
	{
		public const string NotifyDialogOpID = "notifyDialog";

		#region Properties
		public string GetNotifyDialogUrl(string taskID)
		{
			return WebUtility.GetRequestUrl(string.Format("_op={0}&taskID={1}", NotifyDialogOpID, taskID), new string[] { "_op", "taskID" });
		}
		#endregion

		#region Override and event handler
		protected override void OnInit(EventArgs e)
		{
			//Page.PreLoad += new EventHandler(Page_PreLoad);
			string op = WebUtility.GetRequestQueryString("_op", string.Empty);

			if (string.Compare(op, NotifyDialogOpID, true) == 0)
				DoNotifyDialog();

			base.OnInit(e);
		}

		protected override void Render(HtmlTextWriter writer)
		{
			if (this.DesignMode)
			{
				writer.Write(string.Format("<img src=\"{0}\"></img>",
					HttpUtility.HtmlAttributeEncode(
						Page.ClientScript.GetWebResourceUrl(this.GetType(),
						"MCS.Web.WebControls.NotifyDialog.notifyDialogLogo.gif"))));
			}
		}

		private void Page_PreLoad(object sender, EventArgs e)
		{
			string op = WebUtility.GetRequestQueryString("_op", string.Empty);

			if (string.Compare(op, NotifyDialogOpID, true) == 0)
				DoNotifyDialog();
		}
		#endregion

		#region Dialog Operations
		/// <summary>
		/// 如果请求是提醒对话框的，则处理该请求，显示提醒对话框的内容
		/// </summary>
		private void DoNotifyDialog()
		{
			if (HttpContext.Current.Request.RequestType == "POST")
			{
				string taskID = HttpContext.Current.Request.QueryString["taskID"];
				string taskSource = WebUtility.GetRequestFormString("taskSource", "userTask");

				if (string.IsNullOrEmpty(taskID) == false && GetAutoTransferToCompletedTask())
				{
					UserTask task = new UserTask();
					task.TaskID = taskID;
					UserTaskCollection tasks = new UserTaskCollection();
					tasks.Add(task);
					if (taskSource == "userTask")
						UserTaskAdapter.Instance.DeleteUserTasks(tasks);
					else
						UserTaskAdapter.Instance.DeleteUserAccomplishedTasks(tasks);
				}

				SaveAutoTransferToCompletedTaskFlag();

				//WebUtility.ResponseRefreshParentWindowScriptBlock();
				HttpContext.Current.Response.Write(ExtScriptHelper.GetRefreshBridgeScript());
				WebUtility.ResponseTimeoutScriptBlock("top.close();", ExtScriptHelper.DefaultResponseTimeout);

				HttpContext.Current.Response.End();
			}
			else
			{
				Page page = new Page();

				HtmlGenericControl html = new HtmlGenericControl("html");
				WebUtility.SetCurrentPage(page);
				page.Controls.Add(html);

				HtmlHead head = new HtmlHead();
				html.Controls.Add(head);

				HtmlTitle title = new HtmlTitle();
				title.Text = Translator.Translate(Define.DefaultCulture, "提醒消息");
				head.Controls.Add(title);

				HtmlGenericControl body = new HtmlGenericControl("body");
				html.Controls.Add(body);

				WebUtility.AttachPageModules(page);

				string temmplate = GetNotifyDialogHtml();
				string pageHtml = InitNotifyDialogPage(temmplate);

				body.Controls.Add(new LiteralControl(pageHtml));

				((IHttpHandler)page).ProcessRequest(HttpContext.Current);

				HttpContext.Current.Response.End();
			}
		}

		private static string TranslatePrefix(string text, string prefix)
		{
			string result = text;

			if (text.IndexOf(prefix) == 0)
				result = Translator.Translate("Portal", prefix) + text.Substring(prefix.Length);

			return result;
		}

		/// <summary>
		/// 初始化提醒消息的页面内容
		/// </summary>
		/// <param name="html"></param>
		/// <returns></returns>
		private string InitNotifyDialogPage(string html)
		{
			string taskID = HttpContext.Current.Request.QueryString["taskID"];
			string result = html;
			string taskSource = "userTask";

			result = FillPlaceHolder(result, "$notifyTitle$"
				, Translator.Translate(Define.DefaultCulture, "提醒消息"), true);

			result = FillPlaceHolder(result, "$title$"
				, Translator.Translate(Define.DefaultCulture, "标题"), true);

			result = FillPlaceHolder(result, "$content$"
				, Translator.Translate(Define.DefaultCulture, "内容"), true);

			result = FillPlaceHolder(result, "$priority$"
				, Translator.Translate(Define.DefaultCulture, "优先级"), true);

			result = FillPlaceHolder(result, "$from$"
				, Translator.Translate(Define.DefaultCulture, "发送人"), true);

			result = FillPlaceHolder(result, "$sentTime$"
				, Translator.Translate(Define.DefaultCulture, "发送日期"), true);

			result = FillPlaceHolder(result, "$expiredTime$"
				, Translator.Translate(Define.DefaultCulture, "过期时间"), true);

			result = FillPlaceHolder(result, "$settings$"
				, Translator.Translate(Define.DefaultCulture, "个人设定"), true);

			result = FillPlaceHolder(result, "$delAfterRead$"
				, Translator.Translate(Define.DefaultCulture, "阅读后删除"), true);

			result = FillPlaceHolder(result, "$delBtn$"
				, Translator.Translate(Define.DefaultCulture, "关闭(C)"), true);

			result = FillPlaceHolder(result, "$logoSrc",
				Page.ClientScript.GetWebResourceUrl(this.GetType(),
				"MCS.Web.WebControls.NotifyDialog.notifyDialogLogo.gif"), false);

			if (GetDefaultAutoTransferToCompletedTask())
				result = FillPlaceHolder(result, "$transToCompletedTask", "checked", false);
			else
				result = FillPlaceHolder(result, "$transToCompletedTask", string.Empty, false);

			if (string.IsNullOrEmpty(taskID) == false)
			{
				UserTaskCollection tasks = UserTaskAdapter.Instance.GetUserTasks(UserTaskIDType.TaskID, UserTaskFieldDefine.All, taskID);

				if (tasks.Count == 0)
				{
					tasks = UserTaskAdapter.Instance.GetUserAccomplishedTasks(taskID);
					taskSource = "userAccomplishedTask";
				}

				ExceptionHelper.FalseThrow(tasks.Count > 0, "不能找到ID为{0}的提醒消息", taskID);

				UserTask task = tasks[0];

				task.TaskTitle = TranslatePrefix(task.TaskTitle, "文件被撤回：");
				task.TaskTitle = TranslatePrefix(task.TaskTitle, "流程作废：");
				task.TaskTitle = TranslatePrefix(task.TaskTitle, "催办通知: ");

				result = FillPlaceHolder(result, "$taskSource", taskSource, true);
				result = FillPlaceHolder(result, "$taskTitle", task.TaskTitle, true);
				result = FillPlaceHolder(result, "$taskUrl", GetTaskUrl(task), false);
				result = FillPlaceHolder(result, "$taskContent", task.Body, true);
				result = FillPlaceHolder(result, "$taskEmergency", EnumItemDescriptionAttribute.GetDescription(task.Emergency), true);
				result = FillPlaceHolder(result, "$taskSender", task.SourceName, true);
				result = FillPlaceHolder(result, "$taskStartTime", task.TaskStartTime.ToString("yyyy-MM-dd HH:mm:ss"), false);

				if (task.ExpireTime != DateTime.MinValue)
					result = FillPlaceHolder(result, "$taskExpirDate", task.ExpireTime.ToString("yyyy-MM-dd HH:mm:ss"), false);
				else
					result = FillPlaceHolder(result, "$taskExpirDate", string.Empty, false);

				result = FillPlaceHolder(result, "$personalSettingStyle", string.Empty, false);
			}

			return result;
		}

		private static string GetTaskUrl(UserTask task)
		{
			string result = "#";

			try
			{
				IWfProcess process = WfRuntime.GetProcessByProcessID(task.ProcessID);

				if (process != null && process.Descriptor.Url.IsNotEmpty())
				{
					NameValueCollection uriParams = UriHelper.GetUriParamsCollection(process.Descriptor.Url);

					uriParams.Clear();
					uriParams["resourceID"] = task.ResourceID;
					uriParams["processID"] = task.ProcessID;

					result = UriHelper.CombineUrlParams(process.Descriptor.Url, uriParams);
				}
			}
			catch (System.Exception)
			{

			}

			return result;
		}

		private bool GetDefaultAutoTransferToCompletedTask()
		{
			bool result = true;

			HttpContext context = HttpContext.Current;

			HttpCookie cookie = context.Request.Cookies["NotificationData"];

			if (cookie != null)
			{
				try
				{
					if (cookie.Value.ToLower() == "false")
						result = false;
				}
				catch (System.ApplicationException)
				{
					//忽略cookie中的内容错误
				}
			}

			return result;
		}

		private bool GetAutoTransferToCompletedTask()
		{
			HttpContext context = HttpContext.Current;
			bool result = false;

			if (context.Request.Form["transToCompletedTask"] == "true")
				result = true;

			return result;
		}

		private void SaveAutoTransferToCompletedTaskFlag()
		{
			HttpContext context = HttpContext.Current;

			HttpCookie cookie = new HttpCookie("NotificationData");

			cookie.Value = GetAutoTransferToCompletedTask().ToString().ToLower();
			cookie.Expires = DateTime.MaxValue;

			context.Response.Cookies.Add(cookie);
		}

		private string FillPlaceHolder(string html, string placeHolderName, string data, bool htmlEncode)
		{
			if (htmlEncode)
			{
				data = HttpUtility.HtmlEncode(data);
				data = data.Replace("\n", "<br/>");
			}

			return html.Replace(placeHolderName, data);
		}

		private string GetNotifyDialogHtml()
		{
			return ResourceHelper.LoadStringFromResource(Assembly.GetExecutingAssembly(), "MCS.Web.WebControls.NotifyDialog.notifyTaskTemplate.htm");
		}
		#endregion
	}
}
