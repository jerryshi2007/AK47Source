using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using MCS.Web.WebControls;
using MCS.Web.Library;

namespace WorkflowDesigner.ModalDialog
{
	public partial class WfServiceOperationDefList : System.Web.UI.Page
	{
		protected void Page_Load(object sender, EventArgs e)
		{
			WebUtility.RequiredScript(typeof(ClientGrid));
			this.Response.Cache.SetCacheability(HttpCacheability.Public);
		}
	}
}