#undef myDEBUG
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Mvc;

using CIIC.HSR.TSP.WF.UI.Control.Interfaces;
using CIIC.HSR.TSP.WebComponents.Extensions;
using CIIC.HSR.TSP.WF.UI.Control.Controls.Graph;
using MCS.Library.WF.Contracts.Workflow.Runtime;
 

namespace CIIC.HSR.TSP.WF.UI.Control.DefaultActions
{
    /// <summary>
    ///  流程图形导航
    /// </summary>
    public class WFGraphController : Controller
    {

        [HttpPost]
        public ActionResult WFGraphLoad(string processId, WFGraph.GraphOps graphOps)
        {
            try
            {
                WfClientProcessInfo process = null;
#if myDEBUG
                process = new WfClientProcessInfo();
                process.ID = processId;
             
#else
            WFUIRuntimeContext runtime = WFUIRuntimeContext.InitByProcessID(processId);
            process =runtime.Process;
#endif
                string html = WFGraph.GetProcessHtml(process, graphOps);
                return this.JsonSuccess(html);
            }
            catch (System.Exception ex)
            {
                return this.JsonError(ex.Message, ex);
            }                  
        }

        [HttpPost]
        public ActionResult WFGraphLoadBranch(string activityID,WFGraph.GraphOps graphOps, WFGraph.BranchPageOps branchOps)
        {
            try
            {
                if (graphOps.ShowBranchRows > 0)
                {
                    branchOps.PageRows = graphOps.ShowBranchRows;
                }
                string html = WFGraph.GetBranchListHtml(activityID, branchOps);
                return this.JsonSuccess(html);
            }
            catch (System.Exception ex)
            {
                return this.JsonError(ex.Message, ex);
            }
        }
    }
}
