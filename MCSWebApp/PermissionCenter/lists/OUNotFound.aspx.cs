using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using MCS.Library.SOA.DataObjects.Security;

namespace PermissionCenter.lists
{
	public partial class OUNotFound : System.Web.UI.Page
	{
		protected void Page_Load(object sender, EventArgs e)
		{
			this.txtID.InnerText = Request.QueryString["ou"];
			this.hfOuId.Value = SCOrganization.RootOrganizationID;
		}
	}
}