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
using MCS.Library.WF.Contracts.Ogu;

namespace CIIC.HSR.TSP.WF.UI.Control.Controls.StartWorkflow
{
    /// <summary>
    /// 工作流构建器
    /// </summary>
    public class WFStartWorkflowBuilder : WFControlBuilderBase<WFStartWorkflow, WFStartWorkflowBuilder, DropDownButton, DropDownButtonBuilder>
    {
        public WFStartWorkflowBuilder(HtmlHelper helper) :
            base(new WFStartWorkflow(helper.ViewContext, helper.ViewData), helper)
        {
        }

        /// <summary>
        /// 任务标题
        /// </summary>
        /// <param name="taskTitle">标题</param>
        /// <returns>构建器</returns>
        public WFStartWorkflowBuilder TaskTitle(string taskTitle)
        {
            this.Component.TaskTitle = taskTitle;

            return this;
        }

        /// <summary>
        /// 业务地址Url
        /// </summary>
        /// <param name="businessUrl">表单地址</param>
        /// <returns>构建器</returns>
        public WFStartWorkflowBuilder BusinessUrl(string businessUrl)
        {
            this.Component.BusinessUrl = businessUrl;

            return this;
        }

        /// <summary>
        /// 设置模板Id
        /// </summary>
        /// <param name="templateKey">模板Id</param>
        /// <returns>构建器</returns>
        public WFStartWorkflowBuilder TemplateKey(string templateKey)
        {
            this.Component.TemplateKey = templateKey;

            return this;
        }

        /// <summary>
        /// 动态角色审批人列表
        /// </summary>
        public WFStartWorkflowBuilder DictionaryWfClientUser(Dictionary<string, List<WfClientUser>> dictionaryWfClientUser)
        {
            this.Component.DictionaryWfClientUser = dictionaryWfClientUser;

            return this;
        }

        /// <summary>
        /// 下拉框对齐方式
        /// </summary>
        /// <param name="action">下拉框对齐方式</param>
        /// <returns>构建器</returns>
        public WFStartWorkflowBuilder AlignType(Action<AlignTypeBuilder> action)
        {
            this.ButtonWidgetBuilder.AlignType(action);

            return this;
        }

        /// <summary>
        /// 额外属性
        /// </summary>
        /// <param name="action">额外属性</param>
        /// <returns>构建器</returns>
        public WFStartWorkflowBuilder Attributes(Action<CIIC.HSR.TSP.WebComponents.Widgets.Button.AttrBuilder> action)
        {
            this.ButtonWidgetBuilder.Attributes(action);

            return this;
        }

        /// <summary>
        /// 按钮样式
        /// </summary>
        /// <param name="action">按钮样式</param>
        /// <returns>构建器</returns>
        public WFStartWorkflowBuilder ButtonStyle(Action<ButtonStyleBuilder> action)
        {
            this.ButtonWidgetBuilder.ButtonStyle(action);

            return this;
        }

        /// <summary>
        /// 按钮类型
        /// </summary>
        /// <param name="action">按钮类型</param>
        /// <returns>构建器</returns>
        public WFStartWorkflowBuilder ButtonType(Action<ButtonTypeBuilder> action)
        {
            this.ButtonWidgetBuilder.ButtonType(action);

            return this;
        }

        /// <summary>
        /// 按钮的文本
        /// </summary>
        /// <param name="text">文本</param>
        /// <returns>构建器</returns>
        public WFStartWorkflowBuilder Text(string text)
        {
            this.ButtonWidgetBuilder.Text(text);

            return this;
        }

        /// <summary>
        /// 提示框内容，在EnableDialog为true，才起作用
        /// </summary>
        /// <param name="content">内容</param>
        /// <returns></returns>
        public WFStartWorkflowBuilder DialogText(string content)
        {
            this.ButtonWidgetBuilder.DialogText(content);

            return this;
        }

        /// <summary>
        /// 权限编码
        /// </summary>
        /// <param name="code">编码</param>
        /// <returns></returns>
        public WFStartWorkflowBuilder ResourceCode(string code)
        {
            this.ButtonWidgetBuilder.ResourceCode(code);

            return this;
        }

        /// <summary>
        /// 弹出风格
        /// </summary>
        /// <param name="action">弹出风格</param>
        /// <returns>构建器</returns>
        public WFStartWorkflowBuilder PopupMode(Action<PopupModeBuilder> action)
        {
            this.ButtonWidgetBuilder.PopupMode(action);

            return this;
        }

        /// <summary>
        /// 按钮大小
        /// </summary>
        /// <param name="action">按钮大小</param>
        /// <returns>构建器</returns>
        public WFStartWorkflowBuilder SizeMode(Action<SizeModeBuilder> action)
        {
            this.ButtonWidgetBuilder.SizeMode(action);

            return this;
        }

        /// 按钮状态
        /// </summary>
        /// <param name="status">true可用，false禁用</param>
        /// <returns></returns>
        public WFStartWorkflowBuilder Enabled(bool status)
        {
            this.ButtonWidgetBuilder.Enabled(status);

            return this;
        }

        /// 显示下拉
        /// </summary>
        /// <param name="Divide">true是，false否</param>
        /// <returns></returns>
        public WFStartWorkflowBuilder ShowDrop(bool showDrop)
        {
            this.ButtonWidgetBuilder.ShowDrop(showDrop);

            return this;
        }

        /// 图标
        /// </summary>
        /// <param name="icon">图标</param>
        /// <returns></returns>
        public WFStartWorkflowBuilder Icon(string icon)
        {
            this.ButtonWidgetBuilder.Icon(icon);

            return this;
        }
        /// <summary>
        /// 需要滚动条
        /// </summary>
        /// <param name="status">true可用，false禁用</param>
        /// <returns></returns>
        public WFStartWorkflowBuilder ProgressBar(bool progressBar)
        {
            this.ButtonWidgetBuilder.ProgressBar(progressBar);

            return this;
        }

        protected override WidgetBuilderBase<DropDownButton, DropDownButtonBuilder> GetWidgetBuilder(HtmlHelper helper)
        {
            DropDownButtonBuilder builder = new DropDownButtonBuilder(helper);

            builder.Divide(true);

            return builder;
        }

        private DropDownButtonBuilder ButtonWidgetBuilder
        {
            get
            {
                return (DropDownButtonBuilder)base.WidgetBuilder;
            }
        }
    }
}
