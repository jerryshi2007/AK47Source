using CIIC.HSR.TSP.IoC;
using CIIC.HSR.TSP.WF.UI.Control.Controls;
using CIIC.HSR.TSP.WF.UI.Control.Interfaces;
using MCS.Library.WF.Contracts.Ogu;
using MCS.Library.WF.Contracts.Workflow.Runtime;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Mvc;

namespace CIIC.HSR.TSP.WF.UI.Control.ModelBinding
{
    /// <summary>
    /// 流程启动参数构建器
    /// </summary>
    public class WFStartWorkflowParameterBinder : WFParameterBinderBase<WFStartWorkflowParameter>
    {
        public override void OnBindModel(ControllerContext controllerContext, ModelBindingContext bindingContext, WFStartWorkflowParameter wfParameter)
        {
            wfParameter.ProcessStartupParams = new WfClientProcessStartupParams();
            wfParameter.TransferParameter = new WfClientTransferParams();
            wfParameter.ProcessStartupParams.Creator = wfParameter.RuntimeContext.Operator;
            wfParameter.ProcessStartupParams.AutoCommit = true;
        }
    }
}
