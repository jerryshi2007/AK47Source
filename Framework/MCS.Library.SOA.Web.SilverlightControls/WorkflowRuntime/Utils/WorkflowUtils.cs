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
using Newtonsoft.Json;
using WorkflowRuntime.ViewModels;
using Northwoods.GoXam;

namespace WorkflowRuntime.Utils
{
	public static class WorkflowUtils
	{
		public const string CLIENTSCRIPT_PARAM_WORKFLOW = "Workflow";
		public const string CLIENTSCRIPT_PARAM_ACTIVITY = "Activity";
		public const string CLIENTSCRIPT_PARAM_TRANSITION = "Transition";

		public const string DIAGRAM_XELEMENT_ROOTNAME = "root";
		public const string DIAGRAM_XELEMENT_NODENAME = "node";
		public const string DIAGRAM_XELEMENT_LINKNAME = "link";

		public static void ProcessDatetime(WorkflowInfo wfInfo)
		{
			wfInfo.StartTime = wfInfo.StartTime.ToLocalTime();
			wfInfo.EndTime = wfInfo.EndTime.ToLocalTime();

			foreach (ActivityInfo act in wfInfo.Activities)
			{
				act.StartTime = act.StartTime.ToLocalTime();
				act.EndTime = act.EndTime.ToLocalTime();
			}
		}
	}
}
