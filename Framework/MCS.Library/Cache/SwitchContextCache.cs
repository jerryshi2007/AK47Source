using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MCS.Library.Caching
{
	/// <summary>
	/// 开关表的上下文缓存，以字符串为key，bool为值的队列。仅存放于上下文中
	/// </summary>
	public sealed class SwitchContextCache : ContextCacheQueueBase<string, bool>
	{
		private SwitchContextCache()
		{
		}

		/// <summary>
		/// SwitchContextCache的实例，此处必须是属性，动态计算
		/// </summary>
		public static SwitchContextCache Instance
		{
			get
			{
				return ContextCacheManager.GetInstance<SwitchContextCache>();
			}
		}
	}
}
