using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using MCS.Library.Data.DataObjects;
using MCS.Library.Data.Mapping;
using MCS.Library.SOA.DataObjects.Schemas.Configuration;
using MCS.Library.SOA.DataObjects.Schemas.SchemaProperties;
using MCS.Library.SOA.DataObjects.Security.Configuration;

namespace MCS.Library.SOA.DataObjects.Security
{
	/// <summary>
	/// SchemaInfo是SchemaDefine的简单描述类，用于获取Schema的信息，不包含Properties的加载
	/// </summary>
	[Serializable]
	[ORTableMapping("SC.SchemaDefine")]
	public class SchemaInfo
	{
		public SchemaInfo()
		{
		}

		/// <summary>
		/// 得到按照Category过滤的Schema定义集合
		/// </summary>
		/// <param name="categoryNames"></param>
		/// <returns></returns>
		public static SchemaInfoCollection FilterByCategory(params string[] categoryNames)
		{
			SchemaInfoCollection result = new SchemaInfoCollection();

			SchemaInfoCollection schemas = new SchemaInfoCollection();
			schemas.LoadFromConfiguration();

			foreach (string categoryName in categoryNames)
			{
				SchemaInfoCollection filtered = schemas.FilterByCategory(categoryName);

				filtered.ForEach(s => result.AddNotExistsItem(s));
			}

			return result;
		}

		/// <summary>
		/// 得到按照Category过滤的Schema定义集合
		/// </summary>
		/// <param name="categoryName"></param>
		/// <returns></returns>
		public static SchemaInfoCollection FilterByCategory(string categoryName)
		{
			SchemaInfoCollection schemas = new SchemaInfoCollection();
			schemas.LoadFromConfiguration();

			SchemaInfoCollection filtered = schemas.FilterByCategory(categoryName);

			return filtered;
		}

		/// <summary>
		/// 得到按照CodeNameKey过滤的Schema定义集合
		/// </summary>
		/// <param name="codeNameKeys"></param>
		/// <returns></returns>
		public static SchemaInfoCollection FilterByCodeNameKey(params string[] codeNameKeys)
		{
			SchemaInfoCollection result = new SchemaInfoCollection();

			SchemaInfoCollection schemas = new SchemaInfoCollection();
			schemas.LoadFromConfiguration();

			foreach (string codeNameKey in codeNameKeys)
			{
				SchemaInfoCollection filtered = schemas.FilterByCodeNameKey(codeNameKey);

				filtered.ForEach(s => result.AddNotExistsItem(s));
			}

			return result;
		}

		public SchemaInfo(ObjectSchemaConfigurationElement configElem)
		{
			if (configElem != null)
			{
				this.Name = configElem.Name;
				this.Description = configElem.Description;
				this.Category = configElem.Category;
				this.CodeNameKey = configElem.CodeNameKey;
				this.CodeNameValidationMethod = configElem.CodeNameValidationMethod;
				this.FullPathValidationMethod = configElem.FullPathValidationMethod;
				this.SortOrder = configElem.SortOrder;
				this.TableName = configElem.TableName;
				this.SnapshotTable = configElem.SnapshotTable;
				this.IsRelation = configElem.IsRelation;
				this.IsUsersContainer = configElem.IsUsersContainer;
				this.IsUsersContainerMember = configElem.IsUsersContainerMember;
				this.ToSchemaObjectSnapshot = configElem.ToSchemaObjectSnapshot;
			}
		}

		[ORFieldMapping("Name", PrimaryKey = true)]
		public string Name { get; set; }

		public string Description { get; set; }

		public string Category { get; set; }

		public string CodeNameKey { get; set; }

		public SchemaObjectCodeNameValidationMethod CodeNameValidationMethod { get; set; }
		public SCRelationFullPathValidationMethod FullPathValidationMethod { get; set; }

		public int SortOrder { get; set; }

		public bool IsRelation { get; set; }

		/// <summary>
		/// 是不是群组或角色之类的人员的容器
		/// </summary>
		public bool IsUsersContainer { get; set; }

		/// <summary>
		/// 是不是群组或角色之类的人员的容器的成员。通常人员会设置为true
		/// </summary>
		public bool IsUsersContainerMember { get; set; }

		public string TableName { get; set; }

		public string SnapshotTable { get; set; }

		/// <summary>
		/// 是否导出到通用的Snapshot表
		/// </summary>
		public bool ToSchemaObjectSnapshot { get; set; }
	}

	[Serializable]
	public class SchemaInfoCollection : SerializableEditableKeyedDataObjectCollectionBase<string, SchemaInfo>
	{
		public SchemaInfoCollection()
		{
		}

		protected SchemaInfoCollection(SerializationInfo info, StreamingContext context) :
			base(info, context)
		{
		}

		public SchemaInfoCollection(ObjectSchemaConfigurationElementCollection schemaElements)
		{
			LoadFromConfiguration(schemaElements);
		}

		public void LoadFromConfiguration()
		{
			this.LoadFromConfiguration(ObjectSchemaSettings.GetConfig().Schemas);
		}

		public void LoadFromConfiguration(ObjectSchemaConfigurationElementCollection schemaElements)
		{
			if (schemaElements != null)
			{
				foreach (ObjectSchemaConfigurationElement elem in schemaElements)
					this.Add(new SchemaInfo(elem));

				this.Sort((x, y) => x.SortOrder - y.SortOrder);
			}
		}

		/// <summary>
		/// 按照Category进行筛选
		/// </summary>
		/// <param name="categoryName"></param>
		/// <returns></returns>
		public SchemaInfoCollection FilterByCategory(string categoryName)
		{
			SchemaInfoCollection result = new SchemaInfoCollection();

			foreach (SchemaInfo si in this)
			{
				if (si.Category == categoryName)
					result.Add(si);
			}

			result.Sort((x, y) => x.SortOrder - y.SortOrder);

			return result;
		}

		/// <summary>
		/// 按照CodeNameKey进行筛选
		/// </summary>
		/// <param name="codeNameKey"></param>
		/// <returns></returns>
		public SchemaInfoCollection FilterByCodeNameKey(string codeNameKey)
		{
			SchemaInfoCollection result = new SchemaInfoCollection();

			foreach (SchemaInfo si in this)
			{
				if (si.CodeNameKey == codeNameKey)
					result.Add(si);
			}

			result.Sort((x, y) => x.SortOrder - y.SortOrder);

			return result;
		}

		/// <summary>
		/// 将Schema的名称变成一个数组
		/// </summary>
		/// <returns></returns>
		public string[] ToSchemaNames()
		{
			List<string> result = new List<string>(this.Count);

			this.ForEach(sd => result.Add(sd.Name));

			return result.ToArray();
		}

		/// <summary>
		/// 筛选出非关系的对象
		/// </summary>
		/// <returns></returns>
		public SchemaInfoCollection FilterByNotRelation()
		{
			SchemaInfoCollection result = new SchemaInfoCollection();

			foreach (SchemaInfo si in this)
			{
				if (si.IsRelation == false)
					result.Add(si);
			}

			result.Sort((x, y) => x.SortOrder - y.SortOrder);

			return result;
		}

		protected override string GetKeyForItem(SchemaInfo item)
		{
			return item.Name;
		}
	}

	public static class SchemaUtil
	{
		public static bool Contains(this string[] schemas, string schema)
		{
			for (int i = schemas.Length - 1; i >= 0; i--)
				if (schemas[i] == schema)
					return true;

			return false;
		}
	}
}
