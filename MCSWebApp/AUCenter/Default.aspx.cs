using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using AU = MCS.Library.SOA.DataObjects.Security.AUObjects;
using PC = MCS.Library.SOA.DataObjects.Security;

namespace AUCenter
{
	public partial class Default : System.Web.UI.Page
	{
		protected override void OnPreRender(EventArgs e)
		{
			base.OnPreRender(e);
			this.logItems.DataSource = AU.Logs.AUOperationLogAdapter.Instance.LoadRecentSummaryLog(5);
			this.logItems.DataBind();
		}
	}
}