using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MCS.Library.Core;

namespace MCS.Library.Office.OpenXml.Excel
{
	/// <summary>
	/// 工作表显示信息
	/// </summary>
	public class SheetView
	{

		/*(colorId,defaultGridColor,showRuler,
		 * showOutlineSymbols (指示是否表分级显示符号可见。该标志应始终
覆盖SheetPr元素的outlinePr子元素，其属性被命名为
showOutlineSymbols时有冲突))
		 *extLst
		 *pane
		 *pivotSelection
		 */


		private WorkSheet _WrokSheet;

		public SheetView(WorkSheet sheet)
		{
			this._WrokSheet = sheet;
		}


		private bool _TabSelected = false;
		/// <summary>
		/// 设置为打开Excel文件时，默认选择的工作表
		/// </summary>
		public bool TabSelected
		{
			get
			{
				return this._TabSelected;
			}
			set
			{
				if (value && !this._TabSelected)
				{
					foreach (WorkSheet tabSelectSheet in this._WrokSheet.WorkBook.Sheets)
					{
						if (tabSelectSheet.SheetView.TabSelected && tabSelectSheet.PositionID != this._WrokSheet.PositionID)
						{
							tabSelectSheet.SheetView.TabSelected = false;
						}
					}
				}
				this._TabSelected = value;
			}
		}

		public bool ShowRowColHeaders
		{
			get;
			set;
		}

		private ExcelSheetViewType _ViewType = ExcelSheetViewType.Normal;
		/// <summary>
		/// 视图类型
		/// </summary>
		public ExcelSheetViewType ViewType
		{
			get { return this._ViewType; }
			set { this._ViewType = value; }
		}

		/// <summary>
		/// 是否显示公式在单元格上
		/// </summary>
		public bool ShowFormulas
		{
			get;
			set;
		}

		/// <summary>
		/// 是否显示网格
		/// </summary>
		public bool ShowGridLines
		{
            get { return showGridLines; }
            set { showGridLines = value; }
		}

        private bool showGridLines = true;

		/// <summary>
		/// 
		/// </summary>
		public bool ShowHeaders
		{
			get;
			set;
		}

		public bool ShowWhiteSpace
		{
			get;
			set;
		}

		//windowProtection
		/// <summary>
		/// 否被锁定在窗口的窗格由于工作簿的保护
		/// </summary>
		internal bool WindowProtection
		{
			get;
			set;
		}

		//zoomScale
		private int _ZoomScale = 0;
		/// <summary>
		/// 窗口显示比例
		/// </summary>
		public int ZoomScale
		{
			get
			{
				return this._ZoomScale;
			}
			set
			{
				(value < 10 || value > 400).TrueThrow<ArgumentOutOfRangeException>("显示比例的大小值必须在(10-400)之间,当前ZoomScale{0}", value.ToString());
				this._ZoomScale = value;
			}
		}

		//showZeros
		/// <summary>
		/// 如果设置为True表示用零填充没有值的单元格
		/// </summary>
		public bool ShowZeros
		{
			get;
			set;
		}

		//topLeftCell
		public CellAddress TopLeftCell
		{
			get;
			set;
		}

		//selection
		/// <summary>
		/// 当前Excel选中的区域，激活的单元格
		/// </summary>
		public Range SelectedRange
		{
			get;
			set;
		}

		internal SheetView Clone(WorkSheet worksheet)
		{
			SheetView cloneSheetView = new SheetView(worksheet)
			{
				_TabSelected = this._TabSelected,
				TopLeftCell =this.TopLeftCell,
				SelectedRange =this.SelectedRange,
				ShowZeros = this.ShowZeros,
				_ZoomScale =this._ZoomScale,
				WindowProtection = this.WindowProtection,
				ShowWhiteSpace = this.ShowWhiteSpace,
				ShowHeaders =this.ShowHeaders,
				ShowGridLines = this.ShowGridLines,
				ShowFormulas = this.ShowFormulas,
				ShowRowColHeaders =this.ShowRowColHeaders
			};

			return cloneSheetView;
		}

	}
}
