using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Serialization;
using MCS.Library.SOA.Contracts.DataObjects;

namespace MCS.Library.SOA.Contracts.DataObjects.Workflow
{
    [DataContract(IsReference=true)]
    [Serializable]
    public class WfClientBranchProcessGroup
    {
        private WfClientActivity _OwnerActivity = null;
        private string _OwnerActivityID = null;

        public string OwnerActivityID
        {
            get { return _OwnerActivityID; }
            set { _OwnerActivityID = value; }
        }
        private WfClientBranchProcessTemplateDescriptor _ProcessTemplate =null;
      
        public WfClientBranchProcessGroup()
        {

        }

        public WfClientBranchProcessGroup(WfClientActivity owner, WfClientBranchProcessTemplateDescriptor template)
        {
            owner.NullCheck("owner");

            this.OwnerActivity = owner;
            this.ProcessTemplate = template;
        }

        #region IWfBranchProcessGroup Members

        [DataMember]
        public WfClientBranchProcessTemplateDescriptor ProcessTemplate
        {
            get
            {
                if (this._ProcessTemplate == null)
                {
                    this._ProcessTemplate = new WfClientBranchProcessTemplateDescriptor();
                }
                return this._ProcessTemplate;
            }
            set { this._ProcessTemplate = value; }
        }
        [DataMember]
        public WfClientActivity OwnerActivity
        {
            get
            {
                return this._OwnerActivity;
            }
            set
            {
                this._OwnerActivity = value;

 
            }
        }
      
        [DataMember]
        public ClientDataLoadingType LoadingType
        {
            set;
            get;
        }
        #endregion
    }

    [CollectionDataContract(IsReference = true)]
    public class WfClientBranchProcessGroupCollection : EditableKeyedDataObjectCollectionBase<string, WfClientBranchProcessGroup>
    {
        protected override string GetKeyForItem(WfClientBranchProcessGroup item)
        {
            return item.ProcessTemplate.Key;
        }

      

    }
}
