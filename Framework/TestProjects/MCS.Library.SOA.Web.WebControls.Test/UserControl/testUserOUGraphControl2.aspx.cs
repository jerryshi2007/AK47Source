using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using MCS.Web.WebControls;
using MCS.Library.OGUPermission;

namespace MCS.Library.SOA.Web.WebControls.Test.UserControl
{
    public partial class testUserOUGraphControl2 : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {

        }

        protected void userSelector_LoadingObjectToTreeNode(UserOUGraphControl treeControl, IOguObject oguObj, DeluxeTreeNode newTreeNode, ref bool cancel)
        {
            newTreeNode.ShowCheckBox = true;
            newTreeNode.Checked = true;
        }
    }
}