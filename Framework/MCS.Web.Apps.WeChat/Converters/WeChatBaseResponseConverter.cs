using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web.Script.Serialization;
using MCS.Library.Core;
using MCS.Web.Apps.WeChat.DataObjects;

namespace MCS.Web.Apps.WeChat.Converters
{
	/// <summary>
	/// {"base_resp":{"ret":0,"err_msg":"ok"},"redirect_url":"\/cgi-bin\/home?t=home\/index&lang=zh_CN&token=2118804718"}
	/// </summary>
	public class WeChatBaseResponseConverter : JavaScriptConverter
	{
		public override object Deserialize(IDictionary<string, object> dictionary, Type type, JavaScriptSerializer serializer)
		{
			WeChatBaseResponse data = new WeChatBaseResponse();

			data.Ret = dictionary.GetValue("ret", 0);
			data.ErrMsg = dictionary.GetValue("err_msg", string.Empty);

			return data;
		}

		public override IDictionary<string, object> Serialize(object obj, JavaScriptSerializer serializer)
		{
			Dictionary<string, object> dictionary = new Dictionary<string, object>();
			WeChatBaseResponse data = (WeChatBaseResponse)obj;

			dictionary.AddNonDefaultValue("ret", data.Ret);
			dictionary.AddNonDefaultValue("err_msg", data.ErrMsg);

			return dictionary;
		}

		public override IEnumerable<Type> SupportedTypes
		{
			get
			{
				return new Type[] { typeof(WeChatBaseResponse) };
			}
		}
	}
}
