using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MCS.Library.Office.OpenXml.Excel
{
	public class ChartDataLabel
	{
		internal protected ExcelChart _Chart = null;

		public ExcelChart Chart
		{
			get
			{
				return this._Chart;
			}
		}

		public ChartDataLabel(ExcelChart chart, bool isPivot)
		{
			this._Chart = chart;
			this.IsPivot = isPivot;
		}

		public bool ShowValue
		{
			get;
			set;
		}

		public bool ShowCategory
		{
			get;
			set;
		}

		public bool ShowSeriesName
		{
			get;
			set;
		}

		public bool ShowPercent
		{
			get;
			set;
		}

		public bool ShowLeaderLines
		{
			get;
			set;
		}

		public bool ShowBubbleSize
		{
			get;
			set;
		}

		public bool ShowLegendKey
		{
			get;
			set;
		}

		public string Separator
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

		internal bool IsPivot
		{
			get;
			set;
		}
	}

	public sealed class ChartSerieDataLabel : ChartDataLabel
	{
		private ExcelLabelPosition _Position = ExcelLabelPosition.Center;
		public ExcelLabelPosition Position
		{
			get
			{
				return this._Position;
			}
			set
			{
				this._Position = value;
			}
		}

		public ChartSerieDataLabel(ExcelChart chart, bool isPivot)
			: base(chart, isPivot)
		{ 
		}
	}
}
