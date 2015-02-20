using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace MCS.Web.Passport.TestPages
{
	public partial class NeedAccessTicketPage : System.Web.UI.Page
	{
		protected void Page_Load(object sender, EventArgs e)
		{

		}

		protected override void OnPreRender(EventArgs e)
		{
			if (Request.QueryString["uid"] != null)
				paramValue.Text = HttpUtility.HtmlEncode(Request.QueryString["uid"]);

			base.OnPreRender(e);
		}

		protected void Unnamed1_TicketChecking(object sender, WebControls.AccessTicketCheckEventArgs eventArgs)
		{
			
		}
	}
}