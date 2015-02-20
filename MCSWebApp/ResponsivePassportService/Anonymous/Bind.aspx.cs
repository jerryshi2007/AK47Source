using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using MCS.Library.Core;
using MCS.Library.Passport;
using MCS.Library.Web.Controls;

namespace ResponsivePassportService.Anonymous
{
	public partial class Bind : System.Web.UI.Page
	{
		protected void Page_Load(object sender, EventArgs e)
		{

		}

		protected void signInControl_AfterSignIn(SignInContext context)
		{
			if (context.ResultType == SignInResultType.Success)
			{
				string consistentID = PassportSignInSettings.GetConfig().UserIDConverter.GetUserConsistentID(context.UserID);

				OpenIDBinding binding = new OpenIDBinding();

				binding.UserID = consistentID;
				binding.OpenID = Request.QueryString.GetValue("openID", string.Empty);
				binding.OpenIDType = Request.QueryString.GetValue("openIDType", string.Empty);

				PassportSignInSettings.GetConfig().PersistOpenIDBinding.SaveOpenIDBinding(binding);
			}
		}

		protected ISignInUserInfo signInControl_InitSignInControl()
		{
			DefaultSignInUserInfo result = null;

			string openID = Request.QueryString.GetValue("openID", string.Empty);

			if (openID.IsNotEmpty())
			{
				OpenIDBinding binding = PassportSignInSettings.GetConfig().PersistOpenIDBinding.GetBindingByOpenID(openID);

				if (binding != null)
				{
					result = new DefaultSignInUserInfo();

					string logonName = PassportSignInSettings.GetConfig().UserIDConverter.GetUserLogonName(binding.UserID);

					result.UserID = logonName;
					result.OriginalUserID = logonName;
				}
			}

			return result;
		}
	}
}