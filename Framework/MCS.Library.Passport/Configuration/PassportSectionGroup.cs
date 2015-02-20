#region
// -------------------------------------------------
// Assembly	：	DeluxeWorks.Library.Passport
// FileName	：	PassportSectionGroup.cs
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

namespace MCS.Library.Passport
{
	/// <summary>
	/// 和Passport的有关的配置组
	/// </summary>
	internal sealed class PassportSectionGroup : ConfigurationSectionGroup
	{
		/// <summary>
		/// Passport的客户端应用配置节
		/// </summary>
		[ConfigurationProperty("passportClientSettings")]
		public PassportClientSettings ClientSettings
		{
			get
			{
				return base.Sections["passportClientSettings"] as PassportClientSettings; 
			}
		}

		/// <summary>
		/// Passport认证服务所需要的配置节
		/// </summary>
		[ConfigurationProperty("passportSignInSettings")]
		public PassportSignInSettings SignInSettings
		{
			get
			{
				return base.Sections["passportSignInSettings"] as PassportSignInSettings;
			}
		}

		/// <summary>
		/// Passport加解密的配置节
		/// </summary>
		[ConfigurationProperty("passportEncryptionSettings")]
		public PassportEncryptionSettings EncryptionSettings
		{
			get
			{
				return base.Sections["passportEncryptionSettings"] as PassportEncryptionSettings;
			}
		}

		/// <summary>
		/// 身份模拟有关的配置节
		/// </summary>
		[ConfigurationProperty("impersonateSettings")]
		public ImpersonateSettings ImpersonateSettings
		{
			get
			{
				return base.Sections["impersonateSettings"] as ImpersonateSettings;
			}
		}

		/// <summary>
		/// 认证目录的设置有关的配置节
		/// </summary>
		[ConfigurationProperty("authenticateDirSettings")]
		public AuthenticateDirSettings AuthenticateDirSettings
		{
			get
			{
				return base.Sections["authenticateDirSettings"] as AuthenticateDirSettings;
			}
		}

		/// <summary>
		/// 域名映射有关的配置节
		/// </summary>
		[ConfigurationProperty("domainMappingSettings")]
		public DomainMappingSettings DomainMappingSettings
		{
			get
			{
				return base.Sections["domainMappingSettings"] as DomainMappingSettings;
			}
		}
	}
}
