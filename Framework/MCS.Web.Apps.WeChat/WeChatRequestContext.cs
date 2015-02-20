using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MCS.Library.Caching;
using MCS.Library.Core;
using MCS.Web.Apps.WeChat.DataObjects;

namespace MCS.Web.Apps.WeChat
{
	/// <summary>
	/// 微信请求过程中的上下文信息
	/// </summary>
	[ActionContextDescription(Key = "WeChatRequestContext")]
	public class WeChatRequestContext : ActionContextBase<WeChatRequestContext>
	{
		public WeChatLoginInfo LoginInfo
		{
			get;
			set;
		}
	}
}
