using System;
using System.Text;
using System.Collections.Generic;
using MCS.Library.Caching;

namespace MCS.Library.Core
{
	internal sealed class XmlMappingsCache : CacheQueue<System.Type, XmlObjectMappingItemCollection>
	{
		public static readonly XmlMappingsCache Instance = CacheManager.GetInstance<XmlMappingsCache>();

		private XmlMappingsCache()
		{
		}
	}
}
