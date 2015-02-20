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
    public class WfClientProcessInfoJsonConverter : WfClientProcessInfoJsonConverterBase
    {
        private static readonly Type[] _SupportedTypes = new Type[] { typeof(WfClientProcessInfo) };

        public override object Deserialize(IDictionary<string, object> dictionary, Type type, JavaScriptSerializer serializer)
        {
            WfClientProcessInfo processInfo = (WfClientProcessInfo)base.Deserialize(dictionary, type, serializer);

            processInfo.CurrentActivity = JSONSerializerExecute.Deserialize<WfClientActivity>(dictionary.GetValue("currentActivity", (WfClientActivity)null));
            processInfo.PreviousActivity = JSONSerializerExecute.Deserialize<WfClientActivity>(dictionary.GetValue("previousActivity", (WfClientActivity)null));

            JSONSerializerExecute.FillDeserializedCollection(dictionary.GetValue("nextActivities", (object)null), processInfo.NextActivities);

            return processInfo;
        }

        public override IDictionary<string, object> Serialize(object obj, JavaScriptSerializer serializer)
        {
            IDictionary<string, object> dictionary = base.Serialize(obj, serializer);

            WfClientProcessInfo processInfo = (WfClientProcessInfo)obj;

            dictionary.AddNonDefaultValue("currentActivity", processInfo.CurrentActivity);
            dictionary.AddNonDefaultValue("previousActivity", processInfo.PreviousActivity);
            dictionary.AddNonDefaultValue("nextActivities", processInfo.NextActivities);

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
            return new WfClientProcessInfo();
        }
    }
}
