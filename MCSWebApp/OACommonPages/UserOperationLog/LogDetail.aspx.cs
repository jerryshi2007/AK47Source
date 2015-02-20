using System;
using System.Text;
using System.Web;
using System.Web.UI;
using MCS.Library.Core;
using MCS.Library.Globalization;
using MCS.Library.SOA.DataObjects;
using MCS.Library.SOA.DataObjects.Workflow;
using MCS.Web.Library.MVC;

namespace MCS.OA.CommonPages.UserOperationLog
{
	public partial class LogDetail : Page
	{
		protected void Page_Load(object sender, EventArgs e)
		{
			WfRuntime.ProcessContext.EnableSimulation = WfClientContext.SimulationEnabled;

			if (this.IsPostBack == false)
			{
				InitDetailInfo(Request.QueryString["id"]);
				this.DataGridBind();
			}
		}

		private void DataGridBind()
		{
			this.objectDataSource.Condition = string.Format("(LOG_ID = {0})", Convert.ToInt32(Request.QueryString["id"]));
			this.objectDataSource.LastQueryRowCount = -1;

			this.dataGrid.PageIndex = 0;
			this.dataGrid.SelectedKeys.Clear();
			this.dataGrid.DataBind();
		}

		protected void InitDetailInfo(string id)
		{
			int logID = Convert.ToInt32(id);
			Library.SOA.DataObjects.UserOperationLog log = UserOperationLogAdapter.Instance.Load(logID);

			if (log.Equals(null))
			{
				subject.Text =
					appName.Text =
					actName.Text =
					actionName.Text =
					opUserID.Text =
					topDeptID.Text =
					description.Text =
					opDateTime.Text = Translator.Translate(Define.DefaultCulture, "无");
			}

			this.appName.Text = HttpUtility.HtmlEncode((log.ApplicationName == string.Empty) ? Translator.Translate(Define.DefaultCulture, "无") : log.ApplicationName);
			this.actionName.Text = HttpUtility.HtmlEncode((log.OperationName == string.Empty) ? ""
				: Translator.Translate(Define.DefaultCulture, log.OperationName));

			this.actName.Text = HttpUtility.HtmlEncode((log.ActivityName == string.Empty) ? Translator.Translate(Define.DefaultCulture, "无") : Translator.Translate(Define.DefaultCulture, log.ActivityName));

			this.opDateTime.Text = HttpUtility.HtmlEncode((log.OperationDateTime.ToString() == string.Empty) ? Translator.Translate(Define.DefaultCulture, "无") : string.Format("{0:yyyy-MM-dd HH:mm:ss}", log.OperationDateTime));
			this.opUserID.Text = HttpUtility.HtmlEncode((log.Operator.Name.ToString() == string.Empty) ? Translator.Translate(Define.DefaultCulture, "无") : Translator.Translate(Define.DefaultCulture, log.Operator.Name.ToString()));
			this.topDeptID.Text = HttpUtility.HtmlEncode((log.TopDepartment.Name.ToString() == string.Empty) ? Translator.Translate(Define.DefaultCulture, "无") : log.TopDepartment.Name.ToString());
			this.subject.Text = HttpUtility.HtmlEncode((log.Subject == string.Empty) ? "无" : log.Subject);
			this.description.Text =
					HttpUtility.HtmlEncode((log.OperationDescription.ToString() == string.Empty) ?
						Translator.Translate(Define.DefaultCulture, "无") : TranslateLogDescription(log.OperationDescription.ToString()));

			/*
			UserOperationTasksLogCollection collection = UserOperationTasksLogAdapter.Instance.Load(builder => builder.AppendItem("LOG_ID", logID));

			StringBuilder strB = new StringBuilder();
			bool flag = true;
			foreach (var item in collection)
			{
				if (flag)
				{
					flag = false;
					strB.Append(item.SendToUserName);
				}
				else
					strB.AppendFormat(",{0}", item.SendToUserName);
				//this.LB_OperationTasksLog.Items.Add(item.SendToUserName);
			}
			this.operationTasksLog.Text = flag == true ? Translator.Translate(Define.DefaultCulture, "无") : TranslateLogDescription(strB.ToString()); */
			//subject.Text = log.Subject.Equals("") ? "无" : log.Subject;
			//appName.Text = log.ApplicationName.Equals("") ? "无" : log.ApplicationName;
			//actName.Text = log.ActivityName.Equals("") ? "无" : log.ActivityName;
			//actionName.Text = log.ProgramName.Equals("") ? "无" : log.ProgramName;
			//opUserID.Text = log.RealUser.ID.Equals("") ? "无" : log.RealUser.DisplayName;
			//topDeptID.Text = log.TopDepartment.ID.Equals("") ? "无" : log.TopDepartment.Name;
			//description.Text = log.OperationDescription.Equals("") ? "无" : log.OperationDescription;
			//opDateTime.Text = log.OperationDateTime.ToString().Equals("") ? "无" : log.OperationDateTime.ToString();
		}

		private string TranslateLogDescription(string logDescription)
		{
			string result = logDescription;

			result = TranslatePrefix(result, "作废");
			result = TranslatePrefix(result, "撤回");
			result = TranslatePrefix(result, "送签");
			result = TranslatePrefix(result, "流转");
			result = TranslatePrefix(result, "待办转出");

			return result;
		}

		private static string TranslatePrefix(string text, string prefix)
		{
			string result = text;

			if (text.IndexOf(prefix) == 0)
				result = Translator.Translate(Define.DefaultCulture, prefix) + text.Substring(prefix.Length);

			return result;
		}
	}
}