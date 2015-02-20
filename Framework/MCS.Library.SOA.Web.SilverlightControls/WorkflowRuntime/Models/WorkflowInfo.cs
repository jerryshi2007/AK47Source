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

namespace WorkflowRuntime.Models
{
	public class WorkflowInfo
	{
		public string Key { get; set; }
		public string Name { get; set; }
		public string GraphDescription { get; set; }

		public string ResourceID { get; set; }
		public string Status { get; set; }
		public DateTime StartTime { get; set; }
		public DateTime EndTime { get; set; }
		public string Creator { get; set; }
		public string OwnerDepartment { get; set; }
		public string CurrentActivityKey { get; set; }
		public string ApplicationName { get; set; }
		public string ProgramName { get; set; }
		public int UpdateTag { get; set; }

		private List<ActivityInfo> _activities = new List<ActivityInfo>();
		public List<ActivityInfo> Activities { get { return _activities; } set { this._activities = value; } }

		private List<TransitionInfo> _transitions = new List<TransitionInfo>();
		public List<TransitionInfo> Transitions { get { return _transitions; } set { this._transitions = value; } }
	}
}
