using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using MCS.Library.OGUPermission;
using MCS.Web.WebControls;

namespace MCS.OA.CommonPages.UserInfoExtend
{
    public partial class Left : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {

        }

        protected void LoadObectjToTreeNode(UserOUGraphControl treeControl, IOguObject oguObj, DeluxeTreeNode newTreeNode, ref bool cancel)
        {
            if (oguObj.ObjectType == SchemaType.Groups)
                cancel = true;
        }
    }
}