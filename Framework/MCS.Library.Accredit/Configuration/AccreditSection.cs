using System;
using System.Collections.Generic;
using System.Text;
using System.Configuration;

using MCS.Library.Configuration;
using MCS.Library.Core;

namespace MCS.Library.Accredit.Configuration
{
	/// <summary>
	/// 授权平台的统一配置信息节定义
	/// </summary>
	public sealed class AccreditSection : ConfigurationSection
	{
		/// <summary>
		/// 读取配置信息内容
		/// </summary>
		/// <returns></returns>
		public static AccreditSection GetConfig()
		{
			AccreditSection result = (AccreditSection)ConfigurationBroker.GetSection("accreditSection");

			ConfigurationExceptionHelper.CheckSectionNotNull(result, "accreditSection");

			return result;
		}

		/// <summary>
		/// 配置信息节中内容集
		/// </summary>
		[ConfigurationProperty("accreditSettings")]
		public AccreditCollection AccreditSettings
		{
			get
			{
				return (AccreditCollection)this["accreditSettings"];
			}
		}
	}
}
