using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace MCS.OA.CommonPages.AsyncPersist
{
	public partial class Default : System.Web.UI.Page
	{
		public override void ProcessRequest(HttpContext context)
		{
			Server.TransferRequest("~/AsyncPersist/Monitor.aspx", false);
		}
	}
}