using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace MCS.Library.WF.Contracts.Workflow.Descriptors
{
    [DataContract]
    [Serializable]
    public class WfClientActivityOperatorResourceDescriptor : WfClientActivityResourceDescriptorBase
    {
        public WfClientActivityOperatorResourceDescriptor()
        {
        }

        public WfClientActivityOperatorResourceDescriptor(string actKey)
            : base(actKey)
        {
        }
    }
}
