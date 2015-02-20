using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MCS.Library.Caching
{
	/// <summary>
	/// 字符串表的上下文缓存，以字符串为key，字符串为值的队列。仅存放于上下文中
	/// </summary>
	public sealed class StringTableContextCache : ContextCacheQueueBase<string, string>
	{
		private StringTableContextCache()
		{
		}

		/// <summary>
		/// StringTableContextCache的实例，此处必须是属性，动态计算
		/// </summary>
		public static StringTableContextCache Instance
		{
			get
			{
				return ContextCacheManager.GetInstance<StringTableContextCache>();
			}
		}
	}
}
