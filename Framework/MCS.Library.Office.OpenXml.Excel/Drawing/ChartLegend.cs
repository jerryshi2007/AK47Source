using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MCS.Library.Office.OpenXml.Excel
{
	public sealed class ChartLegend
	{
		private ExcelLegendPosition _Position = ExcelLegendPosition.Right;
		public ExcelLegendPosition Position
		{
			get { return this._Position; }
			set { this._Position = value; }
		}

		public bool Overlay
		{
			get;
			set;
		}

		private DrawingFill _Fill = null;
		public DrawingFill Fill
		{
			get
			{
				if (this._Fill == null)
					this._Fill = new DrawingFill();

				return this._Fill;
			}
		}

		private DrawingBorder _Border = null;
		public DrawingBorder Border
		{
			get
			{
				if (this._Border == null)
					this._Border = new DrawingBorder();

				return this._Border;
			}
		}

		private Paragraph _Font = null;
		public Paragraph Font
		{
			get
			{
				if (this._Font == null)
					this._Font = new Paragraph();

				return this._Font;
			}
		}
	}
}
