using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MCS.Library.Core;

namespace MCS.Library.Office.OpenXml.Excel
{
	public sealed class ChartTrendline
	{
		public ChartTrendline(ExcelTrendLine type)
		{
			this._Type = type;
		}

		private ExcelTrendLine _Type = ExcelTrendLine.Linear;
		public ExcelTrendLine Type
		{
			get
			{
				return this._Type;
			}
			set
			{
				this._Type = value;
			}
		}

		public string Name
		{
			get;
			set;
		}

		public decimal Order
		{
			get;
			set;
		}

		public decimal Forward
		{
			get;
			set;
		}

		public decimal Backward
		{
			get;
			set;
		}

		public decimal Intercept
		{
			get;
			set;
		}

		public bool DisplayRSquaredValue
		{
			get;
			set;
		}

		public bool DisplayEquation
		{
			get;
			set;
		}
	}

	public sealed class ChartTrendlineCollection : IEnumerable<ChartTrendline>
	{
		private ChartSerie _Serie = null;

		public ChartTrendlineCollection(ChartSerie serie)
		{
			this._Serie = serie;
		}

		public ChartTrendline Add(ExcelTrendLine type)
		{
			ExceptionHelper.TrueThrow<ArgumentException>(this._Serie.Chart.IsType3D() || this._Serie.Chart.IsTypePercentStacked() ||
				  this._Serie.Chart.IsTypeStacked() || this._Serie.Chart.IsTypePieDoughnut(), "趋势线并不适用于3D图，堆叠图，饼图或圆环图");

			ChartTrendline result = new ChartTrendline(type);

			this._List.Add(result);

			return result;
		}


		private List<ChartTrendline> _List = new List<ChartTrendline>();

		public IEnumerator<ChartTrendline> GetEnumerator()
		{
			return this._List.GetEnumerator();
		}

		System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
		{
			return this._List.GetEnumerator();
		}
	}
}
