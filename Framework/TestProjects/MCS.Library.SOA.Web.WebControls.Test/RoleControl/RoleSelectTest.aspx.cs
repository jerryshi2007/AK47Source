using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace MCS.Library.SOA.Web.WebControls.Test.RoleControl
{
	public partial class RoleSelectTest : System.Web.UI.Page
	{
		protected void Page_Load(object sender, EventArgs e)
		{

		}

		protected override void OnPreRender(EventArgs e)
		{
			selectedResult.InnerText = this.roleSelector.SelectedFullCodeName;

			base.OnPreRender(e);
		}
	}
}