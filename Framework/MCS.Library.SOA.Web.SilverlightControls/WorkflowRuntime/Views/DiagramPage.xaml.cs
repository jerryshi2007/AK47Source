using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;
using Northwoods.GoXam;
using System.Windows.Browser;
using WorkflowRuntime.Models;
using WorkflowRuntime.Utils;
using System.Xml.Linq;
using Northwoods.GoXam.Model;

namespace WorkflowRuntime.Views
{
	public partial class DiagramPage : UserControl
	{
		public DiagramPage()
		{
			InitializeComponent();
		}

		/// <summary>
		/// 子流程菜单
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void btnMenu_Click(object sender, RoutedEventArgs e)
		{
			Node node = Part.FindAncestor<Node>(e.OriginalSource as UIElement);
			if (node == null) return;

			ActivityNode data = node.Data as ActivityNode;
			if (data == null) return;

			var element = sender as DependencyObject;
			DependencyObject topElement = null;

			while (true)
			{
				if (element == null) break;
				topElement = element;
				element = VisualTreeHelper.GetParent(element);
			}

			if (topElement == null) return;

			var main = topElement as MainPage;
			if (main == null) return;
			WebInteraction.OpenBranchProc(main.WrapperID, data.InstanceID);
		}

		//private void Button_Click(object sender, RoutedEventArgs e)
		//{
		//    var diagramModel = this.mainDiagram.Model as GraphLinksModel<ActivityNode, string, string, ActivityLink>;

		//    var xelement = diagramModel.Save<ActivityNode, ActivityLink>(
		//                WorkflowUtils.DIAGRAM_XELEMENT_ROOTNAME,
		//                WorkflowUtils.DIAGRAM_XELEMENT_NODENAME,
		//                WorkflowUtils.DIAGRAM_XELEMENT_LINKNAME);
		//    xelement.Add(new XAttribute("id", this.mainDiagram.Tag.ToString()));

		//    string str = xelement.ToString();
		//}

		private void mainDiagram_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
		{
			foreach (ActivityLink link in ((ILinksModel)mainDiagram.Model).LinksSource)
			{
				link.WfEnabled = false;
			}

			if (mainDiagram.SelectedPart == null)
				return;

			Node node = mainDiagram.SelectedPart as Node;
			if (node == null) 
				return;

			foreach (Link link in node.LinksConnected)
			{
				((ActivityLink)link.Data).WfEnabled = true;
			}
		}
	}
}
