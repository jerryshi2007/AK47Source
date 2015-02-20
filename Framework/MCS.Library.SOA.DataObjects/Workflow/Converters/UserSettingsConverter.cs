using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web.Script.Serialization;
using MCS.Library.Core;
using MCS.Web.Library.Script;

namespace MCS.Library.SOA.DataObjects.Workflow
{
	public class UserSettingsConverter : JavaScriptConverter
	{
		public override object Deserialize(IDictionary<string, object> dictionary, Type type, JavaScriptSerializer serializer)
		{
			UserSettings settings = new UserSettings();
			settings.UserID = DictionaryHelper.GetValue(dictionary, "UserID", string.Empty);

			if (dictionary.ContainsKey("Categories"))
			{
				UserSettingsCategoryCollection categories = JSONSerializerExecute.Deserialize<UserSettingsCategoryCollection>(dictionary["Categories"]);

				settings.Categories.Clear();
				settings.Categories.CopyFrom(categories);

			}

			return settings;
		}

		public override IDictionary<string, object> Serialize(object obj, JavaScriptSerializer serializer)
		{
			IDictionary<string, object> dictionary = new Dictionary<string, object>();

			UserSettings settings = (UserSettings)obj;
			DictionaryHelper.AddNonDefaultValue<string, object>(dictionary, "UserID", settings.UserID);
			DictionaryHelper.AddNonDefaultValue<string, object>(dictionary, "Categories", settings.Categories);

			return dictionary;
		}

		public override IEnumerable<Type> SupportedTypes
		{
			get { return new Type[] { typeof(UserSettings) }; }
		}
	}
}
