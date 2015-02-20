using System;
using System.Collections.Generic;
using System.Text;
using System.Xml;
using MCS.Library.Core;
using System.Collections.ObjectModel;

namespace MCS.Library.Office.SpreadSheet
{
	public enum BorderPosition
	{
		Left,
		Top,
		Right,
		Bottom,
		DiagonalLeft,
		DiagonalRight
	}

	public enum BorderLineStyle
	{
		None,
		Continuous,
		Dash,
		Dot,
		DashDot,
		DashDotDot,
		SlantDashDot,
		Double
	}

	public class BorderNode : NodeBase
	{
		public BorderPosition Position { get; set; }

		private string _Color = StyleNode.DefaultColor;

		public string Color
		{
			get
			{
				return _Color;
			}
			set
			{
				_Color = value;
			}
		}

		public BorderLineStyle LineStyle { get; set; }

		public double Weight { get; set; }

		public override XmlNode AppendXmlNode(XmlNode parent)
		{
			XmlNode node = XmlHelper.AppendNodeWithNamespace(parent, "Border", Namespaces.ss);

			XmlHelper.AppendAttr(node, "Position", Position);
			XmlHelper.AppendNotDefaultAttr(node, "Color", Color);
			XmlHelper.AppendNotDefaultAttr(node, "LineStyle", LineStyle);
			XmlHelper.AppendNotDefaultAttr(node, "Weight", Weight);

			return node;
		}

		public override void FromXmlNode(XmlNode node)
		{
			this.Position = XmlHelper.GetAttributeValue(node, "Position", BorderPosition.Left);
			this.Color = XmlHelper.GetAttributeValue(node, "Color", StyleNode.DefaultColor);
			this.LineStyle = XmlHelper.GetAttributeValue(node, "LineStyle", BorderLineStyle.None);
			this.Weight = XmlHelper.GetAttributeValue(node, "Weight", 0.0);
		}
	}

	public class BorderNodeCollection : Collection<BorderNode>, INode
	{
		#region INode Members

		public void FromXmlNode(XmlNode node)
		{
			XmlNodeList nodes = node.SelectNodes("ss:Border", Namespaces.GetNamespaceManager());

			foreach (XmlNode borderNode in nodes)
			{
				BorderNode border = new BorderNode();

				border.FromXmlNode(borderNode);
				this.Add(border);
			}
		}

		public XmlNode AppendXmlNode(XmlNode parent)
		{
			XmlNode node = XmlHelper.AppendNode(parent, "Borders");

			foreach (BorderNode border in this)
				border.AppendXmlNode(node);

			return node;
		}

		#endregion
	}
}
