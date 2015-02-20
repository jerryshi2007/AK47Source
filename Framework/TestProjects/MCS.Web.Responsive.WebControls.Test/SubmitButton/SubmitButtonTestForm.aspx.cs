using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Threading;

namespace MCS.Web.Responsive.WebControls.Test.SubmitButton
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
			tbx_msg.Text = "查询结果";
			this.SubmitButton2.PopupCaption = "";
		}

		protected void SubmitButton1_Click(object sender, EventArgs e)
		{
			Thread.Sleep(10000);
		}
	}
}