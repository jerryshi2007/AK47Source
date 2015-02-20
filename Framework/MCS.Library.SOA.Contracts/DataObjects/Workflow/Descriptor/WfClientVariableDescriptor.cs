using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Serialization;
using MCS.Library.SOA.Contracts;

namespace MCS.Library.SOA.Contracts.DataObjects.Workflow
{
    [DataContract(IsReference = true)]
    [Serializable]
	public class WfClientVariableDescriptor : WfClientKeyedDescriptorBase
	{
        [DataMember]
        public ClientDataType OriginalType
        {
            get;
            set;
        }
        [DataMember]
        public string OriginalValue
        {
            get;
            set;
        }
       
        [DataMember]
        public object ActualValue
        {
            get;
            set;
        }

	}
   [CollectionDataContract(IsReference=true)]
    public class WfClientVariableDescriptorCollection : WfClientKeyedDescriptorCollectionBase<string, WfClientVariableDescriptor>
	{
        public WfClientVariableDescriptorCollection(WfClientKeyedDescriptorBase owner)
            : base(owner)
        {
           
        }
        public WfClientVariableDescriptorCollection() { }
		protected override string GetKeyForItem(WfClientVariableDescriptor item)
		{
			return item.Key;
		}
	}

}
