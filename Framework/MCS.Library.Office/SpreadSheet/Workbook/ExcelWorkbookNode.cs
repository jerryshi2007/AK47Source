using System;
using System.Collections.Generic;
using System.Text;
using System.Xml;
using MCS.Library.Core;

namespace MCS.Library.Office.SpreadSheet
{
	public class ExcelWorkbookNode : NodeBase
	{
		public int WindowHeight { get; set; }
		public int WindowWidth { get; set; }
		public int WindowTopX { get; set; }
		public int WindowTopY { get; set; }

		public bool ProtectStructure { get; set; }
		public bool ProtectWindows { get; set; }

		public override XmlNode AppendXmlNode(XmlNode parent)
		{
			XmlNode node = XmlHelper.AppendNodeWithNamespace(parent, "ExcelWorkbook", Namespaces.x);

			XmlHelper.AppendNotDefaultNode(node, "WindowHeight", this.WindowHeight);
			XmlHelper.AppendNotDefaultNode(node, "WindowWidth", this.WindowWidth);
			XmlHelper.AppendNotDefaultNode(node, "WindowTopX", this.WindowTopX);
			XmlHelper.AppendNotDefaultNode(node, "WindowTopY", this.WindowTopY);

			XmlHelper.AppendNotDefaultNode(node, "ProtectStructure", this.ProtectStructure);
			XmlHelper.AppendNotDefaultNode(node, "ProtectWindows", this.ProtectWindows);

			return node;
		}

		public override void FromXmlNode(XmlNode node)
		{
			this.WindowHeight = XmlHelper.GetSingleNodeValue(node, "x:WindowHeight", Namespaces.GetNamespaceManager(), 8192);
			this.WindowWidth = XmlHelper.GetSingleNodeValue(node, "x:WindowWidth", Namespaces.GetNamespaceManager(), 18000);
			this.WindowTopX = XmlHelper.GetSingleNodeValue(node, "x:WindowTopX", Namespaces.GetNamespaceManager(), 480);
			this.WindowTopY = XmlHelper.GetSingleNodeValue(node, "x:WindowTopY", Namespaces.GetNamespaceManager(), 75);
			this.ProtectStructure = XmlHelper.GetSingleNodeValue(node, "x:ProtectStructure", Namespaces.GetNamespaceManager(), false);
			this.ProtectWindows = XmlHelper.GetSingleNodeValue(node, "x:ProtectWindows", Namespaces.GetNamespaceManager(), false);
		}
	}
}
