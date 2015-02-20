using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using MCS.Library.Configuration;

namespace MCS.Library.Passport.Social.Configuration
{
	public class QQConnectionSettings : DeluxeConfigurationSection
	{
		public static QQConnectionSettings GetConfig()
		{
			QQConnectionSettings settings = (QQConnectionSettings)ConfigurationBroker.GetSection("qqConnectionSettings");

			ConfigurationExceptionHelper.CheckSectionNotNull(settings, "qqConnectionSettings");

			return settings;
		}

		[ConfigurationProperty("appID")]
		public string AppID
		{
			get
			{
				return (string)this["appID"];
			}
		}

		[ConfigurationProperty("appKey")]
		public string AppKey
		{
			get
			{
				return (string)this["appKey"];
			}
		}

		/// <summary>
		/// 获取授权码的路径
		/// </summary>
		public Uri AuthorizationPath
		{
			get
			{
				return Paths["authorizationPath"].Uri;
			}
		}

		/// <summary>
		/// 获取访问标识的路径
		/// </summary>
		public Uri AccessTokenPath
		{
			get
			{
				return Paths["accessTokenPath"].Uri;
			}
		}

		/// <summary>
		/// 获取OpenID的路径
		/// </summary>
		public Uri OpenIDPath
		{
			get
			{
				return Paths["openIDPath"].Uri;
			}
		}

		/// <summary>
		/// 获取UserInfo的路径
		/// </summary>
		public Uri GetUserInfoPath
		{
			get
			{
				return Paths["getUserInfoPath"].Uri;
			}
		}

		/// <summary>
		/// 登录回调的地址
		/// </summary>
		public Uri LoginCallback
		{
			get
			{
				return Paths["loginCallback"].Uri;
			}
		}

		/// <summary>
		/// 路径配置
		/// </summary>
		[ConfigurationProperty("paths", IsRequired = true)]
		private UriConfigurationCollection Paths
		{
			get
			{
				return (UriConfigurationCollection)this["paths"];
			}
		}
	}
}
