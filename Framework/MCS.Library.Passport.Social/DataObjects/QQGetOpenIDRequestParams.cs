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
	/// 获取OpenID的请求参数
	/// </summary>
	[Serializable]
	public class QQGetOpenIDRequestParams
	{
		public QQGetOpenIDRequestParams()
		{
		}

		public QQGetOpenIDRequestParams(string accessToken)
		{
			accessToken.CheckStringIsNullOrEmpty("accessToken");
			this.AccessToken = accessToken;
		}

		public string AccessToken
		{
			get;
			set;
		}

		/// <summary>
		/// 转换成申请授权码的Url
		/// </summary>
		/// <returns></returns>
		/// <remarks>http://wiki.connect.qq.com/%e8%8e%b7%e5%8f%96%e7%94%a8%e6%88%b7openid_oauth2-0</remarks>
		public string ToUrl()
		{
			this.AccessToken.CheckStringIsNullOrEmpty("AccessToken");

			QQConnectionSettings settings = QQConnectionSettings.GetConfig();

			string result = settings.OpenIDPath.ToString();

			NameValueCollection queryParams = new NameValueCollection();

			queryParams["access_token"] = this.AccessToken;

			return UriHelper.CombineUrlParams(result, queryParams);
		}
	}
}
