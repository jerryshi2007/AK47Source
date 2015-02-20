#region
// -------------------------------------------------
// Assembly	：	DeluxeWorks.Library.Data
// FileName	：	ORMappingItem.cs
// Remark	：	本类描述了数据实体类与数据库字段间进行映射时的关系。
// -------------------------------------------------
// VERSION  	AUTHOR		DATE			CONTENT
// 1.0		    龚文芳	    20070430		创建
// -------------------------------------------------
#endregion
using System;
using System.Xml;
using System.Text;
using System.Collections.Generic;
using System.Reflection;
using System.Diagnostics;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Soap;
using MCS.Library.Core;

namespace MCS.Library.Data.Mapping
{
	/// <summary>
	/// 映射关系类
	/// </summary>
	/// <remarks>
	/// 本类描述了数据实体类与数据库字段间进行映射时的关系
	/// </remarks>
	[DebuggerDisplay("PropertyName={propertyName}")]
	public class ORMappingItem
	{
		private string propertyName = string.Empty;
		private string dataFieldName = string.Empty;
		private string format = string.Empty;
		private bool isIdentity = false;
		private bool primaryKey = false;
		private int length = 0;
		private bool isNullable = true;
		private string subClassTypeDescription = string.Empty;
		private string subClassPropertyName = string.Empty;
		private ClauseBindingFlags bindingFlags = ClauseBindingFlags.All;
		private string defaultExpression = string.Empty;

		private bool encryptProperty = false;
		private string encryptorName = null;

		private MemberInfo memberInfo = null;
		private Type declaringType = null;
		private EnumUsageTypes enumUsage = EnumUsageTypes.UseEnumValue;

		/// <summary>
		/// 构造方法
		/// </summary>
		public ORMappingItem()
		{
		}

		/// <summary>
		/// 写入到XmlWriter
		/// </summary>
		/// <param name="writer">XML编写器</param>
		public void WriteToXml(XmlWriter writer)
		{
			XmlHelper.AppendNotNullAttr(writer, "propertyName", this.propertyName);
			XmlHelper.AppendNotNullAttr(writer, "dataFieldName", this.dataFieldName);
			XmlHelper.AppendNotNullAttr(writer, "format", this.format);

			if (this.isIdentity)
				XmlHelper.AppendNotNullAttr(writer, "isIdentity", this.isIdentity);

			if (this.primaryKey)
				XmlHelper.AppendNotNullAttr(writer, "primaryKey", this.primaryKey);

			if (this.length != 0)
				XmlHelper.AppendNotNullAttr(writer, "length", this.length);

			if (this.isNullable == false)
				XmlHelper.AppendNotNullAttr(writer, "isNullable", this.isNullable);

			if (this.bindingFlags != ClauseBindingFlags.All)
				XmlHelper.AppendNotNullAttr(writer, "bindingFlags", this.bindingFlags.ToString());

			XmlHelper.AppendNotNullAttr(writer, "defaultExpression", this.defaultExpression);

			if (this.enumUsage != EnumUsageTypes.UseEnumValue)
				XmlHelper.AppendNotNullAttr(writer, "enumUsage", this.enumUsage.ToString());

			XmlHelper.AppendNotNullAttr(writer, "subClassPropertyName", this.subClassPropertyName);
			XmlHelper.AppendNotNullAttr(writer, "subClassTypeDescription", this.subClassTypeDescription);

			if (this.encryptProperty)
				XmlHelper.AppendNotNullAttr(writer, "encryptProperty", this.encryptProperty);

			XmlHelper.AppendNotNullAttr(writer, "encryptorName", this.encryptorName);
		}

		/// <summary>
		/// 从XmlReader中设置属性
		/// </summary>
		/// <param name="reader">Xml阅读器</param>
		/// <param name="dType">DeclaringType</param>
		/// <param name="mi">成员属性</param>
		public void ReadFromXml(XmlReader reader, System.Type dType, MemberInfo mi)
		{
			this.memberInfo = mi;
			this.declaringType = dType;

			this.propertyName = XmlHelper.GetAttributeValue(reader, "propertyName", this.propertyName);
			this.dataFieldName = XmlHelper.GetAttributeValue(reader, "dataFieldName", this.dataFieldName);
			this.format = XmlHelper.GetAttributeValue(reader, "format", this.format);

			this.isIdentity = XmlHelper.GetAttributeValue(reader, "isIdentity", this.isIdentity);
			this.primaryKey = XmlHelper.GetAttributeValue(reader, "primaryKey", this.primaryKey);
			this.length = XmlHelper.GetAttributeValue(reader, "length", this.length);

			this.isNullable = XmlHelper.GetAttributeValue(reader, "isNullable", this.isNullable);
			this.bindingFlags = XmlHelper.GetAttributeValue(reader, "bindingFlags", this.bindingFlags);

			this.defaultExpression = XmlHelper.GetAttributeValue(reader, "defaultExpression", this.defaultExpression);

			this.enumUsage = XmlHelper.GetAttributeValue(reader, "enumUsage", this.enumUsage);
			this.subClassPropertyName = XmlHelper.GetAttributeValue(reader, "subClassPropertyName", this.subClassPropertyName);
			this.subClassTypeDescription = XmlHelper.GetAttributeValue(reader, "subClassTypeDescription", this.subClassTypeDescription);

			this.encryptProperty = XmlHelper.GetAttributeValue(reader, "encryptProperty", false);
			this.encryptorName = XmlHelper.GetAttributeValue(reader, "encryptorName", (string)null);
		}

		/// <summary>
		/// 生成SQL Value时候的格式化串。例如{0:0000}
		/// </summary>
		public string Format
		{
			get { return this.format; }
			set { this.format = value; }
		}

		/// <summary>
		/// Enum的值或其描述
		/// </summary>
		public EnumUsageTypes EnumUsage
		{
			get { return this.enumUsage; }
			set { this.enumUsage = value; }
		}

		/// <summary>
		/// 对应的属性为空时，所提供的缺省值表达式
		/// </summary>
		public string DefaultExpression
		{
			get
			{
				return this.defaultExpression;
			}
			set
			{
				this.defaultExpression = value;
			}
		}

		/// <summary>
		/// 对应的属性值会出现在哪些Sql语句中
		/// </summary>
		public ClauseBindingFlags BindingFlags
		{
			get { return this.bindingFlags; }
			set { this.bindingFlags = value; }
		}

		/// <summary>
		/// 字段是否为空
		/// </summary>
		public bool IsNullable
		{
			get { return this.isNullable; }
			set { this.isNullable = value; }
		}

		/// <summary>
		/// 字段长度
		/// </summary>
		public int Length
		{
			get { return this.length; }
			set { this.length = value; }
		}

		/// <summary>
		/// 属性名
		/// </summary>
		public string PropertyName
		{
			get { return this.propertyName; }
			set { this.propertyName = value; }
		}

		/// <summary>
		/// 如果有子对象，对应的子对象属性的名称
		/// </summary>
		public string SubClassPropertyName
		{
			get { return this.subClassPropertyName; }
			set { this.subClassPropertyName = value; }
		}

		/// <summary>
		/// 对应的数据库字段名
		/// </summary>
		public string DataFieldName
		{
			get { return this.dataFieldName; }
			set { this.dataFieldName = value; }
		}

		/// <summary>
		/// 是否标识列
		/// </summary>
		public bool IsIdentity
		{
			get { return this.isIdentity; }
			set { this.isIdentity = value; }
		}

		/// <summary>
		/// 是否主键
		/// </summary>
		public bool PrimaryKey
		{
			get { return this.primaryKey; }
			set { this.primaryKey = value; }
		}

		/// <summary>
		/// MemberInfo类
		/// </summary>
		/// <remarks>
		/// Obtains information about the attributes of a member and provides access to member metadata. 
		/// </remarks>
		public MemberInfo MemberInfo
		{
			get { return this.memberInfo; }
			internal set { this.memberInfo = value; }
		}

		/// <summary>
		/// 属性所属于的类
		/// </summary>
		public Type DeclaringType
		{
			get { return this.declaringType; }
			internal set { this.declaringType = value; }
		}

		/// <summary>
		/// 子对象的类型描述
		/// </summary>
		public string SubClassTypeDescription
		{
			get { return subClassTypeDescription; }
			internal set { this.subClassTypeDescription = value; }
		}

		/// <summary>
		/// 是否加密属性
		/// </summary>
		public bool EncryptProperty
		{
			get { return this.encryptProperty; }
			set { this.encryptProperty = value; }
		}

		/// <summary>
		/// 加密器的名称
		/// </summary>
		public string EncryptorName
		{
			get { return this.encryptorName; }
			set { this.encryptorName = value; }
		}

		/// <summary>
		/// 复制一个MappingItem
		/// </summary>
		/// <returns></returns>
		public ORMappingItem Clone()
		{
			ORMappingItem newItem = new ORMappingItem();

			newItem.dataFieldName = this.dataFieldName;
			newItem.propertyName = this.propertyName;
			newItem.subClassPropertyName = this.subClassPropertyName;
			newItem.isIdentity = this.isIdentity;
			newItem.primaryKey = this.primaryKey;
			newItem.length = this.length;
			newItem.isNullable = this.isNullable;
			newItem.subClassTypeDescription = this.subClassTypeDescription;
			newItem.bindingFlags = this.bindingFlags;
			newItem.defaultExpression = this.defaultExpression;
			newItem.memberInfo = this.memberInfo;
			newItem.enumUsage = this.enumUsage;
			newItem.encryptProperty = this.encryptProperty;
			newItem.encryptorName = this.encryptorName;
			newItem.format = this.format;

			return newItem;
		}
	}
}
