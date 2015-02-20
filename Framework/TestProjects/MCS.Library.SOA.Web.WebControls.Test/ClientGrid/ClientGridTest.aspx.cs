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
using MCS.Web.WebControls;

namespace MCS.Library.SOA.Web.WebControls.Test.HBGrid
{
    public partial class HBGridTest : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            WebUtility.RequiredScript(typeof(ClientGrid));
        }
    }
}
