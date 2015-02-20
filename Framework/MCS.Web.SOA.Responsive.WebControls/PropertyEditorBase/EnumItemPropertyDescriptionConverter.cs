using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web.Script.Serialization;
using MCS.Library.Core;

namespace MCS.Web.Responsive.WebControls
{
	public sealed class EnumItemPropertyDescriptionConverter : JavaScriptConverter
	{
		public override object Deserialize(IDictionary<string, object> dictionary, Type type, JavaScriptSerializer serializer)
		{
			EnumItemPropertyDescription desp = new EnumItemPropertyDescription();

			desp.Value = DictionaryHelper.GetValue(dictionary, "value", string.Empty);
			desp.Text = DictionaryHelper.GetValue(dictionary, "text", string.Empty);

			return desp;
		}

		public override IDictionary<string, object> Serialize(object obj, JavaScriptSerializer serializer)
		{
			EnumItemPropertyDescription desp = (EnumItemPropertyDescription)obj;

			Dictionary<string, object> dictionary = new Dictionary<string, object>();

			dictionary.AddNonDefaultValue("value", desp.Value);
			dictionary.AddNonDefaultValue("text", desp.Text);

			return dictionary;
		}

		public override IEnumerable<Type> SupportedTypes
		{
			get { return new Type[] { typeof(EnumItemPropertyDescription) }; }
		}
	}
}
