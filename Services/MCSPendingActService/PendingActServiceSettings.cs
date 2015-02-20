using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Configuration;
using MCS.Library.Configuration;

namespace MCSPendingActService
{
	public class PendingActServiceSettings : ConfigurationSection
	{
		public static PendingActServiceSettings GetConfig()
		{
			PendingActServiceSettings result = (PendingActServiceSettings)ConfigurationBroker.GetSection("pendingActServiceConfig");

			if (result == null)
				result = new PendingActServiceSettings();

			return result;
		}

		private PendingActServiceSettings()
		{
		}

		[ConfigurationProperty("pendingActServices")]
		public PendingActServiceConfigurationElementCollection PendingActServices
		{
			get { return (PendingActServiceConfigurationElementCollection)this["pendingActServices"]; }
		}
	}

	public sealed class PendingActServiceConfigurationElementCollection
		: NamedConfigurationElementCollection<PendingActServiceConfigurationElement>
	{

	}

	public class PendingActServiceConfigurationElement : NamedConfigurationElement
	{
		[ConfigurationProperty("applicationName", IsRequired = false, DefaultValue = "all")]
		public string ApplicationName
		{
			get
			{
				return (string)this["applicationName"];
			}
		}

		[ConfigurationProperty("programName", IsRequired = false, DefaultValue = "all")]
		public string ProgramName
		{
			get
			{
				return (string)this["programName"];
			}
		}

		[ConfigurationProperty("operator", IsRequired = false)]
		public string Operator
		{
			get
			{
				return (string)this["operator"];
			}
		}
	}
}
