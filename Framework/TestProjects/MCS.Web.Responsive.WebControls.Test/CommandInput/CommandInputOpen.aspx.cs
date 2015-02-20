using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using MCS.Web.Responsive.Library;

namespace MCS.Web.Responsive.WebControls.Test.CommandInput
{
	public partial class CommandInputOpen : System.Web.UI.Page
	{
		protected void Page_Load(object sender, EventArgs e)
		{

		}

		protected void refreshParentWindow_Click(object sender, EventArgs e)
		{
			Response.WriteRefreshParentWindowScriptBlock();
		}

		protected void closeWindow_Click(object sender, EventArgs e)
		{
			Response.WriteCloseWindowScriptBlock();
		}
	}
}