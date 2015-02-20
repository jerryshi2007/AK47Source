using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MCS.Library.Core;
using MCS.Web.Apps.WeChat.DataObjects;
using MCS.Web.Library.Script;
using System.Web.Script.Serialization;

namespace MCS.Web.Apps.WeChat.Converters
{
	public class WeChatGroupConverter : JavaScriptConverter
	{
		public override object Deserialize(IDictionary<string, object> dictionary, Type type, JavaScriptSerializer serializer)
		{
			WeChatGroup data = new WeChatGroup();

			data.AccountID = dictionary.GetValue("accountID", string.Empty);
			data.GroupID = dictionary.GetValue("id", 0);
			data.Name = dictionary.GetValue("name", string.Empty);
			data.Count = dictionary.GetValue("cnt", 0);
			data.CreateTime = dictionary.GetValue("createTime", DateTime.MinValue);
			data.UpdateTime = dictionary.GetValue("updateTime", DateTime.MinValue);

			return data;
		}

		public override IDictionary<string, object> Serialize(object obj, JavaScriptSerializer serializer)
		{
			Dictionary<string, object> dictionary = new Dictionary<string, object>();
			WeChatGroup data = (WeChatGroup)obj;

			dictionary.AddNonDefaultValue("accountID", data.AccountID);
			dictionary.AddNonDefaultValue("id", data.GroupID);
			dictionary.AddNonDefaultValue("name", data.Name);
			dictionary.AddNonDefaultValue("cnt", data.Count);
			dictionary.AddNonDefaultValue("createTime", data.CreateTime);
			dictionary.AddNonDefaultValue("updateTime", data.UpdateTime);

			return dictionary;
		}

		public override IEnumerable<Type> SupportedTypes
		{
			get
			{
				return new Type[] { typeof(WeChatGroup) };
			}
		}
	}
}
