using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Serialization;

namespace MCS.Library.SOA.Contracts.DataObjects.Workflow
{
	[DataContract(IsReference=true)]
    [Serializable]
	public class WfClientActivityDescriptor : WfClientKeyedDescriptorBase
	{
		private WfClientTransitionDescriptorCollection _FromTransitions = null;
		private WfClientTransitionDescriptorCollection _ToTransitions = null;
        private WfClientVariableDescriptorCollection _Variables = null;
		private WfClientResourceDescriptorCollection _Resources = null;
        private WfClientConditionDescriptor _Condition = null;
        private WfClientProcessDescriptor _Process = null;
        
        private WfClientBranchProcessTemplateDescriptorCollection _BranchProcessTemplates = null;
        private WfClientRelativeLinkDescriptorCollection _RelativeLinks = null;
        private WfClientResourceDescriptorCollection _EnterEventReceivers = null;
        private WfClientResourceDescriptorCollection _LeaveEventReceivers = null;
        private WfClientResourceDescriptorCollection _InternalRelativeUsers = null;
        private WfClientExternalUserCollection _ExternalUsers = null;

        public static WfClientActivityDescriptor _DescriptorInstance = null;
       
        private WfClientActivity _Instance = null;

        [DataMember]
        public WfClientActivity Instance
        {
            get { return _Instance; }
            set { _Instance = value; }
        }

        [DataMember]
        public string CodeName { get; set; }

        [DataMember]
        public string ProcessKey { get; set; }

        [DataMember]
        public string Url { get; set; }

        [DataMember]
        public string LevelName { get; set; }

        [DataMember]
        public string Scene { get; set; }

        [DataMember]
        public DateTime EstimateStartTime { get; set; }

        [DataMember]
        public DateTime EstimateEndTime { get; set; }

        [DataMember]
        public decimal EstimateDuration { get; set; }

        [DataMember]
        public WfClientActivityType ActivityType { get; set; }

       
        [DataMember]
        public WfClientResourceDescriptorCollection Resources
        {
            get
            {
                if (this._Resources == null)
                    this._Resources = new WfClientResourceDescriptorCollection();

                return this._Resources;
            }
            set { _Resources = value; }
        }

        [DataMember]
        public WfClientTransitionDescriptorCollection FromTransitions
        {
            get
            {
                if (this._FromTransitions == null)
                    this._FromTransitions = new WfClientTransitionDescriptorCollection(this);

                return this._FromTransitions;
            }
            set
            {
                _FromTransitions = value;
            }
        }

        [DataMember]
        public WfClientTransitionDescriptorCollection ToTransitions
        {
            get
            {
                if (this._ToTransitions == null)
                    this._ToTransitions = new WfClientTransitionDescriptorCollection(this);

                return this._ToTransitions;
            }
            set { _ToTransitions = value; }
        }

        [DataMember]
        public WfClientVariableDescriptorCollection Variables
        {
            get
            {
                if (this._Variables == null)
                    this._Variables = new WfClientVariableDescriptorCollection(this);

                return this._Variables;
            }
            set { _Variables = value; }
        }

        [DataMember]
        public WfClientProcessDescriptor Process
        {
            get
            {
                if (this._Process == null)
                    this._Process = new WfClientProcessDescriptor();

                return this._Process;
            }
            set { _Process = value; }
        }

        [DataMember]
        public WfClientConditionDescriptor Condition
        {
            get
            {
                if (this._Condition == null)
                    this._Condition = new WfClientConditionDescriptor(this);

                return this._Condition;
            }
            set
            {
                _Condition = value;
            }
        }

        private string _AssociatedActivityKey = string.Empty;
        [DataMember]
        public string AssociatedActivityKey
        {
            get { return this._AssociatedActivityKey; }
            set { this._AssociatedActivityKey = value; }
        }

        [DataMember]
        public WfClientBranchProcessTemplateDescriptorCollection BranchProcessTemplates
        {
            get
            {
                if (this._BranchProcessTemplates == null)
                    this._BranchProcessTemplates = new WfClientBranchProcessTemplateDescriptorCollection(this);

                return _BranchProcessTemplates;
            }
            set { _BranchProcessTemplates = value; }
        }

        [DataMember]
        public WfClientRelativeLinkDescriptorCollection RelativeLinks
        {
            get
            {
                if (this._RelativeLinks == null)
                    this._RelativeLinks = new WfClientRelativeLinkDescriptorCollection(this);

                return this._RelativeLinks;
            }
            set { _RelativeLinks = value; }
        }

        [DataMember]
        public WfClientResourceDescriptorCollection EnterEventReceivers
        {
            get
            {
                if (this._EnterEventReceivers == null)
                    this._EnterEventReceivers = new WfClientResourceDescriptorCollection();

                return this._EnterEventReceivers;
            }
            set { this._EnterEventReceivers = value; }
        }

        [DataMember]
        public WfClientResourceDescriptorCollection LeaveEventReceivers
        {
            get
            {
                if (this._LeaveEventReceivers == null)
                    this._LeaveEventReceivers = new WfClientResourceDescriptorCollection();

                return this._LeaveEventReceivers;
            }
            set { this._LeaveEventReceivers = value; }
        }

        [DataMember]
        public WfClientResourceDescriptorCollection InternalRelativeUsers
        {
            get
            {
                if (this._InternalRelativeUsers == null)
                    this._InternalRelativeUsers = new WfClientResourceDescriptorCollection();

                return this._InternalRelativeUsers;
            }
            set { _InternalRelativeUsers = value; }
        }

        [DataMember]
        public WfClientExternalUserCollection ExternalUsers
        {
            get
            {
                if (this._ExternalUsers == null)
                    this._ExternalUsers = new WfClientExternalUserCollection(this);

                return this._ExternalUsers;
            }
            set { _ExternalUsers = value; }
        }
       
	}

	[CollectionDataContract(IsReference=true)]
	public class WfClientActivityDescriptorCollection : WfClientKeyedDescriptorCollectionBase<string, WfClientActivityDescriptor>
	{
        public WfClientActivityDescriptorCollection(WfClientKeyedDescriptorBase owner)
            : base(owner)
        { }
        public WfClientActivityDescriptorCollection()
        { }
       
		protected override string GetKeyForItem(WfClientActivityDescriptor item)
		{
			return item.Key;
		}
	}

}
