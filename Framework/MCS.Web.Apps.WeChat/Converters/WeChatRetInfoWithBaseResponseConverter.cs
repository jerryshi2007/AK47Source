using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web.Script.Serialization;
using MCS.Library.Core;
using MCS.Web.Apps.WeChat.DataObjects;
using MCS.Web.Library.Script;

namespace MCS.Web.Apps.WeChat.Converters
{
	/// <summary>
	/// 带BaseResponse的返回信息的Converter的基类
	/// </summary>
	public abstract class WeChatRetInfoWithBaseResponseConverterBase<T> : JavaScriptConverter where T : WeChatRetInfoWithBaseResponse, new()
	{
		public override object Deserialize(IDictionary<string, object> dictionary, Type type, JavaScriptSerializer serializer)
		{
			T data = new T();

			data.BaseResponse = JSONSerializerExecute.Deserialize<WeChatBaseResponse>(dictionary["base_resp"]);

			DeserializeExtraProperties(data, dictionary, type, serializer);

			return data;
		}

		protected virtual void DeserializeExtraProperties(T data, IDictionary<string, object> dictionary, Type type, JavaScriptSerializer serializer)
		{
		}

		public override IDictionary<string, object> Serialize(object obj, JavaScriptSerializer serializer)
		{
			Dictionary<string, object> dictionary = new Dictionary<string, object>();
			T data = (T)obj;

			dictionary.AddNonDefaultValue("base_resp", data.BaseResponse);

			SerializeExtraProperties(data, dictionary, serializer);

			return dictionary;
		}

		protected virtual void SerializeExtraProperties(T data, IDictionary<string, object> dictionary, JavaScriptSerializer serializer)
		{
		}

		public override IEnumerable<Type> SupportedTypes
		{
			get
			{
				return new Type[] { typeof(T) };
			}
		}
	}
}
