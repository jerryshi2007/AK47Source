using MCS.Library.Core;
using MCS.Library.WF.Contracts.Ogu;
using MCS.Library.WF.Contracts.Workflow.Descriptors;
using MCS.Library.WF.Contracts.Workflow.Runtime;
using MCS.Web.Library.Script;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Script.Serialization;

namespace MCS.Library.WF.Contracts.Json.Converters
{
    public class WfClientMainStreamActivityDescriptorJsonConverter : JavaScriptConverter
    {
        private static readonly Type[] _SupportedTypes = new Type[] { typeof(WfClientMainStreamActivityDescriptor) };

        public override object Deserialize(IDictionary<string, object> dictionary, Type type, JavaScriptSerializer serializer)
        {
            WfClientMainStreamActivityDescriptor result = new WfClientMainStreamActivityDescriptor();

            result.Activity = JSONSerializerExecute.Deserialize<WfClientActivityDescriptor>(dictionary.GetValue("activity", (object)null));
            result.ActivityInstanceID = dictionary.GetValue("activityInstanceID", string.Empty);
            result.Level = dictionary.GetValue("level", 0);
            result.Elapsed = dictionary.GetValue("elapsed", false);
            result.BranchProcessGroupsCount = dictionary.GetValue("branchProcessGroupsCount", 0);
            result.Status = dictionary.GetValue("status", WfClientActivityStatus.NotRunning);
            result.Operator = JSONSerializerExecute.Deserialize<WfClientUser>(dictionary.GetValue("operator", (object)null));
            result.FromTransitionDescriptor = JSONSerializerExecute.Deserialize<WfClientTransitionDescriptor>(dictionary.GetValue("fromTransitionDescriptor", (object)null));
            result.ToTransitionDescriptor = JSONSerializerExecute.Deserialize<WfClientTransitionDescriptor>(dictionary.GetValue("toTransitionDescriptor", (object)null));

            JSONSerializerExecute.FillDeserializedCollection(dictionary.GetValue("assignees", (object)null), result.Assignees);
            JSONSerializerExecute.FillDeserializedCollection(dictionary.GetValue("associatedActivities", (object)null), result.AssociatedActivities);

            return result;
        }

        public override IDictionary<string, object> Serialize(object obj, JavaScriptSerializer serializer)
        {
            WfClientMainStreamActivityDescriptor clientMSActDesp = (WfClientMainStreamActivityDescriptor)obj;

            Dictionary<string, object> dictionary = new Dictionary<string, object>();

            dictionary.AddNonDefaultValue("activityInstanceID", clientMSActDesp.ActivityInstanceID);
            dictionary.AddNonDefaultValue("level", clientMSActDesp.Level);
            dictionary.AddNonDefaultValue("elapsed", clientMSActDesp.Elapsed);
            dictionary.AddNonDefaultValue("activity", clientMSActDesp.Activity);
            dictionary.AddNonDefaultValue("branchProcessGroupsCount", clientMSActDesp.BranchProcessGroupsCount);
            dictionary["status"] = clientMSActDesp.Status;
            dictionary["operator"] = clientMSActDesp.Operator;

            dictionary.Add("fromTransitionDescriptor", clientMSActDesp.FromTransitionDescriptor);
            dictionary.Add("toTransitionDescriptor", clientMSActDesp.ToTransitionDescriptor);
            dictionary.Add("assignees", clientMSActDesp.Assignees);
            dictionary.Add("associatedActivities", clientMSActDesp.AssociatedActivities);

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
