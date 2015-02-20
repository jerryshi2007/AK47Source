using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace PermissionCenter
{
	[Serializable]
	public class AdditionOperation
	{
		public string Label { get; set; }

		public bool Popup { get; set; }

		public string Url { get; set; }

		public string Target { get; set; }

		public AdditionOperation()
		{

		}

		public AdditionOperation(string label, bool popup, string url)
		{
			this.Label = label;
			this.Popup = popup;
			this.Url = url;
		}

		public AdditionOperation(string label, bool popup, string url, string target)
			: this(label, popup, url)
		{
			this.Target = target;
		}
	}
}