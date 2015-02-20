using System;
using System.Collections.Generic;
using System.Text;
using System.Xml;
using MCS.Library.Core;

namespace MCS.Library.Office.SpreadSheet
{
	public enum FontVerticalAlign
	{
		None,
		Subscript,
		Superscript
	}

	public enum FontFamily
	{
		Automatic,
		Decorative,
		Modern,
		Roman,
		Script,
		Swiss
	}

	public enum FontUnderline
	{
		None,
		Single,
		Double,
		SingleAccounting,
		DoubleAccounting
	}

	public class FontNode : NodeBase
	{
		public bool Bold { get; set; }

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

		public string FontName { get; set; }
		public bool Italic { get; set; }
		public bool Outline { get; set; }
		public bool Shadow { get; set; }
		public double Size { get; set; }
		public bool StrikeThrough { get; set; }
		public FontUnderline Underline { get; set; }
		public FontVerticalAlign VerticalAlign { get; set; }
		public int CharSet { get; set; }
		public FontFamily Family { get; set; }

		public override XmlNode AppendXmlNode(XmlNode parent)
		{
			XmlNode node = XmlHelper.AppendNodeWithNamespace(parent, "Font", Namespaces.ss);

			XmlHelper.AppendNotDefaultAttr(node, "Bold", DataConverter.ChangeType<bool, int>(Bold));
			XmlHelper.AppendNotDefaultAttr(node, "Color", Color);
			XmlHelper.AppendNotNullAttr(node, "FontName", FontName);
			XmlHelper.AppendNotDefaultAttr(node, "Italic", DataConverter.ChangeType<bool, int>(Italic));
			XmlHelper.AppendNotDefaultAttr(node, "Outline", DataConverter.ChangeType<bool, int>(Outline));
			XmlHelper.AppendNotDefaultAttr(node, "Shadow", DataConverter.ChangeType<bool, int>(Shadow));
			XmlHelper.AppendNotDefaultAttr(node, "Size", Size);
			XmlHelper.AppendNotDefaultAttr(node, "StrikeThrough", DataConverter.ChangeType<bool, int>(StrikeThrough));
			XmlHelper.AppendNotDefaultAttr(node, "Underline", Underline);
			XmlHelper.AppendNotDefaultAttr(node, "VerticalAlign", VerticalAlign);
			XmlHelper.AppendNotDefaultAttrWithNamespace(node, "CharSet", Namespaces.x, CharSet);
			XmlHelper.AppendNotDefaultAttrWithNamespace(node, "Family", Namespaces.x, Family);

			return node;
		}

		public override void FromXmlNode(XmlNode node)
		{
			this.Bold = XmlHelper.GetAttributeValue(node, "Bold", false);
			this.Color = XmlHelper.GetAttributeValue(node, "Color", StyleNode.DefaultColor);
			this.FontName = XmlHelper.GetAttributeText(node, "FontName");
			this.Italic = XmlHelper.GetAttributeValue(node, "Italic", false);
			this.Outline = XmlHelper.GetAttributeValue(node, "Outline", false);
			this.Shadow = XmlHelper.GetAttributeValue(node, "Shadow", false);
			this.Size = XmlHelper.GetAttributeValue(node, "Size", 10.0);
			this.StrikeThrough = XmlHelper.GetAttributeValue(node, "StrikeThrough", false);
			this.Underline = XmlHelper.GetAttributeValue(node, "Underline", FontUnderline.None);
			this.VerticalAlign = XmlHelper.GetAttributeValue(node, "VerticalAlign", FontVerticalAlign.None);
			this.CharSet = XmlHelper.GetAttributeValue(node, "CharSet", Namespaces.x, 0);
			this.Family = XmlHelper.GetAttributeValue(node, "FontFamily", FontFamily.Automatic);
		}
	}
}
