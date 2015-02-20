using CIIC.HSR.TSP.WebComponents;
using CIIC.HSR.TSP.WebComponents.Widgets.Button;
using CIIC.HSR.TSP.WF.UI.Control.Controls.Abstract;
using CIIC.HSR.TSP.WF.UI.Control.Controls.StartWorkflow;
using CIIC.HSR.TSP.WF.UI.Control.Interfaces;
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

namespace CIIC.HSR.TSP.WF.UI.Control.Controls.RestoreWorkflow
{
    /// <summary>
    /// 恢复工作流控件
    /// </summary>
    [WFControlDescription(WFDefaultActionUrl.Cancel, "$.fn.HSR.Controls.WFRestoreWorkflow.Click", "还原", "您确定要还原流程吗？")]
    public class WFRestoreWorkflow : WFButtonControlBase
    {
        /// <summary>
        /// 构造方法
        /// </summary>
        /// <param name="vc"></param>
        /// <param name="vdd"></param>
        public WFRestoreWorkflow(ViewContext vc, ViewDataDictionary vdd)
            : base(vc, vdd)
        {

        }

        protected override WFParameterWithResponseBase PrepareParameters()
        {
			return new WFRestoreParameter();
        }

        protected override bool GetEnabled()
        {
            bool result = false;

            WFUIRuntimeContext runtime = this.ViewContext.HttpContext.
                Request.GetWFContext();

            if (runtime != null)
            {
                result = (runtime.Process.Status == WfClientProcessStatus.Aborted);

                if (result)
                {
                    //当前环节允许的话，要看是否是待办人或者流程已经办结
                    result = ((runtime.Process.AuthorizationInfo.InMoveToMode || runtime.Process.AuthorizationInfo.IsProcessAdmin));
                }
            }

            return result;
        }
    }
}
