using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MCS.Library.Core;
using System.Xml.Linq;

namespace MCS.Library.SOA.DataObjects
{
	[Serializable]
	[XElementSerializable]
	public class PropertyLayoutSectionDefine : IXElementSerializable
	{
		[XElementFieldSerialize(AlternateFieldName = "_Na")]
		public string Name { get; set; }

		[XElementFieldSerialize(AlternateFieldName = "_DisplayName")]
		public string DisplayName { get; set; }

		[XElementFieldSerialize(AlternateFieldName = "_Columns")]
		public int Columns { get; set; }

		[XElementFieldSerialize(AlternateFieldName = "_DefaultRowHeight")]
		public string DefaultRowHeight { get; set; }

		public PropertyLayoutSectionDefine(PropertyFormSectionConfigurationElement formSectionElem)
		{
			this.Name = formSectionElem.Name;
			this.DisplayName = formSectionElem.DisplayName;
			this.Columns = formSectionElem.Columns;
			this.DefaultRowHeight = formSectionElem.DefaultRowHeight;
		}

		public PropertyLayoutSectionDefine()
		{ }

		public void Serialize(XElement node, XmlSerializeContext context)
		{
			if (this.Name.IsNotEmpty())
				node.SetAttributeValue("_na", this.Name);

			if (this.DisplayName.IsNotEmpty())
				node.SetAttributeValue("_dn", this.DisplayName);

			if (this.Columns > 0)
				node.SetAttributeValue("_columns", this.Columns);

			if (this.DefaultRowHeight.IsNotEmpty())
				node.SetAttributeValue("_drh", this.DefaultRowHeight);
		}

		public void Deserialize(XElement node, XmlDeserializeContext context)
		{
			this.Name = node.Attribute("_na", this.Name);
			this.DisplayName = node.Attribute("_dn", this.DisplayName);
			this.Columns = node.Attribute("_columns", this.Columns);
			this.DefaultRowHeight = node.Attribute("_drh", this.DefaultRowHeight);
		}
	}
}
