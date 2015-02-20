using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using MCS.Library.OGUPermission;
using MCS.Library.SOA.DataObjects;

namespace MCS.Library.SOA.Web.WebControls.Test.OuNavigatorControl
{
	public partial class OuNavigatorTest : System.Web.UI.Page
	{
		protected void Page_Load(object sender, EventArgs e)
		{
			navigator.ObjectID = OguMechanismFactory.GetMechanism().GetObjects<IUser>(SearchOUIDType.LogOnName, "fanhy")[0].ID;
		}
	}
}