using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace WorkflowDesigner.MatrixModalDialog
{
	public partial class DownloadMatrix : System.Web.UI.Page
	{
		protected void Page_Load(object sender, EventArgs e)
		{
			string url = this.ResolveUrl("~/MatrixModalDialog/EditRoleProperty.aspx?editMode=download&RoleID=" + Request.QueryString["RoleID"]);
			this.meta1.Content = "1;url=" + url;
			this.lnkTo.NavigateUrl = url;
		}
	}
}