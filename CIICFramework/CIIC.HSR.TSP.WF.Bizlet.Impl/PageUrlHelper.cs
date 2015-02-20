using CIIC.HSR.TSP.DataAccess;
using CIIC.HSR.TSP.IoC;
using CIIC.HSR.TSP.WF.Bizlet.Contract;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CIIC.HSR.TSP.WF.Bizlet.Impl
{
	/// <summary>
	/// 租户相关帮助类
	/// </summary>
	public class PageUrlHelper
	{
		public static string GetUrl(string pageUrlName)
		{
			UrlSection urlSection = ConfigurationManager.GetSection("urlSetting") as UrlSection;

			string registerUrl = "/MCSWebApp/OACommonPages/AppTrace/appTraceViewer.aspx";

			if (urlSection != null)
			{
				UrlElement urlElem = urlSection.Urls.GetUrlElement(pageUrlName);

				if (urlElem != null && string.IsNullOrEmpty(urlElem.Url) == false)
					registerUrl = urlSection.Urls.GetUrlElement(pageUrlName).Url;
			}

			return registerUrl;
		}
	}
}
