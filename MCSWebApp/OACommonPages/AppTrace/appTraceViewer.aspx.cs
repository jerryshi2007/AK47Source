using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using MCS.Web.Library;

namespace MCS.OA.CommonPages.AppTrace
{
	public partial class appTraceViewer : System.Web.UI.Page
	{
		protected void Page_Load(object sender, EventArgs e)
		{
			Response.Redirect(this.processViewer.DialogUrl);
		}
	}
}