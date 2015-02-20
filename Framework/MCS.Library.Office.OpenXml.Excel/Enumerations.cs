using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MCS.Library.Core;

namespace MCS.Library.Office.OpenXml.Excel
{
	/// <summary>
	/// 工作薄计算模式
	/// </summary>
	public enum ExcelCalcMode
	{
		/// <summary>
		/// 计算模式设置为“自动”
		/// </summary>
		Automatic,
		/// <summary>
		/// 设置计算模式AutomaticNoTable
		/// </summary>
		AutomaticNoTable,

		/// <summary>
		/// 计算模式设置为“手动”
		/// </summary>
		Manual
	}

	/// <summary>
	/// 设置worksheet可见性
	/// </summary>
	public enum ExcelWorksheetHidden
	{
		/// <summary>
		/// 工作表可见
		/// </summary>
		Visible,
		/// <summary>
		/// 隐藏的工作表，但用户过用户界面可以显示通
		/// </summary>
		Hidden,

		/// <summary>
		/// 工作表是隐藏的，用户通过用户界面无法显示
		/// </summary>
		VeryHidden
	}

	/// <summary>
	/// 填充样式
	/// </summary>
	public enum ExcelFillStyle
	{
		None,
		Solid,
		DarkGray,
		MediumGray,
		LightGray,
		Gray125,
		Gray0625,
		DarkVertical,
		DarkHorizontal,
		DarkDown,
		DarkUp,
		DarkGrid,
		DarkTrellis,
		LightVertical,
		LightHorizontal,
		LightDown,
		LightUp,
		LightGrid,
		LightTrellis
	}

	/// <summary>
	/// 边框样式
	/// </summary>
	public enum ExcelBorderStyle
	{
		None,
		Hair,
		Dotted,
		DashDot,
		Thin,
		DashDotDot,
		Dashed,
		MediumDashDotDot,
		MediumDashed,
		MediumDashDot,
		Thick,
		Medium,
		Double
	}

	/// <summary>
	/// 水平对齐方式 
	/// </summary>
	public enum ExcelHorizontalAlignment
	{
		Left,
		Center,
		CenterContinuous,
		Right,
		Fill,
		Distributed,
		Justify
	}

	/// <summary>
	/// 垂直对齐方式 
	/// </summary>
	public enum ExcelVerticalAlignment
	{
		Top,
		Center,
		Bottom,
		Distributed,
		Justify
	}

	/// <summary>
	/// 字体垂直对齐方式
	/// </summary>
	public enum ExcelVerticalAlignmentFont
	{
		None,
		Subscript,
		Superscript
	}

	/// <summary>
	/// 文本水平对齐方式 
	/// </summary>
	public enum ExcelTextAlignHorizontalVml
	{
		Left,
		Center,
		Right
	}

	/// <summary>
	/// 样式类型
	/// </summary>
	public enum ExcelStyleClass
	{
		Numberformat,
		Font,
		Border,
		BorderTop,
		BorderLeft,
		BorderBottom,
		BorderRight,
		BorderDiagonal,
		Fill,
		FillBackgroundColor,
		FillPatternColor,
		NamedStyle,
		Style
	};

	/// <summary>
	/// 样式属性
	/// </summary>
	public enum ExcelStyleProperty
	{
		Format,
		Name,
		Size,
		Bold,
		Italic,
		Strike,
		Color,
		Family,
		Scheme,
		Underline,
		HorizontalAlign,
		VerticalAlign,
		Border,
		NamedStyle,
		Style,
		PatternType,
		ReadingOrder,
		WrapText,
		TextRotation,
		Locked,
		Hidden,
		ShrinkToFit,
		BorderDiagonalUp,
		BorderDiagonalDown,
		XfId,
		Indent
	}

	/// <summary>
	/// Excel 表格样式
	/// </summary>
	public enum ExcelTableStyles
	{
		None,
		Custom,
		Light1,
		Light2,
		Light3,
		Light4,
		Light5,
		Light6,
		Light7,
		Light8,
		Light9,
		Light10,
		Light11,
		Light12,
		Light13,
		Light14,
		Light15,
		Light16,
		Light17,
		Light18,
		Light19,
		Light20,
		Light21,
		Medium1,
		Medium2,
		Medium3,
		Medium4,
		Medium5,
		Medium6,
		Medium7,
		Medium8,
		Medium9,
		Medium10,
		Medium11,
		Medium12,
		Medium13,
		Medium14,
		Medium15,
		Medium16,
		Medium17,
		Medium18,
		Medium19,
		Medium20,
		Medium21,
		Medium22,
		Medium23,
		Medium24,
		Medium25,
		Medium26,
		Medium27,
		Medium28,
		Dark1,
		Dark2,
		Dark3,
		Dark4,
		Dark5,
		Dark6,
		Dark7,
		Dark8,
		Dark9,
		Dark10,
		Dark11
	}

	/// <summary>
	/// Excel 表计算函数
	/// </summary>
	public enum ExcelRowFunctions
	{
		Average,
		Count,
		CountNums,
		Custom,
		Max,
		Min,
		None,
		StdDev,
		Sum,
		Var
	}

	/// <summary>
	/// excel 单元格公式类型
	/// </summary>
	public enum ExcelCellFormulaType
	{
		Normal,
		Array,
		DataTable,
		Shared
	}

	/// <summary>
	/// 打印设置
	/// </summary>
	public enum ExcelOrientation
	{
		Default,
		Portrait,
		Landscape
	}

	/// <summary>
	///指定打印的页面顺序
	/// </summary>
	public enum ExcelPageOrder
	{
		/// <summary>
		/// 垂直页面，然后水平移动。
		/// </summary>
		DownThenOver,
		/// <summary>
		///水平打印，垂直移动
		/// </summary>
		OverThenDown
	}

	public enum ExcelPaperSize
	{
		/// <summary>
		/// Letter paper (8.5 in. by 11 in.)
		/// </summary>
		Letter = 1,
		/// <summary>
		/// Letter small paper (8.5 in. by 11 in.)
		/// </summary>
		LetterSmall = 2,
		/// <summary>
		/// // Tabloid paper (11 in. by 17 in.)
		/// </summary>
		Tabloid = 3,
		/// <summary>
		/// Ledger paper (17 in. by 11 in.)
		/// </summary>
		Ledger = 4,
		/// <summary>
		/// Legal paper (8.5 in. by 14 in.)
		/// </summary>
		Legal = 5,
		/// <summary>
		/// Statement paper (5.5 in. by 8.5 in.)
		/// </summary>
		Statement = 6,
		/// <summary>
		/// Executive paper (7.25 in. by 10.5 in.)
		/// </summary>
		Executive = 7,
		/// <summary>
		/// A3 paper (297 mm by 420 mm)
		/// </summary>
		A3 = 8,
		/// <summary>
		/// A4 paper (210 mm by 297 mm)
		/// </summary>
		A4 = 9,
		/// <summary>
		/// A4 small paper (210 mm by 297 mm)
		/// </summary>
		A4Small = 10,
		/// <summary>
		/// A5 paper (148 mm by 210 mm)
		/// </summary>
		A5 = 11,
		/// <summary>
		/// B4 paper (250 mm by 353 mm)
		/// </summary>
		B4 = 12,
		/// <summary>
		/// B5 paper (176 mm by 250 mm)
		/// </summary>
		B5 = 13,
		/// <summary>
		/// Folio paper (8.5 in. by 13 in.)
		/// </summary>
		Folio = 14,
		/// <summary>
		/// Quarto paper (215 mm by 275 mm)
		/// </summary>
		Quarto = 15,
		/// <summary>
		/// Standard paper (10 in. by 14 in.)
		/// </summary>
		Standard10_14 = 16,
		/// <summary>
		/// Standard paper (11 in. by 17 in.)
		/// </summary>
		Standard11_17 = 17,
		/// <summary>
		/// Note paper (8.5 in. by 11 in.)
		/// </summary>
		Note = 18,
		/// <summary>
		/// #9 envelope (3.875 in. by 8.875 in.)
		/// </summary>
		Envelope9 = 19,
		/// <summary>
		/// #10 envelope (4.125 in. by 9.5 in.)
		/// </summary>
		Envelope10 = 20,
		/// <summary>
		/// #11 envelope (4.5 in. by 10.375 in.)
		/// </summary>
		Envelope11 = 21,
		/// <summary>
		/// #12 envelope (4.75 in. by 11 in.)
		/// </summary>
		Envelope12 = 22,
		/// <summary>
		/// #14 envelope (5 in. by 11.5 in.)
		/// </summary>
		Envelope14 = 23,
		/// <summary>
		/// C paper (17 in. by 22 in.)
		/// </summary>
		C = 24,
		/// <summary>
		/// D paper (22 in. by 34 in.)
		/// </summary>
		D = 25,
		/// <summary>
		/// E paper (34 in. by 44 in.)
		/// </summary>
		E = 26,
		/// <summary>
		/// DL envelope (110 mm by 220 mm)
		/// </summary>
		DLEnvelope = 27,
		/// <summary>
		/// C5 envelope (162 mm by 229 mm)
		/// </summary>
		C5Envelope = 28,
		/// <summary>
		/// C3 envelope (324 mm by 458 mm)
		/// </summary>
		C3Envelope = 29,
		/// <summary>
		/// C4 envelope (229 mm by 324 mm)
		/// </summary>
		C4Envelope = 30,
		/// <summary>
		/// C6 envelope (114 mm by 162 mm)
		/// </summary>
		C6Envelope = 31,
		/// <summary>
		/// C65 envelope (114 mm by 229 mm)
		/// </summary>
		C65Envelope = 32,
		/// <summary>
		/// B4 envelope (250 mm by 353 mm)
		/// </summary>
		B4Envelope = 33,
		/// <summary>
		/// B5 envelope (176 mm by 250 mm)
		/// </summary>
		B5Envelope = 34,
		/// <summary>
		/// B6 envelope (176 mm by 125 mm)
		/// </summary>
		B6Envelope = 35,
		/// <summary>
		/// Italy envelope (110 mm by 230 mm)
		/// </summary>
		ItalyEnvelope = 36,
		/// <summary>
		/// Monarch envelope (3.875 in. by 7.5 in.).
		/// </summary>
		MonarchEnvelope = 37,
		/// <summary>
		/// 6 3/4 envelope (3.625 in. by 6.5 in.)
		/// </summary>
		Six3_4Envelope = 38,
		/// <summary>
		/// US standard fanfold (14.875 in. by 11 in.)
		/// </summary>
		USStandard = 39,
		/// <summary>
		/// German standard fanfold (8.5 in. by 12 in.)
		/// </summary>
		GermanStandard = 40,
		/// <summary>
		/// German legal fanfold (8.5 in. by 13 in.)
		/// </summary>
		GermanLegal = 41,
		/// <summary>
		/// ISO B4 (250 mm by 353 mm)
		/// </summary>
		ISOB4 = 42,
		/// <summary>
		///  Japanese double postcard (200 mm by 148 mm)
		/// </summary>
		JapaneseDoublePostcard = 43,
		/// <summary>
		/// Standard paper (9 in. by 11 in.)
		/// </summary>
		Standard9 = 44,
		/// <summary>
		/// Standard paper (10 in. by 11 in.)
		/// </summary>
		Standard10 = 45,
		/// <summary>
		/// Standard paper (15 in. by 11 in.)
		/// </summary>
		Standard15 = 46,
		/// <summary>
		/// Invite envelope (220 mm by 220 mm)
		/// </summary>
		InviteEnvelope = 47,
		/// <summary>
		/// Letter extra paper (9.275 in. by 12 in.)
		/// </summary>
		LetterExtra = 50,
		/// <summary>
		/// Legal extra paper (9.275 in. by 15 in.)
		/// </summary>
		LegalExtra = 51,
		/// <summary>
		/// Tabloid extra paper (11.69 in. by 18 in.)
		/// </summary>
		TabloidExtra = 52,
		/// <summary>
		/// A4 extra paper (236 mm by 322 mm)
		/// </summary>
		A4Extra = 53,
		/// <summary>
		/// Letter transverse paper (8.275 in. by 11 in.)
		/// </summary>
		LetterTransverse = 54,
		/// <summary>
		/// A4 transverse paper (210 mm by 297 mm)
		/// </summary>
		A4Transverse = 55,
		/// <summary>
		/// Letter extra transverse paper (9.275 in. by 12 in.)
		/// </summary>
		LetterExtraTransverse = 56,
		/// <summary>
		/// SuperA/SuperA/A4 paper (227 mm by 356 mm)
		/// </summary>
		SuperA = 57,
		/// <summary>
		/// SuperB/SuperB/A3 paper (305 mm by 487 mm)
		/// </summary>
		SuperB = 58,
		/// <summary>
		/// Letter plus paper (8.5 in. by 12.69 in.)
		/// </summary>
		LetterPlus = 59,
		/// <summary>
		/// A4 plus paper (210 mm by 330 mm)
		/// </summary>
		A4Plus = 60,
		/// <summary>
		/// A5 transverse paper (148 mm by 210 mm)
		/// </summary>
		A5Transverse = 61,
		/// <summary>
		/// JIS B5 transverse paper (182 mm by 257 mm)
		/// </summary>
		JISB5Transverse = 62,
		/// <summary>
		/// A3 extra paper (322 mm by 445 mm)
		/// </summary>
		A3Extra = 63,
		/// <summary>
		/// A5 extra paper (174 mm by 235 mm)
		/// </summary>
		A5Extra = 64,
		/// <summary>
		/// ISO B5 extra paper (201 mm by 276 mm)
		/// </summary>
		ISOB5 = 65,
		/// <summary>
		/// A2 paper (420 mm by 594 mm)
		/// </summary>
		A2 = 66,
		/// <summary>
		/// A3 transverse paper (297 mm by 420 mm)
		/// </summary>
		A3Transverse = 67,
		/// <summary>
		/// A3 extra transverse paper (322 mm by 445 mm*/
		/// </summary>
		A3ExtraTransverse = 68
	}

	/// <summary>
	/// 打印工作表批注的显示方式
	/// </summary>
	public enum ExcelCellComments
	{

		/// <summary>
		/// 打印的单元格批注
		/// </summary>
		AsDisplayed,
		/// <summary>
		/// 打印在文档的末尾
		/// </summary>
		AtEnd,

		/// <summary>
		/// 不打印单元格批注。
		/// </summary>
		None
	}

	/// <summary>
	/// 定义(ExcelSheetView)的视图设置。
	/// </summary>
	public enum ExcelSheetViewType
	{
		/// <summary>
		/// 普通视图
		/// </summary>
		Normal,

		/// <summary>
		/// 分页预览
		/// </summary>
		PageBreakPreview,

		/// <summary>
		/// 页面布局视图
		/// </summary>
		PageLayout
	}

	/// <summary>
	/// 线的类型
	/// </summary>
	public enum ExcelUnderLineType
	{
		Dash,
		DashHeavy,
		DashLong,
		DashLongHeavy,
		Double,
		DotDash,
		DotDashHeavy,
		DotDotDash,
		DotDotDashHeavy,
		Dotted,
		DottedHeavy,
		Heavy,
		None,
		Single,
		Wavy,
		WavyDbl,
		WavyHeavy,
		Words
	}

	public enum ExcelStrikeType
	{
		Double,
		No,
		Single
	}

	/// <summary>
	/// 文本垂直对齐方式
	/// </summary>
	public enum ExcelTextAlignVerticalVml
	{
		Top,
		Center,
		Bottom
	}

	/// <summary>
	/// 线型
	/// </summary>
	public enum ExcelLineStyleVml
	{
		Solid,
		Round,
		Square,
		Dash,
		DashDot,
		LongDash,
		LongDashDot,
		LongDashDotDot
	}

	/// <summary>
	/// 图片对齐方式
	/// </summary>
	public enum ExcelPictureAlignment
	{
		Left,
		Centered,
		Right
	}

	public enum ExcelEditAs
	{
		/// <summary>
		/// 绝对位置 （开始结束地址)
		/// </summary>
		Absolute,

		/// <summary>
		/// 当前位图，指定行与列，保持图片固定大小
		/// </summary>
		OneCell,

		/// <summary>
		/// 指定当前图形的移动实际的行与列
		/// </summary>
		TwoCell
	}

	public enum ExcelDrawingFillStyle
	{
		NoFill,
		SolidFill,
		GradientFill,
		PatternFill,
		BlipFill,
		GroupFill
	}

	public enum ExcelDrawingLineStyle
	{
		Dash,
		DashDot,
		Dot,
		LongDash,
		LongDashDot,
		LongDashDotDot,
		Solid,
		SystemDash,
		SystemDashDot,
		SystemDashDotDot,
		SystemDot
	}

	public enum ExcelDrawingLineCap
	{
		Flat,   //flat
		Round,  //rnd
		Square  //Sq
	}

	#region ”DataValidation“
	public enum ExcelDataValidationType
	{
		/// <summary>
		/// 整数
		/// </summary>
		Whole,
		/// <summary>
		/// 小数
		/// </summary>
		Decimal,
		/// <summary>
		/// 列表
		/// </summary>
		List,
		/// <summary>
		/// 文本长度
		/// </summary>
		TextLength,
		/// <summary>
		/// 日期
		/// </summary>
		DateTime,
		/// <summary>
		/// 时间
		/// </summary>
		Time,
		/// <summary>
		/// 自定义
		/// </summary>
		Custom
	}

	/// <summary>
	/// 处理无效数据类型
	/// </summary>
	public enum ExcelDataValidationWarningStyle
	{
		/// <summary>
		/// 警告样式除外
		/// </summary>
		undefined,
		/// <summary>
		/// 停止
		/// </summary>
		stop,
		/// <summary>
		/// 警告
		/// </summary>
		warning,
		/// <summary>
		/// 信息
		/// </summary>
		information
	}

	public enum ExcelDataValidationOperator
	{
		any,
		equal,
		notEqual,
		lessThan,
		lessThanOrEqual,
		greaterThan,
		greaterThanOrEqual,
		between,
		notBetween
	}

	internal enum ExcelDataValidationFormulaState
	{
		Value,
		Formula
	}

	#endregion

	#region “Chart"
	public enum ExcelChartType
	{
		Area3D = -4098,
		AreaStacked3D = 78,
		AreaStacked1003D = 79,
		BarClustered3D = 60,
		BarStacked3D = 61,
		BarStacked1003D = 62,
		Column3D = -4100,
		ColumnClustered3D = 54,
		ColumnStacked3D = 55,
		ColumnStacked1003D = 56,
		Line3D = -4101,
		Pie3D = -4102,
		PieExploded3D = 70,
		Area = 1,
		AreaStacked = 76,
		AreaStacked100 = 77,
		BarClustered = 57,
		BarOfPie = 71,
		BarStacked = 58,
		BarStacked100 = 59,
		Bubble = 15,
		Bubble3DEffect = 87,
		ColumnClustered = 51,
		ColumnStacked = 52,
		ColumnStacked100 = 53,
		ConeBarClustered = 102,
		ConeBarStacked = 103,
		ConeBarStacked100 = 104,
		ConeCol = 105,
		ConeColClustered = 99,
		ConeColStacked = 100,
		ConeColStacked100 = 101,
		CylinderBarClustered = 95,
		CylinderBarStacked = 96,
		CylinderBarStacked100 = 97,
		CylinderCol = 98,
		CylinderColClustered = 92,
		CylinderColStacked = 93,
		CylinderColStacked100 = 94,
		Doughnut = -4120,
		DoughnutExploded = 80,
		Line = 4,
		LineMarkers = 65,
		LineMarkersStacked = 66,
		LineMarkersStacked100 = 67,
		LineStacked = 63,
		LineStacked100 = 64,
		Pie = 5,
		PieExploded = 69,
		PieOfPie = 68,
		PyramidBarClustered = 109,
		PyramidBarStacked = 110,
		PyramidBarStacked100 = 111,
		PyramidCol = 112,
		PyramidColClustered = 106,
		PyramidColStacked = 107,
		PyramidColStacked100 = 108,
		Radar = -4151,
		RadarFilled = 82,
		RadarMarkers = 81,
		StockHLC = 88,
		StockOHLC = 89,
		StockVHLC = 90,
		StockVOHLC = 91,
		Surface = 83,
		SurfaceTopView = 85,
		SurfaceTopViewWireframe = 86,
		SurfaceWireframe = 84,
		XYScatter = -4169,
		XYScatterLines = 74,
		XYScatterLinesNoMarkers = 75,
		XYScatterSmooth = 72,
		XYScatterSmoothNoMarkers = 73
	}

	public enum ExcelChartStyle
	{
		None,
		Style1,
		Style2,
		Style3,
		Style4,
		Style5,
		Style6,
		Style7,
		Style8,
		Style9,
		Style10,
		Style11,
		Style12,
		Style13,
		Style14,
		Style15,
		Style16,
		Style17,
		Style18,
		Style19,
		Style20,
		Style21,
		Style22,
		Style23,
		Style24,
		Style25,
		Style26,
		Style27,
		Style28,
		Style29,
		Style30,
		Style31,
		Style32,
		Style33,
		Style34,
		Style35,
		Style36,
		Style37,
		Style38,
		Style39,
		Style40,
		Style41,
		Style42,
		Style43,
		Style44,
		Style45,
		Style46,
		Style47,
		Style48
	}

	public enum ExcelTextAnchoringType
	{
		Bottom,
		Center,
		Distributed,
		Justify,
		Top
	}

	public enum ExcelTextVerticalType
	{
		EastAsianVertical,
		Horizontal,
		MongolianVertical,
		Vertical,
		Vertical270,
		WordArtVertical,
		WordArtVerticalRightToLeft
	}

	public enum ExcelTrendLine
	{
		Exponential,
		Linear,
		Logarithmic,
		MovingAvgerage,
		Polynomial,
		Power
	}

	/// <summary>
	/// 标记样式
	/// </summary>
	public enum ExcelMarkerStyle
	{
		Circle,
		Dash,
		Diamond,
		Dot,
		None,
		Picture,
		Plus,
		Square,
		Star,
		Triangle,
		X,
	}

	public enum ExcelLabelPosition
	{
		BestFit,
		Left,
		Right,
		Center,
		Top,
		Bottom,
		InBase,
		InEnd,
		OutEnd
	}

	public enum ExcelAxisTickMark
	{
		Cross,
		In,
		None,
		Out
	}

	internal enum ExcelAxisType
	{
		Val,
		Cat,
		Date
	}

	public enum ExcelAxisPosition
	{
		Left = 0,
		Bottom = 1,
		Right = 2,
		Top = 3
	}

	public enum ExcelCrosses
	{
		AutoZero,
		Max,
		Min
	}

	public enum ExcelCrossBetween
	{
		Between,
		MidCat
	}

	public enum ExcelTickLabelPosition
	{
		High,
		Low,
		NextTo,
		None
	}

	public enum ExcelAxisOrientation
	{
		MaxMin,
		MinMax
	}

	public enum ExcelDisplayBlanksAs
	{
		Gap,
		Span,
		Zero
	}

	public enum ExcelPieType
	{
		Bar,
		Pie
	}

	public enum ExcelLegendPosition
	{
		Top,
		Left,
		Right,
		Bottom,
		TopRight
	}

	public enum ExcelGrouping
	{
		Standard,
		Clustered,
		Stacked,
		PercentStacked
	}

	public enum ExcelTextHorzOverflowType
	{
		Clip,
		Overflow
	}
	#endregion
}
