using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using MCS.Library.SOA.DataObjects.Workflow;
using MCS.Web.Library.Script;
using MCS.Library.Core;
using System.Xml.Linq;
using MCS.Library.SOA.DataObjects;
using MCS.Library.Principal;
using System.Transactions;
using MCS.Library.Data;

namespace WorkflowDesigner
{
    public class SaveWorkflow : IHttpHandler
    {
        public void ProcessRequest(HttpContext context)
        {
            string json = context.Request["info"];
            string processID = context.Request["processID"];
            context.Response.ContentType = "text/plain";
            WfConverterHelper.RegisterConverters();
            List<WfProcessDescriptor> deserializedProcessDesp;
            IWfProcess process = null;

            if (!string.IsNullOrEmpty(processID))
            {
                process = WorkflowSettings.GetConfig().GetPersistManager().LoadProcessByProcessID(processID);
            }

            try
            {
                deserializedProcessDesp = JSONSerializerExecute.Deserialize<List<WfProcessDescriptor>>(json);
            }
            catch (Exception ex)
            {
                context.Response.Write("反序列化错误:" + ex.Message + ex.StackTrace);
                return;
            }

            try
            {
                foreach (var descriptor in deserializedProcessDesp)
                {
                    if (process != null && descriptor.Key == process.Descriptor.Key)
                    {
                        process.Descriptor = descriptor;
                        WorkflowSettings.GetConfig().GetPersistManager().SaveProcess(process);
                    }
                    else
                    {
                        WfSaveTemplateExecutor executor = new WfSaveTemplateExecutor(descriptor);

                        executor.Execute();
                    }
                }
            }
            catch (Exception ex)
            {
                context.Response.Write("保存错误:" + ex.Message + ex.StackTrace);
                return;
            }

            context.Response.Write("保存成功!");
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