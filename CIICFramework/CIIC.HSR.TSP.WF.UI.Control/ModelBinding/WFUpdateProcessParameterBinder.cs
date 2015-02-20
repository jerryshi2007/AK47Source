using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CIIC.HSR.TSP.WF.UI.Control.Interfaces;
using System.Web.Mvc;
 

namespace CIIC.HSR.TSP.WF.UI.Control.ModelBinding
{
	public class WFUpdateProcessParameterBinder : WFParameterBinderBase<WFUpdateProcessParameter>
	{
        public override void OnBindModel(ControllerContext controllerContext, ModelBindingContext bindingContext, WFUpdateProcessParameter wfParameter)
        {
            base.OnBindModel(controllerContext, bindingContext, wfParameter);
           
            wfParameter.InitExecuteParams(controllerContext.Controller as Controller);
        }
	}
}
