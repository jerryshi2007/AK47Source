using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MCS.Web.Library.Script;
using MCS.Web.Apps.WeChat.DataObjects;

namespace MCS.Web.Apps.WeChat.Converters
{
	public static class WeChatConverterHelper
	{
		public static void RegisterConverters()
		{
			JSONSerializerExecute.RegisterConverter(typeof(WeChatBaseResponseConverter));
			JSONSerializerExecute.RegisterConverter(typeof(WeChatLoginRetInfoConverter));
			JSONSerializerExecute.RegisterConverter(typeof(WeChatGroupConverter));
			JSONSerializerExecute.RegisterConverter(typeof(WeChatFriendConverter));
			JSONSerializerExecute.RegisterConverter(typeof(WeChatRecentMessageConverter));
			JSONSerializerExecute.RegisterConverter(typeof(WeChatUploadFileRetInfoConverter));
			JSONSerializerExecute.RegisterConverter(typeof(WeChatAppMessageConverter));
			JSONSerializerExecute.RegisterConverter(typeof(WeChatSingleSendMessageRetInfoConverter));
		}
	}
}
