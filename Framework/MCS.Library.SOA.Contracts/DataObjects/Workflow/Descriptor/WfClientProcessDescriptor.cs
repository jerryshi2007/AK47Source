using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Serialization;
using MCS.Library.SOA.DataObjects.Workflow;

namespace MCS.Library.SOA.Contracts.DataObjects.Workflow
{
    [DataContract(IsReference = true)]
    public class WfClientProcessDescriptor : WfClientKeyedDescriptorBase
    {
        private WfClientVariableDescriptorCollection _Variables = null;
        private WfClientActivityDescriptorCollection _Activities = null;
        private WfClientRelativeLinkDescriptorCollection _RelativeLinks = null;
        private WfClientResourceDescriptorCollection _CancelEventReceivers = null;
        private WfClientResourceDescriptorCollection _InternalRelativeUsers = null;
        private WfClientExternalUserCollection _ExternalUsers = null;
        //[ThreadStatic]
        public static WfClientProcess StaticProcessInstance =null;
        //[ThreadStatic]
        public static WfProcess StaticServerProcessInstance = null;    
      

        private Dictionary<string, WfClientTransitionDescriptor> _Transitions = new Dictionary<string, WfClientTransitionDescriptor>();

        public Dictionary<string, WfClientTransitionDescriptor> Transitions
        {
            get { return _Transitions; }
            set { _Transitions = value; }
        }
        [DataMember]
        public string ApplicationName { get; set; }

        [DataMember]
        public bool AutoGenerateResourceUsers { get; set; }

        [DataMember]
        public string GraphDescription { get; set; }

        [DataMember]
        public WfClientProcessType ProcessType { get; set; }

        [DataMember]
        public string ProgramName { get; set; }

        [DataMember]
        public string Url { get; set; }

        [DataMember]
        public string Version { get; set; }

        [DataMember]
        public WfClientVariableDescriptorCollection Variables
        {
            get
            {
                if (this._Variables == null)
                    this._Variables = new WfClientVariableDescriptorCollection(this);

                return this._Variables;
            }
            set { this._Variables = value; }
        }

        [DataMember]
        public WfClientActivityDescriptorCollection Activities
        {
            get
            {
                if (this._Activities == null)
                    this._Activities = new WfClientActivityDescriptorCollection(this);

                return this._Activities;
            }
            set { this._Activities = value; }
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
            set { this._RelativeLinks = value; }
        }

        [DataMember]
        public WfClientResourceDescriptorCollection CancelEventReceivers
        {
            get
            {
                if (this._CancelEventReceivers == null)
                    this._CancelEventReceivers = new WfClientResourceDescriptorCollection();

                return this._CancelEventReceivers;
            }
            set { this._CancelEventReceivers = value; }
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
            set { this._InternalRelativeUsers = value; }
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
            set { this._ExternalUsers = value; }
        }

     

    }
}
