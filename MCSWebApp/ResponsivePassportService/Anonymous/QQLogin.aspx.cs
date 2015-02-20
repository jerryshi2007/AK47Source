using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using MCS.Library.Core;
using MCS.Library.Passport.Social.DataObjects;
using MCS.Library.Passport.Social.Configuration;

namespace ResponsivePassportService.Anonymous
{
	public partial class QQLogin : System.Web.UI.Page
	{
		protected void Page_Load(object sender, EventArgs e)
		{
			//提供在获取到OpenID时的转发页面，主要用于脚本间的跨域访问
			Uri bridgeRelativeUrl = new Uri(this.ResolveUrl("OpenIDBridge.aspx"), UriKind.Relative);

			Uri bridgeUri = bridgeRelativeUrl.MakeAbsolute(this.Request.Url);

			QQAuthorizationCodeRequestParams requestParams =
				new QQAuthorizationCodeRequestParams(QQConnectionSettings.GetConfig().LoginCallback.ToString(),
					bridgeUri.ToString());

			Response.Redirect(requestParams.ToUrl());
		}
	}
}