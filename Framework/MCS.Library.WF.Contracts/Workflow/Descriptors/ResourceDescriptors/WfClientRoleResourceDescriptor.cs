using MCS.Library.WF.Contracts.Ogu;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace MCS.Library.WF.Contracts.Workflow.Descriptors
{
    [Serializable]
    [DataContract]
    public class WfClientRoleResourceDescriptor : WfClientResourceDescriptor
    {
        public WfClientRoleResourceDescriptor()
        {
        }

        public WfClientRoleResourceDescriptor(string fullCodeName)
        {
            this.FullCodeName = fullCodeName;
        }

        public string FullCodeName
        {
            get;
            set;
        }
    }
}
