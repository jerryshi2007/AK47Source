using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Text;
using MCS.Library.Core;
using MCS.Library.Passport.Social.Configuration;

namespace MCS.Library.Passport.Social.DataObjects
{
	/// <summary>
	/// 获取验证码的请求参数
	/// </summary>
	[Serializable]
	public class QQAuthorizationCodeRequestParams
	{
		public QQAuthorizationCodeRequestParams()
		{
		}

		public QQAuthorizationCodeRequestParams(string redirectUrl, string state)
		{
			redirectUrl.CheckStringIsNullOrEmpty("redirectUrl");

			this.RedirectUri = redirectUrl;
			this.State = state;
		}

		public string RedirectUri
		{
			get;
			set;
		}

		public string State
		{
			get;
			set;
		}

		public string Scope
		{
			get;
			set;
		}

		/// <summary>
		/// 转换成申请授权码的Url
		/// </summary>
		/// <returns></returns>
		/// <remarks>http://wiki.connect.qq.com/%e4%bd%bf%e7%94%a8authorization_code%e8%8e%b7%e5%8f%96access_token#Step1.EF.BC.9A.E8.8E.B7.E5.8F.96AuthorizationCode</remarks>
		public string ToUrl()
		{
			this.RedirectUri.CheckStringIsNullOrEmpty("RedirectUri");

			QQConnectionSettings settings = QQConnectionSettings.GetConfig();

			string result = settings.AuthorizationPath.ToString();

			NameValueCollection queryParams = new NameValueCollection();

			queryParams["response_type"] = "code";
			queryParams["client_id"] = settings.AppID;
			queryParams["redirect_uri"] = this.RedirectUri;
			queryParams["state"] = this.State;
			queryParams["scope"] = this.Scope;

			return UriHelper.CombineUrlParams(result, queryParams);
		}
	}
}
