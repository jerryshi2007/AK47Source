using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Script.Serialization;
using MCS.Library.Core;
using MCS.Library.WF.Contracts.Json.Converters.DataObjects;
using MCS.Library.WF.Contracts.Workflow.DataObjects;
using MCS.Library.WF.Contracts.Workflow.Descriptors;
using MCS.Web.Library.Script;

namespace MCS.Library.WF.Contracts.Json.Converters
{
    public class WfClientActivityMatrixResourceDescriptorJsonConverter : WfClientResourceDescriptorJsonConverterBase
    {
        private static readonly Type[] _SupportedTypes = new Type[] { typeof(WfClientActivityMatrixResourceDescriptor) };

        public override object Deserialize(IDictionary<string, object> dictionary, Type type, JavaScriptSerializer serializer)
        {
            WfClientActivityMatrixResourceDescriptor resource = (WfClientActivityMatrixResourceDescriptor)base.Deserialize(dictionary, type, serializer);

            resource.ExternalMatrixID = dictionary.GetValue("externalMatrixID", string.Empty);

            JSONSerializerExecute.FillDeserializedCollection(dictionary["definitions"], resource.PropertyDefinitions);
            JSONSerializerExecute.FillDeserializedCollection(dictionary["rows"], resource.Rows);

            return resource;
        }

        public override IDictionary<string, object> Serialize(object obj, JavaScriptSerializer serializer)
        {
            IDictionary<string, object> dictionary = base.Serialize(obj, serializer);

            WfClientActivityMatrixResourceDescriptor resource = (WfClientActivityMatrixResourceDescriptor)obj;

            dictionary["externalMatrixID"] = resource.ExternalMatrixID;
            dictionary["definitions"] = resource.PropertyDefinitions;
            dictionary["rows"] = resource.Rows;

            resource.Rows.FillColumnInfoToRowValues(resource.PropertyDefinitions);

            return dictionary;
        }

        protected override WfClientResourceDescriptor CreateInstance(IDictionary<string, object> dictionary, Type type, JavaScriptSerializer serializer)
        {
            return new WfClientActivityMatrixResourceDescriptor();
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
