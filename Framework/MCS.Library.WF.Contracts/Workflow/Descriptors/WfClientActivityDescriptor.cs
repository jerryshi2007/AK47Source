using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;
using System.Web.Script.Serialization;
using MCS.Library.Core;
using MCS.Library.Globalization;
using MCS.Library.WF.Contracts.Common;

namespace MCS.Library.WF.Contracts.Workflow.Descriptors
{
    [DataContract]
    [Serializable]
    public class WfClientActivityDescriptor : WfClientKeyedDescriptorBase
    {
        private WfClientVariableDescriptorCollection _Variables = null;
        private WfClientConditionDescriptor _Condition = null;
        private WfClientResourceDescriptorCollection _Resources = null;
        private WfClientToTransitionDescriptorCollection _ToTransitions = null;
        private WfClientBranchProcessTemplateCollection _BranchProcessTemplates = null;
        private WfClientRelativeLinkDescriptorCollection _RelativeLinks = null;

        private WfClientProcessDescriptor _Process = null;

        public WfClientActivityDescriptor()
        {
        }

        public WfClientActivityDescriptor(WfClientActivityType activityType)
        {
            this.ActivityType = activityType;
        }

        public WfClientActivityType ActivityType
        {
            get;
            set;
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

        public WfClientVariableDescriptorCollection Variables
        {
            get
            {
                if (this._Variables == null)
                    this._Variables = new WfClientVariableDescriptorCollection();

                return this._Variables;
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

        public WfClientRelativeLinkDescriptorCollection RelativeLinks
        {
            get
            {
                if (this._RelativeLinks == null)
                    this._RelativeLinks = new WfClientRelativeLinkDescriptorCollection();

                return this._RelativeLinks;
            }
        }

        public WfClientToTransitionDescriptorCollection ToTransitions
        {
            get
            {
                if (this._ToTransitions == null)
                    this._ToTransitions = new WfClientToTransitionDescriptorCollection(this);

                return this._ToTransitions;
            }
        }

        public WfClientBranchProcessTemplateCollection BranchProcessTemplates
        {
            get
            {
                if (this._BranchProcessTemplates == null)
                    this._BranchProcessTemplates = new WfClientBranchProcessTemplateCollection();

                return this._BranchProcessTemplates;
            }
        }

        [ScriptIgnore]
        public WfClientProcessDescriptor Process
        {
            get
            {
                return this._Process;
            }
            set
            {
                this._Process = value;
            }
        }

        public WfClientFromTransitionDescriptorCollection GetFromTransitions()
        {
            WfClientFromTransitionDescriptorCollection result = new WfClientFromTransitionDescriptorCollection();

            if (this._Process != null)
                this._Process.FillTransitionsByToActivityKey(result, this.Key);

            return result;
        }
    }

    [DataContract]
    [Serializable]
    public class WfClientActivityDescriptorCollection : WfClientKeyedDescriptorCollectionBase<WfClientActivityDescriptor>
    {
        private WfClientProcessDescriptor _Process = null;
        private WfClientActivityDescriptor _InitialActivity = null;
        private WfClientActivityDescriptor _CompletedActivity = null;

        public WfClientActivityDescriptorCollection()
        {
        }

        public WfClientActivityDescriptorCollection(WfClientProcessDescriptor process)
        {
            this._Process = process;
        }

        protected WfClientActivityDescriptorCollection(SerializationInfo info, StreamingContext context) :
            base(info, context)
        {
        }

        [ScriptIgnore]
        public WfClientProcessDescriptor Process
        {
            get
            {
                return _Process;
            }
        }

        [ScriptIgnore]
        internal WfClientActivityDescriptor InitialActivity
        {
            get
            {
                return this._InitialActivity;
            }
            set
            {
                this._InitialActivity = value;
            }
        }

        [ScriptIgnore]
        internal WfClientActivityDescriptor CompletedActivity
        {
            get
            {
                return this._CompletedActivity;
            }
            set
            {
                this._CompletedActivity = value;
            }
        }

        protected override void OnInsert(int index, object value)
        {
            WfClientActivityDescriptor actDesp = (WfClientActivityDescriptor)value;

            ValidateActivity(actDesp);

            actDesp.Process = _Process;

            switch (actDesp.ActivityType)
            {
                case WfClientActivityType.InitialActivity:
                    this._InitialActivity = actDesp;
                    break;
                case WfClientActivityType.CompletedActivity:
                    this._CompletedActivity = actDesp;
                    break;
            }

            base.OnInsert(index, value);
        }

        protected override void OnRemoveComplete(int index, object value)
        {
            WfClientActivityDescriptor actDesp = (WfClientActivityDescriptor)value;

            base.OnRemoveComplete(index, value);

            switch (actDesp.ActivityType)
            {
                case WfClientActivityType.InitialActivity:
                    this._InitialActivity = null;
                    break;
                case WfClientActivityType.CompletedActivity:
                    this._CompletedActivity = null;
                    break;
            }
        }

        private void ValidateActivity(WfClientActivityDescriptor item)
        {
            switch (item.ActivityType)
            {
                case WfClientActivityType.InitialActivity:
                    CheckActivityType(WfClientActivityType.InitialActivity, "一个流程只能有一个开始活动");
                    break;
                case WfClientActivityType.CompletedActivity:
                    CheckActivityType(WfClientActivityType.CompletedActivity, "一个流程只能有一个结束活动");
                    break;
            }
        }

        private void CheckActivityType(WfClientActivityType activityType, string errorMessage)
        {
            foreach (WfClientActivityDescriptor act in base.List)
            {
                (act.ActivityType == activityType).TrueThrow(Translator.Translate(Define.DefaultCulture, errorMessage));
            }
        }
    }
}
