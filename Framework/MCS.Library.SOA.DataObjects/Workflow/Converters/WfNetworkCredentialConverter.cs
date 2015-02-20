using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web.Script.Serialization;
using MCS.Library.Core;

namespace MCS.Library.SOA.DataObjects.Workflow
{
	public class WfNetworkCredentialConverter : JavaScriptConverter
	{
		public override object Deserialize(IDictionary<string, object> dictionary, Type type, JavaScriptSerializer serializer)
		{
			WfNetworkCredential credential = new WfNetworkCredential();

			credential.Key = DictionaryHelper.GetValue(dictionary, "Key", string.Empty);
			credential.LogOnName = DictionaryHelper.GetValue(dictionary, "LogOnName", string.Empty);
			credential.Password = DictionaryHelper.GetValue(dictionary, "Password", string.Empty);

			return credential;
		}

		public override IDictionary<string, object> Serialize(object obj, JavaScriptSerializer serializer)
		{
			IDictionary<string, object> dictionary = new Dictionary<string, object>();

			WfNetworkCredential credential = (WfNetworkCredential)obj;

			DictionaryHelper.AddNonDefaultValue<string, object>(dictionary, "Key", credential.Key);
			DictionaryHelper.AddNonDefaultValue<string, object>(dictionary, "LogOnName", credential.LogOnName);
			DictionaryHelper.AddNonDefaultValue<string, object>(dictionary, "LogOnNameWithDomain", credential.LogOnNameWithDomain);
			DictionaryHelper.AddNonDefaultValue<string, object>(dictionary, "LogOnNameWithoutDomain", credential.LogOnNameWithoutDomain);
			DictionaryHelper.AddNonDefaultValue<string, object>(dictionary, "Domain", credential.Domain);
			DictionaryHelper.AddNonDefaultValue<string, object>(dictionary, "Password", credential.Password);

			return dictionary;
		}

		public override IEnumerable<Type> SupportedTypes
		{
			get { return new Type[] { typeof(WfNetworkCredential) }; }
		}
	}
}
