using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MCS.Web.Library;

namespace MCS.Web.WebControls
{
    /// <summary>
    /// 退件环节选择对话框参数
    /// </summary>
    [Serializable]
    public class RejectActivitySelectorParams : DialogControlParamsBase
    {
        private string _actID = string.Empty;
		private bool _returnToOperator = false;
		private bool showOpinionInput = false;

        public RejectActivitySelectorParams()
        {
            DialogHeaderText = "选择退件环节";
            DialogTitle = "选择退件环节";
        }

        #region Properties

		/// <summary>
        /// 当前环节 ID
        /// </summary>
        public string ActivityID
        {
            get { return this._actID; }
            set { this._actID = value; }
        }

		/// <summary>
		/// 是否退回给操作人，如果不是，则送给流程的每个定义的人
		/// </summary>
		public bool ReturnToOperator
		{
			get { return this._returnToOperator; }
			set { this._returnToOperator = value; }
		}

		public bool ShowOpinionInput
		{
			get
			{
				return this.showOpinionInput;
			}
			set
			{
				this.showOpinionInput = value;
			}
		}
        #endregion Properties

        #region Override
        public override void LoadDataFromQueryString()
        {
            this.ActivityID = WebUtility.GetRequestQueryValue("activityID", "0");
			this.ReturnToOperator = WebUtility.GetRequestQueryValue("rto", false);
			this.showOpinionInput = WebUtility.GetRequestQueryValue("soi", false);

            base.LoadDataFromQueryString();
        }

        protected override void BuildRequestParams(StringBuilder strB)
        {
            base.BuildRequestParams(strB);

            AppendNotNullStringParam(strB, "activityID", this.ActivityID);
			AppendParam(strB, "rto", this.ReturnToOperator);

			if (this.showOpinionInput)
				AppendParam(strB, "soi", this.showOpinionInput);
        }
        #endregion Override
    }
}
