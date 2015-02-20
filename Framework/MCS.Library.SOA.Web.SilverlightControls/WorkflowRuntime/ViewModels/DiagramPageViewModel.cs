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
using Northwoods.GoXam.Model;
using WorkflowRuntime.Models;
using Northwoods.GoXam;
using WorkflowRuntime.Commands;
using System.Collections.ObjectModel;
using System.Xml.Linq;
using WorkflowRuntime.Utils;
using System.Text;
using System.Linq;
using System.Collections.Generic;

namespace WorkflowRuntime.ViewModels
{
	public class DiagramPageViewModel : ViewModelBase
	{
		public DiagramPageViewModel(WorkflowInfo info)
		{
			WebInteraction.InitCultureInfo();
			InitializeProperty(info);
			InitCommand();
			InitializeDiagramModel();
		}

		#region Page Letters
		#region legend
		public string P_Legend_Header
		{
			get
			{
				return WebInteraction.CultureInfo.ContainsKey("图例") ? WebInteraction.CultureInfo["图例"] : "图例";
			}
		}

		public string P_Legend_DesignedActivity
		{
			get
			{
				return WebInteraction.CultureInfo.ContainsKey("设计的活动节点") ? WebInteraction.CultureInfo["设计的活动节点"] : "设计的活动节点";
			}
		}

		public string P_Legend_AddedActivity
		{
			get
			{
				return WebInteraction.CultureInfo.ContainsKey("新增的活动节点") ? WebInteraction.CultureInfo["新增的活动节点"] : "新增的活动节点";
			}
		}

		public string P_Legend_DeletedActivity
		{
			get
			{
				return WebInteraction.CultureInfo.ContainsKey("删除的活动节点") ? WebInteraction.CultureInfo["删除的活动节点"] : "删除的活动节点";
			}
		}

		public string P_Legend_DynamicActivity
		{
			get
			{
				return WebInteraction.CultureInfo.ContainsKey("模版活动节点") ? WebInteraction.CultureInfo["模版活动节点"] : "模版活动节点";
			}
		}

		public string P_Legend_CloneCompletedActivity
		{
			get
			{
				return WebInteraction.CultureInfo.ContainsKey("克隆已运行活动节点") ? WebInteraction.CultureInfo["克隆已运行活动节点"] : "克隆已运行活动节点";
			}
		}

		public string P_Legend_CloneDesignedActivity
		{
			get
			{
				return WebInteraction.CultureInfo.ContainsKey("克隆设计活动节点") ? WebInteraction.CultureInfo["克隆设计活动节点"] : "克隆设计活动节点";
			}
		}

		public string P_Legend_CloneAddedActivity
		{
			get
			{
				return WebInteraction.CultureInfo.ContainsKey("克隆新增加活动节点") ? WebInteraction.CultureInfo["克隆新增加活动节点"] : "克隆新增加活动节点";
			}
		}

		public string P_Legend_PendingActivity
		{
			get
			{
				return WebInteraction.CultureInfo.ContainsKey("等待中的活动点") ? WebInteraction.CultureInfo["等待中的活动点"] : "等待中的活动点";
			}
		}

		public string P_Legend_CurrentActivity
		{
			get
			{
				return WebInteraction.CultureInfo.ContainsKey("当前活动节点") ? WebInteraction.CultureInfo["当前活动节点"] : "当前活动节点";
			}
		}

		public string P_Legend_CompletedActivity
		{
			get
			{
				return WebInteraction.CultureInfo.ContainsKey("已完成的活动节点") ? WebInteraction.CultureInfo["已完成的活动节点"] : "已完成的活动节点";
			}
		}

		public string P_Legend_CompletedPath
		{
			get
			{
				return WebInteraction.CultureInfo.ContainsKey("已完成活动路径") ? WebInteraction.CultureInfo["已完成活动路径"] : "已完成活动路径";
			}
		}

		public string P_Legend_NotRuningPath
		{
			get
			{
				return WebInteraction.CultureInfo.ContainsKey("未运行活动路径") ? WebInteraction.CultureInfo["未运行活动路径"] : "未运行活动路径";
			}
		}

		public string P_Legend_EnabledPath
		{
			get
			{
				return WebInteraction.CultureInfo.ContainsKey("已禁用活动路径") ? WebInteraction.CultureInfo["已禁用活动路径"] : "已禁用活动路径";
			}
		}

		public string P_Legend_IncludingBranchProcessActivity
		{
			get
			{
				return WebInteraction.CultureInfo.ContainsKey("包含分支流程的活动节点") ? WebInteraction.CultureInfo["包含分支流程的活动节点"] : "包含分支流程的活动节点";
			}
		}
		#endregion
		#region process
		public string P_WfInfo_Header
		{
			get
			{
				return WebInteraction.CultureInfo.ContainsKey("流程信息") ? WebInteraction.CultureInfo["流程信息"] : "流程信息";
			}
		}


        public string P_WfInfo_ProcessID
        {
            get
            {
                return WebInteraction.CultureInfo.ContainsKey("流程ID") ? WebInteraction.CultureInfo["流程ID"] + "：" : "流程ID" + "：";
            }
        }


		public string P_WfInfo_ResourceID
		{
			get
			{
                return WebInteraction.CultureInfo.ContainsKey("资源ID") ? WebInteraction.CultureInfo["资源ID"] + "：" : "资源ID" + "：";
			}
		}

		public string P_WfInfo_Status
		{
			get
			{
				return WebInteraction.CultureInfo.ContainsKey("状态") ? WebInteraction.CultureInfo["状态"] + "：" : "状态" + "：";
			}
		}

		public string P_WfInfo_Creator
		{
			get
			{
				return WebInteraction.CultureInfo.ContainsKey("创建人") ? WebInteraction.CultureInfo["创建人"] + "：" : "创建人" + "：";
			}
		}

		public string P_WfInfo_Organization
		{
			get
			{
				return WebInteraction.CultureInfo.ContainsKey("所属组织") ? WebInteraction.CultureInfo["所属组织"] + "：" : "所属组织" + "：";
			}
		}

		public string P_BeginTime
		{
			get
			{
				return WebInteraction.CultureInfo.ContainsKey("开始时间") ? WebInteraction.CultureInfo["开始时间"] + "：" : "开始时间" + "：";
			}
		}

		public string P_EndTime
		{
			get
			{
				return WebInteraction.CultureInfo.ContainsKey("结束时间") ? WebInteraction.CultureInfo["结束时间"] + "：" : "结束时间" + "：";
			}
		}

		#endregion
		#endregion

		#region Fields
		/// <summary>
		/// 流程KEY
		/// </summary>
		public string Key { get; set; }
		private string _name;
		/// <summary>
		/// 流程名称
		/// </summary>
		public string Name
		{
			get { return _name; }
			set
			{
				_name = value;
				OnPropertyChanged("Name");
			}
		}

		public WorkflowInfo WfInfo { get; set; }

		private GraphLinksModel<ActivityNode, string, string, ActivityLink> _diagramModel;
		public GraphLinksModel<ActivityNode, string, string, ActivityLink> DiagramModel
		{
			get
			{
				if (_diagramModel == null)
				{
					_diagramModel = new GraphLinksModel<ActivityNode, string, string, ActivityLink>()
					{
						Modifiable = true,
						HasUndoManager = true
					};
				}

				return _diagramModel;
			}
		}

		#endregion

		#region commands
		public RelayCommand<object> CloseItemCommand
		{
			get;
			private set;
		}
		#endregion

		public event EventHandler RequestClose;

		#region Methodes
		private void CloseItemCommandExecuted(object para)
		{
			if (RequestClose != null)
			{
				RequestClose(this, EventArgs.Empty);
			}
		}

		private void InitCommand()
		{
			CloseItemCommand = new RelayCommand<object>(CloseItemCommandExecuted, p => { return true; });
		}

		private void InitializeProperty(WorkflowInfo info)
		{
			this.WfInfo = info;
			this.Key = info.Key;
			this.Name = info.Name;
		}

		private void InitializeDiagramModel()
		{
			//LoadOriginalGraph();
			ProcessNodes();
			ProcessLinks();

			#region test
#if DEBUG
			//this.DiagramModel.AddLink(new ActivityLink()
			//{
			//    Key = "key",
			//    Text = "p.Name",
			//    From = "N2",
			//    To = "N0",
			//    WfRuntimeIsPassed = false,
			//    WfReturnLine = true,
			//    FromPort = "portBottom",
			//    ToPort = "portTop"
			//});
#endif
			#endregion
		}

		private void ProcessLinks()
		{
			//删除旧线
			var links = DiagramModel.LinksSource as ObservableCollection<ActivityLink>;
			var removedLinks = new List<ActivityLink>();
			foreach (var link in links)
			{
				//if (WfInfo.Transitions.Count(p => p.Key == link.Key) == 0)
				var linkInfo = WfInfo.Transitions.FirstOrDefault(p => p.FromActivityKey == link.From && p.ToActivityKey == link.To);
				if (linkInfo == null)
				{
					removedLinks.Add(link);
					continue;
				}
				else
				{
					link.WfRuntimeIsPassed = linkInfo.IsPassed;
					link.WfReturnLine = linkInfo.WfReturnLine;
				}
			}

			removedLinks.ForEach(p => DiagramModel.RemoveLink(p));

			//增加新线
			foreach (var tran in WfInfo.Transitions)
			{
				if (links.Count(p => p.From == tran.FromActivityKey && p.To == tran.ToActivityKey) == 0)
				{
					ActivityLink item = new ActivityLink()
					{
						Key = tran.Key,
						From = tran.FromActivityKey,
						To = tran.ToActivityKey,
						WfEnabled = tran.Enabled,
						WfRuntimeIsPassed = tran.IsPassed,
						WfReturnLine = tran.WfReturnLine
					};

					if (item.WfReturnLine && item.WfRuntimeIsPassed)
					{
						item.Category = "PassedAndReturnTemplate";
					}
					else
					{
						if (item.WfEnabled == false)
							item.Category = "EnabledTemplate";
						else if (item.WfRuntimeIsPassed)
							item.Category = "PassedTemplate";
						else if (item.WfReturnLine)
							item.Category = "ReturnTemplate";
						else
							item.Category = "LinkTemplate";
					}

					DiagramModel.AddLink(item);
				}
			}

			foreach (var item in links)
			{
				item.FromPort = "portBottom";
				item.ToPort = "portTop";
			}
		}

		private void ProcessNodes()
		{
			var nodes = DiagramModel.NodesSource as ObservableCollection<ActivityNode>;
			var newActQuery = from act in this.WfInfo.Activities
							  where nodes.Count(p => p.Key == act.Key) == 0
							  select act;

			foreach (var newAct in newActQuery)
			{
				ActivityNode newItem = new ActivityNode()
				{
					Key = newAct.Key,
					WfName = newAct.Name,
					WfDescription = newAct.Description,
					WfEnabled = newAct.Enabled,
					WfRuntimeIsNewAdd = true,
					CloneKey = newAct.CloneKey
				};

				if (string.IsNullOrEmpty(newAct.CloneKey) == false)
					newItem.Category = "CloneTemplate";
				else if (newAct.IsDynamic == true)
					newItem.Category = "DynamicTemplate";
				else
					newItem.Category = "NodeTemplate";

				this.DiagramModel.AddNode(newItem);
			}

			foreach (var node in nodes)
			{
				if (node.Key == this.WfInfo.CurrentActivityKey)
					node.WfRuntimeIsCurrent = true;

				if (this.WfInfo.Activities.Count(p => p.Key == node.Key) == 0)
				{
					node.WfRuntimeIsRemove = true;
				}
				else
				{
					var actInfo = WfInfo.Activities.Single(p => p.Key == node.Key);
					node.WfRuntimeHasBranchProcess = actInfo.HasBranchProcess;
					node.InstanceID = actInfo.ID;
					node.WfRuntimeOperator = actInfo.Operator;
					node.NodeDetail = GetNodeDetail(actInfo);
					if (actInfo != null)
					{
						if (actInfo.Status == "已完成" || actInfo.Status.ToLower() == "completed")
						{
							node.WfRuntimeIsComplete = true;
						}

						if (actInfo.Status == "等待中" || actInfo.Status.ToLower() == "pending")
						{
							node.WfRuntimeIsPending = true;
						}
					}
				}
			}

			this.OnPropertyChanged("DiagramModel");
		}

		private void LoadOriginalGraph()
		{
			if (!string.IsNullOrEmpty(this.WfInfo.GraphDescription))
			{
				DiagramModel.Load<ActivityNode, ActivityLink>(
					XElement.Parse(this.WfInfo.GraphDescription),
					WorkflowUtils.DIAGRAM_XELEMENT_NODENAME,
					WorkflowUtils.DIAGRAM_XELEMENT_LINKNAME);
			}
		}

		private string GetNodeDetail(ActivityInfo activity)
		{
			StringBuilder result = new StringBuilder();

			//if (activity.ActivityType != ActivityType.Completed)
			{
				string letterAct = WebInteraction.CultureInfo.ContainsKey("活动") ? WebInteraction.CultureInfo["活动"] : "活动";
				string letterTmp = WebInteraction.CultureInfo.ContainsKey("模板") ? WebInteraction.CultureInfo["模板"] : "模板";
				string letterStatus = WebInteraction.CultureInfo.ContainsKey("状态") ? WebInteraction.CultureInfo["状态"] : "状态";
				string letterStatusVal = WebInteraction.CultureInfo.ContainsKey(activity.Status) ?
					WebInteraction.CultureInfo[activity.Status] : activity.Status;
				string letterOp = WebInteraction.CultureInfo.ContainsKey("操作人") ? WebInteraction.CultureInfo["操作人"] : "操作人";
				string letterBeginTime = WebInteraction.CultureInfo.ContainsKey("开始时间") ? WebInteraction.CultureInfo["开始时间"] : "开始时间";
				string letterEndTime = WebInteraction.CultureInfo.ContainsKey("结束时间") ? WebInteraction.CultureInfo["结束时间"] : "结束时间";

				result.Append(letterAct + " ID:" + activity.ID + "\n");
				result.Append(letterTmp + " ID:" + activity.Key + "\n");
				result.Append(letterStatus + ":" + letterStatusVal + "\n");
				result.Append(letterOp + ":" + activity.Operator + "\n");

				string startTimeStr = "";
				string endTimeStr = "";
				if (activity.StartTime > DateTime.MinValue.ToLocalTime())
				{
					startTimeStr = activity.StartTime.ToString("yyyy-MM-dd HH:mm:ss");
				}

				if (activity.EndTime > DateTime.MinValue.ToLocalTime())
				{
					endTimeStr = activity.EndTime.ToString("yyyy-MM-dd HH:mm:ss");
				}
				result.Append(letterBeginTime + ":" + startTimeStr + "\n");
				result.Append(letterEndTime + ":" + endTimeStr);
			}

			return result.ToString();
		}
		#endregion
	}
}
