using MCS.Library.Core;
using MCS.Library.WF.Contracts.Ogu;
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
    public class WfClientTransferParamsJsonConverter : JavaScriptConverter
    {
        private static readonly Type[] _SupportedTypes = new Type[] { typeof(WfClientTransferParams) };

        public override object Deserialize(IDictionary<string, object> dictionary, Type type, JavaScriptSerializer serializer)
        {
            WfClientTransferParams transferParams = new WfClientTransferParams();

            transferParams.NextActivityDescriptorKey = dictionary.GetValue("nextActivityDescriptorKey", string.Empty);
            transferParams.FromTransitionDescriptorKey = dictionary.GetValue("fromTransitionDescriptorKey", string.Empty);
            transferParams.Operator = JSONSerializerExecute.Deserialize<WfClientUser>(dictionary.GetValue("operator", (object)null));

            JSONSerializerExecute.FillDeserializedCollection(dictionary["branchTransferParams"], transferParams.BranchTransferParams);
            JSONSerializerExecute.FillDeserializedCollection(dictionary["assignees"], transferParams.Assignees);

            return transferParams;
        }

        public override IDictionary<string, object> Serialize(object obj, JavaScriptSerializer serializer)
        {
            Dictionary<string, object> dictionary = new Dictionary<string, object>();

            WfClientTransferParams transferParams = (WfClientTransferParams)obj;

            dictionary.AddNonDefaultValue("nextActivityDescriptorKey", transferParams.NextActivityDescriptorKey);
            dictionary.AddNonDefaultValue("fromTransitionDescriptorKey", transferParams.FromTransitionDescriptorKey);
            dictionary["operator"] = transferParams.Operator;

            dictionary["branchTransferParams"] = transferParams.BranchTransferParams;
            dictionary["assignees"] = transferParams.Assignees;

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
