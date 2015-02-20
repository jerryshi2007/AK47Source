using CIIC.HSR.TSP.WebComponents;
using CIIC.HSR.TSP.WebComponents.Widgets.DropDownButton;
using CIIC.HSR.TSP.WF.UI.Control.Controls.Abstract;
using CIIC.HSR.TSP.WebComponents.Widgets.Button;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Mvc;
using CIIC.HSR.TSP.WebComponents.Widgets.Toolbar;
using CIIC.HSR.TSP.WF.UI.Control.Controls.ToolbarMoveToWorkflow;
using MCS.Library.WF.Contracts.Ogu;

namespace CIIC.HSR.TSP.WF.UI.Control.Controls.ToolbarMoveToWorkflow
{
    /// <summary>
    /// 工作流构建器
    /// </summary>
    public class WFToolbarMoveToWorkflowBuilder : WFControlBuilderBase<WFToolbarMoveToWorkflow, WFToolbarMoveToWorkflowBuilder, Toolbar, ToolbarBuilder>
    {
        public WFToolbarMoveToWorkflowBuilder(HtmlHelper htmlHelper)
            : base(new WFToolbarMoveToWorkflow(htmlHelper.ViewContext, htmlHelper.ViewData), htmlHelper)
        {
        }

        /// <summary>
        /// 任务标题
        /// </summary>
        /// <param name="taskTitle">标题</param>
        /// <returns>构建器</returns>
        public WFToolbarMoveToWorkflowBuilder TaskTitle(string taskTitle)
        {
            this.Component.TaskTitle = taskTitle;

            return this;
        }

        /// <summary>
        /// 业务地址Url
        /// </summary>
        /// <param name="businessUrl">表单地址</param>
        /// <returns>构建器</returns>
        public WFToolbarMoveToWorkflowBuilder BusinessUrl(string businessUrl)
        {
            this.Component.BusinessUrl = businessUrl;

            return this;
        }


        /// <summary>
        /// 按钮的文本
        /// </summary>
        /// <param name="text">文本</param>
        /// <returns>构建器</returns>
        public WFToolbarMoveToWorkflowBuilder Text(string text)
        {
            this.Component.Text = text;

            return this;
        }

        /// <summary>
        /// 提示框内容，在EnableDialog为true，才起作用
        /// </summary>
        /// <param name="content">内容</param>
        /// <returns></returns>
        public WFToolbarMoveToWorkflowBuilder DialogText(string content)
        {
            this.Component.DialogText = content;

            return this;
        }

        /// <summary>
        /// 图标列表
        /// </summary>
        /// <param name="iconList">图标</param>
        /// <returns></returns>
        public WFToolbarMoveToWorkflowBuilder IconList(string iconList)
        {
            this.Component.IconList = iconList;

            return this;
        }

        /// <summary>
        /// 是否需要分开显示
        /// </summary>
        /// <param name="status">true需要，false不需要</param>
        /// <returns></returns>
        public WFToolbarMoveToWorkflowBuilder IsSplit(bool isSplit)
        {
            this.Component.IsSplit = isSplit;

            return this;
        }
       
        /// <summary>
        /// 动态角色审批人列表
        /// </summary>
        public WFToolbarMoveToWorkflowBuilder DictionaryWfClientUser(Dictionary<string, List<WfClientUser>> dictionaryWfClientUser)
        {
            this.Component.DictionaryWfClientUser = dictionaryWfClientUser;

            return this;
        }

        /// <summary>
        /// 按钮大小
        /// </summary>
        /// <param name="action">按钮大小</param>
        /// <returns>构建器</returns>
        public WFToolbarMoveToWorkflowBuilder SizeMode(Action<SizeModeBuilder> action)
        {
            SizeModeBuilder sizeModeBuilder = new SizeModeBuilder();

            action(sizeModeBuilder);
            Component.SizeMode = sizeModeBuilder.SizeMode;
            return this;
        }

        /// <summary>
        /// 按钮类型
        /// </summary>
        /// <param name="type">按钮或提交按钮</param>
        /// <returns></returns>
        public WFToolbarMoveToWorkflowBuilder ButtonType(Action<ButtonTypeBuilder> action)
        {
            ButtonTypeBuilder buttonTypeBuilder = new ButtonTypeBuilder();
            action(buttonTypeBuilder);
            Component.ButtonType = buttonTypeBuilder.ButtonType;
            return this;
        }

        /// <summary>
        /// 需要滚动条
        /// </summary>
        /// <param name="status">true可用，false禁用</param>
        /// <returns></returns>
        public WFToolbarMoveToWorkflowBuilder ProgressBar(bool progressBar)
        {
            this.Component.ProgressBar = progressBar;
            
            return this;
        }

        /// <summary>
        /// 在组中添加控件
        /// </summary>
        /// <typeparam name="TControl">控件类型</typeparam>
        /// <typeparam name="TBuilder">控件的Builder类型</typeparam>
        /// <param name="controlBuilder">控件的构建器</param>
        /// <returns></returns>
        public WFToolbarMoveToWorkflowBuilder AddControl<TControl, TBuilder>(WidgetBuilderBase<TControl, TBuilder> controlBuilder)
            where TControl : WidgetBase
            where TBuilder : WidgetBuilderBase<TControl, TBuilder>
        {
            this.Component.AddControl(controlBuilder.Component);
            return this;
        }

        public WFToolbarMoveToWorkflowBuilder Direction(Action<ToolbarDirectionBuilder> directionBuilder)
        {
            this.ToolbarWidgetBuilder.Direction(directionBuilder);
            return this;
        }
        
        protected override WidgetBuilderBase<Toolbar, ToolbarBuilder> GetWidgetBuilder(HtmlHelper helper)
        {

            ToolbarBuilder builder = new ToolbarBuilder(helper);
            
            return builder;
        }

        private ToolbarBuilder ToolbarWidgetBuilder
        {
            get
            {
                return (ToolbarBuilder)base.WidgetBuilder;
            }
        }

    }
}
