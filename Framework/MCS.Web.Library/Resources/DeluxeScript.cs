using System;
using System.Collections.Generic;
using System.Text;
using System.Web.UI;

[assembly: WebResource("MCS.Web.Library.Resources.Deluxe.js", "text/javascript")]

namespace MCS.Web.Library
{
    /// <summary>
    /// 
    /// </summary>
    [Script.ClientScriptResource(null, "MCS.Web.Library.Resources.Deluxe.js")]
    public sealed class DeluxeScript
    {
		/// <summary>
		/// 接收命令的客户端Input控件ID
		/// </summary>
		public const string C_CommandIputClientID = "__commandInput";
    }
}
