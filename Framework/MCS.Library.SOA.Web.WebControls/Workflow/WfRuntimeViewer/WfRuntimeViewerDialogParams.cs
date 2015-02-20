using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MCS.Web.Library;

namespace MCS.Web.WebControls
{
	[Serializable]
	public class WfRuntimeViewerParams : DialogControlParamsBase
	{
		private string _ActivityID = string.Empty;

		public string ActivityID
		{
			get { return this._ActivityID; }
			set { this._ActivityID = value; }
		}

		/// <summary>
		/// 从url中提取resourceID和processID参数
		/// </summary>
		public override void LoadDataFromQueryString()
		{
			this._ActivityID = WebUtility.GetRequestQueryValue("owner_activityid", string.Empty);

			base.LoadDataFromQueryString();
		}
	}
}
