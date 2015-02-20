using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MCS.Library.Core;

namespace MCS.Library.SOA.DataObjects.Workflow
{
	[Serializable]
	[XElementSerializable]
	public class WfInitialActivity : WfActivityBase
	{
		private WfInitialActivity()
		{
		}

		public WfInitialActivity(IWfActivityDescriptor descriptor)
			: base(descriptor)
		{
		}
	}
}
