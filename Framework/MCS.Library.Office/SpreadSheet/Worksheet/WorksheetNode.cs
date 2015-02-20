using System;
using System.Collections.Generic;
using System.Text;
using System.Xml;
using MCS.Library.Core;
using System.Collections.ObjectModel;
using System.Data;

namespace MCS.Library.Office.SpreadSheet
{
	public class WorksheetNode : NodeBase
	{
		private TableNode _Table = new TableNode();

		private NamedRangeCollection _Names = new NamedRangeCollection();

		public NamedRangeCollection Names
		{
			get
			{
				return _Names;
			}
		}

		public TableNode Table
		{
			get
			{
				return _Table;
			}
		}

		public string Name { get; set; }

		private WorksheetOptionsNode _WorksheetOptions = null;

		public WorksheetOptionsNode WorksheetOptions
		{
			get
			{
				if (_WorksheetOptions == null)
					_WorksheetOptions = new WorksheetOptionsNode();

				return _WorksheetOptions;
			}
			set
			{
				_WorksheetOptions = value;
			}
		}

		public override XmlNode AppendXmlNode(XmlNode parent)
		{
			XmlNode node = XmlHelper.AppendNodeWithNamespace(parent, "Worksheet", Namespaces.spreadsheet);

			XmlHelper.AppendAttr(node, "Name", this.Name);

			if (_WorksheetOptions != null)
				_WorksheetOptions.AppendXmlNode(parent);

			Table.AppendXmlNode(node);
			Names.AppendXmlNode(node);

			return node;
		}

		public override void FromXmlNode(XmlNode node)
		{
			this.Name = XmlHelper.GetAttributeText(node, "Name");

			XmlNode worksheetOptionsNode = node.SelectSingleNode("x:WorksheetOptions", Namespaces.GetNamespaceManager());

			if (worksheetOptionsNode != null)
				WorksheetOptions.FromXmlNode(worksheetOptionsNode);

			XmlNode tableNode = node.SelectSingleNode("ss:Table", Namespaces.GetNamespaceManager());

			if (tableNode != null)
				Table.FromXmlNode(tableNode);

			XmlNode namesNode = node.SelectSingleNode("ss:Names", Namespaces.GetNamespaceManager());

			if (namesNode != null)
				Names.FromXmlNode(namesNode);
		}
	}

	public class WorksheetNodeCollection : KeyedCollection<string, WorksheetNode>, INode
	{
		protected override string GetKeyForItem(WorksheetNode item)
		{
			return item.Name;
		}

		#region INode Members

		public void FromXmlNode(XmlNode node)
		{
			XmlNodeList nodes = node.SelectNodes("ss:Worksheet", Namespaces.GetNamespaceManager());

			foreach (XmlNode worksheetNode in nodes)
			{
				WorksheetNode worksheet = new WorksheetNode();

				worksheet.FromXmlNode(worksheetNode);

				this.Add(worksheet);
			}
		}

		public XmlNode AppendXmlNode(XmlNode parent)
		{
			XmlNode node = null;

			if (this.Count > 0)
			{
				foreach (WorksheetNode worksheet in this)
					worksheet.AppendXmlNode(parent);
			}

			return node;
		}

		#endregion
	}
}
