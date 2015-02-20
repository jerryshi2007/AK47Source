using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using MCS.Library.Core;
using MCS.Library.Data.DataObjects;
using MCS.Library.SOA.DataObjects.Schemas.Configuration;

namespace MCS.Library.SOA.DataObjects.Schemas.SchemaProperties
{
	/// <summary>
	/// 表示模式标签页的定义
	/// </summary>
	[Serializable]
	public class SchemaTabDefine
	{
		public SchemaTabDefine()
		{
		}

		public SchemaTabDefine(ObjectSchemaTabConfigurationElement element)
		{
			element.NullCheck("element");

			this.Name = element.Name;
			this.Description = element.Description;
		}

		/// <summary>
		/// 获取或设置名称
		/// </summary>
		public string Name { get; set; }

		/// <summary>
		/// 获取或设置描述
		/// </summary>
		public string Description { get; set; }
	}

	/// <summary>
	/// 表示<see cref="SchemaTabDefine"/>对象的集合。
	/// </summary>
	[Serializable]
	public class SchemaTabDefineColleciton : SerializableEditableKeyedDataObjectCollectionBase<string, SchemaTabDefine>
	{
		/// <summary>
		/// 初始化<see cref="SchemaTabDefineColleciton"/>的新实例
		/// </summary>
		public SchemaTabDefineColleciton()
		{
		}


		protected SchemaTabDefineColleciton(SerializationInfo info, StreamingContext context) :
			base(info, context)
		{
		}

		public void LoadFromConfiguration(ObjectSchemaTabConfigurationElementCollection elements)
		{
			this.Clear();

			if (elements != null)
			{
				foreach (ObjectSchemaTabConfigurationElement element in elements)
					this.Add(new SchemaTabDefine(element));
			}
		}

		/// <summary>
		/// 获取集合中指定的<see cref="SchemaTabDefine"/>的键。
		/// </summary>
		/// <param name="item">获取其键的<see cref="SchemaTabDefine"/></param>
		/// <returns>表示键的字符串</returns>
		protected override string GetKeyForItem(SchemaTabDefine item)
		{
			return item.Name;
		}
	}
}
