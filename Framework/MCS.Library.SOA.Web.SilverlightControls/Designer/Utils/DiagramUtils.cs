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
using System.Collections.Generic;
using System.Collections.ObjectModel;
using Designer.Views;
using Designer.ViewModels;

namespace Designer.Utils
{
	public static class DiagramUtils
	{
		public static IWebInterAction WebInterAct = new WebInterAction();

		/// <summary>
		/// 确定是否是设计窗口
		/// </summary>
		/// <param name="element"></param>
		/// <returns></returns>
		public static bool IsMainDiagram(UIElement element)
		{
			bool result = false;

			Overview overview = Part.FindAncestor<Overview>(element);

			if (overview == null)
				result = true;

			return result;
		}

		/// <summary>
		/// 设置工作流属性上下文菜单可见性
		/// </summary>
		/// <param name="diagram"></param>
		/// <param name="visibility"></param>
		public static void SetWfContextMenuVisibility(Diagram diagram, Visibility visibility)
		{
			var main = GetMainView(diagram);
			var menu = main.FindName("contextMenu") as ContextMenu;
			if (menu == null)
				return;

			menu.Visibility = visibility;
		}

		public static void SetDiagramContextMenuVisibility(UIElement element, bool visible)
		{
			var diagram = Part.FindAncestor<Diagram>(element);

			if (diagram == null) 
				return;
		}

		public static void AddActivityTemplate(Diagram diagram, ActivityNode newNode)
		{
			var main = GetMainView(diagram);
			var palette = main.FindName("mainPalette") as Palette;
			if (palette == null) return;

			palette.StartTransaction("AddNewNode");
			palette.Model.AddNode(newNode);
			//var nodes = palette.NodesSource as ObservableCollection<ActivityNode>;
			//nodes.Add(newNode);
			palette.CommitTransaction("AddNewNode");
		}


		private static UserControl GetMainView(Diagram diagram)
		{
			var element = VisualTreeHelper.GetParent(diagram);
			DependencyObject topElement = null;

			while (true)
			{
				if (element == null) break;
				topElement = element;
				element = VisualTreeHelper.GetParent(element);
			}

			if (topElement == null) return null;

			var main = topElement as UserControl;

			return main;
		}

		/// <summary>
		/// 获取所有活动点模板key集合
		/// </summary>
		/// <param name="diagram"></param>
		/// <returns></returns>
		public static List<string> GetTemplateKeys(Diagram diagram)
		{
			var main = GetMainView(diagram);
			var element = (Grid)main.FindName("LayoutRoot");
			var model = element.DataContext as MainPageViewModel;
			return model.TemplateKeys;
		}

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
