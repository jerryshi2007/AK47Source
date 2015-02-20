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
    public class WfClientBranchProcessTemplateDescriptor : WfClientKeyedDescriptorBase
    {
        private WfClientResourceDescriptorCollection _Resources = null;
        private WfClientResourceDescriptorCollection _CancelSubProcessNotifier = null;
        private WfClientConditionDescriptor _Condition = null;
        private WfClientRelativeLinkDescriptorCollection _RelativeLinks = null;

        public WfClientBranchProcessTemplateDescriptor()
        {
        }

        public WfClientBranchProcessTemplateDescriptor(string key)
            : base(key)
        {
        }

        public string BranchProcessKey
        {
            get
            {
                return this.Properties.GetValue("BranchProcessKey", string.Empty);
            }
            set
            {
                this.Properties.AddOrSetValue("BranchProcessKey", value);
            }
        }

        public WfClientBranchProcessExecuteSequence ExecuteSequence
        {
            get
            {
                return this.Properties.GetValue("ExecuteSequence", WfClientBranchProcessExecuteSequence.Parallel);
            }
            set
            {
                this.Properties.AddOrSetValue("ExecuteSequence", value);
            }
        }

        public WfClientBranchProcessBlockingType BlockingType
        {
            get
            {
                return Properties.GetValue("BlockingType", WfClientBranchProcessBlockingType.WaitAllBranchProcessesComplete);
            }
            set
            {
                this.Properties.AddOrSetValue("BlockingType", value);
            }
        }

        public WfClientSubProcessApprovalMode SubProcessApprovalMode
        {
            get
            {
                return this.Properties.GetValue("SubProcessApprovalMode", WfClientSubProcessApprovalMode.NoActivityDecide);
            }
            set
            {
                this.Properties.AddOrSetValue("SubProcessApprovalMode", value);
            }
        }

        public string DefaultProcessName
        {
            get
            {
                return this.Properties.GetValue("DefaultProcessName", string.Empty);
            }
            set
            {
                this.Properties.AddOrSetValue("DefaultProcessName", value);
            }
        }

        public string DefaultUrl
        {
            get
            {
                return this.Properties.GetValue("DefaultUrl", string.Empty);
            }
            set
            {
                this.Properties.AddOrSetValue("DefaultUrl", value);
            }
        }

        public string DefaultTaskTitle
        {
            get
            {
                return Properties.GetValue("DefaultTaskTitle", string.Empty);
            }
            set
            {
                Properties.AddOrSetValue("DefaultTaskTitle", value);
            }
        }

        public WfClientResourceDescriptorCollection Resources
        {
            get
            {
                if (this._Resources == null)
                    this._Resources = new WfClientResourceDescriptorCollection();

                return this._Resources;
            }
        }

        public WfClientResourceDescriptorCollection CancelSubProcessNotifier
        {
            get
            {
                if (this._CancelSubProcessNotifier == null)
                    this._CancelSubProcessNotifier = new WfClientResourceDescriptorCollection();

                return this._CancelSubProcessNotifier;
            }
        }

        public WfClientRelativeLinkDescriptorCollection RelativeLinks
        {
            get
            {
                if (this._RelativeLinks == null)
                    this._RelativeLinks = new WfClientRelativeLinkDescriptorCollection();

                return this._RelativeLinks;
            }
        }

        public WfClientConditionDescriptor Condition
        {
            get
            {
                if (this._Condition == null)
                    this._Condition = new WfClientConditionDescriptor();

                return this._Condition;
            }
            set
            {
                this._Condition = value;
            }
        }

        //TODO: 缺RelativeLinks和OperationDefinition属性
    }

    [DataContract]
    [Serializable]
    public class WfClientBranchProcessTemplateCollection : WfClientKeyedDescriptorCollectionBase<WfClientBranchProcessTemplateDescriptor>
    {
        public WfClientBranchProcessTemplateCollection()
        {
        }

        protected WfClientBranchProcessTemplateCollection(SerializationInfo info, StreamingContext context) :
            base(info, context)
        {
        }
    }
}
