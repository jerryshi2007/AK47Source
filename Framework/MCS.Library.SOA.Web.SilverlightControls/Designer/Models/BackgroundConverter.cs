using System;
using System.Windows.Data;
using System.Windows.Media;
using Designer.Models;
using System.Windows.Resources;
using System.Windows;
using System.IO;
using System.Windows.Markup;

namespace Designer
{
	public sealed class BackgroundConverter : IValueConverter
	{
		public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
		{
			string strName = "normalActivityBrush";

			if (value is bool)
			{
				if ((bool)value == true)
					strName = "dynamicActivityBrush";
			}
			return GetBackgroundByName(strName);
		}


		public static RadialGradientBrush GetBackgroundByName(string strName)
		{
			RadialGradientBrush result = new RadialGradientBrush();
			if (Application.Current.Resources.Contains(strName))
			{
				result = Application.Current.Resources[strName] as RadialGradientBrush;
			}

			//string uri = "/Designer;component/Resources/Brushes.xaml";
			//StreamResourceInfo StreamResourceInfoObj = Application.GetResourceStream(new Uri(uri, UriKind.RelativeOrAbsolute));

			//if (StreamResourceInfoObj != null && StreamResourceInfoObj.Stream != null)
			//{
			//    using (StreamReader StreamReaderObj = new StreamReader(StreamResourceInfoObj.Stream))
			//    {
			//        string resourcemerged = StreamReaderObj.ReadToEnd();

			//        if (string.IsNullOrEmpty(resourcemerged) == false)
			//        {
			//            ResourceDictionary loadresources = XamlReader.Load(resourcemerged) as ResourceDictionary;

			//            if (loadresources.Contains(strName))
			//                result = loadresources[strName] as RadialGradientBrush;
			//        }
			//    }
			//}

			return result;

		}


		public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
		{
			throw new NotImplementedException();
		}
	}
}
