using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using MCS.Library.SOA.DataObjects;
using MCS.Library.SOA.DataObjects.Workflow;
using MCS.Web.Library.MVC;

namespace MCS.OA.CommonPages.UserOperationLog
{
	public partial class logBridge : System.Web.UI.Page
	{
		protected void Page_Load(object sender, EventArgs e)
		{
			string resourceID = Request.QueryString["resourceID"];

			WfRuntime.ProcessContext.EnableSimulation = WfClientContext.SimulationEnabled;

			WfProcessCurrentInfoCollection infos = WfProcessCurrentInfoAdapter.Instance.Load(false,
				builder => builder.AppendItem("RESOURCE_ID", resourceID));

			string newUrl = string.Empty;
			string signStr = string.Empty;
			string keyName = string.Empty;

			if (infos.Count > 0)
			{
				keyName = "appLogView";
			}
			else
			{
				keyName = "oldappLogView";
			}
			if (ResourceUriSettings.GetConfig().Paths[keyName].Uri.ToString().IndexOf('?') != -1)
			{
				signStr = "{0}&{1}";
			}
			else
			{
				signStr = "{0}?{1}";
			}

			newUrl = string.Format(signStr
				 , ResourceUriSettings.GetConfig().Paths[keyName].Uri.ToString()
				 , Request.QueryString.ToString());

			Response.Redirect(newUrl);
		}
	}
}