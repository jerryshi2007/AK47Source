using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace MCS.Web.Responsive.WebControls.Test.HBDropDownList
{
    public partial class HBDropDownListTest : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {

        }

        protected void Button1_Click(object sender, EventArgs e)
        {
            var value = HBDropDownList1.SelectedValue;
            var value1 = HBDropDownList1.SelectedItem.Value;
            var text = HBDropDownList1.SelectedText;
            var text1 = HBDropDownList1.SelectedItem.Text;

            Response.Write(value + "," + value1 + "," + text + "," + text1);
        }
    }
}