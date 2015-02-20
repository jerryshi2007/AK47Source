using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Serialization;
using System.Reflection;

namespace MCS.Library.SOA.Contracts.DataObjects.Workflow
{
    [KnownType(typeof(WfClientActivityDescriptor))]
    [KnownType(typeof(WfClientBranchProcessTemplateDescriptor))]
    [KnownType(typeof(WfClientProcessDescriptor))]    
    [KnownType(typeof(WfClientRelativeLinkDescriptor))]
    [KnownType(typeof(WfClientResourceDescriptor))]
    [KnownType(typeof(WfClientTransitionDescriptor))]
    [KnownType(typeof(WfClientVariableDescriptor))]
    [KnownType(typeof(WfClientActivityOperatorResourceDescriptor))]

 

    [DataContract(IsReference = true)]
	public abstract class WfClientKeyedDescriptorBase
	{
        private ClientPropertyValueCollection _Properties;
        private WfClientProcess _ProcessInstance;
        private string _Key = string.Empty;
        private string _Name = string.Empty;
        private string _Description = string.Empty;

        public WfClientKeyedDescriptorBase() { }
        [DataMember]
        public string Key
        {
            get { return this._Key; }
            set { this._Key = value; }
        }
        [DataMember]
        public string Name
        {
            get { return this._Name; }
            set { this._Name = value; }
        }
        [DataMember]
        public string Description
        {
            get { return this._Description; }
            set { this._Description = value; }
        }
        [DataMember]
        public bool Enabled { get; set; }
        [DataMember]
        public ClientPropertyValueCollection Properties
        {
            get
            {
                if (this._Properties == null)
                    this._Properties = new ClientPropertyValueCollection();

                return _Properties;
            }
            set
            {
                _Properties = value;
            }
        }
        [DataMember]
        public WfClientProcess ProcessInstance
        {
            get
            {
               
                  

                return _ProcessInstance;
            }
            set
            {
                _ProcessInstance = value;
            }
        }
          
	}


    [KnownType(typeof(WfClientActivityDescriptorCollection))]
    [KnownType(typeof(WfClientBranchProcessTemplateDescriptorCollection))]
    //[KnownType(typeof(WfClientProcessDescriptorCollection))]
    [KnownType(typeof(WfClientRelativeLinkDescriptorCollection))]
    [KnownType(typeof(WfClientResourceDescriptorCollection))]
    [KnownType(typeof(WfClientTransitionDescriptorCollection))]
    [KnownType(typeof(WfClientVariableDescriptorCollection))]
    //[KnownType(typeof(WfClientActivityOperatorResourceDescriptorCollection))]
    [CollectionDataContract(IsReference=true)]
    
    public abstract class WfClientKeyedDescriptorCollectionBase<TKey, TItem> : EditableKeyedDataObjectCollectionBase<TKey, TItem> 
    {
        private WfClientKeyedDescriptorBase _Owner;
       
       [DataMember]
        public  WfClientKeyedDescriptorBase Owner
        {
            get { return _Owner; }
            set { _Owner = value; }
        }
        public WfClientKeyedDescriptorCollectionBase(WfClientKeyedDescriptorBase owner)
        {
            _Owner = owner;
        }
        public WfClientKeyedDescriptorCollectionBase()
        {
            
        }
       
       
        

    }
}
