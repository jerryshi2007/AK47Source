using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Reflection;
using MCS.Library.Caching;
using System.Diagnostics;
using System.Runtime.CompilerServices;

namespace MCS.Library.Core
{
	/// <summary>
	/// 类型的属性信息
	/// </summary>
	[DebuggerDisplay("FieldName = {FieldName}")]
	internal struct TypeFieldInfo
	{
		/// <summary>
		/// Field所属的对象的类型
		/// </summary>
		public Type ObjectType;

		/// <summary>
		/// Field的名字
		/// </summary>
		public string FieldName;
	}

	/// <summary>
	/// 
	/// </summary>
	[DebuggerDisplay("Name = {_FieldInfo.Name}")]
	public class ExtendedFieldInfo
	{
		private FieldInfo _FieldInfo = null;
		private bool _IsNotSerialized = false;
		private string _AlternateFieldName = null;
		private bool _IgnoreDeserializeError = false;

		/// <summary>
		/// 
		/// </summary>
		/// <param name="fieldInfo"></param>
		public ExtendedFieldInfo(FieldInfo fieldInfo)
		{
			MemberInfo declaringInfo = fieldInfo;

			if (fieldInfo.IsDefined(typeof(CompilerGeneratedAttribute), false))
				declaringInfo = GetFieldOriginalMemberInfo(fieldInfo);

			XElementFieldSerializeAttribute fieldAttr = AttributeHelper.GetCustomAttribute<XElementFieldSerializeAttribute>(declaringInfo);

			if (fieldAttr != null)
			{
				this._AlternateFieldName = fieldAttr.AlternateFieldName;
				this._IgnoreDeserializeError = fieldAttr.IgnoreDeserializeError;
			}

			this._FieldInfo = fieldInfo;
		}

		private static MemberInfo GetFieldOriginalMemberInfo(FieldInfo fi)
		{
			MemberInfo result = fi;

			Type declaringType = fi.DeclaringType;

			PropertyInfo[] pis = declaringType.GetProperties(BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Public);

			foreach (PropertyInfo pi in pis)
			{
				if (fi.Name == string.Format("<{0}>k__BackingField", pi.Name))
				{
					result = pi;
					break;
				}
			}

			return result;
		}

		/// <summary>
		/// 
		/// </summary>
		public FieldInfo FieldInfo
		{
			get { return this._FieldInfo; }
		}

		/// <summary>
		/// 
		/// </summary>
		public bool IsNotSerialized
		{
			get { return this._IsNotSerialized; }
			set { this._IsNotSerialized = value; }
		}

		/// <summary>
		/// 
		/// </summary>
		public string AlternateFieldName
		{
			get
			{
				string result = this._AlternateFieldName;

				if (result.IsNullOrEmpty() && this.FieldInfo != null)
					result = FieldInfo.Name;

				return result;

			}
			set
			{
				this._AlternateFieldName = value;
			}
		}

		/// <summary>
		/// 忽略反序列化时的错误
		/// </summary>
		public bool IgnoreDeserializeError
		{
			get
			{
				return this._IgnoreDeserializeError;
			}
			set
			{
				this._IgnoreDeserializeError = value;
			}
		}
	}

	internal class TypeFields
	{
		private bool _XElementSerializable = false;

		private Dictionary<TypeFieldInfo, ExtendedFieldInfo> fields = new Dictionary<TypeFieldInfo, ExtendedFieldInfo>();

		/// <summary>
		/// 字段信息
		/// </summary>
		public Dictionary<TypeFieldInfo, ExtendedFieldInfo> Fields
		{
			get { return fields; }
		}

		private TypeFields()
		{
		}

		public bool XElementSerializable
		{
			get { return this._XElementSerializable; }
		}

		/// <summary>
		/// 得到类型的Fields信息
		/// </summary>
		/// <param name="type"></param>
		/// <returns></returns>
		public static TypeFields GetTypeFields(Type type)
		{
			TypeFields result = TypeFieldsCache.Instance.GetOrAddNewValue(type, (cache, key) =>
			{
				TypeFields tf = new TypeFields();

				FillFieldsInfo(type, tf.Fields);

				tf._XElementSerializable = (XElementSerializableAttribute.GetCustomAttribute(type, typeof(XElementSerializableAttribute)) != null);

				Attribute[] attrs = NonXElementSerializedFieldsAttribute.GetCustomAttributes(type, typeof(NonXElementSerializedFieldsAttribute));

				foreach (KeyValuePair<TypeFieldInfo, ExtendedFieldInfo> kp in tf.Fields)
				{
					kp.Value.IsNotSerialized = kp.Value.FieldInfo.IsNotSerialized;

					if (kp.Value.IsNotSerialized == false)
					{
						kp.Value.IsNotSerialized = Array.Exists(attrs, attr =>
						{
							NonXElementSerializedFieldsAttribute nfa = (NonXElementSerializedFieldsAttribute)attr;
							return nfa.OwnerType == kp.Key.ObjectType && nfa.FieldName == kp.Value.FieldInfo.Name;
						});
					}
				}

				cache.Add(key, tf);

				return tf;
			});

			return result;
		}

		/// <summary>
		/// 得到类型具体的Field信息
		/// </summary>
		/// <param name="type"></param>
		/// <param name="fieldName"></param>
		/// <returns></returns>
		public static ExtendedFieldInfo GetTypeField(Type type, string fieldName)
		{
			(type != null).FalseThrow<ArgumentNullException>("type");
			fieldName.CheckStringIsNullOrEmpty("fieldName");

			TypeFields tf = GetTypeFields(type);

			ExtendedFieldInfo fi;

			TypeFieldInfo tfi = new TypeFieldInfo() { ObjectType = type, FieldName = fieldName };

			tf.Fields.TryGetValue(tfi, out fi).FalseThrow("不能在类型{0}中找到字段{1}", type.FullName, fieldName);

			return fi;
		}

		private static void FillFieldsInfo(System.Type type, Dictionary<TypeFieldInfo, ExtendedFieldInfo> result)
		{
			FieldInfo[] fields = type.GetFields(BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.Public);

			foreach (FieldInfo fi in fields)
			{
				TypeFieldInfo tfi = new TypeFieldInfo();

				tfi.FieldName = fi.Name;
				tfi.ObjectType = type;

				result.Add(tfi, new ExtendedFieldInfo(fi));
			}

			if (type.BaseType != typeof(object))
				FillFieldsInfo(type.BaseType, result);
		}
	}

	/// <summary>
	/// 类型属性信息的缓存类
	/// </summary>
	internal sealed class TypeFieldsCache : CacheQueue<Type, TypeFields>
	{
		/// <summary>
		/// 获取缓存实例
		/// </summary>
		public static readonly TypeFieldsCache Instance = CacheManager.GetInstance<TypeFieldsCache>();

		private TypeFieldsCache()
		{
		}
	}
}
