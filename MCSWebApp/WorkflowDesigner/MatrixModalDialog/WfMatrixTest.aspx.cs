using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using MCS.Library.Core;
using System.IO;
using MCS.Library.Office.SpreadSheet;
using MCS.Web.WebControls;
using MCS.Library.SOA.DataObjects.Workflow;
using System.Threading;

namespace WorkflowDesigner.MatrixModalDialog
{
	public partial class WfMatrixTest : System.Web.UI.Page
	{
		protected void uploadProgress_DoUploadProgress(HttpPostedFile file, UploadProgressResult result)
		{
			ExceptionHelper.FalseThrow(Path.GetExtension(file.FileName).ToLower() == ".xml", "'{0}'权限矩阵必须是xml电子表格", file.FileName);

			WorkbookNode workbook = new WorkbookNode();

			workbook.Load(file.InputStream);

			//矩阵在文件第1个sheet，sheet名称为矩阵引用的定义名称
			TableNode table = workbook.Worksheets[0].Table;

			var matrixDef = WfMatrixDefinitionAdapter.Instance.Load(workbook.Worksheets[0].Name);

			WfMatrix matrix = new WfMatrix(matrixDef) { MatrixID = Guid.NewGuid().ToString() };

			if (table.Rows.Count <= 1)
			{
				PrepareResultInfo(result, "没有数据或格式不正确");
				return;
			}

			UploadProgressStatus status = new UploadProgressStatus();
			status.CurrentStep = 1;
			status.MinStep = 0;
			status.MaxStep = table.Rows.Count;

			for (int i = status.CurrentStep; i < status.MaxStep; i++)
			{
				var newRow = new WfMatrixRow() { RowNumber = i };

				for (int j = 0; j < table.Rows[i].Cells.Count; j++)
				{
					if (table.Rows[0].Cells[j].Data.InnerText == "操作人")
					{
						newRow.Operator = table.Rows[i].Cells[j].Data.InnerText;
						continue;
					}

					var newCell = new WfMatrixCell(matrixDef.Dimensions[table.Rows[0].Cells[j].Data.InnerText])
					{
						StringValue = table.Rows[i].Cells[j].Data.InnerText
					};

					newRow.Cells.Add(newCell);
				}
				matrix.Rows.Add(newRow);

				status.CurrentStep = i;
				status.Response();
				Thread.Sleep(100);
			}

			status.CurrentStep = status.MaxStep;
			status.Response();

			WfMatrixAdapter.Instance.Update(matrix);

			string logInfo = string.Format("导入成功，权限矩阵名称：{0}{1}共导入{2}行数据.", matrix.MatrixID, Environment.NewLine, table.Rows.Count - 1);
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

		protected void Button1_Click(object sender, EventArgs e)
		{
			WfMatrixQueryParamCollection queryParams = new WfMatrixQueryParamCollection(this.txtID.Text.Trim());
			queryParams.Add(new WfMatrixQueryParam()
			{
				QueryName = "支付方式",
				QueryValue = txtPaymentType.Text.Trim()
			});

			queryParams.Add(new WfMatrixQueryParam()
			{
				QueryName = "成本中心",
				QueryValue = txtCostCenter.Text.Trim()
			});

			queryParams.Add(new WfMatrixQueryParam()
			{
				QueryName = "部门",
				QueryValue = txtDepartment.Text.Trim()
			});

			var matrix = WfMatrixAdapter.Instance.Query(queryParams);

			if (matrix == null)
			{
				Label1.Text = "未找到相应的数据";
			}
			else
			{
				Label1.Text = "";
				foreach (var row in matrix.Rows)
				{
					Label1.Text += row.OperatorType.ToString() + "/" + row.Operator + "<br/>";
				}
			}
		}
	}
}