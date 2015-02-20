using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MCS.Library.Core;

namespace MCS.Library.Office.OpenXml.Excel
{
	public class CellCollection : ExcelCollectionBase<string, Cell>
	{
		public static readonly char _SplitChar = ':';
		private WorkSheet _Sheet;

		public CellCollection(WorkSheet sheet)
		{
			this._Sheet = sheet;
		}

		#region index
		public Cell this[int rowIndex, int columnIndex]
		{
			get
			{
				string key = rowIndex.ToString() + _SplitChar + columnIndex.ToString();

				if (this.ContainsKey(key))
				{
					return base[key];
				}
				else
				{
					return AddNewCell(rowIndex, columnIndex);
				}
			}
		}

		public new Cell this[string name]
		{
			get
			{
				CellAddress address = CellAddress.Parse(name);
				return this[address];
			}
		}

		public Cell this[CellAddress address]
		{
			get
			{
				return this[address.RowIndex, address.ColumnIndex];
			}
		}
		#endregion

		#region public method
		public bool HasCell(int rowIndex, int columnIndex)
		{
			string key = rowIndex.ToString() + _SplitChar + columnIndex.ToString();

			return this.ContainsKey(key);
		}

		public bool HasCell(string name)
		{
			CellAddress address =CellAddress.Parse(name);
			return this.HasCell(address);
		}

		public bool HasCell(CellAddress address)
		{
			return this.HasCell(address.RowIndex, address.ColumnIndex);
		}

		/// <summary>
		/// 在Row Index或Column Index 发生改变时同步Cell
		/// </summary>
		/// <typeparam name="T"></typeparam>
		/// <param name="index"></param>
		internal void SyncIndex<T>(int index) where T : IIndex
		{
			List<string> oldKeys = new List<string>();
			List<Cell> SyncCells = new List<Cell>();
			int indexType = 0;

			if (typeof(T) == typeof(Row))
			{
				indexType = 0;
			}
			else if (typeof(T) == typeof(Column))
			{
				indexType = 1;
			}

			foreach (var item in this.InnerDict)
			{
				var indexArr = item.Key.Split(_SplitChar);
				if (int.Parse(indexArr[indexType]) > index)
				{
					oldKeys.Add(item.Key);
					SyncCells.Add(item.Value);
				}
			}

			foreach (var item in oldKeys)
			{
				this.InnerDict.Remove(item);
			}

			foreach (var item in SyncCells)
			{
				this.InnerDict.Add(GetKeyForItem(item), item);
			}
		}
		#endregion

		protected override string GetKeyForItem(Cell item)
		{
			return item.Row.Index.ToString() + _SplitChar + item.Column.Index.ToString();
		}

		private Cell AddNewCell(int rowIndex, int columnIndex)
		{
			Row row = null;
			if (this._Sheet.Rows.ContainsKey(rowIndex))
			{
				row = this._Sheet.Rows[rowIndex];
			}
			else
			{
				row = new Row(rowIndex);
				this._Sheet.Rows.Add(row);
			}

			Column column = null;
			if (this._Sheet.Columns.ContainsKey(columnIndex))
			{
				column = this._Sheet.Columns[columnIndex];
			}
			else
			{
				column = new Column(columnIndex);
				this._Sheet.Columns.Add(column);
			}

			var newCell = Cell.CreateNewCell(row, column);

			this._Sheet.SetSheetDimension(rowIndex, columnIndex);

			this._Sheet.Cells.Add(newCell);

			return newCell;
		}

		internal CellCollection Clone(WorkSheet workSheet)
		{
			CellCollection result=new CellCollection(workSheet);
			
			foreach (Cell cell in this)
			{
				result.Add(cell.Clone(workSheet));
			}

			return result;
		}
	}
}
