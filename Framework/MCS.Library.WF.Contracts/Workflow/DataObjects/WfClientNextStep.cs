using MCS.Library.Core;
using MCS.Library.Data.DataObjects;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace MCS.Library.WF.Contracts.Workflow.DataObjects
{
    [DataContract]
    [Serializable]
    public class WfClientNextStep
    {
        public WfClientNextStep()
        {
        }

        public WfClientNextStep(XElement element)
        {
            element.NullCheck("element");

            this.TransitionKey = element.Attribute("transitionKey", string.Empty);
            this.TransitionName = element.Attribute("transitionName", string.Empty);
            this.TransitionDescription = element.Attribute("transitionDescription", string.Empty);

            this.ActivityKey = element.Attribute("activityKey", string.Empty);
            this.ActivityName = element.Attribute("activityName", string.Empty);
            this.ActivityDescription = element.Attribute("activityDescription", string.Empty);
        }

        public string TransitionKey
        {
            get;
            set;
        }

        public string TransitionName
        {
            get;
            set;
        }

        public string TransitionDescription
        {
            get;
            set;
        }

        public string ActivityKey
        {
            get;
            set;
        }

        public string ActivityName
        {
            get;
            set;
        }

        public string ActivityDescription
        {
            get;
            set;
        }

        public void ToXElement(XElement element)
        {
            element.NullCheck("element");

            element.SetAttributeValue("transitionKey", this.TransitionKey);
            element.SetAttributeValue("transitionName", this.TransitionName);
            element.SetAttributeValue("transitionDescription", this.TransitionDescription);

            element.SetAttributeValue("activityKey", this.ActivityKey);
            element.SetAttributeValue("activityName", this.ActivityName);
            element.SetAttributeValue("activityDescription", this.ActivityDescription);
        }
    }

    /// <summary>
    /// 
    /// </summary>
    public static class WfClientNextStepExtension
    {
        /// <summary>
        /// 获取下一步的描述
        /// </summary>
        /// <param name="nextStep"></param>
        /// <returns></returns>
        public static string GetDescription(this WfClientNextStep nextStep)
        {
            string result = string.Empty;

            if (nextStep != null)
            {
                result = nextStep.TransitionName;

                if (result.IsNullOrEmpty())
                    result = nextStep.TransitionDescription;

                if (result.IsNullOrEmpty())
                    result = nextStep.ActivityName;

                if (result.IsNullOrEmpty())
                    result = nextStep.ActivityDescription;
            }

            return result;
        }
    }

    [DataContract]
    [Serializable]
    public class WfClientNextStepCollection : SerializableEditableKeyedDataObjectCollectionBase<string, WfClientNextStep>
    {
        public WfClientNextStepCollection()
        {
        }

        public WfClientNextStepCollection(XElement element)
        {
            element.NullCheck("element");

            this.SelectedKey = element.Attribute("selected", string.Empty);

            foreach (XElement stepElement in element.Elements("Step"))
            {
                WfClientNextStep step = new WfClientNextStep(stepElement);

                this.Add(step);
            }
        }

        protected WfClientNextStepCollection(SerializationInfo info, StreamingContext context) :
            base(info, context)
        {
        }

        protected override string GetKeyForItem(WfClientNextStep item)
        {
            return item.TransitionKey;
        }

        public string SelectedKey
        {
            get;
            set;
        }

        /// <summary>
        /// 得到选择的步骤
        /// </summary>
        /// <returns></returns>
        public WfClientNextStep GetSelectedStep()
        {
            WfClientNextStep result = null;

            if (this.SelectedKey.IsNotEmpty())
            {
                if (this.ContainsKey(this.SelectedKey))
                    result = this[this.SelectedKey];
            }
            return result;
        }

        /// <summary>
        /// 转换为到Xml节点
        /// </summary>
        /// <param name="element"></param>
        public void ToXElement(XElement element)
        {
            element.NullCheck("element");

            foreach (WfClientNextStep step in this)
            {
                XElement stepNode = element.AddChildElement("Step");

                step.ToXElement(stepNode);
            }

            if (this.SelectedKey.IsNotEmpty())
                element.SetAttributeValue("selected", this.SelectedKey);
        }
    }
}
