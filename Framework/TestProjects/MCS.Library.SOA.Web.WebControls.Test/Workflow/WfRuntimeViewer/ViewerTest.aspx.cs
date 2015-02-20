using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using MCS.Library.SOA.DataObjects.Workflow.DTO;
using MCS.Library.SOA.DataObjects.Workflow;
using MCS.Web.WebControls.Configuration;
using System.Threading;
using System.Globalization;

namespace MCS.Library.SOA.Web.WebControls.Test.Workflow.WfRuntimeViewer
{
    public partial class ViewerTest : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            Thread.CurrentThread.CurrentUICulture = CultureInfo.GetCultureInfo("en-US");
            var desc = LoadProcessDescriptor("cb80a469-82bd-8061-42ce-681f775570ef");
            AddNodeToDescription(desc, 3);
            var node3Desc = desc.Activities[3].Descriptor;
            WfTransferParams param = new WfTransferParams(node3Desc);
            desc.MoveTo(param);
            var info = WorkflowInfo.ProcessAdapter(desc);

            this.viewer1.InitializeValue = info;

            desc = LoadProcessDescriptor("cb80a469-82bd-8061-42ce-681f775570ef");
            AddNodeToDescription(desc, 5);
            node3Desc = desc.Activities[3].Descriptor;
            param = new WfTransferParams(node3Desc);
            desc.MoveTo(param);
            info = WorkflowInfo.ProcessAdapter(desc);
            viewer2.InitializeValue = info;
            //this.viewer1.BranchProcessListUrl = "./ModalDialog/WfBranchProcessList.aspx";
        }

        public static IWfProcess LoadProcessDescriptor(string processid)
        {
            WfRuntime.ClearCache();
            IWfProcess process = WfRuntime.GetProcessByProcessID(processid);

            //string xml = File.ReadAllText(@"C:\Documents and Settings\issuser\Desktop\111.txt");

            //Type descType = process.Descriptor.GetType();
            //PropertyInfo graphProp = descType.GetProperty("GraphDescription");
            //graphProp.SetValue(process.Descriptor, xml, null);

            //WorkflowSettings.GetConfig().GetPersistManager().SaveProcess(process);
            //WfRuntime.PersistWorkflows();
            return process;
        }

        public static void AddNodeToDescription(IWfProcess process, int addNumber)
        {
            WfNormalActivityBuilder builder = new WfNormalActivityBuilder();
            var desc = process.Descriptor;
            for (var i = 0; i < addNumber; i++)
            {
                WfActivityDescriptor normalAct = new WfActivityDescriptor("Extra" + i, WfActivityType.NormalActivity);
                normalAct.Name = "Normal" + i;
                normalAct.CodeName = "Normal Activity" + i;
                normalAct.ActivityType = WfActivityType.NormalActivity;
                normalAct.BranchProcessTemplates.Add(new WfBranchProcessTemplateDescriptor("no delete tks!"));
                desc.Activities.Add(normalAct);

                desc.Activities[1].ToTransitions.AddForwardTransition(normalAct);
                normalAct.ToTransitions.AddForwardTransition(desc.Activities[2]);

                WfActivityBase act = WfActivitySettings.GetConfig().GetActivityBuilder(normalAct).CreateActivity(normalAct);
                act.Process = process;
                process.Activities.Add(act);
            }
        }
    }
}