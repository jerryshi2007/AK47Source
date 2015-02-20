using System;
using System.Collections.Generic;
using System.Text;
using System.Xml;
using MCS.Library.Core;

namespace MCS.Library.Office.SpreadSheet
{
	public class DocumentPropertiesNode : NodeBase
	{
		public string Title { get; set; }
		public string Author { get; set; }
		public string LastAuthor { get; set; }
		public DateTime Created { get; set; }
		public DateTime LastSaved { get; set; }
		public string Company { get; set; }
		public string Version { get; set; }

		public override XmlNode AppendXmlNode(XmlNode parent)
		{
			XmlNode node = XmlHelper.AppendNodeWithNamespace(parent, "DocumentProperties", Namespaces.office);

			XmlHelper.AppendNode(node, "Title", this.Title);
			XmlHelper.AppendNode(node, "Author", this.Author);

			if (Created != DateTime.MinValue)
				XmlHelper.AppendNode(node, "Created", string.Format("{0:yyyy-MM-ddTHH:mm:ss}Z", Created));

			if (LastSaved != DateTime.MinValue)
				XmlHelper.AppendNode(node, "LastSaved", string.Format("{0:yyyy-MM-ddTHH:mm:ss}Z", LastSaved));

			XmlHelper.AppendNode(node, "Company", this.Company);
			XmlHelper.AppendNode(node, "Version", this.Version);

			return node;
		}

		public override void FromXmlNode(XmlNode node)
		{
			if (node != null)
			{
				this.Title = XmlHelper.GetSingleNodeText(node, "o:Title", Namespaces.GetNamespaceManager());
				this.Author = XmlHelper.GetSingleNodeText(node, "o:Author", Namespaces.GetNamespaceManager());
				this.Company = XmlHelper.GetSingleNodeText(node, "o:Company", Namespaces.GetNamespaceManager());
				this.Version = XmlHelper.GetSingleNodeText(node, "o:Version", Namespaces.GetNamespaceManager());
			}
		}
	}
}
