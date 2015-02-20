using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Configuration;
using MCS.Library.Configuration;

namespace MCS.Library.Services
{
	public class MossServerInfoConfigurationSettings : ConfigurationSection
	{
		private MossServerInfoConfigurationSettings() { }

		public static MossServerInfoConfigurationSettings GetConfig()
		{
			MossServerInfoConfigurationSettings result = (MossServerInfoConfigurationSettings)ConfigurationBroker.GetSection("mossServerInfoConfigSettings");

			ConfigurationExceptionHelper.CheckSectionNotNull(result, "mossServerInfoConfigSettings");

			return result;
		}

		[ConfigurationProperty("servers")]
		public MossServerInfoConfigrationElementCollection Servers
		{
			get
			{
				return (MossServerInfoConfigrationElementCollection)this["servers"];
			}
		}

	}
}