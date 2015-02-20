using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Xml;
using MCS.Library.Core;
using MCS.Library.Passport;
using MCS.Web.Library.MVC;
using MCS.Web.Library;

namespace MCS.Web.Passport.Services
{
	/// <summary>
	/// Summary description for TicketServiceHandler
	/// </summary>
	public class TicketServiceHandler : IHttpHandler
	{
		private XmlDocument _XmlResponse = null;

		public void ProcessRequest(HttpContext context)
		{
			context.Response.ContentType = "text/xml";

			try
			{
				ControllerHelper.ExecuteMethodByRequest(this);

				if (this._XmlResponse == null)
					throw new ApplicationException("不支持的调用格式");
			}
			catch (System.Exception ex)
			{
				this._XmlResponse = BuildExceptionXml(ex);
			}

			if (this._XmlResponse != null)
				this._XmlResponse.Save(context.Response.OutputStream);
		}

		public bool IsReusable
		{
			get
			{
				return false;
			}
		}

		[ControllerMethod]
		protected void ExecuteMethod(string method, string encodedTicket)
		{
			if (method.IsNullOrEmpty())
				method = "DecryptTicket";

			encodedTicket.NullCheck("encodedTicket");

			switch (method.ToLower())
			{
				case "decryptticket":
					this._XmlResponse = DecryptTicket(encodedTicket);
					break;
				default:
					throw new ApplicationException(string.Format("\"{0}\"是不支持的method", method));
			}
		}

		private static XmlDocument BuildExceptionXml(System.Exception ex)
		{
			ex = ex.GetRealException();

			XmlDocument xmlDoc = XmlHelper.CreateDomDocument("<Exception/>");

			XmlHelper.AppendNode(xmlDoc.DocumentElement, "Message", ex.Message);

			if (WebUtility.AllowResponseExceptionStackTrace())
				XmlHelper.AppendNode(xmlDoc.DocumentElement, "StackTrace", ex.StackTrace);

			return xmlDoc;
		}

		private static XmlDocument DecryptTicket(string encodedTicket)
		{
			encodedTicket.CheckStringIsNullOrEmpty("encodedTicket");

			ITicket ticket = Ticket.DecryptTicket(encodedTicket);

			(ticket == null).TrueThrow("传入的票据内容非法");

			return ticket.SaveToXml();
		}
	}
}