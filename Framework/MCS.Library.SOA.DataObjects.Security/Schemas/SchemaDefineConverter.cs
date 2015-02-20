using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web.Script.Serialization;
using MCS.Library.Core;
using MCS.Library.SOA.DataObjects.Schemas.SchemaProperties;
using MCS.Web.Library.Script;

namespace MCS.Library.SOA.DataObjects.Security
{
	/// <summary>
	/// 表示<see cref="SchemaDefine"/>类型的转换器
	/// </summary>
	public sealed class SchemaDefineConverter : JavaScriptConverter
	{
		/// <summary>
		/// 将所提供的字典转换为<see cref="SchemaDefine"/>类型的对象。
		/// </summary>
		/// <param name="dictionary">作为名称/值对存储的属性数据的 <see cref="T:System.Collections.Generic.IDictionary^2"/>  实例。</param>
		/// <param name="type">所生成对象的类型。</param>
		/// <param name="serializer"><see cref="System.Web.Script.Serialization.JavaScriptSerializer"/>实例。</param>
		/// <returns>反序列化的对象。</returns>
		public override object Deserialize(IDictionary<string, object> dictionary, Type type, JavaScriptSerializer serializer)
		{
			SchemaDefine schemaDefine = new SchemaDefine();

			schemaDefine.Name = DictionaryHelper.GetValue(dictionary, "name", string.Empty);
			schemaDefine.SnapshotTable = DictionaryHelper.GetValue(dictionary, "snapshotTable", string.Empty);
			schemaDefine.Category = DictionaryHelper.GetValue(dictionary, "category", string.Empty);
			schemaDefine.SortOrder = DictionaryHelper.GetValue(dictionary, "sortOrder", 0xFFFF);

			if (dictionary.ContainsKey("properties"))
			{
				SchemaPropertyDefineCollection properties = JSONSerializerExecute.Deserialize<SchemaPropertyDefineCollection>(dictionary["properties"]);
				schemaDefine.Properties.Clear();
				schemaDefine.Properties.CopyFrom(properties);
			}

			if (dictionary.ContainsKey("tabs"))
			{
				SchemaTabDefineColleciton tabs = JSONSerializerExecute.Deserialize<SchemaTabDefineColleciton>(dictionary["Tabs"]);
				schemaDefine.Tabs.Clear();
				schemaDefine.Tabs.CopyFrom(tabs);
			}

			return schemaDefine;
		}
		/// <summary>
		/// 生成名称/值对的字典。
		/// </summary>
		/// <param name="obj">要序列化的对象。</param>
		/// <param name="serializer">负责序列化的对象</param>
		/// <returns>一个对象，包含表示该对象数据的键/值对。</returns>
		public override IDictionary<string, object> Serialize(object obj, JavaScriptSerializer serializer)
		{
			SchemaDefine schemaDefine = (SchemaDefine)obj;

			IDictionary<string, object> dictionary = new Dictionary<string, object>();

			DictionaryHelper.AddNonDefaultValue<string, object>(dictionary, "name", schemaDefine.Name);
			DictionaryHelper.AddNonDefaultValue<string, object>(dictionary, "snapshotTable", schemaDefine.SnapshotTable);
			DictionaryHelper.AddNonDefaultValue<string, object>(dictionary, "category", schemaDefine.Category);
			DictionaryHelper.AddNonDefaultValue<string, object>(dictionary, "sortOrder", schemaDefine.SortOrder);

			DictionaryHelper.AddNonDefaultValue<string, object>(dictionary, "properties", schemaDefine.Properties);
			DictionaryHelper.AddNonDefaultValue<string, object>(dictionary, "tabs", schemaDefine.Tabs);

			return dictionary;
		}
		/// <summary>
		/// 获取受支持类型的集合。
		/// </summary>
		/// <returns>一个实现 <see cref="T:System.Collections.Generic.IEnumerable`1" /> 的对象，用于表示转换器支持的类型。</returns>
		public override IEnumerable<Type> SupportedTypes
		{
			get { return new System.Type[] { typeof(SchemaDefine) }; }
		}
	}
}
