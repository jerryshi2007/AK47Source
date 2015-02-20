using System;
using System.Text;
using System.Collections.Generic;
using MCS.Library;
using MCS.Web.Library;

namespace MCS.Web.WebControls
{
	[Serializable]
	public class MultiProcessControlParam : DialogControlParamsBase
	{
		/// <summary>
		/// 是否显示步骤的错误
		/// </summary>
		public bool ShowStepErrors
		{
			get;
			set;
		}

		public override void LoadDataFromQueryString()
		{
			base.LoadDataFromQueryString();

			this.ShowStepErrors = WebUtility.GetRequestQueryValue("showStepErrors", false);
		}

		protected override void BuildRequestParams(StringBuilder strB)
		{
			base.BuildRequestParams(strB);

			this.AppendNotNullStringParam(strB, "showStepErrors", this.ShowStepErrors.ToString());
		}
	}
}
