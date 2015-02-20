using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using MCS.Library.Expression;

namespace WeChatManage
{
    /// <summary>
    /// Summary description for CommonHandler
    /// </summary>
    public class CommonHandler : IHttpHandler
    {

        public void ProcessRequest(HttpContext context)
        {
            context.Response.ContentType = "text/plain";

            try
            {
                ExpressionParser.Parse(context.Request["express"]);
                RespondResult(context, true, "");
            }
            catch (Exception ex)
            {
                context.Response.Clear();
                RespondResult(context, false, ex.Message.Replace("\"", ""));
            }
        }

        private void RespondResult(HttpContext context, bool success, string message)
        {
            context.Response.Write("{\"Success\":\"" + success.ToString().ToLower() + "\",\"Message\":\"" + message + "\"}");
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