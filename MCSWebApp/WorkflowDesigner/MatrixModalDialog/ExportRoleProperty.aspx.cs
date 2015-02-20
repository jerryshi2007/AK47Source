using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using MCS.Library.SOA.DataObjects.Workflow;
using MCS.Library.Office.SpreadSheet;
using MCS.Web.WebControls;
using MCS.Library.SOA.DataObjects;
using MCS.Library.Core;
using System.Reflection;
using WorkflowDesigner.MatrixModalDialog;
using MCS.Library.Office.OpenXml.Excel;
using System.Drawing;
using System.Net.Mime;
using MCS.Web.Library;

namespace MCS.Applications.AppAdmin.Dialogs
{
	public partial class ExportRoleProperty : System.Web.UI.Page
	{
		private string ExFormat { get; set; }

		protected void Page_Load(object sender, EventArgs e)
		{
			Initialize();

			if (this.ExFormat == "xlsx")
			{
				ExportOpenXmlSpreadsheet();
			}
			else
			{
				ExportExcelXPXml();
			}
		}

		private static string RoleID
		{
			get
			{
				return WebUtility.GetRequestQueryString("RoleID", string.Empty);
			}
		}

		private static string AppCode
		{
			get
			{
				return WebUtility.GetRequestQueryString("AppCode", string.Empty);
			}
		}

		private static string RoleCode
		{
			get
			{
				return WebUtility.GetRequestQueryString("RoleCode", string.Empty);
			}
		}

		private static string RoleName
		{
			get
			{
				return WebUtility.GetRequestQueryString("RoleName", string.Empty);
			}
		}

		private static string DefinitionID
		{
			get
			{
				return WebUtility.GetRequestQueryString("DefinitionID", ExportRoleProperty.RoleID);
			}
		}

		private void Initialize()
		{
			RoleID.CheckStringIsNullOrEmpty("RoleID");

			if (Request["format"] == "xlsx")
			{
				this.ExFormat = "xlsx";
			}
			else
			{
				this.ExFormat = "xml";
			}
		}

		private void ExportExcelXPXml()
		{
			SOARolePropertyDefinitionCollection definition = SOARolePropertyDefinitionAdapter.Instance.LoadByRoleID(ExportRoleProperty.DefinitionID);
			string PropertySheetName = "Matrix";

			WorkbookNode workbook = ExportToExcelXml(definition, PropertySheetName);
			SOARole role = new SOARole(definition) { ID = RoleID };

			SOARolePropertyRowCollection rows = SOARolePropertiesAdapter.Instance.LoadByRole(role, definition);

			if (rows.Count > 0)
			{
				FillMatrixRowsToWorksheet(workbook, rows);
			}

			workbook.Response(GetFileNameByRole(RoleID));
		}

		/// <summary>
		/// 导出到Excel Xml中，非OpenXml
		/// </summary>
		/// <param name="coll"></param>
		/// <param name="propertySheetName"></param>
		/// <returns></returns>
		public static WorkbookNode ExportToExcelXml(SOARolePropertyDefinitionCollection definition, string propertySheetName)
		{
			WorkbookNode workbook = new WorkbookNode();
			string filePath = HttpContext.Current.Server.MapPath("RolePropertyTemplate.xml");
			workbook.Load(filePath);

			ExportToExcelXml(workbook, definition, propertySheetName);

			return workbook;
		}

		public static void ExportToExcelXml(WorkbookNode workbook, SOARolePropertyDefinitionCollection definition, string propertySheetName)
		{
			workbook.NullCheck("workbook");
			propertySheetName.NullCheck("propertySheetName");

			FillWorkSheetTitle(workbook, definition, propertySheetName);
		}

		/// <summary>
		/// 填充Excel Xml的标题列
		/// </summary>
		/// <param name="workbook"></param>
		/// <param name="definition"></param>
		/// <param name="propertySheetName"></param>
		private static void FillWorkSheetTitle(WorkbookNode workbook, SOARolePropertyDefinitionCollection definition, string propertySheetName)
		{
			WorksheetNode worksheet = GetWorksheetFromWorkbook(workbook, propertySheetName);

			worksheet.Names.Clear();
			workbook.Names.Clear();

			worksheet.Table.Rows[1].Cells.Clear();

			int row = 3;
			int column = 1;

			RowNode titleRow = null;

			if (worksheet.Table.Rows.Count > 0)
				titleRow = worksheet.Table.Rows[1];
			else
			{
				titleRow = new RowNode();
				worksheet.Table.Rows.Add(titleRow);
			}

			foreach (SOARolePropertyDefinition dd in definition)
			{
				NamedRangeNode range = new NamedRangeNode();

				range.Name = dd.Name;

				range.RefersTo = string.Format("={0}!R{1}C{2}", worksheet.Name, row, column);
				workbook.Names.Add(range);

				CellNode cell = new CellNode();

				cell.Data.Value = dd.Description.IsNotEmpty() ? dd.Description : dd.Name;

				cell.StyleID = "s17";

				titleRow.Cells.Add(cell);

				column++;
			}
		}

		private static WorksheetNode GetWorksheetFromWorkbook(WorkbookNode workbook, string propertySheetName)
		{
			WorksheetNode worksheet = null;

			if (workbook.Worksheets.Count > 0)
			{
				if (propertySheetName.IsNotEmpty())
				{
					workbook.Worksheets.Contains(propertySheetName).FalseThrow("不能在模板中找到名称为{0}的工作簿", propertySheetName);
					worksheet = workbook.Worksheets[propertySheetName];
				}
				else
				{
					worksheet = workbook.Worksheets[0];
				}
			}
			else
			{
				worksheet = new WorksheetNode();

				workbook.Worksheets.Add(worksheet);
			}

			if (propertySheetName.IsNotEmpty())
				worksheet.Name = propertySheetName;
			else
				worksheet.Name = "Matrix";

			return worksheet;
		}

		//导出带数据的Excel
		private static void FillMatrixRowsToWorksheet(WorkbookNode workbook, SOARolePropertyRowCollection rows)
		{
			NamedLocationCollection locations = workbook.Names.ToLocations();

			locations.SortByColumn();

			WorksheetNode worksheet = workbook.Worksheets[GetWorksheet(locations)];
			int startRowIndex = GetStartRow(locations);
			int currentRowIndex = -1;

			foreach (SOARolePropertyRow matrixRow in rows)
			{
				RowNode row = new RowNode();

				if (currentRowIndex == -1)
				{
					currentRowIndex = startRowIndex + 1;
					row.Index = currentRowIndex;
				}

				for (int i = 0; i < locations.Count; i++)
				{
					CellNode cell = new CellNode();

					CellLocation location = locations[i];

					SOARolePropertyValue propertyValue = matrixRow.Values.FindByColumnName(location.Name);

					string dataValue = null;

					if (propertyValue != null)
						dataValue = propertyValue.Value;
					else
					{
						switch (location.Name.ToLower())
						{
							case "operatortype":
								dataValue = matrixRow.OperatorType.ToString();
								break;
							case "operator":
								dataValue = matrixRow.Operator;
								break;
						}
					}

					if (dataValue != null)
						cell.Data.Value = dataValue;
					else
						cell.Data.Value = string.Empty;

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

		protected static void ExportOpenXmlSpreadsheet()
		{
			SOARole role = null;

			if (AppCode.IsNotEmpty() && RoleCode.IsNotEmpty())
			{
				role = new SOARole(AppCode + ":" + RoleCode);
			}
			else
			{
				SOARolePropertyDefinitionCollection definitions = SOARolePropertyDefinitionAdapter.Instance.LoadByRoleID(ExportRoleProperty.DefinitionID);
				role = new SOARole(definitions) { ID = RoleID };
			}

			WorkBook workBook = role.ToExcelWorkBook();

			workBook.Save(HttpContext.Current.Response.OutputStream);

			HttpContext.Current.Response.AppendExcelOpenXmlHeader(GetFileNameByRole(RoleID));
			HttpContext.Current.Response.End();
		}

		private static string GetFileNameByRole(string defaultName)
		{
			string fileName = string.Empty;

			if (RoleName.IsNotEmpty())
				fileName = RoleName;
			else
				if (RoleCode.IsNotEmpty())
					fileName = RoleCode;

			if (fileName.IsNullOrEmpty())
				fileName = defaultName;

			return fileName;
		}
	}
}