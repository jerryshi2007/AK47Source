using System;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Ink;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;
using System.Collections.Generic;
using WorkflowRuntime.Models;
using System.Windows.Browser;
using Newtonsoft.Json;

namespace WorkflowRuntime
{
	public static class WebInteraction
	{
		public static void OpenBranchProc(string wrapperid, string activityID)
		{
			HtmlPage.Window.Invoke("OpenBranchProc", activityID, wrapperid);
		}

		public static WorkflowInfo LoadProcInfo(string wrapperid)
		{
			var wfInfo = HtmlPage.Window.Invoke("LoadWfInfo", wrapperid).ToString();

			var result = JsonConvert.DeserializeObject<WorkflowInfo>(wfInfo);

			return result;
		}

		public static Dictionary<string, string> CultureInfo { get; set; }

		public static void InitCultureInfo()
		{
			var cultureInfo = HtmlPage.Window.Invoke("GetCultureInfo").ToString();

			if (string.IsNullOrEmpty(cultureInfo))
			{
				CultureInfo = new Dictionary<string, string>();
				return;
			}

			var result = JsonConvert.DeserializeObject<Dictionary<string,string>>(cultureInfo);
			
			CultureInfo = result;
		}
	}
}
