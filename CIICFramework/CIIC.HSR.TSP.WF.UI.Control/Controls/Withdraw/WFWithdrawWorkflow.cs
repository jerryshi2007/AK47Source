using CIIC.HSR.TSP.WebComponents.Widgets.Button;
using CIIC.HSR.TSP.WF.UI.Control.Controls.Abstract;
using CIIC.HSR.TSP.WF.UI.Control.Controls.StartWorkflow;
using CIIC.HSR.TSP.WF.UI.Control.Interfaces;
using MCS.Library.WF.Contracts.Ogu;
using MCS.Library.WF.Contracts.Workflow.Runtime;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Mvc;

namespace CIIC.HSR.TSP.WF.UI.Control.Controls.Withdraw
{
    [WFControlDescription(WFDefaultActionUrl.Withdraw, "$.fn.HSR.Controls.WFWithdrawWorkflow.Click", "撤回")]
    public class WFWithdrawWorkflow : WFButtonControlBase
    {
        public WFWithdrawWorkflow(ViewContext vc, ViewDataDictionary vdd)
            : base(vc, vdd)
        {
        }

        protected override WFParameterWithResponseBase PrepareParameters()
        {
			return new WFWithdrawParameter();
        }

        protected override bool GetEnabled()
        {
            bool result = false;

            WFUIRuntimeContext runtime = runtime = this.ViewContext.HttpContext.
                Request.GetWFContext();

			if (runtime != null && runtime.Process != null && runtime.Process.CurrentActivity != null)
            {
                result = runtime.Process.CanWithdraw;

                if (result)
                {
                    if (runtime.Process.AuthorizationInfo.IsProcessAdmin == false)
                    {
                        result = runtime.Process.CurrentActivity.Descriptor.Properties.GetValue("AllowToBeWithdrawn", true) &&
                           runtime.Process.PreviousActivity.Descriptor.Properties.GetValue("AllowWithdraw", true);

                        if (result)
                        {
                            //不是管理员，进行更严格的权限判断(前一个点的操作人是我)
                            result = runtime.Process.PreviousActivity.Operator.IsNotNullOrEmpty() &&
                                string.Compare(runtime.Process.PreviousActivity.Operator.ID, runtime.CurrentUser.ID, true) == 0;
                        }
                    }
                }
            }

            return result;
        }
    }
}
