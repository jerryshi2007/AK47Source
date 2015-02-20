using CIIC.HSR.TSP.WebComponents;
using CIIC.HSR.TSP.WebComponents.Widgets.Button;
using CIIC.HSR.TSP.WF.UI.Control.Controls.Abstract;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Mvc;

namespace CIIC.HSR.TSP.WF.UI.Control.Controls.FollowWorkFlow
{
    /// <summary>
    /// 工作流构建器
    /// </summary>
    public class WFTrackWorkflowBuilder : WFButtonControlBuilderlBase<WFTrackWorkflow, WFTrackWorkflowBuilder>
    {
        //private WindowBuilder _WindowBuilder = null;

        public WFTrackWorkflowBuilder(HtmlHelper htmlHelper)
            : base(new WFTrackWorkflow(htmlHelper.ViewContext, htmlHelper.ViewData), htmlHelper)
        {
            //_WindowBuilder = new WindowBuilder(htmlHelper);
            //this.Component.InnerWindow = _WindowBuilder.ToComponent();
        }

        ///// <summary>
        ///// 弹出对话框的标题
        ///// </summary>
        ///// <param name="title">标题</param>
        ///// <returns></returns>
        //public WFTrackWorkFlowBuilder WindowTitle(string title)
        //{
        //    Component.WindowTitle = title;
        //    return this;
        //}

        ///// <summary>
        ///// 弹出对话框的宽度
        ///// </summary>
        ///// <param name="width"></param>
        ///// <returns></returns>
        //public WFTrackWorkFlowBuilder WindowWidth(int width)
        //{
        //    Component.WindowWidth = width;
        //    return this;
        //}

        ///// <summary>
        ///// 弹出对话框的高度
        ///// </summary>
        ///// <param name="height"></param>
        ///// <returns></returns>
        //public WFTrackWorkFlowBuilder WindowHeight(int height)
        //{
        //    Component.WindowHeight = height;
        //    return this;
        //}
        ///// <summary>
        ///// 设置弹出对话框的是否为模式对话框
        ///// </summary>
        ///// <param name="modal"></param>
        ///// <returns></returns>
        //public WFTrackWorkFlowBuilder IsModalWindow(bool modal)
        //{
        //    Component.IsModalWindow = modal;
        //    return this;
        //}

        ///// <summary>
        ///// 设置是否为IFrame模式
        ///// </summary>
        ///// <param name="iframe"></param>
        ///// <returns></returns>
        //public WFTrackWorkFlowBuilder IFrameWindow(bool iframe)
        //{
        //    Component.IFrameWindow = iframe;
        //    return this;
        //}
    }
}
