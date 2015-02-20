using System;
using System.Collections.Generic;
using System.Text;
using System.Xml;
using MCS.Library.Core;

namespace MCS.Library.Office.SpreadSheet
{
	public enum AlignmentHorizontal
	{
		Automatic,
		Left,
		Center,
		Right,
		Fill,
		Justify,
		CenterAcrossSelection,
		Distributed,
		JustifyDistributed
	}

	public enum AlignmentVertical
	{
		Automatic,
		Top,
		Bottom,
		Center,
		Justify,
		Distributed,
		JustifyDistributed
	}

	public enum AlignmentReadingOrder
	{
		RightToLeft,
		LeftToRight,
		Context
	}

	public class AlignmentNode : NodeBase
	{
		public AlignmentHorizontal Horizontal { get; set; }
		public AlignmentVertical Vertical { get; set; }
		public int Indent { get; set; }
		public AlignmentReadingOrder ReadingOrder { get; set; }
		public double Rotate { get; set; }
		public bool ShrinkToFit { get; set; }
		public bool VerticalText { get; set; }
		public bool WrapText { get; set; }

		public override XmlNode AppendXmlNode(XmlNode parent)
		{
			XmlNode node = XmlHelper.AppendNodeWithNamespace(parent, "Alignment", Namespaces.ss);

			XmlHelper.AppendNotDefaultAttr(node, "Horizontal", Horizontal);
			XmlHelper.AppendNotDefaultAttr(node, "Indent", Indent);
			XmlHelper.AppendNotDefaultAttr(node, "ReadingOrder", ReadingOrder);

			XmlHelper.AppendNotDefaultAttr(node, "Rotate", Rotate);
			XmlHelper.AppendNotDefaultAttr(node, "ShrinkToFit", DataConverter.ChangeType<bool, int>(ShrinkToFit));
			XmlHelper.AppendNotDefaultAttr(node, "Vertical", Vertical);

			XmlHelper.AppendNotDefaultAttr(node, "VerticalText", VerticalText);
			XmlHelper.AppendNotDefaultAttr(node, "WrapText", DataConverter.ChangeType<bool, int>(WrapText));
			
			return node;
		}

		public override void FromXmlNode(XmlNode node)
		{
			this.Horizontal = XmlHelper.GetAttributeValue(node, "Horizontal", AlignmentHorizontal.Automatic);
			this.Vertical = XmlHelper.GetAttributeValue(node, "Vertical", AlignmentVertical.Automatic);
			this.Indent = XmlHelper.GetAttributeValue(node, "Indent", 0);
			this.ReadingOrder = XmlHelper.GetAttributeValue(node, "ReadingOrder", AlignmentReadingOrder.Context);
			this.Rotate = XmlHelper.GetAttributeValue(node, "Rotate", 0.0);
			this.ShrinkToFit = XmlHelper.GetAttributeValue(node, "ShrinkToFit", false);
			this.VerticalText = XmlHelper.GetAttributeValue(node, "VerticalText", false);
			this.WrapText = XmlHelper.GetAttributeValue(node, "WrapText", false);
		}
	}
}
