using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Configuration;
using MCS.Library.Configuration;

namespace ActivateWfProcessService
{
	class ActivateWfProcessServiceSettings : ConfigurationSection
	{
		public static ActivateWfProcessServiceSettings GetConfig()
		{
			ActivateWfProcessServiceSettings result = (ActivateWfProcessServiceSettings)ConfigurationBroker.GetSection("activateWfProcessConfig");

			if (result == null)
				result = new ActivateWfProcessServiceSettings();

			return result;
		}

		private ActivateWfProcessServiceSettings()
		{
		}

		[ConfigurationProperty("activateWfProcesses")]
		public ActivateWfProcessServiceCfgElementCollection ActivateWfProcesses
		{
			get { return (ActivateWfProcessServiceCfgElementCollection)this["activateWfProcesses"]; }
		}
	}

	sealed class ActivateWfProcessServiceCfgElementCollection
		: NamedConfigurationElementCollection<ActivateWfProcessServiceCfgElement>
	{

	}

	class ActivateWfProcessServiceCfgElement : NamedConfigurationElement
	{
		[ConfigurationProperty("processDescriptorKey")]
		public string ProcessDescriptorKey
		{
			get { return (string)this["processDescriptorKey"]; }
		}

		[ConfigurationProperty("condition")]
		public bool Condition
		{
			get { return (bool)this["condition"]; }
		}
	}
}
