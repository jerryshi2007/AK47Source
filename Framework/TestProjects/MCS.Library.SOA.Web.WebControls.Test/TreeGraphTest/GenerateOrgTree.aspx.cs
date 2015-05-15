using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace MCS.Library.SOA.Web.WebControls.Test.TreeGraphTest
{
    public partial class GenerateOrgTree : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {

        }

        protected void redirectToChart_Click(object sender, EventArgs e)
        {
            Response.Redirect("ResponseOrgTree.ashx");
        }
    }
}