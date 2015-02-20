using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MCS.Library.Office.OpenXml.Excel
{
	public sealed class DrawingBorder
	{
		internal DrawingFill _Fill = null;
		public DrawingFill Fill
		{
			get
			{
				if (this._Fill == null)
				{
					this._Fill = new DrawingFill();
				}
				return this._Fill;
			}
		}

		private ExcelDrawingLineStyle _LineStyle = default(ExcelDrawingLineStyle);
		public ExcelDrawingLineStyle LineStyle
		{
			get
			{
				return this._LineStyle;
			}
			set
			{
				this._LineStyle = value;
			}
		}

		private ExcelDrawingLineCap _LineCap = ExcelDrawingLineCap.Flat;
		public ExcelDrawingLineCap LineCap
		{
			get
			{
				return this._LineCap;
			}
			set
			{
				this._LineCap = value;
			}
		}

		public int Width
		{
			get;
			set;
		}
	}
}
