using MCS.Library.Core;
using MCS.Library.WF.Contracts.Workflow.Descriptors;
using MCS.Web.Library.Script;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Script.Serialization;

namespace MCS.Library.WF.Contracts.Json.Converters
{
    public class WfClientActivityDescriptorJsonConverter : WfClientKeyedDescriptorJsonConverterBase
    {
        private static readonly Type[] _SupportedTypes = new Type[] { typeof(WfClientActivityDescriptor) };

        public override object Deserialize(IDictionary<string, object> dictionary, Type type, JavaScriptSerializer serializer)
        {
            WfClientActivityDescriptor actDesp = (WfClientActivityDescriptor)base.Deserialize(dictionary, type, serializer);

            actDesp.Condition = JSONSerializerExecute.Deserialize<WfClientConditionDescriptor>(dictionary.GetValue("condition", (object)null));

            JSONSerializerExecute.FillDeserializedCollection<WfClientVariableDescriptor>(dictionary.GetValue("variables", (object)null), actDesp.Variables);
            JSONSerializerExecute.FillDeserializedCollection<WfClientResourceDescriptor>(dictionary.GetValue("resources", (object)null), actDesp.Resources);

            JSONSerializerExecute.FillDeserializedCollection<WfClientResourceDescriptor>(dictionary.GetValue("enterEventReceivers", (object)null), actDesp.EnterEventReceivers);
            JSONSerializerExecute.FillDeserializedCollection<WfClientResourceDescriptor>(dictionary.GetValue("leaveEventReceivers", (object)null), actDesp.LeaveEventReceivers);

            JSONSerializerExecute.FillDeserializedCollection<WfClientTransitionDescriptor>(dictionary.GetValue("transition", (object)null), actDesp.ToTransitions);
            JSONSerializerExecute.FillDeserializedCollection<WfClientBranchProcessTemplateDescriptor>(dictionary.GetValue("branchProcessTemplates", (object)null), actDesp.BranchProcessTemplates);
            JSONSerializerExecute.FillDeserializedCollection<WfClientRelativeLinkDescriptor>(dictionary.GetValue("relativeLinks", (object)null), actDesp.RelativeLinks);

            return actDesp;
        }

        public override IDictionary<string, object> Serialize(object obj, JavaScriptSerializer serializer)
        {
            Dictionary<string, object> dictionary = (Dictionary<string, object>)base.Serialize(obj, serializer);
            WfClientActivityDescriptor actDesp = (WfClientActivityDescriptor)obj;

            dictionary["activityType"] = actDesp.ActivityType;
            dictionary.AddNonDefaultValue("condition", actDesp.Condition);

            dictionary.Add("variables", actDesp.Variables);
            dictionary.Add("resources", actDesp.Resources);
            dictionary.Add("enterEventReceivers", actDesp.EnterEventReceivers);
            dictionary.Add("leaveEventReceivers", actDesp.LeaveEventReceivers);
            dictionary.Add("transition", actDesp.ToTransitions);
            dictionary.Add("branchProcessTemplates", actDesp.BranchProcessTemplates);
            dictionary.Add("relativeLinks", actDesp.RelativeLinks);

            return dictionary;
        }

        protected override WfClientKeyedDescriptorBase CreateInstance(string key, IDictionary<string, object> dictionary, Type type, JavaScriptSerializer serializer)
        {
            WfClientActivityType activityType = DictionaryHelper.GetValue(dictionary, "activityType", WfClientActivityType.NormalActivity);

            return new WfClientActivityDescriptor(activityType) { Key = key };
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
