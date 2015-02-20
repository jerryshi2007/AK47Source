using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Text;
using System.Web;
using MCS.Library.Configuration;
using MCS.Library.Core;

namespace MCS.Library.Passport
{
	/// <summary>
	/// 配置OpenID认证的目录
	/// </summary>
	public sealed class OpenIDAuthenticateDirSettings : AuthenticateDirSettingsBase
	{
		/// <summary>
		/// 获取配置认证目录信息
		/// </summary>
		/// <returns></returns>
		public static OpenIDAuthenticateDirSettings GetConfig()
		{
			OpenIDAuthenticateDirSettings settings = (OpenIDAuthenticateDirSettings)ConfigurationBroker.GetSection("openIDAuthenticateDirSettings");

			if (settings == null)
				settings = new OpenIDAuthenticateDirSettings();

			return settings;
		}

		private OpenIDAuthenticateDirSettings()
		{
		}
	}
}
