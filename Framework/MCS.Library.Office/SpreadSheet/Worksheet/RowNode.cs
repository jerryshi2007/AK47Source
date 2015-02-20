using System;
using System.Collections.Generic;
using System.Text;
using System.Xml;
using MCS.Library.Core;

namespace MCS.Library.Office.SpreadSheet
{
	public class RowNode : RowAndColumnBase
	{
		public double Height { get; set; }

		private List<CellNode> _Cells = new List<CellNode>();

		public List<CellNode> Cells
		{
			get
			{
				return _Cells;
			}
		}

		private bool _AutoFitHeight = true;

		public bool AutoFitHeight
		{
			get
			{
				return this._AutoFitHeight;
			}
			set
			{
				this._AutoFitHeight = value;
			}
		}

		public override XmlNode AppendXmlNode(XmlNode parent)
		{
			XmlNode node = base.AppendXmlNode(parent);

			XmlHelper.AppendNotDefaultAttr(node, "Height", Height);

			if (!AutoFitHeight)
				XmlHelper.AppendAttr(node, "AutoFitHeight", DataConverter.ChangeType<bool, int>(AutoFitHeight));

			Cells.ForEach(cell => cell.AppendXmlNode(node));

			return node;
		}

		public override void FromXmlNode(XmlNode node)
		{
			base.FromXmlNode(node);

			this.Height = XmlHelper.GetAttributeValue(node, "Height", 0.0);
			this.AutoFitHeight = XmlHelper.GetAttributeValue(node, "AutoFitHeight", true);

			XmlNodeList cellNodes = node.SelectNodes("ss:Cell", Namespaces.GetNamespaceManager());

			foreach (XmlNode cellNode in cellNodes)
			{
				CellNode cell = new CellNode();

				cell.FromXmlNode(cellNode);
				Cells.Add(cell);
			}
		}

		public CellNode GetCellByIndex(int cellIndex)
		{
			CellNode result = null;

			int currentIndex = 0;

			for (int i = 0; i < Cells.Count; i++)
			{
				CellNode cell = Cells[i];

				if (cell.Index > 0)
					currentIndex = cell.Index;
				else
					currentIndex++;

				if (currentIndex == cellIndex)
				{
					result = cell;
					break;
				}
			}

			return result;
		}

		protected override XmlNode CreateRootNode(XmlNode parent)
		{
			return XmlHelper.AppendNodeWithNamespace(parent, "Row", Namespaces.spreadsheet);
		}
	}
}
