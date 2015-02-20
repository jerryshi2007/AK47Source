using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Configuration;
using MCS.Library.Configuration;

namespace MCS.Library.Accredit
{
	/// <summary>
	/// AD同步组到安全中心的组
	/// </summary>
	public sealed class ADSyncGroupSettings : ConfigurationSection
	{
		public static ADSyncGroupSettings GetConfig()
		{
			ADSyncGroupSettings settings = (ADSyncGroupSettings)ConfigurationBroker.GetSection("adSyncGroupSettings");

			if (settings == null)
				settings = new ADSyncGroupSettings();

			return settings;
		}

		private ADSyncGroupSettings()
		{
		}

        [ConfigurationProperty("groupConfigurations")]
        public GroupConfigurationElementCollection GroupConfigurations
        {
            get
            {
                return (GroupConfigurationElementCollection)this["groupConfigurations"];
            }
        }
	}

	public class GroupConfigurationElementCollection : NamedConfigurationElementCollection<GroupConfigurationElement>
	{
	}

	public class GroupConfigurationElement : NamedConfigurationElement
	{
		/// <summary>
		/// 源DN。AD中的DN，例如“CN=全体党员,OU=远洋地产,DC=SinoOceanLand,DC=Com”。也可以缩写为“CN=全体党员,OU=远洋地产”
		/// </summary>
		[ConfigurationProperty("sourceDN")]
		public string SourceDN
		{
			get
			{
				return (string)this["sourceDN"];
			}
		}

		/// <summary>
		/// 目标路径。安全中心的全路径，例如“机构人员\远洋地产\全体党员”
		/// </summary>
		[ConfigurationProperty("destinationPath")]
		public string DestinationPath
		{
			get
			{
				return (string)this["destinationPath"];
			}
		}
	}
}
