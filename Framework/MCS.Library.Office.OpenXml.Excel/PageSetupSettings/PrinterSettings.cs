using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO.Packaging;
using MCS.Library.Core;

namespace MCS.Library.Office.OpenXml.Excel
{
	public class PrinterSettings
	{
		private WorkSheet _WorkSheet;

		public PrinterSettings(WorkSheet worksheet)
		{
			this._WorkSheet = worksheet;
		}

		#region "pageMargins"
		const string _leftMarginPath = "d:pageMargins/@left";
		//left
		private decimal _LeftMargin = decimal.MinValue;
		public decimal LeftMargin
		{
			get { return this._LeftMargin; }
			set
			{
				this.InitMargins();
				this._LeftMargin = value;
			}
		}

		const string _rightMarginPath = "d:pageMargins/@right";

		//right
		private decimal _RightMargin = decimal.MinValue;
		public decimal RightMargin
		{
			get
			{
				return this._RightMargin;
			}
			set
			{
				this.InitMargins();
				this._RightMargin = value;
			}
		}

		//top
		private decimal _TopMargin = decimal.MinValue;
		public decimal TopMargin
		{
			get
			{
				return this._TopMargin;
			}
			set
			{
				this.InitMargins();
				this._TopMargin = value;
			}
		}

		//bottom
		private decimal _BottomMargin = decimal.MinValue;
		public decimal BottomMargin
		{
			get
			{
				return this._BottomMargin;
			}
			set
			{
				this.InitMargins();
				this._BottomMargin = value;
			}
		}

		//header
		private decimal _HeaderMargin = decimal.MinValue;
		public decimal HeaderMargin
		{
			get
			{
				return this._HeaderMargin;
			}
			set
			{
				this.InitMargins();
				this._HeaderMargin = value;
			}
		}

		//footer
		private decimal _FooterMargin = decimal.MinValue;
		public decimal FooterMargin
		{
			get
			{
				return this._FooterMargin;
			}
			set
			{
				this.InitMargins();
				this._FooterMargin = value;
			}
		}
		#endregion

		#region “pageSetup”
		//orientation
		private ExcelOrientation _Orientation = ExcelOrientation.Default;
		/// <summary>
		/// 纵向或横向
		/// </summary>
		public ExcelOrientation Orientation
		{
			get
			{
				return this._Orientation;
			}
			set
			{
				this._Orientation = value;
			}
		}

		//fitToWidth
		private int _FitToWidth = int.MinValue;
		/// <summary>
		/// 适应页面宽度。FitToPage为true时使用！0表示自动
		/// </summary>
		public int FitToWidth
		{
			get { return this._FitToWidth; }
			set { this._FitToWidth = value; }
		}

		private int _Copies = int.MinValue;
		/// <summary>
		/// 打印张数
		/// </summary>
		public int NumberOfCopiesToPrint
		{
			get { return this._Copies; }
			set { this._Copies = value; }
		}

		private int _FirstPageNumber = int.MinValue;
		/// <summary>
		/// 页码值
		/// </summary>
		public int FirstPageNumber
		{
			get { return this._FirstPageNumber; }
			set { this._FirstPageNumber = value; }
		}

		//fitToHeight
		private int _FitToHeight = int.MinValue;
		/// <summary>
		/// 适应页面宽度。FitToPage为true时使用！0表示自动
		/// </summary>
		public int FitToHeight
		{
			get { return this._FitToHeight; }
			set { this._FitToHeight = value; }
		}

		//scale
		private int _Scale = int.MinValue;
		public int Scale
		{
			get { return this._Scale; }
			set { this._Scale = value; }
		}

		//pageOrder
		private ExcelPageOrder _PageOrder = ExcelPageOrder.OverThenDown;
		public ExcelPageOrder PageOrder
		{
			get { return this._PageOrder; }
			set { this._PageOrder = value; }
		}

		//todo:不明确
		////cellComments
		//private ExcelCellComments _CellComments;
		///// <summary>
		///// 单元格批注打印显示方式
		///// </summary>
		//public ExcelCellComments CellComments
		//{
		//    get { return this._CellComments; }
		//    set { this._CellComments = value; }
		//}

		//blackAndWhite
		/// <summary>
		/// 只打印黑白
		/// </summary>
		public bool BlackAndWhite
		{
			get;
			set;
		}

		//todo: paperHeight

		//@draft
		public bool Draft
		{
			get;
			set;
		}

		//int //paperSize
		private ExcelPaperSize _PaperSize = ExcelPaperSize.Letter;
		public ExcelPaperSize PaperSize
		{
			get { return this._PaperSize; }
			set { this._PaperSize = value; }
		}

		#endregion

		#region "d:sheetPr/d:pageSetUpPr/@fitToPage"
		//sheetPr
		/// <summary>
		/// 页面打印适应页面。
		/// </summary>
		public bool FitToPage { get; set; }

		/// <summary>
		/// 是否自动显示分页
		/// </summary>
		public bool ShowAutoPageBreaks { get; set; }
		#endregion

		#region “printOptions”
		//headings
		/// <summary>
		/// 打印标题（列字母和行号）
		/// </summary>
		public bool ShowHeaders
		{
			get;
			set;
		}

		//gridLines //gridLinesSet
		public bool ShowGridLines
		{
			get;
			set;
		}

		//horizontalCentered
		/// <summary>
		/// 水平居中打印
		/// </summary>
		public bool HorizontalCentered
		{
			get;
			set;
		}

		//verticalCentered
		public bool VerticalCentered
		{
			get;
			set;
		}
		#endregion

		/// <summary>
		/// 打印标题 (标题模版)
		/// </summary>
		public void SetPrintTitles(string rangeAddress)
		{
			string key = "_xlnm.Print_Titles";
			if (this._WorkSheet.Names.ContainsKey(key))
			{
				if (rangeAddress.IsNotEmpty())
				{
					this._WorkSheet.Names[key].Address = Range.Parse(this._WorkSheet, rangeAddress);
				}
				else
				{
					this._WorkSheet.Names.Remove(key);
				}
			}
			else
			{
				if (rangeAddress.IsNotEmpty())
				{
					this._WorkSheet.Names.Add(rangeAddress, key);
				}
			}
		}

		/// <summary>
		/// 打印区域，传入空将删除原有的
		/// </summary>
		public void PrintArea(string rangeAddress)
		{
			string key = "_xlnm.Print_Area";
			if (this._WorkSheet.Names.ContainsKey(key))
			{
				if (rangeAddress.IsNotEmpty())
				{
					this._WorkSheet.Names[key].Address = Range.Parse(this._WorkSheet, rangeAddress);
				}
				else
				{
					this._WorkSheet.Names.Remove(key);
				}
			}
			else
			{
				if (rangeAddress.IsNotEmpty())
				{
					this._WorkSheet.Names.Add(rangeAddress, key);
				}
			}
		}

		private bool IsInitMargins = true;
		private void InitMargins()
		{
			if (this.IsInitMargins)
			{
				this.IsInitMargins = false;
				this.LeftMargin = 0.7087M;
				this.RightMargin = 0.7087M;
				this.TopMargin = 0.7480M;
				this.BottomMargin = 0.7480M;
				this.HeaderMargin = 0.315M;
				this.FooterMargin = 0.315M;
			}
		}
	}
}
