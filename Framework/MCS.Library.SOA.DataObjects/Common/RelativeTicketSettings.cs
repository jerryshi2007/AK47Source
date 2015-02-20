using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Configuration;
using MCS.Library.Configuration;
using MCS.Library.Security;

namespace MCS.Library.SOA.DataObjects
{
	public sealed class RelativeTicketSettings : ConfigurationSection
	{
		public static RelativeTicketSettings GetConfig()
		{
			RelativeTicketSettings settings = (RelativeTicketSettings)ConfigurationBroker.GetSection("relativeTicketSettings");

			if (settings == null)
				settings = new RelativeTicketSettings();

			return settings;
		}

		private RelativeTicketSettings()
		{
		}

		[ConfigurationProperty("urlTransferTimeout", DefaultValue = "00:01:00", IsRequired = false)]
		public TimeSpan UrlTransferTimeout
		{
			get
			{
				return (TimeSpan)this["urlTransferTimeout"];
			}
		}

		public ISymmetricEncryption Encryptor
		{
			get
			{
				ISymmetricEncryption result = null;

				if (TypeFactories.Count > 0)
					result = (ISymmetricEncryption)TypeFactories[0].CreateInstance();
				else
					result = new TicketDESEncryptor();

				return result;
			}
		}

		[ConfigurationProperty("typeFactories", IsRequired = false)]
		private TypeConfigurationCollection TypeFactories
		{
			get
			{
				return (TypeConfigurationCollection)this["typeFactories"];
			}
		}
	}
}
