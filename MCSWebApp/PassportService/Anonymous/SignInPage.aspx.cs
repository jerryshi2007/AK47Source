using System;
using System.Collections;
using System.Configuration;
using System.Data;
using System.Web;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.HtmlControls;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;
using MCS.Library.Core;
using MCS.Library.Passport;
using MCS.Library.Web.Controls;

namespace MCS.Web.Passport
{
	public partial class SignInPage : System.Web.UI.Page
	{
		protected void Page_Load(object sender, EventArgs e)
		{
			Response.Cache.SetNoStore();
		}

		protected void SignInControl_BeforeAuthenticate(LogOnIdentity loi)
		{
			//如果需要验证码认证，将AuthenticationCode放在loi中即可
			//loi.Context["AuthenticationCode"] = UuidHelper.NewUuidString();
		}
	}
}
