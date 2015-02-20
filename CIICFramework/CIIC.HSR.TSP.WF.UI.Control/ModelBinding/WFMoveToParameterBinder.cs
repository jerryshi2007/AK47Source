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
    /// 流转参数绑定
    /// </summary>
    public class WFMoveToParameterBinder : WFParameterBinderBase<WFMoveToParameter>
    {
        public override void OnBindModel(ControllerContext controllerContext, ModelBindingContext bindingContext, WFMoveToParameter wfParameter)
        {
            wfParameter.TransferParameter = new WfClientTransferParams();
            wfParameter.TransferParameter.Operator = wfParameter.RuntimeContext.Operator;
        }
    }
}
