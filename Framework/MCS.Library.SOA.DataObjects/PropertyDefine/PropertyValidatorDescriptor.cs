using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MCS.Library.Core;
using MCS.Library.Data.DataObjects;
using MCS.Library.Validation;
using System.Collections.Specialized;
using System.Xml.Linq;

namespace MCS.Library.SOA.DataObjects
{
	/// <summary>
	/// 属性校验器的描述信息
	/// </summary>
	[Serializable]
	[XElementSerializable]
	public class PropertyValidatorDescriptor : IXElementSerializable
	{
		public PropertyValidatorDescriptor()
		{
		}

		/// <summary>
		/// 从配置信息来构造
		/// </summary>
		/// <param name="validatorElement"></param>
		public PropertyValidatorDescriptor(PropertyDefineValidatorConfigurationElement validatorElement)
		{
			this.Name = validatorElement.Name;
			this.MessageTemplate = validatorElement.MessageTemplate;
			this.Tag = validatorElement.Tag;
			this.TypeDescription = validatorElement.Type;

			this.Parameters.LoadParametersFromConfiguration(validatorElement.Parameters);
		}

		[XElementFieldSerialize(AlternateFieldName = "_N")]
		public string Name
		{
			get;
			set;
		}

		/// <summary>
		/// Validator的类型描述
		/// </summary>
		[XElementFieldSerialize(AlternateFieldName = "_TD")]
		public string TypeDescription
		{
			get;
			set;
		}

		/// <summary>
		/// 提示信息的模板
		/// </summary>
		[XElementFieldSerialize(AlternateFieldName = "_MT")]
		public string MessageTemplate
		{
			get;
			set;
		}

		[XElementFieldSerialize(AlternateFieldName = "_Tag")]
		public string Tag
		{
			get;
			set;
		}

		private PropertyValidatorParameterDescriptorCollection _Parameters = null;

		/// <summary>
		/// Validator的构造参数
		/// </summary>
		public PropertyValidatorParameterDescriptorCollection Parameters
		{
			get
			{
				if (this._Parameters == null)
					this._Parameters = new PropertyValidatorParameterDescriptorCollection();

				return this._Parameters;
			}
		}

		[NonSerialized]
		private Validator _Validator = null;

		public void ResetValidator()
		{
			this._Validator = null;
		}

		public Validator GetValidator()
		{
			if (this._Validator == null)
			{
				NameValueCollection parameters = this.Parameters.ToNameValueCollection();

				parameters["messageTemplate"] = this.MessageTemplate;
				parameters["tag"] = this.Tag;

				this._Validator = (Validator)TypeCreator.CreateInstance(this.TypeDescription, parameters);
			}

			return this._Validator;
		}

		public void Deserialize(System.Xml.Linq.XElement node, XmlDeserializeContext context)
		{
			this.Name = node.Attribute("_N", this.Name);
			this.MessageTemplate = node.Attribute("_MT", this.MessageTemplate);
			this.Tag = node.Attribute("_tag", this.Tag);
			this.TypeDescription = node.Attribute("_TD", this.TypeDescription);

			if (node.HasElements)
			{
				this._Parameters = new PropertyValidatorParameterDescriptorCollection();
				this._Parameters.Deserialize(node, context);
			}
		}

		public void Serialize(System.Xml.Linq.XElement node, XmlSerializeContext context)
		{
			if (this.Name.IsNotEmpty())
				node.SetAttributeValue("_N", this.Name);

			if (this.MessageTemplate.IsNotEmpty())
				node.SetAttributeValue("_MT", this.MessageTemplate);

			if (this.Tag.IsNotEmpty())
				node.SetAttributeValue("_tag", this.Tag);

			if (this.TypeDescription.IsNotEmpty())
				node.SetAttributeValue("_TD", this.TypeDescription);

			if (this._Parameters != null)
				this._Parameters.Serialize(node, context);
		}
	}

	[Serializable]
	[XElementSerializable]
	public class PropertyValidatorDescriptorCollection : EditableDataObjectCollectionBase<PropertyValidatorDescriptor>, IXElementSerializable
	{
		public PropertyValidatorDescriptorCollection()
		{
		}

		public PropertyValidatorDescriptorCollection(PropertyDefineValidatorConfigurationElementCollection validatorsElements)
		{
			LoadValidatorsFromConfiguration(validatorsElements);
		}

		public void LoadValidatorsFromConfiguration(PropertyDefineValidatorConfigurationElementCollection validatorsElements)
		{
			this.Clear();

			if (validatorsElements != null)
			{
				foreach (PropertyDefineValidatorConfigurationElement paramElement in validatorsElements)
				{
					this.Add(new PropertyValidatorDescriptor(paramElement));
				}
			}
		}

		public void CopyFrom(PropertyValidatorDescriptorCollection list)
		{
			ExceptionHelper.FalseThrow<ArgumentNullException>(list != null, "list");

			this.Clear();

			foreach (PropertyValidatorDescriptor item in list)
			{
				this.Add(item);
			}
		}

		public void ResetValidators()
		{
			this.ForEach(pvd => pvd.ResetValidator());
		}

		public void Deserialize(System.Xml.Linq.XElement node, XmlDeserializeContext context)
		{
			IEnumerable<XElement> nodes = node.Descendants("pvDesp");

			foreach (XElement item in nodes)
			{
				PropertyValidatorDescriptor pvDesp = new PropertyValidatorDescriptor();
				pvDesp.Deserialize(item, context);

				this.Add(pvDesp);
			}
		}

		public void Serialize(System.Xml.Linq.XElement node, XmlSerializeContext context)
		{
			foreach (PropertyValidatorDescriptor pvpd in this)
			{
				XElement itemNode = node.AddChildElement("pvDesp");
				pvpd.Serialize(itemNode, context);
			}
		}
	}
}
