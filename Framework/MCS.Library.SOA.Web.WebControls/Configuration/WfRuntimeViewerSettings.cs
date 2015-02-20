using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Configuration;
using MCS.Library.Configuration;

namespace MCS.Web.WebControls.Configuration
{
	public class WfRuntimeViewerSettings : ConfigurationSection
	{
		public static WfRuntimeViewerSettings GetConfig()
		{
			WfRuntimeViewerSettings settings = (WfRuntimeViewerSettings)ConfigurationBroker.GetSection("wfRuntimeViewerSettings");

			if (settings == null)
				settings = new WfRuntimeViewerSettings();

			return settings;
		}

		[ConfigurationProperty("xapUrl", IsRequired = false, DefaultValue = "")]
		public string XapUrl
		{
			get
			{
				return (string)this["xapUrl"];
			}
			set
			{
				this["xapUrl"] = value;
			}
		}

		[ConfigurationProperty("designerUrl", IsRequired = false, DefaultValue = @"http://localhost/MCSWebApp/WorkflowDesigner/default.aspx")]
		public string DesignerUrl
		{
			get
			{
				return (string)this["designerUrl"];
			}
			set
			{
				this["designerUrl"] = value;
			}
		}

		/*
		[ConfigurationProperty("linkPageUrl", IsRequired = false, DefaultValue = @"http://localhost/MCSWebApp/OACommonPages/AppTrace/appTraceViewer.aspx")]
		public string LinkPageUrl
		{
			get
			{
				return (string)this["linkPageUrl"];
			}
			set
			{
				this["linkPageUrl"] = value;
			}

		} */
	}
}
