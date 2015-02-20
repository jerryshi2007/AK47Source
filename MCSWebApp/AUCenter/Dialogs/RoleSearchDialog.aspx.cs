using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using MCS.Library.SOA.DataObjects.Security.Client;

namespace AUCenter.Dialogs
{
	public partial class RoleSearchDialog : System.Web.UI.Page
	{
		protected override void OnInit(EventArgs e)
		{
			base.OnInit(e);
		}

		protected override void OnLoad(EventArgs e)
		{
			base.OnLoad(e);
			this.sm.Services.Add(new ServiceReference() { Path = PCServiceClientSettings.GetConfig().QueryServiceAddress.AbsoluteUri });
		}
	}
}