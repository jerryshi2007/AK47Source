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
using Designer.Models;
using Designer.Utils;

namespace Designer
{
	public class TransitionBrushConverters : Converter
	{
		private static Brush DefaultBrush = new SolidColorBrush(Colors.Black);
		private static Brush Gray = new SolidColorBrush(Colors.LightGray);

		public override object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
		{
			if (value == null || !(bool)value) return Gray;
			return DefaultBrush;
		}
	}

	public class ActivityBrushConverter : Converter
	{
		private Brush DefaultBrush = new SolidColorBrush(DiagramUtils.StringToColor("#FFEEDC82"));
		private Brush GrayBrush = new SolidColorBrush(Colors.LightGray);
		private Brush Green = new SolidColorBrush(Colors.Green);
		private Brush Blue = new SolidColorBrush(Colors.Blue);

		public override object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
		{
			var result = GrayBrush;
			if ((bool)value) return DefaultBrush;
			return GrayBrush;
			//var node = value as ActivityNode;
			//if (node == null) return result;

			//if (node.WfEnabled == false) return result;

			//var actType = (ActivityType)Enum.Parse(typeof(ActivityType), node.Category, true);
			//switch (actType)
			//{
			//    case ActivityType.Initial: result = Green; break;
			//    case ActivityType.Normal: result = Blue; break;
			//    case ActivityType.Completed: result = DefaultBrush; break;
			//}
			//return result;
		}
	}

	public class ActivityTemplateConverter : Converter
	{
		public override object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
		{
			if (value == null) return false;
			if (string.IsNullOrEmpty(value.ToString())) return false;
			return true;
		}
	}
}
