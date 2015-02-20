using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using MCS.Library.OGUPermission;
using MCS.Library.SOA.DataObjects;
using MCS.Web.Library.Script;

namespace MCS.Library.SOA.Web.WebControls.Test.OuUserInputControl
{
	public partial class clientSetDataTest : System.Web.UI.Page
	{
		protected void Page_Load(object sender, EventArgs e)
		{
			JSONSerializerExecute.RegisterConverter(typeof(OguObjectConverter));
		}

		protected override void OnPreRender(EventArgs e)
		{
			IUser testUser = OguMechanismFactory.GetMechanism().GetObjects<IUser>(SearchOUIDType.LogOnName, "fanhy")[0];

			testUser = (IUser)OguUser.CreateWrapperObject(testUser);

			this.ClientScript.RegisterHiddenField("userData", JSONSerializerExecute.Serialize(testUser));

			base.OnPreRender(e);
		}
	}
}