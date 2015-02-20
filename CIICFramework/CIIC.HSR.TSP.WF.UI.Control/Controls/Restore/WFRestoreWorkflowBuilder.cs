using CIIC.HSR.TSP.WebComponents;
using CIIC.HSR.TSP.WebComponents.Widgets.Button;
using CIIC.HSR.TSP.WF.UI.Control.Controls.Abstract;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Mvc;

namespace CIIC.HSR.TSP.WF.UI.Control.Controls.RestoreWorkflow
{
    /// <summary>
    /// 工作流构建器
    /// </summary>
    public class WFRestoreWorkflowBuilder : WFButtonControlBuilderlBase<WFRestoreWorkflow, WFRestoreWorkflowBuilder>
    {
        public WFRestoreWorkflowBuilder(HtmlHelper htmlHelper)
            : base(new WFRestoreWorkflow(htmlHelper.ViewContext, htmlHelper.ViewData), htmlHelper)
        {
        }
    }
}
