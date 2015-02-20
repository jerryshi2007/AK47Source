using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web.Script.Serialization;
using MCS.Library.Core;

namespace MCS.Library.SOA.DataObjects.Workflow
{
	public class WfConditionDescriptorConverter : JavaScriptConverter
	{
		public override object Deserialize(IDictionary<string, object> dictionary, Type type, JavaScriptSerializer serializer)
		{
			WfConditionDescriptor condition = new WfConditionDescriptor();

			condition.Expression = DictionaryHelper.GetValue(dictionary, "Expression", string.Empty);

			return condition;
		}

		public override IDictionary<string, object> Serialize(object obj, JavaScriptSerializer serializer)
		{
			WfConditionDescriptor condition = (WfConditionDescriptor)obj;

			Dictionary<string, object> dictionary = new Dictionary<string, object>();

			DictionaryHelper.AddNonDefaultValue<string, object>(dictionary, "Expression", condition.Expression);

			return dictionary;
		}

		public override IEnumerable<Type> SupportedTypes
		{
			get
			{
				return new System.Type[] { typeof(WfConditionDescriptor) };
			}
		}
	}
}
