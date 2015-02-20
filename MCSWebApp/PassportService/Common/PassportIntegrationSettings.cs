using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Configuration;
using MCS.Library.Configuration;
using System.Security.Cryptography;
using MCS.Library.Core;
using MCS.Library.Security;

namespace MCS.Web.Passport
{
	/// <summary>
	/// 集成Windows认证相关的配置信息
	/// </summary>
	public class PassportIntegrationSettings : ConfigurationSection
	{
		public static PassportIntegrationSettings GetConfig()
		{
			PassportIntegrationSettings settings = (PassportIntegrationSettings)ConfigurationBroker.GetSection("passportIntegrationSettings");

			ConfigurationExceptionHelper.CheckSectionNotNull(settings, "passportIntegrationSettings");

			return settings;
		}

		[ConfigurationProperty("encryptionKey", IsRequired = true)]
		public string EncryptionKey
		{
			get
			{
				return (string)this["encryptionKey"];
			}
		}

		public DES GetDesKey()
		{
			ExceptionHelper.CheckStringIsNullOrEmpty(EncryptionKey, "EncryptionKey");

			return EncryptionKey.ToDES();
		}
	}
}
