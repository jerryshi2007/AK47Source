using System;
using System.Text;
using System.Collections.Generic;
using MCS.Web.Library;

namespace MCS.Web.WebControls
{
	/// <summary>
	/// 流程相关对话框的基类
	/// </summary>
	public abstract class WfProcessDialogControlBase : DialogControlBase<WfProcessDailogControlParams>
	{
		#region Properties
		/// <summary>
		/// 流程的ID
		/// </summary>
		public string ProcessID
		{
			get
			{
				return ControlParams.ProcessID;
			}
			set
			{
				ControlParams.ProcessID = value;
			}
		}

		/// <summary>
		/// 资源ID
		/// </summary>
		public string ResourceID
		{
			get
			{
				return ControlParams.ResourceID;
			}
			set
			{
				ControlParams.ResourceID = value;
			}
		}
		#endregion

		#region Protected
		/// <summary>
		/// 根据resourceID和processID生成弹出对话框的url地址
		/// </summary>
		/// <returns></returns>
		protected override string GetDialogUrl()
		{
			PageRenderMode pageRenderMode = new PageRenderMode(this.UniqueID, "DialogControl");

			string url = WebUtility.GetRequestExecutionUrl(pageRenderMode, "resourceID", "activityID", "processID");

			return url + "&" + this.ControlParams.ToRequestParams();
		}
		#endregion Protected
	}
}
