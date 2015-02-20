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
    public class WfClientActivityAssigneesResourceDescriptor : WfClientActivityResourceDescriptorBase
    {
        public WfClientActivityAssigneesResourceDescriptor()
        {
        }

        public WfClientActivityAssigneesResourceDescriptor(string actKey)
            : base(actKey)
        {
        }
    }
}
