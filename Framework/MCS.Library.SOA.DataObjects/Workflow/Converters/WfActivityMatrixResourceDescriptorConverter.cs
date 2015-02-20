using MCS.Web.Library.Script;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Script.Serialization;

namespace MCS.Library.SOA.DataObjects.Workflow
{
    public class WfActivityMatrixResourceDescriptorConverter : WfResourceDescriptorConverterBase
    {
        private static readonly Type[] _SupportedTypes = new Type[] { typeof(WfActivityMatrixResourceDescriptor) };

        public override object Deserialize(IDictionary<string, object> dictionary, Type type, JavaScriptSerializer serializer)
        {
            WfActivityMatrixResourceDescriptor resource = (WfActivityMatrixResourceDescriptor)base.Deserialize(dictionary, type, serializer);

            JSONSerializerExecute.FillDeserializedCollection(dictionary["definitions"], resource.PropertyDefinitions);
            JSONSerializerExecute.FillDeserializedCollection(dictionary["rows"], resource.Rows);

            return resource;
        }

        public override IDictionary<string, object> Serialize(object obj, JavaScriptSerializer serializer)
        {
            IDictionary<string, object> dictionary = base.Serialize(obj, serializer);

            WfActivityMatrixResourceDescriptor resource = (WfActivityMatrixResourceDescriptor)obj;

            dictionary["definitions"] = resource.PropertyDefinitions;
            dictionary["rows"] = resource.Rows;

            FillColumnInfoToRowValues(resource.PropertyDefinitions, resource.Rows);

            return dictionary;
        }

        protected override WfResourceDescriptor CreateInstance(IDictionary<string, object> dictionary, Type type, JavaScriptSerializer serializer)
        {
            return new WfActivityMatrixResourceDescriptor();
        }

        /// <summary>
        /// 所支持的类型
        /// </summary>
        public override IEnumerable<Type> SupportedTypes
        {
            get
            {
                return _SupportedTypes;
            }
        }

        private static void FillColumnInfoToRowValues(SOARolePropertyDefinitionCollection columns, SOARolePropertyRowCollection rows)
        {
            foreach (SOARolePropertyRow row in rows)
            {
                foreach (SOARolePropertyValue pv in row.Values)
                {
                    SOARolePropertyDefinition column = columns[pv.Column.Name];

                    pv.SetColumnInfo(column);
                }
            }
        }
    }
}
