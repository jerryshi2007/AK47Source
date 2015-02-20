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
    public abstract class WFButtonControlBuilderlBase<TViewComponent, TBuilder> : WFControlBuilderBase<TViewComponent, TBuilder, Button, ButtonBuilder>
        where TViewComponent : WFButtonControlBase
        where TBuilder : WFButtonControlBuilderlBase<TViewComponent, TBuilder>
    {
        public WFButtonControlBuilderlBase(TViewComponent component, HtmlHelper helper)
            : base(component, helper)
        {
        }

        /// <summary>
        /// 按钮的文本
        /// </summary>
        /// <param name="text">文本</param>
        /// <returns>构建器</returns>
        public TBuilder Text(string text)
        {
            this.ButtonWidgetBuilder.Text(text);

            return (TBuilder)this;
        }

        /// <summary>
        /// 设置大小模式
        /// </summary>
        /// <param name="sizeMode">较大，中大，很小</param>
        /// <returns></returns>
        public TBuilder SizeMode(Action<SizeModeBuilder> action)
        {
            this.ButtonWidgetBuilder.SizeMode(action);

            return (TBuilder)this;
        }

        /// <summary>
        /// 提示框内容，在EnableDialog为true，才起作用
        /// </summary>
        /// <param name="content">内容</param>
        /// <returns></returns>
        public TBuilder DialogText(string content)
        {
            this.ButtonWidgetBuilder.DialogText(content);

            return (TBuilder)this;
        }

        /// <summary>
        /// 样式类，支持所有BootStrap图标样式
        /// </summary>
        /// <param name="icon">样式类名</param>
        /// <returns></returns>
        public TBuilder Icon(string icon)
        {
            this.ButtonWidgetBuilder.Icon(icon);

            return (TBuilder)this;
        }

        /// <summary>
        /// 权限编码
        /// </summary>
        /// <param name="code">编码</param>
        /// <returns></returns>
        public TBuilder ResourceCode(string code)
        {
            this.ButtonWidgetBuilder.ResourceCode(code);

            return (TBuilder)this;
        }

        /// <summary>
        /// 按钮风格
        /// </summary>
        /// <param name="style">风格类型，如大按钮、小按钮等</param>
        /// <returns></returns>
        public TBuilder ButtonStyle(Action<ButtonStyleBuilder> action)
        {
            this.ButtonWidgetBuilder.ButtonStyle(action);

            return (TBuilder)this;
        }

        /// <summary>
        /// 按钮类型
        /// </summary>
        /// <param name="type">按钮或提交按钮</param>
        /// <returns></returns>
        public TBuilder ButtonType(Action<ButtonTypeBuilder> action)
        {
            this.ButtonWidgetBuilder.ButtonType(action);

            return (TBuilder)this;
        }

        /// <summary>
        /// 按钮状态
        /// </summary>
        /// <param name="status">true可用，false禁用</param>
        /// <returns></returns>
        public TBuilder Enabled(bool status)
        {
            this.ButtonWidgetBuilder.Enabled(status);

            return (TBuilder)this;
        }

        /// <summary>
        /// 需要滚动条
        /// </summary>
        /// <param name="status">true可用，false禁用</param>
        /// <returns></returns>
        public TBuilder ProgressBar(bool progressBar)
        {
            this.ButtonWidgetBuilder.ProgressBar(progressBar);

            return (TBuilder)this;
        }

        protected override WidgetBuilderBase<Button, ButtonBuilder> GetWidgetBuilder(HtmlHelper helper)
        {
            return new ButtonBuilder(helper);
        }

        protected ButtonBuilder ButtonWidgetBuilder
        {
            get
            {
                return (ButtonBuilder)base.WidgetBuilder;
            }
        }
    }
}
