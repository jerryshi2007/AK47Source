using CIIC.HSR.TSP.WebComponents;
using CIIC.HSR.TSP.WebComponents.Widgets.Button;
using CIIC.HSR.TSP.WF.UI.Control.Controls.Abstract;
using CIIC.HSR.TSP.WF.UI.Control.Controls.StartWorkflow;
using CIIC.HSR.TSP.WF.UI.Control.Interfaces;
using MCS.Library.Globalization;
using MCS.Library.WF.Contracts.Workflow.Descriptors;
using MCS.Library.WF.Contracts.Workflow.Runtime;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;

namespace CIIC.HSR.TSP.WF.UI.Control.Controls.CancelWorkflow
{
    /// <summary>
    /// 作废工作流控件
    /// </summary>
    [WFControlDescription(WFDefaultActionUrl.Cancel, "$.fn.HSR.Controls.WFCancelWorkflow.Click", "作废")]
    public class WFCancelWorkflow : WFButtonControlBase
    {
        /// <summary>
        /// 构造方法
        /// </summary>
        /// <param name="vc"></param>
        /// <param name="vdd"></param>
        public WFCancelWorkflow(ViewContext vc, ViewDataDictionary vdd)
            : base(vc, vdd)
        {

        }

        protected override WFParameterWithResponseBase PrepareParameters()
        {
            return new WFCancelParameter();
        }

        protected override bool GetEnabled()
        {
            bool result = false;

            WFUIRuntimeContext runtime = this.ViewContext.HttpContext.
                Request.GetWFContext();

            if (runtime != null && runtime.Process != null)
            {
                result = (runtime.Process.Status == WfClientProcessStatus.Running ||
                            runtime.Process.Status == WfClientProcessStatus.Completed ||
                            runtime.Process.Status == WfClientProcessStatus.Paused ||
                            runtime.Process.Status == WfClientProcessStatus.Maintaining);

                if (result)
                {
                    //当前环节允许的话，要看是否是待办人或者流程已经办结
                    result = ((runtime.Process.AuthorizationInfo.InMoveToMode || runtime.Process.Status == WfClientProcessStatus.Completed) &&
                        runtime.Process.CurrentActivity.Descriptor.Properties.GetValue("AllowAbortProcess", true));

                    if (result == false)
                        result = runtime.Process.AuthorizationInfo.IsProcessAdmin;
                }
            }

            return result;
        }

        protected override void InitButtonAttributes(Button button)
        {
            WFUIRuntimeContext runtime = runtime = this.ViewContext.HttpContext.
                Request.GetWFContext();

            if (runtime != null && runtime.Process != null && runtime.Process.CurrentActivity != null)
            {
                if (string.IsNullOrEmpty(button.Text))
                    button.Text = Translator.Translate(CultureDefine.DefaultCulture,
                        runtime.Process.CurrentActivity.Descriptor.Properties.GetValue("AbortButtonName", "作废"));
            }

            base.InitButtonAttributes(button);
        }
    }
}
