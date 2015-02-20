using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MCS.Library.Core;
using MCS.Web.Library;

namespace MCS.Web.WebControls
{
	[Serializable]
	public class RoleGraphControlParams : DialogControlParamsBase
	{
		public const string DefaultDialogTitle = "请选择角色";

		private string _SelectedFullCodeName = string.Empty;

		public string SelectedFullCodeName
		{
			get { return this._SelectedFullCodeName; }
			set { this._SelectedFullCodeName = value; }
		}

		/// <summary>
		/// 从Request的QueryString获得类参数
		/// </summary>
		public override void LoadDataFromQueryString()
		{
			this._SelectedFullCodeName = WebUtility.GetRequestQueryValue("selectedFullCodeName", string.Empty);

			base.LoadDataFromQueryString();
		}

		/// <summary>
		/// 将类参数添加到url中
		/// </summary>
		/// <param name="strB"></param>
		protected override void BuildRequestParams(StringBuilder strB)
		{
			base.BuildRequestParams(strB);

			if (this._SelectedFullCodeName.IsNotEmpty())
				base.AppendParam(strB, "selectedFullCodeName", this._SelectedFullCodeName);
		}
	}
}
