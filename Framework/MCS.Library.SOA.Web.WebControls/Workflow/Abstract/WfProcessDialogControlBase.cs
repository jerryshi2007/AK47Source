using System;
using System.Text;
using System.Collections.Generic;
using MCS.Web.Library;

namespace MCS.Web.WebControls
{
	/// <summary>
	/// ������ضԻ���Ļ���
	/// </summary>
	public abstract class WfProcessDialogControlBase : DialogControlBase<WfProcessDailogControlParams>
	{
		#region Properties
		/// <summary>
		/// ���̵�ID
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
		/// ��ԴID
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
		/// ����resourceID��processID���ɵ����Ի����url��ַ
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
