using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MCS.Library.Core;

namespace MCS.Library.Office.OpenXml.Excel
{
	public class TableColumnCollection : IEnumerable<TableColumn>
	{
		private List<TableColumn> _TableColumns = new List<TableColumn>();
		internal Dictionary<string, int> _TableColumnNames = new Dictionary<string, int>();

		private Table _Table;

		internal TableColumnCollection(Table table)
		{
			this._Table = table;
		}

		internal void InitColumns(IEnumerable<TableColumnDescription> desColumns)
		{
			foreach (TableColumnDescription tcdes in desColumns.OrderBy(tc => tc.SortOrder))
			{
				TableColumn item = new TableColumn(this._Table, tcdes);

				this.Add(item);
			}
		}

		public void Add(TableColumn tableColumn)
		{
			if (tableColumn.Position == int.MinValue)
			{
				tableColumn.Position = this._TableColumns.Count;
			}
			if (tableColumn.Name.IsNullOrEmpty())
			{
				tableColumn.Name = string.Format("列{0}", tableColumn.Position + 1);
				this._Table.NextColumnID += 1;
			}
			this._TableColumns.Add(tableColumn);
			this._TableColumnNames.Add(this._TableColumns[this.Count - 1].Name, this.Count - 1);
		}

		/// <summary>
		/// 专用于加载时
		/// </summary>
		/// <param name="tableColumn"></param>
		internal void LoadAdd(TableColumn tableColumn)
		{
			tableColumn.Position = this._TableColumns.Count;
			this._TableColumns.Add(tableColumn);
			this._TableColumnNames.Add(this._TableColumns[this.Count - 1].Name, this.Count - 1);
		}

		public TableColumn this[string Name]
		{
			get
			{
				if (this._TableColumnNames.ContainsKey(Name))
				{
					return this._TableColumns[this._TableColumnNames[Name]];
				}
				else
				{
					return null;
				}
			}
		}

		public TableColumn this[int Index]
		{
			get
			{
				ExceptionHelper.TrueThrow<ArgumentOutOfRangeException>(Index < 0 || Index >= this._TableColumns.Count, "索引超出范围");
				return this._TableColumns[Index];
			}
		}

		/// <summary>
		/// Find Index
		/// </summary>
		/// <param name="key"></param>
		/// <returns></returns>
		public int FindIndexByID(string key)
		{
			if (this._TableColumnNames.ContainsKey(key))
				return this._TableColumnNames[key];
			else
				return int.MinValue;
		}

		/// <summary>
		/// 根据列名检查是否存在指定的列名
		/// </summary>
		/// <param name="key"></param>
		/// <returns></returns>
		public bool ContainsKey(string key)
		{
			return this._TableColumnNames.ContainsKey(key);
		}

		public void Remove(string key)
		{
			int colIndex = this.FindIndexByID(key);

			if (colIndex != int.MinValue)
			{
				TableColumn item = this[key];
				this._Table.Rows.Remove(item);

				for (int i = colIndex + 1; i < this.Count; i++)
				{
					item = this._TableColumns[i];
					this._TableColumnNames[item.Name] -= 1;
				}

				this._Table.Address = Range.Parse(this._Table.Address.StartColumn, this._Table.Address.StartRow, this._Table.Address.EndColumn - 1, this._Table.Address.EndRow);
				this._TableColumnNames.Remove(key);
				this._TableColumns.RemoveAt(colIndex);
			}
		}

		public int Count
		{
			get
			{
				return this._TableColumns.Count;
			}
		}

		public IEnumerator<TableColumn> GetEnumerator()
		{
			return this._TableColumns.GetEnumerator();
		}

		System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
		{
			return this._TableColumns.GetEnumerator();
		}

		internal TableColumnCollection Clone(Table table)
		{
			TableColumnCollection cloneColumns = new TableColumnCollection(table);

			foreach (TableColumn tc in this)
			{
				cloneColumns.Add(tc.Clone(table));
			}

			return cloneColumns;
		}

	}
}
