using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Serialization;
using MCS.Library.SOA.Contracts.DataObjects.Workflow;
using MCS.Library.SOA.Contracts.DataObjects;


namespace MCS.Library.SOA.DataObjects.Workflow
{
	[Serializable]
    [DataContract(IsReference = true)]
	public class WfClientInitialActivity : WfClientActivity
	{
		private WfClientInitialActivity()
		{
		}

        public WfClientInitialActivity(WfClientActivityDescriptor descriptor): base(descriptor)
		{
		}
	}
}
