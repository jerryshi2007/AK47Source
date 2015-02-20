using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace MCS.Library.SOA.Web.WebControls.Test.SubmitButton
{
	public partial class SubmitButtonInUpdatePanelMode : System.Web.UI.Page
	{
		protected void Page_Load(object sender, EventArgs e)
		{

		}

		protected void submitBtn_Click(object sender, EventArgs e)
		{
			Thread.Sleep(2000);
		}
	}
}