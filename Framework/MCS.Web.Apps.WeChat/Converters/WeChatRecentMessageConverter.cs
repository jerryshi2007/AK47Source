using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web.Script.Serialization;
using MCS.Library.Core;
using MCS.Web.Apps.WeChat.DataObjects;

namespace MCS.Web.Apps.WeChat.Converters
{
	public class WeChatRecentMessageConverter : JavaScriptConverter
	{
		public override object Deserialize(IDictionary<string, object> dictionary, Type type, JavaScriptSerializer serializer)
		{
			WeChatRecentMessage data = new WeChatRecentMessage();

			data.AccountID = dictionary.GetValue("accountID", string.Empty);
			data.MessageID = dictionary.GetValue("id", 0L);
			data.MessageType = dictionary.GetValue("type", WeChatMessageType.Text);
			data.FakeID = dictionary.GetValue("fakeid", string.Empty);
			data.NickName = dictionary.GetValue("nick_name", string.Empty);

			long sendTimeValue = dictionary.GetValue("date_time", 0L) * 1000;

			data.SentTime = sendTimeValue.JavascriptDateNumberToDateTime().ToLocalTime();

			data.Content = dictionary.GetValue("content", string.Empty);
			data.Source = dictionary.GetValue("source", string.Empty);
			data.MessageStatus = dictionary.GetValue("msg_status", 0);
			data.HasReply = dictionary.GetValue("has_reply", false);
			data.RefuseReason = dictionary.GetValue("refuse_reason", string.Empty);
			data.ToFakeID = dictionary.GetValue("to_uin", string.Empty);

			return data;
		}

		public override IDictionary<string, object> Serialize(object obj, JavaScriptSerializer serializer)
		{
			Dictionary<string, object> dictionary = new Dictionary<string, object>();
			WeChatRecentMessage data = (WeChatRecentMessage)obj;

			dictionary.AddNonDefaultValue("accountID", data.AccountID);
			dictionary.AddNonDefaultValue("id", data.MessageID);
			dictionary.AddNonDefaultValue("type", data.MessageType);
			dictionary.AddNonDefaultValue("fakeid", data.FakeID);
			dictionary.AddNonDefaultValue("nick_name", data.NickName);
			dictionary.AddNonDefaultValue("date_time", data.SentTime.ToJavascriptDateNumber() / 1000);
			dictionary.AddNonDefaultValue("content", data.Content);
			dictionary.AddNonDefaultValue("source", data.Source);
			dictionary.AddNonDefaultValue("msg_status", data.MessageStatus);
			dictionary.AddNonDefaultValue("has_reply", data.HasReply);
			dictionary.AddNonDefaultValue("refuse_reason", data.RefuseReason);
			dictionary.AddNonDefaultValue("to_uin", data.ToFakeID);

			return dictionary;
		}

		public override IEnumerable<Type> SupportedTypes
		{
			get
			{
				return new Type[] { typeof(WeChatRecentMessage) };
			}
		}
	}
}
