using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using MCS.Library.Passport;
using MCS.Library.Core;

namespace MCS.Web.Passport.Integration
{
	public partial class WISignInPage : System.Web.UI.Page
	{
		protected void Page_Load(object sender, EventArgs e)
		{

		}

		protected ISignInUserInfo SignInControl_InitSignInControl()
		{
			DefaultSignInUserInfo userInfo = null;

			string logonName = PassportManager.DecryptUserID(Request.QueryString["uid"]);

			string windowsLogonName = Request.ServerVariables["LOGON_USER"];

			if (logonName.IsNullOrEmpty())
				logonName = windowsLogonName;

			if (string.IsNullOrEmpty(logonName) == false)
			{
				userInfo = new DefaultSignInUserInfo();

				LogOnIdentity loi = new LogOnIdentity(logonName);

				userInfo.UserID = loi.LogOnNameWithoutDomain;
				userInfo.Domain = loi.Domain;
				userInfo.Properties["WindowsIntegrated"] = true;

				LogOnIdentity wloi = new LogOnIdentity(windowsLogonName);

				userInfo.OriginalUserID = wloi.LogOnNameWithoutDomain;
			}

			return userInfo;
		}
	}
}
