using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using MCS.Library.Core;
using MCS.Web.Library;
using MCS.Library.OGUPermission;

namespace MCS.Library.SOA.Web.WebControls.Test.OuNavigatorControl
{
	public partial class OguTarget : System.Web.UI.Page
	{
		protected void Page_Load(object sender, EventArgs e)
		{
			string oguID = WebUtility.GetRequestQueryString("id", string.Empty);

			if (oguID.IsNotEmpty())
			{
				IOrganization org = OguMechanismFactory.GetMechanism().GetObjects<IOrganization>(SearchOUIDType.Guid, oguID)[0];
				oguInfo.Text = HttpUtility.HtmlEncode(string.Format("部门ID为{0}，名称为{1}", org.ID, org.DisplayName));
			}
		}
	}
}