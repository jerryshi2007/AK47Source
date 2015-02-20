using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using MCS.Library.Configuration;

namespace MCS.Library.SOA.DataObjects.Schemas.Configuration
{
	/// <summary>
	/// Schema类型配置元素的基类
	/// </summary>
	public abstract class ObjectSchemaConfigurationElementBase : TypeConfigurationElement
	{
		/// <summary>
		/// 基本对象表的名称。默认为SC.SchemaObject
		/// </summary>
		[ConfigurationProperty("tableName", IsRequired = false)]
		public string TableName
		{
			get
			{
				return (string)this["tableName"];
			}
		}

		/// <summary>
		/// 快照的表名
		/// </summary>
		[ConfigurationProperty("snapshotTable", IsRequired = false)]
		public string SnapshotTable
		{
			get
			{
				return (string)this["snapshotTable"];
			}
		}

		/// <summary>
		/// 是否导出到通用的Snapshot表
		/// </summary>
		[ConfigurationProperty("toSchemaObjectSnapshot", IsRequired = false, DefaultValue = true)]
		public bool ToSchemaObjectSnapshot
		{
			get
			{
				return (bool)this["toSchemaObjectSnapshot"];
			}
		}

		/// <summary>
		/// 快照的类别
		/// </summary>
		[ConfigurationProperty("category", IsRequired = false)]
		public string Category
		{
			get
			{
				return (string)this["category"];
			}
		}

		/// <summary>
		/// 是不是表示关系的对象。确定哪些对象是基础对象，哪些用来表示关系
		/// </summary>
		[ConfigurationProperty("isRelation", DefaultValue = false, IsRequired = false)]
		public bool IsRelation
		{
			get
			{
				return (bool)this["isRelation"];
			}
		}

		/// <summary>
		/// 排序号
		/// </summary>
		[ConfigurationProperty("sortOrder", DefaultValue = 0, IsRequired = false)]
		public int SortOrder
		{
			get
			{
				return (int)this["sortOrder"];
			}
		}

		/// <summary>
		/// Schema所包含的页签
		/// </summary>
		[ConfigurationProperty("tabs", IsRequired = false)]
		public ObjectSchemaTabConfigurationElementCollection Tabs
		{
			get
			{
				return (ObjectSchemaTabConfigurationElementCollection)this["tabs"];
			}
		}

		/// <summary>
		/// Schema所包含的属性组
		/// </summary>
		[ConfigurationProperty("schemaClasses", IsRequired = false)]
		public ObjectSchemaClassConfigurationElementCollection Groups
		{
			get
			{
				return (ObjectSchemaClassConfigurationElementCollection)this["schemaClasses"];
			}
		}
	}
}
