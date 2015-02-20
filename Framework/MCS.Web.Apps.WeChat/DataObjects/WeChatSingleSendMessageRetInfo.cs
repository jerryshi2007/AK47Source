using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MCS.Web.Apps.WeChat.DataObjects
{
	/// <summary>
	/// 单发消息的返回结果
	/// {"base_resp":{"ret":0,"err_msg":"ok"}}
	/// </summary>
	[Serializable]
	public class WeChatSingleSendMessageRetInfo : WeChatRetInfoWithBaseResponse
	{
	}
}
