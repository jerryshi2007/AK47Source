using CIIC.HSR.TSP.WF.UI.Control.ModelBinding;
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
    /// 默认流转参数
    /// </summary>
    [ModelBinder(typeof(WFMoveToDefaultParameterBinder))]
    public class WFMoveToDefaultParameter : WFParameterBase, IWFOperation<ResponseData>
    {
        /// <summary>
        /// 流转到默认节点
        /// </summary>
        /// <returns>流程实例</returns>
        public ResponseData Execute()
        {
            throw new NotImplementedException();
        }
    }
}
