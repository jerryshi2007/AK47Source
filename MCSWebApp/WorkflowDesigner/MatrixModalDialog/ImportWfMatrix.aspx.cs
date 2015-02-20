using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using MCS.Web.WebControls;
using MCS.Library.Core;
using MCS.Library.Office.SpreadSheet;
using MCS.Library.SOA.DataObjects.Workflow;
using System.IO;
using System.Threading;

namespace WorkflowDesigner.MatrixModalDialog
{
	public partial class ImportWfMatrix : System.Web.UI.Page
	{
		protected void Page_Load(object sender, EventArgs e)
		{
			Response.Write(Request["processkey"] + "|" + Request["activitykey"]);
		}

		protected void uploadProgress_DoUploadProgress(HttpPostedFile file, UploadProgressResult result)
		{
			ExceptionHelper.FalseThrow(Path.GetExtension(file.FileName).ToLower() == ".xml", "'{0}'权限矩阵必须是xml电子表格", file.FileName);

			UploadProgressStatus status = new UploadProgressStatus();
			status.CurrentStep = 1;
			status.MinStep = 0;
			status.MaxStep = 20;

			WfMatrix.ImportExistMatrixFromExcelXml(file.InputStream, () =>
			{
				if (status.CurrentStep + 1 < status.MaxStep)
				{
					status.CurrentStep++;
				}
				status.Response();
			}, Request["processkey"]);

			status.MaxStep = 20;
			status.Response();

			string logInfo = string.Format("导入完成");
			PrepareResultInfo(result, logInfo);
		}

		private static void PrepareResultInfo(UploadProgressResult result, string logInfo)
		{
			result.DataChanged = true;
			result.CloseWindow = false;
			result.ProcessLog = logInfo;
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

	}
}