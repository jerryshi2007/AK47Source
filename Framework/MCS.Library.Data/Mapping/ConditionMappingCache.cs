using System;
using System.Text;
using System.Collections.Generic;
using MCS.Library.Caching;

namespace MCS.Library.Data.Mapping
{
	internal sealed class ConditionMappingCache : CacheQueue<System.Type, ConditionMappingItemCollection>
	{
		public static readonly ConditionMappingCache Instance = CacheManager.GetInstance<ConditionMappingCache>();

		private ConditionMappingCache()
		{
		}
	}
}
