using System;
using System.Collections.Generic;
using System.Text;
using System.Xml;
using MCS.Library.Core;

namespace MCS.Library.Office.SpreadSheet
{
	public abstract class RowAndColumnBase : NodeBase
	{
		public int Index { get; set; }
		public string Caption { get; set; }
		public bool Hidden { get; set; }
		public int Span { get; set; }
		public string StyleID { get; set; }

		protected abstract XmlNode CreateRootNode(XmlNode parent);

		public override XmlNode AppendXmlNode(XmlNode parent)
		{
			XmlNode node = CreateRootNode(parent);

			XmlHelper.AppendNotNullAttrWithNamespace(node, "Caption", Namespaces.c, Caption);
			XmlHelper.AppendNotDefaultAttrWithNamespace(node, "Index", Namespaces.spreadsheet, Index);
			XmlHelper.AppendNotDefaultAttr(node, "Hidden", DataConverter.ChangeType<bool, int>(Hidden));
			XmlHelper.AppendNotDefaultAttr(node, "Span", Span);
			XmlHelper.AppendNotNullAttr(node, "StyleID", StyleID);

			return node;
		}

		public override void FromXmlNode(XmlNode node)
		{
			this.Caption = XmlHelper.GetAttributeText(node, "Caption", Namespaces.c);
			this.Index = XmlHelper.GetAttributeValue(node, "Index", Namespaces.spreadsheet, 0);
			this.Hidden = XmlHelper.GetAttributeValue(node, "Hidden", false);
			this.Span = XmlHelper.GetAttributeValue(node, "Span", 0);
			this.StyleID = XmlHelper.GetAttributeText(node, "StyleID");
		}
	}
}
