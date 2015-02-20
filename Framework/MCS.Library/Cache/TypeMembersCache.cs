using System;
using System.Text;
using System.Reflection;
using System.Collections.Generic;
using MCS.Library.Core;

namespace MCS.Library.Caching
{
	/// <summary>
	/// 为类型的成员提供Cache功能
	/// </summary>
	public abstract class TypeMembersCacheQueue<TMember> : CacheQueue<Type, Dictionary<string, TMember>> where TMember : MemberInfo
	{
	}

	/// <summary>
	/// 为类型的Properties提供Cache功能的基类
	/// </summary>
	public abstract class TypePropertiesCacheQueueBase : TypeMembersCacheQueue<PropertyInfo>
	{
		/// <summary>
		/// 得到类型的属性信息。必须重载
		/// </summary>
		/// <param name="type"></param>
		/// <returns></returns>
		protected abstract PropertyInfo[] GetProperties(System.Type type);

		/// <summary>
		/// 得到类型的属性定义
		/// </summary>
		/// <param name="type"></param>
		/// <param name="propertyName"></param>
		/// <returns></returns>
		public PropertyInfo GetPropertyInfo(System.Type type, string propertyName)
		{
			PropertyInfo result = GetPropertyInfoDirectly(type, propertyName);

			ExceptionHelper.FalseThrow(result != null,
				"不能在类型{0}中找到属性{1}", type.Name, propertyName);

			return result;
		}

		/// <summary>
		/// 得到属性字典。属性名称为Key
		/// </summary>
		/// <param name="type"></param>
		/// <returns></returns>
		public Dictionary<string, PropertyInfo> GetPropertyDictionary(System.Type type)
		{
			type.NullCheck("type");

			return this.GetOrAddNewValue(type, (cache, key) =>
			{
				Dictionary<string, PropertyInfo> newItem = new Dictionary<string, PropertyInfo>();

				PropertyInfo[] pis = GetProperties(type);

				foreach (PropertyInfo pi in pis)
				{
					if (newItem.ContainsKey(pi.Name) == false)
						newItem.Add(pi.Name, pi);
				}

				cache.Add(key, newItem);

				return newItem;
			});
		}

		/// <summary>
		/// 得到类型的属性定义
		/// </summary>
		/// <param name="type"></param>
		/// <param name="propertyName"></param>
		/// <returns></returns>
		public PropertyInfo GetPropertyInfoDirectly(System.Type type, string propertyName)
		{
			type.NullCheck("type");
			propertyName.CheckStringIsNullOrEmpty("propertyName");

			Dictionary<string, PropertyInfo> propDict = GetPropertyDictionary(type);

			PropertyInfo result = null;

			propDict.TryGetValue(propertyName, out result);

			return result;
		}

		/// <summary>
		/// 得到对象的属性值
		/// </summary>
		/// <param name="target"></param>
		/// <param name="propertyName"></param>
		/// <returns></returns>
		public object GetObjectPropertyValue(object target, string propertyName)
		{
			return GetPropertyInfo(target.GetType(), propertyName).GetValue(target, null);
		}

		/// <summary>
		/// 设置对象的属性值
		/// </summary>
		/// <param name="target"></param>
		/// <param name="propertyName"></param>
		/// <param name="value"></param>
		public void SetObjectPropertyValue(object target, string propertyName, object value)
		{
			PropertyInfo pi = GetPropertyInfo(target.GetType(), propertyName);

			if (pi.CanWrite)
				pi.SetValue(target, value, null);
		}
	}

	/// <summary>
	/// 得到类型的实例化的公有或私有的属性
	/// </summary>
	public sealed class TypePropertiesWithNonPublicCacheQueue : TypePropertiesCacheQueueBase
	{
		/// <summary>
		/// 获取实例
		/// </summary>
		public static readonly TypePropertiesWithNonPublicCacheQueue Instance = CacheManager.GetInstance<TypePropertiesWithNonPublicCacheQueue>();

		//实现SingleTon模式
		private TypePropertiesWithNonPublicCacheQueue()
		{
		}

		/// <summary>
		/// 得到类型的基本属性（Instance, Public，NonPublic）
		/// </summary>
		/// <param name="type"></param>
		/// <returns></returns>
		protected override PropertyInfo[] GetProperties(Type type)
		{
			return type.GetProperties(BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);
		}
	}

	/// <summary>
	/// 为类型的实例化，公有的Properties提供Cache功能
	/// </summary>
	public sealed class TypeFlattenHierarchyPropertiesCacheQueue : TypePropertiesCacheQueueBase
	{
		/// <summary>
		/// 获取实例
		/// </summary>
		public static readonly TypeFlattenHierarchyPropertiesCacheQueue Instance = CacheManager.GetInstance<TypeFlattenHierarchyPropertiesCacheQueue>();

		//实现SingleTon模式
		private TypeFlattenHierarchyPropertiesCacheQueue()
		{
		}

		/// <summary>
		/// 得到类型的基本属性（Instance, Public，继承的）
		/// </summary>
		/// <param name="type"></param>
		/// <returns></returns>
		protected override PropertyInfo[] GetProperties(Type type)
		{
			return type.GetProperties(BindingFlags.Instance | BindingFlags.Public | BindingFlags.FlattenHierarchy);
		}
	}

	/// <summary>
	/// 为类型的Properties提供Cache功能
	/// </summary>
	public sealed class TypePropertiesCacheQueue : TypePropertiesCacheQueueBase
	{
		/// <summary>
		/// 获取实例
		/// </summary>
		public static readonly TypePropertiesCacheQueue Instance = CacheManager.GetInstance<TypePropertiesCacheQueue>();

		//实现SingleTon模式
		private TypePropertiesCacheQueue()
		{
		}

		/// <summary>
		/// 得到类型的基本属性（Instance, Public，非继承的）
		/// </summary>
		/// <param name="type"></param>
		/// <returns></returns>
		protected override PropertyInfo[] GetProperties(Type type)
		{
			return type.GetProperties();
		}
	}
}
