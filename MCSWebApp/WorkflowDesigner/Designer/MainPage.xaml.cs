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
using Designer.ViewModels;
using System.Windows.Browser;
using Designer.Models;
using System.Xml.Linq;
using Newtonsoft.Json;
using Designer.Utils;
using Designer.Views;
using Northwoods.GoXam;
using System.Text;
using Northwoods.GoXam.Model;

namespace Designer
{
	public partial class MainPage : UserControl, IDesignerInterAction
	{
		private MainPageViewModel _mainPageVW;

		public MainPage()
		{
			InitializeComponent();
			this.MouseRightButtonDown += (o, e) => e.Handled = true;
			_mainPageVW = new MainPageViewModel(new WebInterAction());
			this.LayoutRoot.DataContext = _mainPageVW;

			HtmlPage.RegisterScriptableObject("SLM", this);
			_mainPageVW.WebInterAct.LoadProcessInstanceDescription();
		}

		private void SetLinkRoutePoints(DiagramPageViewModel vw)
		{
			if (vw == null) return;

			TabItem tabItem = (TabItem)this.tabControl.Items.Single(p => ((TabItem)p).DataContext == vw);

			if (tabItem == null) return;

			var links = ((DiagramPage)tabItem.Content).mainDiagram.Links;

			foreach (Link link in links)
			{
				ActivityLink linkData = link.Data as ActivityLink;
				if (linkData != null)
				{
					linkData.Points = new List<Point>(link.Route.Points);
				}
			}
		}

		#region 客户端调用
		[ScriptableMember]
		public void CreateNewWorkflow(string key)
		{
			if (string.IsNullOrEmpty(key)) throw new ArgumentException("请指定新建流程的KEY值");
			_mainPageVW.AddNewCommand.Execute(WorkflowInfo.CreateEmptyWorkflow(key));
		}

		[ScriptableMember]
		public void OpenWorkflow(string wfJsonStr)
		{
			var wfList = JsonConvert.DeserializeObject<List<WorkflowInfo>>(wfJsonStr);
			foreach (var info in wfList)
			{
				_mainPageVW.AddNewCommand.Execute(info);
			}
		}

		[ScriptableMember]
		public void LayoutCurrentDiagram()
		{
			if (_mainPageVW.SelectedItem == null) return;

			var form = _mainPageVW.SelectedItem.Content as UserControl;
			if (form == null) return;

			var diagram = form.FindName("mainDiagram") as Northwoods.GoXam.Diagram;
			if (diagram == null) return;

			diagram.LayoutDiagram();
		}

		[ScriptableMember]
		public void UpdateDiagramData(string upType, string tagKey, string name, string value)
		{
			var vw = _mainPageVW.CurrentDiagram;
			switch (upType)
			{
				case WorkflowUtils.CLIENTSCRIPT_PARAM_WORKFLOW:
					if (name == "Name") vw.Name = value;
					if (name == "Key") vw.Key = value;
					break;
				case WorkflowUtils.CLIENTSCRIPT_PARAM_ACTIVITY:
					var node = vw.DiagramModel.FindNodeByKey(tagKey);
					if (node == null) return;

					if (name == "Name") node.WfName = value;
					else if (name == "Description") node.WfDescription = value;
					else if (name == "Enabled") node.WfEnabled = bool.Parse(value);
					else if (name == "Branch") node.WfHasBranchProcess = bool.Parse(value);
					else if (name == "IsDynamic") node.IsDynamic = bool.Parse(value);
					break;
				case WorkflowUtils.CLIENTSCRIPT_PARAM_TRANSITION:
					var link = vw.DiagramModel.LinksSource.Cast<ActivityLink>().Single(p => p.Key == tagKey);
					if (link == null) return;

					if (name == "Name" || name == "Condition") link.Text = value;
					else if (name == "Enabled") link.WfEnabled = bool.Parse(value);
					else if (name == "IsReturn") link.WfReturnLine = bool.Parse(value);

					break;
			}
		}

		[ScriptableMember]
		public string GetWorkflowGraph(string wfKey)
		{
			if (string.IsNullOrEmpty(wfKey))
			{
				List<DiagramInfo> list = new List<DiagramInfo>();
				foreach (var diagramVW in _mainPageVW.DiagramDataSource)
				{
					SetLinkRoutePoints(diagramVW);
					var xelement = diagramVW.DiagramModel.Save<ActivityNode, ActivityLink>(
						WorkflowUtils.DIAGRAM_XELEMENT_ROOTNAME,
						WorkflowUtils.DIAGRAM_XELEMENT_NODENAME,
						WorkflowUtils.DIAGRAM_XELEMENT_LINKNAME);
					xelement.Add(new XAttribute("id", diagramVW.Key));

					list.Add(new DiagramInfo() { Key = diagramVW.Key, Value = xelement.ToString() });
				}

				return JsonConvert.SerializeObject(list);
			}

			var vw = _mainPageVW.DiagramDataSource.Single(p => p.Key == wfKey);
			SetLinkRoutePoints(vw);
			var xml = vw.DiagramModel.Save<ActivityNode, ActivityLink>(
					WorkflowUtils.DIAGRAM_XELEMENT_ROOTNAME,
					WorkflowUtils.DIAGRAM_XELEMENT_NODENAME,
					WorkflowUtils.DIAGRAM_XELEMENT_LINKNAME);
			xml.Add(new XAttribute("id", vw.Key));

			return xml.ToString();
		}

		[ScriptableMember]
		public string RemoveActivityTemplate()
		{
			List<ActivityNode> nodes = new List<ActivityNode>();

			var parts = this.mainPalette.SelectedParts;
			foreach (var part in parts)
			{
				var node = part as Node;
				if (node == null) continue;

				var nodeData = node.Data as ActivityNode;
				if (nodeData.Key == "Normal" || nodeData == null) continue;

				nodes.Add(nodeData);
			}

			if (nodes.Count == 0) return "";

			var confirmDelete = HtmlPage.Window.Confirm(string.Format("您确定要删除这{0}个模板吗？", nodes.Count));

			if (!confirmDelete) return "canceled";

			this.mainPalette.StartTransaction("RemoveNode");
			foreach (var node in nodes)
			{
				this.mainPalette.Model.RemoveNode(node);
				this._mainPageVW.TemplateKeys.Remove(node.Key);
			}

			StringBuilder sb = new StringBuilder();
			nodes.ForEach(p => sb.Append(p.Key + ","));

			sb.Remove(sb.Length - 1, 1);

			return sb.ToString();
		}

		[ScriptableMember]
		public void AddActivitySelfLink()
		{
			if (_mainPageVW.SelectedItem == null) return;

			var form = _mainPageVW.SelectedItem.Content as UserControl;
			if (form == null) return;

			var diagram = form.FindName("mainDiagram") as Northwoods.GoXam.Diagram;
			if (diagram == null) return;

			Node node = diagram.SelectedNode;

			if (node == null)
			{
				MessageBox.Show("请选择需要添加自连线节点！");
				return;
			}

			ActivityNode nodeData = node.Data as ActivityNode;
			if (nodeData.Category == ActivityType.Completed.ToString())
			{
				MessageBox.Show("结束点不能添加自连线！");
				return;
			}

			var selfLinkNum = node.LinksConnected.Count(p => p.FromNode == p.ToNode);

			if (selfLinkNum >= 1)
			{
				MessageBox.Show("一个节点只能有一条自连线！");
				return;
			}

			diagram.StartTransaction("addselflink");
			var model = diagram.Model as GraphLinksModel<ActivityNode, string, string, ActivityLink>;
			model.AddLink(nodeData, "portBottom", nodeData, "portTop");
			diagram.CommitTransaction("addselflink");
		}

		[ScriptableMember]
		public void UpdateActivityTemplate(string templateID,string activityKey)
		{
			if (this._mainPageVW.SelectedItem == null)
				return;

			var form = _mainPageVW.SelectedItem.Content as UserControl;
			if (form == null) return;

			var diagram = form.FindName("mainDiagram") as Northwoods.GoXam.Diagram;
			if (diagram == null)
				return;

			ActivityNode newNode = null;
			if (diagram.SelectedPart != null)
			{
				if (diagram.SelectedPart.Data != null)
				{
					newNode = diagram.SelectedPart.Data as ActivityNode; 
				}
			}
			if (newNode == null)
			{ 
				DiagramPageViewModel datacontex= diagram.DataContext  as DiagramPageViewModel;

			    foreach(ActivityNode item in  datacontex.DiagramModel.NodesSource)
				{
				     if(string.Compare(item.Key,activityKey)==0)
					 {
					    newNode =item;
						break;
					 }
				}
			}
	
			newNode = newNode.WfClone(templateID);

			this.mainPalette.StartTransaction("AddNewNode");
			this.mainPalette.Model.AddNode(newNode);
			this.mainPalette.CommitTransaction("AddNewNode");

			this._mainPageVW.TemplateKeys.Add(templateID);
		}
		#endregion
	}
}
