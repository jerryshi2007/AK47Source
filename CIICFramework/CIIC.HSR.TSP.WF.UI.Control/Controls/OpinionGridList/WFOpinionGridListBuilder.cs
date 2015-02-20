using CIIC.HSR.TSP.WebComponents;
using CIIC.HSR.TSP.WebComponents.Widgets.TextBox;
using CIIC.HSR.TSP.WF.UI.Control.Controls.Abstract;
using CIIC.HSR.TSP.WF.UI.Control.Controls.Comments;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Mvc;

namespace CIIC.HSR.TSP.WF.UI.Control.Controls.OpinionGridList
{
    /// <summary>
    /// 工作流意见文本框构造器
    /// </summary>
    public class WFOpinionGridListBuilder<TModel> : WidgetBuilderBase<WFOpinionGridList<TModel>, WFOpinionGridListBuilder<TModel>>
    {
        public WFOpinionGridListBuilder(HtmlHelper<TModel> htmlHelper)
            : base(new WFOpinionGridList<TModel>(htmlHelper), htmlHelper)
        {
    
        }
 
        ///// <summary>
        ///// 页数
        ///// </summary>
        /// <param name="pageSize">页数</param>
        /// <returns>构建器</returns>
        public WFOpinionGridListBuilder<TModel> PageSize(int pageSize)
        {
            this.Component.PageSize = pageSize;

            return this;
        }

        ///// <summary>
        ///// 标题
        ///// </summary>
        /// <param name="pageSize">标题</param>
        /// <returns>构建器</returns>
        public WFOpinionGridListBuilder<TModel> Title(string title)
        {
            this.Component.Title = title;

            return this;
        }

        ///// <summary>
        ///// 高度
        ///// </summary>
        /// <param name="height">高度</param>
        /// <returns>高度</returns>
        public WFOpinionGridListBuilder<TModel> Height(int height)
        {
            this.Component.Height = height;

            return this;
        }


    }
}
