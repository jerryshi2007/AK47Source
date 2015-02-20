using System;
using System.Collections.Generic;
using System.Text;
using System.Xml;
using MCS.Library.Core;

namespace MCS.Library.Office.SpreadSheet
{
	public enum LayoutOrientation
	{
		Portrait,
		Landscape
	}

	public class LayoutNode : NodeBase
	{
		public bool CenterHorizontal { get; set; }
		public bool CenterVertical { get; set; }
		public LayoutOrientation Orientation { get; set; }
		public int StartPageNumber { get; set; }

		public override XmlNode AppendXmlNode(XmlNode parent)
		{
			XmlNode node = XmlHelper.AppendNodeWithNamespace(parent, "Layout", Namespaces.x);

			XmlHelper.AppendNotDefaultAttr(node, "CenterHorizontal", DataConverter.ChangeType<bool, int>(CenterHorizontal));
			XmlHelper.AppendNotDefaultAttr(node, "CenterVertical", DataConverter.ChangeType<bool, int>(CenterVertical));

			XmlHelper.AppendNotDefaultAttr(node, "Orientation", Orientation);
			XmlHelper.AppendNotDefaultAttr(node, "StartPageNumber", StartPageNumber);

			return node;
		}

		public override void FromXmlNode(XmlNode node)
		{
			this.CenterHorizontal = XmlHelper.GetAttributeValue(node, "CenterHorizontal", false);
			this.CenterVertical = XmlHelper.GetAttributeValue(node, "CenterVertical", false);
			this.Orientation = XmlHelper.GetAttributeValue(node, "Orientation", LayoutOrientation.Portrait);
			this.StartPageNumber = XmlHelper.GetAttributeValue(node, "StartPageNumber", 0);
		}
	}
}
