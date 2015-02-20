#region
// -------------------------------------------------
// Assembly	��	DeluxeWorks.Library
// FileName	��	ConfigurationCache.cs
// Remark	��	���ڴ�� Configuration �� Cache
// -------------------------------------------------
// VERSION  	AUTHOR		DATE			CONTENT
// 1.0		    ����	    20070430		����
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
	/// ���ڴ�� Configuration��Cache�����Ǻϲ���Configuration
	/// </summary>
	sealed class ConfigurationCache : PortableCacheQueue<string, System.Configuration.Configuration>
	{
		/// <summary>
		/// ��ȡʵ��
		/// </summary>
		public static readonly ConfigurationCache Instance = CacheManager.GetInstance<ConfigurationCache>();

		private ConfigurationCache()
		{

		}
	}

	/// <summary>
	/// ���ڴ��ConfigurationSection��Cache
	/// </summary>
	sealed class ConfigurationSectionCache : PortableCacheQueue<string, ConfigurationSection>
	{
		/// <summary>
		/// ��ȡʵ��
		/// </summary>
		public static readonly ConfigurationSectionCache Instance = CacheManager.GetInstance<ConfigurationSectionCache>();

		private ConfigurationSectionCache()
		{
		}
	}

	/// <summary>
	/// ���ڴ�� Configuration �� Cache���������ļ�ΪKey�������ڻ��浥���������ļ�
	/// </summary>
	sealed class ConfigurationFileCache : PortableCacheQueue<string, System.Configuration.Configuration>
	{
		/// <summary>
		/// ��ȡʵ��
		/// </summary>
		public static readonly ConfigurationFileCache Instance = CacheManager.GetInstance<ConfigurationFileCache>();

		private ConfigurationFileCache()
		{
		}
	}
}
