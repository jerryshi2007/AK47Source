using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MCS.Library.Caching;

namespace MCS.Library.SOA.DataObjects
{
	internal class SOARolePropertiesDefinitionCache : CacheQueue<string, SOARolePropertyDefinitionCollection>
	{
		public static readonly SOARolePropertiesDefinitionCache Instance = CacheManager.GetInstance<SOARolePropertiesDefinitionCache>();
	}
}
