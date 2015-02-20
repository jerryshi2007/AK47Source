using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MCS.Library.Core
{
	/// <summary>
	/// XElement序列化中，不需要序列化的字段定义属性
	/// </summary>
	[AttributeUsageAttribute(AttributeTargets.Class | AttributeTargets.Struct, Inherited = true, AllowMultiple = true)]
	public class NonXElementSerializedFieldsAttribute : Attribute
	{
		private Type _OwnerType = null;

		private string _FieldName = string.Empty;

		/// <summary>
		/// 构造方法
		/// </summary>
		public NonXElementSerializedFieldsAttribute()
		{
		}

		/// <summary>
		/// 构造方法
		/// </summary>
		/// <param name="ownerType"></param>
		/// <param name="fieldName"></param>
		public NonXElementSerializedFieldsAttribute(Type ownerType, string fieldName)
		{
			this._OwnerType = ownerType;
			this._FieldName = fieldName;
		}

		/// <summary>
		/// 
		/// </summary>
		public Type OwnerType
		{
			get { return _OwnerType; }
			set { _OwnerType = value; }
		}

		/// <summary>
		/// 不需要序列化的字段名称，逗号分隔，大小写敏感
		/// </summary>
		public string FieldName
		{
			get
			{
				return this._FieldName; 
			}
			set
			{
				this._FieldName = value;
			}
		}
	}
}
