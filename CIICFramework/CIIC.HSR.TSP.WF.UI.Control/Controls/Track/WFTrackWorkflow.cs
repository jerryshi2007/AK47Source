using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using CIIC.HSR.TSP.WebComponents;
using CIIC.HSR.TSP.WebComponents.Widgets.Button;
using CIIC.HSR.TSP.WF.UI.Control.Controls.Abstract;
using CIIC.HSR.TSP.WF.UI.Control.Controls.StartWorkflow;
using CIIC.HSR.TSP.WF.UI.Control.Interfaces;
using MCS.Library.WF.Contracts.Workflow.Descriptors;
using MCS.Library.WF.Contracts.Workflow.Runtime;
using Newtonsoft.Json;
using CIIC.HSR.TSP.WF.Bizlet.Impl;
namespace CIIC.HSR.TSP.WF.UI.Control.Controls.FollowWorkFlow
{
	/// <summary>
	///跟踪工作流控件  
	/// </summary>
	[WFControlDescription("", "$.fn.HSR.Controls.WFTrackWorkflow.Click", "跟踪")]
	public class WFTrackWorkflow : WFButtonControlBase
	{
		/// <summary>
		/// 构造方法
		/// </summary>
		/// <param name="vc"></param>
		/// <param name="vdd"></param>
		public WFTrackWorkflow(ViewContext vc, ViewDataDictionary vdd)
			: base(vc, vdd)
		{

		}
		protected override WFParameterWithResponseBase PrepareParameters()
		{
			WFTrackParameter param = new WFTrackParameter();
			WFUIRuntimeContext runtime = runtime = this.ViewContext.HttpContext.
			   Request.GetWFContext();

			if (runtime != null && runtime.Process != null)
			{

                //获取配置Url的地址
                string trackProcessWindowUrl = PageUrlHelper.GetUrl("wfRuntimeViewerlinkPageUrl");
				string processId = runtime.Process.ID;
				string resourceId = runtime.Process.ResourceID;

				trackProcessWindowUrl = trackProcessWindowUrl + "?processID=" + processId + "&resourceID=" + resourceId;
				param.WindowUrl = trackProcessWindowUrl;
			}

			return param;
		}

		protected override bool GetEnabled()
		{
			WFUIRuntimeContext runtime = runtime = this.ViewContext.HttpContext.
				Request.GetWFContext();

			return (runtime != null && runtime.Process != null);
		}

	}
}
