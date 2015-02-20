using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using MCS.Library.Core;
using MCS.Library.Office.OpenXml.Excel;

namespace MCS.Library.SOA.DataObjects
{
	//SOARole的扩展功能
	public static class SOARoleExtension
	{
		/// <summary>
		/// 转换为OpenXml的WorkBook
		/// </summary>
		/// <param name="role"></param>
		/// <returns></returns>
		public static WorkBook ToExcelWorkBook(this SOARole role)
		{
			role.NullCheck("role");

			WorkBook workBook = WorkBook.CreateNew();

			FillFileInfo(workBook, role);

			WorkSheet workSheet = workBook.Sheets[WorkBook.DefaultSheetName];
			workSheet.Name = "Matrix";

			Row titleRow = new Row(1) { Height = 30d };
			titleRow.Style.Fill.SetBackgroundColor(Color.LightGray, ExcelFillStyle.Solid);
			titleRow.Style.Font.Size = 20;
			workSheet.Rows.Add(titleRow);

			workSheet.Cells[titleRow.Index, 1].Value = "角色属性";

			CreateHeaderRow(role, workSheet);

			FillSheetData(role, workSheet);

			return workBook;
		}

		private static void FillFileInfo(WorkBook workBook, SOARole role)
		{
			try
			{
				workBook.FileDetails.Title = role.Name;
				workBook.FileDetails.Subject = role.FullCodeName;
			}
			catch (System.Exception)
			{
			}
		}

		private static void FillSheetData(SOARole role, WorkSheet ws)
		{
			int rowIndex = 4;
			SOARolePropertyRowCollection rows = SOARolePropertiesAdapter.Instance.LoadByRole(role, role.PropertyDefinitions);

			foreach (SOARolePropertyRow row in rows)
			{
				foreach (DefinedName name in ws.Names)
				{
					SOARolePropertyValue propertyValue = row.Values.FindByColumnName(name.Name);

					string dataValue = null;

					if (propertyValue != null)
						dataValue = propertyValue.Value;
					else
					{
						switch (name.Name.ToLower())
						{
							case "operatortype":
								dataValue = row.OperatorType.ToString();
								break;
							case "operator":
								dataValue = row.Operator;
								break;
						}
					}

					if (dataValue != null)
						ws.Cells[rowIndex, name.Address.StartColumn].Value = dataValue;
				}

				rowIndex++;
			}
		}

		private static void CreateHeaderRow(SOARole role, WorkSheet ws)
		{
			Row headRow = new Row(3);
			headRow.Style.Fill.SetBackgroundColor(Color.Gold, ExcelFillStyle.Solid);
			headRow.Style.Border.Top.Style = ExcelBorderStyle.Thin;
			headRow.Style.Border.Top.Color.SetColor(Color.Black);
			headRow.Style.Border.Bottom.Style = ExcelBorderStyle.Thin;
			headRow.Style.Border.Bottom.Color.SetColor(Color.Black);
			headRow.Style.Border.Left.Style = ExcelBorderStyle.Thin;
			headRow.Style.Border.Left.Color.SetColor(Color.Black);
			headRow.Style.Font.Bold = true;
			ws.Rows.Add(headRow);

			int columnIndex = 1;

			foreach (SOARolePropertyDefinition dimension in role.PropertyDefinitions)
			{
				ws.Cells[headRow.Index, columnIndex].Value = dimension.Description.IsNotEmpty() ? dimension.Description : dimension.Name;
				ws.Names.Add(CellAddress.Parse(columnIndex, headRow.Index).ToString(), dimension.Name);

				columnIndex++;
			}
		}
	}
}
