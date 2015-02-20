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
    public class WfClientActivityJsonConverter : JavaScriptConverter
    {
        private static readonly Type[] _SupportedTypes = new Type[] { typeof(WfClientActivity) };

        public override object Deserialize(IDictionary<string, object> dictionary, Type type, JavaScriptSerializer serializer)
        {
            WfClientActivity activity = new WfClientActivity();

            activity.ID = dictionary.GetValue("id", string.Empty);
            activity.DescriptorKey = dictionary.GetValue("descriptorKey", string.Empty);
            activity.StartTime = dictionary.GetValue("startTime", DateTime.MinValue);
            activity.EndTime = dictionary.GetValue("endTime", DateTime.MinValue);
            activity.LoadingType = dictionary.GetValue("loadingType", WfClientDataLoadingType.Memory);
            activity.MainStreamActivityKey = dictionary.GetValue("mainStreamActivityKey", string.Empty);
            activity.BranchProcessReturnValue = dictionary.GetValue("branchProcessReturnValue", WfClientBranchProcessReturnType.AllFalse);
            activity.BranchProcessGroupsCount = dictionary.GetValue("branchProcessGroupsCount", 0);
            activity.Operator = JSONSerializerExecute.Deserialize<WfClientUser>(dictionary.GetValue("operator", (object)null));
            activity.Status = dictionary.GetValue("status", WfClientActivityStatus.NotRunning);

            JSONSerializerExecute.FillDeserializedCollection(dictionary.GetValue("assignees", (object)null), activity.Assignees);
            JSONSerializerExecute.FillDeserializedCollection(dictionary.GetValue("candidates", (object)null), activity.Candidates);

            activity.Descriptor = JSONSerializerExecute.Deserialize<WfClientActivityDescriptor>(dictionary.GetValue("descriptor", (object)null));

            return activity;
        }

        public override IDictionary<string, object> Serialize(object obj, JavaScriptSerializer serializer)
        {
            WfClientActivity activity = (WfClientActivity)obj;

            Dictionary<string, object> dictionary = new Dictionary<string, object>();

            dictionary.AddNonDefaultValue("id", activity.ID);
            dictionary.AddNonDefaultValue("descriptorKey", activity.DescriptorKey);
            dictionary.AddNonDefaultValue("startTime", activity.StartTime);
            dictionary.AddNonDefaultValue("endTime", activity.EndTime);
            dictionary.AddNonDefaultValue("loadingType", activity.LoadingType);
            dictionary.AddNonDefaultValue("mainStreamActivityKey", activity.MainStreamActivityKey);
            dictionary.AddNonDefaultValue("operator", activity.Operator);
            dictionary.AddNonDefaultValue("status", activity.Status);
            dictionary.Add("assignees", activity.Assignees);
            dictionary.Add("candidates", activity.Candidates);
            dictionary.Add("branchProcessReturnValue", activity.BranchProcessReturnValue);
            dictionary.Add("branchProcessGroupsCount", activity.BranchProcessGroupsCount);
            dictionary.AddNonDefaultValue("descriptor", activity.Descriptor);

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
