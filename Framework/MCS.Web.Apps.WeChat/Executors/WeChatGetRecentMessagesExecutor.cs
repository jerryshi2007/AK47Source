using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Net;
using System.Text;
using MCS.Library.Core;
using MCS.Web.Apps.WeChat.DataObjects;
using MCS.Web.Library.Script;

namespace MCS.Web.Apps.WeChat.Executors
{
	/// <summary>
	/// 获取用户最近的消息
	/// </summary>
	public class WeChatGetRecentMessagesExecutor : WeChatAuthenticatedExecutorBase
	{
		private WeChatRecentMessageCollection _Messages = new WeChatRecentMessageCollection();
		private int _PageSize = 20;

		public WeChatGetRecentMessagesExecutor(int pageIndex, int pageSize, WeChatLoginInfo loginInfo)
			: base(loginInfo)
		{
			this.PageIndex = pageIndex;
			this.PageSize = pageSize;
		}

		public WeChatRecentMessageCollection Messages
		{
			get
			{
				return this._Messages;
			}
		}

		public int PageSize
		{
			get
			{
				return this._PageSize;
			}
			set
			{
				this._PageSize = value;
			}
		}

		public int PageIndex
		{
			get;
			private set;
		}

		protected override HttpWebRequest PrepareWebRequest()
		{
			NameValueCollection parameters = new NameValueCollection();

			parameters["t"] = "message/list";
			parameters["offset"] = (this.PageIndex * this.PageSize).ToString();
			parameters["count"] = this.PageSize.ToString();
			parameters["day"] = "7";
			parameters["token"] = this.LoginInfo.Token;
			parameters["lang"] = "zh-CN";

			string url = "https://mp.weixin.qq.com/cgi-bin/message?" + parameters.ToUrlParameters(true);

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

			string templateA = "list : ({\"msg_item\":";
			string templateB = "}).msg_item,\r";

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
