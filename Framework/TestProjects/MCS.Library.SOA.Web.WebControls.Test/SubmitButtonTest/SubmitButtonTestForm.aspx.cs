using System;
using System.Web;
using System.Data;
using System.Web.UI;
using System.Threading;
using System.Collections;
using System.Web.Security;
using System.Configuration;
using System.Web.UI.WebControls;
using System.Web.UI.HtmlControls;
using System.Web.UI.WebControls.WebParts;

namespace MCS.Library.SOA.Web.WebControls.Test.SubmitButton
{
	public partial class SubmitButtonTestForm : System.Web.UI.Page
	{
		protected void Page_Load(object sender, EventArgs e)
		{

		}

		protected void submitBtn_Click(object sender, EventArgs e)
		{
			Thread.Sleep(5000);
		}

        protected void SubmitButton2_Click(object sender, EventArgs e)
        {
            Thread.Sleep(2000);
            tbx_msg.Text = "²éÑ¯½á¹û";
            this.SubmitButton2.PopupCaption = "";
        }

		protected void SubmitButton1_Click(object sender, EventArgs e)
		{
			Thread.Sleep(10000);
		}
	}
}
