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
using Designer.Models;
using System.Collections.Generic;
using Northwoods.GoXam;
using System.Collections.ObjectModel;
using Northwoods.GoXam.Model;
using Designer.Commands;
using System.Xml.Linq;
using System.ComponentModel;
using Designer.Utils;


namespace Designer.ViewModels
{
	public class MainPageViewModel : ViewModelBase
	{
		public MainPageViewModel(IWebInterAction webAct)
		{
			WebInterAct = webAct;
			this.AddNewCommand = new RelayCommand<WorkflowInfo>(this.AddNewCommandExecuted, p => { return true; });
		}

		#region Fields
		public IWebInterAction WebInterAct;

		private GraphLinksModel<ActivityNode, string, String, ActivityLink> _paletteModel;
		/// <summary>
		/// Palette控件数据
		/// </summary>
		public GraphLinksModel<ActivityNode, string, String, ActivityLink> PaletteModel
		{
			get
			{
				if (_paletteModel == null)
				{
					_paletteModel = new GraphLinksModel<ActivityNode, string, string, ActivityLink>();
					_paletteModel.Modifiable = true;
					var defaultActNodeKey = "00c5e8cd-d5b8-43fe-a38c-b54c37507873";

					var nodeList = new List<ActivityNode>()
					{
						new ActivityNode(){ 
							Key=defaultActNodeKey, Category="Normal", WfName = "活动点名称", 
							WfDescription="活动点描述", WfEnabled=true,Text="Normal", Figure=NodeFigure.Rectangle}
						//new ActivityNode(){ Key="Conditional", Category="Conditional",Text="IF3", Figure=NodeFigure.Decision},
						//new ActivityNode(){ Key="Composite", Category="Composite",Text="Composite4", Figure=NodeFigure.FramedRectangle}
					};

					var actInfoList = WebInterAct.LoadActivityTemplate();
					this.TemplateKeys.Add(defaultActNodeKey);
					foreach (var actInfo in actInfoList)
					{
						nodeList.Add(new ActivityNode()
						{
							Key = actInfo.Key,//"Normal",设置为normal在使用时生成新的Key
							TemplateID = actInfo.Key,
							Category = actInfo.ActivityType.ToString(),
							WfName = actInfo.Name,
							WfEnabled = actInfo.Enabled,
							WfDescription = actInfo.Description
						});

						this.TemplateKeys.Add(actInfo.Key);
					}

					_paletteModel.NodesSource = nodeList;
				}

				return _paletteModel;
			}
		}

		private ObservableCollection<DiagramPageViewModel> _diagramDataSource;
		public ObservableCollection<DiagramPageViewModel> DiagramDataSource
		{
			get
			{
				if (_diagramDataSource == null)
				{
					_diagramDataSource = new ObservableCollection<DiagramPageViewModel>();
				}

				return _diagramDataSource;
			}
		}

		private TabItem _selectedItem;
		/// <summary>
		/// 当前选中的Tab
		/// </summary>
		public TabItem SelectedItem
		{
			get { return _selectedItem; }
			set
			{
				if (_selectedItem == value) return;

				_selectedItem = value;
				OnPropertyChanged("SelectedItem");

				if (value == null) return;

				var vw = value.DataContext as DiagramPageViewModel;
				WebInterAct.LoadProperty(WorkflowUtils.CLIENTSCRIPT_PARAM_WORKFLOW,
					vw.Key,
					WorkflowUtils.ExtractWorkflowInfoJson(vw));
			}
		}

		/// <summary>
		/// 当前编辑的流程
		/// </summary>
		public DiagramPageViewModel CurrentDiagram
		{
			get
			{
				if (_selectedItem == null) return null;
				return _selectedItem.DataContext as DiagramPageViewModel;
			}
		}

		/// <summary>
		/// 存储活动点模板Key,
		/// </summary>
		public List<string> TemplateKeys = new List<string>();
		#endregion

		#region commands
		public RelayCommand<WorkflowInfo> AddNewCommand
		{
			get;
			private set;
		}
		#endregion


		#region private
		/// <summary>
		/// Tab关闭时触发
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void Item_RequestClose(object sender, EventArgs e)
		{
			DiagramPageViewModel item = sender as DiagramPageViewModel;
			DiagramDataSource.Remove(item);
			this.WebInterAct.DeleteProcess(WorkflowUtils.CLIENTSCRIPT_PARAM_WORKFLOW,
				item.Key,
				WorkflowUtils.ExtractWorkflowInfoJson(item));
		}

		private void ItemModeAdd(object sender, EventArgs e)
		{

		}

		private void AddNewCommandExecuted(WorkflowInfo info)
		{
			if (info == null) throw new ArgumentNullException("WorkflowInfo不能为空");

			DiagramPageViewModel vw = new DiagramPageViewModel(info, WebInterAct);

			vw.RequestClose += this.Item_RequestClose;
			this.DiagramDataSource.Add(vw);
			WebInterAct.LoadProperty(WorkflowUtils.CLIENTSCRIPT_PARAM_WORKFLOW,
				vw.Key,
				WorkflowUtils.ExtractWorkflowInfoJson(vw));
		}
		#endregion
	}
}
