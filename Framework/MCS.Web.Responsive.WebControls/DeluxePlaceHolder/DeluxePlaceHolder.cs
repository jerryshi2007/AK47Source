using MCS.Library.Core;
using MCS.Web.Library;
using MCS.Web.Responsive.Library;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace MCS.Web.Responsive.WebControls
{
	/// <summary>
	/// 模板的加载模式
	/// </summary>
	public enum TemplateLoadingMode
	{
		AsText,
		AsControl,
	}

	/// <summary>
	/// 用于加载模板，或者占位信息的控件
	/// </summary>
	[DefaultProperty("Text")]
	[ToolboxData("<{0}:DeluxePlaceHolder runat=server></{0}:DeluxePlaceHolder>")]
	public class DeluxePlaceHolder : Control
	{
		/// <summary>
		/// 模板的路径
		/// </summary>
		[Bindable(true), Category("Appearance"), DefaultValue("")]
		public string TemplatePath
		{
			get
			{
				return this.ViewState.GetValue("TemplatePath", string.Empty);
			}
			set
			{
				this.ViewState.SetViewStateValue("TemplatePath", value);
			}
		}

		/// <summary>
		/// 模板是否作为控件来对待（会执行的）
		/// </summary>
		[Bindable(true), Category("Appearance"), DefaultValue(TemplateLoadingMode.AsControl)]
		public TemplateLoadingMode LoadingMode
		{
			get
			{
				return this.ViewState.GetValue("LoadingMode", TemplateLoadingMode.AsControl);
			}
			set
			{
				this.ViewState.SetViewStateValue("LoadingMode", value);
			}
		}

		protected override void Render(HtmlTextWriter writer)
		{
			if (this.DesignMode == false)
			{
				string templateText = this.GetTemplateText();

				if (this.LoadingMode == TemplateLoadingMode.AsControl)
					templateText = this.Page.ParseControl(templateText).GetHtml();

				writer.Write(templateText);
			}
			else
				base.Render(writer);
		}

		/// <summary>
		/// 得到解析后的模板文件路径
		/// </summary>
		/// <returns></returns>
		private string GetResolvedTemplatePath()
		{
			string result = this.TemplatePath;

			if (this.TemplatePath.IsNotEmpty())
				result = this.ResolveUrl(this.TemplatePath);

			return result;
		}

		private string GetTemplateText()
		{
			string result = string.Empty;
			string virtualPath = this.GetResolvedTemplatePath();

			if (virtualPath.IsNotEmpty())
				result = WebXmlDocumentCache.GetDocument(virtualPath);

			return result;
		}
	}
}
