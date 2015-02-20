using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using MCS.Library.Core;

namespace ResponsivePassportService.Template
{
	public partial class ResponsiveBindingTemplate : System.Web.UI.UserControl
	{
		protected void Page_Load(object sender, EventArgs e)
		{
			if (this.IsPostBack == false)
				signInName.Text = Request.QueryString.GetValue("userName", string.Empty);
		}
	}
}