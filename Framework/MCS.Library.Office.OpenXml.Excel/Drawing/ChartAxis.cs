using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MCS.Library.Office.OpenXml.Excel
{
	public sealed class ChartAxis
	{
		private ExcelAxisTickMark _MajorTickMark = ExcelAxisTickMark.None;
		public ExcelAxisTickMark MajorTickMark
		{
			get
			{
				return this._MajorTickMark;
			}
			set
			{
				this._MajorTickMark = value;
			}
		}

		private ExcelAxisTickMark _MinorTickMark = ExcelAxisTickMark.None;
		public ExcelAxisTickMark MinorTickMark
		{
			get
			{
				return this._MinorTickMark;
			}
			set
			{
				this._MinorTickMark = value;
			}
		}

		private ExcelAxisType _AxisType = ExcelAxisType.Val;
		internal ExcelAxisType AxisType
		{
			get
			{
				return this._AxisType;
			}
		}

		private ExcelAxisPosition _AxisPosition = ExcelAxisPosition.Left;
		public ExcelAxisPosition AxisPosition
		{
			get
			{
				return this._AxisPosition;
			}
			set
			{
				this._AxisPosition = value;
			}
		}

		private ExcelCrosses _Crosses = ExcelCrosses.AutoZero;
		public ExcelCrosses Crosses
		{
			get
			{
				return this._Crosses;
			}
			set
			{
				this._Crosses = value;
			}
		}

		private ExcelCrossBetween _CrossBetween = ExcelCrossBetween.Between;
		public ExcelCrossBetween CrossBetween
		{
			get
			{
				return this._CrossBetween;
			}
			set
			{
				this._CrossBetween = value;
			}
		}

		private ExcelTickLabelPosition _LabelPosition = ExcelTickLabelPosition.None;
		public ExcelTickLabelPosition LabelPosition
		{
			get
			{
				return this._LabelPosition;
			}
			set
			{
				this._LabelPosition = value;
			}
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
				return this._Font;
			}
			set
			{
				this._Font = value;
			}
		}

		private ExcelTickLabelPosition _TickLabelPosition = ExcelTickLabelPosition.None ;
		public ExcelTickLabelPosition TickLabelPosition
		{
			get
			{
				return this._TickLabelPosition;
			}
			set
			{
				this._LabelPosition = value;
			}
		}

		private ExcelChartTitle _Title = null;
		public ExcelChartTitle Title
		{
			get
			{
				if (this._Title == null)
					this._Title = new ExcelChartTitle();
				return this._Title;
			}
		}

		private ExcelAxisOrientation _Orientation = ExcelAxisOrientation.MinMax;
		public ExcelAxisOrientation Orientation
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

		public double? CrossesAt
		{
			get;
			set;
		}

		public string Format
		{
			get;
			set;
		}

		public bool Deleted
		{
			get;
			set;
		}

		public double? MinValue
		{
			get;
			set;
		}

		public double? MaxValue
		{
			get;
			set;
		}

		public double? MajorUnit
		{
			get;
			set;
		}

		public double? LogBase
		{
			get;
			set;
		}
	}
}
