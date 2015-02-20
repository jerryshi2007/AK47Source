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
	/// <summary>
	/// 发送文本消息给某个人
	/// </summary>
	public class WeChatSingleSendTextMessageExecutor : WeChatSingleSendMessageExecutorBase
	{
		public WeChatSingleSendTextMessageExecutor(string fakeID, string content, WeChatLoginInfo loginInfo)
			: base(fakeID, WeChatAppMessageType.Text, loginInfo)
		{
			content.CheckStringIsNullOrEmpty("content");

			this.Content = content;
		}

		public string Content
		{
			get;
			private set;
		}

		protected override void PreparePostParams(HttpWebRequest request, NameValueCollection postParams)
		{
			this.Content.CheckStringIsNullOrEmpty("Content");

			postParams["content"] = this.Content;
		}
	}
}
