using MCS.Library.Core;
using MCS.Library.Office.OpenXml.Excel;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace OpenAndSaveExcelTest
{
    public partial class MainForm : Form
    {
        public MainForm()
        {
            InitializeComponent();

            Application.ThreadException += Application_ThreadException;
        }

        private void buttonOpenAndSaveFile_Click(object sender, EventArgs e)
        {
            if (openExceFileDialog.ShowDialog() == DialogResult.OK)
            {
                WorkBook workbook = WorkBook.Load(openExceFileDialog.FileName);

                DataTable dt = PrepareDataTable();
                FillDataTableData(dt, 10);

                WorkSheet sheet = workbook.Sheets["任务单"];

                (sheet != null).FalseThrow("不能找到名称为\"{0}\"工作表", "任务单");

                Table table = sheet.Tables["表1"];

                (table != null).FalseThrow("不能找到名称为\"{0}\"表格", "表");

                table.FillData(dt.DefaultView);
                //table.FillData(dt.DefaultView, (cell, cellParameters) =>
                //{
                //    cell.Value = cellParameters.PropertyValue;
                //    cell.Style.Font.Color.SetColor(Color.Blue);
                //});

                string dir = Path.GetDirectoryName(Application.ExecutablePath);

                string path = Path.Combine(dir, "output.xlsx");
                workbook.Save(path);

                Shell32.Shell shell = new Shell32.Shell();

                shell.Open(path);
            }
        }

        private static DataTable PrepareDataTable()
        {
            DataTable table = new DataTable("Table1");

            table.Columns.Add(new DataColumn("客户编号", typeof(string)));
            table.Columns.Add(new DataColumn("任务单编号", typeof(double)));
            table.Columns.Add(new DataColumn("业务复杂度", typeof(string)));

            return table;
        }

        private static void FillDataTableData(DataTable table, int count)
        {
            string[] complexity = new string[] { "简单", "中等", "复杂" };

            Random rnd = new Random((int)DateTime.Now.Ticks);

            for (int i = 0; i < count; i++)
            {
                DataRow row = table.NewRow();

                row["客户编号"] = string.Format("Book: {0}", i);
                row["任务单编号"] = 50 + rnd.Next(25) - 25;
                row["业务复杂度"] = complexity[rnd.Next(complexity.Length)];

                table.Rows.Add(row);
            }
        }

        private void Application_ThreadException(object sender, ThreadExceptionEventArgs e)
        {
            MessageBox.Show(this, e.Exception.ToString());
        }
    }
}
