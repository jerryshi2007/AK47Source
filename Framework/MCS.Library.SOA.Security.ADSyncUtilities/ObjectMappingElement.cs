using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MCS.Library.Configuration;
using System.Configuration;

namespace MCS.Library.SOA.Security.ADSyncUtilities
{
	public sealed class ObjectMappingElement : NamedConfigurationElement
	{
		[ConfigurationProperty("scObjectName", IsRequired = true)]
		public string SCObjectName
		{
			get
			{
				return (string)base["scObjectName"];
			}

			set
			{
				base["scObjectName"] = value;
			}
		}

		[SettingsDescription("AD对象的名称，请使用RDN的表示法，例如OU=机构A")]
		[ConfigurationProperty("adObjectName", IsRequired = true)]
		public string ADObjectName
		{
			get
			{
				return (string)base["adObjectName"];
			}

			set
			{
				base["adObjectName"] = value;
			}
		}
	}
}
