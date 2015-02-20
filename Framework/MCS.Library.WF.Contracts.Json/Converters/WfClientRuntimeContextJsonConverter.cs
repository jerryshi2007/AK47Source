using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Script.Serialization;
using MCS.Library.Core;
using MCS.Library.WF.Contracts.Ogu;
using MCS.Library.WF.Contracts.Workflow.DataObjects;
using MCS.Library.WF.Contracts.Workflow.Runtime;
using MCS.Web.Library.Script;

namespace MCS.Library.WF.Contracts.Json.Converters
{
    public class WfClientRuntimeContextJsonConverter : JavaScriptConverter
    {
        private static readonly Type[] _SupportedTypes = new Type[] { typeof(WfClientRuntimeContext) };

        public override object Deserialize(IDictionary<string, object> dictionary, Type type, JavaScriptSerializer serializer)
        {
            WfClientRuntimeContext context = new WfClientRuntimeContext();

            context.AutoCalculate = dictionary.GetValue("autoCalculate", false);
            context.TaskTitle = dictionary.GetValue("taskTitle", string.Empty);
            context.NotifyTitle = dictionary.GetValue("notifyTitle", string.Empty);
            context.Operator = JSONSerializerExecute.Deserialize<WfClientUser>(dictionary.GetValue("operator", (object)null));
            context.Opinion = JSONSerializerExecute.Deserialize<WfClientOpinion>(dictionary.GetValue("opinion", (object)null));
            context.AutoPersist = dictionary.GetValue("autoPersist", true);

            JSONSerializerExecute.FillDeserializedDictionary(dictionary, "applicationRuntimeParameters", context.ApplicationRuntimeParameters);
            JSONSerializerExecute.FillDeserializedDictionary(dictionary, "processContext", context.ProcessContext);

            return context;
        }

        public override IDictionary<string, object> Serialize(object obj, JavaScriptSerializer serializer)
        {
            Dictionary<string, object> dictionary = new Dictionary<string, object>();

            WfClientRuntimeContext context = (WfClientRuntimeContext)obj;

            dictionary.AddNonDefaultValue("autoCalculate", context.AutoCalculate);
            dictionary["taskTitle"] = context.TaskTitle;
            dictionary["notifyTitle"] = context.NotifyTitle;
            dictionary["operator"] = context.Operator;
            dictionary["autoPersist"] = context.AutoPersist;
            dictionary["applicationRuntimeParameters"] = context.ApplicationRuntimeParameters;
            dictionary["processContext"] = context.ProcessContext;
            dictionary.AddNonDefaultValue("opinion", context.Opinion);

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
