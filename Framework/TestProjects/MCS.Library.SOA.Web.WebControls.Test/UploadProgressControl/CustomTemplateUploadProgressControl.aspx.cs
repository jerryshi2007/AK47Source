using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using MCS.Library.Core;
using MCS.Library.Office.SpreadSheet;
using MCS.Web.WebControls;
using System.Reflection;

namespace MCS.Library.SOA.Web.WebControls.Test.UploadProgressControl
{
	public partial class CustomTemplateUploadProgressControl : System.Web.UI.Page
	{
		protected void Page_Load(object sender, EventArgs e)
		{

		}

		protected void uploadProgress_DoUploadProgress(HttpPostedFile file, UploadProgressResult result)
		{
			string tag = uploadProgress.Tag;

			const int baseRowIndex = 3;

			ExceptionHelper.FalseThrow(Path.GetExtension(file.FileName).ToLower() == ".xml",
				"'{0}' must be a xml file.", file.FileName);

			WorkbookNode workbook = new WorkbookNode();

			workbook.Load(file.InputStream);

			ExceptionHelper.FalseThrow(workbook.Worksheets.Contains("PC Tracker Form"),
				"The workbook must contains a 'PC Tracker Form' worksheet.");

			Dictionary<string, CellLocation> fieldLocations = BuildNameColumnDictionary(workbook);

			TableNode table = workbook.Worksheets["PC Tracker Form"].Table;

			StringBuilder strB = new StringBuilder();

			if (table.Rows.Count > 3)
			{
				int currentRowIndex = baseRowIndex;

				UploadProgressStatus status = new UploadProgressStatus();

				status.CurrentStep = 1;
				status.MinStep = 0;
				status.MaxStep = table.Rows.Count - currentRowIndex;

				int currentVirtualRow = baseRowIndex;

				for (int i = status.MinStep; i < status.MaxStep; i++)
				{
					currentRowIndex = baseRowIndex + i;

					RowNode row = table.Rows[currentRowIndex];

					if (row.Index > 0)
						currentVirtualRow = row.Index;
					else
						currentVirtualRow++;

					if (strB.Length > 0)
						strB.Append("\n");

					strB.AppendFormat("Processed={0}, Row={1}:", (i + 1), currentVirtualRow);

					foreach (KeyValuePair<string, CellLocation> kp in fieldLocations)
					{
						CellNode cell = row.GetCellByIndex(kp.Value.Column);

						strB.AppendFormat(";Name={0}, Value={1}", kp.Key, cell != null ? cell.Data.Value : string.Empty);
					}

					status.CurrentStep = i;
					status.Response();

					int ms = 1000;

					if (Request.Form["longProgress"] == "true")
						ms = 2000;

					Thread.Sleep(ms);	//假装等待
				}

				status.CurrentStep = status.MaxStep;
				status.Response();
			}

			result.DataChanged = true;
			result.CloseWindow = false;
			result.ProcessLog = strB.ToString();
		}

		private static Dictionary<string, CellLocation> BuildNameColumnDictionary(WorkbookNode workbook)
		{
			Dictionary<string, CellLocation> result = new Dictionary<string, CellLocation>();

			foreach (NamedRangeNode nr in workbook.Names)
			{
				result.Add(nr.Name, nr.ParseReferTo());
			}

			return result;
		}

		protected void uploadProgress_BeforeNormalPreRender(object sender, EventArgs e)
		{
			uploadProgress.Tag = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss.fff");
		}

		protected void uploadProgress_LoadingDialogContent(object sender, LoadingDialogContentEventArgs eventArgs)
		{
			eventArgs.Content = ResourceHelper.LoadStringFromResource(Assembly.GetExecutingAssembly(),
				"MCS.Library.SOA.Web.WebControls.Test.UploadProgressControl.customUploadProgressControlTemplate.htm");
		}
	}
}