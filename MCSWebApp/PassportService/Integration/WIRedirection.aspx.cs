using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using MCS.Library.Passport;
using System.Xml;
using MCS.Library.Core;

namespace MCS.Web.Passport.Integration
{
	public partial class WIRedirection : System.Web.UI.Page
	{
		protected void Page_Load(object sender, EventArgs e)
		{
			string returnUrl = HttpUtility.UrlDecode(Request.QueryString["ru"]);

			ITicket ticket = BuildTicket();

			if (string.IsNullOrEmpty(ticket.SignInInfo.UserID) == false)
			{
				XmlDocument xmlDoc = ticket.SaveToXml();

				StringEncryption encryption = new StringEncryption();

				byte[] encTicket = encryption.EncryptString(xmlDoc.OuterXml, PassportIntegrationSettings.GetConfig().GetDesKey());

				string t = Convert.ToBase64String(encTicket);

				if (returnUrl != null)
				{
					string redirectUrl = returnUrl;

					if (returnUrl.LastIndexOf('?') >= 0)
						redirectUrl += "&";
					else
						redirectUrl += "?";

					redirectUrl += "t=" + HttpUtility.UrlEncode(t);

					Response.Redirect(redirectUrl);
				}
				else
					Helper.ShowTicketInfo(ticket, ticketInfo);
			}
			else
				Helper.ShowTicketInfo(ticket, ticketInfo);
		}

		private static ITicket BuildTicket()
		{
			HttpRequest request = HttpContext.Current.Request;

			string appID = HttpUtility.UrlDecode(request.QueryString["appID"]);
			string ip = request.QueryString["ip"];

			if (string.IsNullOrEmpty(ip))
				ip = request.UserHostAddress;

			string logonName = request.ServerVariables["LOGON_USER"];
			LogOnIdentity loi = new LogOnIdentity(logonName);

			XmlDocument xmlDoc = XmlHelper.CreateDomDocument("<Ticket/>");
			XmlElement root = xmlDoc.DocumentElement;

			DateTime now = DateTime.Now;

			XmlHelper.AppendNode(root, "AppSSID", Guid.NewGuid().ToString());
			XmlHelper.AppendNode(root, "AppID", appID);
			XmlHelper.AppendNode(root, "AppSTime", now.ToString("yyyy-MM-dd HH:mm:ss"));
			XmlHelper.AppendNode(root, "IP", ip);

			XmlElement signInInfoNode = (XmlElement)XmlHelper.AppendNode(root, "SignInInfo");

			XmlHelper.AppendNode(signInInfoNode, "SSID", Guid.NewGuid().ToString());
			XmlHelper.AppendNode(signInInfoNode, "STime", now.ToString("yyyy-MM-dd HH:mm:ss"));
			XmlHelper.AppendNode(signInInfoNode, "UID", loi.LogOnNameWithoutDomain);
			XmlHelper.AppendNode(signInInfoNode, "WI", "True");
			XmlHelper.AppendNode(signInInfoNode, "DO", loi.Domain);
			XmlHelper.AppendNode(signInInfoNode, "AS", request.Url.Host + ":" + request.Url.Port);

			return new Ticket(xmlDoc.OuterXml);
		}

		private void RenderInfo(string returnUrl)
		{

		}
	}
}
