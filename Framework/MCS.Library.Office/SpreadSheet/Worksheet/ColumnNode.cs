using System;
using System.Collections.Generic;
using System.Text;
using System.Xml;
using MCS.Library.Core;

namespace MCS.Library.Office.SpreadSheet
{
	public class ColumnNode : RowAndColumnBase
	{
		private bool _AutoFitWidth = true;

		public double Width { get; set; }

		public bool AutoFitWidth
		{
			get
			{
				return this._AutoFitWidth;
			}
			set
			{
				this._AutoFitWidth = value;
			}
		}

		public override XmlNode AppendXmlNode(XmlNode parent)
		{
			XmlNode node = base.AppendXmlNode(parent);

			XmlHelper.AppendNotDefaultAttr(node, "Width", Width);

			if (!AutoFitWidth)
				XmlHelper.AppendAttr(node, "AutoFitWidth", DataConverter.ChangeType<bool, int>(AutoFitWidth));

			return node;
		}

		public override void FromXmlNode(XmlNode node)
		{
			base.FromXmlNode(node);

			this.Width = XmlHelper.GetAttributeValue(node, "Width", 0.0);
			this.AutoFitWidth = XmlHelper.GetAttributeValue(node, "AutoFitWidth", true);
		}

		protected override XmlNode CreateRootNode(XmlNode parent)
		{
			return XmlHelper.AppendNodeWithNamespace(parent, "Column", Namespaces.spreadsheet);
		}
	}
}
