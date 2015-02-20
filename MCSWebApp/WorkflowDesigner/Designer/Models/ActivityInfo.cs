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
	/// <summary>
	/// same as \MCSFramework\02.Develop\Framework\MCS.Library.SOA.DataObjects\Workflow\Common\Enumrations.cs
	/// </summary>
	public enum ActivityType
	{
		Normal,
		Initial,
		Composite,
		Conditional,
		Completed
	}

	public class ActivityInfo
	{
		public string Key { get; set; }
		public string Name { get; set; }
		public string Description { get; set; }
		public bool Enabled { get; set; }
		public bool IsDynamic { get; set; }
		public List<object> BranchProcessTemplates { get; set; }
		public string TemplateID { get; set; }
		public ActivityType ActivityType { get; set; }
	}
}
