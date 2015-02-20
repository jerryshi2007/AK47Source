using System;
using System.Collections.Generic;
using System.Text;
using System.Xml;
using MCS.Library.Core;

namespace MCS.Library.Office.SpreadSheet
{
	public abstract class HeaderAndFooterNodeBase : NodeBase
	{
		private const double DefaultMargin = 0.5;

		private double _Margin = DefaultMargin;

		public double Margin
		{
			get { return _Margin; }
			set { _Margin = value; }
		}

		public string Data { get; set; }

		public override XmlNode AppendXmlNode(XmlNode parent)
		{
			XmlNode node = CreateRootNode(parent);

			XmlHelper.AppendAttr(node, "Margin", Margin);
			XmlHelper.AppendNotNullAttr(node, "Data", Data);

			return node;
		}

		protected abstract XmlNode CreateRootNode(XmlNode parent);

		public override void FromXmlNode(XmlNode node)
		{
			this._Margin = XmlHelper.GetAttributeValue(node, "Margin", Namespaces.x, DefaultMargin);
			this.Data = XmlHelper.GetAttributeText(node, "Data", Namespaces.x);
		}
	}

	public class HeaderNode : HeaderAndFooterNodeBase
	{
		protected override XmlNode CreateRootNode(XmlNode parent)
		{
			return XmlHelper.AppendNodeWithNamespace(parent, "Header", Namespaces.x);
		}

		public override void FromXmlNode(XmlNode node)
		{
			base.FromXmlNode(node);
		}
	}

	public class FooterNode : HeaderAndFooterNodeBase
	{
		protected override XmlNode CreateRootNode(XmlNode parent)
		{
			return XmlHelper.AppendNodeWithNamespace(parent, "Footer", Namespaces.x);
		}
	}
}
