using System;
using System.Collections.Generic;
using System.Text;
using MCS.Library.Caching;

namespace MCS.Web.Responsive.Library
{
	/// <summary>
	/// 基于字典的Cache
	/// </summary>
	internal sealed class DictionaryCache : CacheQueue<DictionaryItemKey, string>
	{
		/// <summary>
		/// Instance
		/// </summary>
		public static readonly DictionaryCache Instance = CacheManager.GetInstance<DictionaryCache>();

		private DictionaryCache()
		{
		}
	}

	/// <summary>
	/// 
	/// </summary>
	internal struct DictionaryItemKey
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
