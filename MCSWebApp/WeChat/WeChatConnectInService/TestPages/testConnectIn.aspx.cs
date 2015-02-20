using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using MCS.Library.Core;

namespace WeChatConnectInService.TestPages
{
	public partial class testConnectIn : System.Web.UI.Page
	{
		protected void Page_Load(object sender, EventArgs e)
		{
			long ticks = DateTime.Now.ToJavascriptDateNumber();

			DateTime dt = ticks.JavascriptDateNumberToDateTime();
		}
	}
}