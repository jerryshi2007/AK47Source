using System;
using System.Collections.Generic;
using System.Text;
using System.Configuration;

using MCS.Library.Configuration;
using MCS.Library.Core;

namespace MCS.Library.Accredit.Configuration
{
	sealed class BaseWebSection : ConfigurationSection
	{
		public static BaseWebSection GetConfig()
		{
			try
			{
				BaseWebSection section = ConfigurationBroker.GetSection("baseWebPageSection") as BaseWebSection;
				if (section == null)
					section = new BaseWebSection();

				return section;
			}
			catch (ConfigurationErrorsException ex)
			{
				System.Diagnostics.Debug.WriteLine(ex.Message + Environment.NewLine + ex.StackTrace, "re");
				//当前节点可以不配置，直接创建对象并使用默认值
				return new BaseWebSection();
				//throw new ConfigurationErrorsException(string.Format(HYW2007Resource.ConfigurationErrMgs, "baseWebPageSection"));
			}
		}

		[ConfigurationProperty("showErrorDebug", DefaultValue = false)]
		public bool ShowErrorDebug
		{
			get
			{
				return (bool)this["showErrorDebug"];
			}
		}

		[ConfigurationProperty("errorImgUrl", DefaultValue = "")]
		public string ErrorImgUrl
		{
			get
			{
				return (string)this["errorImgUrl"];
			}
		}
	}
}
