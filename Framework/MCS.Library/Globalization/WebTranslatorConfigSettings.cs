using System;
using System.Text;
using System.Configuration;
using System.Collections.Generic;
using MCS.Library.Configuration;

namespace MCS.Web.Library
{
	/// <summary>
	/// 
	/// </summary>
	public sealed class WebTranslatorConfigSettings : DeluxeConfigurationSection
	{
		/// <summary>
		/// 
		/// </summary>
		/// <returns></returns>
		public static WebTranslatorConfigSettings GetConfig()
		{
			WebTranslatorConfigSettings result = (WebTranslatorConfigSettings)ConfigurationBroker.GetSection("webTranslatorConfigSettings");

			ConfigurationExceptionHelper.CheckSectionNotNull(result, "webTranslatorConfigSettings");

			return result;
		}

		private WebTranslatorConfigSettings()
		{
		}

		/// <summary>
		/// 
		/// </summary>
		[ConfigurationProperty("cultureFileRoot", IsRequired = true)]
		public string CultureFileRoot
		{
			get
			{
				return (string)this["cultureFileRoot"];
			}
		}
	}
}
