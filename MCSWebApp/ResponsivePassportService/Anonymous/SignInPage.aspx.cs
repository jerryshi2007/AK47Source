using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using MCS.Library.Core;

namespace ResponsivePassportService.Anonymous
{
	public partial class SignInPage : System.Web.UI.Page
	{
		protected void Page_Load(object sender, EventArgs e)
		{

		}

		protected void signInControl_BeforeAuthenticate(MCS.Library.Core.LogOnIdentity loi)
		{
			//如果需要验证码认证，将AuthenticationCode放在loi中即可
			//loi.Context["AuthenticationCode"] = UuidHelper.NewUuidString();
			//loi.Context["AlternativeUserIDs"] = new string[] { "fanhy" };
		}
	}
}