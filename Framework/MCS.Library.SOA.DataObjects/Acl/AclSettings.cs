using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Configuration;
using MCS.Library.Core;
using MCS.Library.Configuration;

namespace MCS.Library.SOA.DataObjects
{
	public sealed class AclSettings : ConfigurationSection
	{
		private AclSettings()
		{ }

		public static AclSettings GetConfig()
		{
			AclSettings result = (AclSettings)ConfigurationBroker.GetSection("aclSettings");

			if (result == null)
			{
				result = new AclSettings();
			}

			return result;
		}

		public IEnumerable<IAclUpdater> Operations
		{
			get
			{
				if (OperationElements.Count > 0)
				{
					foreach (TypeConfigurationElement elem in OperationElements)
						yield return (IAclUpdater)elem.CreateInstance();
				}
				else
				{
					yield return DefaultAclUpdater.Instance;
				}
			}
		}

		[ConfigurationProperty("operations")]
		private TypeConfigurationCollection OperationElements
		{
			get
			{
				return (TypeConfigurationCollection)this["operations"];
			}
		}
	}
}
