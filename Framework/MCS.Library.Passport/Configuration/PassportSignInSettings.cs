#region
// -------------------------------------------------
// Assembly	：	DeluxeWorks.Library.Passport
// FileName	：	PassportSignInSettings.cs
// Remark	：	
// -------------------------------------------------
// VERSION  	AUTHOR		DATE			CONTENT
// 1.0
// 1.1          胡自强      2008-12-2       添加注释
// -------------------------------------------------
#endregion
using System;
using System.Text;
using System.Configuration;
using System.Collections.Generic;
using MCS.Library.Core;
using MCS.Library.Configuration;
using MCS.Library.Passport.Properties;

namespace MCS.Library.Passport
{
	/// <summary>
	/// 单点登录服务配置
	/// </summary>
	public sealed class PassportSignInSettings : ConfigurationSection
	{
		private const string C_SIGNIN_COOKIE_KEY = "HPassportSignIn";

		private const string C_SIGNIN_PAGEDATA_COOKIE_KEY = "SignInPageData";

		/// <summary>
		/// 读取单点登录服务配置
		/// </summary>
		/// <returns>认证服务器配置信息</returns>
		/// <remarks>
		/// <code source="..\Framework\TestProjects\DeluxeWorks.Library.Passport.Test\ConfigurationTest.cs" region="SignInConfigTest" lang="cs" title="获取认证服务端配置信息" />
		/// </remarks>
		public static PassportSignInSettings GetConfig()
		{
			PassportSignInSettings result =
				(PassportSignInSettings)ConfigurationBroker.GetSection("passportSignInSettings");

			ConfigurationExceptionHelper.CheckSectionNotNull(result, "passportSignInSettings");

			return result;
		}

		private PassportSignInSettings()
		{
		}

		/// <summary>
		/// 缺省的过期时间
		/// </summary>
		public TimeSpan DefaultTimeout
		{
			get
			{
				return TimeSpan.FromSeconds((double)DefaultTimeoutInt);
			}
		}

		/// <summary>
		/// 缺省的滑动过期时间
		/// </summary>
		public TimeSpan SlidingExpiration
		{
			get
			{
				return TimeSpan.FromSeconds((double)SlidingExpirationInt);
			}
		}
		/// <summary>
		/// 是否Windows集成认证
		/// </summary>
		[ConfigurationProperty("isWindowsIntegrated", DefaultValue = false)]
		public bool IsWindowsIntegrated
		{
			get
			{
				return (bool)this["isWindowsIntegrated"];
			}
		}

		/// <summary>
		/// Windows认证出现异常时，是否抛出，如果不抛出，会显示缺省的认证页面
		/// </summary>
		[ConfigurationProperty("throwWindowsSignInError", DefaultValue = true)]
		public bool ThrowWindowsSignInError
		{
			get
			{
				return (bool)this["throwWindowsSignInError"];
			}
		}

		/// <summary>
		/// 是否使用模拟的时间
		/// </summary>
		[ConfigurationProperty("useSimulateTime", IsRequired = false, DefaultValue = false)]
		public bool UseSimulateTime
		{
			get
			{
				return (bool)this["useSimulateTime"];
			}
		}

		/// <summary>
		/// 登录信息使用的Cookie的Key
		/// </summary>
		[ConfigurationProperty("signInCookieKey", IsRequired = false, DefaultValue = PassportSignInSettings.C_SIGNIN_COOKIE_KEY)]
		public string SignInCookieKey
		{
			get
			{
				return (string)this["signInCookieKey"];
			}
		}

		/// <summary>
		/// 认证页面数据的Cookie的Key
		/// </summary>
		[ConfigurationProperty("signInPageDataCookieKey", IsRequired = false, DefaultValue = PassportSignInSettings.C_SIGNIN_PAGEDATA_COOKIE_KEY)]
		public string SignInPageDataCookieKey
		{
			get
			{
				return (string)this["signInPageDataCookieKey"];
			}
		}

		/// <summary>
		/// 认证信息的保存寿命是否是Session方式的
		/// </summary>
		public bool IsSessionBased
		{
			get
			{
				return DefaultTimeout.TotalSeconds == -2;
			}
		}

		/// <summary>
		/// 负责认证的插件
		/// </summary>
		public IAuthenticator Authenticator
		{
			get
			{
				return (IAuthenticator)TypeFactories["authenticator"].CreateInstance();
			}
		}

		/// <summary>
		/// 负责认证的插件2
		/// </summary>
		public IAuthenticator2 Authenticator2
		{
			get
			{
				IAuthenticator2 result = null;

				object authenticator = this.TypeFactories["authenticator"].CreateInstance();

				result = authenticator as IAuthenticator2;

				return result;
			}
		}

		/// <summary>
		/// SignInInfo和Ticket的持久化器
		/// </summary>
		public IPersistSignInInfo PersistSignInInfo
		{
			get
			{
				return (IPersistSignInInfo)TypeFactories["persistSignInInfo"].CreateInstance();
			}
		}

		/// <summary>
		/// OpenIDBinding的持久化器
		/// </summary>
		public IPersistOpenIDBinding PersistOpenIDBinding
		{
			get
			{
				return (IPersistOpenIDBinding)TypeFactories["persistOpenIDBinding"].CreateInstance();
			}
		}

		/// <summary>
		/// 用户信息的修该器
		/// </summary>
		public IUserInfoUpdater UserInfoUpdater
		{
			get
			{
				return (IUserInfoUpdater)TypeFactories["userInfoUpdater"].CreateInstance();
			}
		}

		/// <summary>
		/// 用户ID的转换器
		/// </summary>
		public IUserIDConverter UserIDConverter
		{
			get
			{
				return (IUserIDConverter)TypeFactories["userIDConverter"].CreateInstance();
			}
		}

		/// <summary>
		/// 一些插件的配置
		/// </summary>
		[ConfigurationProperty("typeFactories", IsRequired = true)]
		private TypeConfigurationCollection TypeFactories
		{
			get
			{
				return (TypeConfigurationCollection)this["typeFactories"];
			}
		}

		/// <summary>
		/// 通知器的配置
		/// </summary>
		[ConfigurationProperty("signInNotifiers")]
		public TypeConfigurationCollection SignInNotifiers
		{
			get
			{
				return (TypeConfigurationCollection)this["signInNotifiers"];
			}
		}

		///// <summary>
		///// Windows集成环境下允许使用的域环境设置，要求设置长名称[value]和短名称[name]
		///// </summary>
		//[ConfigurationProperty("domains")]
		//public NameValueConfigurationCollection Domains
		//{
		//    get
		//    {
		//        return (NameValueConfigurationCollection)this["domains"];
		//    }
		//}

		/// <summary>
		/// 是否存在滑动过期时间
		/// </summary>
		public bool HasSlidingExpiration
		{
			get
			{
				return SlidingExpiration != TimeSpan.Zero;
			}
		}

		/// <summary>
		/// 加密Ticket所使用
		/// </summary>
		public string RsaKeyValue
		{
			get
			{
				return RsaKeyValueElement.Value;
			}
		}

		#region 私有属性
		[ConfigurationProperty("slidingExpiration", DefaultValue = 0)]
		private int SlidingExpirationInt
		{
			get
			{
				return (int)this["slidingExpiration"];
			}
		}

		/// <summary>
		/// -1,表示永远有效；-2表示当前浏览器的Session
		/// </summary>
		[ConfigurationProperty("defaultTimeout", DefaultValue = -2)]
		private int DefaultTimeoutInt
		{
			get
			{
				return (int)this["defaultTimeout"];
			}
		}

		[ConfigurationProperty("rsaKeyValue", IsRequired = true)]
		private SignInRsaKeyValueConfigurationElement RsaKeyValueElement
		{
			get
			{
				return (SignInRsaKeyValueConfigurationElement)this["rsaKeyValue"];
			}
		}
		#endregion 私有属性
	}

	/// <summary>
	/// Rsa加密配置信息
	/// </summary>
	public class SignInRsaKeyValueConfigurationElement : ConfigurationElement
	{
		private static string sValue = string.Empty;

		/// <summary>
		/// Rsa配置的string值
		/// </summary>
		public string Value
		{
			get
			{
				return SignInRsaKeyValueConfigurationElement.sValue;
			}
		}
		/// <summary>
		/// 读入Rsa配置信息
		/// </summary>
		/// <param name="reader">XmlReader</param>
		/// <param name="serializeCollectionKey"></param>
		protected override void DeserializeElement(System.Xml.XmlReader reader, bool serializeCollectionKey)
		{
			lock (typeof(SignInRsaKeyValueConfigurationElement))
			{
				if (SignInRsaKeyValueConfigurationElement.sValue == string.Empty)
					SignInRsaKeyValueConfigurationElement.sValue = reader.ReadOuterXml();
				else
					reader.ReadOuterXml();
			}
		}
	}
}
