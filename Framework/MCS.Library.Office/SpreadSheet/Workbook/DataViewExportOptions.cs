using System;
using System.Text;
using System.Collections.Generic;
using System.Data;

namespace MCS.Library.Office.SpreadSheet
{
	public delegate void DateViewExportColumnHeaderDelegate(DataColumn dataColumn, ColumnNode sheetColumn, object context);
	public delegate void DateViewExportColumnDataDelegate(DataColumn dataColumn, CellNode sheetCell, object dataValue, object context);

	/// <summary>
	/// 输出到DataView的参数
	/// </summary>
	public class DataViewExportOptions
	{
		private bool exportColumnHeader = true;
		private int startRow = 0;
		private int startColumn = 0;
		private List<string> ignoredColumnNames = new List<string>();

		public event DateViewExportColumnHeaderDelegate DateViewExportColumnHeader;
		public event DateViewExportColumnDataDelegate DateViewExportColumnData;

		/// <summary>
		/// 起始列，默认为0
		/// </summary>
		public int StartColumn
		{
			get { return this.startColumn; }
			set { this.startColumn = value; }
		}

		/// <summary>
		/// 起始行，默认为0
		/// </summary>
		public int StartRow
		{
			get { return this.startRow; }
			set { this.startRow = value; }
		}

		public bool ExportColumnHeader
		{
			get
			{
				return this.exportColumnHeader;
			}
			set
			{
				this.exportColumnHeader = value;
			}
		}

		/// <summary>
		/// 忽略的字段名称
		/// </summary>
		public List<string> IgnoredColumnNames
		{
			get
			{
				return this.ignoredColumnNames;
			}
		}

		/// <summary>
		/// 导出时，事件回调的Context对象
		/// </summary>
		public object Context
		{
			get;
			set;
		}

		internal void OnDateViewExportColumnHeader(DataColumn dataColumn, ColumnNode sheetColumn, object context)
		{
			if (DateViewExportColumnHeader != null)
				DateViewExportColumnHeader(dataColumn, sheetColumn, context);
		}

		internal void OnDateViewExportColumnData(DataColumn dataColumn, CellNode sheetCell, object dataValue, object context)
		{
			if (DateViewExportColumnData != null)
				DateViewExportColumnData(dataColumn, sheetCell, dataValue, context);
		}
	}
}
