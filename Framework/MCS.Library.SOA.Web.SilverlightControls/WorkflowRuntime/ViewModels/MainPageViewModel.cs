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
using WorkflowRuntime.Models;
using System.Collections.Generic;
using Northwoods.GoXam;
using System.Collections.ObjectModel;
using Northwoods.GoXam.Model;
using WorkflowRuntime.Commands;
using System.Xml.Linq;
using System.ComponentModel;
using WorkflowRuntime.Utils;


namespace WorkflowRuntime.ViewModels
{
	public class MainPageViewModel : ViewModelBase
	{
		public MainPageViewModel()
		{
			this.AddNewCommand = new RelayCommand<WorkflowInfo>(this.AddNewCommandExecuted, p => { return true; });
		}

		#region Fields
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
		}

		private void AddNewCommandExecuted(WorkflowInfo info)
		{
			if (info == null) throw new ArgumentNullException("WorkflowInfo不能为空");
			WorkflowUtils.ProcessDatetime(info);
			DiagramPageViewModel vw = new DiagramPageViewModel(info);

			vw.RequestClose += this.Item_RequestClose;
			this.DiagramDataSource.Add(vw);

			this.OnPropertyChanged("DiagramDataSource");
		}
		#endregion
	}
}
