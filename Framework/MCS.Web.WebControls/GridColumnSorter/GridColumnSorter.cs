using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Drawing.Design;
using MCS.Web.Library;
using MCS.Web.Library.Script;

[assembly: WebResource("MCS.Web.WebControls.GridColumnSorter.bannerbg.gif", "image/gif")]
[assembly: WebResource("MCS.Web.WebControls.GridColumnSorter.bgw.png", "image/png")]
[assembly: WebResource("MCS.Web.WebControls.GridColumnSorter.sorttag.png", "image/png")]

[assembly: WebResource("MCS.Web.WebControls.GridColumnSorter.GridColumnSorter.css", "text/css", PerformSubstitution = true)]
[assembly: WebResource("MCS.Web.WebControls.GridColumnSorter.GridColumnSorter.js", "application/x-javascript")]
namespace MCS.Web.WebControls
{
    /// <summary>
    /// 排序器停靠位置
    /// </summary>
    public enum SorterDockPosition
    {
        /// <summary>
        /// 居左
        /// </summary>
        Left = 1,
        /// <summary>
        /// 居右
        /// </summary>
        Right = 2
    }

    /// <summary>
    /// 表格列排序控件
    /// </summary>
    [DefaultProperty("Text")]
    [ToolboxData("<{0}:GridColumnSorter runat=server></{0}:GridColumnSorter>")]
    [ClientCssResource("MCS.Web.WebControls.GridColumnSorter.GridColumnSorter.css")]
    [ClientScriptResource("MCS.Web.WebControls.GridColumnSorter", "MCS.Web.WebControls.GridColumnSorter.GridColumnSorter.js")]
    //[ParseChildren(typeof(List<SortItem>))]
    public class GridColumnSorter : ScriptControlBase, IButtonControl
    {
        private string sortExpr; //内部使用，排序的表达式
        private SortDirection dir; //内部使用，排序的方向 
        private List<SortItem> sortItems = new List<SortItem>();
        private static readonly object EventClick;
        private static readonly object EventCommand;

        static GridColumnSorter()
        {
            EventClick = new object();
            EventCommand = new object();
        }

        /// <summary>
        /// 构造函数
        /// </summary>
        public GridColumnSorter()
            : base(true,HtmlTextWriterTag.Div)
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
        /// 排序器停靠位置
        /// </summary>
        [DefaultValue(SorterDockPosition.Right)]
        public SorterDockPosition DockPosition
        {
            get
            {
                return WebControlUtility.GetViewStateValue(ViewState, "DockPosition", SorterDockPosition.Right);
            }
            set
            {
                WebControlUtility.SetViewStateValue(ViewState, "DockPosition", value);
            }
        }

        /// <summary>
        /// SortItems
        /// </summary>
        [PersistenceMode(PersistenceMode.InnerProperty), Description("SortItems")]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
        [Browsable(false)]
        public List<SortItem> SortItems
        {
            get { return this.sortItems; }
        }

        /// <summary>
        /// 默认排序
        /// </summary>
        [DefaultValue("默认排序")]
        public string DefaultOrderName
        {
            get
			{
				return WebControlUtility.GetViewStateValue(ViewState, "DefaultOrderName", "默认排序");
			}
			set
			{
				WebControlUtility.SetViewStateValue(ViewState, "DefaultOrderName", value);
			}
        }

        /// <summary>
        /// 获取或设置一个值，表示是否应阻止排序菜单被显示出来
        /// </summary>
        [DefaultValue(false)]
        public bool PreventRenderChildren
        {
            get
            {
                return WebControlUtility.GetViewStateValue(ViewState, "PreventRenderChildren", false);
            }
            set
            {
                WebControlUtility.SetViewStateValue(ViewState, "PreventRenderChildren", value);
            }
        }

        /// <summary>
        /// 呈现内容
        /// </summary>
        /// <param name="output"></param>
        protected override void RenderContents(HtmlTextWriter output)
        {
            RenderCaption(output);
            if (!PreventRenderChildren)
                RenderDropList(output);
        }
        	
        /// <summary>
		/// 加载ClientState
		/// </summary>
		/// <param name="clientState">客户端状态</param>
        protected override void LoadClientState(string clientState)
        {
            if (string.IsNullOrEmpty(clientState) == false)
            {
                this.sortItems = JSONSerializerExecute.Deserialize<List<SortItem>>(clientState);
            }
        }

        /// <summary>
        /// 保存ClientState
        /// </summary>
        /// <returns>条件序列化的字符串</returns>
        protected override string SaveClientState()
        {
            string result = string.Empty;
            result = JSONSerializerExecute.Serialize(this.sortItems);
            return result;
        }

        private void RenderDropList(HtmlTextWriter output)
        {
            if (this.HasControls())
            {
                output.BeginRender();
                output.AddAttribute(HtmlTextWriterAttribute.Class, "mcsc-drop-container-wrapper");
                output.RenderBeginTag(HtmlTextWriterTag.Div);
                
                if (this.DockPosition == SorterDockPosition.Right)
                {
                    output.AddAttribute(HtmlTextWriterAttribute.Class, "mcsc-drop-container right");
                }
                else
                {
                    output.AddAttribute(HtmlTextWriterAttribute.Class, "mcsc-drop-container left");
                }

                output.RenderBeginTag(HtmlTextWriterTag.Div);
                output.AddAttribute(HtmlTextWriterAttribute.Class, "mcsc-banner");
                output.RenderBeginTag(HtmlTextWriterTag.Div);
                output.WriteEncodedText("选择排序方式");
                output.RenderEndTag();

                output.RenderBeginTag(HtmlTextWriterTag.Dl);
                foreach (Control ctrl in this.Controls)
                {
                    if (ctrl is LiteralControl)
                        continue; //忽略标记中的任何文本
                    output.RenderBeginTag(HtmlTextWriterTag.Dd);
                    ctrl.RenderControl(output);

                    output.RenderEndTag();
                }
                output.RenderEndTag();
                output.AddAttribute(HtmlTextWriterAttribute.Class, "mcsc-clear");
                output.RenderBeginTag(HtmlTextWriterTag.Div);
                output.RenderEndTag();
                output.RenderEndTag();
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

            if (base.IsEnabled)
            {
                var postBackOptions = this.GetPostBackOptions();
                string postBackEventReference = null;
                if (postBackOptions != null)
                {
                    postBackEventReference = this.Page.ClientScript.GetPostBackEventReference(postBackOptions, true);
                }
                if (string.IsNullOrEmpty(postBackEventReference))
                {
                    postBackEventReference = "javascript:void(0)";
                }
                output.AddAttribute(HtmlTextWriterAttribute.Href, postBackEventReference);
            }
            else
            {
                output.AddAttribute(HtmlTextWriterAttribute.Href, "javascript:void(0)");
            }
            output.RenderBeginTag(HtmlTextWriterTag.A);
            output.AddAttribute(HtmlTextWriterAttribute.Class, "mcsc-sort-rule");
            output.RenderBeginTag(HtmlTextWriterTag.Span);
            string sortName = this.DefaultOrderName;
            string sortDirCss = "";
            if (string.IsNullOrEmpty(this.sortExpr) == false)
            {
                sortName = GetSortDescriptionFor(sortExpr);
                if (sortName != null)
                {
                    if (this.dir == SortDirection.Ascending)
                        sortDirCss = "mcsc-sort-asc";
                    else
                        sortDirCss = "mcsc-sort-desc";
                }
                else
                {
                    sortName = this.DefaultOrderName;
                }
            }

            output.WriteEncodedText(sortName);
            output.AddAttribute(HtmlTextWriterAttribute.Class, sortDirCss);
            output.RenderBeginTag(HtmlTextWriterTag.I);
            output.RenderEndTag();//i
            output.RenderEndTag();//span
            output.RenderEndTag();//a
            output.RenderEndTag();//li
            output.RenderEndTag();//ul

        }

        private string GetSortDescriptionFor(string sortExpr)
        {
            string rst = null;
            foreach (Control ctrl in this.Controls)
            {
                if (ctrl is IButtonControl)
                {
                    IButtonControl button = (IButtonControl)ctrl;
                    if (button.CommandName == "Sort" && button.CommandArgument == sortExpr)
                    {
                        rst = button.Text;
                        break;
                    }
                }
            }
            return rst;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="e"></param>
        protected override void OnPreRender(EventArgs e)
        {
            this.dir = SortDirection.Ascending;
            this.sortExpr = null;
            GridView grid = FindOuterGrid(this.Parent);
            if (grid != null)
            {
                this.dir = grid.SortDirection;
                this.sortExpr = grid.SortExpression;
                this.CommandArgument = sortExpr;
            }

            base.OnPreRender(e);
        }

        private PostBackOptions GetPostBackOptions()
        {
            PostBackOptions options = new PostBackOptions(this, string.Empty)
            {
                RequiresJavaScriptProtocol = true
            };
            if (!string.IsNullOrEmpty(this.PostBackUrl))
            {
                options.ActionUrl = HttpUtility.UrlPathEncode(base.ResolveClientUrl(this.PostBackUrl));
                if ((!base.DesignMode && (this.Page != null)) && string.Equals(this.Page.Request.Browser.Browser, "IE", StringComparison.OrdinalIgnoreCase))
                {
                    options.ActionUrl = QuoteJScriptString(options.ActionUrl, true);
                }
            }
            if (this.CausesValidation && (this.Page.GetValidators(this.ValidationGroup).Count > 0))
            {
                options.PerformValidation = true;
                options.ValidationGroup = this.ValidationGroup;
            }
            return options;

        }

        /// <summary>
        /// 将JS脚本串作为引用字符串
        /// </summary>
        /// <param name="value"></param>
        /// <param name="forUrl"></param>
        /// <returns></returns>
        private static string QuoteJScriptString(string value, bool forUrl)
        {
            StringBuilder builder = null;
            if (string.IsNullOrEmpty(value))
            {
                return string.Empty;
            }
            int startIndex = 0;
            int count = 0;
            for (int i = 0; i < value.Length; i++)
            {
                switch (value[i])
                {
                    case '%':
                        {
                            if (!forUrl)
                            {
                                break;
                            }
                            if (builder == null)
                            {
                                builder = new StringBuilder(value.Length + 6);
                            }
                            if (count > 0)
                            {
                                builder.Append(value, startIndex, count);
                            }
                            builder.Append("%25");
                            startIndex = i + 1;
                            count = 0;
                            continue;
                        }
                    case '\'':
                        {
                            if (builder == null)
                            {
                                builder = new StringBuilder(value.Length + 5);
                            }
                            if (count > 0)
                            {
                                builder.Append(value, startIndex, count);
                            }
                            builder.Append(@"\'");
                            startIndex = i + 1;
                            count = 0;
                            continue;
                        }
                    case '\\':
                        {
                            if (builder == null)
                            {
                                builder = new StringBuilder(value.Length + 5);
                            }
                            if (count > 0)
                            {
                                builder.Append(value, startIndex, count);
                            }
                            builder.Append(@"\\");
                            startIndex = i + 1;
                            count = 0;
                            continue;
                        }
                    case '\t':
                        {
                            if (builder == null)
                            {
                                builder = new StringBuilder(value.Length + 5);
                            }
                            if (count > 0)
                            {
                                builder.Append(value, startIndex, count);
                            }
                            builder.Append(@"\t");
                            startIndex = i + 1;
                            count = 0;
                            continue;
                        }
                    case '\n':
                        {
                            if (builder == null)
                            {
                                builder = new StringBuilder(value.Length + 5);
                            }
                            if (count > 0)
                            {
                                builder.Append(value, startIndex, count);
                            }
                            builder.Append(@"\n");
                            startIndex = i + 1;
                            count = 0;
                            continue;
                        }
                    case '\r':
                        {
                            if (builder == null)
                            {
                                builder = new StringBuilder(value.Length + 5);
                            }
                            if (count > 0)
                            {
                                builder.Append(value, startIndex, count);
                            }
                            builder.Append(@"\r");
                            startIndex = i + 1;
                            count = 0;
                            continue;
                        }
                    case '"':
                        {
                            if (builder == null)
                            {
                                builder = new StringBuilder(value.Length + 5);
                            }
                            if (count > 0)
                            {
                                builder.Append(value, startIndex, count);
                            }
                            builder.Append("\\\"");
                            startIndex = i + 1;
                            count = 0;
                            continue;
                        }
                }
                count++;
            }
            if (builder == null)
            {
                return value;
            }
            if (count > 0)
            {
                builder.Append(value, startIndex, count);
            }
            return builder.ToString();

        }

        private GridView FindOuterGrid(Control parent)
        {
            if (parent == null)
            {
                throw new HttpException("GridColumnSorter控件外应该存在一个GridView");
            }
            else if (parent is GridView)
            {
                return parent as GridView;
            }
            else
            {
                return FindOuterGrid(parent.Parent);
            }
        }

        /// <summary>
        /// CreateChildControls
        /// </summary>
        protected override void CreateChildControls()
        {
            foreach (var sortItem in this.SortItems)
            {
                LinkButton btn=new LinkButton();
                btn.Text = sortItem.Text;
                btn.CommandName = this.CommandName;
                btn.CommandArgument = sortItem.SortExpression;

                this.Controls.Add(btn);
            }

            base.CreateChildControls();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="writer"></param>
        protected override void Render(HtmlTextWriter writer)
        {
            base.Render(writer);
        }

        /// <summary>
        /// CausesValidation
        /// </summary>
        [Themeable(false), DefaultValue(true), Description("Button_CausesValidation"), Category("Behavior")]
        public virtual bool CausesValidation
        {
            get
            {
                return WebControlUtility.GetViewStateValue(ViewState, "CausesValidation", true);
            }
            set
            {
                WebControlUtility.SetViewStateValue(ViewState, "CausesValidation", value);
            }
        }


        /// <summary>
        /// 在单击此控件的Text时发生。
        /// </summary>
        [Description("LinkButton_OnClick"), Category("Action")]
        public event EventHandler Click
        {
            add
            {
                base.Events.AddHandler(EventClick, value);
            }
            remove
            {
                base.Events.RemoveHandler(EventClick, value);
            }
        }

        /// <summary>
        /// 当单击此控件时发生
        /// </summary>
        [Category("Action"), Description("Button_OnCommand")]
        public event CommandEventHandler Command
        {
            add
            {
                base.Events.AddHandler(EventCommand, value);
            }
            remove
            {
                base.Events.RemoveHandler(EventCommand, value);
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="eventArgument"></param>
        protected override void RaisePostBackEvent(string eventArgument)
        {
            //base.ValidateEvent(this.UniqueID, eventArgument);
            if (this.CausesValidation)
            {
                this.Page.Validate(this.ValidationGroup);
            }
            this.OnClick(EventArgs.Empty);
            this.OnCommand(new CommandEventArgs(this.CommandName, this.CommandArgument));
        }

        /// <summary>
        /// 引发<see cref="Click"/>事件
        /// </summary>
        /// <param name="e">包含事件参数的<see cref="EventArgs"/></param>
        protected virtual void OnClick(EventArgs e)
        {
            EventHandler handler = (EventHandler)base.Events[EventClick];
            if (handler != null)
            {
                handler(this, e);
            }
        }

        /// <summary>
        /// 引发<see cref="Command"/>事件。
        /// </summary>
        /// <param name="e">包含事件参数的<see cref="CommandEventArgs"/></param>
        protected virtual void OnCommand(CommandEventArgs e)
        {
            CommandEventHandler handler = (CommandEventHandler)base.Events[EventCommand];
            if (handler != null)
            {
                handler(this, e);
            }
            base.RaiseBubbleEvent(this, e);
        }


        /// <summary>
        /// 命令名称
        /// </summary>
        [DefaultValue("Sort")]
        [Browsable(false)]
        public string CommandName
        {
            get
            {
                return "Sort";
            }
            set
            {
                throw new InvalidOperationException("不允许修改此属性");
            }
        }

        /// <summary>
        /// 
        /// </summary>
        [Themeable(false), DefaultValue(""), Bindable(true), Category("Behavior"), Description("WebControl_CommandArgument")]
        public string CommandArgument
        {
            get
            {
                return WebControlUtility.GetViewStateValue(ViewState, "CommandArgument", "");
            }
            set
            {
                WebControlUtility.SetViewStateValue(ViewState, "CommandArgument", value);
            }
        }

        /// <summary>
        /// 
        /// </summary>
        [Category("Behavior"), Editor("System.Web.UI.Design.UrlEditor, System.Design", typeof(UITypeEditor)), UrlProperty("*.aspx"), DefaultValue(""), Description("Button_PostBackUrl"), Themeable(false)]
        public virtual string PostBackUrl
        {
            get
            {
                return WebControlUtility.GetViewStateValue(ViewState, "PostBackUrl", "");
            }
            set
            {
                WebControlUtility.SetViewStateValue(ViewState, "PostBackUrl", value);
            }
        }

        /// <summary>
        /// 
        /// </summary>
        [DefaultValue(""), Bindable(true), Description("LinkButton_Text"), Category("Appearance"), PersistenceMode(PersistenceMode.InnerDefaultProperty), Localizable(true)]
        public virtual string Text
        {
            get
            {
                return WebControlUtility.GetViewStateValue(ViewState, "Text", "");
            }
            set
            {
                WebControlUtility.SetViewStateValue(ViewState, "Text", value);
            }
        }

        /// <summary>
        /// 
        /// </summary>
        [Description("PostBackControl_ValidationGroup"), Themeable(false), DefaultValue(""), Category("Behavior")]
        public virtual string ValidationGroup
        {
            get
            {
                return WebControlUtility.GetViewStateValue(ViewState, "ValidationGroup", "");
            }
            set
            {
                WebControlUtility.SetViewStateValue(ViewState, "ValidationGroup", value);
            }
          
        }
    }

    /// <summary>
    /// SortItem
    /// </summary>
    [Serializable]
    public class SortItem 
    {
        /// <summary>
        /// 文本
        /// </summary>
        public string Text
        {
            get;
            set;
        }

        /// <summary>
        /// 排序表达式
        /// </summary>
        public string SortExpression
        {
            get;
            set;
        }
    }
}
