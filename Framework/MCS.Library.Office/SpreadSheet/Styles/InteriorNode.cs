using System;
using System.Collections.Generic;
using System.Text;
using System.Xml;
using MCS.Library.Core;

namespace MCS.Library.Office.SpreadSheet
{
	public enum InteriorPattern
	{
		None,
		Solid,
		Gray75,
		Gray50,
		Gray25,
		Gray125,
		Gray0625,
		HorzStripe,
		VertStripe,
		ReverseDiagStripe,
		DiagStripe,
		DiagCross,
		ThickDiagCross,
		ThinHorzStripe,
		ThinVertStripe,
		ThinReverseDiagStripe,
		ThinDiagStripe,
		ThinHorzCross,
		ThinDiagCross
	}

	public class InteriorNode : NodeBase
	{
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

		private string _PatternColor = StyleNode.DefaultColor;

		public string PatternColor
		{
			get
			{
				return _PatternColor;
			}
			set
			{
				_PatternColor = value;
			}
		}

		public InteriorPattern Pattern { get; set; }

		public override XmlNode AppendXmlNode(XmlNode parent)
		{
			XmlNode node = XmlHelper.AppendNodeWithNamespace(parent, "Interior", Namespaces.ss);

			XmlHelper.AppendNotDefaultAttr(node, "Color", Color);
			XmlHelper.AppendNotDefaultAttr(node, "Pattern", Pattern);

			if (PatternColor == StyleNode.DefaultColor)
				XmlHelper.AppendNotDefaultAttr(node, "PatternColor", Color);
			else
				XmlHelper.AppendNotDefaultAttr(node, "PatternColor", PatternColor);

			return node;
		}

		public override void FromXmlNode(XmlNode node)
		{
			this.Color = XmlHelper.GetAttributeValue(node, "Color", StyleNode.DefaultColor);
			this.Pattern = XmlHelper.GetAttributeValue(node, "Pattern", InteriorPattern.None);
			this.PatternColor = XmlHelper.GetAttributeValue(node, "PatternColor", StyleNode.DefaultColor);
		}
	}
}
