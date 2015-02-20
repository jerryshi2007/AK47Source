using System;
using System.Text;
using System.Collections.Generic;
using MCS.Web.Library;
using MCS.Web.WebControls;

namespace MCS.Web.WebControls
{
	[Serializable]
	public class OpinionInputDialogParams : DialogControlParamsBase
	{
		private string emptyOpinionPrompt = "请填写意见";

		/// <summary>
		/// 是否允许空意见
		/// </summary>
		public bool AllowEmptyOpinion
		{
			get;
			set;
		}

		/// <summary>
		/// 空意见的提示信息
		/// </summary>
		public string EmptyOpinionPrompt
		{
			get
			{
				return this.emptyOpinionPrompt;
			}
			set
			{
				this.emptyOpinionPrompt = value;
			}
		}

		public override void LoadDataFromQueryString()
		{
			this.AllowEmptyOpinion = WebUtility.GetRequestQueryValue("aeo", false);
			this.EmptyOpinionPrompt = WebUtility.GetRequestQueryValue("eoa", "请填写意见");

			base.LoadDataFromQueryString();
		}

		protected override void BuildRequestParams(StringBuilder strB)
		{
			base.BuildRequestParams(strB);

			AppendParam(strB, "aeo", this.AllowEmptyOpinion);
			AppendParam(strB, "eoa", this.EmptyOpinionPrompt);
		}
	}
}
