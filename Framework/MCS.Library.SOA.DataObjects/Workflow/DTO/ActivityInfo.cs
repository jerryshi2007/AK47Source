using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MCS.Library.SOA.DataObjects.Workflow.DTO
{
	public class ActivityInfo
	{
		public string Key { get; set; }
		public string CloneKey { get; set; }
		public string Name { get; set; }
		public string Description { get; set; }
		public bool Enabled { get; set; }
		public bool IsDynamic { get; set; }
		public WfActivityType ActivityType { get; set; }

		public string ID { get; set; }
		public string Status { get; set; }
		public DateTime StartTime { get; set; }
		public DateTime EndTime { get; set; }
		public string Operator { get; set; }
		public bool HasBranchProcess { get; set; }
	}
}
