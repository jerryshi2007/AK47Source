using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Linq;
using System.Xml;

namespace MCS.Library.Office.OpenXml.Excel
{
	internal static class ExcelCommon
	{
		internal static class HeaderFooter
		{
			/// <summary>
			/// 当前页码 "current page #"
			/// </summary>
			public static readonly string PageNumber = @"&P";

			/// <summary>
			/// 总页数码 "total pages"
			/// </summary>
			public static readonly string NumberOfPages = @"&N";

			/// <summary>
			/// 文本的字体颜色
			/// RGB颜色指定为RRGGBB
			/// 指定的主题颜色是TTSNN TT是主题颜色标识，S是不是“+”或“ - ”色调/明暗值，NN是色调/明暗值。
			/// </summary>
			public static readonly string FontColor = @"&K";

			/// <summary>
			/// 工作表标签名称码
			/// </summary>
			public static readonly string SheetName = @"&A";

			/// <summary>
			/// 文件路径 码
			/// </summary>
			public static readonly string FilePath = @"&Z";

			/// <summary>
			/// 文优名称
			/// </summary>
			public static readonly string FileName = @"&F";

			/// <summary>
			/// 当前日期
			/// </summary>
			public static readonly string CurrentDate = @"&D";

			/// <summary>
			/// 时间码
			/// </summary>
			public static readonly string CurrentTime = @"&T";

			/// <summary>
			/// 背景图片码
			/// </summary>
			public static readonly string Image = @"&G";

			/// <summary>
			/// 提纲样式
			/// </summary>
			public static readonly string OutlineStyle = @"&O";

			/// <summary>
			/// 阴影样式码
			/// </summary>
			public static readonly string ShadowStyle = @"&H";

		}

		internal static class ExcelDrawing
		{
			internal static readonly int EMU_PER_PIXEL = 9525;
			internal static readonly float STANDARD_DPI = 96;
		}

		internal static class DataValidationSchemaNames
		{
			public const string Whole = "whole";
			public const string Decimal = "decimal";
			public const string List = "list";
			public const string TextLength = "textLength";
			public const string Date = "date";
			public const string Time = "time";
			public const string Custom = "custom";
		}

		public static readonly XNamespace Schema_WorkBook_Main = @"http://schemas.openxmlformats.org/spreadsheetml/2006/main";
		public const string Schema_Relationships = @"http://schemas.openxmlformats.org/officeDocument/2006/relationships";

		public const string Schema_Relationships_PrinterSettings = @"http://purl.oclc.org/ooxml/officeDocument/relationships/printerSettings";

		public  const string Schema_Drawings = @"http://schemas.openxmlformats.org/drawingml/2006/main";
		public const string Schema_SheetDrawings = @"http://schemas.openxmlformats.org/drawingml/2006/spreadsheetDrawing";

		public const string Schema_Chart = @"http://schemas.openxmlformats.org/drawingml/2006/chart";
		public const string Schema_Hyperlink = @"http://schemas.openxmlformats.org/officeDocument/2006/relationships/hyperlink";
		public const string Schema_Comment = @"http://schemas.openxmlformats.org/officeDocument/2006/relationships/comments";
		public const string Schema_Image = @"http://schemas.openxmlformats.org/officeDocument/2006/relationships/image";
		//Office properties
		public const string Schema_Cp = @"http://schemas.openxmlformats.org/package/2006/metadata/core-properties";
		public const string Schema_Extended = @"http://schemas.openxmlformats.org/officeDocument/2006/extended-properties";
		public const string Schema_Custom = @"http://schemas.openxmlformats.org/officeDocument/2006/custom-properties";
		public const string Schema_Dc = @"http://purl.org/dc/elements/1.1/";
		public const string Schema_DcTerms = @"http://purl.org/dc/terms/";
		public const string Schema_DcmiType = @"http://purl.org/dc/dcmitype/";
		public const string Schema_Xsi = @"http://www.w3.org/2001/XMLSchema-instance";
		public const string Schema_Vt = @"http://schemas.openxmlformats.org/officeDocument/2006/docPropsVTypes";

		public const string Schema_Xml = @"http://www.w3.org/XML/1998/namespace";
        public const string Schema_Ext = @"http://schemas.microsoft.com/office/spreadsheetml/2009/9/main";
        public const string Schema_Ext_Sqref = @"http://schemas.microsoft.com/office/excel/2006/main";
        

		//public static readonly string[] SchemaNodeOrder = new string[] { "fileVersion", "fileSharing", "workbookPr", "workbookProtection", "bookViews", "sheets", "functionGroups", "functionPrototypes", "externalReferences", "definedNames", "calcPr", "oleSize", "customWorkbookViews", "pivotCaches", "smartTagPr", "smartTagTypes", "webPublishing", "fileRecoveryPr", };

		public const int WorkSheet_MaxColumns = 16384;
		public const int WorkSheet_MaxRows = 1048576;
		public const double WorkSheet_DefaultRowHeight = 15;
		public const double WorkSheet_DefaultColumnWidth = 9.140625;

		public const string ContentType_WorkBook = @"application/vnd.openxmlformats-officedocument.spreadsheetml.sheet.main+xml";
		public const string ContentType_WorkSheet = @"application/vnd.openxmlformats-officedocument.spreadsheetml.worksheet+xml";
		public const string ContentType_SharedString = @"application/vnd.openxmlformats-officedocument.spreadsheetml.sharedStrings+xml";
		public const string ContentType_Theme = @"application/vnd.openxmlformats-officedocument.theme+xml";
		public const string ContentType_Styles = @"application/vnd.openxmlformats-officedocument.spreadsheetml.styles+xml";
		public const string ContentType_CalculationChain = @"application/vnd.openxmlformats-officedocument.spreadsheetml.calcChain+xml";
		public const string ContentType_Table = @"application/vnd.openxmlformats-officedocument.spreadsheetml.table+xml";
		public const string ContentType_PrinterSettings = @"application/vnd.openxmlformats-officedocument.spreadsheetml.printerSettings";
		public const string ContentType_Comments = @"application/vnd.openxmlformats-officedocument.spreadsheetml.comments+xml";
		public const string ContentType_vmlDrawing = @"application/vnd.openxmlformats-officedocument.vmlDrawing";
		public const string ContentType_sheetDrawing = @"application/vnd.openxmlformats-officedocument.drawing+xml";
		public const string ContentType_sheetChart = @"application/vnd.openxmlformats-officedocument.drawingml.chart+xml";

		public static readonly Uri Uri_Workbook = new Uri("/xl/workbook.xml", UriKind.Relative);
		public static readonly Uri Uri_Styles = new Uri("/xl/styles.xml", UriKind.Relative);
		public static readonly Uri Uri_Theme = new Uri("/xl/theme/theme1.xml", UriKind.Relative);
		public static readonly Uri Uri_SharedStrings = new Uri("/xl/sharedStrings.xml", UriKind.Relative);
		public static readonly Uri Uri_PropertiesCore = new Uri("/docProps/core.xml", UriKind.Relative);
		public static readonly Uri Uri_PropertiesExtended = new Uri("/docProps/app.xml", UriKind.Relative);
		public static readonly Uri Uri_PropertiesCustom = new Uri("/docProps/custom.xml", UriKind.Relative);
		public static readonly Uri Uri_CalculationChain = new Uri("/xl/calcChain.xml", UriKind.Relative);

		public const string schemaMicrosoftVml = @"urn:schemas-microsoft-com:vml";
		public const string schemaMicrosoftOffice = "urn:schemas-microsoft-com:office:office";
		public const string schemaMicrosoftExcel = "urn:schemas-microsoft-com:office:excel";

		public static readonly XmlNamespaceManager DefaultNameSpaceManager;
		static ExcelCommon()
		{
			DefaultNameSpaceManager = CreateDefaultWorkBookXmlNamespace();
		}

		private static XmlNamespaceManager CreateDefaultWorkBookXmlNamespace()
		{
			NameTable nt = new NameTable();
			XmlNamespaceManager ns = new XmlNamespaceManager(nt);
			ns.AddNamespace(string.Empty, ExcelCommon.Schema_WorkBook_Main.NamespaceName);
			ns.AddNamespace("d", ExcelCommon.Schema_WorkBook_Main.NamespaceName);
			ns.AddNamespace("vt", ExcelCommon.Schema_Vt);
			// extended properties (app.xml)
			ns.AddNamespace("xp", ExcelCommon.Schema_Extended);
			// custom properties
			ns.AddNamespace("ctp", ExcelCommon.Schema_Custom);
			ns.AddNamespace("cp", ExcelCommon.Schema_Cp);
			ns.AddNamespace("dc", ExcelCommon.Schema_Dc);
			ns.AddNamespace("dcterms", ExcelCommon.Schema_DcTerms);
			ns.AddNamespace("dcmitype", ExcelCommon.Schema_DcmiType);
			ns.AddNamespace("xsi", ExcelCommon.Schema_Xsi);

			return ns;
		}
	}
}
