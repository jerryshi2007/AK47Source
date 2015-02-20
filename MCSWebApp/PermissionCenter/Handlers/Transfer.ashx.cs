using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using MCS.Library.SOA.DataObjects;

namespace PermissionCenter.Handlers
{
    /// <summary>
    /// Transfer 的摘要说明
    /// </summary>
    public class Transfer : IHttpHandler
    {

        public void ProcessRequest(HttpContext context)
        {
            context.Response.Cache.SetCacheability(HttpCacheability.NoCache);
            switch (context.Request.QueryString["type"])
            {
                case "processKey":
                    string url;
                    if (ResourceUriSettings.GetConfig().Paths["workflowDesigner"] != null)
                        url = ResourceUriSettings.GetConfig().Paths["workflowDesigner"].Uri.ToString();
                    else
                        url = "/MCSWebApp/WorkflowDesigner/default.aspx";

                    url += "?processDescKey=" + context.Server.UrlEncode(context.Request.QueryString["id"]);
                    context.Response.Redirect(url, true);
                    break;
                default:
                    break;
            }

        }

        public bool IsReusable
        {
            get
            {
                return false;
            }
        }
    }
}