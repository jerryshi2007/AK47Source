using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web.Script.Serialization;
using MCS.Library.Core;

namespace MCS.Library.WcfExtensions
{
	/// <summary>
	/// WfErrorDTO的JSON序列化器
	/// </summary>
	public class WfErrorDTOConverter : JavaScriptConverter
	{
		public override IDictionary<string, object> Serialize(object obj, JavaScriptSerializer serializer)
		{
			Dictionary<string, object> dictionary = new Dictionary<string, object>();
			WfErrorDTO error = (WfErrorDTO)obj;
			
			DictionaryHelper.AddNonDefaultValue<string, object>(dictionary, "name", error.Name);
			DictionaryHelper.AddNonDefaultValue<string, object>(dictionary, "message", error.Message);
			DictionaryHelper.AddNonDefaultValue<string, object>(dictionary, "description", error.Description);
			DictionaryHelper.AddNonDefaultValue<string, object>(dictionary, "number", error.Number);

			return dictionary;
		}

		public override object Deserialize(IDictionary<string, object> dictionary, Type type, JavaScriptSerializer serializer)
		{
			WfErrorDTO error = new WfErrorDTO();

			error.Name = DictionaryHelper.GetValue(dictionary, "name", string.Empty);
			error.Message = DictionaryHelper.GetValue(dictionary, "message", string.Empty);
			error.Description = DictionaryHelper.GetValue(dictionary, "description", string.Empty);
			error.Number = DictionaryHelper.GetValue(dictionary, "number", 0);

			return error;
		}

		public override IEnumerable<Type> SupportedTypes
		{
			get
			{
				return new Type[] { typeof(WfErrorDTO) };
			}
		}
	}
}
