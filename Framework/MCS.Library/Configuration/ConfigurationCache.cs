#region
// -------------------------------------------------
// Assembly	：	DeluxeWorks.Library
// FileName	：	ConfigurationCache.cs
// Remark	：	用于存放 Configuration 的 Cache
// -------------------------------------------------
// VERSION  	AUTHOR		DATE			CONTENT
// 1.0		    王翔	    20070430		创建
// -------------------------------------------------
#endregion
using System;
using System.Text;
using System.Configuration;
using System.Collections.Generic;

using MCS.Library.Caching;

namespace MCS.Library.Configuration
{
	/// <summary>
	/// 用于存放 Configuration的Cache，这是合并过Configuration
	/// </summary>
	sealed class ConfigurationCache : PortableCacheQueue<string, System.Configuration.Configuration>
	{
		/// <summary>
		/// 获取实例
		/// </summary>
		public static readonly ConfigurationCache Instance = CacheManager.GetInstance<ConfigurationCache>();

		private ConfigurationCache()
		{

		}
	}

	/// <summary>
	/// 用于存放ConfigurationSection的Cache
	/// </summary>
	sealed class ConfigurationSectionCache : PortableCacheQueue<string, ConfigurationSection>
	{
		/// <summary>
		/// 获取实例
		/// </summary>
		public static readonly ConfigurationSectionCache Instance = CacheManager.GetInstance<ConfigurationSectionCache>();

		private ConfigurationSectionCache()
		{
		}
	}

	/// <summary>
	/// 用于存放 Configuration 的 Cache，但是以文件为Key。适用于缓存单独的配置文件
	/// </summary>
	sealed class ConfigurationFileCache : PortableCacheQueue<string, System.Configuration.Configuration>
	{
		/// <summary>
		/// 获取实例
		/// </summary>
		public static readonly ConfigurationFileCache Instance = CacheManager.GetInstance<ConfigurationFileCache>();

		private ConfigurationFileCache()
		{
		}
	}
}
