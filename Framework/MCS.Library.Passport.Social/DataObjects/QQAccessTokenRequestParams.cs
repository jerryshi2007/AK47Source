using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MCS.Library.Core;
using MCS.Library.Passport.Social.Configuration;
using System.Collections.Specialized;

namespace MCS.Library.Passport.Social.DataObjects
{
	/// <summary>
	/// 获取访问码的请求参数
	/// </summary>
	[Serializable]
	public class QQAccessTokenRequestParams
	{
		public QQAccessTokenRequestParams()
		{
		}

		public QQAccessTokenRequestParams(string redirectUrl, string code)
		{
			redirectUrl.CheckStringIsNullOrEmpty("redirectUrl");
			code.CheckStringIsNullOrEmpty("code");

			this.RedirectUri = redirectUrl;
			this.Code = code;
		}

		public string RedirectUri
		{
			get;
			set;
		}

		public string Code
		{
			get;
			set;
		}

		/// <summary>
		/// 转换成申请访问码的Url
		/// </summary>
		/// <returns></returns>
		/// <remarks>http://wiki.connect.qq.com/%e4%bd%bf%e7%94%a8authorization_code%e8%8e%b7%e5%8f%96access_token#Step2.EF.BC.9A.E9.80.9A.E8.BF.87AuthorizationCode.E8.8E.B7.E5.8F.96AccessToken</remarks>
		public string ToUrl()
		{
			this.RedirectUri.CheckStringIsNullOrEmpty("RedirectUri");
			this.Code.CheckStringIsNullOrEmpty("Code");

			QQConnectionSettings settings = QQConnectionSettings.GetConfig();

			string result = settings.AccessTokenPath.ToString();

			NameValueCollection queryParams = new NameValueCollection();

			queryParams["grant_type"] = "authorization_code";
			queryParams["client_id"] = settings.AppID;
			queryParams["client_secret"] = settings.AppKey;
			queryParams["redirect_uri"] = this.RedirectUri;
			queryParams["code"] = this.Code;

			return UriHelper.CombineUrlParams(result, queryParams);
		}
	}
}
