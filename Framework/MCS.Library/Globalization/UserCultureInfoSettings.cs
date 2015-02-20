using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MCS.Library.Configuration;
using System.Configuration;

namespace MCS.Library.Globalization
{
	/// <summary>
	/// 获取用户CultureInfo的配置节
	/// </summary>
	public sealed class UserCultureInfoSettings : DeluxeConfigurationSection
	{
		/// <summary>
		/// 得到配置节
		/// </summary>
		/// <returns></returns>
		public static UserCultureInfoSettings GetConfig()
		{
			UserCultureInfoSettings result = (UserCultureInfoSettings)ConfigurationBroker.GetSection("userCultureInfoSettings");

			if (result == null)
				result = new UserCultureInfoSettings();

			return result;
		}

		/// <summary>
		/// 用户的Culture信息的获取器
		/// </summary>
		public IUserCultureInfoAccessor UserCultureInfoAccessor
		{
			get
			{
				return GetUserCultureInfoAccessor();
			}
		}

		[ConfigurationProperty("typeFactories", IsRequired = false)]
		private TypeConfigurationCollection TypeFactories
		{
			get
			{
				return (TypeConfigurationCollection)this["typeFactories"];
			}
		}

		private IUserCultureInfoAccessor GetUserCultureInfoAccessor()
		{
			IUserCultureInfoAccessor result = null;

			if (this.TypeFactories.ContainsKey("cultureInfoAccessor"))
			{
				try
				{
					result = (IUserCultureInfoAccessor)this.TypeFactories["cultureInfoAccessor"].CreateInstance();
				}
				catch (TypeLoadException)
				{
				}
			}

			return result;
		}
	}
}
