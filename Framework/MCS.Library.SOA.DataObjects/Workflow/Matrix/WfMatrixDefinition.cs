using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MCS.Library.Data.DataObjects;
using MCS.Library.Data.Mapping;
using MCS.Library.Office.SpreadSheet;
using MCS.Library.Core;
using System.Reflection;

namespace MCS.Library.SOA.DataObjects.Workflow
{
	/// <summary>
	/// 授权矩阵定义
	/// </summary>
	[Serializable]
	[ORTableMapping("WF.MATRIX_DEFINITION")]
	public class WfMatrixDefinition
	{
		[ORFieldMapping("DEF_KEY", PrimaryKey = true)]
		public string Key
		{
			get;
			set;
		}

		[ORFieldMapping("NAME")]
		public string Name
		{
			get;
			set;
		}

		[ORFieldMapping("DESCRIPTION")]
		public string Description
		{
			get;
			set;
		}

		[ORFieldMapping("ENABLED")]
		public bool Enabled
		{
			get;
			set;
		}

		private WfMatrixDimensionDefinitionCollection _Dimensions = null;

		[NoMapping]
		public WfMatrixDimensionDefinitionCollection Dimensions
		{
			get
			{
				if (this._Dimensions == null)
					this._Dimensions = new WfMatrixDimensionDefinitionCollection();

				return this._Dimensions;
			}
		}

		public WorkbookNode ExportToExcelXml()
		{
			WorkbookNode workbook = new WorkbookNode();

			WfMatrixDefinitionExportOptions options = new WfMatrixDefinitionExportOptions();

			options.StartRow = 3;
			options.TitleCellStyleID = "s17";

			workbook.Load(ResourceHelper.GetResourceStream(Assembly.GetExecutingAssembly(), this.GetType().Namespace + ".Matrix.MatrixTemplate.xml"));

			ExportToExcelXml(workbook, options);

			return workbook;
		}

		public void ExportToExcelXml(WorkbookNode workbook, WfMatrixDefinitionExportOptions options)
		{
			workbook.NullCheck("workbook");
			options.NullCheck("options");

			FillWorkbook(this, workbook, options);
		}

		private static void FillWorkbook(WfMatrixDefinition definition, WorkbookNode workbook, WfMatrixDefinitionExportOptions options)
		{
			WorksheetNode worksheet = GetWorksheetFromWorkbook(workbook, options);

			worksheet.Names.Clear();
			workbook.Names.Clear();

			worksheet.Table.Rows[1].Cells.Clear();

			int row = options.StartRow;
			int column = options.StartColumn;

			RowNode titleRow = null;

			if (worksheet.Table.Rows.Count > 0)
				titleRow = worksheet.Table.Rows[1];
			else
			{
				titleRow = new RowNode();
				worksheet.Table.Rows.Add(titleRow);
			}

			foreach (WfMatrixDimensionDefinition dd in definition.Dimensions)
			{
				NamedRangeNode range = new NamedRangeNode();

				range.Name = dd.DimensionKey;

				range.RefersTo = string.Format("={0}!R{1}C{2}", worksheet.Name, row, column);
				workbook.Names.Add(range);

				CellNode cell = new CellNode();

				cell.Data.Value = dd.Name;

				if (options.TitleCellStyleID.IsNotEmpty())
					cell.StyleID = options.TitleCellStyleID;

				titleRow.Cells.Add(cell);

				column++;
			}
		}

		private static WorksheetNode GetWorksheetFromWorkbook(WorkbookNode workbook, WfMatrixDefinitionExportOptions options)
		{
			WorksheetNode worksheet = null;

			if (workbook.Worksheets.Count > 0)
			{
				if (options.MatrixSheetName.IsNotEmpty())
				{
					workbook.Worksheets.Contains(options.MatrixSheetName).FalseThrow("不能在模板中找到名称为{0}的工作簿", options.MatrixSheetName);
					worksheet = workbook.Worksheets[options.MatrixSheetName];
				}
				else
				{
					worksheet = workbook.Worksheets[0];
				}
			}
			else
			{
				worksheet = new WorksheetNode();

				workbook.Worksheets.Add(worksheet);
			}

			if (options.MatrixSheetName.IsNotEmpty())
				worksheet.Name = options.MatrixSheetName;
			else
				worksheet.Name = "Matrix";

			return worksheet;
		}
	}

	[Serializable]
	public class WfMatrixDefinitionCollection : SerializableEditableKeyedDataObjectCollectionBase<string, WfMatrixDefinition>
	{
		protected override string GetKeyForItem(WfMatrixDefinition item)
		{
			return item.Key;
		}
	}
}
