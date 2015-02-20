using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using MCS.Library.SOA.DataObjects;
using MCS.Library.Principal;
using MCS.Web.Library.Script;

namespace MCSResponsiveOAPortal.Services
{
    /// <summary>
    /// JsonServices 的摘要说明
    /// </summary>
    public class JsonServices : IHttpHandler
    {
        delegate void ProcessMethodHandler(HttpContext context);

        public void ProcessRequest(HttpContext context)
        {
            ProcessMethodHandler handler;

            if (IsAcceptJson(context.Request.AcceptTypes) && GetMethod(context.Request, out handler))
            {
                handler(context);
            }
            else
            {
                context.Response.StatusCode = 400;
            }
        }

        private bool IsAcceptJson(string[] types)
        {
            bool result = false;
            for (int i = types.Length - 1; i >= 0; i--)
            {
                var type = types[i];
                if (type == "*/*" || type == "json" || type == "text/javascript")
                {
                    result = true;
                    break;
                }
                else
                {
                    int p = type.IndexOf('/');
                    if (p >= 0 && p < type.Length)
                    {
                        if (string.Compare(type, p + 1, "json", 0, 4, StringComparison.CurrentCultureIgnoreCase) == 0)
                        {
                            result = true;
                            break;
                        }
                    }
                }
            }

            return result;
        }

        private bool GetMethod(HttpRequest httpRequest, out ProcessMethodHandler method)
        {
            string m = httpRequest.Params["method"];
            switch (m)
            {
                case "GetTaskStat":
                    method = new ProcessMethodHandler(this.GetTaskStat);
                    break;
                default:
                    method = null;
                    break;
            }

            return method != null;
        }

        public void GetTaskStat(HttpContext context)
        {
            object[] result = TaskStat.GetUserTaskStatusData();
            
            ResponseJson(context, JSONSerializerExecute.Serialize(result));

        }

        private static void ResponseJson(HttpContext context, string returnJson)
        {
            context.Response.ContentType = "application/json";
            context.Response.Output.Write(returnJson);
        }

        public bool IsReusable
        {
            get
            {
                return true;
            }
        }
    }
}