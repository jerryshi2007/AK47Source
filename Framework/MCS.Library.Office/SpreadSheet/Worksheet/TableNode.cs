using System;
using System.Collections.Generic;
using System.Text;
using System.Xml;
using MCS.Library.Core;

namespace MCS.Library.Office.SpreadSheet
{
	public class TableNode : NodeBase
	{
		private List<RowNode> _Rows = new List<RowNode>();

		public List<RowNode> Rows
		{
			get
			{
				return _Rows;
			}
		}

		private List<ColumnNode> _Columns = new List<ColumnNode>();

		public List<ColumnNode> Columns
		{
			get
			{
				return _Columns;
			}
		}

		public int ExpandedColumnCount { get; set; }
		public int ExpandedRowCount { get; set; }

		private double _DefaultColumnWidth = 48;

		public double DefaultColumnWidth
		{
			get
			{
				return _DefaultColumnWidth;
			}
			set
			{
				_DefaultColumnWidth = value;
			}
		}

		private double _DefaultRowHeight = 12.75;

		public double DefaultRowHeight
		{
			get
			{
				return _DefaultRowHeight;
			}
			set
			{
				_DefaultRowHeight = value;
			}
		}

		public int LeftCell { get; set; }
		public int TopCell { get; set; }
		public string StyleID { get; set; }
		public bool FullColumns { get; set; }
		public bool FullRows { get; set; }

		/// <summary>
		/// 得到最大的虚拟行数
		/// </summary>
		/// <returns></returns>
		public int GetTotalVirtualRows()
		{
			int count = 0;

			foreach (RowNode row in Rows)
			{
				if (row.Index == 0)
					count++;
				else
					count = row.Index;
			}

			return count;
		}

		public RowNode GetRowByIndex(int rowIndex)
		{
			RowNode result = null;

			int currentIndex = 0;

			for (int i = 0; i < Rows.Count; i++)
			{
				RowNode row = Rows[i];

				if (row.Index > 0)
					currentIndex = row.Index;
				else
					currentIndex++;

				if (currentIndex == rowIndex)
				{
					result = row;
					break;
				}
			}

			return result;
		}

		public override XmlNode AppendXmlNode(XmlNode parent)
		{
			XmlNode node = XmlHelper.AppendNodeWithNamespace(parent, "Table", Namespaces.ss);

			if (ExpandedColumnCount > 0)
				XmlHelper.AppendNotNullAttr(node, "ExpandedColumnCount", ExpandedColumnCount);

			if (ExpandedRowCount > 0)
				XmlHelper.AppendNotNullAttr(node, "ExpandedRowCount", ExpandedRowCount);

			XmlHelper.AppendNotDefaultAttr(node, "DefaultColumnWidth", DefaultColumnWidth);
			XmlHelper.AppendNotDefaultAttr(node, "DefaultRowHeight", DefaultRowHeight);
			XmlHelper.AppendNotDefaultAttr(node, "LeftCell", LeftCell);
			XmlHelper.AppendNotDefaultAttr(node, "TopCell", TopCell);
			XmlHelper.AppendNotNullAttr(node, "StyleID", StyleID);

			XmlHelper.AppendNotDefaultAttrWithNamespace(node, "FullColumns", Namespaces.x, DataConverter.ChangeType<bool, int>(FullColumns));
			XmlHelper.AppendNotDefaultAttrWithNamespace(node, "FullRows", Namespaces.x, DataConverter.ChangeType<bool, int>(FullRows));

			Columns.ForEach(column => column.AppendXmlNode(node));
			Rows.ForEach(row => row.AppendXmlNode(node));

			return node;
		}

		public override void FromXmlNode(XmlNode node)
		{
			this.ExpandedColumnCount = XmlHelper.GetAttributeValue(node, "ExpandedColumnCount", 0);
			this.ExpandedRowCount = XmlHelper.GetAttributeValue(node, "ExpandedRowCount", 0);

			this.DefaultColumnWidth = XmlHelper.GetAttributeValue(node, "DefaultColumnWidth", 0.0);
			this.DefaultRowHeight = XmlHelper.GetAttributeValue(node, "DefaultRowHeight", 0.0);
			this.LeftCell = XmlHelper.GetAttributeValue(node, "LeftCell", 0);
			this.TopCell = XmlHelper.GetAttributeValue(node, "TopCell", 0);
			this.StyleID = XmlHelper.GetAttributeText(node, "StyleID");

			this.FullColumns = XmlHelper.GetAttributeValue(node, "FullColumns", Namespaces.x, false);
			this.FullRows = XmlHelper.GetAttributeValue(node, "FullRows", Namespaces.x, false);

			XmlNodeList columnNodes = node.SelectNodes("ss:Column", Namespaces.GetNamespaceManager());

			foreach (XmlNode columnNode in columnNodes)
			{
				ColumnNode column = new ColumnNode();

				column.FromXmlNode(columnNode);

				Columns.Add(column);
			}

			XmlNodeList rowNodes = node.SelectNodes("ss:Row", Namespaces.GetNamespaceManager());

			foreach (XmlNode rowNode in rowNodes)
			{
				RowNode row = new RowNode();

				row.FromXmlNode(rowNode);

				Rows.Add(row);
			}
		}
	}
}
