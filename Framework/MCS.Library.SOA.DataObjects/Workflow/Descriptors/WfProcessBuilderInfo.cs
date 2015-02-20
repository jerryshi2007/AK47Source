using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MCS.Library.SOA.DataObjects.Workflow.Builders;

namespace MCS.Library.SOA.DataObjects.Workflow
{
	public class WfProcessBuilderInfo
	{
		public WfProcessBuilderInfo()
		{
		}

		public WfProcessBuilderInfo(WfProcessBuilderBase builder, string processName)
		{
			this.Builder = builder;
			this.ProcessName = processName;
		}

		public WfProcessBuilderBase Builder { get; set; }
		public string ProcessName { get; set; }
	}
}
