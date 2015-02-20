using CIIC.HSR.TSP.WF.UI.Control.Controls.Abstract;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Mvc;

namespace CIIC.HSR.TSP.WF.UI.Control.Controls.Withdraw
{
    public class WFWithdrawWorkflowBuilder : WFButtonControlBuilderlBase<WFWithdrawWorkflow, WFWithdrawWorkflowBuilder>
    {
        public WFWithdrawWorkflowBuilder(HtmlHelper helper) :
            base(new WFWithdrawWorkflow(helper.ViewContext, helper.ViewData), helper)
        {
        }
    }
}
