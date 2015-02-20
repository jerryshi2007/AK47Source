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
	/// {"base_resp":{"ret":0,"err_msg":"ok"},"redirect_url":"\/cgi-bin\/home?t=home\/index&lang=zh_CN&token=2118804718"}
	/// </summary>
	public class WeChatLoginRetInfoConverter : WeChatRetInfoWithBaseResponseConverterBase<WeChatLoginRetInfo>
	{
		protected override void DeserializeExtraProperties(WeChatLoginRetInfo data, IDictionary<string, object> dictionary, Type type, JavaScriptSerializer serializer)
		{
			data.RedirectUrl = dictionary.GetValue("redirect_url", string.Empty);
		}

		protected override void SerializeExtraProperties(WeChatLoginRetInfo data, IDictionary<string, object> dictionary, JavaScriptSerializer serializer)
		{
			dictionary.AddNonDefaultValue("redirect_url", data.RedirectUrl);
		}
	}
}
