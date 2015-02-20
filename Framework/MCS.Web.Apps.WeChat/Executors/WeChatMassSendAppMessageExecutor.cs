using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Web;
using MCS.Library.Core;
using MCS.Web.Apps.WeChat.DataObjects;
using MCS.Web.Library.Script;

namespace MCS.Web.Apps.WeChat.Executors
{
	public class WeChatMassSendAppMessageExecutor : WeChatMassSendMessageExecutorBase
	{
		public WeChatMassSendAppMessageExecutor(int groupID, string appMsgID, WeChatLoginInfo loginInfo)
			: base(groupID, WeChatAppMessageType.Html, loginInfo)
		{
			appMsgID.IsNotEmpty().FalseThrow("待发送的消息ID不能为空");

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

			postParams["appmsgid"] = this.AppMessageID;
		}
	}
}
