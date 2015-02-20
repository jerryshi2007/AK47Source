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
namespace CIIC.HSR.TSP.WF.UI.Control.Controls.PauseWorkflow
{
	/// <summary>
	/// 暂停工作流控件
	/// </summary>
    [WFControlDescription(WFDefaultActionUrl.Pause, "$.fn.HSR.Controls.WFPauseWorkflow.Click", "暂停", "您确定要暂停流程吗？")]
	public class WFPauseWorkflow : WFButtonControlBase
	{
		/// <summary>
		/// 构造方法
		/// </summary>
		/// <param name="vc"></param>
		/// <param name="vdd"></param>
		public WFPauseWorkflow(ViewContext vc, ViewDataDictionary vdd)
			: base(vc, vdd)
		{

		}

		protected override WFParameterWithResponseBase PrepareParameters()
		{
			return new WFPauseParameter();
		}

		protected override bool GetEnabled()
		{
			bool result = false;

			WFUIRuntimeContext runtime = this.ViewContext.HttpContext.
				Request.GetWFContext();

			if (runtime != null && runtime.Process != null)
			{
				result = (runtime.Process.Status == WfClientProcessStatus.Running && runtime.Process.AuthorizationInfo.IsProcessAdmin);
			}

			return result;
		}
	}
}
