using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MCS.Library.Core;
using MCS.Library.Data.Mapping;
using MCS.Library.Data.DataObjects;
using MCS.Library.Office.SpreadSheet;
using MCS.Library.OGUPermission;
using System.IO;
using MCS.Library.Principal;
using MCS.Library.Office.OpenXml.Excel;
using System.Drawing;
using System.Data;

namespace MCS.Library.SOA.DataObjects.Workflow
{
	[Serializable]
	[ORTableMapping("WF.MATRIX_MAIN")]
	public class WfMatrix : ILoadableDataEntity
	{
		public WfMatrix()
		{
		}

		public WfMatrix(WfMatrixDefinition definition)
		{
			definition.NullCheck("definition");

			this._Definition = definition;
		}

		[ORFieldMapping("MATRIX_ID", PrimaryKey = true)]
		public string MatrixID { get; set; }

		[ORFieldMapping("PROCESS_KEY")]
		public string ProcessKey { get; set; }

		[ORFieldMapping("ACTIVITY_KEY")]
		public string ActivityKey { get; set; }

		[ORFieldMapping("CREATOR_ID")]
		public string CreatorID { get; set; }

		[ORFieldMapping("CREATOR_NAME")]
		public string CreatorName { get; set; }

		[SqlBehavior(DefaultExpression = "getdate()", BindingFlags = ClauseBindingFlags.Insert | ClauseBindingFlags.Update)]
		[ORFieldMapping("LAST_MODIFY_TIME")]
		public DateTime LastModifyTime { get; set; }

		[NoMapping]
		public bool Loaded
		{
			get;
			set;
		}

		private WfMatrixDefinition _Definition = null;

		[SubClassORFieldMapping("Key", "DEF_KEY")]
		[SubClassType(typeof(WfMatrixDefinition))]
		public WfMatrixDefinition Definition
		{
			get
			{
				return this._Definition;
			}
			internal set
			{
				this._Definition = value;
			}
		}

		private WfMatrixRowCollection _Rows = null;

		public WfMatrixRowCollection Rows
		{
			get
			{
				if (this._Rows == null)
				{
					this._Rows = new WfMatrixRowCollection(this);

					if (this.Loaded)
						WfMatrixAdapter.Instance.FillMatrixRows(this.MatrixID, this._Rows);
				}

				return this._Rows;
			}
		}

		/// <summary>
		/// 将矩阵导出成Xml格式
		/// </summary>
		/// <param name="roleAsPerson"></param>
		/// <returns></returns>
		public WorkbookNode ExportToExcelXml(bool roleAsPerson)
		{
			this.Definition.NullCheck("Matrix.Definition");

			WorkbookNode workbook = this.Definition.ExportToExcelXml();

			FillMatrixRowsToWorksheet(workbook, this, roleAsPerson);

			return workbook;
		}

		/// <summary>
		/// 将矩阵导出成OpenXml
		/// </summary>
		/// <param name="roleAsPerson"></param>
		/// <returns></returns>
		public MemoryStream ExportToExcel2007(bool roleAsPerson)
		{
			this.Definition.NullCheck("Matrix.Definition");

			WorkBook wb = WorkBook.CreateNew();
			WorkSheet ws = wb.Sheets["sheet1"];
			ws.Name = "Matrix";
			//new WorkSheet(wb, "Matrix");
			//wb.Sheets.Clear();
			//wb.Sheets.Add(ws);

			Row titleRow = new Row(1) { Height = 30d };
			titleRow.Style.Fill.SetBackgroundColor(Color.LightGray, ExcelFillStyle.Solid);
			titleRow.Style.Font.Size = 20;

			Row headRow = new Row(3);
			headRow.Style.Fill.SetBackgroundColor(Color.Gold, ExcelFillStyle.Solid);
			headRow.Style.Border.Top.Style = ExcelBorderStyle.Thin;
			headRow.Style.Border.Top.Color.SetColor(Color.Black);
			headRow.Style.Border.Bottom.Style = ExcelBorderStyle.Thin;
			headRow.Style.Border.Bottom.Color.SetColor(Color.Black);
			headRow.Style.Border.Left.Style = ExcelBorderStyle.Thin;
			headRow.Style.Border.Left.Color.SetColor(Color.Black);
			headRow.Style.Font.Bold = true;

			ws.Rows.Add(titleRow);
			ws.Rows.Add(headRow);

			ws.Cells[titleRow.Index, 1].Value = "授权矩阵";

			int columnIndex = 1;
			foreach (var dimension in this.Definition.Dimensions)
			{
				ws.Cells[headRow.Index, columnIndex].Value = dimension.Name;
				ws.Names.Add(CellAddress.Parse(columnIndex, headRow.Index).ToString(), dimension.DimensionKey);
				columnIndex++;
			}

			int rowIndex = 4;
			foreach (var row in this.Rows)
			{
				foreach (var cell in row.Cells)
				{
					var cIndex = ws.Names[cell.Definition.DimensionKey].Address.StartColumn;
					ws.Cells[rowIndex, cIndex].Value = GetCellValue(roleAsPerson, cell, row);
				}
				rowIndex++;
			}

			MemoryStream exdelStream = new MemoryStream();
			wb.Save(exdelStream);
			//exdelStream.Seek(0, SeekOrigin.Begin);

			return exdelStream;
		}

		/// <summary>
		/// 创建单元格的值
		/// </summary>
		/// <param name="roleAsPerson"></param>
		/// <param name="cell"></param>
		/// <param name="row"></param>
		/// <returns></returns>
		private static string GetCellValue(bool roleAsPerson, WfMatrixCell cell, WfMatrixRow row)
		{
			if (roleAsPerson == false)
			{
				return cell.StringValue;
			}

			if (cell.Definition.DimensionKey == "OperatorType")
			{
				return WfMatrixOperatorType.Person.ToString();
			}

			if (cell.Definition.DimensionKey == "Operator")
			{
				var typeCell = row.Cells.Find(p => p.Definition.DimensionKey == "OperatorType");
				if (typeCell.StringValue == WfMatrixOperatorType.Role.ToString())
				{
					var users = GetUsersInRole(cell.StringValue);

					StringBuilder namesBuilder = new StringBuilder();
					foreach (var user in users)
					{
						namesBuilder.AppendFormat("{0},", user.LogOnName);
					}
					if (namesBuilder.Length > 0)
					{
						namesBuilder.Remove(namesBuilder.Length - 1, 1);
					}

					return namesBuilder.ToString();
				}
			}

			return cell.StringValue;
		}

		private static void ImportExcel2007(Stream importStream, WfMatrix matrix, Action notifier, string processDescKey)
		{
			DataTable dt = DocumentHelper.GetRangeValuesAsTable(importStream, "Matrix", "A3");

			int rowIndex = 0;
			foreach (DataRow row in dt.Rows)
			{
				WfMatrixRow matrixRow = new WfMatrixRow(matrix) { RowNumber = rowIndex };
				foreach (var dimension in matrix.Definition.Dimensions)
				{
					WfMatrixCell matrixCell = new WfMatrixCell(dimension);
					matrixCell.StringValue = row[dimension.DimensionKey].ToString();

					switch (dimension.DimensionKey)
					{
						case "Operator":
							matrixRow.Operator = row[dimension.DimensionKey].ToString();
							break;
						case "OperatorType":
							WfMatrixOperatorType opType = WfMatrixOperatorType.Person;
							Enum.TryParse(row[dimension.DimensionKey].ToString(), out opType);
							matrixRow.OperatorType = opType;
							break;
						default:
							break;
					}
					matrixRow.Cells.Add(matrixCell);
				}

				if (notifier != null)
				{
					notifier();
				}

				rowIndex++;
				matrix.Rows.Add(matrixRow);
			}

			WfMatrixAdapter.Instance.DeleteByProcessKey(matrix.ProcessKey);
			WfMatrixAdapter.Instance.Update(matrix);
		}

		public static void ImportExistMatrixFromExcel2007(Stream stream, Action notifier, string processDescKey)
		{
			WfMatrix matrix = WfMatrixAdapter.Instance.LoadByProcessKey(processDescKey, true);
			matrix.Rows.Clear();

			ImportExcel2007(stream, matrix, notifier, processDescKey);
		}

		public static void ImportNewMatrixFromExcel2007(Stream stream, Action notifier, string processDescKey, WfMatrixDefinition matrixDef)
		{
			WfMatrix matrix = new WfMatrix(matrixDef)
			{
				MatrixID = Guid.NewGuid().ToString(),
				ProcessKey = processDescKey,
				CreatorID = DeluxeIdentity.CurrentUser.ID,
				CreatorName = DeluxeIdentity.CurrentUser.Name
			};
			ImportExcel2007(stream, matrix, notifier, processDescKey);
		}

		/// <summary>
		/// 从ExcelXml中导入已经存在的矩阵
		/// </summary>
		/// <param name="stream"></param>
		/// <param name="notifier"></param>
		/// <param name="processDescKey"></param>
		public static void ImportExistMatrixFromExcelXml(Stream stream, Action notifier, string processDescKey)
		{
			WfMatrix matrix = WfMatrixAdapter.Instance.LoadByProcessKey(processDescKey, true);
			matrix.Rows.Clear();

			ImportExcelXml(stream, matrix, notifier);
		}

		/// <summary>
		/// 从ExcelXml中导入已经存在的矩阵
		/// </summary>
		/// <param name="stream"></param>
		/// <param name="notifier"></param>
		/// <param name="processDescKey"></param>
		public static void ImportNewMatrixFromExcelXml(Stream stream, Action notifier, string processDescKey, WfMatrixDefinition matrixDef)
		{
			WfMatrix matrix = new WfMatrix(matrixDef)
			{
				MatrixID = Guid.NewGuid().ToString(),
				ProcessKey = processDescKey,
				CreatorID = DeluxeIdentity.CurrentUser.ID,
				CreatorName = DeluxeIdentity.CurrentUser.Name
			};
			//ImportFromExcel2007(stream, notifier, processDescKey);
			ImportExcelXml(stream, matrix, notifier);
		}

		private static void ImportExcelXml(Stream stream, WfMatrix matrix, Action notifier)
		{
			WorkbookNode workbook = new WorkbookNode();

			workbook.Load(stream);

			ExceptionHelper.FalseThrow(workbook.Worksheets.Contains("Matrix"),
				"The workbook must contains a 'Matrix' worksheet.");

			NamedLocationCollection fieldLocations = workbook.Names.ToLocations();

			TableNode table = workbook.Worksheets["Matrix"].Table;

			int baseRowIndex = GetStartRow(fieldLocations);

			RowNode titleRow = table.GetRowByIndex(baseRowIndex);

			int currentRowIndex = table.Rows.IndexOf(titleRow) + 1;

			if (table.Rows.Count > currentRowIndex)
			{
				int currentVirtualRow = baseRowIndex;
				int count = table.Rows.Count - currentRowIndex;
				for (int i = 0; i < count; i++)
				{
					RowNode row = table.Rows[currentRowIndex];

					if (row.Index > 0)
						currentVirtualRow = row.Index;
					else
						currentVirtualRow++;

					GenerateMatrixRow(matrix, row, fieldLocations, i);

					if (notifier != null)
					{
						notifier();
					}

					currentRowIndex++;
				}
			}
			WfMatrixAdapter.Instance.DeleteByProcessKey(matrix.ProcessKey);
			WfMatrixAdapter.Instance.Update(matrix);
		}

		private static WfMatrixRow GenerateMatrixRow(WfMatrix matrix, RowNode rowNode, NamedLocationCollection locations, int index)
		{
			WfMatrixRow mRow = new WfMatrixRow(matrix);

			mRow.RowNumber = index;

			int emptyCellCount = 0;

			foreach (WfMatrixDimensionDefinition dd in matrix.Definition.Dimensions)
			{
				CellLocation location = locations[dd.DimensionKey];

				CellNode cell = rowNode.GetCellByIndex(location.Column);

				WfMatrixCell mCell = new WfMatrixCell(dd);

				mCell.StringValue = cell.Data.InnerText.Trim();

				mRow.Cells.Add(mCell);

				switch (dd.DimensionKey)
				{
					case "Operator":
						mRow.Operator = cell.Data.InnerText;
						break;
					case "OperatorType":
						WfMatrixOperatorType opType = WfMatrixOperatorType.Person;
						Enum.TryParse(cell.Data.InnerText, out opType);
						mRow.OperatorType = opType;
						break;
					default:
						if (mCell.StringValue.IsNullOrEmpty())
							emptyCellCount++;
						break;
				}
			}

			if (emptyCellCount >= matrix.Definition.Dimensions.Count - 2)
			{
				//如果每一列都为空（不算Operator和OperatorType），那么认为整行都为空
				mRow = null;
			}
			else
				matrix.Rows.Add(mRow);

			return mRow;
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="rolePath">应用名称：角色名称</param>
		/// <returns></returns>
		public static OguObjectCollection<IUser> GetUsersInRole(string rolePath)
		{
			OguObjectCollection<IUser> result = null;

			int splitterCharIndex = rolePath.IndexOf(':');

			if (splitterCharIndex <= 0)
			{
				result = new OguObjectCollection<IUser>();
			}
			else
			{
				string appName = rolePath.Substring(0, splitterCharIndex);
				string roleNames = rolePath.Substring(splitterCharIndex + 1);

				result = RoleImpl.GetUsersFromRoles("", appName, roleNames, DelegationMaskType.All, SidelineMaskType.All, "");
			}

			return result;
		}

		private static void FillMatrixRowsToWorksheet(WorkbookNode workbook, WfMatrix matrix, bool roleAsPerson)
		{
			NamedLocationCollection locations = workbook.Names.ToLocations();

			locations.SortByColumn();

			WorksheetNode worksheet = workbook.Worksheets[GetWorksheet(locations)];
			int startRowIndex = GetStartRow(locations);
			int currentRowIndex = -1;

			foreach (WfMatrixRow matrixRow in matrix.Rows)
			{
				RowNode row = new RowNode();

				if (currentRowIndex == -1)
				{
					currentRowIndex = startRowIndex + 1;
					row.Index = currentRowIndex;
				}

				foreach (CellLocation location in locations)
				{
					CellNode cell = new CellNode();
					WfMatrixCell mCell = matrixRow.Cells.Find(c => c.Definition.DimensionKey == location.Name);

					if (mCell != null)
					{
						cell.Data.Value = GetCellValue(roleAsPerson, mCell, matrixRow);
					}
					row.Cells.Add(cell);
				}

				worksheet.Table.Rows.Add(row);
			}
		}

		private static string GetWorksheet(NamedLocationCollection locations)
		{
			string worksheetName = string.Empty;

			if (locations.Count > 0)
				worksheetName = locations[0].WorksheetName;

			return worksheetName;
		}

		private static int GetStartRow(NamedLocationCollection locations)
		{
			int result = 0;

			if (locations.Count > 0)
				result = locations[0].Row;

			return result;
		}
	}

	[Serializable]
	public class WfMatrixCollection : SerializableEditableKeyedDataObjectCollectionBase<string, WfMatrix>
	{
		protected override string GetKeyForItem(WfMatrix item)
		{
			return item.MatrixID;
		}
	}
}
