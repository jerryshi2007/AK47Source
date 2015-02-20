using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web.Script.Serialization;
using MCS.Library.Core;

namespace MCS.Library.SOA.DataObjects.Workflow
{
    public sealed class WfParameterDescriptorConverter : JavaScriptConverter
    {
        public override object Deserialize(IDictionary<string, object> dictionary, Type type, JavaScriptSerializer serializer)
        {
            WfParameterDescriptor wfParameter = new WfParameterDescriptor();
            //wfParameter.Key = DictionaryHelper.GetValue(dictionary, "key", string.Empty);
            wfParameter.ParameterType = DictionaryHelper.GetValue(dictionary, "parameterType", PropertyDataType.String);
            wfParameter.ParameterName = DictionaryHelper.GetValue(dictionary, "parameterName", string.Empty);
            wfParameter.ControlID = DictionaryHelper.GetValue(dictionary, "controlID", string.Empty);
            wfParameter.ControlPropertyName = DictionaryHelper.GetValue(dictionary, "controlPropertyName", string.Empty);
            wfParameter.AutoCollect = DictionaryHelper.GetValue(dictionary, "autoCollect", true);

            return wfParameter;
        }

        public override IDictionary<string, object> Serialize(object obj, JavaScriptSerializer serializer)
        {
            IDictionary<string, object> dictionary = new Dictionary<string, object>();

            WfParameterDescriptor wfParameter = (WfParameterDescriptor)obj;

            //DictionaryHelper.AddNonDefaultValue<string, object>(dictionary, "key", wfParameter.Key);
            DictionaryHelper.AddNonDefaultValue<string, object>(dictionary, "parameterType", wfParameter.ParameterType);
            DictionaryHelper.AddNonDefaultValue<string, object>(dictionary, "parameterName", wfParameter.ParameterName);
            DictionaryHelper.AddNonDefaultValue<string, object>(dictionary, "controlID", wfParameter.ControlID);
            DictionaryHelper.AddNonDefaultValue<string, object>(dictionary, "controlPropertyName", wfParameter.ControlPropertyName);
            DictionaryHelper.AddNonDefaultValue<string, object>(dictionary, "autoCollect", wfParameter.AutoCollect);

            return dictionary;
        }

        public override IEnumerable<Type> SupportedTypes
        {
            get { return new System.Type[] { typeof(WfParameterDescriptor) }; }
        }
    }
}
