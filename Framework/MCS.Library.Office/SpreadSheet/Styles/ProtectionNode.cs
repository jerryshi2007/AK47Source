using System;
using System.Xml;
using System.Text;
using System.Collections.Generic;
using MCS.Library.Core;

namespace MCS.Library.Office.SpreadSheet
{
	public class ProtectionNode : NodeBase
	{
		private bool _Protected = true;

		public bool Protected
		{
			get
			{
				return _Protected;
			}
			set
			{
				_Protected = value;
			}
		}

		public bool HideFormula { get; set; }

		public override XmlNode AppendXmlNode(XmlNode parent)
		{
			XmlNode node = XmlHelper.AppendNodeWithNamespace(parent, "Protection", Namespaces.ss);

			XmlHelper.AppendNotDefaultAttr(node, "Protected", DataConverter.ChangeType<bool, int>(Protected));
			XmlHelper.AppendNotDefaultAttrWithNamespace(node, "HideFormula", Namespaces.x, DataConverter.ChangeType<bool, int>(HideFormula));

			return node;
		}

		public override void FromXmlNode(XmlNode node)
		{
			this.Protected = XmlHelper.GetAttributeValue(node, "Protected", Namespaces.ss, false);
			this.HideFormula = XmlHelper.GetAttributeValue(node, "HideFormula", Namespaces.ss, false);
		}
	}
}
