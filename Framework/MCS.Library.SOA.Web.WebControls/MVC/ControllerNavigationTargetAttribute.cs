using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MCS.Web.Library.MVC
{
	public enum ControllerNavigationType
	{
		Transfer,
		Redirect
	}

	[AttributeUsage(AttributeTargets.Class, AllowMultiple = false, Inherited = true)]
	public class ControllerNavigationTargetAttribute : Attribute
	{
		public string Name { get; set; }
		public string Url { get; set; }
		public string FeatureName { get; set; }
		public ControllerNavigationType TransferType { get; set; }

		public ControllerNavigationTargetAttribute(string name, string url)
		{
			this.Name = name;
			this.Url = url;
		}

		public ControllerNavigationTargetAttribute(string name, string url, string featureName)
		{
			this.Name = name;
			this.Url = url;
			this.FeatureName = featureName;
		}
	}
}
