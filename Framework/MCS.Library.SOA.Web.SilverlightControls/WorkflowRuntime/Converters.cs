using System;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Ink;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;
using Northwoods.GoXam;
using WorkflowRuntime.Utils;
using WorkflowRuntime.Models;

namespace WorkflowRuntime
{
	public class NodeCategoryBrushConverter : Converter
	{
		private static Brush CommonNodeBrush = new SolidColorBrush(DiagramUtils.StringToColor("#FFEEDC82"));
		private static Brush CurrentNodeBrush = new SolidColorBrush(DiagramUtils.StringToColor("#FFBBEE22"));
		private static Brush PendingNodeBrush = new SolidColorBrush(DiagramUtils.StringToColor("#F7FDFD04"));
		private static Brush NewAddNodeBrush = new SolidColorBrush(DiagramUtils.StringToColor("#FF11BBEE"));
		private static Brush DeleteNodeBrush = new SolidColorBrush(Colors.LightGray);

		public override object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
		{
			var node = value as ActivityNode;
			if (node == null) return CommonNodeBrush;

			if (node.WfRuntimeIsPending) return PendingNodeBrush;
			if (node.WfRuntimeIsCurrent) return CurrentNodeBrush;
			if (node.WfRuntimeIsNewAdd) return NewAddNodeBrush;
			if (node.WfRuntimeIsRemove) return DeleteNodeBrush;

			return CommonNodeBrush;
		}
	}

	public class VisibilityConverter : Converter
	{
		public override object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
		{
			bool visibility = (bool)value;

			return visibility ? Visibility.Visible : Visibility.Collapsed;
		}
	}

	public class DatetimeConverter : Converter
	{
		public override object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
		{
			if (value is DateTime)
			{
				var temp = (DateTime)value;
				if (temp > DateTime.MinValue.ToLocalTime())
				{
					return temp.ToString("yyyy-MM-dd HH:mm:ss");
				}
				return "";
			}
			return value;
		}
	}
}
