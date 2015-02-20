using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MCS.Library.Office.OpenXml.Excel
{
	public sealed class ColumnCollection : ExcelIndexCollection<Column>
	{

		public ColumnCollection(WorkSheet sheet)
			: base(sheet)
		{

		}

		public ColumnCollection(WorkSheet sheet, int capacity)
			: base(sheet, capacity)
		{
		}

		public ColumnCollection(WorkSheet sheet, IEqualityComparer<int> comparer)
			: base(sheet, comparer)
		{
		}

		public ColumnCollection(WorkSheet sheet, IEnumerable<Column> collection)
			: base(sheet, collection)
		{
		}

		public new void Add(Column col)
		{
			if (col.Width == 0)
			{
				col.Width = this._WorkSheet.DefaultColumnWidth;
			}
			base.Add(col);
		}
	}
}
