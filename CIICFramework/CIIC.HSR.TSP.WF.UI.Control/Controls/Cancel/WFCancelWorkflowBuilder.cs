using CIIC.HSR.TSP.WebComponents;
using CIIC.HSR.TSP.WebComponents.Widgets.Button;
using CIIC.HSR.TSP.WF.UI.Control.Controls.Abstract;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Mvc;

namespace CIIC.HSR.TSP.WF.UI.Control.Controls.CancelWorkflow
{
    /// <summary>
    /// 工作流构建器
    /// </summary>
    public class WFCancelWorkflowBuilder : WFButtonControlBuilderlBase<WFCancelWorkflow, WFCancelWorkflowBuilder>
    {
        public WFCancelWorkflowBuilder(HtmlHelper htmlHelper)
            : base(new WFCancelWorkflow(htmlHelper.ViewContext, htmlHelper.ViewData), htmlHelper)
        {
        }
    }
}
