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
using WorkflowRuntime.ViewModels;
using System.Windows.Browser;
using WorkflowRuntime.Models;
using System.Xml.Linq;
using Newtonsoft.Json;
using WorkflowRuntime.Utils;
using WorkflowRuntime.Views;
using Northwoods.GoXam;
using System.Text;

namespace WorkflowRuntime
{
	public partial class MainPage : UserControl
	{
		private MainPageViewModel _mainPageVW;
		public string WrapperID { get; private set; }

		public MainPage(string wrapperid)
		{
			InitializeComponent();

			HiddenMouseRightButton();

			InitializeViewModel();

			this.WrapperID = wrapperid;
			ShowWfDiagram();

			HtmlPage.RegisterScriptableObject("SLM", this);

            LayoutRoot.AddHandler(Grid.MouseLeftButtonUpEvent, new MouseButtonEventHandler(this.LayoutRoot_MouseLeftButtonUp), false);
            LayoutRoot.AddHandler(Grid.MouseRightButtonUpEvent, new MouseButtonEventHandler(this.LayoutRoot_MouseRightButtonUp), false);

		}

		private void ShowWfDiagram()
		{
			var info = WebInteraction.LoadProcInfo(this.WrapperID);
			if (info == null) return;
			_mainPageVW.AddNewCommand.Execute(info);
		}

		private void InitializeViewModel()
		{
			_mainPageVW = new MainPageViewModel();
			this.DataContext = _mainPageVW;
		}

		private void HiddenMouseRightButton()
		{
			this.MouseRightButtonDown += (o, e) => e.Handled = true;
		}

		private bool IsExistWorkflowInfo(string wfKey)
		{
			if (_mainPageVW.DiagramDataSource.Count(p => p.WfInfo.Key == wfKey) > 0) return true;

			return false;
		}

		private void ChangeSelectedItem(string wfKey)
		{
			var item = (TabItem)tabControl.Items.Single(p => ((DiagramPageViewModel)((TabItem)p).DataContext).Key == wfKey);

			this.tabControl.SelectedItem = item;
		}


		#region client method
		[ScriptableMember]
		public void OpenBranchProcess(string procJsonStr)
		{
			List<WorkflowInfo> wfList = JsonConvert.DeserializeObject<List<WorkflowInfo>>(procJsonStr);
			foreach (WorkflowInfo info in wfList)
			{
				if (IsExistWorkflowInfo(info.Key))
				{
					ChangeSelectedItem(info.Key);
				}
				else
				{
					_mainPageVW.AddNewCommand.Execute(info);
				}

			}
		}

		[ScriptableMember]
		public string GetProcessIDList()
		{
			if (_mainPageVW.DiagramDataSource.Count == 0) return "";

			StringBuilder strBuilder = new StringBuilder();
			foreach (var data in _mainPageVW.DiagramDataSource)
			{
				strBuilder.Append(data.Key);
				strBuilder.Append(",");
			}

			strBuilder.Remove(strBuilder.Length - 1, 1);
			return strBuilder.ToString();
		}

		[ScriptableMember]
		public void SetSelectedItem(string processID)
		{
			if (IsExistWorkflowInfo(processID))
			{
				ChangeSelectedItem(processID);
			}
		}

		[ScriptableMember]
		public string GetCurrentTabSelectedElementData()
		{
			var selectedItem = this.tabControl.SelectedItem as TabItem;

			if (selectedItem == null)
				return string.Empty;

			var form = selectedItem.Content as UserControl;

			if (form == null)
				return string.Empty;

			var diagram = form.FindName("mainDiagram") as Northwoods.GoXam.Diagram;

			if (diagram == null)
				return string.Empty;

			var selectedNode = diagram.SelectedNode;

			if (selectedNode == null)
				return string.Empty;

			var data = selectedNode.Data;

			if (data == null)
				return string.Empty;

			return JsonConvert.SerializeObject(data);
		}

		[ScriptableMember]
		public string GetSelectedCurrentProcessKey()
		{
			var selectedItem = this.tabControl.SelectedItem as TabItem;
			if (selectedItem == null) return "";

			return (selectedItem.DataContext as DiagramPageViewModel).Key;
		}

		[ScriptableMember]
		public string GetSelectedCurrentProcessWfInfo()
		{
			string strResult = "";

			TabItem selectedItem = this.tabControl.SelectedItem as TabItem;
			if (selectedItem != null)
			{
				WorkflowInfo selectedInfo = (selectedItem.DataContext as DiagramPageViewModel).WfInfo;
				strResult = string.Format("{0},{1}", selectedInfo.ResourceID, selectedInfo.Key);
				//strResult = JsonConvert.SerializeObject(selectedInfo);
			}

			return strResult;
		}

		[ScriptableMember]
		public string GetSelectedProcessInfo()
		{
			string strResult = "";

			TabItem selectedItem = this.tabControl.SelectedItem as TabItem;
			if (selectedItem != null)
			{
				WorkflowInfo selectedInfo = (selectedItem.DataContext as DiagramPageViewModel).WfInfo;
				strResult = JsonConvert.SerializeObject(selectedInfo);
			}

			return strResult;
		}

		#endregion

		[ScriptableMember]
		public string SelectedProcessInfoFuncName
		{
			get;
			set;
		}

		private void tabControl_SelectionChanged(object sender, SelectionChangedEventArgs e)
		{
			if (!string.IsNullOrEmpty(SelectedProcessInfoFuncName))
			{
				string js = string.Format(@"if (typeof {0} != 'undefined'& typeof {0} =='function') {0}", SelectedProcessInfoFuncName);

				js += string.Format("('{0}')", this.WrapperID);

				HtmlPage.Window.Eval(js);
			}

		}


        #region OnWfElementClicked

        /// <summary>
        /// 事件类型：左键，右键
        /// </summary>
        public enum EventType
        {
            Left = 0,
            Right = 1,
        }

        /// <summary>
        ///  对象类型：空白，点，线
        /// </summary>
        public enum SrcElement
        {
            Null = 0,
            Point = 1,
            Line = 2,
        }

        /// <summary>
        /// 选中对象
        /// </summary>
        private class WfElement
        {
            /// <summary>
            /// 事件类型
            /// </summary>
            public EventType EventType { get; set; }
            /// <summary>
            /// X坐标
            /// </summary>
            public double X { get; set; }
            /// <summary>
            /// Y坐标
            /// </summary>
            public double Y { get; set; }
            /// <summary>
            /// 对象信息
            /// </summary>
            public string ExtraInfo { get; set; }
            /// <summary>
            /// 对象类型
            /// </summary>
            public SrcElement SrcElement { get; set; }
        }

        /// <summary>
        /// 获取对象信息
        /// </summary>
        /// <param name="element">对象类型</param>
        /// <param name="selectedID">选中对象ID</param>
        /// <returns>对象信息（对象序列化的字符串信息）</returns>
        private string GetExtraInfo(SrcElement element, string selectedID)
        {
            string extraInfo = string.Empty;
            if (!string.IsNullOrEmpty(selectedID))
            {
                TabItem selectedItem = this.tabControl.SelectedItem as TabItem;
                if (selectedItem != null)
                {
                    WorkflowInfo selectedInfo = (selectedItem.DataContext as DiagramPageViewModel).WfInfo;
                    if (element == SrcElement.Point)
                    {
                        foreach (var activity in selectedInfo.Activities)
                        {
                            if (activity.ID == selectedID)
                            {
                                extraInfo = JsonConvert.SerializeObject(activity);
                                break;
                            }
                        }
                    }
                    else if (element == SrcElement.Line)
                    {
                        foreach (var transction in selectedInfo.Transitions)
                        {
                            if (transction.Key == selectedID)
                            {
                                extraInfo = JsonConvert.SerializeObject(transction);
                                break;
                            }
                        }
                    }
                }
            }
            return extraInfo;
        }

        /// <summary>
        /// 获取选中对象
        /// </summary>
        /// <param name="e">事件数据</param>
        /// <param name="eventType">事件类型</param>
        /// <returns>选中对象</returns>
        private WfElement GetWfElement(MouseButtonEventArgs e, EventType eventType)
        {
            WfElement element = new WfElement();
            element.X = e.GetPosition(null).X;
            element.Y = e.GetPosition(null).Y;
            element.EventType = eventType;
            if (((System.Windows.FrameworkElement)e.OriginalSource).DataContext is WorkflowRuntime.ViewModels.DiagramPageViewModel)
            {
                element.SrcElement = SrcElement.Null;
                element.ExtraInfo = string.Empty;
            }
            else if (((System.Windows.FrameworkElement)e.OriginalSource).DataContext is Northwoods.GoXam.PartManager.PartBinding
                && ((Northwoods.GoXam.PartManager.PartBinding)(((System.Windows.FrameworkElement)(((System.Windows.RoutedEventArgs)(e)).OriginalSource)).DataContext)).Node != null)
            {
                string selectedNodeID = ((WorkflowRuntime.Models.ActivityNode)(((Northwoods.GoXam.Part)(((Northwoods.GoXam.PartManager.PartBinding)(((System.Windows.FrameworkElement)(((System.Windows.RoutedEventArgs)(e)).OriginalSource)).DataContext)).Node)).Data)).InstanceID;

                element.ExtraInfo = GetExtraInfo(SrcElement.Point, selectedNodeID);
                element.SrcElement = SrcElement.Point;

            }
            else if (((System.Windows.FrameworkElement)e.OriginalSource).DataContext is Northwoods.GoXam.PartManager.PartBinding
                && ((Northwoods.GoXam.PartManager.PartBinding)(((System.Windows.FrameworkElement)(((System.Windows.RoutedEventArgs)(e)).OriginalSource)).DataContext)).Link != null)
            {
                string selectedLineID = ((WorkflowRuntime.Models.ActivityLink)(((Northwoods.GoXam.Part)(((Northwoods.GoXam.PartManager.PartBinding)(((System.Windows.FrameworkElement)(((System.Windows.RoutedEventArgs)(e)).OriginalSource)).DataContext)).Link)).Data)).Key;

                element.ExtraInfo = GetExtraInfo(SrcElement.Line, selectedLineID);
                element.SrcElement = SrcElement.Line;
            }
            return element;
        }

        /// <summary>
        /// 调用事件处理方法
        /// </summary>
        /// <param name="element">选中对象</param>
        private void WfElementClicked(WfElement element)
        {
            string js = @"var data={}; data.eventType='" + ((int)element.EventType).ToString() + "';data.x=" + element.X.ToString() + ";data.y=" + element.Y.ToString() + ";data.extraInfo='" + element.ExtraInfo + "';data.srcElement='" + ((int)element.SrcElement).ToString() + "'; if (typeof OnWfElementClicked != 'undefined'& typeof OnWfElementClicked =='function') OnWfElementClicked(data);";
            HtmlPage.Window.Eval(js);
        }
        #endregion

        private void LayoutRoot_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            e.Handled = true;
            WfElementClicked(GetWfElement(e, EventType.Left));
        }

		private void LayoutRoot_MouseRightButtonUp(object sender, MouseButtonEventArgs e)
		{
			e.Handled = true;
			WfElementClicked(GetWfElement(e, EventType.Right));
		}
	}
}
