using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MCS.Library.Core;

namespace MCS.Library.SOA.DataObjects.Workflow
{
	[Serializable]
	[XElementSerializable]
	public class WfNormalActivity : WfActivityBase
	{
		private WfNormalActivity()
		{

		}

		public WfNormalActivity(IWfActivityDescriptor descriptor): base(descriptor)
		{
		}
	}
}
