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

namespace WorkflowRuntime.Models
{
	public class ActivityLink : GraphLinksModelLinkData<String, String>
	{
		private string _key;
		public string Key
		{
			get { return _key; }
			set
			{
				if (_key != value)
				{
					var oldval = _key;
					_key = value;
					RaisePropertyChanged("Key", oldval, value);
				}
			}
		}

		private bool _wfEnabled;
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

		private bool _wfReturnLine;
		public bool WfReturnLine
		{
			get { return _wfReturnLine; }
			set
			{
				if (_wfReturnLine != value)
				{
					var oldval = _wfReturnLine;
					_wfReturnLine = value;
					RaisePropertyChanged("WfReturnLine", oldval, value);
				}
			}
		}

		#region 运行时属性
		private Color _heightlight;
		public Color Heightlight
		{
			get
			{
				if (_heightlight == null)
				{
					_heightlight = Colors.Red;
				}
				return _heightlight;
			}
			set
			{
				if (_heightlight != value)
				{
					var oldval = _heightlight;
					_heightlight = value;
					RaisePropertyChanged("Heightlight", oldval, value);
				}
			}
		}

		private bool _wfRuntimeIsPassed;
		public bool WfRuntimeIsPassed
		{
			get { return _wfRuntimeIsPassed; }
			set
			{
				if (_wfRuntimeIsPassed != value)
				{
					var oldval = _wfRuntimeIsPassed;
					_wfRuntimeIsPassed = value;
					RaisePropertyChanged("WfRuntimeIsPassed", oldval, value);
				}
			}
		}
		#endregion

		public override System.Xml.Linq.XElement MakeXElement(System.Xml.Linq.XName n)
		{
			var xelement = base.MakeXElement(n);
			xelement.Add(XHelper.Attribute("Key", this.Key, ""));
			xelement.Add(XHelper.Attribute("WfEnabled", this.WfEnabled, !this.WfEnabled));
			xelement.Add(XHelper.Attribute("WfReturnLine", this.WfReturnLine, !this.WfReturnLine));
			return xelement;
		}

		public override void LoadFromXElement(System.Xml.Linq.XElement e)
		{
			base.LoadFromXElement(e);

			this.Key = XHelper.Read("Key", e, "");
			this.WfEnabled = XHelper.Read("WfEnabled", e, true);
			this.WfReturnLine = XHelper.Read("WfReturnLine", e, false);
		}
	}
}
