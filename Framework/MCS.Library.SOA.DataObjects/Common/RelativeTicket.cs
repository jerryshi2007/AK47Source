using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web;
using MCS.Library.Core;
using MCS.Library.Principal;
using MCS.Web.Library;
using MCS.Web.Library.Script;

namespace MCS.Library.SOA.DataObjects
{
	/// <summary>
	/// 两个页面之间相互引用的Ticket
	/// </summary>
	public class RelativeTicket
	{
		private string userID = string.Empty;
		private string clientIP = string.Empty;
		private DateTime clickTime = DateTime.MinValue;
		private string uriReferer = null;
		private string targetUri = null;

		public RelativeTicket()
		{
		}

		/// <summary>
		/// 根据上下文环境生成一个Ticket
		/// </summary>
		/// <returns></returns>
		public static RelativeTicket GenerateTicket(string target)
		{
			RelativeTicket ticket = new RelativeTicket();

			if (DeluxePrincipal.IsAuthenticated)
				ticket.userID = DeluxeIdentity.CurrentUser.ID;

			ticket.clientIP = EnvironmentHelper.GetClientIP();
			ticket.clickTime = DateTime.UtcNow;
			ticket.targetUri = target;

			if (EnvironmentHelper.Mode == InstanceMode.Web)
				ticket.uriReferer = HttpContext.Current.Request.Url.ToString();

			return ticket;
		}

		/// <summary>
		/// 检查当前UriReferer和Ticket中的的一致性
		/// </summary>
		public void CheckUriReferer()
		{
			if (EnvironmentHelper.Mode == InstanceMode.Web && this.uriReferer.IsNotEmpty())
			{
				Uri currentUriReferer = HttpContext.Current.Request.UrlReferrer;

				(currentUriReferer != null).FalseThrow("不能从当前请求中获取UrlReferer，请确认当前页面是从其它页面链接过来的");

				(string.Compare(currentUriReferer.ToString(), this.uriReferer, true) == 0).FalseThrow(
					"当前的UrlReferer是{0}，票据中的Url是{1}，两者不一致", currentUriReferer, this.uriReferer);
			}
		}

		/// <summary>
		/// 得到一个加密的串
		/// </summary>
		/// <returns></returns>
		public string EncryptToString()
		{
			string data = JSONSerializerExecute.Serialize(this);
			byte[] encData = RelativeTicketSettings.GetConfig().Encryptor.EncryptString(data);

			return Convert.ToBase64String(encData);
		}

		public static RelativeTicket DecryptFromString(string ticketString)
		{
			ExceptionHelper.CheckStringIsNullOrEmpty(ticketString, "ticketString");

			byte[] data = Convert.FromBase64String(ticketString);
			string decData = RelativeTicketSettings.GetConfig().Encryptor.DecryptString(data);
			RelativeTicket ticket = (RelativeTicket)JSONSerializerExecute.DeserializeObject(decData, typeof(RelativeTicket));

			return ticket;
		}

		public static string GetRequestTicketUrl(string requestUrl, string redirectUrl)
		{
			string result = requestUrl;

			if (result.IndexOf("?") >= 0)
				result += "&";
			else
				result += "?";

			RelativeTicket ticket = RelativeTicket.GenerateTicket(redirectUrl);

			return result + "requestTicket=" + HttpUtility.UrlEncode(ticket.EncryptToString());
		}


		/// <summary>
		/// 用户ID
		/// </summary>
		public string UserID
		{
			get
			{
				return this.userID;
			}
			set
			{
				this.userID = value;
			}
		}

		/// <summary>
		/// 点击时间
		/// </summary>
		public DateTime ClickTime
		{
			get
			{
				return this.clickTime;
			}
			set
			{
				this.clickTime = value;
			}
		}

		/// <summary>
		/// 客户端IP
		/// </summary>
		public string ClientIP
		{
			get
			{
				return this.clientIP;
			}
			set
			{
				this.clientIP = value;
			}
		}

		/// <summary>
		/// 参照引用的的Url
		/// </summary>
		public string UriReferer
		{
			get
			{
				return this.uriReferer;
			}
			set
			{
				this.uriReferer = value;
			}
		}

		/// <summary>
		/// 目标Uri
		/// </summary>
		public string TargetUri
		{
			get
			{
				return this.targetUri;
			}
			set
			{
				this.targetUri = value;
			}
		}

		/// <summary>
		/// 判断Ticket和当前HttpContext中的信息是否吻合，包括ClientIP，身份和时间差
		/// </summary>
		/// <returns></returns>
		public bool IsValid(out string reason)
		{
			bool result = true;

			DateTime now = DateTime.UtcNow;

			TimeSpan tSpan = now - this.ClickTime;

			tSpan = TimeSpan.FromTicks(Math.Abs(tSpan.Ticks));

			string currentClientIP = EnvironmentHelper.GetClientIP();
			string currenctUserID = DeluxePrincipal.IsAuthenticated ? DeluxeIdentity.CurrentUser.ID : this.UserID;

			try
			{
				(string.Compare(currentClientIP, this.ClientIP, true) == 0).FalseThrow<InvalidOperationException>(
					"Ticket中的ClientIP为{0}，当前请求的ClientIP为{1}，不匹配", this.ClientIP, currentClientIP);

				(string.Compare(currenctUserID, this.UserID, true) == 0).FalseThrow<InvalidOperationException>(
					"Ticket中的UserID为{0}，当前请求的UserID为{1}，不匹配", this.UserID, currenctUserID);

				 (tSpan.CompareTo(RelativeTicketSettings.GetConfig().UrlTransferTimeout) <= 0).FalseThrow<InvalidOperationException>(
					 "Ticket中的ClickTime为{0}，与服务器当前时间{1}差别太大", this.ClickTime, now);

				 reason = string.Empty;
			}
			catch(InvalidOperationException ex)
			{
				reason = ex.Message;
				result = false;
			}

			return result;
		}

		public void CheckIsValid()
		{
			string reason;

			IsValid(out reason).FalseThrow(reason);
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="obj"></param>
		/// <returns></returns>
		public override bool Equals(object obj)
		{
			bool result = false;

			RelativeTicket objParam = (RelativeTicket)obj;

			TimeSpan tSpan = objParam.ClickTime - this.ClickTime;

			if (objParam.ClientIP == this.ClientIP && objParam.UserID == this.UserID
					&& tSpan.CompareTo(RelativeTicketSettings.GetConfig().UrlTransferTimeout) <= 0)
				result = true;

			return result;
		}

		public override int GetHashCode()
		{
			return base.GetHashCode();
		}
	}
}
