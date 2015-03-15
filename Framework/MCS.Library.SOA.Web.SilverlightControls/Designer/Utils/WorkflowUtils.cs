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
using Designer.Models;
using Newtonsoft.Json;
using Designer.ViewModels;
using Northwoods.GoXam;

namespace Designer.Utils
{
	public class WorkflowUtils
	{
		public const string CLIENTSCRIPT_PARAM_WORKFLOW = "Workflow";
		public const string CLIENTSCRIPT_PARAM_ACTIVITY = "Activity";
		public const string CLIENTSCRIPT_PARAM_TRANSITION = "Transition";

		public const string DIAGRAM_XELEMENT_ROOTNAME = "root";
		public const string DIAGRAM_XELEMENT_NODENAME = "node";
		public const string DIAGRAM_XELEMENT_LINKNAME = "link";

		public static string CurrentKey = string.Empty;

		/// <summary>
		/// 将 node 信息转化成 info 信息
		/// </summary>
		/// <param name="node"></param>
		/// <returns></returns>
		public static ActivityInfo ExtractActivityInfo(ActivityNode node)
		{
			ActivityType actType = ActivityType.Normal;

			if (node.IsDynamic == false)
				 actType = (ActivityType)Enum.Parse(typeof(ActivityType), node.Category, true);

			ActivityInfo actInfo = new ActivityInfo()
			{
				Key = node.Key,
				Name = node.WfName,
				Description = node.WfDescription,
				Enabled = node.WfEnabled,
				ActivityType = actType,
				TemplateID = node.TemplateID
			};
			return actInfo;
		}

		public static string ExtractActivityInfoJson(ActivityNode node)
		{
			var info = ExtractActivityInfo(node);
			return JsonConvert.SerializeObject(info);
		}

		/// <summary>
		/// 将 link 信息转化成 info 信息
		/// </summary>
		/// <param name="link"></param>
		/// <returns></returns>
		public static TransitionInfo ExtractTransitionInfo(ActivityLink link)
		{
			TransitionInfo tranInfo = new TransitionInfo()
			{
				Key = link.Key,
				Name = link.Text,
				Enabled = link.WfEnabled,
				FromActivityKey = link.From,
				ToActivityKey = link.To,
				IsReturn = link.WfReturnLine
			};
			return tranInfo;
		}

		public static string ExtractTransitionInfoJson(ActivityLink link)
		{
			var info = ExtractTransitionInfo(link);
			return JsonConvert.SerializeObject(info);
		}

		/// <summary>
		/// 将 流程图形 信息转化成 info 信息
		/// </summary>
		/// <param name="link"></param>
		/// <returns></returns>
		public static WorkflowInfo ExtractWorkflowInfo(DiagramPageViewModel diagram)
		{
			WorkflowInfo wfInfo = new WorkflowInfo()
			{
				Key = diagram.Key,
				Name = diagram.Name
			};
			return wfInfo;
		}

		public static string ExtractWorkflowInfoJson(DiagramPageViewModel diagram)
		{
			var info = ExtractWorkflowInfo(diagram);
			return JsonConvert.SerializeObject(info);
		}

		public static string ExtractWorkflowInfoJson(Diagram diagram)
		{
			var info = new WorkflowInfo()
			{
				Key = diagram.Tag.ToString()
			};
			return JsonConvert.SerializeObject(info);
		}
	}


}
