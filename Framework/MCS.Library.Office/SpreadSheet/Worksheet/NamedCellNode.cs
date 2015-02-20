using System;
using System.Collections.Generic;
using System.Text;
using System.Xml;
using MCS.Library.Core;

namespace MCS.Library.Office.SpreadSheet
{
	public class NamedCellNode : NodeBase
	{
		public string Name { get; set; }

		public override XmlNode AppendXmlNode(XmlNode parent)
		{
			XmlNode node = XmlHelper.AppendNodeWithNamespace(parent, "NamedCell", Namespaces.spreadsheet);

			XmlHelper.AppendAttrWithNamespace(node, "Name", Namespaces.ss, Name);

			return node;
		}

		public override void FromXmlNode(XmlNode node)
		{
			this.Name = XmlHelper.GetAttributeText(node, "Name", Namespaces.ss);
		}
	}
}
