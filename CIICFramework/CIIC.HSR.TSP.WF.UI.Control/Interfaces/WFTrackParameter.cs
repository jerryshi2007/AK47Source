using CIIC.HSR.TSP.WF.UI.Control.Controls;
using CIIC.HSR.TSP.WF.UI.Control.ModelBinding;
using MCS.Library.WF.Contracts.Ogu;
using MCS.Library.WF.Contracts.Proxies;
using MCS.Library.WF.Contracts.Workflow.Runtime;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Mvc;

namespace CIIC.HSR.TSP.WF.UI.Control.Interfaces
{
    /// <summary>
    /// 流转参数上下文
    /// </summary>
    [ModelBinder(typeof(WFTrackParameterBinder))]
    public class WFTrackParameter : WFParameterWithResponseBase
    {
        /// <summary>
        /// 窗口Url
        /// </summary>
        public string WindowUrl { get; set; }

        protected override void InternalExecute(ResponseData response)
        {
        }
    }
}
