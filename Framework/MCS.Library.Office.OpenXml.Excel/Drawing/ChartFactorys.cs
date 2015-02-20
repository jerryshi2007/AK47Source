using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MCS.Library.Office.OpenXml.Excel
{
	internal static class ChartFactory
	{
		public static ExcelChart CreateChart(ExcelChartType chartType,string chartName, WorkSheet worksheet)
		{
			switch (chartType)
			{
				case ExcelChartType.Pie:
				case ExcelChartType.PieExploded:
				case ExcelChartType.Pie3D:
				case ExcelChartType.PieExploded3D:
					return new ExcelPieChart(chartName,chartType, worksheet);
				case ExcelChartType.BarOfPie:
				case ExcelChartType.PieOfPie:
					return new ExcelOfPieChart(chartName, chartType, worksheet);
				case ExcelChartType.Doughnut:
				case ExcelChartType.DoughnutExploded:
					return new ExcelDoughnutChart(chartName, chartType, worksheet);
				case ExcelChartType.BarClustered:
				case ExcelChartType.BarStacked:
				case ExcelChartType.BarStacked100:
				case ExcelChartType.BarClustered3D:
				case ExcelChartType.BarStacked3D:
				case ExcelChartType.BarStacked1003D:
				case ExcelChartType.ConeBarClustered:
				case ExcelChartType.ConeBarStacked:
				case ExcelChartType.ConeBarStacked100:
				case ExcelChartType.CylinderBarClustered:
				case ExcelChartType.CylinderBarStacked:
				case ExcelChartType.CylinderBarStacked100:
				case ExcelChartType.PyramidBarClustered:
				case ExcelChartType.PyramidBarStacked:
				case ExcelChartType.PyramidBarStacked100:
				case ExcelChartType.ColumnClustered:
				case ExcelChartType.ColumnStacked:
				case ExcelChartType.ColumnStacked100:
				case ExcelChartType.Column3D:
				case ExcelChartType.ColumnClustered3D:
				case ExcelChartType.ColumnStacked3D:
				case ExcelChartType.ColumnStacked1003D:
				case ExcelChartType.ConeCol:
				case ExcelChartType.ConeColClustered:
				case ExcelChartType.ConeColStacked:
				case ExcelChartType.ConeColStacked100:
				case ExcelChartType.CylinderCol:
				case ExcelChartType.CylinderColClustered:
				case ExcelChartType.CylinderColStacked:
				case ExcelChartType.CylinderColStacked100:
				case ExcelChartType.PyramidCol:
				case ExcelChartType.PyramidColClustered:
				case ExcelChartType.PyramidColStacked:
				case ExcelChartType.PyramidColStacked100:
					return new ExcelBarChart(chartName, chartType, worksheet);
				case ExcelChartType.XYScatter:
				case ExcelChartType.XYScatterLines:
				case ExcelChartType.XYScatterLinesNoMarkers:
				case ExcelChartType.XYScatterSmooth:
				case ExcelChartType.XYScatterSmoothNoMarkers:
					return new ExcelScatterChart(chartName,chartType, worksheet);
				case ExcelChartType.Line:
				case ExcelChartType.Line3D:
				case ExcelChartType.LineMarkers:
				case ExcelChartType.LineMarkersStacked:
				case ExcelChartType.LineMarkersStacked100:
				case ExcelChartType.LineStacked:
				case ExcelChartType.LineStacked100:
					return new ExcelLineChart(chartName, chartType, worksheet);
				default:
					return new ExcelChart(chartName,chartType, worksheet);
			}
		}
	}

	internal static class ChartSerieFactory
	{
		public static ChartSerie CreateChartSerie(ExcelChart chart, bool isPivot, Range seriesAddress, Range xSeriesAddress)
		{
			ChartSerie result = null ;
			switch (chart.ChartType)
			{
				case ExcelChartType.XYScatter:
				case ExcelChartType.XYScatterLines:
				case ExcelChartType.XYScatterLinesNoMarkers:
				case ExcelChartType.XYScatterSmooth:
				case ExcelChartType.XYScatterSmoothNoMarkers:
					result = new ScatterChartSerie(chart, isPivot);
					break;
				case ExcelChartType.Pie:
				case ExcelChartType.Pie3D:
				case ExcelChartType.PieExploded:
				case ExcelChartType.PieExploded3D:
				case ExcelChartType.PieOfPie:
				case ExcelChartType.Doughnut:
				case ExcelChartType.DoughnutExploded:
				case ExcelChartType.BarOfPie:
					result = new PieChartSerie(chart, isPivot);
					break;
				case ExcelChartType.Line:
				case ExcelChartType.LineMarkers:
				case ExcelChartType.LineMarkersStacked:
				case ExcelChartType.LineMarkersStacked100:
				case ExcelChartType.LineStacked:
				case ExcelChartType.LineStacked100:
					result = new LineChartSerie(chart, isPivot);
					if (chart.ChartType == ExcelChartType.LineMarkers ||
						chart.ChartType == ExcelChartType.LineMarkersStacked ||
						chart.ChartType == ExcelChartType.LineMarkersStacked100)
					{
						((LineChartSerie)result).Marker = ExcelMarkerStyle.Square;
					}
					((LineChartSerie)result).Smooth = ((ExcelLineChart)chart).Smooth;
					break;

				default:
					result = new ChartSerie(chart, isPivot);
					break;
			}
			result.Series = seriesAddress;
			result.XSeries = xSeriesAddress;

			return result;
		}
	}
}
