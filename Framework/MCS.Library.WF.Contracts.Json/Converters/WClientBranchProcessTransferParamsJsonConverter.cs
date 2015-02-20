
using MCS.Library.Core;
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
    public class WClientBranchProcessTransferParamsJsonConverter : JavaScriptConverter
    {
        private static readonly Type[] _SupportedTypes = new Type[] { typeof(WfClientBranchProcessTransferParams) };

        public override object Deserialize(IDictionary<string, object> dictionary, Type type, JavaScriptSerializer serializer)
        {
            WfClientBranchProcessTransferParams transferParams = new WfClientBranchProcessTransferParams();

            transferParams.Template = JSONSerializerExecute.Deserialize<WfClientBranchProcessTemplateDescriptor>(dictionary.GetValue("template", (object)null));

            JSONSerializerExecute.FillDeserializedCollection(dictionary["branchParams"], transferParams.BranchParams);

            return transferParams;
        }

        public override IDictionary<string, object> Serialize(object obj, JavaScriptSerializer serializer)
        {
            Dictionary<string, object> dictionary = new Dictionary<string, object>();

            WfClientBranchProcessTransferParams startupParams = (WfClientBranchProcessTransferParams)obj;

            dictionary["template"] = startupParams.Template;
            dictionary["branchParams"] = startupParams.BranchParams;

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
