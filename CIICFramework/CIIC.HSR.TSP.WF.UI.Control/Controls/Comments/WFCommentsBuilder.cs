using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Mvc;
using CIIC.HSR.TSP.WebComponents;
using CIIC.HSR.TSP.WebComponents.Widgets.TextBox;
using CIIC.HSR.TSP.WF.UI.Control.Controls.Abstract;
using CIIC.HSR.TSP.WF.UI.Control.Controls.Comments;

namespace CIIC.HSR.TSP.WF.UI.Control.Controls.Comments
{
	/// <summary>
	/// 工作流意见文本框构造器
	/// </summary>
	public class WFCommentsBuilder<TModel> : WidgetBuilderBase<WFComments<TModel>, WFCommentsBuilder<TModel>>
	{

		private TextBoxBuidler<TModel, string> _TextBuilder = null;
		public WFCommentsBuilder(HtmlHelper<TModel> htmlHelper)
			: base(new WFComments<TModel>(htmlHelper.ViewContext, htmlHelper.ViewData), htmlHelper)
		{
			_TextBuilder = new TextBoxBuidler<TModel, string>(htmlHelper);
			this.Component.InnerTextBox = _TextBuilder.ToComponent();
		}

		/// <summary>
		/// 列数
		/// </summary>
		public WFCommentsBuilder<TModel> Collumns(int collumns)
		{
			this.Component.Collumns = collumns;

			return this;
		}

		/// <summary>
		/// 行数
		/// </summary>
		public WFCommentsBuilder<TModel> Lines(int lines)
		{
			this.Component.Lines = lines;

			return this;
		}

		/// <summary>
		/// 是否可用
		/// </summary>
		public WFCommentsBuilder<TModel> Enabled(bool enabled)
		{
			this.Component.Enabled = enabled;

			return this;
		}
        /// <summary>
        /// 意见输入控件容器ID
        /// </summary>
        public WFCommentsBuilder<TModel> OpinionContainerId(string opinionContainerId)
		{
            this.Component.OpinionContainerId = opinionContainerId;

			return this;
		}
	}
}
