using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Reflection;
using MCS.Library.Caching;

namespace MCS.Library.Core
{
	internal sealed class InvokeMethodDelegateCache : PortableCacheQueue<MethodInfo, Delegate>
	{
		/// <summary>
		/// 获取实例
		/// </summary>
		public static readonly InvokeMethodDelegateCache Instance = CacheManager.GetInstance<InvokeMethodDelegateCache>();

		private InvokeMethodDelegateCache()
		{
		}
	}
}
