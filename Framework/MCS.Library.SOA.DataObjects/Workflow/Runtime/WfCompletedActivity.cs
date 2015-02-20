using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MCS.Library.Core;

namespace MCS.Library.SOA.DataObjects.Workflow
{
	[Serializable]
	[XElementSerializable]
	public class WfCompletedActivity : WfActivityBase
	{
		private WfCompletedActivity()
		{
		}

		public WfCompletedActivity(IWfActivityDescriptor descriptor)
			: base(descriptor)
		{
		}
	}
}
