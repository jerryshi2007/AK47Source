using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MCS.Library.Office.OpenXml.Excel
{
	public class TableRowCollection : IEnumerable<TableRow>
	{
		private SortedDictionary<int, TableRow> rows = new SortedDictionary<int, TableRow>();

		private Table table;

		public TableRowCollection(Table table)
		{
			this.table = table;
		}

		public int Count
		{
			get
			{
				return this.rows.Count;
			}
		}

		public void Remove(int index)
		{
			if (this.rows.ContainsKey(index))
			{
				this.rows.Remove(index);
			}
		}

		public void Remove(TableRow tr)
		{
			Remove(tr.RowIndex);
		}

		public TableRow NewTableRow()
		{
			int count = this.Count;
			TableRow tr = new TableRow(this.table.Columns, count);
			
			this.rows.Add(count, tr);

			return tr;
		}

		public TableRow this[int index]
		{
			get
			{
				if (this.rows.ContainsKey(index))
				{
					return this.rows[index];
				}
				else
				{
					return null;
				}
			}
		}

		public bool ContainsKey(int index)
		{
			return this.rows.ContainsKey(index);
		}

		/// <summary>
		/// 清空所有的行
		/// </summary>
		public void Clear()
		{
			this.rows.Clear();
		}

		internal void Add(TableRow tr)
		{
			this.rows.Add(tr.RowIndex, tr);
		}

		internal void Remove(TableColumn column)
		{
			foreach (TableRow tr in this.rows.Values)
			{
				tr.Remove(column);
			}
		}

		public IEnumerator<TableRow> GetEnumerator()
		{
			return this.rows.Values.GetEnumerator();
		}

		System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
		{
			return this.rows.Values.GetEnumerator();
		}

		internal TableRowCollection Clone(Table cloneTable)
		{
			TableRowCollection cloneRows = new TableRowCollection(cloneTable);

			foreach (TableRow tr in this)
			{
				cloneRows.Add(tr.Clone(cloneTable));
			}

			return cloneRows;
		}
	}
}
