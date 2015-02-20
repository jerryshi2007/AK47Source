using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web.Script.Serialization;
using MCS.Library.Core;

namespace MCS.Library.SOA.DataObjects.Workflow
{
	public class WfRelativeLinkDescriptorConverter : WfDescriptorConverterBase
	{
		public override object Deserialize(IDictionary<string, object> dictionary, Type type, JavaScriptSerializer serializer)
		{
			WfRelativeLinkDescriptor linkDesp = (WfRelativeLinkDescriptor)base.Deserialize(dictionary, type, serializer);

			linkDesp.Category = DictionaryHelper.GetValue(dictionary, "Category", linkDesp.Category);
			linkDesp.Url = DictionaryHelper.GetValue(dictionary, "Url", linkDesp.Url);
			linkDesp.Description = DictionaryHelper.GetValue(dictionary, "Description", linkDesp.Description);

			return linkDesp;
		}

		public override IDictionary<string, object> Serialize(object obj, JavaScriptSerializer serializer)
		{
			WfRelativeLinkDescriptor linkDesp = (WfRelativeLinkDescriptor)obj;

			IDictionary<string, object> dictionary = base.Serialize(obj, serializer);

			if (!dictionary.ContainsKey("Category"))
			{
				dictionary.Add("Category", linkDesp.Category);
			}
			if (!dictionary.ContainsKey("Url"))
			{
				dictionary.Add("Url", linkDesp.Url);
			}
			if (!dictionary.ContainsKey("Description"))
			{
				dictionary.Add("Description", linkDesp.Description);
			}

			return dictionary;
		}

		protected override WfKeyedDescriptorBase CreateInstance(string key, IDictionary<string, object> dictionary, Type type, System.Web.Script.Serialization.JavaScriptSerializer serializer)
		{
			return new WfRelativeLinkDescriptor(key);
		}

		public override IEnumerable<Type> SupportedTypes
		{
			get
			{
				return new System.Type[] { typeof(WfRelativeLinkDescriptor), typeof(IWfRelativeLinkDescriptor) };
			}
		}
	}
}
