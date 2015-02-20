﻿using System;
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

            JSONSerializerExecute.FillDeserializedCollection(dictionary["definitions"], resource.PropertyDefinitions);
            JSONSerializerExecute.FillDeserializedCollection(dictionary["rows"], resource.Rows);

            return resource;
        }

        public override IDictionary<string, object> Serialize(object obj, JavaScriptSerializer serializer)
        {
            IDictionary<string, object> dictionary = base.Serialize(obj, serializer);

            WfClientActivityMatrixResourceDescriptor resource = (WfClientActivityMatrixResourceDescriptor)obj;

            dictionary["definitions"] = resource.PropertyDefinitions;
            dictionary["rows"] = resource.Rows;

            FillColumnInfoToRowValues(resource.PropertyDefinitions, resource.Rows);

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

        private static void FillColumnInfoToRowValues(WfClientRolePropertyDefinitionCollection columns, WfClientRolePropertyRowCollection rows)
        {
            foreach (WfClientRolePropertyRow row in rows)
            {
                foreach (WfClientRolePropertyValue pv in row.Values)
                {
                    WfClientRolePropertyDefinition column = columns[pv.Column.Name];

                    pv.SetColumnInfo(column);
                }
            }
        }
    }
}
