using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using MCS.Library.Core;
using MCS.Library.Passport.Social.Configuration;
using MCS.Library.Passport.Social.DataObjects;
using MCS.Library.Passport.Social.Mechanism;
using System.Collections.Specialized;

namespace ResponsivePassportService.Anonymous
{
	public partial class QQLoginCallback : System.Web.UI.Page
	{
		protected void Page_Load(object sender, EventArgs e)
		{

		}

		protected override void OnPreRender(EventArgs e)
		{
			this.bridge.Attributes["src"] = GeBridgeUri();
			this.bridge.Attributes["onload"] = "top.close()";

			base.OnPreRender(e);
		}

		private string GeBridgeUri()
		{
			string state = Request.QueryString.GetValue("state", string.Empty);

			string result = string.Empty;

			if (state.IsNotEmpty())
			{
				string code = Request.QueryString.GetValue("Code", string.Empty);

				if (code.IsNotEmpty())
				{
					SocialUserInfo userInfo = GetQQUserInfo();

					NameValueCollection parameters = UriHelper.GetUriParamsCollection(state);

					parameters["openID"] = userInfo.OpenID;
					parameters["openIDType"] = "QQ";
					parameters["userName"] = userInfo.UserName;

					//QQAccessTokenRequestParams requestParams = new QQAccessTokenRequestParams(
					//    QQConnectionSettings.GetConfig().LoginCallback.ToString(),
					//    code);

					//QQAccessTokenResponseParams responseParams = QQLoginConnectionManager.GetAccessToken(requestParams);

					//QQGetOpenIDRequestParams getOpenParams = new QQGetOpenIDRequestParams(responseParams.AccessToken);

					//QQGetOpenIDResponseParams openIDResponse = QQLoginConnectionManager.GetOpenID(getOpenParams);

					//NameValueCollection parameters = UriHelper.GetUriParamsCollection(state);

					//parameters["openID"] = openIDResponse.OpenID;
					//parameters["openIDType"] = "QQ";

					result = UriHelper.CombineUrlParams(state, parameters);
				}
			}

			return result;
		}

		private SocialUserInfo GetQQUserInfo()
		{
			SocialUserInfo result = null;

			string code = Request.QueryString.GetValue("Code", string.Empty);

			if (code.IsNotEmpty())
			{
				QQAccessTokenRequestParams requestParams = new QQAccessTokenRequestParams(
					QQConnectionSettings.GetConfig().LoginCallback.ToString(),
					code);

				QQAccessTokenResponseParams responseParams = QQLoginConnectionManager.GetAccessToken(requestParams);

				QQGetOpenIDRequestParams getOpenParams = new QQGetOpenIDRequestParams(responseParams.AccessToken);

				QQGetOpenIDResponseParams openIDResponse = QQLoginConnectionManager.GetOpenID(getOpenParams);

				QQGetUserInfoResponseParams getUserInfoResponse = QQLoginConnectionManager.GetUserInfo(new QQGetUserInfoRequestParams(getOpenParams.AccessToken, openIDResponse.OpenID));

				result = getUserInfoResponse.ToSocialUserInfo();
			}

			return result;
		}
	}
}