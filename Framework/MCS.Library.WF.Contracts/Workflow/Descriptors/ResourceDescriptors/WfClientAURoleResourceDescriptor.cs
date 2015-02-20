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
    public class WfClientAURoleResourceDescriptor : WfClientResourceDescriptor
    {
        public WfClientAURoleResourceDescriptor()
        {
        }

        public WfClientAURoleResourceDescriptor(string fullCodeName)
        {
        }

        public string FullCodeName
        {
            get;
            set;
        }
    }
}
