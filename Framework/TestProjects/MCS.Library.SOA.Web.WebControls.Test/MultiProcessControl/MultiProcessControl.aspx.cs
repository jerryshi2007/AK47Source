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

namespace MCS.Library.SOA.Web.WebControls.Test.MultiProcessControl
{
	public partial class MultiProcessControl : System.Web.UI.Page
	{
		protected void Page_Load(object sender, EventArgs e)
		{

		}

		protected void MutifyProcessControl1_ExecuteStep(object data)
		{
			System.Threading.Thread.Sleep(1000);
			
			//throw new Exception("Error Testing");
		}

		protected void MutifyProcessControl1_OnError(Exception ex, object data, ref bool isThrow)
		{
			//isThrow = false;
			string msg = ex.Message;
			msg += data.ToString();
		}
	}
}
