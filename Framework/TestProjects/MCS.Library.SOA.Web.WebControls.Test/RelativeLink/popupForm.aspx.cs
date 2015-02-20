using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using MCS.Library.SOA.DataObjects;

namespace MCS.Library.SOA.Web.WebControls.Test.RelativeLink
{
	public partial class popupForm : System.Web.UI.Page
	{
		protected void Page_Load(object sender, EventArgs e)
		{
			if (Request.QueryString["requestTicket"] != null)
			{
				RelativeTicket ticket = RelativeTicket.DecryptFromString((string)Request.QueryString["requestTicket"]);

				ticket.CheckIsValid();
			}
		}

		protected override void OnPreRender(EventArgs e)
		{
			if (Request.UrlReferrer != null)
				httpRefer.Text = HttpUtility.HtmlEncode(Request.UrlReferrer.ToString());

			base.OnPreRender(e);
		}
	}
}