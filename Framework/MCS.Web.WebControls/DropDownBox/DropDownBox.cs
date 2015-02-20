using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MCS.Web.Library.Script;
using System.ComponentModel;
using System.Web.UI;
using System.Runtime;

[assembly: WebResource("MCS.Web.WebControls.DropDownBox.DropDownBox.js", "application/x-javascript")]
namespace MCS.Web.WebControls
{
	/// <summary>
	/// 表示一个可以下拉的容器(未完工)
	/// </summary>
	[ClientCssResource("MCS.Web.WebControls.GridColumnSorter.GridColumnSorter.css")]
	[ClientScriptResource("MCS.Web.WebControls.DropDownBox", "MCS.Web.WebControls.DropDownBox.DropDownBox.js")]
	[DefaultProperty("Text")]
	public class DropDownBox : ScriptControlBase, INamingContainer
	{
		private ITemplate _captionTemplate;
		private ITemplate _contentTemplate;
		/// <summary>
		/// 初始化新实例
		/// </summary>
		public DropDownBox()
			: base(false, System.Web.UI.HtmlTextWriterTag.Div)
		{
			this.CssClass = "mcsc-drop-sorter";
		}

		/// <summary>
		/// Css类
		/// </summary>
		[DefaultValue("mcsc-drop-sorter")]
		public override string CssClass
		{
			get
			{
				return base.CssClass;
			}
			set
			{
				base.CssClass = value;
			}
		}

		/// <summary>
		/// 由 ASP.NET 页面框架调用，以通知使用基于合成的实现的服务器控件创建它们包含的任何子控件，以便为回发或呈现做准备。
		/// </summary>
		protected override void CreateChildControls()
		{
			base.CreateChildControls();
			if (this._captionTemplate != null)
			{

			}
		}

		/// <summary>
		/// 呈现内容
		/// </summary>
		/// <param name="output"></param>
		protected override void RenderContents(HtmlTextWriter output)
		{
			RenderCaption(output);

			RenderDropContainer(output);
		}

		/// <summary>
		/// 生成下拉框
		/// </summary>
		/// <param name="output"></param>
		private void RenderDropContainer(HtmlTextWriter output)
		{
			if (this.HasControls())
			{
				output.BeginRender();
				output.AddAttribute(HtmlTextWriterAttribute.Class, "mcsc-drop-container-wrapper");
				output.RenderBeginTag(HtmlTextWriterTag.Div);

				output.RenderEndTag();
				output.EndRender();
			}
		}

		private void RenderCaption(HtmlTextWriter output)
		{
			output.RenderBeginTag(HtmlTextWriterTag.Ul);
			output.RenderBeginTag(HtmlTextWriterTag.Li);
			output.AddAttribute(HtmlTextWriterAttribute.Class, "mcsc-seldown");
			output.RenderBeginTag(HtmlTextWriterTag.S);
			output.AddAttribute(HtmlTextWriterAttribute.Class, "mcsc-arrow");
			output.RenderBeginTag(HtmlTextWriterTag.S);
			output.RenderEndTag();//s
			output.RenderEndTag();//s

			output.WriteEncodedText("Hello");

			output.RenderEndTag();//li
			output.RenderEndTag();//ul

		}

		/// <summary>
		/// 标题模板
		/// </summary>
		[PersistenceMode(PersistenceMode.InnerProperty), DefaultValue((string)null), Browsable(false), TemplateContainer(typeof(DropDownBox), BindingDirection.TwoWay), Description("标题模板")]
		public ITemplate CaptionTemplate
		{
			[TargetedPatchingOptOut("Performance critical to inline this type of method across NGen image boundaries")]
			get
			{
				return this._captionTemplate;
			}

			[TargetedPatchingOptOut("Performance critical to inline this type of method across NGen image boundaries")]
			set
			{
				this._captionTemplate = value;
			}
		}

		/// <summary>
		/// 内容模板
		/// </summary>
		[PersistenceMode(PersistenceMode.InnerProperty), DefaultValue((string)null), Browsable(false), TemplateContainer(typeof(DropDownBox), BindingDirection.TwoWay), Description("内容模板")]
		public ITemplate ContentTemplate
		{
			[TargetedPatchingOptOut("Performance critical to inline this type of method across NGen image boundaries")]
			get
			{
				return this._contentTemplate;
			}

			[TargetedPatchingOptOut("Performance critical to inline this type of method across NGen image boundaries")]
			set
			{
				this._contentTemplate = value;
			}
		}
	}
}
