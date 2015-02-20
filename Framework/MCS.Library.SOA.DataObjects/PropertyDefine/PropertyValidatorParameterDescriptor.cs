using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections.Specialized;
using MCS.Library.Core;
using MCS.Library.Data.DataObjects;
using System.Xml.Linq;

namespace MCS.Library.SOA.DataObjects
{
	/// <summary>
	/// 校验器相关的参数定义
	/// </summary>
	[Serializable]
	[XElementSerializable]
	public class PropertyValidatorParameterDescriptor : IXElementSerializable
	{
		public PropertyValidatorParameterDescriptor()
		{
		}

		public PropertyValidatorParameterDescriptor(PropertyDefineValidatorParameterConfigurationElement configElement)
		{
			this.ParamName = configElement.Name;
			this.DataType = configElement.DataType;
			this.ParamValue = configElement.ParamValue;
		}

		public PropertyValidatorParameterDescriptor(string paramName, string paramValue)
		{
			this.ParamName = paramName;
			this.ParamValue = paramValue;
		}

		public PropertyValidatorParameterDescriptor(string paramName, PropertyDataType dataType, string paramValue)
		{
			this.ParamName = paramName;
			this.DataType = dataType;
			this.ParamValue = paramValue;
		}

		[XElementFieldSerialize(AlternateFieldName = "_PN")]
		public string ParamName
		{
			get;
			set;
		}

		[XElementFieldSerialize(AlternateFieldName = "_PV")]
		public string ParamValue
		{
			get;
			set;
		}

		private PropertyDataType _DataType = PropertyDataType.String;

		public PropertyDataType DataType
		{
			get
			{
				return this._DataType;
			}
			set
			{
				this._DataType = value;
			}
		}

		public void Deserialize(System.Xml.Linq.XElement node, XmlDeserializeContext context)
		{
			this.ParamName = node.Attribute("_parName", this.ParamName);
			this.DataType = node.Attribute("_dType", PropertyDataType.String);
			this.ParamValue = node.Attribute("_parV", this.ParamValue);

		}

		public void Serialize(System.Xml.Linq.XElement node, XmlSerializeContext context)
		{
			if (this.ParamName.IsNotEmpty())
				node.SetAttributeValue("_parName", this.ParamName);

			if (this.DataType != PropertyDataType.String)
				node.SetAttributeValue("_dType", this.DataType);

			if (this.ParamValue.IsNotEmpty())
				node.SetAttributeValue("_parV", this.ParamValue);
		}
	}

	[Serializable]
	[XElementSerializable]
	public class PropertyValidatorParameterDescriptorCollection : EditableKeyedDataObjectCollectionBase<string, PropertyValidatorParameterDescriptor>, IXElementSerializable
	{
		public PropertyValidatorParameterDescriptorCollection()
		{
		}

		public PropertyValidatorParameterDescriptorCollection(PropertyDefineValidatorParameterConfigurationElementCollection paramsElements)
		{
			LoadParametersFromConfiguration(paramsElements);
		}

		public void LoadParametersFromConfiguration(PropertyDefineValidatorParameterConfigurationElementCollection paramsElements)
		{
			this.Clear();

			if (paramsElements != null)
			{
				foreach (PropertyDefineValidatorParameterConfigurationElement paramElement in paramsElements)
				{
					this.Add(new PropertyValidatorParameterDescriptor(paramElement));
				}
			}
		}

		public PropertyValidatorParameterDescriptor Add(string paramName, string paramValue)
		{
			PropertyValidatorParameterDescriptor result = new PropertyValidatorParameterDescriptor(paramName, paramValue);

			this.Add(result);

			return result;
		}

		public PropertyValidatorParameterDescriptor Add(string paramName, PropertyDataType dataType, string paramValue)
		{
			PropertyValidatorParameterDescriptor result = new PropertyValidatorParameterDescriptor(paramName, dataType, paramValue);

			this.Add(result);

			return result;
		}

		public NameValueCollection ToNameValueCollection()
		{
			NameValueCollection result = new NameValueCollection(StringComparer.OrdinalIgnoreCase);

			foreach (PropertyValidatorParameterDescriptor pvpd in this)
			{
				result[pvpd.ParamName] = pvpd.ParamValue;
			}

			return result;
		}

		public List<object> GetRealTypeValues()
		{
			List<object> values = new List<object>();

			foreach (PropertyValidatorParameterDescriptor pvpd in this)
			{
				Type realType = typeof(string);

				if (pvpd.DataType.TryToRealType(out realType))
					values.Add(DataConverter.ChangeType(pvpd.ParamValue, realType));
			}

			return values;
		}

		protected override string GetKeyForItem(PropertyValidatorParameterDescriptor item)
		{
			return item.ParamName;
		}

		public void Deserialize(System.Xml.Linq.XElement node, XmlDeserializeContext context)
		{
			IEnumerable<XElement> nodes = node.Descendants("pvpd");

			foreach (XElement item in nodes)
			{
				PropertyValidatorParameterDescriptor pvpd = new PropertyValidatorParameterDescriptor();
				pvpd.Deserialize(item, context);

				this.Add(pvpd);
			}
		}

		public void Serialize(System.Xml.Linq.XElement node, XmlSerializeContext context)
		{
			foreach (PropertyValidatorParameterDescriptor pvpd in this)
			{
				XElement itemNode = node.AddChildElement("pvpd");
				pvpd.Serialize(itemNode, context);
			}
		}
	}
}
