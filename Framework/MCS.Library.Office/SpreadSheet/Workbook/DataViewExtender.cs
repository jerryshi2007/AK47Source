using System;
using System.Collections.Generic;
using System.Text;
using System.Data;
using MCS.Library.Core;

namespace MCS.Library.Office.SpreadSheet
{
	public static class DataViewExtender
	{
		public static WorkbookNode ExportToSpreadSheet(this DataView view, string worksheetName)
		{
			DataViewExportOptions options = new DataViewExportOptions();

			return ExportToSpreadSheet(view, worksheetName, options);
		}

		public static WorkbookNode ExportToSpreadSheet(this DataView view, string worksheetName, DataViewExportOptions options)
		{
			ExceptionHelper.FalseThrow<ArgumentNullException>(view != null, "view");
			ExceptionHelper.CheckStringIsNullOrEmpty(worksheetName, "worksheetName");
			ExceptionHelper.FalseThrow<ArgumentNullException>(options != null, "options");

			WorkbookNode workbook = new WorkbookNode();

			InitStyles(workbook);
			workbook.Worksheets.Add(BuildWorkSheet(worksheetName, view, options));

			return workbook;
		}

		public static void FillIntoSpreadSheet(this DataView view, WorkbookNode workbook, string worksheetName)
		{
			DataViewExportOptions options = new DataViewExportOptions();

			FillIntoSpreadSheet(view, workbook, worksheetName, options);
		}

		public static void FillIntoSpreadSheet(this DataView view, WorkbookNode workbook, string worksheetName, DataViewExportOptions options)
		{
			ExceptionHelper.FalseThrow<ArgumentNullException>(view != null, "view");
			ExceptionHelper.FalseThrow<ArgumentNullException>(workbook != null, "workbook");
			ExceptionHelper.CheckStringIsNullOrEmpty(worksheetName, "worksheetName");
			ExceptionHelper.FalseThrow<ArgumentNullException>(options != null, "options");

			InitStyles(workbook);

			ExceptionHelper.FalseThrow(workbook.Worksheets.Contains(worksheetName), "不能找到名称为{0}的Worksheet", worksheetName);

			WorksheetNode worksheet = workbook.Worksheets[worksheetName];		

			FillIntoWorksheet(worksheet, view, options);
		}

		#region Export Method
		private static void InitStyles(WorkbookNode workbook)
		{
			StyleNode styleDateTime = new StyleNode();

			styleDateTime.ID = "NormalDateTime";
			styleDateTime.NumberFormat.Format = "yyyy-mm-dd hh:mm:ss";

			workbook.Styles.Add(styleDateTime);
		}

		private static void FillIntoWorksheet(WorksheetNode worksheet, DataView view, DataViewExportOptions options)
		{
			foreach (DataColumn column in view.Table.Columns)
			{
				if (options.IgnoredColumnNames.Exists(c => c == column.ColumnName) == false)
				{
					ColumnNode columnNode = new ColumnNode();

					columnNode.Caption = column.ColumnName;

					options.OnDateViewExportColumnHeader(column, columnNode, options.Context);

					worksheet.Table.Columns.Add(columnNode);
				}
			}

			bool isFirstRow = true;

			if (options.ExportColumnHeader)
			{
				RowNode rowNode = BuildHeaderRow(view.Table.Columns);

				if (options.StartRow > 0)
					rowNode.Index = options.StartRow;

				worksheet.Table.Rows.Add(rowNode);

				isFirstRow = false;
			}

			foreach (DataRowView drv in view)
			{
				RowNode rowNode = new RowNode();

				if (isFirstRow)
				{
					if (options.StartRow > 0)
						rowNode.Index = options.StartRow;

					isFirstRow = false;
				}

				bool isFirstColumn = true;

				foreach (DataColumn column in view.Table.Columns)
				{
					if (options.IgnoredColumnNames.Exists(c => c == column.ColumnName) == false)
					{
						CellNode cellNode = new CellNode();

						cellNode.Data.Type = GetCellDataType(column);
						object dataValue = drv[column.ColumnName];

						if (dataValue != null)
						{
							if (cellNode.Data.Type == CellDataType.DateTime && (dataValue is DateTime))
							{
								cellNode.Data.Value = string.Format("{0:yyyy-MM-ddTHH:mm:ss}", dataValue);
								cellNode.StyleID = "NormalDateTime";
							}
							else
								cellNode.Data.Value = dataValue.ToString();
						}

						if (isFirstColumn)
						{
							if (options.StartColumn > 0)
								cellNode.Index = options.StartColumn;

							isFirstColumn = false;
						}

						options.OnDateViewExportColumnData(column, cellNode, dataValue, options.Context);

						rowNode.Cells.Add(cellNode);
					}
				}

				worksheet.Table.Rows.Add(rowNode);
			}
		}

		private static WorksheetNode BuildWorkSheet(string name, DataView view, DataViewExportOptions options)
		{
			WorksheetNode worksheet = new WorksheetNode();

			worksheet.Name = name;

			FillIntoWorksheet(worksheet, view, options);

			return worksheet;
		}

		private static CellDataType GetCellDataType(DataColumn column)
		{
			CellDataType dataType = CellDataType.String;

			switch (Type.GetTypeCode(column.DataType))
			{
				case TypeCode.DateTime:
					dataType = CellDataType.DateTime;
					break;
				case TypeCode.Int16:
				case TypeCode.Int32:
				case TypeCode.Int64:
				case TypeCode.Byte:
				case TypeCode.Decimal:
				case TypeCode.Double:
				case TypeCode.SByte:
				case TypeCode.Single:
				case TypeCode.UInt16:
				case TypeCode.UInt32:
				case TypeCode.UInt64:
					dataType = CellDataType.Number;
					break;
				case TypeCode.Boolean:
					dataType = CellDataType.Boolean;
					break;
			}

			return dataType;
		}

		private static RowNode BuildHeaderRow(DataColumnCollection columns)
		{
			RowNode rowNode = new RowNode();

			foreach (DataColumn column in columns)
			{
				CellNode cellNode = new CellNode();

				cellNode.Data.Value = column.ColumnName;

				rowNode.Cells.Add(cellNode);
			}

			return rowNode;
		}

		private static DataTable CreateDemoDataTable()
		{
			DataTable table = new DataTable();

			table.Columns.Add("Seq", typeof(int));
			table.Columns.Add("User Name", typeof(string));
			table.Columns.Add("Birthday", typeof(DateTime));

			Random rand = new Random((int)DateTime.Now.Ticks);

			for (int i = 0; i < 10; i++)
			{
				DataRow row = table.NewRow();

				row["Seq"] = i;
				row["User Name"] = string.Format("User Name: {0:0000}", i);
				row["Birthday"] = new DateTime(1972, 4, 26).AddDays(rand.Next(6000) - 3000);

				table.Rows.Add(row);
			}

			return table;
		}
		#endregion Export Method
	}
}
