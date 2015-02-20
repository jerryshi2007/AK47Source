using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web.Script.Serialization;
using MCS.Library.Core;
using MCS.Web.Library.Script;

namespace MCS.Library.SOA.DataObjects.Workflow
{
	class WfServiceAddressDefinitionConverter : JavaScriptConverter
	{
		public override object Deserialize(IDictionary<string, object> dictionary, Type type, JavaScriptSerializer serializer)
		{
			WfServiceAddressDefinition svcAddressDef = new WfServiceAddressDefinition();
			svcAddressDef.Key = DictionaryHelper.GetValue(dictionary, "Key", string.Empty);
			svcAddressDef.Address = DictionaryHelper.GetValue(dictionary, "Address", string.Empty);
			svcAddressDef.ServiceNS = DictionaryHelper.GetValue(dictionary, "ServiceNS", string.Empty);
			svcAddressDef.RequestMethod = DictionaryHelper.GetValue(dictionary, "RequestMethod", WfServiceRequestMethod.Get);
			svcAddressDef.ContentType = DictionaryHelper.GetValue(dictionary, "ContentType", WfServiceContentType.Form);

			if (dictionary.ContainsKey("Credential"))
			{
				svcAddressDef.Credential = JSONSerializerExecute.Deserialize<WfNetworkCredential>(dictionary["Credential"]);
			}

			return svcAddressDef;
		}

		public override IDictionary<string, object> Serialize(object obj, JavaScriptSerializer serializer)
		{
			IDictionary<string, object> dictionary = new Dictionary<string, object>();

			WfServiceAddressDefinition svcAddressDef = (WfServiceAddressDefinition)obj;

			DictionaryHelper.AddNonDefaultValue<string, object>(dictionary, "Key", svcAddressDef.Key);
			DictionaryHelper.AddNonDefaultValue<string, object>(dictionary, "Address", svcAddressDef.Address);
			DictionaryHelper.AddNonDefaultValue<string, object>(dictionary, "ServiceNS", svcAddressDef.ServiceNS);
			DictionaryHelper.AddNonDefaultValue<string, object>(dictionary, "RequestMethod", svcAddressDef.RequestMethod);
			DictionaryHelper.AddNonDefaultValue<string, object>(dictionary, "ContentType", svcAddressDef.ContentType);

			dictionary.Add("Credential", svcAddressDef.Credential);

			return dictionary;
		}

		public override IEnumerable<Type> SupportedTypes
		{
			get
			{
				return new System.Type[] { typeof(WfServiceAddressDefinition) };
			}
		}
	}
}
