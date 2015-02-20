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
    public abstract class WfClientActivityResourceDescriptorBase : WfClientResourceDescriptor
    {
        public WfClientActivityResourceDescriptorBase()
        {
        }

        public WfClientActivityResourceDescriptorBase(string actKey)
        {
            this.ActivityKey = actKey;
        }

        // <summary>
        /// 流程环节的Key
        /// </summary>
        public string ActivityKey
        {
            get;
            set;
        }
    }
}
