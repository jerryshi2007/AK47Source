using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Net;
using System.Text;
using System.Text.RegularExpressions;
using System.Web;
using MCS.Library.Core;
using MCS.Web.Apps.WeChat.DataObjects;
using MCS.Web.Library.Script;

namespace MCS.Web.Apps.WeChat.Executors
{
	public class WeChatGetFriendMessagesExecutor : WeChatAuthenticatedExecutorBase
	{
		private WeChatRecentMessageCollection _Messages = new WeChatRecentMessageCollection();

		public WeChatGetFriendMessagesExecutor(string fakeID, WeChatLoginInfo loginInfo)
			: base(loginInfo)
		{
			fakeID.CheckStringIsNullOrEmpty("toFakeID");

			this.FakeID = fakeID;
		}

		public string FakeID
		{
			get;
			private set;
		}

		public WeChatRecentMessageCollection Messages
		{
			get
			{
				return this._Messages;
			}
		}

		protected override HttpWebRequest PrepareWebRequest()
		{
			NameValueCollection parameters = new NameValueCollection();

			parameters["t"] = "message/send";
			parameters["action"] = "index";
			parameters["tofakeid"] = this.FakeID;
			parameters["token"] = this.LoginInfo.Token;
			parameters["lang"] = "zh-CN";

			string url = "https://mp.weixin.qq.com/cgi-bin/singlesendpage?" + parameters.ToUrlParameters(true);

			HttpWebRequest request = (HttpWebRequest)WebRequest.Create(url);

			request.CookieContainer = this.LoginInfo.LoginCookie;
			request.ContentType = "text/html; charset=UTF-8";
			request.Method = "GET";
			request.UserAgent = "Mozilla/5.0 (Windows NT 5.1; rv:2.0.1) Gecko/20100101 Firefox/4.0.1";

			return request;
		}

		protected override void ProcessResponseText(string responseText)
		{
			WeChatRecentMessageCollection result = new WeChatRecentMessageCollection();

			string templateA = "\"msg_items\":{\"msg_item\":";
			string templateB = "}};\r";

			int startIndex = responseText.IndexOf(templateA);

			if (startIndex >= 0)
			{
				int endIndex = responseText.IndexOf(templateB, startIndex);

				if (endIndex >= 0)
				{
					string data = responseText.Substring(startIndex + templateA.Length, endIndex - startIndex - templateA.Length);

					result = JSONSerializerExecute.Deserialize<WeChatRecentMessageCollection>(data);
				}
			}

			this._Messages = result;
		}
	}
}
