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

namespace Designer.Models
{
	public class WorkflowInfo
	{
		public static WorkflowInfo CreateEmptyWorkflow(string wfKey)
		{
			if (string.IsNullOrEmpty(wfKey)) throw new ArgumentNullException("workflow key 不能为空");

			ActivityInfo init = new ActivityInfo()
			{
				Key = "N0",
				Name = "起 始",
				Description = "流程开始",
				Enabled = true,
				ActivityType = ActivityType.Initial
			};

			ActivityInfo end = new ActivityInfo()
			{
				Key = "N1",
				Name = "结 束",
				Description = "流程结束",
				Enabled = true,
				ActivityType = ActivityType.Completed
			};

			return new WorkflowInfo()
			{
				Key = wfKey,
				Name = "新建流程",
				Activities = { init, end }
			};
		}

		public string Key { get; set; }
		public string Name { get; set; }
		public string GraphDescription { get; set; }

		private List<ActivityInfo> _activities = new List<ActivityInfo>();
		public List<ActivityInfo> Activities { get { return _activities; } set { this._activities = value; } }

		private List<TransitionInfo> _transitions = new List<TransitionInfo>();
		public List<TransitionInfo> Transitions { get { return _transitions; } set { this._transitions = value; } }
	}
}
