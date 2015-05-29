using System;
using System.Collections.Generic;
using System.Text;
using MCS.Library.Caching;

namespace MCS.Library.Services
{
	/// <summary>
	/// 基于字典的Cache
	/// </summary>
	public sealed class ServiceDictionaryCache : CacheQueue<ServiceDictionaryItemKey, string>
	{
		/// <summary>
		/// Instance
		/// </summary>
		public static readonly ServiceDictionaryCache Instance = CacheManager.GetInstance<ServiceDictionaryCache>();

		private ServiceDictionaryCache()
		{
		}
	}

	/// <summary>
	/// 
	/// </summary>
    public struct ServiceDictionaryItemKey
	{
		/// <summary>
		/// 
		/// </summary>
		public string SourceText { get; set; }

		/// <summary>
		/// 
		/// </summary>
		public string SourceCultureName { get; set; }

		/// <summary>
		/// 
		/// </summary>
		public string TargetCultureName { get; set; }

		public string Category { get; set; }
	}
}
