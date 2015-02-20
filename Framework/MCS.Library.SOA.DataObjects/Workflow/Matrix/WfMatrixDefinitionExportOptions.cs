using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace MCS.Library.SOA.DataObjects.Workflow
{
	/// <summary>
	/// 矩阵定义导出的属性
	/// </summary>
	public class WfMatrixDefinitionExportOptions
	{
		private int _StartRow = 1;
		private int _StartColumn = 1;
		private string _MatrixSheetName = "Matrix";
		private string _TitleCellStyleID = string.Empty;

		public string TitleCellStyleID
		{
			get
			{
				return this._TitleCellStyleID;
			}
			set
			{
				this._TitleCellStyleID = value;
			}
		}

		public string MatrixSheetName
		{
			get
			{
				return this._MatrixSheetName;
			}
			set
			{
				this._MatrixSheetName = value;
			}
		}

		public int StartColumn
		{
			get
			{
				return this._StartColumn;
			}
			set
			{
				this._StartColumn = value;
			}
		}

		public int StartRow
		{
			get
			{
				return this._StartRow;
			}
			set
			{
				this._StartRow = value;
			}
		}


	}
}