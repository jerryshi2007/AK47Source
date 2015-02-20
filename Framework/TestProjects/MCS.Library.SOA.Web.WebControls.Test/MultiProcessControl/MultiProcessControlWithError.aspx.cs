using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace MCS.Library.SOA.Web.WebControls.Test.MultiProcessControl
{
	public partial class MultiProcessControlWithError : System.Web.UI.Page
	{
		protected void Page_Load(object sender, EventArgs e)
		{

		}

		protected void MutifyProcessControl1_ExecuteStep(object data)
		{
			System.Threading.Thread.Sleep(500);

			Random rnd = new Random((int)DateTime.Now.Ticks);

			if (rnd.Next(2) > 0)
				throw new Exception("Error Testing");
		}

		protected void MutifyProcessControl1_OnError(Exception ex, object data, ref bool isThrow)
		{
			isThrow = false;
		}
	}
}