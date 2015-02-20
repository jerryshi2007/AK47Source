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
    public class WfClientProcessJsonConverter : WfClientProcessInfoJsonConverterBase
    {
        private static readonly Type[] _SupportedTypes = new Type[] { typeof(WfClientProcess) };

        public override object Deserialize(IDictionary<string, object> dictionary, Type type, JavaScriptSerializer serializer)
        {
            WfClientProcess process = (WfClientProcess)base.Deserialize(dictionary, type, serializer);

            JSONSerializerExecute.FillDeserializedCollection(dictionary.GetValue("activities", (object)null), process.Activities);

            process.NormalizeActivities();

            return process;
        }

        public override IDictionary<string, object> Serialize(object obj, JavaScriptSerializer serializer)
        {
            IDictionary<string, object> dictionary = base.Serialize(obj, serializer);

            WfClientProcess process = (WfClientProcess)obj;

            dictionary.AddNonDefaultValue("descriptor", process.Descriptor);
            dictionary.AddNonDefaultValue("mainStream", process.MainStream);
            dictionary.AddNonDefaultValue("activities", process.Activities);

            return dictionary;
        }

        public override IEnumerable<Type> SupportedTypes
        {
            get
            {
                return _SupportedTypes;
            }
        }

        protected override WfClientProcessInfoBase CreateInstance(IDictionary<string, object> dictionary, Type type, JavaScriptSerializer serializer)
        {
            WfClientProcessDescriptor processDesp = null;

            if (dictionary.ContainsKey("descriptor"))
                processDesp = JSONSerializerExecute.Deserialize<WfClientProcessDescriptor>(dictionary["descriptor"]);

            WfClientProcessDescriptor mainStream = null;

            if (dictionary.ContainsKey("mainStream"))
                mainStream = JSONSerializerExecute.Deserialize<WfClientProcessDescriptor>(dictionary["mainStream"]);

            return new WfClientProcess(processDesp, mainStream);
        }
    }
}
