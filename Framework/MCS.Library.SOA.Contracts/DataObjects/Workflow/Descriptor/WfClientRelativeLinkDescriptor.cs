using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Serialization;
using MCS.Library.SOA.Contracts;

namespace MCS.Library.SOA.Contracts.DataObjects.Workflow
{
  
    [DataContract(IsReference = true)]
	public class WfClientRelativeLinkDescriptor : WfClientKeyedDescriptorBase
	{
        [DataMember]
        public string Url
        {
            get;
            set;
        }
        [DataMember]
        public string Category
        {
            get;
            set;
        }
	}
  [CollectionDataContract(IsReference=true)]
    public class WfClientRelativeLinkDescriptorCollection : WfClientKeyedDescriptorCollectionBase<string, WfClientRelativeLinkDescriptor>
    {
        public WfClientRelativeLinkDescriptorCollection(WfClientKeyedDescriptorBase owner):base(owner)
        {
           
        }
        public WfClientRelativeLinkDescriptorCollection() { }
        protected override string GetKeyForItem(WfClientRelativeLinkDescriptor item)
        {
            return item.Key;
        }
    }
	
	

}
