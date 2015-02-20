using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using MCS.Library.Passport;
using MCS.Library.Principal;

namespace ResponsivePassportService.TestPages
{
	public partial class OpenIDIdentityTest : System.Web.UI.Page
	{
		protected void Page_Load(object sender, EventArgs e)
		{

		}

		protected override void OnPreRender(EventArgs e)
		{
			if (OpenIDIdentity.Current != null)
			{
				this.openIDContainer.InnerText = OpenIDIdentity.Current.OpenID;

				OpenIDBinding binding = PassportSignInSettings.GetConfig().PersistOpenIDBinding.GetBindingByOpenID(OpenIDIdentity.Current.OpenID);

				if (binding != null)
					this.userIDContainer.InnerText = binding.UserID;
			}

			base.OnPreRender(e);
		}
	}
}