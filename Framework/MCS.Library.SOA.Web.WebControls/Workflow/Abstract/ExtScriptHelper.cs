using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MCS.Library.SOA.DataObjects;

namespace MCS.Web.WebControls
{
	/// <summary>
	/// 生成脚本或Html
	/// </summary>
	internal static class ExtScriptHelper
	{
		public const int DefaultResponseTimeout = 1000;

		public static string GetRefreshBridgeScript()
		{
			string html = string.Empty;

			if (ResourceUriSettings.GetConfig().Paths.ContainsKey("refreshBridge"))
			{
				html = string.Format("<iframe style=\"display:none\" src=\"{0}\"></iframe>",
					ResourceUriSettings.GetConfig().Paths["refreshBridge"].Uri.ToString());
			}

			return html;
		}
	}
}
