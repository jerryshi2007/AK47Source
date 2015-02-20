using System;
using System.Collections.Generic;
using System.Text;
using System.Xml;
using MCS.Library.Core;

namespace MCS.Library.Office.SpreadSheet
{
	public class PhoneticTextNode : NodeBase
	{
		public bool Visible { get; set; }

		public override XmlNode AppendXmlNode(XmlNode parent)
		{
			XmlNode node = XmlHelper.AppendNodeWithNamespace(parent, "PhoneticText", Namespaces.x);

			XmlHelper.AppendNotDefaultAttr(node, "Visible", DataConverter.ChangeType<bool, int>(Visible));

			return node;
		}

		public override void FromXmlNode(XmlNode node)
		{
			this.Visible = XmlHelper.GetAttributeValue(node, "Visible", false);
		}
	}
}
