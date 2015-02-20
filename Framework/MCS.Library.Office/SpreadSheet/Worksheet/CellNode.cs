using System;
using System.Collections.Generic;
using System.Text;
using System.Xml;
using MCS.Library.Core;

namespace MCS.Library.Office.SpreadSheet
{
	public class CellNode : NodeBase
	{
		private int _Index = 0;

		public int Index
		{
			get
			{
				return _Index;
			}
			set
			{
				_Index = value;
			}
		}

		public string StyleID { get; set; }

		private DataNode _Data = null;

		public DataNode Data
		{
			get
			{
				if (_Data == null)
					_Data = new DataNode() { OutputTypeAttribute = true };

				return _Data;
			}
			set
			{
				_Data = null;
			}
		}

		private CommentNode _Comment = null;

		public CommentNode Comment
		{
			get
			{
				if (_Comment == null)
					_Comment = new CommentNode();

				return _Comment;
			}
			set
			{
				_Comment = value;
			}
		}

		private NamedCellNode _NamedCell = null;

		public NamedCellNode NamedCell
		{
			get
			{
				if (_NamedCell == null)
					_NamedCell = new NamedCellNode();

				return _NamedCell;
			}
			set
			{
				_NamedCell = value;
			}
		}

		private PhoneticTextNode _PhoneticText = null;

		public PhoneticTextNode PhoneticText
		{
			get
			{
				if (_PhoneticText == null)
					_PhoneticText = new PhoneticTextNode();

				return _PhoneticText;
			}
			set
			{
				_PhoneticText = value;
			}
		}

		public string PasteFormula { get; set; }
		public string ArrayRange { get; set; }
		public string Formula { get; set; }
		public string HRef { get; set; }
		public int MergeAcross { get; set; }
		public int MergeDown { get; set; }
		public string HRefScreenTip { get; set; }

		public override XmlNode AppendXmlNode(XmlNode parent)
		{
			XmlNode node = XmlHelper.AppendNodeWithNamespace(parent, "Cell", Namespaces.spreadsheet);

			XmlHelper.AppendNotDefaultAttr(node, "Index", Index);
			XmlHelper.AppendNotNullAttr(node, "StyleID", StyleID);
			XmlHelper.AppendNotNullAttrWithNamespace(node, "PasteFormula", Namespaces.c, PasteFormula);

			XmlHelper.AppendNotNullAttr(node, "ArrayRange", ArrayRange);
			XmlHelper.AppendNotNullAttr(node, "Formula", Formula);
			XmlHelper.AppendNotNullAttr(node, "HRef", HRef);
			XmlHelper.AppendNotDefaultAttr(node, "MergeAcross", MergeAcross);
			XmlHelper.AppendNotDefaultAttr(node, "MergeDown", MergeDown);
			XmlHelper.AppendNotNullAttrWithNamespace(node, "HRefScreenTip", Namespaces.x, HRefScreenTip);

			if (_Data != null)
				_Data.AppendXmlNode(node);

			if (_Comment != null)
				_Comment.AppendXmlNode(node);

			if (_NamedCell != null)
				_NamedCell.AppendXmlNode(node);

			if (_PhoneticText != null)
				_PhoneticText.AppendXmlNode(node);

			return node;
		}

		public override void FromXmlNode(XmlNode node)
		{
			this.Index = XmlHelper.GetAttributeValue(node, "Index", 0);
			this.StyleID = XmlHelper.GetAttributeText(node, "StyleID");
			this.PasteFormula = XmlHelper.GetAttributeText(node, "PasteFormula", Namespaces.c);
			this.ArrayRange = XmlHelper.GetAttributeText(node, "ArrayRange");
			this.Formula = XmlHelper.GetAttributeText(node, "Formula");
			this.HRef = XmlHelper.GetAttributeText(node, "HRef");
			this.MergeAcross = XmlHelper.GetAttributeValue(node, "MergeAcross", 0);
			this.MergeDown = XmlHelper.GetAttributeValue(node, "MergeDown", 0);
			this.HRefScreenTip = XmlHelper.GetAttributeText(node, "HRefScreenTip", Namespaces.x);

			XmlNode dataNode = node.SelectSingleNode("ss:Data", Namespaces.GetNamespaceManager());

			if (dataNode != null)
				Data.FromXmlNode(dataNode);

			XmlNode commentNode = node.SelectSingleNode("ss:Comment", Namespaces.GetNamespaceManager());

			if (commentNode != null)
				Comment.FromXmlNode(commentNode);

			XmlNode namedCellNode = node.SelectSingleNode("ss:NamedCell", Namespaces.GetNamespaceManager());

			if (namedCellNode != null)
				NamedCell.FromXmlNode(namedCellNode);

			XmlNode phoneticTextNode = node.SelectSingleNode("ss:PhoneticText", Namespaces.GetNamespaceManager());

			if (phoneticTextNode != null)
				PhoneticText.FromXmlNode(phoneticTextNode);
		}
	}
}
