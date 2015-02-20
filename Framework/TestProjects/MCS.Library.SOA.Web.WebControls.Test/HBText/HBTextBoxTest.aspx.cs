using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace MCS.Library.SOA.Web.WebControls.Test.HBText
{
    public partial class HBTextBoxTest : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            //this.tbx1.ReadOnly = true;
            if (!IsPostBack)
            {
                this.tbx1.Text = "3434343434343";
            }
        }

        protected void Button1_Click(object sender, EventArgs e)
        {
            this.div_msg.InnerText = "HBTextBox.Text：" + this.tbx1.Text;
        }
    }
}