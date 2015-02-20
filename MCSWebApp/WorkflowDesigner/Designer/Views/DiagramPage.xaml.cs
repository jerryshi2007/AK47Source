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
using Designer.Models;
using Designer.Utils;
using Shared.Silverlight.Controls;

namespace Designer.Views
{
	public partial class DiagramPage : UserControl
	{
		public static readonly string WFCONTEXT_MENU = "wfMenuNode";
		public static readonly string ACTCONTEXT_MENU = "menuNode";

		public DiagramPage()
		{
			InitializeComponent();
			this.mainDiagram.DefaultTool = new WfToolManager();
			this.myOverview.Observed = this.mainDiagram;
			this.WebMethod = new WebInterAction();
			this.mainDiagram.LayoutCompleted += UpdateRoutes;

            
            App myapp = App.Current as App;
            if (myapp.paras != null)
            {
                string enableSimulation = myapp.paras["enableSimulation"];
                bool flag = true;
                bool.TryParse(enableSimulation, out flag);
                if (this.processContextMenu != null)
                {
                    ContextMenuItem subMenuItem = this.processContextMenu.Items[5] as ContextMenuItem;
                    if (flag)
                        subMenuItem.Visibility = Visibility.Visible;
                    else
                        subMenuItem.Visibility = Visibility.Collapsed;
                }
            }
            
          
		}

		public IWebInterAction WebMethod { get; private set; }

		#region Goxam events
		/// <summary>
		/// 显示活动点上链接接入点
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void Node_MouseEnter(object sender, MouseEventArgs e)
		{
			if (!DiagramUtils.IsMainDiagram(sender as UIElement)) return;

			SetPortsVisible(sender as UIElement, true);
		}

		/// <summary>
		/// 隐藏活动点上链接接入点
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void Node_MouseLeave(object sender, MouseEventArgs e)
		{
			if (!DiagramUtils.IsMainDiagram(sender as UIElement)) return;

			SetPortsVisible(sender as UIElement, false);
		}

		/// <summary>
		/// 设置活动点上链接接入点是否可见
		/// </summary>
		/// <param name="uielement"></param>
		/// <param name="visible"></param>
		private void SetPortsVisible(UIElement uielement, bool visible)
		{
			SpotPanel sp = uielement as SpotPanel;
			if (sp == null) return;

			Node n = Part.FindAncestor<Node>(sp);
			if (n == null) return;

			n.Tag = visible;
		}

		private void mainDiagram_TemplateApplied(object sender, DiagramEventArgs e)
		{
			var bind = new System.Windows.Data.Binding("Scale");
			bind.Source = mainDiagram.Panel;
			bind.Mode = System.Windows.Data.BindingMode.TwoWay;
			zoomSlider.SetBinding(Slider.ValueProperty, bind);
		}

		/// <summary>
		/// 设置选中的Part，加载其属性
		/// </summary>
		/// <param name="element"></param>
		public void SetSelectPartAsCurrent(UIElement element)
		{
			var node = Part.FindAncestor<Node>(element);
			if (node != null)
			{
				var data = node.Data as ActivityNode;
				//将当前元素设为选择元素
				node.Diagram.SelectedPart = node;
				//属性设计器加载当前元素属性
				WebMethod.LoadProperty(WorkflowUtils.CLIENTSCRIPT_PARAM_ACTIVITY,
					node.Diagram.Tag.ToString(),
					WorkflowUtils.ExtractActivityInfoJson(data));
				return;
			}

			var link = Part.FindAncestor<Link>(element);
			if (link != null)
			{
				var data = link.Data as ActivityLink;
				link.Diagram.SelectedPart = link;
				WebMethod.LoadProperty(WorkflowUtils.CLIENTSCRIPT_PARAM_TRANSITION,
					link.Diagram.Tag.ToString(),
					WorkflowUtils.ExtractTransitionInfoJson(data));
			}
		}

		public void ContextMenuTemplate_Click(object sender, RoutedEventArgs e)
		{
			string templateID = Guid.NewGuid().ToString();
			HtmlPage.Window.Invoke("SaveActivityTemplate", templateID);

			ActivityNode nodeData = ((Northwoods.GoXam.PartManager.PartBinding)((sender as FrameworkElement).DataContext)).Data as ActivityNode;
			//保存到数据库
			DiagramUtils.WebInterAct.SaveActivityTemplate(templateID);
			//保存到palette中
			DiagramUtils.AddActivityTemplate(this.mainDiagram, nodeData.WfClone(templateID));
			DiagramUtils.GetTemplateKeys(this.mainDiagram).Add(templateID);
		}

		private void btnCondition_Click(object sender, RoutedEventArgs e)
		{
			var element = sender as UIElement;
			if (!DiagramUtils.IsMainDiagram(element)) return;

			SetSelectPartAsCurrent(element);
			DiagramUtils.WebInterAct.OpenEditor(sender, EditorType.Condition);
		}

		private void btnVariable_Click(object sender, RoutedEventArgs e)
		{
			var element = sender as UIElement;
			if (!DiagramUtils.IsMainDiagram(element)) return;

			SetSelectPartAsCurrent(element);
			DiagramUtils.WebInterAct.OpenEditor(sender, EditorType.Variables);
		}

		private void Process_MouseButton(object sender, MouseButtonEventArgs e)
		{
			//e.Handled = true;
			if (string.Compare(this.mainDiagram.Tag.ToString(), WorkflowUtils.CurrentKey) != 0)
			{
				HtmlPage.Window.Invoke("LoadProperty", WorkflowUtils.CLIENTSCRIPT_PARAM_WORKFLOW, this.mainDiagram.Tag.ToString(), WorkflowUtils.ExtractWorkflowInfoJson(this.mainDiagram));

				WorkflowUtils.CurrentKey = this.mainDiagram.Tag.ToString();
			}
		}

		private void Link_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
		{
			e.Handled = true;
			Northwoods.GoXam.PartManager.PartBinding currentBinding = (Northwoods.GoXam.PartManager.PartBinding)((sender as FrameworkElement).DataContext);
			ActivityLink linkData = currentBinding.Data as ActivityLink;
			if (linkData == null)
				return;

			string strKey = string.Format("{0}@{1}", this.mainDiagram.Tag.ToString(), linkData.Key);
			if (string.Compare(strKey, WorkflowUtils.CurrentKey) != 0)
			{
				this.WebMethod.LoadProperty(WorkflowUtils.CLIENTSCRIPT_PARAM_TRANSITION,
					this.mainDiagram.Tag.ToString(),
					WorkflowUtils.ExtractTransitionInfoJson(linkData));

				WorkflowUtils.CurrentKey = strKey;
			}
		}

		private void mainDiagram_MouseRightButtonUp(object sender, MouseButtonEventArgs e)
		{
			if (string.Compare(this.mainDiagram.Tag.ToString(), WorkflowUtils.CurrentKey) != 0)
			{
				this.WebMethod.LoadProperty(WorkflowUtils.CLIENTSCRIPT_PARAM_WORKFLOW,
					this.mainDiagram.Tag.ToString(),
					WorkflowUtils.ExtractWorkflowInfoJson(this.mainDiagram));
			}
		}
		#endregion

		private void UpdateRoutes(object sender, DiagramEventArgs e)
		{
			this.mainDiagram.LayoutCompleted -= UpdateRoutes;
			foreach (Link link in mainDiagram.Links)
			{
				ActivityLink linkData = link.Data as ActivityLink;
				if (linkData != null && linkData.Points != null && linkData.Points.Count() > 1)
				{
					link.Route.Points = (IList<Point>)linkData.Points;
				}
			}
			mainDiagram.PartManager.UpdatesRouteDataPoints = true;
		}

		private void ContextMenu_Opened(object sender, RoutedEventArgs e)
		{
			//var position = this.mainDiagram.LastMousePointInModel;
			//ContextMenuService.GetContextMenu(sender as FrameworkElement);
			//(sender as Shared.Silverlight.Controls.ContextMenu).OpenPopup(position);

			Northwoods.GoXam.PartManager.PartBinding currentBinding = (Northwoods.GoXam.PartManager.PartBinding)((sender as FrameworkElement).DataContext);
			ActivityNode nodeData = currentBinding.Data as ActivityNode;
			if (nodeData == null)
				return;

			string strKey = string.Format("{0}@{1}", this.mainDiagram.Tag.ToString(), nodeData.Key);
			if (string.Compare(strKey, WorkflowUtils.CurrentKey) != 0)
			{
				HtmlPage.Window.Invoke("LoadProperty", WorkflowUtils.CLIENTSCRIPT_PARAM_ACTIVITY, this.mainDiagram.Tag.ToString(), WorkflowUtils.ExtractActivityInfoJson(nodeData));

				WorkflowUtils.CurrentKey = strKey;
			}

		}

		private void ActivityNode_MouseRightButtonDown(object sender, MouseButtonEventArgs e)
		{
			e.Handled = true;
		}

		private void ContextMenuItem_Checked(object sender, RoutedEventArgs e)
		{
			HtmlPage.Window.Invoke("SetPropertyValue", "IsDynamic", "true");
		}

		private void ContextMenuItem_UnChecked(object sender, RoutedEventArgs e)
		{
			HtmlPage.Window.Invoke("SetPropertyValue", "IsDynamic", "false");
		}

	}
}
