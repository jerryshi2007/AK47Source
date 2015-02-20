using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MCS.Library.Core;

namespace MCS.Library.Office.OpenXml.Excel
{
	public class ChartSerie
	{
		private ExcelChart _Chart = null;

		public ExcelChart Chart
		{
			get
			{
				return this._Chart;
			}
		}

		public ChartSerie(ExcelChart chart)
		{
			this._Chart = chart;
		}

		public ChartSerie(ExcelChart chart, bool isPivot)
			: this(chart)
		{
			this.IsPivot = isPivot;
		}

		public string Header
		{
			get;
			set;
		}

		public Range HeaderAddress
		{
			get;
			set;
		}

		public Range Series
		{
			get;
			set;
		}

		public Range XSeries
		{
			get;
			set;
		}

		private ChartTrendlineCollection _TrendLines = null;
		/// <summary>
		/// 趋势线集合的访问
		/// </summary>
		public ChartTrendlineCollection TrendLines
		{
			get
			{
				if (this._TrendLines == null)
					this._TrendLines = new ChartTrendlineCollection(this);

				return this._TrendLines;
			}
		}

		internal bool IsPivot
		{
			get;
			set;
		}
	}

	public sealed class ScatterChartSerie : ChartSerie
	{
		public ScatterChartSerie(ExcelChart chart, bool isPivot)
			: base(chart, isPivot)
		{
		}

		public int Smooth
		{
			get;
			set;
		}

		private ExcelMarkerStyle _Marker = ExcelMarkerStyle.None;
		public ExcelMarkerStyle Marker
		{
			get
			{
				return this._Marker;
			}
			set
			{
				this._Marker = value;
			}
		}
	}

	public sealed class PieChartSerie : ChartSerie
	{
		public PieChartSerie(ExcelChart chart, bool isPivot)
			: base(chart, isPivot)
		{
		}

		private int _Explosion;
		public int Explosion
		{
			get
			{
				return this._Explosion;
			}
			set
			{
				ExceptionHelper.TrueThrow<ArgumentOutOfRangeException>(value < 0 || value > 400, "Explosion 范围是0到400");
				this._Explosion = value;
			}
		}

		private ChartSerieDataLabel _DataLabel = null;

		public ChartSerieDataLabel DataLabel
		{
			get
			{
				if (this._DataLabel == null)
					this._DataLabel = new ChartSerieDataLabel(base.Chart, false);

				return _DataLabel;
			}
		}
	}

	public sealed class LineChartSerie : ChartSerie
	{
		public LineChartSerie(ExcelChart chart, bool isPivot)
			: base(chart, isPivot)
		{
		}

		private ChartSerieDataLabel _DataLabel = null;

		public ChartSerieDataLabel DataLabel
		{
			get
			{
				if (this._DataLabel == null)
					this._DataLabel = new ChartSerieDataLabel(base.Chart, IsPivot);

				return this._DataLabel;
			}
		}

		private ExcelMarkerStyle _Marker = ExcelMarkerStyle.None;
		public ExcelMarkerStyle Marker
		{
			get
			{
				return this._Marker;
			}
			set
			{
				this._Marker = value;
			}
		}

		public bool Smooth
		{
			get;
			set; 
		}
	}

	public sealed class ChartSerieCollection : IEnumerable<ChartSerie>
	{
		private List<ChartSerie> _List = new List<ChartSerie>();

		private ExcelChart _Chart = null;

		public ChartSerieCollection(ExcelChart chart)
		{
			this._Chart = chart;
		}

		public ChartSerie Add(Range Serie, Range XSerie)
		{
			ChartSerie item = ChartSerieFactory.CreateChartSerie(this._Chart, false, Serie, XSerie);
			this._List.Add(item);

			return item;
		}

		public IEnumerator<ChartSerie> GetEnumerator()
		{
			return this._List.GetEnumerator();
		}

		System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
		{
			return this._List.GetEnumerator();
		}
	}
}
