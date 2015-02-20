#region
// -------------------------------------------------
// Assembly	：	DeluxeWorks.Library.Data
// FileName	：	ORMappingItemCollection.cs
// Remark	：	映射关系集合类。
// -------------------------------------------------
// VERSION  	AUTHOR		DATE			CONTENT
// 1.0		    龚文芳	    20070430		创建
// -------------------------------------------------
#endregion
using System;
using System.Text;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Xml;
using System.Reflection;
using MCS.Library.Core;
using MCS.Library.Data.DataObjects;

namespace MCS.Library.Data.Mapping
{
	/// <summary>
	/// 映射关系集合类
	/// </summary>
	/// <remarks>
	/// 映射关系集合类
	/// <seealso cref="MCS.Library.Data.Mapping.ORMappingItem"/>
	/// </remarks>
	public class ORMappingItemCollection : EditableKeyedDataObjectCollectionBase<string, ORMappingItem>
	{
		private string tableName = string.Empty;

		/// <summary>
		/// ORMappingItemCollection类的构造函数
		/// </summary>
		/// <remarks>
		/// ORMappingItemCollection类，一个ORMappingItem的集合类，不区分大小写
		/// </remarks>
		public ORMappingItemCollection()
			: base(StringComparer.OrdinalIgnoreCase)
		{
		}

		/// <summary>
		/// 表名
		/// </summary>
		public string TableName
		{
			get { return this.tableName; }
			set { this.tableName = value; }
		}

		/// <summary>
		/// 写入到XmlWriter
		/// </summary>
		/// <param name="writer">Xml编写器对象</param>
		public void WriteToXml(XmlWriter writer)
		{
			ExceptionHelper.FalseThrow<ArgumentNullException>(writer != null, "writer");

			writer.WriteStartElement("ORMapping");
			writer.WriteAttributeString("tableName", this.tableName);

			foreach (ORMappingItem item in this)
			{
				writer.WriteStartElement("Item");
				item.WriteToXml(writer);
				writer.WriteEndElement();
			}

			writer.WriteEndElement();
		}

		/// <summary>
		/// 从XmlReader中读取
		/// </summary>
		/// <param name="reader">Xml阅读器对象</param>
		/// <param name="type">对象类型</param>
		public void ReadFromXml(XmlReader reader, System.Type type)
		{
			ExceptionHelper.FalseThrow<ArgumentNullException>(reader != null, "reader");
			ExceptionHelper.FalseThrow<ArgumentNullException>(type != null, "type");

			this.Clear();
			Dictionary<string, MemberInfo> miDict = GetMemberInfoDict(type);

			while (reader.EOF == false)
			{
				reader.Read();

				if (reader.IsStartElement("ORMapping"))
				{
					this.tableName = XmlHelper.GetAttributeValue(reader, "tableName", string.Empty);
					reader.ReadToDescendant("Item");
				}

				if (reader.IsStartElement("Item"))
				{
					string propName = reader.GetAttribute("propertyName");
					string subClassPropertyName = reader.GetAttribute("subClassPropertyName");
					string subClassTypeDescription = reader.GetAttribute("subClassTypeDescription");

					MemberInfo mi = null;

					if (miDict.TryGetValue(propName, out mi))
					{
						if (string.IsNullOrEmpty(subClassPropertyName) == false)
							if (string.IsNullOrEmpty(subClassTypeDescription) == false)
								mi = ORMapping.GetSubClassMemberInfoByName(subClassPropertyName,
									TypeCreator.GetTypeInfo(subClassTypeDescription));
							else
								mi = ORMapping.GetSubClassMemberInfoByName(subClassPropertyName, mi);

						if (mi != null)
							ReadItemFromXml(reader, type, mi);
					}
				}
			}
		}

		/// <summary>
		/// 根据映射关系的中的对象类型来过滤
		/// </summary>
		/// <param name="type"></param>
		/// <returns></returns>
		public ORMappingItemCollection FilterMappingInfoByDeclaringType(System.Type type)
		{
			ORMappingItemCollection result = new ORMappingItemCollection();

			result.TableName = this.TableName;

			foreach (ORMappingItem item in this)
			{
				if (item.DeclaringType == type)
					result.Add(item);
			}

			return result;
		}

		/// <summary>
		/// 复制Mapping的集合
		/// </summary>
		/// <returns></returns>
		public ORMappingItemCollection Clone()
		{
			ORMappingItemCollection items = new ORMappingItemCollection();

			items.tableName = this.tableName;

			foreach (ORMappingItem item in this)
			{
				items.Add(item.Clone());
			}

			return items;
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="fieldName"></param>
		/// <returns></returns>
		public bool Contains(string fieldName)
		{
			return ContainsKey(fieldName);
		}

		/// <summary>
		/// 删除
		/// </summary>
		/// <param name="fieldName"></param>
		public void Remove(string fieldName)
		{
			fieldName.CheckStringIsNullOrEmpty("fieldName");

			base.Remove(item => string.Compare(item.DataFieldName, fieldName, true) == 0);
		}

		private Dictionary<string, MemberInfo> GetMemberInfoDict(System.Type type)
		{
			MemberInfo[] mis = type.GetMembers(BindingFlags.Instance | BindingFlags.Public | BindingFlags.FlattenHierarchy);

			Dictionary<string, MemberInfo> dict = new Dictionary<string, MemberInfo>();

			foreach (MemberInfo mi in mis)
			{
				if (mi.MemberType == MemberTypes.Property || mi.MemberType == MemberTypes.Field)
					dict[mi.Name] = mi;
			}

			return dict;
		}

		private void ReadItemFromXml(XmlReader reader, System.Type type, MemberInfo mi)
		{
			ORMappingItem item = new ORMappingItem();

			item.ReadFromXml(reader, type, mi);

			this.Add(item);
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="item"></param>
		/// <returns></returns>
		protected override string GetKeyForItem(ORMappingItem item)
		{
			return item.DataFieldName;
		}
	}
}