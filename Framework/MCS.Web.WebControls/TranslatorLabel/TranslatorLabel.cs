using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using MCS.Library.Globalization;
using MCS.Web.Library;

namespace MCS.Web.WebControls
{
	/// <summary>
	/// 带有翻译功能的Label
	/// </summary>
	[ToolboxData(@"<{0}:TranslatorLabel runat=""server"" Text=""TranslatorLabel""></{0}:TranslatorLabel>")]
	[DataBindingHandler("System.Web.UI.Design.TextDataBindingHandler, System.Design, Version=2.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a")]
	[DefaultProperty("Text")]
	[ParseChildren(false)]
	[ControlBuilder(typeof(LabelControlBuilder))]
	[ControlValueProperty("Text")]
	[Designer("System.Web.UI.Design.WebControls.LabelDesigner, System.Design, Version=2.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a")]
	public class TranslatorLabel : Label
	{
		/// <summary>
		/// Category
		/// </summary>
		public string Category
		{
			get
			{
				return WebControlUtility.GetViewStateValue(ViewState, "Category", string.Empty);
			}
			set
			{
				WebControlUtility.SetViewStateValue(ViewState, "Category", value);
			}
		}

		/// <summary>
		/// 是否自动进行Html Encode
		/// </summary>
		[DefaultValue(false)]
		[Description("知否自动进行HtmlEncode")]
		public bool HtmlEncode
		{
			get
			{
				return WebControlUtility.GetViewStateValue(ViewState, "HtmlEncode", false);
			}
			set
			{
				WebControlUtility.SetViewStateValue(ViewState, "HtmlEncode", value);
			}
		}

		/// <summary>
		/// 渲染
		/// </summary>
		/// <param name="writer"></param>
		protected override void RenderContents(HtmlTextWriter writer)
		{
			string text = Translator.Translate(this.Category, this.Text);

			if (HtmlEncode)
				text = HttpUtility.HtmlEncode(text);

			writer.Write(text);
		}
	}
}
