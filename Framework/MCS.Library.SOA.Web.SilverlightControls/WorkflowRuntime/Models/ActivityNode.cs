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

namespace WorkflowRuntime.Models
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


		#region 运行时属性
		private string _nodeDetail;
		public string NodeDetail
		{
			get { return _nodeDetail; }
			set
			{
				if (_nodeDetail != value)
				{
					var oldval = _nodeDetail;
					_nodeDetail = value;
					RaisePropertyChanged("NodeDetail", oldval, value);
				}
			}
		}

		private string _instanceID;
		public string InstanceID
		{
			get { return _instanceID; }
			set
			{
				if (_instanceID != value)
				{
					var oldval = _instanceID;
					_instanceID = value;
					RaisePropertyChanged("InstanceID", oldval, value);
				}
			}
		}

		private string _CloneKey =string.Empty ;
		public string CloneKey
		{
			get { return _CloneKey; }
			set
			{
				if (_CloneKey != value)
				{
					var oldval = _CloneKey;
					_CloneKey = value;
					RaisePropertyChanged("CloneKey", oldval, value);
				}
			}
		}

		private bool _wfRuntimeIsRemove;
		public bool WfRuntimeIsRemove
		{
			get { return _wfRuntimeIsRemove; }
			set
			{
				if (_wfRuntimeIsRemove != value)
				{
					var oldval = _wfRuntimeIsRemove;
					_wfRuntimeIsRemove = value;
					RaisePropertyChanged("WfRuntimeIsRemove", oldval, value);
				}
			}
		}

		private bool _wfRuntimeIsNewAdd;
		public bool WfRuntimeIsNewAdd
		{
			get { return _wfRuntimeIsNewAdd; }
			set
			{
				if (_wfRuntimeIsNewAdd != value)
				{
					var oldval = _wfRuntimeIsNewAdd;
					_wfRuntimeIsNewAdd = value;
					RaisePropertyChanged("WfRuntimeIsNewAdd", oldval, value);
				}
			}
		}

		private bool _WfRuntimeIsPending = false;
		public bool WfRuntimeIsPending
		{
			get { return this._WfRuntimeIsPending; }
			set
			{
				if (_WfRuntimeIsPending != value)
				{
					var oldval = _WfRuntimeIsPending;
					_WfRuntimeIsPending = value;
					RaisePropertyChanged("WfRuntimeIsPending", oldval, value);
				}
			}
		}

		private bool _wfRuntimeIsCurrent;
		public bool WfRuntimeIsCurrent
		{
			get { return _wfRuntimeIsCurrent; }
			set
			{
				if (_wfRuntimeIsCurrent != value)
				{
					var oldval = _wfRuntimeIsCurrent;
					_wfRuntimeIsCurrent = value;
					RaisePropertyChanged("WfRuntimeIsCurrent", oldval, value);
				}
			}
		}

		private bool _wfRuntimeIsComplete;
		public bool WfRuntimeIsComplete
		{
			get { return _wfRuntimeIsComplete; }
			set
			{
				if (_wfRuntimeIsComplete != value)
				{
					var oldval = _wfRuntimeIsComplete;
					_wfRuntimeIsComplete = value;
					RaisePropertyChanged("WfRuntimeIsComplete", oldval, value);
				}
			}
		}

		private bool _wfRuntimeHasBranchProcess;
		public bool WfRuntimeHasBranchProcess
		{
			get { return _wfRuntimeHasBranchProcess; }
			set
			{
				if (_wfRuntimeHasBranchProcess != value)
				{
					var oldval = _wfRuntimeHasBranchProcess;
					_wfRuntimeHasBranchProcess = value;
					RaisePropertyChanged("WfRuntimeHasBranchProcess", oldval, value);
				}
			}
		}

		private string _wfRuntimeOperator;
		public string WfRuntimeOperator
		{
			get { return _wfRuntimeOperator; }
			set
			{
				if (_wfRuntimeOperator != value)
				{
					var oldval = _wfRuntimeOperator;
					_wfRuntimeOperator = value;
					RaisePropertyChanged("WfRuntimeOperator", oldval, value);
				}
			}
		}

		public string P_Node_Name
		{
			get
			{
				return WebInteraction.CultureInfo.ContainsKey("名称") ? WebInteraction.CultureInfo["名称"] : "名称";
			}
		}
		public string P_Node_Operator
		{
			get
			{
				return WebInteraction.CultureInfo.ContainsKey("操作人") ? WebInteraction.CultureInfo["操作人"] : "操作人";
			}
		}

		public string P_Node_CloneKey
		{
			get
			{
				return WebInteraction.CultureInfo.ContainsKey("克隆KEY") ? WebInteraction.CultureInfo["克隆KEY"] : "克隆KEY";
			}
		}
		#endregion

		#endregion



		public override XElement MakeXElement(XName n)
		{
			XElement rootElement = base.MakeXElement(n);
			rootElement.Add(XHelper.Attribute("WfName", this.WfName, ""));

			rootElement.Add(XHelper.Attribute("WfDescription", this.WfDescription, ""));
			rootElement.Add(XHelper.Attribute("WfEnabled", this.WfEnabled, true));
			return rootElement;
		}

		public override void LoadFromXElement(XElement e)
		{
			base.LoadFromXElement(e);
			this.WfName = XHelper.Read("WfName", e, "");

			this.WfDescription = XHelper.Read("WfDescription", e, "");
			this.WfEnabled = XHelper.Read("WfEnabled", e, true);
		}
	}
}
