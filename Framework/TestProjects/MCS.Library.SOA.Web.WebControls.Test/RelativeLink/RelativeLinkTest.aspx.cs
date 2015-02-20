using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using MCS.Library.SOA.DataObjects;

namespace MCS.Library.SOA.Web.WebControls.Test.RelativeLink
{
	public partial class RelativeLinkTest : System.Web.UI.Page
	{
		protected void Page_Load(object sender, EventArgs e)
		{

		}

		protected override void OnPreRender(EventArgs e)
		{
			popupLink.HRef = RelativeTicket.GetRequestTicketUrl("bridge.aspx", "popupForm.aspx?resourceID=abcd");
		
			base.OnPreRender(e);
		}
	}
}