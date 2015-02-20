using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Serialization;

namespace MCS.Library.SOA.Contracts.DataObjects.Workflow
{
    [DataContract(IsReference = true)]
    [Serializable]
    public class WfClientNormalActivity : WfClientActivity
    {
        private WfClientNormalActivity()
        {
        }

        public WfClientNormalActivity(WfClientActivityDescriptor descriptor) : base(descriptor)
        {
        }
	}
}
