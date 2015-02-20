using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MCS.Library.Core;
using MCS.Library.Data.Mapping;
using MCS.Library.SOA.DataObjects.Schemas.Configuration;

namespace MCS.Library.SOA.DataObjects.Schemas.SchemaProperties
{
	[Serializable]
	[XElementSerializable]
	public abstract class SchemaDefineBase
	{
		private SchemaPropertyDefineCollection _Properties = null;
		private SchemaTabDefineColleciton _Tabs = null;

		/// <summary>
		/// 从配置元素初始化
		/// </summary>
		/// <param name="schemaConfig"></param>
		protected virtual void InitFromConfigurationElement(ObjectSchemaConfigurationElementBase schemaConfig)
		{
			schemaConfig.NullCheck("schemaConfig");

			this.Name = schemaConfig.Name;
			this.TableName = schemaConfig.TableName;
			this.SnapshotTable = schemaConfig.SnapshotTable;
			this.Category = schemaConfig.Category;

			this.SortOrder = schemaConfig.SortOrder;
			this.IsRelation = schemaConfig.IsRelation;

			this.Tabs.LoadFromConfiguration(schemaConfig.Tabs);
		}

		[NoMapping]
		public SchemaPropertyDefineCollection Properties
		{
			get
			{
				if (this._Properties == null)
					this._Properties = new SchemaPropertyDefineCollection();

				return this._Properties;
			}
		}

		[NoMapping]
		public SchemaTabDefineColleciton Tabs
		{
			get
			{
				if (this._Tabs == null)
					this._Tabs = new SchemaTabDefineColleciton();

				return this._Tabs;
			}
		}

		[XElementFieldSerialize(AlternateFieldName = "_Name")]
		[ORFieldMapping("Name", PrimaryKey = true)]
		public string Name { get; set; }

		public string TableName { get; set; }
		public string SnapshotTable { get; set; }

		public string Category { get; set; }

		public int SortOrder { get; set; }

		public bool IsRelation { get; set; }

		public bool ToSchemaObjectSnapshot { get; set; }
	}
}
