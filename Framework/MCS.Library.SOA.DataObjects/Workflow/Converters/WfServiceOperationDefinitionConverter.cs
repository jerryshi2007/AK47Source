using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web.Script.Serialization;
using MCS.Library.Core;
using MCS.Web.Library.Script;

namespace MCS.Library.SOA.DataObjects.Workflow
{
    class WfServiceOperationDefinitionConverter : JavaScriptConverter
    {
        public override object Deserialize(IDictionary<string, object> dictionary, Type type, JavaScriptSerializer serializer)
        {
            WfServiceOperationDefinition svcOperationDef = new WfServiceOperationDefinition();

            svcOperationDef.Key = DictionaryHelper.GetValue(dictionary, "Key", string.Empty);
            svcOperationDef.OperationName = DictionaryHelper.GetValue(dictionary, "OperationName", string.Empty);
            svcOperationDef.InvokeWhenPersist = DictionaryHelper.GetValue(dictionary, "InvokeWhenPersist", false);

            if (dictionary.ContainsKey("Timeout"))
            {
                string strTimeout = DictionaryHelper.GetValue(dictionary, "Timeout", string.Empty);

                if (strTimeout.IsNotEmpty())
                {
                    string[] parts = strTimeout.Split('T');

                    if (parts.Length > 1)
                        strTimeout = parts[1];

                    TimeSpan timeoutSpan = TimeSpan.Parse(strTimeout);
                    svcOperationDef.Timeout = timeoutSpan;
                }
            }

            svcOperationDef.RtnXmlStoreParamName = DictionaryHelper.GetValue(dictionary, "RtnXmlStoreParamName", string.Empty);

            if (dictionary.ContainsKey("AddressDef"))
            {
                var addressDef = JSONSerializerExecute.Deserialize<WfServiceAddressDefinition>(dictionary["AddressDef"]);
                svcOperationDef.AddressDef = addressDef;
            }

            if (dictionary.ContainsKey("Params"))
            {
                var templates = JSONSerializerExecute.Deserialize<WfServiceOperationParameterCollection>(dictionary["Params"]);
                svcOperationDef.Params.Clear();
                svcOperationDef.Params.CopyFrom(templates);
            }

            return svcOperationDef;
        }

        public override IDictionary<string, object> Serialize(object obj, JavaScriptSerializer serializer)
        {
            IDictionary<string, object> dictionary = new Dictionary<string, object>();

            WfServiceOperationDefinition svcAddressDef = (WfServiceOperationDefinition)obj;

            DictionaryHelper.AddNonDefaultValue<string, object>(dictionary, "OperationName", svcAddressDef.OperationName);
            DictionaryHelper.AddNonDefaultValue<string, object>(dictionary, "RtnXmlStoreParamName", svcAddressDef.RtnXmlStoreParamName);
            DictionaryHelper.AddNonDefaultValue<string, object>(dictionary, "Key", svcAddressDef.Key);
            DictionaryHelper.AddNonDefaultValue<string, object>(dictionary, "InvokeWhenPersist", svcAddressDef.InvokeWhenPersist);

            DateTime dtTimeOut = new DateTime(2008, 8, 8, svcAddressDef.Timeout.Hours, svcAddressDef.Timeout.Minutes, svcAddressDef.Timeout.Seconds);

            DictionaryHelper.AddNonDefaultValue<string, object>(dictionary, "Timeout", dtTimeOut);

            dictionary.Add("Params", svcAddressDef.Params);
            dictionary.Add("AddressDef", svcAddressDef.AddressDef);

            return dictionary;
        }

        public override IEnumerable<Type> SupportedTypes
        {
            get
            {
                return new System.Type[] { typeof(WfServiceOperationDefinition) };
            }
        }
    }
}
