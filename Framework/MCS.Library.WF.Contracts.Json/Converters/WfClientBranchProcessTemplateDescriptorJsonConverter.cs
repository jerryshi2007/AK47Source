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
    public class WfClientBranchProcessTemplateDescriptorJsonConverter : WfClientKeyedDescriptorJsonConverterBase
    {
        private static readonly Type[] _SupportedTypes = new Type[] { typeof(WfClientBranchProcessTemplateDescriptor) };

        public override object Deserialize(IDictionary<string, object> dictionary, Type type, JavaScriptSerializer serializer)
        {
            WfClientBranchProcessTemplateDescriptor template = (WfClientBranchProcessTemplateDescriptor)base.Deserialize(dictionary, type, serializer);

            template.Condition = JSONSerializerExecute.Deserialize<WfClientConditionDescriptor>(dictionary.GetValue("condition", (object)null));

            JSONSerializerExecute.FillDeserializedCollection<WfClientResourceDescriptor>(dictionary.GetValue("resources", (object)null), template.Resources);
            JSONSerializerExecute.FillDeserializedCollection<WfClientResourceDescriptor>(dictionary.GetValue("cancelSubProcessNotifier", (object)null), template.CancelSubProcessNotifier);
            JSONSerializerExecute.FillDeserializedCollection<WfClientRelativeLinkDescriptor>(dictionary.GetValue("relativeLinks", (object)null), template.RelativeLinks);

            return template;
        }

        public override IDictionary<string, object> Serialize(object obj, JavaScriptSerializer serializer)
        {
            Dictionary<string, object> dictionary = (Dictionary<string, object>)base.Serialize(obj, serializer);
            WfClientBranchProcessTemplateDescriptor template = (WfClientBranchProcessTemplateDescriptor)obj;

            dictionary.AddNonDefaultValue("condition", template.Condition);

            dictionary.Add("resources", template.Resources);
            dictionary.Add("cancelSubProcessNotifier", template.CancelSubProcessNotifier);
            dictionary.Add("relativeLinks", template.RelativeLinks);

            return dictionary;
        }

        protected override WfClientKeyedDescriptorBase CreateInstance(string key, IDictionary<string, object> dictionary, Type type, JavaScriptSerializer serializer)
        {
            return new WfClientBranchProcessTemplateDescriptor();
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
