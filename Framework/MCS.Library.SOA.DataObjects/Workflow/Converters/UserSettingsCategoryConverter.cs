using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web.Script.Serialization;
using MCS.Library.Core;
using MCS.Web.Library.Script;

namespace MCS.Library.SOA.DataObjects.Workflow
{
	public class UserSettingsCategoryConverter : JavaScriptConverter
	{
		public override object Deserialize(IDictionary<string, object> dictionary, Type type, JavaScriptSerializer serializer)
		{
			UserSettingsCategory category = new UserSettingsCategory();

			category.Name = DictionaryHelper.GetValue(dictionary, "Name", string.Empty);
			category.Description = DictionaryHelper.GetValue(dictionary, "Description", string.Empty);

			PropertyValueCollection properties = JSONSerializerExecute.Deserialize<PropertyValueCollection>(dictionary["Properties"]);
			category.Properties.Clear();
			category.Properties.CopyFrom(properties);

			return category;
		}

		public override IDictionary<string, object> Serialize(object obj, JavaScriptSerializer serializer)
		{
			IDictionary<string, object> dictionary = new Dictionary<string, object>();

			UserSettingsCategory category = (UserSettingsCategory)obj;

			DictionaryHelper.AddNonDefaultValue<string, object>(dictionary, "Name", category.Name);
			DictionaryHelper.AddNonDefaultValue<string, object>(dictionary, "Description", category.Description);
			DictionaryHelper.AddNonDefaultValue<string, object>(dictionary, "Properties", category.Properties);

			return dictionary;
		}

		public override IEnumerable<Type> SupportedTypes
		{
			get { return new Type[] { typeof(UserSettingsCategory) }; }
		}
	}
}
