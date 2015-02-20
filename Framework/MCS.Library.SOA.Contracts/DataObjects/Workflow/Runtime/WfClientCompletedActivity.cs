using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Serialization;
using MCS.Library.SOA.Contracts.DataObjects.Workflow;

namespace MCS.Library.SOA.Contracts.DataObjects.Workflow
{
    [DataContract(IsReference = true)]
    [Serializable]
    public class WfClientCompletedActivity : WfClientActivity
	{
        public WfClientCompletedActivity()
		{
		}

        public WfClientCompletedActivity(WfClientActivityDescriptor descriptor): base(descriptor)
		{
		}
	}
}
