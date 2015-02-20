using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net;

namespace MCS.Web.Apps.WeChat.DataObjects
{
	[Serializable]
	public class WeChatLoginInfo
	{
		/// <summary>
		/// 登录后得到的令牌
		/// </summary>        
		public string Token
		{
			get;
			set;
		}

		/// <summary>
		/// 登录后得到的cookie
		/// </summary>
		public CookieContainer LoginCookie
		{
			get;
			set;
		}

		/// <summary>
		/// 创建时间
		/// </summary>
		public DateTime CreateDate
		{
			get;
			set;
		}

		/// <summary>
		/// 微信公众号的ID
		/// </summary>
		public string AccountID
		{
			get;
			set;
		}

		public string Ticket
		{
			get;
			set;
		}

		public string Error
		{
			get;
			set;
		}

		public override string ToString()
		{
			return string.Format("Token: {0}, Account ID: {1}, Ticket: {2}\n", this.Token, this.AccountID, this.Ticket);
		}
	}
}
