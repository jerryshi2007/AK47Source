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
	public class WeChatUploadFileRetInfoConverter : WeChatRetInfoWithBaseResponseConverterBase<WeChatUploadFileRetInfo>
	{
		protected override void DeserializeExtraProperties(WeChatUploadFileRetInfo data, IDictionary<string, object> dictionary, Type type, JavaScriptSerializer serializer)
		{
			data.Location = dictionary.GetValue("location", string.Empty);
			data.Type = dictionary.GetValue("type", string.Empty);
			data.Content = dictionary.GetValue("content", 0);
		}

		protected override void SerializeExtraProperties(WeChatUploadFileRetInfo data, IDictionary<string, object> dictionary, JavaScriptSerializer serializer)
		{
			dictionary.AddNonDefaultValue("location", data.Location);
			dictionary.AddNonDefaultValue("type", data.Type);
			dictionary.AddNonDefaultValue("content", data.Content);
		}
	}
}
