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
using WorkflowRuntime.Models;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using WorkflowRuntime.Views;

namespace WorkflowRuntime.Utils
{
	public static class DiagramUtils
	{
		/// <summary>
		/// 颜色字符转为对象
		/// </summary>
		/// <param name="color">e.g. #FFEEDC82</param>
		/// <returns></returns>
		public static Color StringToColor(string color)
		{
			if (color.StartsWith("#")) color = color.Replace("#", string.Empty);
			int v = int.Parse(color, System.Globalization.NumberStyles.HexNumber);
			return new Color()
			{
				A = Convert.ToByte((v >> 24) & 255),
				R = Convert.ToByte((v >> 16) & 255),
				G = Convert.ToByte((v >> 8) & 255),
				B = Convert.ToByte((v >> 0) & 255)
			};
		}
	}
}
