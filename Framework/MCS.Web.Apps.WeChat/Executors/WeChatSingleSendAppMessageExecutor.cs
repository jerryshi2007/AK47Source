using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using MCS.Library.Core;
using MCS.Web.Apps.WeChat.DataObjects;
using System.Collections.Specialized;

namespace MCS.Web.Apps.WeChat.Executors
{
	/// <summary>
	/// 发送图文消息给某个人
	/// </summary>
	public class WeChatSingleSendAppMessageExecutor : WeChatSingleSendMessageExecutorBase
	{
		public WeChatSingleSendAppMessageExecutor(string fakeID, string appMsgID, WeChatLoginInfo loginInfo)
			: base(fakeID, WeChatAppMessageType.Html, loginInfo)
		{
			appMsgID.CheckStringIsNullOrEmpty("appMsgID");

			this.AppMessageID = appMsgID;
		}

		public string AppMessageID
		{
			get;
			private set;
		}

		protected override void PreparePostParams(HttpWebRequest request, NameValueCollection postParams)
		{
			this.AppMessageID.CheckStringIsNullOrEmpty("AppMessageID");

			postParams["app_id"] = this.AppMessageID;
			postParams["appmsgid"] = this.AppMessageID;
		}
	}
}
