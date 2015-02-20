using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;
using MCS.Library.Data.DataObjects;

namespace MCS.Library.WF.Contracts.Workflow.Descriptors
{
    [Serializable]
    [DataContract]
    public abstract class WfClientKeyedDescriptorCollectionBase<T> : SerializableEditableKeyedDataObjectCollectionBase<string, T> where T : WfClientKeyedDescriptorBase
    {
        public WfClientKeyedDescriptorCollectionBase()
        {
        }

        protected WfClientKeyedDescriptorCollectionBase(SerializationInfo info, StreamingContext context) :
            base(info, context)
        {
        }

        protected override string GetKeyForItem(T item)
        {
            return item.Key;
        }
    }
}
