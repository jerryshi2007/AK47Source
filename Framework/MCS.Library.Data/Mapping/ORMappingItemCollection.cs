#region
// -------------------------------------------------
// Assembly	��	DeluxeWorks.Library.Data
// FileName	��	ORMappingItemCollection.cs
// Remark	��	ӳ���ϵ�����ࡣ
// -------------------------------------------------
// VERSION  	AUTHOR		DATE			CONTENT
// 1.0		    ���ķ�	    20070430		����
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
	/// ӳ���ϵ������
	/// </summary>
	/// <remarks>
	/// ӳ���ϵ������
	/// <seealso cref="MCS.Library.Data.Mapping.ORMappingItem"/>
	/// </remarks>
	public class ORMappingItemCollection : EditableKeyedDataObjectCollectionBase<string, ORMappingItem>
	{
		private string tableName = string.Empty;

		/// <summary>
		/// ORMappingItemCollection��Ĺ��캯��
		/// </summary>
		/// <remarks>
		/// ORMappingItemCollection�࣬һ��ORMappingItem�ļ����࣬�����ִ�Сд
		/// </remarks>
		public ORMappingItemCollection()
			: base(StringComparer.OrdinalIgnoreCase)
		{
		}

		/// <summary>
		/// ����
		/// </summary>
		public string TableName
		{
			get { return this.tableName; }
			set { this.tableName = value; }
		}

		/// <summary>
		/// д�뵽XmlWriter
		/// </summary>
		/// <param name="writer">Xml��д������</param>
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
		/// ��XmlReader�ж�ȡ
		/// </summary>
		/// <param name="reader">Xml�Ķ�������</param>
		/// <param name="type">��������</param>
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
		/// ����ӳ���ϵ���еĶ�������������
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
		/// ����Mapping�ļ���
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
		/// ɾ��
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