using CIIC.HSR.TSP.WF.Bizlet.Common;
using CIIC.HSR.TSP.WF.Bizlet.Impl;
using CIIC.HSR.TSP.WF.UI.Control.Controls;
using CIIC.HSR.TSP.WF.UI.Control.Interfaces;
using MCS.Library.WF.Contracts.Json.Converters;
using MCS.Library.WF.Contracts.Workflow.Runtime;
using MCS.Web.Library.Script;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;

namespace CIIC.HSR.TSP.WF.UI.Control.ModelBinding
{
	public abstract class WFParameterBinderBase<T> : IModelBinder where T : WFParameterBase
	{
		public object BindModel(ControllerContext controllerContext, ModelBindingContext bindingContext)
		{
			WfClientJsonConverterHelper.Instance.RegisterConverters();

			HttpRequestBase request = controllerContext.HttpContext.Request;

			string param = request.Form[Consts.WFParas];

			param = HttpUtility.UrlDecode(param);
			
			T wfParameter = JSONSerializerExecute.Deserialize<T>(param);

			wfParameter.RuntimeContext = new WfClientRuntimeContext();

			WFUIRuntimeContext uiContext = controllerContext.HttpContext.Request.GetWFContext();

			wfParameter.RuntimeContext.Operator = uiContext.CurrentUser;
			wfParameter.TenantCode = uiContext.TenantCode;

            if (null == uiContext.Process || !uiContext.Process.ApplicationRuntimeParameters.ContainsKey(Consts.EmailCollector))
            {
                wfParameter.EMailCollector = new MailCollector();
            }
            else
            { 
                string emailArgs=uiContext.Process.ApplicationRuntimeParameters[Consts.EmailCollector].ToString();
                if (!string.IsNullOrEmpty(emailArgs))
                {
                    wfParameter.EMailCollector = JsonConvert.DeserializeObject<MailCollector>(emailArgs);
                }
                else
                {
                    wfParameter.EMailCollector = new MailCollector();
                }
            }
            
			OnBindModel(controllerContext, bindingContext, wfParameter);

			return wfParameter;
		}

		public virtual void OnBindModel(ControllerContext controllerContext, ModelBindingContext bindingContext, T wfParameter)
		{
		}
	}
}
