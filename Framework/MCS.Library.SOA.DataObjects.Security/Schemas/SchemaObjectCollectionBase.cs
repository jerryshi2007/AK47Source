using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using MCS.Library.Core;
using MCS.Library.Data.DataObjects;
using MCS.Library.Data.Mapping;
using MCS.Library.SOA.DataObjects.Schemas.Configuration;
using MCS.Library.SOA.DataObjects.Security.Configuration;
using MCS.Library.SOA.DataObjects.Schemas.SchemaProperties;

namespace MCS.Library.SOA.DataObjects.Security
{
	[Serializable]
	public abstract class SchemaObjectCollectionBase<T, TFilterResult> :
		EditableDataObjectCollectionBase<T>
		where T : SchemaObjectBase
		where TFilterResult : EditableDataObjectCollectionBase<T>
	{
		/// <summary>
		/// 创建过滤结果的集合
		/// </summary>
		/// <returns></returns>
		protected abstract TFilterResult CreateFilterResultCollection();

		public virtual void AddRange(IEnumerable<T> source)
		{
			foreach (T item in source)
			{
				this.Add(item);
			}
		}

		/// <summary>
		/// 获取按状态进行过滤的结果的集合
		/// </summary>
		/// <param name="filter"><see cref="SchemaObjectStatusFilterTypes"/>值之一，表示过滤的类型</param>
		/// <returns></returns>
		public TFilterResult FilterByStatus(SchemaObjectStatusFilterTypes filter)
		{
			TFilterResult result = CreateFilterResultCollection();

			foreach (T obj in this)
			{
				if ((filter & SchemaObjectStatusFilterTypes.Normal) != SchemaObjectStatusFilterTypes.None &&
					obj.Status == SchemaObjectStatus.Normal)
				{
					result.Add(obj);
				}

				if ((filter & SchemaObjectStatusFilterTypes.Deleted) != SchemaObjectStatusFilterTypes.None &&
					obj.Status == SchemaObjectStatus.Deleted)
				{
					result.Add(obj);
				}
			}

			return result;
		}

		public void LoadFromDataView(DataView view)
		{
			foreach (DataRowView drv in view)
			{
				T obj = (T)SchemaExtensions.CreateObject((string)drv["SchemaType"]);

				obj.FromString((string)drv["Data"]);

				ORMapping.DataRowToObject(drv.Row, obj);

				this.Add(obj);
			}
		}
	}

	[Serializable]
	public abstract class SchemaObjectEditableKeyedCollectionBase<T, TFilterResult> :
		SerializableEditableKeyedDataObjectCollectionBase<string, T>
		where T : SchemaObjectBase
		where TFilterResult : EditableKeyedDataObjectCollectionBase<string, T>
	{
		public SchemaObjectEditableKeyedCollectionBase()
			: base(100)
		{
		}

		public SchemaObjectEditableKeyedCollectionBase(int capacity)
			: base(capacity)
		{
		}

		protected SchemaObjectEditableKeyedCollectionBase(SerializationInfo info, StreamingContext context) :
			base(info, context)
		{
		}

		/// <summary>
		/// 创建过滤结果的集合
		/// </summary>
		/// <returns></returns>
		protected abstract TFilterResult CreateFilterResultCollection();

		public TFilterResult FilterByStatus(SchemaObjectStatusFilterTypes filter)
		{
			TFilterResult result = CreateFilterResultCollection();

			foreach (T obj in this)
			{
				if ((filter & SchemaObjectStatusFilterTypes.Normal) != SchemaObjectStatusFilterTypes.None &&
					obj.Status == SchemaObjectStatus.Normal)
				{
					result.Add(obj);
				}

				if ((filter & SchemaObjectStatusFilterTypes.Deleted) != SchemaObjectStatusFilterTypes.None &&
					obj.Status == SchemaObjectStatus.Deleted)
				{
					result.Add(obj);
				}
			}

			return result;
		}

		public void LoadFromDataView(DataView view, Action<DataRow, T> action)
		{
			Dictionary<string, ObjectSchemaConfigurationElement> schemaElements = new Dictionary<string, ObjectSchemaConfigurationElement>(StringComparer.OrdinalIgnoreCase);

			ObjectSchemaSettings settings = ObjectSchemaSettings.GetConfig();

			foreach (DataRowView drv in view)
			{
				string schemaType = (string)drv["SchemaType"];

				ObjectSchemaConfigurationElement schemaElement = null;

				if (schemaElements.TryGetValue(schemaType, out schemaElement) == false)
				{
					schemaElement = settings.Schemas[schemaType];

					schemaElements.Add(schemaType, schemaElement);
				}

				if (schemaElement != null)
				{
					T obj = (T)schemaElement.CreateInstance(schemaType);

					obj.FromString((string)drv["Data"]);

					ORMapping.DataRowToObject(drv.Row, obj);

					if (action != null)
						action(drv.Row, obj);

					if (this.ContainsKey(obj.ID) == false)
						this.Add(obj);
				}
			}
		}

		public void LoadFromDataView(DataView view)
		{
			LoadFromDataView(view, null);
		}

		public void LoadFromDataReader(IDataReader reader)
		{
			Dictionary<string, ObjectSchemaConfigurationElement> schemaElements = new Dictionary<string, ObjectSchemaConfigurationElement>(StringComparer.OrdinalIgnoreCase);

			ObjectSchemaSettings settings = ObjectSchemaSettings.GetConfig();

			while (reader.Read())
			{
				string schemaType = (string)reader["SchemaType"];
				ObjectSchemaConfigurationElement schemaElement = null;

				if (schemaElements.TryGetValue(schemaType, out schemaElement) == false)
				{
					schemaElement = settings.Schemas[schemaType];

					schemaElements.Add(schemaType, schemaElement);
				}

				if (schemaElement != null)
				{
					T obj = (T)schemaElement.CreateInstance(schemaType);

					obj.FromString((string)reader["Data"]);

					ORMapping.DataReaderToObject(reader, obj);

					if (this.ContainsKey(obj.ID) == false)
						this.Add(obj);
				}
			}
		}

		public string[] ToIDArray()
		{
			List<string> result = new List<string>(this.Count);

			this.ForEach(s => result.Add(s.ID));

			return result.ToArray();
		}
	}
}
