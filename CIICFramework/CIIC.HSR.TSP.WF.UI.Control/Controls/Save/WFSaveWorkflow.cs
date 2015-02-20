using CIIC.HSR.TSP.WebComponents;
using CIIC.HSR.TSP.WebComponents.Widgets.Button;
using CIIC.HSR.TSP.WF.UI.Control.Controls.Abstract;
using CIIC.HSR.TSP.WF.UI.Control.Controls.StartWorkflow;
using CIIC.HSR.TSP.WF.UI.Control.Interfaces;
using MCS.Library.WF.Contracts.Workflow.Descriptors;
using MCS.Library.WF.Contracts.Workflow.Runtime;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;

namespace CIIC.HSR.TSP.WF.UI.Control.Controls.SaveWorkflow
{
	/// <summary>
	/// 保存工作流控件
	/// </summary>
    [WFControlDescription(WFDefaultActionUrl.Save, "$.fn.HSR.Controls.WFSaveWorkflow.Click", "保存", "您确定要保存吗？")]
	public class WFSaveWorkflow : WFButtonControlBase
	{
		/// <summary>
		/// 构造方法
		/// </summary>
		/// <param name="vc"></param>
		/// <param name="vdd"></param>
		public WFSaveWorkflow(ViewContext vc, ViewDataDictionary vdd)
			: base(vc, vdd)
		{

		}

		protected override WFParameterWithResponseBase PrepareParameters()
		{
			return new WFSaveParameter();
		}

		protected override bool GetEnabled()
		{
			bool result = false;

			WFUIRuntimeContext runtime = this.ViewContext.HttpContext.
				Request.GetWFContext();

			if (runtime != null && runtime.Process != null)
				result = (runtime.Process.AuthorizationInfo.InMoveToMode &&
					   runtime.Process.CurrentActivity.Descriptor.Properties.GetValue("AllowSave", true));

			return result;
		}
	}
}
