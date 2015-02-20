using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MCS.Web.Library;
using MCS.Library.Globalization;
using MCS.Web.WebControls;

namespace MCS.Web.WebControls
{
	[Serializable]
	public class PredefinedOpinionDialogParams : DialogControlParamsBase
	{
		private string _userID = string.Empty;

		public PredefinedOpinionDialogParams()
		{
			DialogHeaderText = Translator.Translate(Define.DefaultCulture, "常用意见");
			DialogTitle = DialogHeaderText;
		}

		public string UserID
		{
			get { return this._userID; }
			set { this._userID = value; }
		}

		public override void LoadDataFromQueryString()
		{
			this._userID = WebUtility.GetRequestQueryValue("userID", "0");

			base.LoadDataFromQueryString();
		}

		protected override void BuildRequestParams(StringBuilder strB)
		{
			base.BuildRequestParams(strB);

			AppendNotNullStringParam(strB, "userID", this._userID);
		}
	}
}
