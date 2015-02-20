using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using MCS.Web.WebControls;

namespace MCS.Library.SOA.Web.WebControls.Test.UserPresenceControl
{
	public partial class ucImageTest : System.Web.UI.Page
	{
		protected void Page_Load(object sender, EventArgs e)
		{
			ucStatus.Src = ControlResources.UCStatusUrl;
		}
	}
}