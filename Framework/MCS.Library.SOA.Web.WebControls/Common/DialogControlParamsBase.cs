using System;
using System.Collections.Generic;
using System.Text;
using System.Web;
using MCS.Web.Library;

namespace MCS.Web.WebControls
{
	/// <summary>
	/// 对话框控件参数的基类
	/// </summary>
	[Serializable]
	public abstract class DialogControlParamsBase
	{
		private bool loadedFromQueryString = false;

		private string dialogTitle = string.Empty;
		private string dialogHeaderText = string.Empty;
		private string logoImageUrl = string.Empty;
		private string category = string.Empty;
		private string tag = string.Empty;

		/// <summary>
		/// 是否从QueryString加载
		/// </summary>
		public bool LoadedFromQueryString
		{
			get { return this.loadedFromQueryString; }
			set { this.loadedFromQueryString = value; }
		}

		/// <summary>
		/// 对话框的窗口标题
		/// </summary>
		public string DialogTitle
		{
			get { return this.dialogTitle; }
			set { this.dialogTitle = value; }
		}

		/// <summary>
		/// Culture对应的Category
		/// </summary>
		public string Category
		{
			get { return this.category; }
			set { this.category = value; }
		}

		/// <summary>
		/// 对话框顶端的标题（不是窗口标题）
		/// </summary>
		public string DialogHeaderText
		{
			get { return this.dialogHeaderText; }
			set { this.dialogHeaderText = value; }
		}

		/// <summary>
		/// 对话框内容中，标题左侧的图标
		/// </summary>
		public string LogoImageUrl
		{
			get { return this.logoImageUrl; }
			set { this.logoImageUrl = value; }
		}

		public string Tag
		{
			get { return this.tag; }
			set { this.tag = value; }
		}

		/// <summary>
		/// 从Request.QueryString填充数据
		/// </summary>
		public virtual void LoadDataFromQueryString()
		{
			this.loadedFromQueryString = true;
			this.dialogTitle = WebUtility.GetRequestQueryValue("dialogTitle", this.dialogTitle);
			this.dialogHeaderText = WebUtility.GetRequestQueryValue("dialogHeaderText", this.dialogHeaderText);
			this.logoImageUrl = WebUtility.GetRequestQueryValue("logoImageUrl", this.logoImageUrl);
			this.category = WebUtility.GetRequestQueryValue("category", this.category);
			this.tag = WebUtility.GetRequestQueryValue("tag", this.tag);
		}

		/// <summary>
		/// 输出成RequestParam
		/// </summary>
		/// <returns></returns>
		public string ToRequestParams()
		{
			StringBuilder strB = new StringBuilder();

			BuildRequestParams(strB);

			return strB.ToString();
		}

		/// <summary>
		/// 在StringBuilder中添加参数
		/// </summary>
		/// <param name="strB"></param>
		protected virtual void BuildRequestParams(StringBuilder strB)
		{
			AppendNotNullStringParam(strB, "dialogTitle", this.dialogTitle);
			AppendNotNullStringParam(strB, "dialogHeaderText", this.dialogHeaderText);
			AppendNotNullStringParam(strB, "logoImageUrl", this.logoImageUrl);
			AppendNotNullStringParam(strB, "category", this.category);
			AppendNotNullStringParam(strB, "tag", this.tag);
		}

		/// <summary>
		/// 在url串中添加一个非空的参数
		/// </summary>
		/// <param name="strB"></param>
		/// <param name="paramName"></param>
		/// <param name="data"></param>
		protected void AppendNotNullStringParam(StringBuilder strB, string paramName, string data)
		{
			if (string.IsNullOrEmpty(data) == false)
				AppendParam(strB, paramName, data);
		}

		protected void AppendParam(StringBuilder strB, string paramName, object data)
		{
			if (strB.Length > 0)
				strB.Append("&");

			strB.AppendFormat("{0}={1}", HttpUtility.UrlEncode(paramName), HttpUtility.UrlEncode(data.ToString()));
		}
	}
}
