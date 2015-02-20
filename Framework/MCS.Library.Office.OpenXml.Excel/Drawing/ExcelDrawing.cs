using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MCS.Library.Office.OpenXml.Excel
{
	public abstract class ExcelDrawing
	{
		private WorkSheet _WorkSheet;

		internal WorkSheet WorkSheet
		{
			get
			{
				return this._WorkSheet;
			}
		}

		protected internal ExcelDrawing(WorkSheet worksheet)
		{
			this._WorkSheet = worksheet;
		}

		internal DrawingPosition _From;
		public DrawingPosition From
		{
			get
			{
				if (this._From == null)
				{
					this._From = new DrawingPosition();
				}

				return this._From;
			}
		}

		internal DrawingPosition _To;
		public DrawingPosition To
		{
			get
			{
				if (this._To == null)
				{
					this._To = new DrawingPosition();
				}

				return this._To;
			}
		}

		private ExcelEditAs _EditAs = ExcelEditAs.TwoCell;
		/// <summary>
		/// 调整Excel图片大小时，Excel列宽
		/// </summary>
		public ExcelEditAs EditAs
		{
			get
			{
				return this._EditAs;
			}
			set
			{
				this._EditAs = value;
			}
		}

		public bool Locked
		{
			get;
			set;
		}

		public bool Print
		{
			get;
			set;
		}

		public string Name
		{
			get;
			set;
		}

        
		public void SetPosition(int PixelTop, int PixelLeft)
		{
			int width = GetPixelWidth();
			int height = GetPixelHeight();

			SetPixelTop(PixelTop);
			SetPixelLeft(PixelLeft);

			SetPixelWidth(width);
			SetPixelHeight(height);
		}

		internal void SetPixelWidth(int pixels)
		{
			SetPixelWidth(pixels, ExcelCommon.ExcelDrawing.STANDARD_DPI);
		}

		internal void SetPixelHeight(int pixels)
		{
			SetPixelHeight(pixels, ExcelCommon.ExcelDrawing.STANDARD_DPI);
		}

		internal void SetPixelLeft(int pixels)
		{
			decimal mdw = this._WorkSheet.WorkBook.MaxFontWidth;
			int prevPix = 0;
			int pix = (int)decimal.Truncate(((256 * GetColumnWidth(1) + decimal.Truncate(128 / (decimal)mdw)) / 256) * mdw);
			int col = 2;

			while (pix < pixels)
			{
				prevPix = pix;
				pix += (int)decimal.Truncate(((256 * GetColumnWidth(col++) + decimal.Truncate(128 / (decimal)mdw)) / 256) * mdw);
			}
			if (pix == pixels)
			{
				From.Column = col - 1;
				From.ColumnOff = 0;
			}
			else
			{
				From.Column = col - 2;
				From.ColumnOff = (pixels - prevPix) * ExcelCommon.ExcelDrawing.EMU_PER_PIXEL;
			}
		}

		internal void SetPixelTop(int pixels)
		{
			decimal mdw = this._WorkSheet.WorkBook.MaxFontWidth;
			int prevPix = 0;
			int pix = (int)(GetRowWidth(1) / 0.75);
			int row = 2;

			while (pix < pixels)
			{
				prevPix = pix;
				pix += (int)(GetRowWidth(row++) / 0.75);
			}

			if (pix == pixels)
			{
				From.Row = row - 1;
				From.RowOff = 0;
			}
			else
			{
				From.Row = row - 2;
				From.RowOff = (pixels - prevPix) * ExcelCommon.ExcelDrawing.EMU_PER_PIXEL;
			}
		}

		internal int GetPixelHeight()
		{
			decimal mdw = this._WorkSheet.WorkBook.MaxFontWidth;

			int pix = -(From.RowOff / ExcelCommon.ExcelDrawing.EMU_PER_PIXEL);
			for (int row = From.Row + 1; row <= To.Row; row++)
			{
				pix += (int)(GetRowWidth(row) / 0.75);
			}
			pix += To.RowOff / ExcelCommon.ExcelDrawing.EMU_PER_PIXEL;

			return pix;
		}

		internal int GetPixelWidth()
		{
			decimal mdw = this._WorkSheet.WorkBook.MaxFontWidth;

			int pix = -From.ColumnOff / ExcelCommon.ExcelDrawing.EMU_PER_PIXEL;
			for (int col = From.Column + 1; col <= To.Column; col++)
			{
				pix += (int)decimal.Truncate(((256 * GetColumnWidth(col) + decimal.Truncate(128 / (decimal)mdw)) / 256) * mdw);
			}
			pix += To.ColumnOff / ExcelCommon.ExcelDrawing.EMU_PER_PIXEL;
			return pix;
		}

		internal void SetPixelHeight(int pixels, float dpi)
		{
			decimal mdw = this._WorkSheet.WorkBook.MaxFontWidth;
			pixels = (int)(pixels / (dpi / ExcelCommon.ExcelDrawing.STANDARD_DPI) + .5);
			int pixOff = pixels - (int)(GetRowHeight(this.From.Row + 1) / 0.75) - (this.From.RowOff / ExcelCommon.ExcelDrawing.EMU_PER_PIXEL);
			int prevPixOff = pixels;
			int row = this.From.Row + 1;

			while (pixOff >= 0)
			{
				prevPixOff = pixOff;
				pixOff -= (int)(GetRowHeight(++row) / 0.75);
			}
			this.To.Row = row - 1;
			if (this.From.Row == this.To.Row)
			{
				this.To.RowOff = this.From.RowOff + (pixels) * ExcelCommon.ExcelDrawing.EMU_PER_PIXEL;
			}
			else
			{
				this.To.RowOff = prevPixOff * ExcelCommon.ExcelDrawing.EMU_PER_PIXEL;
			}
		}

		internal void SetPixelWidth(int pixels, float dpi)
		{
			decimal mdw = this._WorkSheet.WorkBook.MaxFontWidth;
			pixels = (int)(pixels / (dpi / ExcelCommon.ExcelDrawing.STANDARD_DPI) + .5);
			int pixOff = (int)pixels - ((int)decimal.Truncate(((256 * GetColumnWidth(this.From.Column + 1) + decimal.Truncate(128 / (decimal)mdw)) / 256) * mdw) - this.From.ColumnOff / ExcelCommon.ExcelDrawing.EMU_PER_PIXEL);
			int prevPixOff = this.From.ColumnOff / ExcelCommon.ExcelDrawing.EMU_PER_PIXEL + (int)pixels;
			int col = this.From.Column + 2;

			while (pixOff >= 0)
			{
				prevPixOff = pixOff;
				pixOff -= (int)decimal.Truncate(((256 * GetColumnWidth(col++) + decimal.Truncate(128 / (decimal)mdw)) / 256) * mdw);
			}

			this.To.Column = col - 2;
			this.To.ColumnOff = prevPixOff * ExcelCommon.ExcelDrawing.EMU_PER_PIXEL;
		}

		private decimal GetColumnWidth(int col)
		{
			if (this._WorkSheet.Columns.ContainsKey(col))
			{
				return (decimal)this._WorkSheet.Columns[col].VisualWidth;
			}
			else
			{
				return (decimal)this._WorkSheet.DefaultColumnWidth;
			}
		}

		private double GetRowWidth(int row)
		{
			if (this._WorkSheet.Rows.ContainsKey(row))
			{
				return (double)this._WorkSheet.Rows[row].Height;
			}
			else
			{
				return (double)this._WorkSheet.DefaultRowHeight;
			}
		}

		private int GetRowHeight(int row)
		{
			if (this._WorkSheet.Rows.ContainsKey(row))
			{
				return (int)this._WorkSheet.Rows[row].Height;
			}
			else
			{
				return (int)this._WorkSheet.DefaultRowHeight;
			}
		}
	}
}
