using CIIC.HSR.TSP.WebComponents;
using CIIC.HSR.TSP.WebComponents.Widgets.Button;
using CIIC.HSR.TSP.WF.UI.Control.Controls.Abstract;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Mvc;

namespace CIIC.HSR.TSP.WF.UI.Control.Controls.PauseWorkflow
{
    /// <summary>
    /// 工作流构建器
    /// </summary>
    /// <summary>
    public class WFPauseWorkflowBuilder : WFButtonControlBuilderlBase<WFPauseWorkflow, WFPauseWorkflowBuilder>
    {
        public WFPauseWorkflowBuilder(HtmlHelper htmlHelper)
            : base(new WFPauseWorkflow(htmlHelper.ViewContext, htmlHelper.ViewData), htmlHelper)
        {
        }
    }
}
