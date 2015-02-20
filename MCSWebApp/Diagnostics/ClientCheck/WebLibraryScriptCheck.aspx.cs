using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using MCS.Web.Library;
using MCS.Web.Library.Script;
using MCS.Web.Library.Resources;

namespace Diagnostics.ClientCheck
{
	public partial class WebLibraryScriptCheck : System.Web.UI.Page
	{
		protected void Page_Load(object sender, EventArgs e)
		{
			WebUtility.RequiredScript(typeof(ControlBaseScript));
			WebUtility.RequiredScript(typeof(ClientMsgResources));
		}
	}
}