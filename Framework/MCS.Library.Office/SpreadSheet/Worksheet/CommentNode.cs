using System;
using System.Collections.Generic;
using System.Text;
using System.Xml;
using MCS.Library.Core;

namespace MCS.Library.Office.SpreadSheet
{
	public class CommentNode : NodeBase
	{
		private DataNode _Data = new DataNode() { OutputTypeAttribute = false };

		public DataNode Data
		{
			get { return _Data; }
		}

		public string Author { get; set; }
		public bool ShowAlways { get; set; }

		public override XmlNode AppendXmlNode(XmlNode parent)
		{
			XmlNode node = XmlHelper.AppendNodeWithNamespace(parent, "Comment", Namespaces.ss);

			XmlHelper.AppendNotNullAttr(node, "Author", Author);
			XmlHelper.AppendNotDefaultAttr(node, "ShowAlways", DataConverter.ChangeType<bool, int>(ShowAlways));

			Data.AppendXmlNode(node);

			return node;
		}

		public override void FromXmlNode(XmlNode node)
		{
			XmlNode dataNode = node.SelectSingleNode("ss:Data", Namespaces.GetNamespaceManager());

			if (dataNode != null)
				Data.FromXmlNode(dataNode);

			this.Author = XmlHelper.GetAttributeText(node, "Author");
			this.ShowAlways = XmlHelper.GetAttributeValue(node, "ShowAlways", false);
		}
	}
}
