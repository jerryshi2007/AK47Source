using System;
using System.Collections.Generic;
using System.Text;
using System.Web.UI;

[assembly: WebResource("MCS.Web.Responsive.Library.Resources.Deluxe.js", "text/javascript")]

namespace MCS.Web.Responsive.Library
{
    /// <summary>
    /// 
    /// </summary>
    [Script.ClientScriptResource(null, "MCS.Web.Responsive.Library.Resources.Deluxe.js")]
    public sealed class DeluxeScript
    {
		/// <summary>
		/// ��������Ŀͻ���Input�ؼ�ID
		/// </summary>
		public const string C_CommandIputClientID = "__commandInput";
    }
}
