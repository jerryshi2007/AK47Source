using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web.Script.Serialization;
using MCS.Library.OGUPermission;
using MCS.Web.Library.Script;
using MCS.Library.Core;

namespace MCS.Library.SOA.DataObjects.Workflow
{
	public class WfDynamicResourceDescriptorConverter : WfResourceDescriptorConverterBase
	{
		public override object Deserialize(IDictionary<string, object> dictionary, Type type, JavaScriptSerializer serializer)
		{
			WfDynamicResourceDescriptor dynResourceDesp = (WfDynamicResourceDescriptor)base.Deserialize(dictionary, type, serializer);

			dynResourceDesp.Name = DictionaryHelper.GetValue(dictionary, "Name", string.Empty);

			WfConditionDescriptor condition = JSONSerializerExecute.Deserialize<WfConditionDescriptor>(dictionary["Condition"]);
			dynResourceDesp.Condition = condition;

			return dynResourceDesp;
		}

		public override IDictionary<string, object> Serialize(object obj, JavaScriptSerializer serializer)
		{
			WfDynamicResourceDescriptor dynResourceDesp = (WfDynamicResourceDescriptor)obj;

			IDictionary<string, object> dictionary = base.Serialize(obj, serializer);

			dictionary.Add("Name", dynResourceDesp.Name);
			dictionary.Add("Condition", dynResourceDesp.Condition);

			return dictionary;
		}

		public override IEnumerable<Type> SupportedTypes
		{
			get { return new Type[] { typeof(WfDynamicResourceDescriptor) }; }
		}

		protected override WfResourceDescriptor CreateInstance(IDictionary<string, object> dictionary, Type type, JavaScriptSerializer serializer)
		{
			return new WfDynamicResourceDescriptor();
		}
	}
}
