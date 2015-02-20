using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Configuration;
using MCS.Library.Configuration;

namespace MCS.Web.WebControls
{
	public sealed class OfficeViewerWrapperSettings : ConfigurationSection
	{
		private OfficeViewerWrapperSettings()
		{
		}

		public static OfficeViewerWrapperSettings GetConfig()
		{
			OfficeViewerWrapperSettings settings = (OfficeViewerWrapperSettings)ConfigurationBroker.GetSection("officeViewerWrapperSettings");

			if (settings == null)
				settings = new OfficeViewerWrapperSettings();

			return settings;
		}

		[ConfigurationProperty("codebase", IsRequired = false, DefaultValue = "/MCSWebApp/HBWebHelperControl/officeviewer.cab#7,5,0,331")]
		public string Codebase
		{
			get
			{
				return (string)this["codebase"];
			}
			set
			{
				this["codebase"] = value;
			}
		}

		[ConfigurationProperty("licenseName", IsRequired = false, DefaultValue = "30daytrial")]
		public string LicenseName
		{
			get
			{
				return (string)this["licenseName"];
			}
			set
			{
				this["licenseName"] = value;
			}
		}

		[ConfigurationProperty("licenseCode", IsRequired = false, DefaultValue = "EDWD-3333-2222-1111")]
		public string LicenseCode
		{
			get
			{
				return (string)this["licenseCode"];
			}
			set
			{
				this["licenseCode"] = value;
			}
		}

		[ConfigurationProperty("classID", IsRequired = false, DefaultValue = "clsid:7677E74E-5831-4C9E-A2DD-9B1EF9DF2DB4")]
		public string ClassID
		{
			get
			{
				return (string)this["classID"];
			}
			set
			{
				this["classID"] = value;
			}
		}
	}
}
