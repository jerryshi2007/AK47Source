using CIIC.HSR.TSP.WebComponents;
using CIIC.HSR.TSP.WebComponents.Widgets.Button;
using CIIC.HSR.TSP.WebComponents.Widgets.DropDownButton;
using CIIC.HSR.TSP.WF.UI.Control.Controls.Abstract;
using MCS.Library.WF.Contracts.Ogu;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Mvc;

namespace CIIC.HSR.TSP.WF.UI.Control.Controls.MoveTo
{
    public class WFMoveToBuilder : WFControlBuilderBase<WFMoveTo, WFMoveToBuilder, DropDownButton, DropDownButtonBuilder>
    {
        public WFMoveToBuilder(HtmlHelper htmlHelper)
            : base(new WFMoveTo(htmlHelper.ViewContext, htmlHelper.ViewData), htmlHelper)
        {
        }

        /// <summary>
        /// 流程Id
        /// </summary>
        /// <param name="processId">流程Id</param>
        /// <returns>构建器</returns>
        public WFMoveToBuilder ProcessId(string processId)
        {
            this.Component.ProcessId = processId;

            return this;
        }

        /// <summary>
        /// 业务Id
        /// </summary>
        /// <param name="resourceId">业务Id</param>
        /// <returns>构建器</returns>
        public WFMoveToBuilder ResourceId(string resourceId)
        {
            this.Component.ResourceId = resourceId;

            return this;
        }

        /// <summary>
        /// 节点Id
        /// </summary>
        /// <param name="activityId">节点id</param>
        /// <returns></returns>
        public WFMoveToBuilder ActivityId(string activityId)
        {
            this.Component.ActivityId = activityId;

            return this;
        }

        /// <summary>
        /// 任务标题
        /// </summary>
        /// <param name="taskTitle">标题</param>
        /// <returns>构建器</returns>
        public WFMoveToBuilder TaskTitle(string taskTitle)
        {
            this.Component.TaskTitle = taskTitle;

            return this;
        }

        /// <summary>
        /// 业务地址Url
        /// </summary>
        /// <param name="businessUrl">表单地址</param>
        /// <returns>构建器</returns>
        public WFMoveToBuilder BusinessUrl(string businessUrl)
        {
            this.Component.BusinessUrl = businessUrl;

            return this;
        }

        /// <summary>
        /// 动态角色审批人列表
        /// </summary>
        public WFMoveToBuilder DictionaryWfClientUser(Dictionary<string, List<WfClientUser>> dictionaryWfClientUser)
        {
            this.Component.DictionaryWfClientUser = dictionaryWfClientUser;

            return this;
        }

        /// <summary>
        /// 下拉框对齐方式
        /// </summary>
        /// <param name="action">下拉框对齐方式</param>
        /// <returns>构建器</returns>
        public WFMoveToBuilder AlignType(Action<AlignTypeBuilder> action)
        {
            this.ButtonWidgetBuilder.AlignType(action);

            return this;
        }

        /// <summary>
        /// 额外属性
        /// </summary>
        /// <param name="action">额外属性</param>
        /// <returns>构建器</returns>
        public WFMoveToBuilder Attributes(Action<CIIC.HSR.TSP.WebComponents.Widgets.Button.AttrBuilder> action)
        {
            this.ButtonWidgetBuilder.Attributes(action);

            return this;
        }

        /// <summary>
        /// 按钮样式
        /// </summary>
        /// <param name="action">按钮样式</param>
        /// <returns>构建器</returns>
        public WFMoveToBuilder ButtonStyle(Action<ButtonStyleBuilder> action)
        {
            this.ButtonWidgetBuilder.ButtonStyle(action);

            return this;
        }

        /// <summary>
        /// 按钮类型
        /// </summary>
        /// <param name="action">按钮类型</param>
        /// <returns>构建器</returns>
        public WFMoveToBuilder ButtonType(Action<ButtonTypeBuilder> action)
        {
            this.ButtonWidgetBuilder.ButtonType(action);

            return this;
        }

        /// <summary>
        /// 提示文本
        /// </summary>
        /// <param name="action">提示文本</param>
        /// <returns>构建器</returns>
        public WFMoveToBuilder DialogText(string dialogText)
        {
            this.ButtonWidgetBuilder.DialogText(dialogText);

            return this;
        }

        /// <summary>
        /// 是否可用
        /// </summary>
        /// <param name="enabled">是否可用</param>
        /// <returns>构建器</returns>
        public WFMoveToBuilder Enabled(bool enabled)
        {
            this.ButtonWidgetBuilder.Enabled(enabled);

            return this;
        }

        /// <summary>
        /// 弹出风格
        /// </summary>
        /// <param name="action">弹出风格</param>
        /// <returns>构建器</returns>
        public WFMoveToBuilder PopupMode(Action<PopupModeBuilder> action)
        {
            this.ButtonWidgetBuilder.PopupMode(action);

            return this;
        }

        /// <summary>
        /// 按钮大小
        /// </summary>
        /// <param name="action">按钮大小</param>
        /// <returns>构建器</returns>
        public WFMoveToBuilder SizeMode(Action<SizeModeBuilder> action)
        {
            this.ButtonWidgetBuilder.SizeMode(action);

            return this;
        }

        /// <summary>
        /// 按钮名
        /// </summary>
        /// <param name="text">按钮名</param>
        /// <returns></returns>
        public WFMoveToBuilder Text(string text)
        {
            this.ButtonWidgetBuilder.Text(text);

            return this;
        }
        public WFMoveToBuilder Icon(string icon)
        {
            this.ButtonWidgetBuilder.Icon(icon);

            return this;
        }
        /// <summary>
        /// 需要滚动条
        /// </summary>
        /// <param name="status">true可用，false禁用</param>
        /// <returns></returns>
        public WFMoveToBuilder ProgressBar(bool progressBar)
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
