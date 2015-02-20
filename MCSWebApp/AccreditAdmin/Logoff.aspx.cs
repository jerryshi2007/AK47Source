using System;
using System.Data;
using System.Configuration;
using System.Collections;
using System.Web;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;
using System.Web.UI.HtmlControls;
using MCS.Library.Passport;

namespace MCS.Applications.AccreditAdmin
{
	public partial class Logoff : System.Web.UI.Page
	{
		protected void Page_Load(object sender, EventArgs e)
		{
			Response.Cache.SetNoStore();
			PassportManager.ClearSignInCookie();
			string url = this.Request.Url.AbsoluteUri;
			url = url.Substring(0, url.LastIndexOf("/") + 1);

			Response.Redirect(PassportManager.GetLogOnOrLogOffUrl(url + "OGUAdmin.aspx", true, true));
			
			//Response.Redirect(PassportManager.GetLogOnOrLogOffUrl("~/OGUAdmin.aspx", true));
		}
	}
}
