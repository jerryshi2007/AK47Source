using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Data;
using MCS.Library.Core;

namespace MCS.Library.Office.OpenXml.Excel
{
    /// <summary>
    ///  Excel文档操作辅助类
    /// </summary>
    public static class DocumentHelper
    {
        /// <summary>
        /// 将Excel数据填充到指定的模板
        /// </summary>
        /// <param name="input">模板文件件流</param>
        /// <param name="worksheetName">工作表名称</param>
        /// <param name="dvData">待填充数据</param>
        /// <returns></returns>
        public static MemoryStream FillExcelTemplatesByDefinedName(Stream input, string worksheetName, System.Data.DataView dvData, bool buyColumnName = false)
        {
            input.NullCheck("模版流不能为这空！");
            dvData.NullCheck("填充的数据不能为空");
            worksheetName.NullCheck("工作薄名称不能为空");
            MemoryStream fillExcelStream = new MemoryStream();
            WorkBook package = WorkBook.Load(input);
            WorkSheet worksheet = package.Sheets[worksheetName];

            worksheet.NullCheck("指定的工作表名称不存在！");

            Dictionary<string, CellAddress> dtExcel = new Dictionary<string, CellAddress>();
            List<string> formulaDictionary = new List<string>();

            DefinedName nameRange;
            CellAddress cellAddress = default(CellAddress);
            Cell formulaCell = null;

            foreach (DataColumn dc in dvData.Table.Columns)
            {
                if (buyColumnName)
                {
                    nameRange = worksheet.Names[dc.ColumnName];
                }
                else
                {
                    nameRange = worksheet.Names[string.IsNullOrEmpty(dc.Caption) ? dc.ColumnName : dc.Caption];
                }
                if (nameRange != null)
                {
                    cellAddress = CellAddress.Parse(nameRange.Address.StartColumn, nameRange.Address.StartRow);
                    dtExcel.Add(dc.ColumnName, cellAddress);
                    formulaCell = worksheet.Cells[cellAddress.RowIndex + 1, cellAddress.ColumnIndex];
                    if (formulaCell != null)
                    {
                        if (formulaCell.Formula.IsNotEmpty())
                        {
                            formulaDictionary.Add(dc.ColumnName);
                        }
                    }
                }
            }

            int formulaRowIndex = cellAddress.RowIndex + 1, addRowCount = 0;
            worksheet.InserRows(formulaRowIndex, dvData.Count - 1, formulaRowIndex);
            foreach (DataRowView dr in dvData)
            {
                foreach (KeyValuePair<string, CellAddress> key in dtExcel)
                {
                    if (!formulaDictionary.Contains(key.Key))
                    {
                        worksheet.Cells[formulaRowIndex + addRowCount, key.Value.ColumnIndex].Value = dr[key.Key];
                    }
                }
                addRowCount++;
            }

            package.Save(fillExcelStream);

            fillExcelStream.Seek(0, SeekOrigin.Begin);

            return fillExcelStream;
        }

        /// <summary>
        /// 创建一个指定Excel 工作表名称Excel
        /// </summary>
        /// <param name="worksheetName">工作表名称</param>
        /// <param name="excelAddress">开始填充数据Excel地址（例如：B2)</param>
        /// <param name="dvData">待填充数据</param>
        /// <param name="isPrintHeaders">是否显示数据源列名</param>
        /// <returns></returns>
        public static MemoryStream DocumentBuilder(string worksheetName, string excelAddress, System.Data.DataView dvData)
        {
            excelAddress.NullCheck("工作薄名称不能为空");

            MemoryStream createExcelStream = new MemoryStream();
            WorkBook workbook = WorkBook.CreateNew();
            WorkSheet worksheet = workbook.Sheets["sheet1"];
            if (worksheetName.IsNotEmpty())
                worksheet.Name = worksheetName;

            dvData.Table.TableName = string.Empty;

            worksheet.LoadFromDataView(CellAddress.Parse(excelAddress), ExcelTableStyles.None, dvData, null);
            //worksheet.LoadFromDataTable(excelAddress, dvData);
            workbook.Save(createExcelStream);

            return createExcelStream;
        }

        /// <summary>
        /// 创建文档，并将数填充到ExcelTable中
        /// </summary>
        /// <param name="worksheetName">工作表名称</param>
        /// <param name="beginAddress">开始单元格</param>
        /// <param name="tableName">ExcelTable名称</param>
        /// <param name="dvData">数据源</param>
        /// <param name="tableStyle">ExcelTable 样式</param>
        /// <param name="isPrintHeaders"></param>
        /// <returns></returns>
        public static byte[] CreateDocumentAndTable(string worksheetName, string beginAddress, string tableName, System.Data.DataView dvData, ExcelTableStyles tableStyle)
        {
            WorkBook workbook = WorkBook.CreateNew();
            WorkSheet worksheet = workbook.Sheets["sheet1"];
            if (worksheetName.IsNotEmpty())
                worksheet.Name = worksheetName;

            dvData.Table.TableName = tableName;
            worksheet.LoadFromDataView(CellAddress.Parse(beginAddress), tableStyle, dvData, null);
            //worksheet.LoadFromDataView(beginAddress, dvData, tableName, tableStyle);

            return workbook.SaveAsBytes();
        }

        /// <summary>
        /// 创建一个带计算公式的ExcelTable模版
        /// </summary>
        /// <param name="tableName">工作表名称</param>
        /// <param name="beginAddress">开始址址</param>
        /// <param name="colums">字典存储，key,列名，value，计算公式
        /// (例如:"ROUND(IF(OR([@社保缴纳类别值]=1,[@社保缴纳类别值]=3,[@社保缴纳类别值]=5), [@医疗有效基数], 0)*0.008,2)");
        /// </param>
        /// <returns></returns>
        public static byte[] CreateFormulaTableTemplate(string tableName, string beginAddress, IDictionary<string, string> colums)
        {
            WorkBook workbook = WorkBook.CreateNew();
            WorkSheet worksheet = workbook.Sheets["sheet1"];
            worksheet.Tables.Add(tableName, beginAddress, colums);

            return workbook.SaveAsBytes();
        }

        /// <summary>
        /// 根据指定工作表上的ExcelTable名称，填充数据
        /// </summary>
        /// <param name="input">Excel模版流文件</param>
        /// <param name="dv">待填充数据</param>
        /// <param name="worksheetName">工作薄名称</param>
        /// <param name="tableName">ExcelTable名称</param>
        /// <returns></returns>
        public static byte[] FillExcelTable(Stream input, DataView dv, string worksheetName, string tableName)
        {
            dv.NullCheck("数据源不能为空！");
            input.NullCheck("数据模板为空！");
            WorkBook workbook = WorkBook.Load(input);

            WorkSheet worksheet = workbook.Sheets[worksheetName];
            worksheet.NullCheck(string.Format("不存在指定的{0}工作薄！", worksheetName));
            Table excelTable = worksheet.Tables[tableName];
            excelTable.NullCheck(string.Format("不存在指定的{0}Excel表格！", tableName));

            excelTable.Rows.Clear();
            excelTable.FillData(dv);

            return workbook.SaveAsBytes();
        }

        /// <summary>
        /// 将数据返回指定Excel中指定工作薄上，指定ExcelTabel的数据
        /// </summary>
        /// <param name="input">Excel文件流</param>
        /// <param name="worksheetName">Excel工作薄名称</param>
        /// <param name="tableName">ExcelTable名称</param>
        /// <returns></returns>
        public static DataTable GetExcelTableData(Stream input, string worksheetName, string tableName)
        {
            input.NullCheck("数据模板为空！");
            WorkBook workbook = WorkBook.Load(input);
            WorkSheet worksheet = workbook.Sheets[worksheetName];
            worksheet.NullCheck(string.Format("不存在指定的{0}工作薄！", worksheetName));
            Table table = worksheet.Tables[tableName];
            table.NullCheck(string.Format("不存在指定的{0}Excel表格！", tableName));

            return table.AsDataTable();
        }

        /// <summary>
        /// 将Excel文件中开始到结束位置的数据转换成DataTable
        /// </summary>
        /// <param name="fileStream">Excel文件流</param>
        /// <param name="workSheetName">工作表名称</param>
        /// <param name="beginAddress">开始位置</param>
        /// <param name="endAddress">结束位置</param>
        /// <returns></returns>
        public static DataTable GetRangeValues(Stream fileStream, string workSheetName, string beginAddress, string endAddress)
        {
            DataTable dt = new DataTable("RangValue");
            WorkBook workbook = WorkBook.Load(fileStream);
            WorkSheet worksheet = workbook.Sheets[workSheetName];
            CellAddress begionCell = CellAddress.Parse(beginAddress);
            CellAddress endCell = CellAddress.Parse(endAddress);

            for (int i = begionCell.ColumnIndex; i <= endCell.ColumnIndex; i++)
            {
                dt.Columns.Add(new DataColumn(ExcelHelper.GetColumnLetterFromNumber(i)));
            }
            for (int j = begionCell.RowIndex; j <= endCell.RowIndex; j++)
            {
                DataRow dr = dt.NewRow();
                int temCol = begionCell.ColumnIndex;
                foreach (DataColumn dc in dt.Columns)
                {
                    dr[dc.ColumnName] = worksheet.Cells[j, temCol].Value;
                    temCol++;
                }
                dt.Rows.Add(dr);
            }

            return dt;
        }

        /// <summary>
        /// 指定起始单元格，提取Excel文件数据，起始行使用Excel名称定义
        /// </summary>
        /// <param name="fileStream">Excel文件流</param>
        /// <param name="workSheetName">工作表名称</param>
        /// <param name="beginAddress">开始位置</param>
        /// <param name="throwException">数据不存在时是否抛出异常</param>
        /// <returns>返回首行创建成TableHeader</returns>
        public static DataTable GetRangeValuesAsTable(Stream fileStream, string workSheetName, string beginAddress, bool throwException = false)
        {
            WorkBook workbook = WorkBook.Load(fileStream);

            return GetRangeValuesAsTable(workbook, workSheetName, beginAddress, throwException);
        }

        /// <summary>
        /// 指定起始单元格，提取Excel文件数据，起始行使用Excel名称定义
        /// </summary>
        /// <param name="workbook">工作簿对象</param>
        /// <param name="workSheetName">工作表名称</param>
        /// <param name="beginAddress">开始位置</param>
        /// <param name="throwException">数据不存在时是否抛出异常</param>
        /// <returns>返回首行创建成TableHeader</returns>
        public static DataTable GetRangeValuesAsTable(WorkBook workbook, string workSheetName, string beginAddress, bool throwException = false)
        {
            workbook.NullCheck("workbook");
            workSheetName.CheckStringIsNullOrEmpty("workSheetName");

            WorkSheet sheet = workbook.Sheets[workSheetName];

            sheet.NullCheck(string.Format("不存在指定的{0}工薄！", workSheetName));

            return GetRangeValuesAsTable(sheet, beginAddress, throwException);
        }

        /// <summary>
        /// 从WorkSheet提取DataTable
        /// </summary>
        /// <param name="sheet">工作表对象</param>
        /// <param name="beginAddress">开始位置</param>
        /// <param name="throwException">数据不存在时是否抛出异常</param>
        /// <returns>返回首行创建成TableHeader</returns>
        public static DataTable GetRangeValuesAsTable(WorkSheet sheet, string beginAddress, bool throwException = false)
        {
            sheet.NullCheck("workbook");

            DataTable dt = new DataTable("RangValue");

            CellAddress begionCell = CellAddress.Parse(beginAddress);
            CellAddress endCell = CellAddress.Parse(sheet.Dimension.EndColumn, sheet.Dimension.EndRow);

            for (int i = begionCell.ColumnIndex; i <= endCell.ColumnIndex; i++)
            {
                var headCellAdress = CellAddress.Parse(i, begionCell.RowIndex);
                var namedRange = sheet.Names.FirstOrDefault(p => p.Address.StartRow == headCellAdress.RowIndex &&
                    p.Address.StartRow == p.Address.EndRow &&
                    p.Address.EndColumn == headCellAdress.ColumnIndex &&
                    p.Address.StartColumn == p.Address.EndColumn);

                if (namedRange == null)
                {
                    object objValue = sheet.Cells[begionCell.RowIndex, i].Value;

                    if (objValue != null)
                        dt.Columns.Add(new DataColumn(objValue.ToString()));
                }
                else
                {
                    dt.Columns.Add(new DataColumn(namedRange.Name));
                }
            }

            for (int j = begionCell.RowIndex + 1; j <= endCell.RowIndex; j++)
            {
                DataRow dr = dt.NewRow();
                int temCol = begionCell.ColumnIndex;
                int colIndex = 0;

                while (temCol <= endCell.ColumnIndex && colIndex < dt.Columns.Count)
                {
                    dr[colIndex] = sheet.Cells[j, temCol].Value;
                    temCol++;
                    colIndex++;
                }

                bool isEmptyRow = true;
                for (int i = 0; i < dt.Columns.Count; i++)
                {
                    if (!string.IsNullOrEmpty(dr[i].ToString()))
                    {
                        isEmptyRow = false;
                        break;
                    }
                }

                if (isEmptyRow == false)
                    dt.Rows.Add(dr);
            }

            ExceptionHelper.TrueThrow(throwException && dt.Rows.Count == 0, "未在Excel中找到数据");

            return dt;
        }

        /// <summary>
        /// 根据WorkSheet定义单元格名称，转换成DataView
        /// </summary>
        /// <param name="fileSource">Excel文件流</param>
        /// <param name="sheetName">工作薄名称</param>
        /// <returns></returns>
        public static DataView ByDefinedNameExportSheetData(Stream fileSource, string sheetName)
        {
            DataTable dt = new DataTable();
            WorkBook workbook = WorkBook.Load(fileSource);
            WorkSheet worksheet = workbook.Sheets[sheetName];
            Dictionary<string, int> excelDataViewHeader = new Dictionary<string, int>();
            int beginRowIndex = CreateBuyDefinedNameExportSheetDataTableHeader(worksheet, dt, excelDataViewHeader);

            beginRowIndex++;
            for (int i = beginRowIndex; i <= worksheet.Dimension.EndRow; i++)
            {
                DataRow dr = dt.NewRow();
                foreach (KeyValuePair<string, int> dataKey in excelDataViewHeader)
                {
                    dr[dataKey.Key] = worksheet.Cells[i, dataKey.Value].Value;
                }
                dt.Rows.Add(dr);
            }

            return dt.DefaultView;
        }

        private static int CreateBuyDefinedNameExportSheetDataTableHeader(WorkSheet worksheet, DataTable dt, Dictionary<string, int> excelDataViewHeader)
        {
            int beginRowIndex = 0;
            bool isGetBeginRowindex = true;
            foreach (DefinedName nameCell in worksheet.Names)
            {
                if (nameCell.Address.StartColumn == nameCell.Address.EndColumn && nameCell.Address.StartRow == nameCell.Address.EndRow)
                {
                    DataColumn dc = new DataColumn(nameCell.Name);
                    dc.Caption = nameCell.NameValue.ToString();
                    dt.Columns.Add(dc);
                    excelDataViewHeader.Add(nameCell.Name, nameCell.Address.StartColumn);

                    if (isGetBeginRowindex)
                        beginRowIndex = nameCell.Address.StartRow;
                }
            }

            return beginRowIndex;
        }
    }
}
