using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MCS.Library.Core;
using MCS.Library.Office.OpenXml.Excel.Styles;
using System.Xml;
using System.IO.Packaging;
using System.IO;
using System.Globalization;

namespace MCS.Library.Office.OpenXml.Excel
{
	public class WorkBookStylesWrapper : IPersistable
	{
		// private Uri _StylesUri = new Uri("/xl/styles.xml", UriKind.Relative);

		private const string NumberFormatsPath = "d:styleSheet/d:numFmts";
		private const string FontsPath = "d:styleSheet/d:fonts";
		private const string FillsPath = "d:styleSheet/d:fills";
		private const string BordersPath = "d:styleSheet/d:borders";
		private const string CellStyleXfsPath = "d:styleSheet/d:cellStyleXfs";
		private const string CellXfsPath = "d:styleSheet/d:cellXfs";
		private const string CellStylesPath = "d:styleSheet/d:cellStyles";

		public StyleCollection<NumberFormatXmlWrapper> NumberFormats = new StyleCollection<NumberFormatXmlWrapper>();
		public StyleCollection<FontXmlWrapper> Fonts = new StyleCollection<FontXmlWrapper>();
		public StyleCollection<FillXmlWrapper> Fills = new StyleCollection<FillXmlWrapper>();
		public StyleCollection<BorderXmlWrapper> Borders = new StyleCollection<BorderXmlWrapper>();
		public StyleCollection<CellStyleXmlWrapper> CellStyleXfs = new StyleCollection<CellStyleXmlWrapper>();
		public StyleCollection<CellStyleXmlWrapper> CellXfs = new StyleCollection<CellStyleXmlWrapper>();
		public StyleCollection<NamedStyleXmlWrapper> NamedStyles = new StyleCollection<NamedStyleXmlWrapper>();

		private WorkBook _WorkBook;

		internal WorkBookStylesWrapper(WorkBook workbook)
		{
			this._WorkBook = workbook;

			// NumberFormatXmlWrapper.AddBuildIn(this.NumberFormats);
		}

		internal WorkBookStylesWrapper()
		{
		}

		private SheetTableStyles _TableStyles;
		public SheetTableStyles TableStyles
		{
			get
			{
				if (this._TableStyles == null)
				{
					this._TableStyles = new SheetTableStyles();
				}
				return this._TableStyles;
			}
		}

		#region “Save”
		void IPersistable.Save(ExcelSaveContext context)
		{
			context.LinqWriter.WriteStyles(this);
		}
		#endregion

		void IPersistable.Load(ExcelLoadContext context)
		{
			if (context.Package.PartExists(ExcelCommon.Uri_Styles))
			{
				var root = context.Package.GetXElementFromUri(ExcelCommon.Uri_Styles);
				context.Reader.ReadStyles(root, this);
			}
		}
	}
}