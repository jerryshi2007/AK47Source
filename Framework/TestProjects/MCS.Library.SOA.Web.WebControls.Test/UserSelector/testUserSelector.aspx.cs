using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Text;
using MCS.Library.OGUPermission;

namespace MCS.Library.SOA.Web.WebControls.Test.UserSelector
{
    public partial class testUserSelector1 : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            UserSelector_Not_MultiSelect.Root = (IOrganization)OguMechanismFactory.GetMechanism().GetRoot().Children[0];
        }
    }
}