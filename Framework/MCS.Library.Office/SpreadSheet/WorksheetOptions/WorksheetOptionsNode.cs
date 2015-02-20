using System;
using System.Collections.Generic;
using System.Text;
using System.Xml;
using MCS.Library.Core;

namespace MCS.Library.Office.SpreadSheet
{
	public class WorksheetOptionsNode : NodeBase
	{
		private PageSetupNode _PageSetup = null;

		public PageSetupNode PageSetup
		{
			get
			{
				if (_PageSetup == null)
					_PageSetup = new PageSetupNode();

				return _PageSetup;
			}
			set
			{
				_PageSetup = value;
			}
		}

		public override XmlNode AppendXmlNode(XmlNode parent)
		{
			XmlNode node = XmlHelper.AppendNodeWithNamespace(parent, "WorksheetOptions", Namespaces.x);

			if (_PageSetup != null)
				_PageSetup.AppendXmlNode(node);

			return node;
		}

		public override void FromXmlNode(XmlNode node)
		{
			XmlNode pageSetupNode = node.SelectSingleNode("x:PageSetup", Namespaces.GetNamespaceManager());

			if (pageSetupNode != null)
				PageSetup.FromXmlNode(pageSetupNode);
		}
	}
}
