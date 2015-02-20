using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Xml.Linq;
using MCS.Library.Core;
using MCS.Library.Data.DataObjects;
using MCS.Library.Validation;

namespace MCS.Library.SOA.DataObjects
{
	[Serializable]
	[XElementSerializable]
	public class PropertyValueCollection : SerializableEditableKeyedDataObjectCollectionBase<string, PropertyValue>, IXElementSerializable, ISimpleXmlSerializer
	{
		public PropertyValueCollection()
		{
		}

		public PropertyValueCollection(SerializationInfo info, StreamingContext context) :
			base(info, context)
		{
		}

		public T GetValue<T>(string name, T defaultValue)
		{
			name.CheckStringIsNullOrEmpty("key");

			T result = defaultValue;

			PropertyValue v = this[name];

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

			//name.CheckStringIsNullOrEmpty("key");

			//T result = defaultValue;

			//PropertyValue v = this[name];

			//if (v != null)
			//{
			//    result = (T)DataConverter.ChangeType(v.StringValue, result.GetType());
			//}

			//return result;
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

			PropertyValue v = this[name];

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
		/// 根据验证信息验证properties
		/// </summary>
		/// <returns></returns>
		public ValidationResults Validate()
		{
			ValidationResults results = new ValidationResults();

			this.ForEach(pv => pv.Validate(results));

			return results;
		}

		public void Write()
		{
			Dictionary<string, IPropertyPersister<PropertyValue>> dicPers = PropertiesPersisterHelper<PropertyValue>.GetAllPropertiesPersisters();

			if (dicPers.Count == 0)
				return;

			using (PersisterContext<PropertyValue> context = PersisterContext<PropertyValue>.CreatePersisterContext(this, null))
			{
				foreach (PropertyValue item in this)
				{
					if (item.Definition.PersisterKey.IsNotEmpty())
					{
						if (dicPers.ContainsKey(item.Definition.PersisterKey))
							dicPers[item.Definition.PersisterKey].Write(item, context);
					}
				}
			}
		}

		public void Read()
		{
			Dictionary<string, IPropertyPersister<PropertyValue>> dicPers = PropertiesPersisterHelper<PropertyValue>.GetAllPropertiesPersisters();

			if (dicPers.Count == 0)
				return;

			using (PersisterContext<PropertyValue> context = PersisterContext<PropertyValue>.CreatePersisterContext(this, null))
			{
				foreach (PropertyValue item in this)
				{
					if (item.Definition.PersisterKey.IsNotEmpty())
					{
						if (dicPers.ContainsKey(item.Definition.PersisterKey))
							dicPers[item.Definition.PersisterKey].Read(item, context);
					}
				}
			}
		}

		public void InitFromPropertyDefineCollection(PropertyDefineCollection propDefinitions)
		{
			this.Clear();

			AppendFromPropertyDefineCollection(propDefinitions);
		}

		public void AppendFromPropertyDefineCollection(PropertyDefineCollection propDefinitions)
		{
			foreach (PropertyDefine propDef in propDefinitions)
			{
				PropertyValue pv = new PropertyValue(propDef);

				this.Add(pv);
			}
		}

		/// <summary>
		/// 将原来的属性替换
		/// </summary>
		/// <param name="definedProperties"></param>
		public void ReplaceDefinedProperties(PropertyDefineCollection definedProperties)
		{
			this.ResetDefinedProperties(definedProperties, true, true);
		}

		/// <summary>
		/// 当前的属性值集合与新的属性定义进行合并
		/// </summary>
		/// <param name="propDefinitions"></param>
		public void MergeDefinedProperties(PropertyDefineCollection definedProperties)
		{
			MergeDefinedProperties(definedProperties, false);
		}

		/// <summary>
		/// 当前的属性值集合与新的属性定义进行合并
		/// </summary>
		/// <param name="definedProperties"></param>
		/// <param name="forceOverride">是否强制覆盖</param>
		public void MergeDefinedProperties(PropertyDefineCollection definedProperties, bool forceOverride)
		{
			this.ResetDefinedProperties(definedProperties, forceOverride, false);
		}

		/// <summary>
		/// 将sourceProperties中的属性的值复制到当前属性中。如果不存在当前的属性，则忽略。
		/// </summary>
		/// <param name="sourceProperties"></param>
		public void ReplaceExistedPropertyValues(PropertyValueCollection sourceProperties)
		{
			sourceProperties.NullCheck("sourceProperties");

			foreach (PropertyValue pv in sourceProperties)
			{
				if (this.ContainsKey(pv.Definition.Name))
					this[pv.Definition.Name].StringValue = pv.StringValue;
			}
		}

		/// <summary>
		/// 将属性值转换为字典
		/// </summary>
		/// <returns></returns>
		public virtual Dictionary<string, object> ToDictionary()
		{
			Dictionary<string, object> result = new Dictionary<string, object>();

			foreach (PropertyValue pv in this)
			{
				if (result.ContainsKey(pv.Definition.Name) == false)
					result.Add(pv.Definition.Name, pv.GetRealValue());
			}

			return result;
		}

		/// <summary>
		/// 重新设置当前properties
		/// </summary>
		/// <param name="definedProperties"></param>
		/// <param name="forceOverride">是否强制覆盖</param>
		/// <param name="resetValue">是否重新设置StringValue</param>
		private void ResetDefinedProperties(PropertyDefineCollection definedProperties, bool forceOverride, bool resetValue)
		{
			foreach (PropertyDefine pd in definedProperties)
			{
				PropertyValue pv = this[pd.Name];

				if (pv == null)
					this.Add(new PropertyValue(pd));
				else
				{
					pv.Definition = pd;

					if (resetValue)
						pv.StringValue = pd.DefaultValue;
				}
			}
		}

		protected override string GetKeyForItem(PropertyValue item)
		{
			return item.Definition.Name;
		}

		public void Serialize(XElement node, XmlSerializeContext context)
		{
			List<int> idList = new List<int>(50);
			foreach (PropertyValue pv in this)
			{

				int objID = 0;
				var newpv = pv.Clone();
				if (newpv.Definition.DefaultValue.IsNullOrEmpty())
					newpv.Definition.DefaultValue = string.Empty;

				if (newpv.StringValue.IsNullOrEmpty())
					newpv.StringValue = string.Empty;

				if (context.ObjectContext.TryGetValue(newpv, out objID) && newpv.Definition.DefaultValue.ToLower() == newpv.StringValue.ToLower())
				{
					idList.Add(objID);
				}
				else
				{
					XElement itemNode = node.AddChildElement("I");
					newpv.Serialize(itemNode, context);
				}
			}
			if (idList.Count > 0)
				node.SetAttributeValue("_pvs", string.Join(",", idList.ToArray()));
		}

		public void Deserialize(XElement node, XmlDeserializeContext context)
		{
			this.Clear();
			var itemNodes = from vNodes in node.Descendants("I")
							select vNodes;
			if (itemNodes.FirstOrDefault() == null)
				itemNodes = from vNodes in node.Descendants("Item")
							select vNodes;
			foreach (XElement itemNode in itemNodes)
			{
				PropertyValue pv = new PropertyValue();
				pv.Deserialize(itemNode, context);

				if (pv.Definition.Name != null)
					this.Add(pv);
			}
			string idList = node.Attribute("_pvs", string.Empty);
			if (!idList.IsNullOrEmpty())
			{
				string[] idArray = idList.Split(',');
				foreach (string id in idArray)
				{
					XElement element = new XElement("Custom");
					element.SetAttributeValue("v", id);
					PropertyValue pv = new PropertyValue();
					pv.Deserialize(element, context);

					if (pv.Definition.Name != null)
						this.Add(pv);
				}
			}
		}

		#region ISimpleXmlSerializer Members

		void ISimpleXmlSerializer.ToXElement(XElement element, string refNodeName)
		{
			foreach (PropertyValue pv in this)
			{
				((ISimpleXmlSerializer)pv).ToXElement(element, refNodeName);
			}
		}

		#endregion
	}
}
