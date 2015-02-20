using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using MCS.Library.SOA.DataObjects;
using MCS.Web.WebControls;

namespace MCS.Library.SOA.Web.WebControls.Test
{
    public partial class CloneWithDialogUploadFileControl : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {

        }

        protected void Button1_Click(object sender, EventArgs e)
        {
            var data = MaterialControl.GetCommonDeltaMaterials();
        }
       
    }
}