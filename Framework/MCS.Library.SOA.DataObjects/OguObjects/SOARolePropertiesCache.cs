using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MCS.Library.Caching;

namespace MCS.Library.SOA.DataObjects
{
	internal sealed class SOARolePropertiesCache : CacheQueue<string, SOARolePropertyRowCollection>
	{
		public static readonly SOARolePropertiesCache Instance = CacheManager.GetInstance<SOARolePropertiesCache>();
	}
}
