using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web.Script.Serialization;
using MCS.Library.Core;

namespace MCS.Library.SOA.DataObjects.Workflow
{
	public class WfExternalUserConverter : JavaScriptConverter
	{
		public override object Deserialize(IDictionary<string, object> dictionary, Type type, JavaScriptSerializer serializer)
		{
			WfExternalUser externalUser = new WfExternalUser();
			externalUser.Key = DictionaryHelper.GetValue(dictionary, "Key", string.Empty);
			externalUser.Name = DictionaryHelper.GetValue(dictionary, "Name", string.Empty);
			externalUser.Gender = DictionaryHelper.GetValue(dictionary, "Gender", Gender.Female);
			externalUser.Phone = DictionaryHelper.GetValue(dictionary, "Phone", string.Empty);
			externalUser.MobilePhone = DictionaryHelper.GetValue(dictionary, "MobilePhone", string.Empty);
			externalUser.Title = DictionaryHelper.GetValue(dictionary, "Title", string.Empty);
			externalUser.Email = DictionaryHelper.GetValue(dictionary, "Email", string.Empty);

			return externalUser;
		}

		public override IDictionary<string, object> Serialize(object obj, JavaScriptSerializer serializer)
		{
			IDictionary<string, object> dictionary = new Dictionary<string, object>();

			WfExternalUser externalUser = (WfExternalUser)obj;
			DictionaryHelper.AddNonDefaultValue<string, object>(dictionary, "Key", externalUser.Key);
			DictionaryHelper.AddNonDefaultValue<string, object>(dictionary, "Name", externalUser.Name);
			DictionaryHelper.AddNonDefaultValue<string, object>(dictionary, "Gender",externalUser.Gender);
			DictionaryHelper.AddNonDefaultValue<string, object>(dictionary, "Phone", externalUser.Phone);
			DictionaryHelper.AddNonDefaultValue<string, object>(dictionary, "MobilePhone", externalUser.MobilePhone);
			DictionaryHelper.AddNonDefaultValue<string, object>(dictionary, "Title", externalUser.Title);
			DictionaryHelper.AddNonDefaultValue<string, object>(dictionary, "Email", externalUser.Email);
			
			return dictionary;
		}

		public override IEnumerable<Type> SupportedTypes
		{
			get { return new System.Type[] { typeof(WfExternalUser) }; }
		}
	}
}
