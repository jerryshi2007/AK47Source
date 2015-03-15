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
using System.Collections.Generic;

namespace Designer.Models
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
