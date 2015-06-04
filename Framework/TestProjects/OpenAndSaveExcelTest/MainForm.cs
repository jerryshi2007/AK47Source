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

                string dir = Path.GetDirectoryName(Application.ExecutablePath);

                workbook.Save(Path.Combine(dir, "output.xlsx"));
            }
        }
    }
}
