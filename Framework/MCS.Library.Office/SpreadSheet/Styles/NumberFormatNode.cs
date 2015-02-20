using System;
using System.Collections.Generic;
using System.Text;
using System.Xml;
using MCS.Library.Core;

namespace MCS.Library.Office.SpreadSheet
{
	public class NumberFormatNode : NodeBase
	{
		private string _Format = "General";

		public string Format
		{
			get
			{
				return _Format;
			}
			set
			{
				_Format = value;
			}
		}

		public override XmlNode AppendXmlNode(XmlNode parent)
		{
			XmlNode node = XmlHelper.AppendNodeWithNamespace(parent, "NumberFormat", Namespaces.ss);

			if (Format != "General")
				XmlHelper.AppendNotNullAttr(node, "Format", Format);

			return node;
		}

		public override void FromXmlNode(XmlNode node)
		{
			this.Format = XmlHelper.GetAttributeText(node, "Format");
		}
	}
}
