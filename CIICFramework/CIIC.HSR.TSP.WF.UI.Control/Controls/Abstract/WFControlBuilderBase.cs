using CIIC.HSR.TSP.WebComponents;
using CIIC.HSR.TSP.WebComponents.Widgets.Button;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Mvc;

namespace CIIC.HSR.TSP.WF.UI.Control.Controls.Abstract
{
    public abstract class WFControlBuilderBase<TViewComponent, TBuilder, TWidget, TWidgetBuilder> : WidgetBuilderBase<TViewComponent, TBuilder>
        where TViewComponent : WFControlBase
        where TBuilder : WFControlBuilderBase<TViewComponent, TBuilder, TWidget, TWidgetBuilder>
        where TWidget : WidgetBase
        where TWidgetBuilder : WidgetBuilderBase<TWidget, TWidgetBuilder>
    {
        private WidgetBuilderBase<TWidget, TWidgetBuilder> _WidgetBuilder = null;

        public WFControlBuilderBase(TViewComponent component, HtmlHelper helper)
            : base(component, helper)
        {
            this._WidgetBuilder = this.GetWidgetBuilder(helper);
            this.Component.Widget = this.WidgetBuilder.ToComponent();
        }

        protected WidgetBuilderBase<TWidget, TWidgetBuilder> WidgetBuilder
        {
            get
            {
                return this._WidgetBuilder;
            }
        }

        //protected ButtonBuilder ButtonBuilder
        //{
        //    get
        //    {
        //        return this._ButtonBuilder;
        //    }
        //}

        protected abstract WidgetBuilderBase<TWidget, TWidgetBuilder> GetWidgetBuilder(HtmlHelper helper);

        /// <summary>
        /// 设置请求URL
        /// </summary>
        /// <param name="actionUrl">请求URL</param>
        /// <returns>构建器</returns>
        public TBuilder ActionUrl(string actionUrl)
        {
            this.Component.ActionUrl = actionUrl;

            return (TBuilder)this;
        }

        /// <summary>
        /// 调用前客户端事件名称
        /// </summary>
        /// <param name="handler">事件名称</param>
        /// <returns>构建器</returns>
        public TBuilder BeforeClick(string handler)
        {
            this.Component.BeforeClick = handler;

            return (TBuilder)this;
        }

        /// <summary>
        /// 调用后客户端事件名称
        /// </summary>
        /// <param name="handler">事件名称</param>
        /// <returns>构建器</returns>
        public TBuilder AfterClick(string handler)
        {
            this.Component.AfterClick = handler;

            return (TBuilder)this;
        }

        /// <summary>
        /// 审核意见控件ID
        /// </summary>
        /// <param name="handler">事件名称</param>
        /// <returns>构建器</returns>
        public TBuilder CommentsControlId(string commentsControlId)
        {
            this.Component.CommentsControlId = commentsControlId;

            return (TBuilder)this;
        }
        /// <summary>
        /// 用户单击一个控件所表示的业务行为，如同意、流转等
        /// </summary>
        /// <param name="actonResult">业务行为</param>
        /// <returns>构建器</returns>
        public TBuilder ActionResult(string actonResult)
        {
            this.Component.ActionResult = actonResult;

            return (TBuilder)this;
        }
    }
}
