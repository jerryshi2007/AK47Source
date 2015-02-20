using MCS.Library.Office.OpenXml.Excel;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;

namespace ExcelOpenXmlTest
{
    [TestClass]
    public class WriteWorkBookTest
    {
        public TestContext TestContext
        {
            get;
            set;
        }

        [TestMethod]
        public void WriteWorkBook()
        {
            WorkBook wb = WorkBook.CreateNew();

            wb.Sheets["sheet1"].Name = "FirstSheet";

            wb.Save("WriteWorkBook.xlsx");

            WorkBook loaded = WorkBook.Load("WriteWorkBook.xlsx");

            Assert.IsNotNull(loaded.Sheets["FirstSheet"]);
        }

        [TestMethod]
        public void WriteCellsToWorkBook()
        {
            WorkBook wb = WorkBook.CreateNew();

            WorkSheet sheet = wb.Sheets["sheet1"];

            sheet.Name = "FirstSheet";

            for (int r = 1; r <= 2000; r++)
            {
                for (int c = 1; c <= 50; c++)
                {
                    sheet.Cells[r, c].Value = CellAddress.Parse(c, r).ToString();
                }
            }

            wb.Save("WriteCellsToWorkBook.xlsx");
        }

        [TestMethod]
        public void WriteDataTableToCells()
        {
            WorkBook wb = WorkBook.CreateNew();

            WorkSheet sheet = wb.Sheets["sheet1"];

            sheet.Name = "FirstSheet";

            DataTable table = PrepareDataTable();

            FillDataTableData(table, 10240);

            sheet.LoadFromDataView(table.DefaultView);

            wb.Save("WriteDateTableToCells.xlsx");

            CheckColumnDataType("WriteDateTableToCells.xlsx", table);
        }

        [TestMethod]
        public void WriteBookWithAttributesToCells()
        {
            WorkBook wb = WorkBook.CreateNew();

            WorkSheet sheet = wb.Sheets["sheet1"];

            sheet.Name = "FirstSheet";

            IEnumerable<BookWithAttributes> books = FillBookWithAttributesData(10240);

            sheet.LoadFromCollection(books);

            wb.Save("WriteBookWithAttributesToCells.xlsx");
        }

        [TestMethod]
        public void WriteBookWithoutAttributesToCells()
        {
            WorkBook wb = WorkBook.CreateNew();

            WorkSheet sheet = wb.Sheets["sheet1"];

            sheet.Name = "FirstSheet";

            IEnumerable<BookWithoutAttributes> books = FillBookWithoutAttributesData(10240);

            TableDescription tableDesp = new TableDescription("Books");

            tableDesp.AllColumns.Add(new TableColumnDescription(new DataColumn("书名", typeof(string))) { PropertyName = "Name" });
            tableDesp.AllColumns.Add(new TableColumnDescription(new DataColumn("价格", typeof(double))) { PropertyName = "Price" });
            tableDesp.AllColumns.Add(new TableColumnDescription(new DataColumn("发行日期", typeof(DateTime))) { PropertyName = "IssueDate" });

            sheet.LoadFromCollection(books, tableDesp, (cell, dcp) =>
            {
                cell.Value = dcp.PropertyValue;
            });

            wb.Save("WriteBookWithoutAttributesToCells.xlsx");
        }

        private static DataTable PrepareDataTable()
        {
            DataTable table = new DataTable("Books");

            table.Columns.Add(new DataColumn("Book", typeof(string)));
            table.Columns.Add(new DataColumn("Price", typeof(double)));
            table.Columns.Add(new DataColumn("IssueDate", typeof(DateTime)));

            return table;
        }

        private static void FillDataTableData(DataTable table, int count)
        {
            Random rnd = new Random((int)DateTime.Now.Ticks);

            for (int i = 0; i < count; i++)
            {
                DataRow row = table.NewRow();

                row["Book"] = string.Format("Book: {0}", i);
                row["Price"] = 50 + rnd.Next(25) - 25;
                row["IssueDate"] = DateTime.Now.AddDays(rnd.Next(100));

                table.Rows.Add(row);
            }
        }

        private IEnumerable<BookWithAttributes> FillBookWithAttributesData(int count)
        {
            Random rnd = new Random((int)DateTime.Now.Ticks);

            List<BookWithAttributes> books = new List<BookWithAttributes>();

            for (int i = 0; i < count; i++)
            {
                BookWithAttributes book = new BookWithAttributes();

                FillBookData(book, rnd, i);

                books.Add(book);
            }

            return books;
        }

        private IEnumerable<BookWithoutAttributes> FillBookWithoutAttributesData(int count)
        {
            Random rnd = new Random((int)DateTime.Now.Ticks);

            List<BookWithoutAttributes> books = new List<BookWithoutAttributes>();

            for (int i = 0; i < count; i++)
            {
                BookWithoutAttributes book = new BookWithoutAttributes();

                FillBookData(book, rnd, i);

                books.Add(book);
            }

            return books;
        }

        private static void FillBookData(BookBase book, Random rnd, int i)
        {
            book.Name = string.Format("Book: {0}", i);
            book.Price = 50 + rnd.Next(25) - 25;
            book.IssueDate = DateTime.Now.AddDays(rnd.Next(100));
        }

        private static void CheckColumnDataType(string fileName, DataTable sourceTable)
        {
            WorkBook wb = WorkBook.Load(fileName);

            Table table = wb.Sheets.First().Tables[0];

            DataTable destTable = table.AsDataTable();

            foreach (DataColumn sourceColumn in sourceTable.Columns)
            {
                DataColumn destColumn = destTable.Columns[sourceColumn.ColumnName];

                Assert.IsNotNull(destColumn);

                Assert.AreEqual(sourceColumn.DataType, destColumn.DataType);
            }
        }
    }
}
