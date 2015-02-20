using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using MCS.Library.Core;
using MCS.Web.Library;
using MCS.Web.WebControls;
using MCS.Library.Office;
using MCS.Library.Office.SpreadSheet;
using MCS.Library.SOA.DataObjects.Workflow;
using System.Text;
using System.IO;

namespace MCS.Library.SOA.Web.WebControls.Test.WfMatrixTest
{
	public partial class OutputDimensionMatrix : System.Web.UI.Page
	{
		protected void Page_Load(object sender, EventArgs e)
		{

		}

		protected void generateButton_Click(object sender, EventArgs e)
		{
			WorkbookNode workbook = CreateMatrixDefinition().ExportToExcelXml();

			workbook.Response("Matrix");
		}

		protected void generateMatrixButton_Click(object sender, EventArgs e)
		{
			WfMatrix matrix = CreateMatrix();

			WorkbookNode workbook = matrix.ExportToExcelXml(false);

			workbook.Response("Matrix");
		}

		private static WfMatrix CreateMatrix()
		{
			WfMatrix matrix = new WfMatrix(CreateMatrixDefinition());

			AddMatrixRow(matrix, "1001", "0");
			AddMatrixRow(matrix, "1002", "1");
			AddMatrixRow(matrix, "1001", "0");
			AddMatrixRow(matrix, "1002", "1");

			return matrix;
		}

		private static WfMatrixRow AddMatrixRow(WfMatrix matrix, params string[] values)
		{
			WfMatrixRow row = new WfMatrixRow(matrix);

			row.RowNumber = matrix.Rows.Count;

			for (int i = 0; i < matrix.Definition.Dimensions.Count; i++)
			{
				WfMatrixDimensionDefinition dd = matrix.Definition.Dimensions[i];

				WfMatrixCell cell = new WfMatrixCell(dd);

				if (i < values.Length)
					cell.StringValue = values[i];

				row.Cells.Add(cell);
			}

			matrix.Rows.Add(row);

			return row;
		}

		private static WfMatrixDefinition CreateMatrixDefinition()
		{
			WfMatrixDefinition md = new WfMatrixDefinition();

			md.Name = "测试矩阵";
			md.Key = "TestMatrix";

			WfMatrixDimensionDefinition ddConstCenter = new WfMatrixDimensionDefinition();

			ddConstCenter.DimensionKey = "ConstCenter";
			ddConstCenter.Name = "成本中心";

			md.Dimensions.Add(ddConstCenter);

			WfMatrixDimensionDefinition ddPaymentMethod = new WfMatrixDimensionDefinition();

			ddPaymentMethod.DimensionKey = "PaymentMethod";
			ddPaymentMethod.Name = "付款方式";

			md.Dimensions.Add(ddPaymentMethod);

			return md;
		}

		protected void uploadProgress_DoUploadProgress(HttpPostedFile file, UploadProgressResult result)
		{
			ExceptionHelper.FalseThrow(Path.GetExtension(file.FileName).ToLower() == ".xml",
				"'{0}' must be a xml file.", file.FileName);

			WorkbookNode workbook = new WorkbookNode();

			workbook.Load(file.InputStream);

			ExceptionHelper.FalseThrow(workbook.Worksheets.Contains("Matrix"),
				"The workbook must contains a 'Matrix' worksheet.");

			NamedLocationCollection fieldLocations = workbook.Names.ToLocations();

			TableNode table = workbook.Worksheets["Matrix"].Table;

			StringBuilder strB = new StringBuilder();

			int baseRowIndex = GetStartRow(fieldLocations);

			RowNode titleRow = table.GetRowByIndex(baseRowIndex);

			int currentRowIndex = table.Rows.IndexOf(titleRow) + 1;

			if (table.Rows.Count > currentRowIndex)
			{
				UploadProgressStatus status = new UploadProgressStatus();

				status.CurrentStep = 1;
				status.MinStep = 0;
				status.MaxStep = table.Rows.Count - currentRowIndex;

				int currentVirtualRow = baseRowIndex;

				for (int i = status.MinStep; i < status.MaxStep; i++)
				{
					RowNode row = table.Rows[currentRowIndex];

					if (row.Index > 0)
						currentVirtualRow = row.Index;
					else
						currentVirtualRow++;

					if (strB.Length > 0)
						strB.Append("\n");

					strB.AppendFormat("Processed={0}, Row={1}:", (i + 1), currentVirtualRow);

					foreach (CellLocation location in fieldLocations)
					{
						CellNode cell = row.GetCellByIndex(location.Column);

						strB.AppendFormat(";Name={0}, Value={1}", location.Name, cell != null ? cell.Data.Value : string.Empty);
					}

					status.CurrentStep = i;
					status.Response();

					currentRowIndex++;
				}

				status.CurrentStep = status.MaxStep;
				status.Response();
			}

			result.DataChanged = true;
			result.CloseWindow = false;
			result.ProcessLog = strB.ToString();
		}

		private static int GetStartRow(NamedLocationCollection locations)
		{
			int result = 0;

			if (locations.Count > 0)
				result = locations[0].Row;

			return result;
		}
	}
}