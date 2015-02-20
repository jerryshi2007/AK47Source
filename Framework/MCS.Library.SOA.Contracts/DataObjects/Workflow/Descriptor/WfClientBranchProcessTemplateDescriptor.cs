using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Serialization;

namespace MCS.Library.SOA.Contracts.DataObjects.Workflow
{
    [DataContract(IsReference = true)]
    [Serializable]
    public class WfClientBranchProcessTemplateDescriptor : WfClientKeyedDescriptorBase
	{
        
        private WfClientResourceDescriptorCollection _Resources;
        [DataMember]
        public string BranchProcessKey
        {
            get;
            set;
        }
       
        [DataMember]
        public WfClientBranchProcessExecuteSequence ExecuteSequence
        {
            get;
            set;
        }
        [DataMember]
        public WfClientBranchProcessBlockingType BlockingType
        {
            get;
            set;
        }
        [DataMember]
        public WfClientResourceDescriptorCollection Resources
        {
            get
            {
                if (this._Resources == null)
                    this._Resources = new WfClientResourceDescriptorCollection();

                return _Resources;
            }
            set
            {
                _Resources = value;
            }
        }
	}
   [CollectionDataContract(IsReference=true)]
    public class WfClientBranchProcessTemplateDescriptorCollection : WfClientKeyedDescriptorCollectionBase<string, WfClientBranchProcessTemplateDescriptor>
    {
        public WfClientBranchProcessTemplateDescriptorCollection(WfClientKeyedDescriptorBase owner)
            : base(owner)
        {
           
        }
        public WfClientBranchProcessTemplateDescriptorCollection() { }
        protected override string GetKeyForItem(WfClientBranchProcessTemplateDescriptor item)
        {
            return item.Key;
        }
    }
}
