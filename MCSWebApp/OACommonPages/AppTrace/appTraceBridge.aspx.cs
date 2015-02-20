using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using MCS.Library.SOA.DataObjects.Workflow;
using MCS.Library.SOA.DataObjects;

namespace MCS.OA.CommonPages.AppTrace
{
    public partial class appTraceBridge : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            string resourceID = Request.QueryString["resourceID"];

            WfProcessCurrentInfoCollection infos =
                WfProcessCurrentInfoAdapter.Instance.Load(false,
                builder => builder.AppendItem("RESOURCE_ID", resourceID));

            string newUrl = "";
            string signStr = "";
            string keyName = "";
            if (infos.Count > 0)
            {
                keyName = "appTrace";
            }
            else
            {
                keyName = "oldAppTrace";
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