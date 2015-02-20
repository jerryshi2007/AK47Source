using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Mvc;

namespace CIIC.HSR.TSP.WF.UI.Control.ModelBinding
{
    /// <summary>
    /// 默认流转参数
    /// </summary>
    public class WFMoveToDefaultParameterBinder : IModelBinder
    {
        /// <summary>
        /// 构建默认流转参数对象
        /// </summary>
        /// <param name="controllerContext"></param>
        /// <param name="bindingContext"></param>
        /// <returns>默认流转参数对象</returns>
        public object BindModel(ControllerContext controllerContext, ModelBindingContext bindingContext)
        {
            throw new NotImplementedException();
        }
    }
}
