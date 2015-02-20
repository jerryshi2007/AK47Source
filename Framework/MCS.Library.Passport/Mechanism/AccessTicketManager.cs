using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web;
using MCS.Library.Core;
using MCS.Library.Globalization;

namespace MCS.Library.Passport
{
	/// <summary>
	/// 页面访为票据的访问类
	/// </summary>
	public static class AccessTicketManager
	{
		/// <summary>
		/// 从url中得到票据
		/// </summary>
		/// <returns></returns>
		public static AccessTicket GetAccessTicket()
		{
			Common.CheckHttpContext();

			HttpRequest request = HttpContext.Current.Request;

			string aTicketString = request.QueryString[AccessTicket.AccessTicketParamName];

			AccessTicket result = null;

			if (aTicketString.IsNotEmpty())
				result = AccessTicket.FromString(Common.DecryptString(aTicketString));

			return result;
		}

		/// <summary>
		/// 是否是合法的票据
		/// </summary>
		/// <param name="timeout">有效期</param>
		/// <returns></returns>
		public static bool IsValidAccessTicket(TimeSpan timeout)
		{
			bool result = false;

			AccessTicket ticket = GetAccessTicket();

			if (ticket != null)
				result = ticket.TimeStampIsValid(timeout);

			return result;
		}

		/// <summary>
		/// 从Url中的参数中，检查访问票据
		/// </summary>
		/// <param name="matchedUrl">需要匹配的url，如果为null，表示不需要检查</param>
		/// <param name="urlCheckParts">Url中需要检查的部分</param>
		/// <param name="timeout">有效期</param>
		public static AccessTicket CheckAccessTicket(Uri matchedUrl, AccessTicketUrlCheckParts urlCheckParts, TimeSpan timeout)
		{
			AccessTicket ticket = GetAccessTicket();

			(ticket == null).TrueThrow<AccessTicketCheckException>(Translator.Translate(Define.DefaultCategory, "您没有权限访问此页面"));

			(ticket.TimeStampIsValid(timeout)).FalseThrow<AccessTicketCheckException>
				(Translator.Translate(Define.DefaultCategory, "访问票据已经过期，您没有权限访问此页面"));

			if (matchedUrl != null)
				ticket.UrlIsValid(matchedUrl, urlCheckParts).FalseThrow<AccessTicketCheckException>
					(Translator.Translate(Define.DefaultCategory, "票据中的地址不匹配，您没有权限访问此页面"));

			return ticket;
		}

		/// <summary>
		/// 生成访问票据
		/// </summary>
		/// <returns></returns>
		public static AccessTicket GenerateTicket()
		{
			AccessTicket aTicket = new AccessTicket();

			aTicket.GenerateTime = DateTime.Now;

			return aTicket;
		}
	}
}
