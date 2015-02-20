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
using Northwoods.GoXam;
using System.Xml.Linq;
using Designer.Commands;
using System.Windows.Browser;
using Designer.Utils;

namespace Designer.Models
{
	public class ActivityNode : GraphLinksModelNodeData<string>
	{
		#region composite node properties

		private NodeFigure _figure = NodeFigure.Rectangle;
		public NodeFigure Figure
		{
			get { return _figure; }
			set
			{
				if (_figure != value)
				{
					var oldval = _figure;
					_figure = value;
					RaisePropertyChanged("Figure", oldval, value);
				}
			}
		}

		private string _templateID;
		public string TemplateID
		{
			get { return _templateID; }
			set
			{
				if (_templateID != value)
				{
					var oldval = _figure;
					_templateID = value;
					RaisePropertyChanged("TemplateID", oldval, value);
				}
			}
		}

		private string _wfName;
		/// <summary>
		/// 活动点名称
		/// </summary>
		public string WfName
		{
			get { return _wfName; }
			set
			{
				if (_wfName != value)
				{
					var oldval = _wfName;
					_wfName = value;
					RaisePropertyChanged("WfName", oldval, value);
				}
			}
		}

		private string _wfDescription;
		/// <summary>
		/// 活动点描述
		/// </summary>
		public string WfDescription
		{
			get { return _wfDescription; }
			set
			{
				if (_wfDescription != value)
				{
					var oldval = _wfDescription;
					_wfDescription = value;
					RaisePropertyChanged("WfDescription", oldval, value);
				}
			}
		}

		private bool _wfEnabled;
		/// <summary>
		/// 活动点是否可用
		/// </summary>
		public bool WfEnabled
		{
			get { return _wfEnabled; }
			set
			{
				if (_wfEnabled != value)
				{
					var oldval = _wfEnabled;
					_wfEnabled = value;
					RaisePropertyChanged("WfEnabled", oldval, value);
				}
			}
		}

		private bool _IsDynamic = false;

		public bool IsDynamic
		{
			get { return this._IsDynamic; }
			set
			{
				if (this._IsDynamic != value)
				{
					var oldval = this._IsDynamic;
					this._IsDynamic = value;

					RaisePropertyChanged("IsDynamic", oldval, value);

					//this.SetCurrentCategory();
				}
			}
		}

		private bool _wfHasBranchProcess;
		/// <summary>
		/// 是否包含分支流程
		/// </summary>
		public bool WfHasBranchProcess
		{
			get { return _wfHasBranchProcess; }
			set
			{
				if (_wfHasBranchProcess != value)
				{
					var oldval = _wfHasBranchProcess;
					_wfHasBranchProcess = value;
					RaisePropertyChanged("WfHasBranchProcess", oldval, value);
				}
			}
		}
		#endregion

		public override XElement MakeXElement(XName n)
		{
			XElement rootElement = base.MakeXElement(n);
			rootElement.Add(XHelper.Attribute("WfName", this.WfName, ""));

			rootElement.Add(XHelper.Attribute("WfDescription", this.WfDescription, ""));
			rootElement.Add(XHelper.Attribute("WfEnabled", this.WfEnabled, true));
			rootElement.Add(XHelper.Attribute("IsDynamic", this.IsDynamic, false));
			return rootElement;
		}

		public override void LoadFromXElement(XElement e)
		{
			base.LoadFromXElement(e);
			this.WfName = XHelper.Read("WfName", e, "");

			this.WfDescription = XHelper.Read("WfDescription", e, "");
			this.WfEnabled = XHelper.Read("WfEnabled", e, true);
			this.IsDynamic = XHelper.Read("IsDynamic", e, false);
		}

		override public void ChangeDataValue(ModelChangedEventArgs e, bool undo)
		{
			if (e == null)
				return;
			if (e.PropertyName == "IsDynamic")
			{
				this.IsDynamic = (bool)e.GetValue(undo);
			}
			else
			{
				base.ChangeDataValue(e, undo);
			}
		}

		public ActivityNode WfClone(string newKey)
		{
			return new ActivityNode()
			{
				Key = newKey,
				WfName = this.WfName,
				WfEnabled = this.WfEnabled,
				WfDescription = this.WfDescription,
				Category = this.Category,
				Text = this.Text,
				Figure = this.Figure,
				TemplateID = newKey,
				IsDynamic = this.IsDynamic
			};
		}
		#region Command

		private RelayCommand<string> _PropertyChangeCommand = null;

		public RelayCommand<string> PropertyChangeCommand
		{
			get
			{
				if (this._PropertyChangeCommand == null)
					this._PropertyChangeCommand = new RelayCommand<string>(PropertyChangeCommandExecuted, CanPropertyValueCommand);

				return this._PropertyChangeCommand;
			}
		}

		private static void PropertyChangeCommandExecuted(string para)
		{
			HtmlPage.Window.Invoke("OpenEditor", para);
		}

		private RelayCommand<string> _SetPropertyCommand = null;

		public RelayCommand<string> SetPropertyCommand
		{
			get
			{
				if (this._SetPropertyCommand == null)
					this._SetPropertyCommand = new RelayCommand<string>(s =>
					{
						string strKey = string.Format("{0}@{1}", s, this.Key);
						if (string.Compare(strKey, WorkflowUtils.CurrentKey) != 0)
						{
							HtmlPage.Window.Invoke("SetPropertyValue", "IsDynamic", this.IsDynamic.ToString());
							//HtmlPage.Window.Invoke("LoadProperty", WorkflowUtils.CLIENTSCRIPT_PARAM_ACTIVITY, this.mainDiagram.Tag.ToString(), WorkflowUtils.ExtractActivityInfoJson(nodeData));
							WorkflowUtils.CurrentKey = strKey;
						}
					}, CanPropertyValueCommand);

				return this._SetPropertyCommand;
			}
		}

		private static bool CanPropertyValueCommand(string para)
		{
			return string.IsNullOrEmpty(para) == false;
		}

		//private void SetCurrentCategory()
		//{
		//    if (this.IsDynamic == true)
		//    {
		//        if (string.Compare("Normal", this.Category) == 0)
		//            this.Category = "Dynamic";
		//    }
		//    else
		//    {
		//        if (string.Compare(this.Category, "Dynamic") == 0)
		//            this.Category = "Normal";
		//    }
		//}
		#endregion
	}
}
