using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using MCS.Library.Core;
using MCS.Library.Data.DataObjects;
using MCS.Library.SOA.DataObjects.Schemas.Configuration;

namespace MCS.Library.SOA.DataObjects.Security
{
	[Serializable]
	[XElementSerializable]
	public class SchemaDefineCollection : SerializableEditableKeyedDataObjectCollectionBase<string, SchemaDefine>
	{
		public SchemaDefineCollection()
		{
		}

		protected SchemaDefineCollection(SerializationInfo info, StreamingContext context) :
			base(info, context)
		{
		}

		public SchemaDefineCollection(ObjectSchemaConfigurationElementCollection objectsConfig)
		{
			AppendPropertiesFromConfiguration(objectsConfig);
		}

		protected override string GetKeyForItem(SchemaDefine item)
		{
			return item.Name;
		}

		public void LoadFromConfiguration()
		{
			this.LoadFromConfiguration(ObjectSchemaSettings.GetConfig().Schemas);
		}

		public void LoadFromConfiguration(ObjectSchemaConfigurationElementCollection objectsConfig)
		{
			this.Clear();

			AppendPropertiesFromConfiguration(objectsConfig);
		}

		public void AppendPropertiesFromConfiguration(ObjectSchemaConfigurationElementCollection objectsConfig)
		{
			if (objectsConfig != null)
			{
				foreach (ObjectSchemaConfigurationElement objectElem in objectsConfig)
				{
					SchemaDefine sd = new SchemaDefine(objectElem);

					if (this.ContainsKey(sd.Name) == false)
						this.Add(sd);
				}
			}
		}

		/// <summary>
		/// 按照Category进行筛选
		/// </summary>
		/// <param name="categoryName"></param>
		/// <returns></returns>
		public SchemaDefineCollection FilterByCategory(string categoryName)
		{
			SchemaDefineCollection result = new SchemaDefineCollection();

			foreach (SchemaDefine si in this)
			{
				if (si.Category == categoryName)
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
	}
}
