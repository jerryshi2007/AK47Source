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
	/// 获取访问UserInfo的请求参数
	/// </summary>
	[Serializable]
	public class QQGetUserInfoRequestParams
	{
		public QQGetUserInfoRequestParams()
		{
		}

		public QQGetUserInfoRequestParams(string accessToken, string openID)
		{
			accessToken.CheckStringIsNullOrEmpty("accessToken");
			openID.CheckStringIsNullOrEmpty("openID");

			this.AccessToken = accessToken;
			this.OpenID = openID;
		}

		public string AccessToken
		{
			get;
			set;
		}

		public string OpenID
		{
			get;
			set;
		}

		/// <summary>
		/// 转换成申请访问码的Url
		/// </summary>
		/// <returns></returns>
		/// <remarks>http://wiki.connect.qq.com/openapi%e8%b0%83%e7%94%a8%e8%af%b4%e6%98%8e_oauth2-0</remarks>
		public string ToUrl()
		{
			this.AccessToken.CheckStringIsNullOrEmpty("AccessToken");
			this.OpenID.CheckStringIsNullOrEmpty("OpenID");

			QQConnectionSettings settings = QQConnectionSettings.GetConfig();

			string result = settings.GetUserInfoPath.ToString();

			NameValueCollection queryParams = new NameValueCollection();

			queryParams["access_token"] = this.AccessToken;
			queryParams["oauth_consumer_key"] = settings.AppID;
			queryParams["openid"] = this.OpenID;

			return UriHelper.CombineUrlParams(result, queryParams);
		}
	}
}
