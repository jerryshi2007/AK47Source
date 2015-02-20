using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using MCS.Library.Core;
using MCS.Library.OGUPermission;
using MCS.Library.Passport;
using MCS.Library.Principal;

namespace MCS.OA.Portal.Mechanism
{
	public partial class RemoveMechanismCache : System.Web.UI.Page
	{
		protected void Page_Load(object sender, EventArgs e)
		{
			ExceptionHelper.FalseThrow(DeluxePrincipal.Current.IsInRole(RolesDefineConfig.GetConfig().RolesDefineCollection["ProcessAdmin"].Roles), "你没有查看此页的权限");
		}

		protected void ButtonRemoveOguCache_Click(object sender, EventArgs e)
		{
			OguMechanismFactory.GetMechanism().RemoveAllCache();
			Successful();
		}

		protected void ButtonRemovePermissionCache_Click(object sender, EventArgs e)
		{
			PermissionMechanismFactory.GetMechanism().RemoveAllCache();
			Successful();
		}

		private void Successful()
		{
			this.ClientScript.RegisterStartupScript(this.GetType(), "successful",
				"<script type = 'text/javascript'>alertSuccessful();</script>");
		}
	}
}