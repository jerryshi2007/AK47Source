using System;
using System.Linq;
using System.Text;
using System.Collections.Generic;
using MCS.Web.Library;
using MCS.Library.Globalization;

namespace MCS.Web.WebControls
{
	/// <summary>
	/// 流程环节编辑器
	/// </summary>
	[Serializable]
	public class WfActivityDescriptorEditorParams : WfProcessDailogControlParams
	{
		private string currentActivityKey = string.Empty;
		private string operation = string.Empty;
		private bool showCirculateUsers = false;
		private bool allAgreeWhenConsign = true;

		public WfActivityDescriptorEditorParams()
		{
			string title = Translator.Translate(Define.DefaultCulture, "流程调整");

			DialogHeaderText = title;
			DialogTitle = title;
		}

		public bool AllAgreeWhenConsign
		{
			get { return this.allAgreeWhenConsign; }
			set { this.allAgreeWhenConsign = value; }
		}

		public bool ShowCirculateUsers
		{
			get { return this.showCirculateUsers; }
			set { this.showCirculateUsers = value; }
		}

		public string CurrentActivityKey
		{
			get { return this.currentActivityKey; }
			set { this.currentActivityKey = value; }
		}

		public string Operation
		{
			get { return this.operation; }
			set { this.operation = value; }
		}

		public override void LoadDataFromQueryString()
		{
			this.currentActivityKey = WebUtility.GetRequestQueryValue("currentActivityKey", string.Empty);
			this.operation = WebUtility.GetRequestQueryValue("op", "add");
			this.showCirculateUsers = WebUtility.GetRequestQueryValue("showCirculateUsers", false);
			this.allAgreeWhenConsign = WebUtility.GetRequestQueryValue("allAgreeWhenConsign", true);

			base.LoadDataFromQueryString();
		}

		protected override void BuildRequestParams(StringBuilder strB)
		{
			base.BuildRequestParams(strB);

			AppendNotNullStringParam(strB, "currentActivityKey", this.currentActivityKey);
			AppendNotNullStringParam(strB, "op", this.operation);
			AppendParam(strB, "showCirculateUsers", this.showCirculateUsers);
			AppendParam(strB, "allAgreeWhenConsign", this.allAgreeWhenConsign);
		}
	}
}
