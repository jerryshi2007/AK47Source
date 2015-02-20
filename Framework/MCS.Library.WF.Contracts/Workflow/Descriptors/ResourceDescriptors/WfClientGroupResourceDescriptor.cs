using MCS.Library.WF.Contracts.Ogu;
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
    public class WfClientGroupResourceDescriptor : WfClientResourceDescriptor
    {
        public WfClientGroupResourceDescriptor()
        {
        }

        public WfClientGroupResourceDescriptor(WfClientGroup group)
        {
            this.Group = group;
        }

        public WfClientGroup Group
        {
            get;
            set;
        }
    }
}
