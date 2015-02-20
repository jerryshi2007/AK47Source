using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using MCS.Library.SOA.DataObjects.Workflow;
using MCS.Library.Office.SpreadSheet;
using MCS.Web.WebControls;
using System.Net.Mime;
using System.IO;

namespace WorkflowDesigner.MatrixModalDialog
{
	public partial class ExportMatrix : System.Web.UI.Page
	{
		private enum ExportType
		{
			Matrix,
			Definition
		}

		private enum ExportFormat
		{
			Xml,
			Xlsx
		}

		private ExportType ExType { get; set; }
		private bool RoleAsPerson { get; set; }
		private ExportFormat ExFormat { get; set; }
		private string ExportKey { get; set; }

		protected void Page_Load(object sender, EventArgs e)
		{
			Initialize();

			switch (this.ExType)
			{
				case ExportType.Definition:
					ExportMatrixDefinition();
					break;
				case ExportType.Matrix:
					ExportMatrixData();
					break;
			}
		}

		private void Initialize()
		{
			var roleAsPerson = false;

			if (string.IsNullOrEmpty(Request["key"]))
			{
				throw new ArgumentException("参数key无效！");
			}

			if (Request["cmd"] == "ExportMatrixDefinition")
			{
				this.ExType = ExportType.Definition;
			}
			else
			{
				this.ExType = ExportType.Matrix;
				if (!Boolean.TryParse(Request["roleAsPerson"], out roleAsPerson))
				{
					throw new ArgumentException("参数roleAsPerson无效！");
				}
			}

			if (Request["format"] == "xlsx")
			{
				this.ExFormat = ExportFormat.Xlsx;
			}
			else
			{
				this.ExFormat = ExportFormat.Xml;
			}

			this.RoleAsPerson = roleAsPerson;
			this.ExportKey = Request["key"];
		}

		private void ExportMatrixDefinition()
		{
			WfMatrixDefinition definition = WfMatrixDefinitionAdapter.Instance.Load(this.ExportKey);

			WorkbookNode workbook = definition.ExportToExcelXml();

			workbook.Response(definition.Key);
		}

		private void ExportMatrixData()
		{
			WfMatrix matrix = WfMatrixAdapter.Instance.Load(this.ExportKey);
			matrix.Loaded = true;

			switch (this.ExFormat)
			{
				case ExportFormat.Xlsx:
					Response.Clear();
					Response.ClearHeaders();
					using (MemoryStream fileSteam = matrix.ExportToExcel2007(this.RoleAsPerson))
					{
						fileSteam.CopyTo(Response.OutputStream);
					}
					//Response.BinaryWrite(bytes);
					//Response.ContentType = MediaTypeNames.Text.Xml;
					Response.ContentType = "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet";
					Response.AppendHeader("CONTENT-DISPOSITION", "attachment;filename=" + matrix.ProcessKey + ".xlsx");
					Response.Flush();
					Response.End();
					break;
				case ExportFormat.Xml:
					WorkbookNode workbook = matrix.ExportToExcelXml(this.RoleAsPerson);
					workbook.Response(matrix.ProcessKey);
					break;
			}
		}
	}
}