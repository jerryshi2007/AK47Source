using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MCS.Library.Data.Mapping;
using MCS.Library.SOA.DataObjects.Schemas.SchemaProperties;
using System.Xml.Linq;
using System.Globalization;
using System.Collections;
using MCS.Library.Data.DataObjects;

namespace MCS.Library.SOA.DataObjects.Security.AUObjects
{
	[ORTableMapping("SC.SchemaPropertyExtensions")]
	public class SchemaPropertyExtension
	{
		private string targetSchemaType;
		private string sourceID;
		private string desc;
		private SchemaPropertyDefineCollection def;

		public SchemaPropertyExtension(string targetSchemaType, string sourceID, string description)
		{
			this.targetSchemaType = targetSchemaType;
			this.sourceID = sourceID;
			this.desc = description;
		}

		[ORFieldMapping("TargetSchemaType", PrimaryKey = true)]
		public string TargetSchemaType
		{
			get { return targetSchemaType; }
		}

		[ORFieldMapping("SourceID", PrimaryKey = true)]
		public string SourceID
		{
			get { return sourceID; }
		}

		/// <summary>
		/// 仅ORMapping使用
		/// </summary>
		[ORFieldMapping("Definition")]
		public string InternalDefinitionXml
		{
			get
			{
				if (this.def != null)
				{
					return ToXml(def);
				}

				return null;
			}

			set
			{
				this.def = FromXml(value);
			}
		}

		private SchemaPropertyDefineCollection FromXml(string value)
		{
			//System.IO.StringWriter sw = new System.IO.StringWriter();
			//new System.Xml.Serialization.XmlSerializer(typeof(SchemaPropertyDefineCollection)).Deserialize(sw, def);
			//return sw.ToString();
			XElement root = XDocument.Parse(value).Element("properties");
			if (root == null)
				throw new FormatException("未能找到元素的根元素properties");

			SchemaPropertyDefineCollection collection = new SchemaPropertyDefineCollection();

			foreach (XElement item in root.Elements("property"))
			{
				collection.Add(ToProperty(item));
			}

			return collection;

		}

		private SchemaPropertyDefine ToProperty(XElement item)
		{
			SchemaPropertyDefine define = new SchemaPropertyDefine();
			XAttribute attr;
			XElement elem;
			if ((attr = item.Attribute("name")) != null)
				define.Name = attr.Value;
			if ((attr = item.Attribute("allowOverride")) != null)
				define.AllowOverride = bool.Parse(attr.Value);
			if ((attr = item.Attribute("category")) != null)
				define.Category = attr.Value;
			if ((attr = item.Attribute("dataType")) != null)
				define.DataType = (PropertyDataType)Enum.Parse(typeof(PropertyDataType), attr.Value);
			if ((attr = item.Attribute("defaultValue")) != null)
				define.DefaultValue = attr.Value;
			if ((attr = item.Attribute("description")) != null)
				define.Description = attr.Value;
			if ((attr = item.Attribute("displayName")) != null)
				define.DisplayName = attr.Value;
			if ((attr = item.Attribute("editorKey")) != null)
				define.EditorKey = attr.Value;
			if ((attr = item.Attribute("editorParams")) != null)
				define.EditorParams = attr.Value;
			if ((attr = item.Attribute("editorParamsSettingsKey")) != null)
				define.EditorParamsSettingsKey = attr.Value;
			if ((attr = item.Attribute("isRequired")) != null)
				define.IsRequired = bool.Parse(attr.Value);
			if ((attr = item.Attribute("maxLength")) != null)
				define.MaxLength = int.Parse(attr.Value, CultureInfo.InvariantCulture);
			if ((attr = item.Attribute("persisterKey")) != null)
				define.PersisterKey = attr.Value;
			if ((attr = item.Attribute("readOnly")) != null)
				define.ReadOnly = bool.Parse(attr.Value);
			if ((attr = item.Attribute("showTitle")) != null)
				define.ShowTitle = bool.Parse(attr.Value);
			if ((attr = item.Attribute("tab")) != null)
				define.Tab = attr.Value;
			if ((attr = item.Attribute("visible")) != null)
				define.Visible = bool.Parse(attr.Value);
			else
				define.Visible = true;

			if ((elem = item.Element("validators")) != null)
			{
				ReadValidators(define.Validators, elem);
			}

			return define;
		}

		private void ReadValidators(PropertyValidatorDescriptorCollection collection, XElement container)
		{
			foreach (var item in container.Elements("validator"))
			{
				collection.Add(ValidatorFromNode(item));
			}
		}

		private PropertyValidatorDescriptor ValidatorFromNode(XElement item)
		{
			PropertyValidatorDescriptor descriptor = new PropertyValidatorDescriptor();
			XElement elem;
			XAttribute attr;
			if ((attr = item.Attribute("messageTemplate")) != null)
				descriptor.MessageTemplate = attr.Value;
			if ((attr = item.Attribute("name")) != null)
				descriptor.Name = attr.Value;
			if ((attr = item.Attribute("tag")) != null)
				descriptor.Tag = attr.Value;
			if ((attr = item.Attribute("typeDescription")) != null)
				descriptor.TypeDescription = attr.Value;
			if ((elem = item.Element("parameters")) != null)
			{
				AddParameters(elem, descriptor.Parameters);
			}

			return descriptor;
		}

		private void AddParameters(XElement elem, PropertyValidatorParameterDescriptorCollection target)
		{
			foreach (var item in elem.Elements("parameter"))
			{
				target.Add(new PropertyValidatorParameterDescriptor(item.Attribute("paraName").Value, (PropertyDataType)Enum.Parse(typeof(PropertyDataType), item.Attribute("dataType").Value), item.Attribute("paraValue").Value));
			}
		}

		private string ToXml(SchemaPropertyDefineCollection def)
		{
			//System.IO.StringWriter sw = new System.IO.StringWriter();
			//new System.Xml.Serialization.XmlSerializer(typeof(SchemaPropertyDefineCollection)).Serialize(sw, def);
			//return sw.ToString();
			var xDoc = new XDocument();
			var xRoot = new XElement("properties");
			xDoc.Add(xRoot);
			foreach (SchemaPropertyDefine item in def)
			{
				xRoot.Add(ToElement(item));
			}

			return xDoc.ToString(SaveOptions.OmitDuplicateNamespaces);
		}

		private XElement ToElement(SchemaPropertyDefine item)
		{
			XElement xElem = new XElement("property");
			xElem.Add(new XAttribute("name", item.Name));
			xElem.Add(new XAttribute("allowOverride", item.AllowOverride));
			if (string.IsNullOrEmpty(item.Category) == false)
				xElem.Add(new XAttribute("category", item.Category));
			xElem.Add(new XAttribute("dataType", item.DataType));
			if (item.DefaultValue != null)
				xElem.Add(new XAttribute("defaultValue", item.DefaultValue));
			if (string.IsNullOrEmpty(item.Description) == false)
				xElem.Add(new XAttribute("description", item.Description));
			if (string.IsNullOrEmpty(item.DisplayName) == false)
				xElem.Add(new XAttribute("displayName", item.DisplayName));
			if (string.IsNullOrEmpty(item.EditorKey) == false)
				xElem.Add(new XAttribute("editorKey", item.EditorKey));
			if (string.IsNullOrEmpty(item.EditorParams) == false)
				xElem.Add(new XAttribute("editorParams", item.EditorParams));
			if (string.IsNullOrEmpty(item.EditorParamsSettingsKey) == false)
				xElem.Add(new XAttribute("editorParamsSettingsKey", item.EditorParamsSettingsKey));
			xElem.Add(new XAttribute("isRequired", item.IsRequired));
			if (item.MaxLength > 0)
				xElem.Add(new XAttribute("maxLength", item.MaxLength));
			if (string.IsNullOrEmpty(item.PersisterKey) == false)
				xElem.Add(new XAttribute("persisterKey", item.PersisterKey));
			if (item.ReadOnly)
				xElem.Add(new XAttribute("readOnly", "true"));
			xElem.Add(new XAttribute("showTitle", item.ShowTitle));
			if (string.IsNullOrEmpty(item.Tab) == false)
				xElem.Add(new XAttribute("tab", item.Tab));
			if (item.Visible == false)
				xElem.Add(new XAttribute("visible", item.Visible));
			if (item.Validators != null && item.Validators.Count > 0)
			{
				var xValidators = new XElement("validators");
				xElem.Add(xValidators);
				foreach (var v in item.Validators)
				{
					ValidatorToNode(v, xValidators);
				}
			}

			return xElem;
		}

		private void ValidatorToNode(PropertyValidatorDescriptor descriptor, XElement containerElement)
		{
			XElement elem = new XElement("validator");
			elem.Add(new XAttribute("messageTemplate", descriptor.MessageTemplate));
			elem.Add(new XAttribute("name", descriptor.Name));
			elem.Add(new XAttribute("tag", descriptor.Tag));
			elem.Add(new XAttribute("typeDescription", descriptor.TypeDescription));
			if (descriptor.Parameters != null && descriptor.Parameters.Count > 0)
			{
				var xParams = new XElement("parameters");
				elem.Add(xParams);
				foreach (var para in descriptor.Parameters)
				{
					xParams.Add(new XElement("parameter", new XAttribute("dataType", para.DataType), new XAttribute("paraName", para.ParamName), new XAttribute("paraValue", para.ParamValue)));
				}
			}
		}

		[NoMapping]
		public SchemaPropertyDefineCollection Properties
		{
			get
			{
				if (def == null)
					def = new SchemaPropertyDefineCollection();
				return def;
			}
		}

		public string Description
		{
			get { return desc; }
			set { desc = value; }
		}

		public override int GetHashCode()
		{
			return this.sourceID.GetHashCode() << 5 | this.targetSchemaType.GetHashCode();
		}
	}

	public class SchemaPropertyExtensionCollection : SerializableEditableKeyedDataObjectCollectionBase<SchemaPropertyExtensionKey, SchemaPropertyExtension>
	{
		public SchemaPropertyExtension this[string sourceID, string targetSchemaType]
		{
			get { return this[new SchemaPropertyExtensionKey(sourceID, targetSchemaType)]; }
		}

		protected override SchemaPropertyExtensionKey GetKeyForItem(SchemaPropertyExtension item)
		{
			return new SchemaPropertyExtensionKey(item.SourceID, item.TargetSchemaType);
		}
	}

	public struct SchemaPropertyExtensionKey
	{
		private string sourceID;
		private string targetSchemaType;

		public string SourceID
		{
			get { return sourceID; }
		}

		public string TargetSchemaType
		{
			get { return targetSchemaType; }
		}

		public SchemaPropertyExtensionKey(string sourceID, string targetSchemaType)
		{
			if (string.IsNullOrEmpty(sourceID)) throw new ArgumentNullException("sourceID");
			if (string.IsNullOrEmpty(targetSchemaType)) throw new ArgumentNullException("targetSchemaType");

			this.sourceID = sourceID;
			this.targetSchemaType = targetSchemaType;
		}

		public override int GetHashCode()
		{
			return this.sourceID.GetHashCode() << 5 | this.targetSchemaType.GetHashCode();
		}

		public override bool Equals(object obj)
		{
			bool eq = obj is SchemaPropertyExtensionKey;
			if (eq)
			{
				SchemaPropertyExtensionKey key = (SchemaPropertyExtensionKey)obj;
				eq = key.sourceID == this.sourceID && key.targetSchemaType == this.targetSchemaType;
			}

			return eq;
		}
	}
}
