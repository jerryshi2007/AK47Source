using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MCS.Web.Library;
using MCS.Library.Core;
using MCS.Web.Library.MVC;

namespace MCS.Web.WebControls
{
	[Serializable]
	public class WfRuntimeViewerWrapperControlParams : DialogControlParamsBase
	{
		private string _ProcessID = string.Empty;

		public string ProcessID
		{
			get { return this._ProcessID; }
			set { this._ProcessID = value; }
		}

		/// <summary>
		/// 从url中提取resourceID和processID参数
		/// </summary>
		public override void LoadDataFromQueryString()
		{
			this._ProcessID = WebUtility.GetRequestQueryValue("processID", string.Empty);

			base.LoadDataFromQueryString();
		}

		protected override void BuildRequestParams(StringBuilder strB)
		{
			base.BuildRequestParams(strB);

			string processID = this._ProcessID;

			if (string.IsNullOrEmpty(processID) && WfClientContext.Current.OriginalActivity != null)
				processID = WfClientContext.Current.OriginalActivity.Process.ID;

			AppendNotNullStringParam(strB, "processID", processID);
		}
	}
}
