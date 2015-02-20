using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO.Packaging;
using System.Xml.Linq;
using System.Drawing;
using System.Drawing.Imaging;
using MCS.Library.Core;

namespace MCS.Library.Office.OpenXml.Excel
{
	public sealed class DrawingCollection : IEnumerable<ExcelDrawing>, IPersistable
	{
		private List<ExcelDrawing> _Drawings = new List<ExcelDrawing>();
		private Dictionary<string, int> _DrawingNames = new Dictionary<string, int>();

		internal WorkSheet _WorkSheet;

		public DrawingCollection(WorkSheet worksheet)
		{
			this._WorkSheet = worksheet;
		}

		internal DrawingCollection(WorkSheet worksheet, string relationshipId)
			: this(worksheet)
		{
			this.RelationshipID = relationshipId;
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

        public List<ExcelDrawing> this[string name]
        {
            get
            {
                List<ExcelDrawing> drawCol = new List<ExcelDrawing>();
                foreach (ExcelDrawing item in this)
                {
                    if (item.Name == name)
                    {
                        drawCol.Add(item);
                    }
                }
                return drawCol;
            }
        }

		void IPersistable.Save(ExcelSaveContext context)
		{
			context.LinqWriter.WriteWrokSheetDrawing(this);
		}

		void IPersistable.Load(ExcelLoadContext context)
		{
			PackageRelationship drawingsRelation = context.Package.GetPart(this._WorkSheet.SheetUri).GetRelationship(this.RelationshipID);
			this.DrawingUri = PackUriHelper.ResolvePartUri(drawingsRelation.SourceUri, drawingsRelation.TargetUri);
			XElement drawingsElement = context.Package.GetXElementFromUri(this.DrawingUri);
            context.Reader.ReadWrokSheetDrawings(this, drawingsElement, context);
		}

		/// <summary>
		/// 添加 一个对象
		/// </summary>
		/// <param name="drawing"></param>
		internal void AddWrapper(string name, ExcelDrawing drawing)
		{
			this._Drawings.Add(drawing);
			this._DrawingNames.Add(name.ToLower(), this._Drawings.Count - 1);
            
		}

		public ExcelPicture AddPicture(string name, Image image, ImageFormat imageFormat)
		{
			ExcelPicture pic = new ExcelPicture(this._WorkSheet) { Name = name, Image = image, ImageFormat = imageFormat };
			pic.EditAs = ExcelEditAs.OneCell;
			pic.SetPosDefaults(image);
			this.AddWrapper(name, pic);

			return pic;
		}

		public ExcelChart AddChart(string chartName, ExcelChartType chartType)
		{
			ExceptionHelper.TrueThrow(this._DrawingNames.ContainsKey(chartName.ToLower()), "已存在相同名{0}", chartName);

			ExceptionHelper.TrueThrow<NotImplementedException>(chartType == ExcelChartType.Bubble || chartType == ExcelChartType.Bubble3DEffect || chartType == ExcelChartType.Radar ||
			chartType == ExcelChartType.RadarFilled || chartType == ExcelChartType.RadarMarkers || chartType == ExcelChartType.StockHLC ||
			chartType == ExcelChartType.StockOHLC || chartType == ExcelChartType.StockVOHLC || chartType == ExcelChartType.Surface ||
			chartType == ExcelChartType.SurfaceTopView || chartType == ExcelChartType.SurfaceTopViewWireframe || chartType == ExcelChartType.SurfaceWireframe, "在当前版本不支持图表类型");

			ExcelChart newChart = ChartFactory.CreateChart(chartType, chartName, this._WorkSheet);
			this.AddWrapper(chartName, newChart);

			return newChart;
		}

		public int Count
		{
			get
			{
				return this._Drawings.Count;
			}
		}

		public IEnumerator<ExcelDrawing> GetEnumerator()
		{
			return this._Drawings.GetEnumerator();
		}

		System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
		{
			return this._Drawings.GetEnumerator();
		}
	}
}
