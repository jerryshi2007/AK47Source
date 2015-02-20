using MCS.Library.Office.OpenXml.Excel;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ExcelOpenXmlTest
{
    public static class OutputHelper
    {
        public static void Output(this WorkSheet sheet, int startRowIndex, int startColumnIndex)
        {
            if (sheet != null)
            {
                Cell startCell = sheet.Cells[startRowIndex, startColumnIndex];

                for (int r = startCell.Row.Index; r <= sheet.Rows.Count; r++)
                {
                    for (int c = startCell.Column.Index; c <= sheet.Columns.Count; c++)
                    {
                        Cell cell = sheet.Cells[r + 0, c + 0];
                        Console.Write("Row: {0}, Col: {1}, Address: {2}, Text: {3} ", r, c, cell.ToAddress(), cell.Value);
                    }

                    Console.WriteLine();
                }
            }
        }

        public static void Output(this DataTable table)
        {
            if (table != null)
            {
                for (int i = 0; i < table.Rows.Count; i++)
                {
                    Console.Write("Row Index: {0} ", i);

                    DataRow row = table.Rows[i];

                    foreach (DataColumn column in table.Columns)
                        Console.Write("{0} ", row[column.ColumnName].ToString());

                    Console.WriteLine();
                }
            }
        }

        public static void OutputColumnsInfo(this Table table)
        {
            if (table != null)
            {
                foreach (TableColumn column in table.Columns)
                    Console.Write("Column Name: {0} ", column.Name);

                Console.WriteLine();
            }
        }

        public static void OutputColumnInfo(this DataTable table)
        {
            if (table != null)
            {
                foreach (DataColumn column in table.Columns)
                    Console.Write("Column Name: {0}, {1} ", column.ColumnName, column.DataType.Name);

                Console.WriteLine();
            }
        }
    }
}
