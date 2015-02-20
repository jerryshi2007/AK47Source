using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using MCS.Library.Passport;
using MCS.Library.Core;

namespace MCS.Web.Passport.TestPages
{
	public partial class DecryptTicketTest : System.Web.UI.Page
	{
		protected void Page_Load(object sender, EventArgs e)
		{

		}

		protected override void OnPreRender(EventArgs e)
		{
			this.ticketString.Text = PrepareTicket();

			base.OnPreRender(e);
		}

		private static string PrepareTicket()
		{
			ISignInInfo signInInfo = SignInInfo.Create("SinoOceanLand\\liumh");

			ITicket ticket = Ticket.Create(signInInfo);

			return ticket.ToEncryptString();
		}
	}
}