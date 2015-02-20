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
	/// <summary>
	/// 为组群发消息的执行器
	/// </summary>
	public class WeChatMassSendTextMessageExecutor : WeChatMassSendMessageExecutorBase
	{
		public WeChatMassSendTextMessageExecutor(int groupID, string message, WeChatLoginInfo loginInfo)
			: base(groupID, WeChatAppMessageType.Text, loginInfo)
		{
			message.IsNotEmpty().FalseThrow("待发送的消息不能为空");

			this.Message = message;
		}

		public string Message
		{
			get;
			set;
		}

		protected override void ProcessResponseText(string responseText)
		{
			WeChatSendMessageRetInfo result = JSONSerializerExecute.Deserialize<WeChatSendMessageRetInfo>(responseText);

			if (result.ret != 0)
				throw new ApplicationException(result.msg);
		}

		protected override void PreparePostParams(HttpWebRequest request, NameValueCollection postParams)
		{
			postParams["content"] = this.Message;
		}
	}
}
