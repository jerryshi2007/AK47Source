using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using MCS.Library.Caching;

namespace MCS.Library.Core
{
	internal abstract class PropertyGetMethodDelegateCacheBase : PortableCacheQueue<PropertyMethodDelegateCacheKey, Delegate>
	{
		public Delegate GetOrAddNewValue(PropertyInfo propertyInfo, Type valueType, PortableCacheItemNotExistsAction action)
		{
			PropertyMethodDelegateCacheKey key = new PropertyMethodDelegateCacheKey(propertyInfo, valueType);

			return GetOrAddNewValue(key, action);
		}
	}

	internal sealed class PropertyGetMethodDelegateCache : PropertyGetMethodDelegateCacheBase
	{
		/// <summary>
		/// 获取实例
		/// </summary>
		public static readonly PropertyGetMethodDelegateCache Instance = CacheManager.GetInstance<PropertyGetMethodDelegateCache>();

		private PropertyGetMethodDelegateCache()
		{
		}
	}

	internal sealed class PropertySetMethodDelegateCache : PropertyGetMethodDelegateCacheBase
	{
		/// <summary>
		/// 获取实例
		/// </summary>
		public static readonly PropertySetMethodDelegateCache Instance = CacheManager.GetInstance<PropertySetMethodDelegateCache>();

		private PropertySetMethodDelegateCache()
		{
		}
	}
}
