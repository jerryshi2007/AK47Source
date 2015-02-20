using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using MCS.Web.WebControls;
using MCS.Library.OGUPermission;
using System.Web.UI.HtmlControls;
using System.Text;

namespace MCS.Library.SOA.Web.WebControls.Test
{
    public partial class TestWithUpdatePanel : System.Web.UI.Page
	{
        protected void Page_Load(object sender, EventArgs e)
        {
            userPresence.UserID = OguMechanismFactory.GetMechanism().GetObjects<IUser>(SearchOUIDType.LogOnName, "chenlong3")[0].ID;// "22c3b351-a713-49f2-8f06-6b888a280fff";
            userPresence1.UserID = OguMechanismFactory.GetMechanism().GetObjects<IUser>(SearchOUIDType.LogOnName, "chenlong3")[0].ID;// "22c3b351-a713-49f2-8f06-6b888a280fff";
        }

        protected void btnPostBack_Click(object sender, EventArgs e)
        {
            
        }
	}
}