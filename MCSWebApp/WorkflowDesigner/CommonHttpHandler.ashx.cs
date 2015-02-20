using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using MCS.Library.Expression;
using MCS.Library.SOA.DataObjects.Workflow;

namespace WorkflowDesigner
{
    /// <summary>
    /// 用于ajax请求的handler
    /// </summary>
    public class CommonHttpHandler : IHttpHandler
    {
        public void ProcessRequest(HttpContext context)
        {
            context.Response.ContentType = "text/plain";

            try
            {
                string action = context.Request.Params["Action"];

                //if (context.Request["Action"] == "GetProcName")
                if (string.Compare(action, "GetProcName") == 0)
                {
                    GetProcessName(context);
                }
                else
                {
                    ExpressionParser.Parse(context.Request["express"]);
                    RespondResult(context, true, "");
                }
            }
            catch (Exception ex)
            {
                context.Response.Clear();
                RespondResult(context, false, ex.Message.Replace("\"", ""));
            }
        }

        private void GetProcessName(HttpContext context)
        {
            if (string.IsNullOrEmpty(context.Request.Params["ProcKey"]))
            {
                RespondResult(context, false, "ProcKey不能为空!");
                return;
            }

            string procKey = context.Request["ProcKey"];
            WfProcessDescriptorInfoCollection processDescInfos =
                WfProcessDescriptorInfoAdapter.Instance.LoadWfProcessDescriptionInfos(builder => builder.AppendItem("PROCESS_KEY", procKey), true);

            if (processDescInfos.Count == 0)
            {
                RespondResult(context, false, string.Format("不能找到KEY为{0}的流程模板!", procKey));
                return;
            }
            RespondResult(context, true, processDescInfos[0].ProcessName);
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