using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Configuration;
using MCS.Library.Configuration;

namespace MCS.Library.Core
{
	/// <summary>
	/// 注册和配置了系统相关的Validator，可以通过一个Key来表示一个Validator
	/// </summary>
	public sealed class ValidatorSettings : ConfigurationSection
	{
		/// <summary>
		/// 得到Validator的配置信息
		/// </summary>
		/// <returns></returns>
		public static ValidatorSettings GetConfig()
		{
			ValidatorSettings result = (ValidatorSettings)ConfigurationBroker.GetSection("validatorSettings");

			if (result == null)
				result = new ValidatorSettings();

			return result;
		}

		/// <summary>
		/// Validator集合
		/// </summary>
		[ConfigurationProperty("validators", IsRequired = false)]
		public TypeConfigurationCollection Validators
		{
			get
			{
				return (TypeConfigurationCollection)this["validators"];
			}
		}
	}
}
