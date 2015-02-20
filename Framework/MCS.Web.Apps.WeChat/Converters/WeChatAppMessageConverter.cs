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
	public class WeChatAppMessageConverter : JavaScriptConverter
	{
		public override object Deserialize(IDictionary<string, object> dictionary, Type type, JavaScriptSerializer serializer)
		{
			WeChatAppMessage data = new WeChatAppMessage();

			data.AppMessageID = dictionary.GetValue("app_id", 0);
			data.FileID = dictionary.GetValue("file_id", 0);
			data.Title = dictionary.GetValue("title", string.Empty);
			data.Digest = dictionary.GetValue("digest", string.Empty);
			data.Author = dictionary.GetValue("author", string.Empty);
			data.ContentUrl = dictionary.GetValue("content_url", string.Empty);
			data.ImageUrl = dictionary.GetValue("img_url", string.Empty);

			long createTimeValue = dictionary.GetValue("create_time", 0L) * 1000;
			data.CreateTime = createTimeValue.JavascriptDateNumberToDateTime().ToLocalTime();

			int showCoverValue = dictionary.GetValue("show_cover_pic", 0);

			data.ShowCover = showCoverValue != 0 ? true : false;

			return data;
		}

		public override IDictionary<string, object> Serialize(object obj, JavaScriptSerializer serializer)
		{
			Dictionary<string, object> dictionary = new Dictionary<string, object>();
			WeChatAppMessage data = (WeChatAppMessage)obj;

			dictionary.AddNonDefaultValue("app_id", data.AppMessageID);
			dictionary.AddNonDefaultValue("file_id", data.FileID);
			dictionary.AddNonDefaultValue("title", data.Title);
			dictionary.AddNonDefaultValue("digest", data.Digest);
			dictionary.AddNonDefaultValue("author", data.Author);
			dictionary.AddNonDefaultValue("content_url", data.ContentUrl);
			dictionary.AddNonDefaultValue("img_url", data.ImageUrl);
			dictionary.AddNonDefaultValue("create_time", data.CreateTime.ToJavascriptDateNumber() / 1000);
			dictionary.AddNonDefaultValue("show_cover_pic", data.ShowCover ? 1 : 0);

			return dictionary;
		}

		public override IEnumerable<Type> SupportedTypes
		{
			get
			{
				return new Type[] { typeof(WeChatAppMessage) };
			}
		}
	}
}
