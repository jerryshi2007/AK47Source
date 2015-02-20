using MCS.Library.Core;
using MCS.Library.WF.Contracts.Workflow.DataObjects;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Script.Serialization;

namespace MCS.Library.WF.Contracts.Json.Converters.DataObjects
{
    public class WfClientNextStepJsonConverter : JavaScriptConverter
    {
        private static readonly Type[] _SupportedTypes = new Type[] { typeof(WfClientNextStep) };

        public override object Deserialize(IDictionary<string, object> dictionary, Type type, JavaScriptSerializer serializer)
        {
            WfClientNextStep nextStep = new WfClientNextStep();

            nextStep.TransitionKey = DictionaryHelper.GetValue(dictionary, "transitionKey", string.Empty);
            nextStep.TransitionName = DictionaryHelper.GetValue(dictionary, "transitionName", string.Empty);
            nextStep.TransitionDescription = DictionaryHelper.GetValue(dictionary, "transitionDescription", string.Empty);

            nextStep.ActivityKey = DictionaryHelper.GetValue(dictionary, "activityKey", string.Empty);
            nextStep.ActivityName = DictionaryHelper.GetValue(dictionary, "activityName", string.Empty);
            nextStep.ActivityDescription = DictionaryHelper.GetValue(dictionary, "activityDescription", string.Empty);

            return nextStep;
        }

        public override IDictionary<string, object> Serialize(object obj, JavaScriptSerializer serializer)
        {
            Dictionary<string, object> dictionary = new Dictionary<string, object>();

            WfClientNextStep nextStep = (WfClientNextStep)obj;

            DictionaryHelper.AddNonDefaultValue(dictionary, "transitionKey", nextStep.TransitionKey);
            DictionaryHelper.AddNonDefaultValue(dictionary, "transitionName", nextStep.TransitionName);
            DictionaryHelper.AddNonDefaultValue(dictionary, "transitionDescription", nextStep.TransitionDescription);

            DictionaryHelper.AddNonDefaultValue(dictionary, "activityKey", nextStep.ActivityKey);
            DictionaryHelper.AddNonDefaultValue(dictionary, "activityName", nextStep.ActivityName);
            DictionaryHelper.AddNonDefaultValue(dictionary, "activityDescription", nextStep.ActivityDescription);

            return dictionary;
        }

        public override IEnumerable<Type> SupportedTypes
        {
            get
            {
                return _SupportedTypes;
            }
        }
    }
}
