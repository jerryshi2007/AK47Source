using System;
using System.Collections.Generic;
using System.Text;
using System.Xml;
using MCS.Library.Core;

namespace MCS.Library.Office.SpreadSheet
{
	public class PageMarginsNode : NodeBase
	{
		private double _Bottom = 1;

		public double Bottom
		{
			get { return _Bottom; }
			set { _Bottom = value; }
		}

		private double _Left = 0.75;

		public double Left
		{
			get { return _Left; }
			set { _Left = value; }
		}
		
		private double _Right = 0.75;

		public double Right
		{
			get { return _Right; }
			set { _Right = value; }
		}
		
		private double _Top = 1;

		public double Top
		{
			get { return _Top; }
			set { _Top = value; }
		}

		public override XmlNode AppendXmlNode(XmlNode parent)
		{
			XmlNode node = XmlHelper.AppendNodeWithNamespace(parent, "PageMargins", Namespaces.x);

			XmlHelper.AppendAttr(node, "Bottom", Bottom);
			XmlHelper.AppendAttr(node, "Left", Left);
			XmlHelper.AppendAttr(node, "Right", Right);
			XmlHelper.AppendAttr(node, "Top", Top);

			return node;
		}

		public override void FromXmlNode(XmlNode node)
		{
			this.Top = XmlHelper.GetAttributeValue(node, "Top", Namespaces.x, 1.0);
			this.Bottom = XmlHelper.GetAttributeValue(node, "Bottom", Namespaces.x, 1.0);
			this.Left = XmlHelper.GetAttributeValue(node, "Left", Namespaces.x, 0.75);
			this.Right = XmlHelper.GetAttributeValue(node, "Right", Namespaces.x, 0.75);
		}
	}
}
