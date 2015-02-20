using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MCS.Library.SOA.DataObjects.Workflow.DTO
{
	public class TransitionInfo
	{
		public string Key { get; set; }
		public string Name { get; set; }
		public bool Enabled { get; set; }
		public string FromActivityKey { get; set; }
		public string ToActivityKey { get; set; }
		public bool WfReturnLine { get; set; }

		public bool IsPassed { get; set; }
	}
}
