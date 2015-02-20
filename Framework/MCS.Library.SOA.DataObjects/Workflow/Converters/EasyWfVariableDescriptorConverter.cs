using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web.Script.Serialization;
using MCS.Library.Core;
using MCS.Web.Library.Script;

namespace MCS.Library.SOA.DataObjects.Workflow
{
	/// <summary>
	/// 简版流程变量的序列化器
	/// </summary>
	public class EasyWfVariableDescriptorConverter : EasyWfDescriptorConverterBase
	{
		public override object Deserialize(IDictionary<string, object> dictionary, Type type, JavaScriptSerializer serializer)
		{
			WfVariableDescriptor variable = (WfVariableDescriptor)base.Deserialize(dictionary, type, serializer);

			variable.OriginalType = DictionaryHelper.GetValue(dictionary, "OriginalType", DataType.String);
			variable.OriginalValue = DictionaryHelper.GetValue(dictionary, "OriginalValue", string.Empty);

			return variable;
		}

		public override IDictionary<string, object> Serialize(object obj, JavaScriptSerializer serializer)
		{
			WfVariableDescriptor variable = (WfVariableDescriptor)obj;

			IDictionary<string, object> dictionary = base.Serialize(obj, serializer);

			DictionaryHelper.AddNonDefaultValue<string, object>(dictionary, "OriginalType", variable.OriginalType);
			DictionaryHelper.AddNonDefaultValue<string, object>(dictionary, "OriginalValue", variable.OriginalValue);

			return dictionary;
		}

		protected override WfKeyedDescriptorBase CreateInstance(string key, IDictionary<string, object> dictionary, Type type, JavaScriptSerializer serializer)
		{
			return new WfVariableDescriptor(key);
		}

		public override IEnumerable<Type> SupportedTypes
		{
			get { return new Type[] { typeof(WfVariableDescriptor) }; }
		}
	}
}

