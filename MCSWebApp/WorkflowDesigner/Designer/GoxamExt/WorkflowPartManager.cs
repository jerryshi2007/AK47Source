using System;
using System.Net;
using System.Windows;
using System.Linq;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Ink;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;
using Northwoods.GoXam;
using Designer.Models;
using System.Windows.Browser;
using Newtonsoft.Json;
using System.Collections.ObjectModel;
using Designer.Utils;
using System.Collections.Generic;
using Northwoods.GoXam.Model;

namespace Designer
{
	public class WorkflowPartManager : PartManager
	{
		public WorkflowPartManager() : this(new WebInterAction()) { }

		public WorkflowPartManager(IWebInterAction client)
			: base()
		{
			this._webMethod = client;
			this.UpdatesRouteDataPoints = false;
		}

		private IWebInterAction _webMethod;

		////初始化过程中也会执行！！！
		protected override void OnNodeAdded(Node node)
		{
			base.OnNodeAdded(node);

			ActivityNode data = node.Data as ActivityNode;

			if (data == null) return;

			if (IsOverviewDiagram(node)) return;

			if (data.IsDynamic == false)
			{
				ActivityType nodeType = (ActivityType)Enum.Parse(typeof(ActivityType), data.Category, false);
				if (!IsValidKey(data.Key, nodeType, node.Diagram))
				{
					data.Key = CreateActivityKey();
				}
			}
			else
			{
				if (!IsValidKey(data.Key, ActivityType.Normal, node.Diagram))
				{
					data.Key = CreateActivityKey();
				}
				data.IsDynamic = true;
			}

			_webMethod.LoadProperty(WorkflowUtils.CLIENTSCRIPT_PARAM_ACTIVITY,
				node.Diagram.Tag.ToString(),
				WorkflowUtils.ExtractActivityInfoJson(data));
		}

		private static bool IsValidKey(string key, ActivityType nodeType, Diagram diagram)
		{
			if (string.IsNullOrEmpty(key))
			{
				return false;
			}

			if (nodeType == ActivityType.Initial || nodeType == ActivityType.Completed)
			{
				return true;
			}

			if (!DiagramUtils.GetTemplateKeys(diagram).Contains(key))
			{
				return true;
			}

			return false;
		}

		protected override void OnNodeRemoving(Node node)
		{
			base.OnNodeRemoving(node);

			var data = node.Data as ActivityNode;
			if (data == null) return;
			if (IsOverviewDiagram(node)) return;

			string procName = node.Diagram.Tag.ToString();
			_webMethod.DeleteProcess(WorkflowUtils.CLIENTSCRIPT_PARAM_ACTIVITY, procName, data.Key);
		}

		protected override void OnLinkAdded(Link link)
		{
			bool selfLink = false;
			if (link.FromNode == link.ToNode)
			{
				selfLink = true;
				link.Route.FromSpot = Spot.MiddleLeft;
				link.Route.ToSpot = Spot.MiddleTop;
			}

			base.OnLinkAdded(link);

			var data = link.Data as ActivityLink;
			if (data == null) return;
			if (IsOverviewDiagram(link)) return;

			if (string.IsNullOrEmpty(data.Key))
			{
				data.Key = CreateTransitionKey();
				data.WfEnabled = true;
			}

			var nodeData = link.ToNode.Data as ActivityNode;
			if (selfLink || nodeData.Category == ActivityType.Initial.ToString()) data.WfReturnLine = true;

			_webMethod.LoadProperty(WorkflowUtils.CLIENTSCRIPT_PARAM_TRANSITION,
				link.Diagram.Tag.ToString(),
				WorkflowUtils.ExtractTransitionInfoJson(data));
		}

		protected override void OnLinkRemoving(Link link)
		{
			base.OnLinkRemoving(link);

			var data = link.Data as ActivityLink;
			if (data == null) return;
			if (IsOverviewDiagram(link)) return;

			string procName = link.Diagram.Tag.ToString();
			_webMethod.DeleteProcess(WorkflowUtils.CLIENTSCRIPT_PARAM_TRANSITION, procName, data.Key);
		}

		protected override void UpdateRouteDataPoints(Link link)
		{
			if (!this.UpdatesRouteDataPoints) return;
			ActivityLink data = link.Data as ActivityLink;
			if (data != null)
			{
				data.Points = new List<Point>(link.Route.Points);
			}
		}

		/// <summary>
		/// 过滤重复连线
		/// </summary>
		/// <param name="linkdata"></param>
		/// <param name="model"></param>
		/// <returns></returns>
		protected override bool FilterLinkForData(object linkdata, IDiagramModel model)
		{
			var actLink = linkdata as ActivityLink;

			if (actLink == null) return base.FilterLinkForData(linkdata, model);
			var existLinks = this.Diagram.Links.Count(p =>
			{
				var lData = p.Data as ActivityLink;
				if (lData == null) return false;

				if (lData.From == actLink.From && lData.To == actLink.To && lData.FromPort == actLink.FromPort && lData.ToPort == actLink.ToPort)
				{
					return true;
				}
				else
				{
					return false;
				}
			});

			if (existLinks != 0)
			{
				return false;
			}

			return base.FilterLinkForData(linkdata, model);
		}

		#region private method
		private bool IsOverviewDiagram(Part part)
		{
			var diagram = part.Diagram as Overview;
			if (diagram == null) return false;

			return true;
		}

		private string CreateActivityKey()
		{
			var nodes = this.Diagram.NodesSource as ObservableCollection<ActivityNode>;

			int i = 0;
			string result = string.Empty;

			while (true)
			{
				result = "N" + i;
				if (nodes.Count(p => p.Key == result) == 0) break;
				i++;
			}

			return result;
		}

		private string CreateTransitionKey()
		{
			var links = this.Diagram.LinksSource as ObservableCollection<ActivityLink>;

			int i = 0;
			string result = string.Empty;

			while (true)
			{
				result = "L" + i;
				if (links.Count(p => p.Key == result) == 0) break;
				i++;
			}

			return result;
		}
		#endregion
	}
}
