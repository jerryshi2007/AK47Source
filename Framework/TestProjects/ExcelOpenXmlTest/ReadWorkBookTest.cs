using MCS.Library.Office.OpenXml.Excel;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Data;

namespace ExcelOpenXmlTest
{
    [TestClass]
    public class ReadWorkBookTest
    {
        [TestMethod]
        public void OpenWorkBookTest()
        {
            WorkBook wb = LoadBooksFile();

            foreach (WorkSheet sheet in wb.Sheets)
                Console.WriteLine(sheet.Name);
        }

        [TestMethod]
        public void ReadAllContent()
        {
            WorkSheet sheet = GetFirstBooksSheet();

            Console.WriteLine("Rows: {0}, Cols: {1}", sheet.Rows.Count, sheet.Columns.Count);

            sheet.Output(1, 1);
        }

        [TestMethod]
        public void ReadContentFromBookmark()
        {
            WorkSheet sheet = GetFirstBooksSheet();

            Assert.IsNotNull(sheet.Names["Content"]);

            DefinedName bookmark = sheet.Names["Content"];

            sheet.Output(bookmark.Address.StartRow, bookmark.Address.StartColumn);
        }

        [TestMethod]
        public void ReadTableContent()
        {
            WorkSheet sheet = GetTableSheet();

            Table table = sheet.Tables["BooksTable"];

            Assert.IsNotNull(table);

            table.OutputColumnsInfo();

            sheet.Output(table.Address.StartRow, table.Address.StartColumn);
        }

        [TestMethod]
        public void ReadTableAsDataTable()
        {
            WorkSheet sheet = GetTableSheet();

            Table table = sheet.Tables["BooksTable"];

            Assert.IsNotNull(table);

            DataTable dt = table.AsDataTable();

            dt.OutputColumnInfo();
            dt.Output();
        }

        private static WorkBook LoadBooksFile()
        {
            WorkBook wb = WorkBook.Load("books.xlsx");

            Assert.IsNotNull("FirstSheet");

            return wb;
        }

        private static WorkSheet GetFirstBooksSheet()
        {
            WorkBook wb = WorkBook.Load("books.xlsx");

            Assert.IsNotNull("FirstSheet");

            return wb.Sheets["FirstSheet"];
        }

        private static WorkSheet GetTableSheet()
        {
            WorkBook wb = WorkBook.Load("books.xlsx");

            Assert.IsNotNull("TableSheet");

            return wb.Sheets["TableSheet"];
        }
    }
}
