using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using MCS.Web.Library;
using MCS.Web.WebControls;

namespace WorkflowDesigner.ModalDialog
{
	public partial class WfRelativeLinkEditor : System.Web.UI.Page
	{
		protected void Page_Load(object sender, EventArgs e)
		{
			WebUtility.RequiredScript(typeof(ClientGrid));
		}
	}
}