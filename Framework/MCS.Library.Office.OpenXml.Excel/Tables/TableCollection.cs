using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MCS.Library.Core;

namespace MCS.Library.Office.OpenXml.Excel
{
	public class TableCollection : IEnumerable<Table>
	{
		private List<Table> _Tables = new List<Table>();
		internal Dictionary<string, int> _TableNames = new Dictionary<string, int>();
		private WorkSheet _WorkSheet;

		public TableCollection(WorkSheet workSheet)
		{
			this._WorkSheet = workSheet;
		}

		internal Table Add(Table table)
		{
			this._Tables.Add(table);
			this._TableNames.Add(table.Name, this._Tables.Count - 1);

			return table;
		}

		/// <summary>
		/// 创建一个新的Table
		/// </summary>
		/// <param name="name">名称</param>
		/// <param name="rangeAddress">Table开始到结束(例如:B2:D8)</param>
		/// <returns></returns>
		public Table Add(string name, string rangeAddress)
		{
			Table table = new Table(this._WorkSheet, name, Range.Parse(rangeAddress));

			return this.Add(table);
		}

		/// <summary>
		/// 添加一个指定列名与名称的Table
		/// </summary>
		/// <param name="name"></param>
		/// <param name="rangeAddress"></param>
		/// <param name="colums"></param>
		/// <returns></returns>
		public Table Add(string name, Range rangeAddress, List<string> colums)
		{
			Table table = new Table(this._WorkSheet, name, rangeAddress);
			int columIndex = 0;
			foreach (string colum in colums)
			{
				TableColumn tcolumn = new TableColumn(table, colum, columIndex);
				this._WorkSheet.Cells[rangeAddress.StartRow, rangeAddress.StartColumn + columIndex].Value = colum;
				table.Columns.Add(tcolumn);

				columIndex++;
			}
			return this.Add(table);
		}

		/// <summary>
		/// 创建一个带有公式的Table
		/// </summary>
		/// <param name="name"></param>
		/// <param name="begionAddress"></param>
		/// <param name="colums"></param>
		/// <returns></returns>
		public Table Add(string name, string begionAddress, IDictionary<string, string> colums)
		{
			CellAddress startCell = CellAddress.Parse(begionAddress);

			return this.Add(name, startCell, colums);
		}

		public Table Add(string name, CellAddress begionAddress, IDictionary<string, string> colums)
		{
			return this.Add(name, begionAddress, colums, 1);
		}

		public Table Add(string name, CellAddress begionAddress, IDictionary<string, string> colums, int rowCount)
		{
			int endRow = begionAddress.RowIndex + rowCount;
			if (rowCount == 0)
				endRow++;

			//Range tableRange = Range.Parse(this._WorkSheet, begionAddress.ColumnIndex, begionAddress.RowIndex, begionAddress.ColumnIndex + colums.Count - 1, endRow);
			Table createTable = new Table(this._WorkSheet, name, begionAddress);
			int columIndex = 0;
			foreach (KeyValuePair<string, string> columKey in colums)
			{
				TableColumn tcolumn = new TableColumn(createTable, columKey.Key, columIndex);
				if (columKey.Value.IsNotEmpty())
					tcolumn.ColumnFormula = ExcelHelper.UserTableColumFormulaToExcelColum(columKey.Value, name);

				createTable.Columns.Add(tcolumn);
				columIndex++;
			}
			createTable.NextColumnID = columIndex + 1;

			return this.Add(createTable);
		}

		public bool TryGetTable(string tableName, out Table outTB)
		{
			bool result = false;
			outTB = null;

			if (this._TableNames.ContainsKey(tableName))
			{
				outTB = this._Tables[this._TableNames[tableName]];
				result = true;
			}

			return result;
		}

		public Table this[int Index]
		{
			get
			{
				ExceptionHelper.TrueThrow<ArgumentOutOfRangeException>(Index < 0 || Index >= this._TableNames.Count, "索引超出范围");

				return this._Tables[Index];
			}
		}

		public Table this[string Name]
		{
			get
			{
				if (this._TableNames.ContainsKey(Name))
					return this._Tables[this._TableNames[Name]];
				else
					return null;
			}
		}

		public bool ContainsKey(string tableName)
		{
			return this._TableNames.ContainsKey(tableName);
		}

		public int Count
		{
			get
			{
				return this._Tables.Count;
			}
		}

		internal TableCollection Clone(WorkSheet worksheet)
		{
			TableCollection cloneResult = new TableCollection(worksheet);

			foreach (Table table in this)
			{
				cloneResult.Add(table.Clone(worksheet));
			}

			return cloneResult;
		}

		public IEnumerator<Table> GetEnumerator()
		{
			return this._Tables.GetEnumerator();
		}

		System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
		{
			return this._Tables.GetEnumerator();
		}
	}
}
