using System;
using System.Text;
using System.Reflection;
using System.Collections.Generic;
using MCS.Library.Core;

namespace MCS.Library.Caching
{
	/// <summary>
	/// Ϊ���͵ĳ�Ա�ṩCache����
	/// </summary>
	public abstract class TypeMembersCacheQueue<TMember> : CacheQueue<Type, Dictionary<string, TMember>> where TMember : MemberInfo
	{
	}

	/// <summary>
	/// Ϊ���͵�Properties�ṩCache���ܵĻ���
	/// </summary>
	public abstract class TypePropertiesCacheQueueBase : TypeMembersCacheQueue<PropertyInfo>
	{
		/// <summary>
		/// �õ����͵�������Ϣ����������
		/// </summary>
		/// <param name="type"></param>
		/// <returns></returns>
		protected abstract PropertyInfo[] GetProperties(System.Type type);

		/// <summary>
		/// �õ����͵����Զ���
		/// </summary>
		/// <param name="type"></param>
		/// <param name="propertyName"></param>
		/// <returns></returns>
		public PropertyInfo GetPropertyInfo(System.Type type, string propertyName)
		{
			PropertyInfo result = GetPropertyInfoDirectly(type, propertyName);

			ExceptionHelper.FalseThrow(result != null,
				"����������{0}���ҵ�����{1}", type.Name, propertyName);

			return result;
		}

		/// <summary>
		/// �õ������ֵ䡣��������ΪKey
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
		/// �õ����͵����Զ���
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
		/// �õ����������ֵ
		/// </summary>
		/// <param name="target"></param>
		/// <param name="propertyName"></param>
		/// <returns></returns>
		public object GetObjectPropertyValue(object target, string propertyName)
		{
			return GetPropertyInfo(target.GetType(), propertyName).GetValue(target, null);
		}

		/// <summary>
		/// ���ö��������ֵ
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
	/// �õ����͵�ʵ�����Ĺ��л�˽�е�����
	/// </summary>
	public sealed class TypePropertiesWithNonPublicCacheQueue : TypePropertiesCacheQueueBase
	{
		/// <summary>
		/// ��ȡʵ��
		/// </summary>
		public static readonly TypePropertiesWithNonPublicCacheQueue Instance = CacheManager.GetInstance<TypePropertiesWithNonPublicCacheQueue>();

		//ʵ��SingleTonģʽ
		private TypePropertiesWithNonPublicCacheQueue()
		{
		}

		/// <summary>
		/// �õ����͵Ļ������ԣ�Instance, Public��NonPublic��
		/// </summary>
		/// <param name="type"></param>
		/// <returns></returns>
		protected override PropertyInfo[] GetProperties(Type type)
		{
			return type.GetProperties(BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);
		}
	}

	/// <summary>
	/// Ϊ���͵�ʵ���������е�Properties�ṩCache����
	/// </summary>
	public sealed class TypeFlattenHierarchyPropertiesCacheQueue : TypePropertiesCacheQueueBase
	{
		/// <summary>
		/// ��ȡʵ��
		/// </summary>
		public static readonly TypeFlattenHierarchyPropertiesCacheQueue Instance = CacheManager.GetInstance<TypeFlattenHierarchyPropertiesCacheQueue>();

		//ʵ��SingleTonģʽ
		private TypeFlattenHierarchyPropertiesCacheQueue()
		{
		}

		/// <summary>
		/// �õ����͵Ļ������ԣ�Instance, Public���̳еģ�
		/// </summary>
		/// <param name="type"></param>
		/// <returns></returns>
		protected override PropertyInfo[] GetProperties(Type type)
		{
			return type.GetProperties(BindingFlags.Instance | BindingFlags.Public | BindingFlags.FlattenHierarchy);
		}
	}

	/// <summary>
	/// Ϊ���͵�Properties�ṩCache����
	/// </summary>
	public sealed class TypePropertiesCacheQueue : TypePropertiesCacheQueueBase
	{
		/// <summary>
		/// ��ȡʵ��
		/// </summary>
		public static readonly TypePropertiesCacheQueue Instance = CacheManager.GetInstance<TypePropertiesCacheQueue>();

		//ʵ��SingleTonģʽ
		private TypePropertiesCacheQueue()
		{
		}

		/// <summary>
		/// �õ����͵Ļ������ԣ�Instance, Public���Ǽ̳еģ�
		/// </summary>
		/// <param name="type"></param>
		/// <returns></returns>
		protected override PropertyInfo[] GetProperties(Type type)
		{
			return type.GetProperties();
		}
	}
}
