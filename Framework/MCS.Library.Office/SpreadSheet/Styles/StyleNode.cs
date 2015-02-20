using System;
using System.Collections.Generic;
using System.Text;
using System.Xml;
using MCS.Library.Core;
using System.Collections.ObjectModel;

namespace MCS.Library.Office.SpreadSheet
{
	public class StyleNode : NodeBase
	{
		public const string DefaultColor = "Automatic";

		public string ID { get; set; }
		public string Name { get; set; }

		private AlignmentNode _Alignment = null;

		private BorderNodeCollection _Borders = new BorderNodeCollection();

		public BorderNodeCollection Borders
		{
			get { return _Borders; }
		}

		public AlignmentNode Alignment
		{
			get
			{
				if (_Alignment == null)
					_Alignment = new AlignmentNode();

				return _Alignment;
			}
			set
			{
				_Alignment = value;
			}
		}

		private FontNode _Font = null;

		public FontNode Font
		{
			get
			{
				if (_Font == null)
					_Font = new FontNode();

				return _Font;
			}
			set
			{
				_Font = value;
			}
		}

		private InteriorNode _Interior = null;

		public InteriorNode Interior
		{
			get
			{
				if (_Interior == null)
					_Interior = new InteriorNode();

				return _Interior;
			}
			set
			{
				_Interior = value;
			}
		}

		private NumberFormatNode _NumberFormat = null;

		public NumberFormatNode NumberFormat
		{
			get
			{
				if (_NumberFormat == null)
					_NumberFormat = new NumberFormatNode();

				return _NumberFormat;
			}
			set
			{
				_NumberFormat = value;
			}
		}

		private ProtectionNode _Protection = null;

		public ProtectionNode Protection
		{
			get
			{
				if (_Protection == null)
					_Protection = new ProtectionNode();

				return _Protection;
			}
			set
			{
				_Protection = value;
			}
		}

		public override XmlNode AppendXmlNode(XmlNode parent)
		{
			XmlNode node = XmlHelper.AppendNode(parent, "Style");

			XmlHelper.AppendAttrWithNamespace(node, "ID", Namespaces.ss, ID);
			XmlHelper.AppendNotNullAttrWithNamespace(node, "Name", Namespaces.ss, Name);

			if (_Alignment != null)
				_Alignment.AppendXmlNode(node);

			Borders.AppendXmlNode(node);

			if (_Font != null)
				_Font.AppendXmlNode(node);

			if (_Interior != null)
				_Interior.AppendXmlNode(node);

			if (_NumberFormat != null)
				_NumberFormat.AppendXmlNode(node);

			if (_Protection != null)
				_Protection.AppendXmlNode(node);

			return node;
		}

		public override void FromXmlNode(XmlNode node)
		{
			this.ID = XmlHelper.GetAttributeText(node, "ID");
			this.Name = XmlHelper.GetAttributeText(node, "Name");

			XmlNode alignmentNode = node.SelectSingleNode("ss:Alignment", Namespaces.GetNamespaceManager());

			if (alignmentNode != null)
				Alignment.FromXmlNode(alignmentNode);

			XmlNode bordersNode = node.SelectSingleNode("ss:Borders", Namespaces.GetNamespaceManager());

			if (bordersNode != null)
				Borders.FromXmlNode(bordersNode);

			XmlNode fontNode = node.SelectSingleNode("ss:Font", Namespaces.GetNamespaceManager());

			if (fontNode != null)
				Font.FromXmlNode(fontNode);

			XmlNode interiorNode = node.SelectSingleNode("ss:Interior", Namespaces.GetNamespaceManager());

			if (interiorNode != null)
				Interior.FromXmlNode(interiorNode);

			XmlNode numberFormatNode = node.SelectSingleNode("ss:NumberFormat", Namespaces.GetNamespaceManager());

			if (numberFormatNode != null)
				NumberFormat.FromXmlNode(numberFormatNode);

			XmlNode protectionNode = node.SelectSingleNode("ss:Protection", Namespaces.GetNamespaceManager());

			if (protectionNode != null)
				Protection.FromXmlNode(protectionNode);
		}
	}

	public class StyleNodeCollection : KeyedCollection<string, StyleNode>, INode
	{
		protected override string GetKeyForItem(StyleNode item)
		{
			return item.ID;
		}

		#region INode Members

		public void FromXmlNode(XmlNode node)
		{
			XmlNodeList nodes = node.SelectNodes("ss:Style", Namespaces.GetNamespaceManager());

			foreach (XmlNode styleNode in nodes)
			{
				StyleNode style = new StyleNode();

				style.FromXmlNode(styleNode);
				this.Add(style);
			}
		}

		public XmlNode AppendXmlNode(XmlNode parent)
		{
			XmlNode node = null;

			if (this.Count > 0)
			{
				node = XmlHelper.AppendNode(parent, "Styles");

				foreach (StyleNode style in this)
					style.AppendXmlNode(node);
			}

			return node;
		}

		#endregion
	}

}
