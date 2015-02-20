using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MCS.Library.Caching;

namespace MCS.Library.SOA.DataObjects
{
	/// <summary>
	/// 用户个人设置的缓存
	/// </summary>
	internal sealed class UserSettingsCache : CacheQueue<string, UserSettings>
	{
		public static readonly UserSettingsCache Instance = CacheManager.GetInstance<UserSettingsCache>();

		private UserSettingsCache()
		{
		}
	}

	/// <summary>
	/// 依赖于当前的架构配置，配置文件修改后缓存即失效
	/// </summary>
	internal class UserSettingsCacheItemDependency : DependencyBase
	{
		private int originalConfigHashCode = 0;

		public UserSettingsCacheItemDependency()
		{
			this.originalConfigHashCode = UserSettingsConfig.GetConfig().GetHashCode();
		}

		public override bool HasChanged
		{
			get
			{
				return this.originalConfigHashCode != UserSettingsConfig.GetConfig().GetHashCode();
			}
		}
	}
}
