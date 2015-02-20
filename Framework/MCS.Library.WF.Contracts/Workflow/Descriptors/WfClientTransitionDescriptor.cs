using MCS.Library.Data.DataObjects;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;
using System.Web.Script.Serialization;

namespace MCS.Library.WF.Contracts.Workflow.Descriptors
{
    [DataContract]
    [Serializable]
    public class WfClientTransitionDescriptor : WfClientKeyedDescriptorBase
    {
        private WfClientVariableDescriptorCollection _Variables = null;
        private WfClientConditionDescriptor _Condition = null;

        public WfClientTransitionDescriptor()
        {
        }

        public WfClientTransitionDescriptor(string key)
            : base(key)
        {
        }

        public WfClientTransitionDescriptor(string fromActKey, string toActKey)
        {
            this.FromActivityKey = fromActKey;
            this.ToActivityKey = toActKey;
        }

        public string FromActivityKey
        {
            get;
            set;
        }

        public string ToActivityKey
        {
            get;
            set;
        }

        [ScriptIgnore]
        public WfClientActivityDescriptor FromActivity
        {
            get;
            internal set;
        }

        [ScriptIgnore]
        public WfClientActivityDescriptor ToActivity
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
    }

    [DataContract]
    [Serializable]
    public class WfClientToTransitionDescriptorCollection : WfClientKeyedDescriptorCollectionBase<WfClientTransitionDescriptor>
    {
        private WfClientActivityDescriptor _FromActivity = null;

        public WfClientToTransitionDescriptorCollection()
        {
        }

        public WfClientToTransitionDescriptorCollection(WfClientActivityDescriptor fromAct)
        {
            this._FromActivity = fromAct;
        }

        public WfClientActivityDescriptor FromActivity
        {
            get
            {
                return this._FromActivity;
            }
        }

        protected override void OnInsert(int index, object value)
        {
			WfClientTransitionDescriptor transition = (WfClientTransitionDescriptor)value;

			transition.FromActivity = this.FromActivity;

            if (this.FromActivity != null)
            {
                if (this.FromActivity.Process != null)
                {
					if (this.FromActivity.Process.Activities.ContainsKey(transition.ToActivityKey))
						transition.ToActivity = this.FromActivity.Process.Activities[transition.ToActivityKey];
                }
            }

            base.OnInsert(index, value);
        }
    }

    [DataContract]
    [Serializable]
    public class WfClientFromTransitionDescriptorCollection : WfClientKeyedDescriptorCollectionBase<WfClientTransitionDescriptor>
    {
        public WfClientFromTransitionDescriptorCollection()
        {
        }
    }
}
