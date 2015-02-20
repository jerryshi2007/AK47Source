using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using MCS.Library.Caching;

namespace MCS.Library.Core
{
	internal abstract class FieldGetMethodDelegateCacheBase : PortableCacheQueue<FieldMethodDelegateCacheKey, Delegate>
	{
		public Delegate GetOrAddNewValue(FieldInfo fieldInfo, Type valueType, PortableCacheItemNotExistsAction action)
		{
			FieldMethodDelegateCacheKey key = new FieldMethodDelegateCacheKey(fieldInfo, valueType);

			return GetOrAddNewValue(key, action);
		}
	}

	internal class FieldGetMethodDelegateCache : FieldGetMethodDelegateCacheBase
	{
		/// <summary>
		/// 获取实例
		/// </summary>
		public static readonly FieldGetMethodDelegateCache Instance = CacheManager.GetInstance<FieldGetMethodDelegateCache>();

		private FieldGetMethodDelegateCache()
		{
		}
	}

	internal class FieldSetMethodDelegateCache : FieldGetMethodDelegateCacheBase
	{
		/// <summary>
		/// 获取实例
		/// </summary>
		public static readonly FieldSetMethodDelegateCache Instance = CacheManager.GetInstance<FieldSetMethodDelegateCache>();

		private FieldSetMethodDelegateCache()
		{
		}
	}
}
