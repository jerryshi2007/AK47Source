using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MCS.Library.Core;
using MCS.Web.Apps.WeChat.DataObjects;

namespace MCS.Web.Apps.WeChat.Executors
{
	/// <summary>
	/// 已经认证的执行器的基类，需要初始化LoginInfo才能使用
	/// </summary>
	public abstract class WeChatAuthenticatedExecutorBase : WeChatExecutorBase
	{
		public WeChatAuthenticatedExecutorBase(WeChatLoginInfo loginInfo)
		{
			loginInfo.NullCheck("loginInfo");

			this.LoginInfo = loginInfo;
		}

		public WeChatLoginInfo LoginInfo
		{
			get;
			private set;
		}
	}
}
