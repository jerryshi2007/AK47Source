using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Services;
using System.Xml;
using MCS.Library.Core;
using MCS.Library.Passport;

namespace MCS.Web.Passport.Services
{
	/// <summary>
	/// Summary description for TicketService
	/// </summary>
	[WebService(Namespace = "http://tempuri.org/")]
	[WebServiceBinding(ConformsTo = WsiProfiles.BasicProfile1_1)]
	[System.ComponentModel.ToolboxItem(false)]
	[System.Web.Script.Services.ScriptService]
	public class TicketService : System.Web.Services.WebService
	{
		/// <summary>
		/// 解密票据，返回Ticket的xml
		/// </summary>
		/// <param name="ticket">加密的票据字符串(BASE64编码)</param>
		/// <returns>解密后的票据的xml</returns>
		[WebMethod]
		public string DecryptTicket(string encodedTicket)
		{
			encodedTicket.CheckStringIsNullOrEmpty("encodedTicket");

			ITicket ticket = Ticket.DecryptTicket(encodedTicket);

			XmlDocument xmlDoc = ticket.SaveToXml();

			return xmlDoc.OuterXml;
		}
	}
}
