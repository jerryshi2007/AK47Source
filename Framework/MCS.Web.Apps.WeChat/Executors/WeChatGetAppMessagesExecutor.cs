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
	public class WeChatGetAppMessagesExecutor : WeChatAuthenticatedExecutorBase
	{
		private int _PageSize = 10;
		private WeChatAppMessageType _MessageType = WeChatAppMessageType.Html;
		private WeChatAppMessageCollection _AppMessages = new WeChatAppMessageCollection();

		public WeChatGetAppMessagesExecutor(int pageIndex, int pageSize, WeChatAppMessageType messageType, WeChatLoginInfo loginInfo)
			: base(loginInfo)
		{
			this.PageIndex = pageIndex;
			this.PageSize = pageSize;
			this._MessageType = messageType;
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

		public WeChatAppMessageType MessageType
		{
			get
			{
				return this._MessageType;
			}
			private set
			{
				this._MessageType = value;
			}
		}

		public WeChatAppMessageCollection AppMessages
		{
			get
			{
				return this._AppMessages;
			}
		}

		protected override HttpWebRequest PrepareWebRequest()
		{
			NameValueCollection parameters = new NameValueCollection();

			parameters["t"] = "media/appmsg_list";
			parameters["begin"] = (this.PageIndex * this.PageSize).ToString();
			parameters["count"] = this.PageSize.ToString();
			parameters["type"] = ((int)this.MessageType).ToString();
			parameters["action"] = "list";
			parameters["token"] = this.LoginInfo.Token;
			parameters["lang"] = "zh-CN";

			string url = "https://mp.weixin.qq.com/cgi-bin/appmsg?" + parameters.ToUrlParameters(true);

			HttpWebRequest request = (HttpWebRequest)WebRequest.Create(url);

			request.CookieContainer = this.LoginInfo.LoginCookie;
			request.ContentType = "text/html; charset=UTF-8";
			request.Method = "GET";
			request.UserAgent = "Mozilla/5.0 (Windows NT 5.1; rv:2.0.1) Gecko/20100101 Firefox/4.0.1";

			return request;
		}

		protected override void ProcessResponseText(string responseText)
		{
			string templateA = "wx.cgiData = {\"item\":";
			string templateB = ",\"file_cnt\":{";
			int startIndex = responseText.IndexOf(templateA);

			if (startIndex >= 0)
			{
				int endIndex = responseText.IndexOf(templateB, startIndex);

				if (endIndex >= 0)
				{
					string data = responseText.Substring(startIndex + templateA.Length, endIndex - startIndex - templateA.Length);

					this._AppMessages = JSONSerializerExecute.Deserialize<WeChatAppMessageCollection>(data);

					this._AppMessages.ForEach(m => m.MessageType = this.MessageType);
				}
			}
		}
	}
}
