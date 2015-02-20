using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MCS.Library.Core;
using System.IO.Packaging;
using System.Xml.Linq;

namespace MCS.Library.Office.OpenXml.Excel
{
	public class ExcelChart : ExcelDrawing,IPersistable
	{
		public ExcelChart(string chartName, ExcelChartType type, WorkSheet wroksheet)
			: base(wroksheet)
		{
			this.ChartType = type;
			this.Name = chartName;
		}

        void IPersistable.Save(ExcelSaveContext context)
        {

            //context.LinqWriter.WriteWrokSheetChart(this,context.);
        }

        void IPersistable.Load(ExcelLoadContext context)
        {
            //PackageRelationship drawingsRelation = context.Package.GetPart(this.WorkSheet.SheetUri).GetRelationship(this.RelationshipID);
            //this.DrawingUri = PackUriHelper.ResolvePartUri(drawingsRelation.SourceUri, drawingsRelation.TargetUri);
            //XElement drawingsElement = context.Package.GetXElementFromUri(this.DrawingUri);
            //context.Reader.ReadWrokSheetDrawings(this, drawingsElement);


            PackageRelationship drawingsRelation = context.Package.GetPart(this.DrawingUri).GetRelationship(this.RelationshipID);
            this.DrawingUri = PackUriHelper.ResolvePartUri(drawingsRelation.SourceUri, drawingsRelation.TargetUri);
            XElement drawingsElement = context.Package.GetXElementFromUri(this.DrawingUri);
            context.Reader.ReadPieCharts(this, drawingsElement, context);
        }


        internal ChartDataLabel _DataLabel = null;

        public ChartDataLabel DataLabel
        {
            get
            {
                if (this._DataLabel == null)
                    this._DataLabel = new ChartDataLabel(this, false);

                return this._DataLabel;
            }
        }
        internal CategoryAxisData _CategoryAxisData = null;
        public CategoryAxisData CategoryAxisData
        {
            get
            {
                if (this._CategoryAxisData == null)
                    this._CategoryAxisData = new CategoryAxisData();

                return this._CategoryAxisData;
            }
        }

        internal CategoryAxisValue _CategoryAxisValue = null;
        public CategoryAxisValue CategoryAxisValue
        {
            get
            {
                if (this._CategoryAxisValue == null)
                    this._CategoryAxisValue = new CategoryAxisValue();

                return this._CategoryAxisValue;
            }
        }

        internal string RelationshipID
        {
            get;
            set;
        }

        internal Uri DrawingUri
        {
            get;
            set;
        }

		public ExcelChartType ChartType { get; internal set; }

		public bool RoundedCorners { get; set; }

		internal ExcelChartTitle _Title = null;
		public  ExcelChartTitle Title
		{
			get
			{
				if (this._Title == null)
					this._Title = new ExcelChartTitle();

				return this._Title;
			}
		}

		private bool _AutoTitleDeleted = false;
		public bool AutoTitleDeleted
		{
			get
			{
				return this._AutoTitleDeleted;
			}
			set
			{
				this._AutoTitleDeleted = value;
			}
		}

		private ChartSerieCollection _ChartSeries = null;
		public ChartSerieCollection Series
		{
			get
			{
				if (this._ChartSeries == null)
					this._ChartSeries = new ChartSerieCollection(this);

				return this._ChartSeries;
			}
		}

		private List<ChartAxis> _Axis = new List<ChartAxis>();
		public List<ChartAxis> Axis
		{
			get
			{
				return this._Axis;
			}
		}

		private ChartAxis _XAxis = new ChartAxis();
		public ChartAxis XAxis
		{
			get
			{
				return this._XAxis;
			}
			private set
			{
				this._XAxis = value;
			}
		}

		private ChartAxis _YAxis = new ChartAxis();
		public ChartAxis YAxis
		{
			get
			{
				return this._YAxis;
			}
			private set
			{
				this._YAxis = value;
			}
		}

		private bool _UseSecondaryAxis = false;
		public bool UseSecondaryAxis
		{
			get
			{
				return this._UseSecondaryAxis;
			}
			set
			{
				this._UseSecondaryAxis = value;
			}
		}

		private ExcelChartStyle _Style = ExcelChartStyle.None;
		public ExcelChartStyle Style
		{
			get
			{
				return this._Style;
			}
			set
			{
				this._Style = value;
			}
		}

		public bool ShowHiddenData
		{
			get;
			set;
		}

		private ExcelDisplayBlanksAs _DisplayBlanksAs = ExcelDisplayBlanksAs.Gap;
		public ExcelDisplayBlanksAs DisplayBlanksAs
		{
			get
			{
				return this._DisplayBlanksAs;
			}
			set
			{
				this._DisplayBlanksAs = value;
			}
		}

		private ChartPlotArea _PlotArea = null;
		public ChartPlotArea PlotArea
		{
			get
			{
				if (this._PlotArea == null)
					this._PlotArea = new ChartPlotArea(this);

				return this._PlotArea;
			}
		}

		internal ChartLegend _Legend = null;
		public ChartLegend Legend
		{
			get
			{
				if (this._Legend == null)
					this._Legend = new ChartLegend();

				return this._Legend;
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

		public ExcelView3D View3D
		{
			get
			{
				ExceptionHelper.TrueThrow(IsType3D() == false, "不是3D类型，请不要设置");

				return new ExcelView3D();
			}
		}

		private ExcelGrouping _Grouping = ExcelGrouping.Clustered;
		public ExcelGrouping Grouping
		{
			get
			{
				return this._Grouping;
			}
			set
			{
				this._Grouping = value;
			}
		}

		public bool VaryColors
		{
			get;
			set;
		}

		protected internal bool IsType3D()
		{
			return this.ChartType == ExcelChartType.Area3D || this.ChartType == ExcelChartType.AreaStacked3D ||
				   this.ChartType == ExcelChartType.AreaStacked1003D || this.ChartType == ExcelChartType.BarClustered3D ||
				   this.ChartType == ExcelChartType.BarStacked3D || this.ChartType == ExcelChartType.BarStacked1003D ||
				   this.ChartType == ExcelChartType.Column3D || this.ChartType == ExcelChartType.ColumnClustered3D ||
				   this.ChartType == ExcelChartType.ColumnStacked3D || this.ChartType == ExcelChartType.ColumnStacked1003D ||
				   this.ChartType == ExcelChartType.Line3D || this.ChartType == ExcelChartType.Pie3D ||
				   this.ChartType == ExcelChartType.PieExploded3D || this.ChartType == ExcelChartType.Bubble3DEffect ||
				   this.ChartType == ExcelChartType.ConeBarClustered || this.ChartType == ExcelChartType.ConeBarStacked ||
				   this.ChartType == ExcelChartType.ConeBarStacked100 || this.ChartType == ExcelChartType.ConeCol ||
				   this.ChartType == ExcelChartType.ConeColClustered || this.ChartType == ExcelChartType.ConeColStacked ||
				   this.ChartType == ExcelChartType.ConeColStacked100 || this.ChartType == ExcelChartType.CylinderBarClustered ||
				   this.ChartType == ExcelChartType.CylinderBarStacked || this.ChartType == ExcelChartType.CylinderBarStacked100 ||
				   this.ChartType == ExcelChartType.CylinderCol || this.ChartType == ExcelChartType.CylinderColClustered ||
				   this.ChartType == ExcelChartType.CylinderColStacked || this.ChartType == ExcelChartType.CylinderColStacked100 ||
				   this.ChartType == ExcelChartType.PyramidBarClustered || this.ChartType == ExcelChartType.PyramidBarStacked ||
				   this.ChartType == ExcelChartType.PyramidBarStacked100 || this.ChartType == ExcelChartType.PyramidCol ||
				   this.ChartType == ExcelChartType.PyramidColClustered || this.ChartType == ExcelChartType.PyramidColStacked ||
				   this.ChartType == ExcelChartType.PyramidColStacked100;
		}

		protected internal bool IsTypePercentStacked()
		{
			return this.ChartType == ExcelChartType.AreaStacked100 || this.ChartType == ExcelChartType.BarStacked100 ||
				   this.ChartType == ExcelChartType.BarStacked1003D || this.ChartType == ExcelChartType.ColumnStacked100 ||
				   this.ChartType == ExcelChartType.ColumnStacked1003D || this.ChartType == ExcelChartType.ConeBarStacked100 ||
				   this.ChartType == ExcelChartType.ConeColStacked100 || this.ChartType == ExcelChartType.CylinderBarStacked100 ||
				   this.ChartType == ExcelChartType.CylinderColStacked || this.ChartType == ExcelChartType.LineMarkersStacked100 ||
				   this.ChartType == ExcelChartType.LineStacked100 || this.ChartType == ExcelChartType.PyramidBarStacked100 ||
				   this.ChartType == ExcelChartType.PyramidColStacked100;
		}

		protected internal bool IsTypeStacked()
		{
			return this.ChartType == ExcelChartType.AreaStacked || this.ChartType == ExcelChartType.AreaStacked3D ||
				   this.ChartType == ExcelChartType.BarStacked || this.ChartType == ExcelChartType.BarStacked3D ||
				   this.ChartType == ExcelChartType.ColumnStacked3D || this.ChartType == ExcelChartType.ColumnStacked ||
				   this.ChartType == ExcelChartType.ConeBarStacked || this.ChartType == ExcelChartType.ConeColStacked ||
				   this.ChartType == ExcelChartType.CylinderBarStacked || this.ChartType == ExcelChartType.CylinderColStacked ||
				   this.ChartType == ExcelChartType.LineMarkersStacked || this.ChartType == ExcelChartType.LineStacked ||
				   this.ChartType == ExcelChartType.PyramidBarStacked || this.ChartType == ExcelChartType.PyramidColStacked;
		}

		protected internal bool IsTypePieDoughnut()
		{
			return this.ChartType == ExcelChartType.Pie || this.ChartType == ExcelChartType.PieExploded ||
				   this.ChartType == ExcelChartType.PieOfPie || this.ChartType == ExcelChartType.Pie3D ||
				   this.ChartType == ExcelChartType.PieExploded3D || this.ChartType == ExcelChartType.BarOfPie ||
				   this.ChartType == ExcelChartType.Doughnut || this.ChartType == ExcelChartType.DoughnutExploded;
		}
	}

	public class ExcelPieChart : ExcelChart,IPersistable
	{
		public ExcelPieChart(string chartName, ExcelChartType type, WorkSheet wroksheet)
			: base(chartName, type, wroksheet)
		{
           
		}

        public ExcelPieChart(string chartName, ExcelChartType type, WorkSheet wroksheet, Uri drawUri,string rId)
            : base(chartName, type, wroksheet)
        {
            this.DrawingUri = drawUri;
            this.RelationshipID = rId;
        }

        //internal ChartDataLabel _DataLabel = null;

        //public ChartDataLabel DataLabel
        //{
        //    get
        //    {
        //        if (this._DataLabel == null)
        //            this._DataLabel = new ChartDataLabel(this, false);

        //        return this._DataLabel;
        //    }
        //}
        //internal CategoryAxisData _CategoryAxisData = null;
        //public CategoryAxisData CategoryAxisData
        //{
        //    get
        //    {
        //        if (this._CategoryAxisData == null)
        //            this._CategoryAxisData = new CategoryAxisData();

        //        return this._CategoryAxisData;
        //    }
        //}

        //internal CategoryAxisValue _CategoryAxisValue = null;
        //public CategoryAxisValue CategoryAxisValue
        //{
        //    get
        //    {
        //        if (this._CategoryAxisValue == null)
        //            this._CategoryAxisValue = new CategoryAxisValue();

        //        return this._CategoryAxisValue;
        //    }
        //}

        
        

        void IPersistable.Save(ExcelSaveContext context)
        {

            //context.LinqWriter.WriteWrokSheetChart(this,context.);
        }

        void IPersistable.Load(ExcelLoadContext context)
        {
            //XElement drawingsElement = context.Package.GetXElementFromUri(this.ChartUri);
            //context.Reader.ReadCharts(this, drawingsElement,context);

            PackageRelationship drawingsRelation = context.Package.GetPart(this.DrawingUri).GetRelationship(this.RelationshipID);
            this.DrawingUri = PackUriHelper.ResolvePartUri(drawingsRelation.SourceUri, drawingsRelation.TargetUri);
            XElement drawingsElement = context.Package.GetXElementFromUri(this.DrawingUri);
            context.Reader.ReadPieCharts(this, drawingsElement, context);
        }
	}

	public sealed class ExcelOfPieChart : ExcelPieChart
	{
		public ExcelOfPieChart(string chartName, ExcelChartType type, WorkSheet wroksheet)
			: base(chartName, type, wroksheet)
		{
		}

		private ExcelPieType _OfPieType = ExcelPieType.Pie;
		public ExcelPieType OfPieType
		{
			get
			{
				return this._OfPieType;
			}
			internal set
			{
				this._OfPieType = value;
			}
		}
	}

	public sealed class ExcelDoughnutChart : ExcelPieChart
	{
		public ExcelDoughnutChart(string chartName, ExcelChartType type, WorkSheet wroksheet)
			: base(chartName, type, wroksheet)
		{
		}
	}

	public class ExcelBarChart : ExcelChart
	{
		public ExcelBarChart(string chartName, ExcelChartType type, WorkSheet wroksheet)
			: base(chartName, type, wroksheet)
		{
		}
	}

	public sealed class ExcelScatterChart : ExcelChart
	{
		public ExcelScatterChart(string chartName, ExcelChartType type, WorkSheet wroksheet)
			: base(chartName,type, wroksheet)
		{
		}
	}

	public sealed class ExcelLineChart : ExcelChart
	{
		public ExcelLineChart(string chartName, ExcelChartType type, WorkSheet wroksheet)
			: base(chartName,type, wroksheet)
		{
		}

		public bool Marker
		{
			get;
			set;
		}

		public bool Smooth
		{
			get;
			set;
		}

        //private ChartDataLabel _DataLabel = null;
        //public ChartDataLabel DataLabel
        //{
        //    get
        //    {
        //        if (this._DataLabel == null)
        //            this._DataLabel = new ChartDataLabel(this, false);

        //        return this._DataLabel;
        //    }
        //}
	}

	public sealed class ChartCollection : IEnumerable<ExcelChart>
	{
		List<ExcelChart> _List = new List<ExcelChart>();

		private ExcelChart _FirstChart = null;

		internal ChartCollection(ExcelChart firstChart)
		{
			this._FirstChart = firstChart;
			this._List.Add(firstChart);
		}

		public int Count
		{
			get
			{
				return this._List.Count;
			}
		}

		public ExcelChart this[int positionID]
		{
			get
			{
				return this._List[positionID];
			}
		}

		public ExcelChart Add(string chartName, ExcelChartType chartType)
		{
			ExcelChart newChart = ChartFactory.CreateChart(chartType, chartName, this._FirstChart.WorkSheet);

			this._List.Add(newChart);

			return newChart;
		}

		public IEnumerator<ExcelChart> GetEnumerator()
		{
			return this._List.GetEnumerator();
		}

		System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
		{
			return this._List.GetEnumerator();
		}
	}
}
