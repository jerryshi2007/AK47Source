using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MCS.Library.Core;
using System.Collections.Specialized;

namespace MCS.Library.Passport.Social.DataObjects
{
	/// <summary>
	/// 获取访问码的返回结果
	/// </summary>
	[Serializable]
	public class QQAccessTokenResponseParams
	{
		public QQAccessTokenResponseParams()
		{
		}

		public QQAccessTokenResponseParams(string responseText)
		{
			ParseResponseText(responseText);
		}

		public string AccessToken
		{
			get;
			set;
		}

		public TimeSpan Expires
		{
			get;
			set;
		}

		public string RefreshToken
		{
			get;
			set;
		}

		public override string ToString()
		{
			return string.Format("AccessToken: {0}; Expires: {1}, RefreshToken: {2}",
				this.AccessToken,
				this.Expires.TotalSeconds,
				this.RefreshToken);
		}

		private void ParseResponseText(string responseText)
		{
			responseText.CheckStringIsNullOrEmpty("responseText");

			NameValueCollection responseParams = UriHelper.GetUriParamsCollection(responseText);

			this.AccessToken = responseParams["access_token"];
			this.RefreshToken = responseParams["refresh_token"];

			string expiresText = responseParams["expires_in"];

			if (expiresText.IsNotEmpty())
			{
				int senonds = 0;

				if (int.TryParse(expiresText, out senonds))
					this.Expires = TimeSpan.FromSeconds(senonds);
			}
		}
	}
}
