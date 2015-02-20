using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using System.Web.Script.Serialization;

namespace MCS.Library.WF.Contracts.Workflow.Descriptors
{
    [DataContract]
    [Serializable]
    public class WfClientProcessDescriptor : WfClientKeyedDescriptorBase
    {
        private WfClientVariableDescriptorCollection _Variables = null;
        private WfClientActivityDescriptorCollection _Activities = null;
        private WfClientRelativeLinkDescriptorCollection _RelativeLinks = null;

        public string ApplicationName
        {
            get { return this.Properties.GetValue("ApplicationName", string.Empty); }
            set { this.Properties.AddOrSetValue("ApplicationName", value); }
        }

        public string ProgramName
        {
            get { return this.Properties.GetValue("ProgramName", string.Empty); }
            set { this.Properties.AddOrSetValue("ProgramName", value); }
        }

        public string Url
        {
            get { return this.Properties.GetValue("Url", string.Empty); }
            set { this.Properties.AddOrSetValue("Url", value); }
        }

        public bool AutoGenerateResourceUsers
        {
            get { return this.Properties.GetValue("AutoGenerateResourceUsers", true); }
            set { this.Properties.AddOrSetValue("AutoGenerateResourceUsers", value); }
        }

        public WfClientProcessType ProcessType
        {
            get { return this.Properties.GetValue("ProcessType", WfClientProcessType.Approval); }
            set { this.Properties.AddOrSetValue("ProcessType", value); }
        }

        public string DefaultTaskTitle
        {
            get { return this.Properties.GetValue("DefaultTaskTitle", string.Empty); }
            set { this.Properties.AddOrSetValue("DefaultTaskTitle", value); }
        }

        public string DefaultNotifyTaskTitle
        {
            get { return this.Properties.GetValue("DefaultNotifyTaskTitle", string.Empty); }
            set { this.Properties.AddOrSetValue("DefaultNotifyTaskTitle", value); }
        }

        public bool DefaultReturnValue
        {
            get { return this.Properties.GetValue("DefaultReturnValue", false); }
            set { this.Properties.AddOrSetValue("DefaultReturnValue", value); }
        }

        [ScriptIgnore]
        public WfClientActivityDescriptor InitialActivity
        {
            get
            {
                return this.Activities.InitialActivity;
            }
        }

        [ScriptIgnore]
        public WfClientActivityDescriptor CompletedActivity
        {
            get
            {
                return this.Activities.CompletedActivity;
            }
        }

        public WfClientActivityDescriptorCollection Activities
        {
            get
            {
                if (this._Activities == null)
                    this._Activities = new WfClientActivityDescriptorCollection(this);

                return this._Activities;
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

        public WfClientVariableDescriptorCollection Variables
        {
            get
            {
                if (this._Variables == null)
                    this._Variables = new WfClientVariableDescriptorCollection();

                return this._Variables;
            }
        }

        public string FindNotUsedActivityKey()
        {
            return FindNotUsedActivityKey("N");
        }

        public string FindNotUsedActivityKey(string Prefix)
        {
            int keyNum = 0;

            for (int i = 0; i < this.Activities.Count; i++)
            {
                string activityKey = this.Activities[i].Key.Replace(Prefix, "");
                int parseResult = 0;

                if (int.TryParse(activityKey, out parseResult) == false)
                    continue;

                if (keyNum < parseResult)
                    keyNum = parseResult;
            }

            return Prefix + ++keyNum;
        }

        public string FindNotUsedTransitionKey()
        {
            int keyNum = 0;
            int parseResult = 0;

            for (int i = 0; i < this.Activities.Count; i++)
            {
                foreach (WfClientTransitionDescriptor tran in this.Activities[i].ToTransitions)
                {
                    string tranKey = tran.Key.Replace("L", "");

                    if (int.TryParse(tranKey, out parseResult) == false)
                        continue;

                    if (keyNum < parseResult)
                        keyNum = parseResult;
                }
            }

            return "L" + ++keyNum;
        }

        internal void FillTransitionsByToActivityKey(WfClientFromTransitionDescriptorCollection result, string actKey)
        {
            foreach (WfClientActivityDescriptor actDesp in this.Activities)
            {
                WfClientTransitionDescriptor transition = actDesp.ToTransitions.Find(t => t.ToActivityKey == actKey);

                if (transition != null)
                    result.Add(transition);
            }
        }

        /// <summary>
        /// 反序列化后重新设置线的属性
        /// </summary>
        public void NormalizeAllTransitions()
        {
            foreach (WfClientActivityDescriptor actDesp in this.Activities)
            {
                foreach (WfClientTransitionDescriptor transition in actDesp.ToTransitions)
                {
                    if (transition.ToActivity == null)
                    {
                        if (this.Activities.ContainsKey(transition.ToActivityKey))
                            transition.ToActivity = this.Activities[transition.ToActivityKey];
                    }
                }
            }
        }
    }
}
