using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Net;
using System.Text;
using MCS.Library.Core;
using MCS.Web.Apps.WeChat.DataObjects;

namespace MCS.Web.Apps.WeChat.Executors
{
	public class WeChatGetLoginInExtraInfoExecutor : WeChatAuthenticatedExecutorBase
	{
		public WeChatGetLoginInExtraInfoExecutor(WeChatLoginInfo loginInfo)
			: base(loginInfo)
		{
		}

		protected override HttpWebRequest PrepareWebRequest()
		{
			NameValueCollection parameters = new NameValueCollection();

			parameters["t"] = "message/list";
			parameters["token"] = this.LoginInfo.Token;
			parameters["lang"] = "zh-CN";

			string url = "https://mp.weixin.qq.com/cgi-bin/home?" + parameters.ToUrlParameters(true);

			HttpWebRequest request = (HttpWebRequest)WebRequest.Create(url);

			request.CookieContainer = this.LoginInfo.LoginCookie;
			request.ContentType = "text/html; charset=UTF-8";
			request.Method = "GET";
			request.UserAgent = "Mozilla/5.0 (Windows NT 5.1; rv:2.0.1) Gecko/20100101 Firefox/4.0.1";

			return request;
		}

		protected override void ProcessResponseText(string responseText)
		{
			string templateA = "            ticket:\"";
			string templateB = "\",\r";

			int startIndex = responseText.IndexOf(templateA);

			if (startIndex >= 0)
			{
				int endIndex = responseText.IndexOf(templateB, startIndex);

				if (endIndex >= 0)
				{
					string ticket = responseText.Substring(startIndex + templateA.Length, endIndex - startIndex - templateA.Length);

					this.LoginInfo.Ticket = ticket;
				}
			}
		}
	}
}
