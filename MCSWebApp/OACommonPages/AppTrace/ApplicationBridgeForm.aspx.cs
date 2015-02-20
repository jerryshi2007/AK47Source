using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Collections.Specialized;
using MCS.Library.Core;
using MCS.Web.Library;
using MCS.Library.SOA.DataObjects.Workflow;
using MCS.Library.SOA.DataObjects;

namespace MCS.OA.CommonPages.AppTrace
{
	public partial class ApplicationBridgeForm : System.Web.UI.Page
	{
		protected void Page_Load(object sender, EventArgs e)
		{
			string processID = WebUtility.GetRequestQueryString("processID", string.Empty);

			if (processID.IsNotEmpty())
			{
				IWfProcess process = WfRuntime.GetProcessByProcessID(processID);

				string url = GetProcessUrl(process);

				if (url.IsNotEmpty())
					Response.Redirect(url);
			}
		}

		private static string GetProcessUrl(IWfProcess process)
		{
			string result = string.Empty;

			if (process.CurrentActivity != null)
			{
				UserTaskCollection tasks = UserTaskAdapter.Instance.LoadUserTasks(
					builder => builder.AppendItem("ACTIVITY_ID", process.CurrentActivity.ID));

				if (tasks.Count > 0)
					result = tasks[0].NormalizedUrl;
			}

			if (result.IsNullOrEmpty())
			{
				string infoID = process.ResourceID;

				if (AppCommonInfoAdapter.Instance.Exists(infoID) == false)
					infoID = process.ID;

				if (AppCommonInfoAdapter.Instance.Exists(infoID))
				{
					AppCommonInfo commonInfo = AppCommonInfoAdapter.Instance.Load(infoID);

					NameValueCollection uriParams = UriHelper.GetUriParamsCollection(commonInfo.Url);

					uriParams["processID"] = process.ID;

					result = UriHelper.CombineUrlParams(commonInfo.Url, uriParams);
				}
			}

			if (result.IsNotEmpty())
				result = UserTask.GetNormalizedUrl(process.Descriptor.ApplicationName, process.Descriptor.ProgramName, result);

			return result;
		}
	}
}