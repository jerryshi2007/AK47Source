using MCS.Library.Core;
using MCS.Library.Data.Builder;
using MCS.Library.Data.DataObjects;
using MCS.Library.Globalization;
using MCS.Library.Office.OpenXml.Excel;
using MCS.Library.OGUPermission;
using System;
using System.Collections.Generic;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MCS.Library.SOA.DataObjects.Workflow
{
    /// <summary>
    /// 流程操作的扩展方法
    /// </summary>
    public static class ProcessExtensions
    {
        /// <summary>
        /// 用户是否在Acl中
        /// </summary>
        /// <param name="process"></param>
        /// <param name="user"></param>
        /// <returns></returns>
        public static bool IsUserInAcl(this IWfProcess process, IUser user)
        {
            bool result = false;

            if (OguUser.IsNotNullOrEmpty(user))
            {
                InSqlClauseBuilder inBuilder = new InSqlClauseBuilder("RESOURCE_ID");

                inBuilder.AppendItem(process.ResourceID, process.ID);

                WhereSqlClauseBuilder whereBuilder = new WhereSqlClauseBuilder();

                whereBuilder.AppendItem("OBJECT_TYPE", "Users");
                whereBuilder.AppendItem("OBJECT_ID", user.ID);

                ConnectiveSqlClauseCollection connective = new ConnectiveSqlClauseCollection(inBuilder, whereBuilder);

                WfAclItemCollection aclItems = WfAclAdapter.Instance.LoadByBuilder(connective);

                result = aclItems.Count > 0;
            }

            return result;
        }

        /// <summary>
        /// 将WorkSheet转换为WfActivityMatrixResourceDescriptor
        /// </summary>
        /// <param name="workBook"></param>
        /// <param name="workSheetName"></param>
        /// <param name="beginAddress"></param>
        /// <returns></returns>
        public static WfActivityMatrixResourceDescriptor ToActivityMatrixResourceDescriptor(this WorkBook workBook, string workSheetName, string beginAddress)
        {
            workBook.NullCheck("workBook");
            workSheetName.CheckStringIsNullOrEmpty("workSheetName");
            beginAddress.CheckStringIsNullOrEmpty("beginAddress");

            DataTable matrixTable = DocumentHelper.GetRangeValuesAsTable(workBook, "Matrix", "A3");

            return new WfActivityMatrixResourceDescriptor(matrixTable);
        }

        /// <summary>
        /// 将WfActivityMatrixResourceDescriptor填充到Excel的WorkBook中
        /// </summary>
        /// <param name="workBook"></param>
        /// <param name="activityMatrix"></param>
        public static void FillActivityMatrixResourceDescriptor(this WorkBook workBook, WfActivityMatrixResourceDescriptor activityMatrix)
        {
            workBook.NullCheck("workBook");

            workBook.Sheets.Remove("Matrix");

            WorkSheet sheet = new WorkSheet(workBook, "Matrix");

            workBook.Sheets.Add(sheet);

            sheet.FillActivityMatrixResourceDescriptor(activityMatrix);
        }

        /// <summary>
        /// 将WfActivityMatrixResourceDescriptor填充到Excel的WorkSheet中
        /// </summary>
        /// <param name="sheet"></param>
        /// <param name="activityMatrix"></param>
        public static void FillActivityMatrixResourceDescriptor(this WorkSheet sheet, WfActivityMatrixResourceDescriptor activityMatrix)
        {
            sheet.NullCheck("sheet");
            activityMatrix.NullCheck("activityMatrix");

            int startRowIndex = 1;

            Row titleRow = new Row(startRowIndex) { Height = 30d };
            titleRow.Style.Fill.SetBackgroundColor(Color.LightGray, ExcelFillStyle.Solid);
            titleRow.Style.Font.Size = 20;

            sheet.Rows.Add(titleRow);
            sheet.Cells[titleRow.Index, 1].Value = "角色属性";

            startRowIndex += 2;

            CreateMatrixHeaderRow(sheet, activityMatrix, startRowIndex++);

            FillMatrixSheetData(sheet, activityMatrix, startRowIndex);
        }

        private static void CreateMatrixHeaderRow(WorkSheet sheet, WfActivityMatrixResourceDescriptor activityMatrix, int startRowIndex)
        {
            Row headerRow = new Row(startRowIndex);

            headerRow.Style.Fill.SetBackgroundColor(Color.Gold, ExcelFillStyle.Solid);
            headerRow.Style.Border.Top.Style = ExcelBorderStyle.Thin;
            headerRow.Style.Border.Top.Color.SetColor(Color.Black);
            headerRow.Style.Border.Bottom.Style = ExcelBorderStyle.Thin;
            headerRow.Style.Border.Bottom.Color.SetColor(Color.Black);
            headerRow.Style.Border.Left.Style = ExcelBorderStyle.Thin;
            headerRow.Style.Border.Left.Color.SetColor(Color.Black);
            headerRow.Style.Font.Bold = true;

            sheet.Rows.Add(headerRow);

            int columnIndex = 1;

            foreach (SOARolePropertyDefinition dimension in activityMatrix.PropertyDefinitions)
            {
                sheet.Cells[headerRow.Index, columnIndex].Value = dimension.Description.IsNotEmpty() ? dimension.Description : dimension.Name;
                sheet.Names.Add(CellAddress.Parse(columnIndex, headerRow.Index).ToString(), dimension.Name);

                columnIndex++;
            }
        }

        private static int FillMatrixSheetData(WorkSheet sheet, WfActivityMatrixResourceDescriptor activityMatrix, int startRowIndex)
        {
            SOARolePropertyRowCollection rows = activityMatrix.Rows;

            foreach (SOARolePropertyRow row in rows)
            {
                foreach (DefinedName bookmark in sheet.Names)
                {
                    object dataValue = GetDataValueByBookmark(bookmark, row);

                    if (dataValue != null)
                        sheet.Cells[startRowIndex, bookmark.Address.StartColumn].Value = dataValue;
                }

                startRowIndex++;
            }

            return startRowIndex;
        }

        private static object GetDataValueByBookmark(DefinedName bookmark, SOARolePropertyRow row)
        {
            object result = null;

            SOARolePropertyValue propertyValue = row.Values.FindByColumnName(bookmark.Name);

            if (propertyValue != null)
            {
                if (propertyValue.Column.DataType != ColumnDataType.String)
                    result = DataConverter.ChangeType(typeof(string), propertyValue.Value, propertyValue.Column.RealDataType);
                else
                    result = propertyValue.Value;
            }
            else
            {
                switch (bookmark.Name.ToLower())
                {
                    case "operatortype":
                        result = row.OperatorType.ToString();
                        break;
                    case "operator":
                        result = row.Operator;
                        break;
                }
            }

            return result;
        }
    }
}
