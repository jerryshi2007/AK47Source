using MCS.Library.Office.OpenXml.Excel;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace OpenAndSaveExcelTest
{
    public partial class MainForm : Form
    {
        public MainForm()
        {
            InitializeComponent();
        }

        private void buttonOpenAndSaveFile_Click(object sender, EventArgs e)
        {
            if (openExceFileDialog.ShowDialog() == DialogResult.OK)
            {
                WorkBook workbook = WorkBook.Load(openExceFileDialog.FileName);

                DataTable dt = PrepareDataTable();
                FillDataTableData(dt, 10);

                WorkSheet sheet = workbook.Sheets["任务单"];

                if (sheet != null && sheet.Tables.Count > 0)
                {
                    Table table = sheet.Tables[0];

                    table.FillData(dt.DefaultView);
                    //table.FillData(dt.DefaultView, (cell, cellParameters) =>
                    //{
                    //    cell.Value = cellParameters.PropertyValue;
                    //    cell.Style.Font.Color.SetColor(Color.Blue);
                    //});
                }

                string dir = Path.GetDirectoryName(Application.ExecutablePath);

                string path = Path.Combine(dir, "output.xlsx");
                workbook.Save(path);

                Shell32.Shell shell = new Shell32.Shell();

                shell.Open(path);
            }
        }

        private static DataTable PrepareDataTable()
        {
            DataTable table = new DataTable("表1");

            table.Columns.Add(new DataColumn("员工工号", typeof(string)));
            table.Columns.Add(new DataColumn("考勤项目", typeof(string)));
            table.Columns.Add(new DataColumn("年度", typeof(string)));
            table.Columns.Add(new DataColumn("法定假转入", typeof(decimal)));
            table.Columns.Add(new DataColumn("公司福利假转入", typeof(decimal)));
            table.Columns.Add(new DataColumn("法定假初始值", typeof(decimal)));
            table.Columns.Add(new DataColumn("公司福利假初始值", typeof(decimal)));

            return table;
        }

        private static void FillDataTableData(DataTable table, int count)
        {
            string[] complexity = new string[] { "简单", "中等", "复杂" };

            Random rnd = new Random((int)DateTime.Now.Ticks);

            for (int i = 0; i < count; i++)
            {
                DataRow row = table.NewRow();

                row["员工工号"] = "员工工号" + rnd.Next();
                row["考勤项目"] = "L14";
                row["年度"] = "2015";
                row["法定假转入"] = 0;
                row["公司福利假转入"] = 10;
                row["法定假初始值"] = 2;
                row["公司福利假初始值"] = 3;

                table.Rows.Add(row);
            }
        }
    }
}
