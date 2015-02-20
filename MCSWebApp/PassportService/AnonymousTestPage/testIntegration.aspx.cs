using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using MCS.Library.Passport;

namespace MCS.Web.Passport.TestPages
{
	public partial class testIntegration : System.Web.UI.Page
	{
		protected void Page_Load(object sender, EventArgs e)
		{
			string t = Request.QueryString["t"];

			if (string.IsNullOrEmpty(t))
				RedirectToIntegrationPage();
			else
			{
				byte[] encTicket = Convert.FromBase64String(t);

				StringEncryption encryption = new StringEncryption();

				string ticketString = encryption.DecryptString(encTicket, PassportIntegrationSettings.GetConfig().GetDesKey());

				Ticket ticket = new Ticket(ticketString);

				Helper.ShowTicketInfo(ticket, ticketInfo);

				ticketXml.InnerText = ticketString;
			}
		}

		private void RedirectToIntegrationPage()
		{
			string redirectPage = string.Format("../Integration/WIRedirection.aspx?appID=testApp&ru={0}",
				this.Request.Url.ToString());

			Response.Redirect(redirectPage);
		}
	}
}
