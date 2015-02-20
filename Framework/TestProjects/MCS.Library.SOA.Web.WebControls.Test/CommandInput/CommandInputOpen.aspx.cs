using System;
using System.Data;
using System.Configuration;
using System.Collections;
using System.Web;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;
using System.Web.UI.HtmlControls;
using MCS.Web.Library;

namespace MCS.SOA.Web.WebControls.Test.CommandInput
{
    public partial class CommandInputOpen : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {

        }

        protected void Button1_Click(object sender, EventArgs e)
        {
            WebUtility.RefreshParentWindow();
        }

        protected void Button2_Click(object sender, EventArgs e)
        {
            WebUtility.CloseWindow();
        }
    }
}
