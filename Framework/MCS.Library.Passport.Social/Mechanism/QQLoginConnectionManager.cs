using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MCS.Library.Core;
using MCS.Library.Passport.Social.DataObjects;
using MCS.Library.Passport.Social.Executors;

namespace MCS.Library.Passport.Social.Mechanism
{
	public static class QQLoginConnectionManager
	{
		public static QQAccessTokenResponseParams GetAccessToken(QQAccessTokenRequestParams requestParams)
		{
			QQGetAccessTokenExecutor executor = new QQGetAccessTokenExecutor(requestParams);

			executor.Execute();

			return executor.ResponseParams;
		}

		public static QQGetOpenIDResponseParams GetOpenID(QQGetOpenIDRequestParams requestParams)
		{
			QQGetOpenIDExecutor executor = new QQGetOpenIDExecutor(requestParams);

			executor.Execute();

			return executor.ResponseParams;
		}

		public static QQGetUserInfoResponseParams GetUserInfo(QQGetUserInfoRequestParams requestParams)
		{
			QQGetUserInfoExecutor executor = new QQGetUserInfoExecutor(requestParams);

			executor.Execute();

			return executor.ResponseParams;
		}

		/// <summary>
		/// 从类似于callback( {"client_id":"YOUR_APPID","openid":"YOUR_OPENID"} );的结果中，分析出
		/// {"client_id":"YOUR_APPID","openid":"YOUR_OPENID"}
		/// </summary>
		/// <param name="responseText"></param>
		/// <returns></returns>
		public static string GetResponseJsonString(string responseText)
		{
			string result = string.Empty;

			if (responseText.IsNotEmpty())
			{
				int left = responseText.IndexOf('{');
				int right = responseText.LastIndexOf('}');

				if (left >= 0 && right >= 0)
					result = responseText.Substring(left, right - left + 1);
			}

			return result;
		}
	}
}
