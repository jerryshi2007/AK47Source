using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace MCS.Library.SOA.Web.WebControls.Test.HBRadioButtonList
{
    public partial class Test : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {

        }

        protected void Button1_Click(object sender, EventArgs e)
        {
            var value = this.HBRadioButtonList1.SelectedValue;
            var text = this.HBRadioButtonList1.SelectedText;

            Response.Write("value:" + value + " text:" + text);
        }
    }
}