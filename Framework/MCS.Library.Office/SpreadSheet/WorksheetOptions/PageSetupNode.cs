using System;
using System.Collections.Generic;
using System.Text;
using System.Xml;
using MCS.Library.Core;

namespace MCS.Library.Office.SpreadSheet
{
	public class PageSetupNode : NodeBase
	{
		private HeaderNode _Header = null;

		public HeaderNode Header
		{
			get
			{
				if (_Header == null)
					_Header = new HeaderNode();

				return _Header;
			}
			set
			{
				_Header = value;
			}
		}

		private FooterNode _Footer = null;

		public FooterNode Footer
		{
			get
			{
				if (_Footer == null)
					_Footer = new FooterNode();

				return _Footer;
			}
			set
			{
				_Footer = value;
			}
		}

		private LayoutNode _Layout = null;

		public LayoutNode Layout
		{
			get
			{
				if (_Layout == null)
					_Layout = new LayoutNode();

				return _Layout;
			}
			set
			{
				_Layout = value;
			}
		}

		private PageMarginsNode _PageMargins = null;

		public PageMarginsNode PageMargins
		{
			get
			{
				if (_PageMargins == null)
					_PageMargins = new PageMarginsNode();

				return _PageMargins;
			}
			set
			{
				_PageMargins = value;
			}
		}

		public override XmlNode AppendXmlNode(XmlNode parent)
		{
			XmlNode node = XmlHelper.AppendNodeWithNamespace(parent, "PageSetup", Namespaces.x);

			if (_Header != null)
				_Header.AppendXmlNode(node);

			if (_Footer != null)
				_Footer.AppendXmlNode(node);

			if (_Layout != null)
				_Layout.AppendXmlNode(node);

			if (_PageMargins == null)
				_PageMargins.AppendXmlNode(node);

			return node;
		}

		public override void FromXmlNode(XmlNode node)
		{
			XmlNode headerNode = node.SelectSingleNode("x:Header", Namespaces.GetNamespaceManager());

			if (headerNode != null)
				Header.FromXmlNode(headerNode);

			XmlNode footerNode = node.SelectSingleNode("x:Footer", Namespaces.GetNamespaceManager());

			if (footerNode != null)
				Footer.FromXmlNode(footerNode);

			XmlNode layoutNode = node.SelectSingleNode("x:Layout", Namespaces.GetNamespaceManager());

			if (layoutNode != null)
				Footer.FromXmlNode(layoutNode);

			XmlNode pageMarginsNode = node.SelectSingleNode("x:PageMargins", Namespaces.GetNamespaceManager());

			if (pageMarginsNode != null)
				PageMargins.FromXmlNode(pageMarginsNode);
		}
	}
}
