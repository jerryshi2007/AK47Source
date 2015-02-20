using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using MCS.Library.Core;

namespace ResponsivePassportService.Anonymous
{
	public partial class OpenIDBridge : System.Web.UI.Page
	{
		protected void Page_Load(object sender, EventArgs e)
		{
			this.openIDField.Value = string.Format("openID={0}&openIDType={1}&userName={2}",
				Request.QueryString.GetValue("openID", string.Empty),
				Request.QueryString.GetValue("openIDType", string.Empty),
				HttpUtility.UrlEncode(Request.QueryString.GetValue("userName", string.Empty)));
		}
	}
}