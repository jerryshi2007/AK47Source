using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MCS.Library.Office.OpenXml.Excel
{
	public sealed class ChartPlotArea
	{
		private ExcelChart _FirstChart;

		internal ChartPlotArea(ExcelChart firstChart)
		{
			this._FirstChart = firstChart;
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
	}
}
