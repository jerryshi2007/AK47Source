using MCS.Library.Core;
using MCS.Library.Globalization;
using MCS.Library.Office.OpenXml.Excel;
using MCS.Library.WF.Contracts.Workflow.DataObjects;
using MCS.Library.WF.Contracts.Workflow.Descriptors;
using System;
using System.Collections.Generic;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MCS.Library.WF.Contracts.Converters
{
    public static class ExcelExtensions
    {
        // <summary>
        /// 从WorkSheet填充矩阵
        /// </summary>
        /// <param name="matrix"></param>
        /// <param name="sheet"></param>
        public static WfClientApprovalMatrix ToApprovalMatrix(this WorkBook workBook)
        {
            workBook.NullCheck("workBook");

            DataTable matrixTable = DocumentHelper.GetRangeValuesAsTable(workBook, "Matrix", "A3");

            return new WfClientApprovalMatrix(matrixTable);
        }

        /// <summary>
        /// 从WorkSheet填充矩阵
        /// </summary>
        /// <param name="matrix"></param>
        /// <param name="sheet"></param>
        public static WfClientApprovalMatrix ToApprovalMatrix(this WorkSheet sheet)
        {
            sheet.NullCheck("sheet");

            DataTable matrixTable = DocumentHelper.GetRangeValuesAsTable(sheet, "A3");

            return new WfClientApprovalMatrix(matrixTable);
        }

        /// <summary>
        /// 矩阵转换为Excel的WorkBook的流
        /// </summary>
        /// <param name="matrix"></param>
        /// <returns></returns>
        public static Stream ToExcelStream(this IWfClientMatrixContainer matrix)
        {
            matrix.NullCheck("matrix");

            WorkBook workBook = matrix.ToWorkBook();

            MemoryStream stream = new MemoryStream();
            workBook.Save(stream);
            stream.Seek(0, SeekOrigin.Begin);

            return stream;
        }

        /// <summary>
        /// 矩阵转换为Excel的WorkBook
        /// </summary>
        /// <param name="matrix"></param>
        /// <returns></returns>
        public static WorkBook ToWorkBook(this IWfClientMatrixContainer matrix)
        {
            WorkBook workBook = WorkBook.CreateNew();
            workBook.Sheets.Clear();

            WorkSheet sheet = matrix.ToWorkSheet(workBook);

            workBook.Sheets.Add(sheet);

            return workBook;
        }

        /// <summary>
        /// 转换为Excel的WorkSheet
        /// </summary>
        /// <param name="matrix"></param>
        /// <param name="workBook"></param>
        /// <returns></returns>
        public static WorkSheet ToWorkSheet(this IWfClientMatrixContainer matrix, WorkBook workBook)
        {
            matrix.NullCheck("matrix");

            WorkSheet sheet = new WorkSheet(workBook, "Matrix");

            Row titleRow = new Row(1) { Height = 30d };
            titleRow.Style.Fill.SetBackgroundColor(Color.LightGray, ExcelFillStyle.Solid);
            titleRow.Style.Font.Size = 20;
            sheet.Rows.Add(titleRow);

            sheet.Cells[titleRow.Index, 1].Value = "审批矩阵";

            CreateMatrixHeaderRow(matrix, sheet);

            FillMatrixSheetData(matrix, sheet);

            return sheet;
        }

        public static void CreateMatrixHeaderRow(this IWfClientMatrixContainer matrix, WorkSheet sheet)
        {
            matrix.NullCheck("matrix");
            sheet.NullCheck("sheet");

            Row headRow = new Row(3);

            headRow.Style.Fill.SetBackgroundColor(Color.Gold, ExcelFillStyle.Solid);
            headRow.Style.Border.Top.Style = ExcelBorderStyle.Thin;
            headRow.Style.Border.Top.Color.SetColor(Color.Black);
            headRow.Style.Border.Bottom.Style = ExcelBorderStyle.Thin;
            headRow.Style.Border.Bottom.Color.SetColor(Color.Black);
            headRow.Style.Border.Left.Style = ExcelBorderStyle.Thin;
            headRow.Style.Border.Left.Color.SetColor(Color.Black);
            headRow.Style.Font.Bold = true;
            sheet.Rows.Add(headRow);

            int columnIndex = 1;

            foreach (WfClientRolePropertyDefinition dimension in matrix.PropertyDefinitions)
            {
                sheet.Cells[headRow.Index, columnIndex].Value = dimension.Description.IsNotEmpty() ? dimension.Description : dimension.Name;
                sheet.Names.Add(CellAddress.Parse(columnIndex, headRow.Index).ToString(), dimension.Name);

                columnIndex++;
            }
        }

        public static void FillMatrixSheetData(this IWfClientMatrixContainer matrix, WorkSheet sheet)
        {
            matrix.NullCheck("matrix");
            sheet.NullCheck("sheet");

            int rowIndex = 4;
            WfClientRolePropertyRowCollection rows = matrix.Rows;

            foreach (WfClientRolePropertyRow row in rows)
            {
                foreach (DefinedName name in sheet.Names)
                {
                    var propertyValue = row.Values.FindByColumnName(name.Name);

                    object dataValue = null;

                    if (propertyValue != null)
                    {
                        if (propertyValue.Column.DataType != Data.DataObjects.ColumnDataType.String)
                        {
                            dataValue = DataConverter.ChangeType(typeof(string),
                                propertyValue.Value,
                                propertyValue.Column.RealDataType);
                        }
                        else
                        {
                            dataValue = propertyValue.Value;
                        }
                    }
                    else
                    {
                        if (matrix.MatrixType != WfClientMatrixType.ApprovalMatrix)
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
                    }

                    if (dataValue != null)
                        sheet.Cells[rowIndex, name.Address.StartColumn].Value = dataValue;
                }

                rowIndex++;
            }
        }
    }
}
