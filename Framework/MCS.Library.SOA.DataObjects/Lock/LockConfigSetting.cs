using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Configuration;
using MCS.Library.Configuration;

namespace MCS.Library.SOA.DataObjects
{
	public sealed class LockConfigSetting : ConfigurationSection
	{
		private LockConfigSetting()
		{ }

		private const string LockSettingsName = "lockSettings";

		public static LockConfigSetting GetConfig()
		{
			LockConfigSetting result =
				   (LockConfigSetting)ConfigurationBroker.GetSection(LockSettingsName);

			ConfigurationExceptionHelper.CheckSectionNotNull(result, LockSettingsName);

			return result;
		}

		/// <summary>
		/// 是否启用锁机制
		/// </summary>
		[ConfigurationProperty("enabled", IsRequired = false, DefaultValue = "false")]
		public bool Enabled
		{
			get
			{
				return (bool)this["enabled"];
			}
		}

		/// <summary>
		/// 默认有效时间，单位是秒。如果上一次锁的检查时间超过此有效时间，则表明没有心跳
		/// </summary>
		[ConfigurationProperty("defaultEffectiveTime", IsRequired = true, DefaultValue = "00:04:00")]
		public TimeSpan DefaultEffectiveTime
		{
			get
			{
				return (TimeSpan)this["defaultEffectiveTime"];
			}
		}

		/// <summary>
		/// 默认检查锁的时间间隔，单位是秒。锁的拥有者需要以此时间为周期，来更新锁的检查时间，表明还有心跳
		/// </summary>
		[ConfigurationProperty("defaultCheckIntervalTime", IsRequired = true, DefaultValue = "00:01:00")]
		public TimeSpan DefaultCheckIntervalTime
		{
			get
			{
				return (TimeSpan)this["defaultCheckIntervalTime"];
			}
		}
	}
}
