using System;
using System.Collections.Generic;
using MCS.Library.Core;
using MCS.Library.Data.DataObjects;
using MCS.Library.Validation;
using System.Runtime.Serialization;
using System.Collections;

namespace MCS.Library.SOA.DataObjects.Schemas.SchemaProperties
{
	/// <summary>
	/// 表示<see cref="SchemaPropertyValue"/>的集合
	/// </summary>
	[Serializable]
	[XElementSerializable]
	public class SchemaPropertyValueCollection : SerializableEditableKeyedDataObjectCollectionBase<string, SchemaPropertyValue>
	{
		/// <summary>
		/// 初始化<see cref="SchemaPropertyValueCollection"/>的新实例
		/// </summary>
		public SchemaPropertyValueCollection()
		{
		}

		/// <summary>
		/// 使用指定的<see cref="SerializationInfo"/>和<see cref="StreamingContext"/>初始化<see cref="SchemaPropertyValueCollection"/>的新实例
		/// </summary>
		/// <param name="info">存储将对象序列化或反序列化所需的全部数据</param>
		/// <param name="context">序列化描述的上下文</param>
		protected SchemaPropertyValueCollection(SerializationInfo info, StreamingContext context) :
			base(info, context)
		{
		}

		///// <summary>
		///// 使用指定的<see cref="SchemaDefine"/>初始化<see cref="SchemaPropertyValueCollection"/>的新实例
		///// </summary>
		///// <param name="schema">表示模式定义的<see cref="SchemaDefine"/></param>
		//public SchemaPropertyValueCollection(SchemaDefine schema)
		//{
		//    if (schema != null)
		//        schema.Properties.ForEach(pd => this.Add(new SchemaPropertyValue(pd)));
		//}

		/// <summary>
		/// 获取集合中指定的<see cref="SchemaTabDefine"/>的键。
		/// </summary>
		/// <param name="item">获取其键的<see cref="SchemaPropertyValue"/></param>
		/// <returns>表示键的字符串</returns>
		protected override string GetKeyForItem(SchemaPropertyValue item)
		{
			return item.Definition.Name;
		}

		/// <summary>
		/// 获取集合中指定键所对应的值
		/// </summary>
		/// <typeparam name="T">默认值的类型</typeparam>
		/// <param name="name">属性的名称</param>
		/// <param name="defaultValue">缺省值</param>
		/// <returns> 属性的值 或 缺省值</returns>
		public T GetValue<T>(string name, T defaultValue)
		{
			name.CheckStringIsNullOrEmpty("key");

			T result = defaultValue;

			SchemaPropertyValue v = this[name];

			if (v != null)
			{
				if (result != null)
					result = (T)DataConverter.ChangeType(v.StringValue, result.GetType());
				else
				{
					Type targetType = null;
					if (v.Definition.DataType == PropertyDataType.Enum)
						targetType = Type.GetType(v.Definition.EditorParams);
					else
						targetType = v.Definition.DataType.ToRealType();

					object realValue = DataConverter.ChangeType(v.StringValue, targetType);

					result = (T)DataConverter.ChangeType(realValue, typeof(T));
				}
			}

			return result;
		}

		/// <summary>
		/// 设置属性的值。如果该属性不存在，则抛出异常
		/// </summary>
		/// <typeparam name="T"></typeparam>
		/// <param name="name"></param>
		/// <param name="data"></param>
		public void SetValue<T>(string name, T data)
		{
			TrySetValue(name, data).FalseThrow("不能找到名称为{0}的属性", name);
		}

		/// <summary>
		/// 尝试去设置属性。如果该属性不存在，则返回false，否则返回true
		/// </summary>
		/// <typeparam name="T"></typeparam>
		/// <param name="name"></param>
		/// <param name="data"></param>
		/// <returns></returns>
		public bool TrySetValue<T>(string name, T data)
		{
			name.CheckStringIsNullOrEmpty("key");

			SchemaPropertyValue v = this[name];

			bool result = (v != null);

			if (result)
			{
				if (data != null)
					v.StringValue = data.ToString();
				else
					v.StringValue = string.Empty;
			}

			return result;
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="propDefinitions"></param>
		public void InitFromPropertyDefineCollection(SchemaPropertyDefineCollection propDefinitions)
		{
			this.Clear();

			AppendFromPropertyDefineCollection(propDefinitions);
		}

		public void AppendFromPropertyDefineCollection(SchemaPropertyDefineCollection propDefinitions)
		{
			foreach (SchemaPropertyDefine propDef in propDefinitions)
			{
				SchemaPropertyValue pv = new SchemaPropertyValue(propDef);

				this.Add(pv);
			}
		}

		/// <summary>
		/// 当前的属性值集合与新的属性定义进行合并
		/// </summary>
		/// <param name="definedProperties"></param>
		public void MergeDefinedProperties(PropertyDefineCollection definedProperties)
		{
			foreach (SchemaPropertyDefine pd in definedProperties)
			{
				SchemaPropertyValue pv = this[pd.Name];

				if (pv == null)
				{
					this.Add(new SchemaPropertyValue(pd));
				}
				else
				{
					if (pv.Definition.AllowOverride)
					{
						pv.Definition = pd;
					}
				}
			}
		}

		/// <summary>
		/// 从PropertyValue的集合导入数据。仅仅导入StringValue，没有定义部分
		/// </summary>
		/// <param name="values"></param>
		public void FromPropertyValues(IEnumerable<PropertyValue> values)
		{
			if (values != null)
			{
				foreach (PropertyValue pv in values)
				{
					SchemaPropertyValue spv = this[pv.Definition.Name];

					if (spv != null)
						spv.FromPropertyVaue(pv);
				}
			}
		}

		public PropertyValueCollection ToPropertyValues()
		{
			PropertyValueCollection result = new PropertyValueCollection();

			this.ForEach(spv => result.Add((PropertyValue)spv));

			return result;
		}

		public void Write()
		{
			Dictionary<string, IPropertyPersister<SchemaPropertyValue>> dicPers = PropertiesPersisterHelper<SchemaPropertyValue>.GetAllPropertiesPersisters();

			if (dicPers.Count > 0)
			{
				using (PersisterContext<SchemaPropertyValue> context = PersisterContext<SchemaPropertyValue>.CreatePersisterContext(this, null))
				{
					foreach (SchemaPropertyValue item in this)
					{
						if (item.Definition.PersisterKey.IsNotEmpty())
						{
							if (dicPers.ContainsKey(item.Definition.PersisterKey))
								dicPers[item.Definition.PersisterKey].Write(item, context);
						}
					}
				}
			}
		}

		public void Read()
		{
			Dictionary<string, IPropertyPersister<SchemaPropertyValue>> dicPers = PropertiesPersisterHelper<SchemaPropertyValue>.GetAllPropertiesPersisters();

			if (dicPers.Count > 0)
			{
				using (PersisterContext<SchemaPropertyValue> context = PersisterContext<SchemaPropertyValue>.CreatePersisterContext(this, null))
				{
					foreach (SchemaPropertyValue item in this)
					{
						if (item.Definition.PersisterKey.IsNotEmpty())
						{
							if (dicPers.ContainsKey(item.Definition.PersisterKey))
								dicPers[item.Definition.PersisterKey].Read(item, context);
						}
					}
				}
			}
		}

		/// <summary>
		/// 按照tab属性来给属性分组
		/// </summary>
		/// <returns></returns>
		public Dictionary<string, SchemaPropertyValueCollection> GroupByTab()
		{
			Dictionary<string, SchemaPropertyValueCollection> result = new Dictionary<string, SchemaPropertyValueCollection>();

			foreach (SchemaPropertyValue spv in this)
			{
				SchemaPropertyValueCollection spvc = null;

				if (result.TryGetValue(spv.Definition.Tab, out spvc) == false)
				{
					spvc = new SchemaPropertyValueCollection();

					result.Add(spv.Definition.Tab, spvc);
				}

				spvc.Add(spv);
			}

			return result;
		}

		public static implicit operator PropertyValueCollection(SchemaPropertyValueCollection spvc)
		{
			spvc.NullCheck("spvc");

			return spvc.ToPropertyValues();
		}
	}
}
