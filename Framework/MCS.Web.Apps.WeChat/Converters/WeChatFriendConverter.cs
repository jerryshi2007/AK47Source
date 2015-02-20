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
	public class WeChatFriendConverter : JavaScriptConverter
	{
		public override object Deserialize(IDictionary<string, object> dictionary, Type type, JavaScriptSerializer serializer)
		{
			WeChatFriend data = new WeChatFriend();

			data.FakeID = dictionary.GetValue("id", string.Empty);
			data.NickName = dictionary.GetValue("nick_name", string.Empty);
			data.RemarkName = dictionary.GetValue("remark_name", string.Empty);
			data.GroupID = dictionary.GetValue("group_id", 0);
			data.OpenID = dictionary.GetValue("openID", string.Empty);
			data.AccountID = dictionary.GetValue("accountID", string.Empty);
			data.CreateTime = dictionary.GetValue("createTime", DateTime.MinValue);
			data.UpdateTime = dictionary.GetValue("updateTime", DateTime.MinValue);

			return data;
		}

		public override IDictionary<string, object> Serialize(object obj, JavaScriptSerializer serializer)
		{
			Dictionary<string, object> dictionary = new Dictionary<string, object>();
			WeChatFriend data = (WeChatFriend)obj;

			dictionary.AddNonDefaultValue("id", data.FakeID);
			dictionary.AddNonDefaultValue("nick_name", data.NickName);
			dictionary.AddNonDefaultValue("remark_name", data.RemarkName);
			dictionary.AddNonDefaultValue("group_id", data.GroupID);
			dictionary.AddNonDefaultValue("openID", data.OpenID);
			dictionary.AddNonDefaultValue("accountID", data.AccountID);
			dictionary.AddNonDefaultValue("createTime", data.CreateTime);
			dictionary.AddNonDefaultValue("updateTime", data.UpdateTime);

			return dictionary;
		}

		public override IEnumerable<Type> SupportedTypes
		{
			get
			{
				return new Type[] { typeof(WeChatFriend) };
			}
		}
	}
}
