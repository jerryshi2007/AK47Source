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
		/// �Ƿ���ʾ����Ĵ���
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
