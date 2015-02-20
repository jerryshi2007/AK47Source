using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using MCS.Web.Library;
using MCS.Library.Core;

namespace WorkflowDesigner.ModalDialog
{
	public partial class WfConditionEditor : System.Web.UI.Page
	{
		protected void Page_Load(object sender, EventArgs e)
		{
            bool showNameInput = Request.QueryString.GetValue<bool>("showNameInput", false);
            txtName.Attributes["style"] = showNameInput ? "display:block" : "display:none";
            divName.Attributes["style"] = showNameInput ? "display:block" : "display:none";
		}
	}
}