using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Mvc;
using CIIC.HSR.TSP.WF.UI.Control.Controls.Abstract;
using CIIC.HSR.TSP.WF.UI.Control.Controls.StartWorkflow;
using CIIC.HSR.TSP.WF.UI.Control.Interfaces;
 

namespace CIIC.HSR.TSP.WF.UI.Control.Controls.UpdateProcess
{
	[WFControlDescription(WFDefaultActionUrl.UpdateProcess, "$.fn.HSR.Controls.WFUpdateProcess.Click", "更新流程参数")]
	public class WFUpdateProcess : WFButtonControlBase
	{
		/// <summary>
		/// 构造方法
		/// </summary>
		/// <param name="vc"></param>
		/// <param name="vdd"></param>
		public WFUpdateProcess(ViewContext vc, ViewDataDictionary vdd)
			: base(vc, vdd)
		{
		} 

        private Dictionary<string, string> _updateElements = new Dictionary<string, string>();

        /// <summary>
        /// 需要更新的客户端容器元素字典，key为容器元素ID，value为PartialView NAME
        /// </summary>
        internal Dictionary<string,string> UpdateElements
        {
            get 
            {
                return _updateElements;
            }
        }

		protected override bool GetEnabled()
		{
          
			bool result = false;

			WFUIRuntimeContext runtime = this.ViewContext.HttpContext.
				Request.GetWFContext();

			if (runtime != null && runtime.Process != null)
				result = runtime.Process.AuthorizationInfo.InMoveToMode;

			return result;
		}

		protected override WFParameterWithResponseBase PrepareParameters()
		{
			WFUpdateProcessParameter param = new WFUpdateProcessParameter();
            param.UpdateElements  = this.UpdateElements;
			return param;
		}

	}
}
