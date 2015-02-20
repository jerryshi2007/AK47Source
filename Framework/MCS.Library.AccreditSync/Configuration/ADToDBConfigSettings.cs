using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Configuration;
using MCS.Library.Configuration;

namespace MCS.Library.Accredit
{
	public sealed class ADToDBConfigSettings : ConfigurationSection
	{
		public static ADToDBConfigSettings GetConfig()
		{
			ADToDBConfigSettings settings = (ADToDBConfigSettings)ConfigurationBroker.GetSection("adToDBConfigSettings");

			if (settings == null)
				settings = new ADToDBConfigSettings();

			return settings;
		}

		private ADToDBConfigSettings()
		{
		}

		[ConfigurationProperty("rootOUName", DefaultValue = "机构人员", IsRequired = false)]
		public string RootOUName
		{
			get
			{
				return (string)base["rootOUName"];
			}
		}

		[ConfigurationProperty("rootOUPath", DefaultValue = "", IsRequired = false)]
		public string RootOUPath
		{
			get
			{
				return (string)base["rootOUPath"];
			}
		}

        [ConfigurationProperty("userInfoExtendConnectionName", DefaultValue = "OldPlatform", IsRequired = false)]
		public string UserInfoExtendConnectionName
		{
			get
			{
				return (string)base["userInfoExtendConnectionName"];
			}
		}
	}
}
