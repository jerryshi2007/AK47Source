using MCS.Library.WF.Contracts.Workflow.Descriptors;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace MCS.Library.WF.Contracts.Workflow.Runtime
{
    [DataContract]
    [Serializable]
    public class WfClientNextActivity
    {
        public WfClientTransitionDescriptor Transition
        {
            get;
            set;
        }

        public WfClientActivity Activity
        {
            get;
            set;
        }
    }
}
