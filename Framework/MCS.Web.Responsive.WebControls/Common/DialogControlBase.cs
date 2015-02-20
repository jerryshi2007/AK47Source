using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Drawing.Design;
using System.Reflection;
using System.Text;
using System.Web;
using System.Web.UI;
using System.Web.UI.HtmlControls;
using MCS.Library.Core;
using MCS.Library.Globalization;
using MCS.Web.Library.Script;
using MCS.Web.Responsive.Library;
using MCS.Web.Responsive.Library.Script;

namespace MCS.Web.Responsive.WebControls
{
    /// <summary>
    /// 对话框类的控件基类。
    /// 该控件定义了基本对话框的参数处理，加载了基本的模板
    /// </summary>
    public abstract class DialogControlBase : ScriptControlBase, INamingContainer
    {
        private const string TicketParamName = "t";

        public event EventHandler BeforeNormalPreRender;
        public event LoadingDialogContentEventHander LoadingDialogContent;

        /// <summary>
        /// 构造方法
        /// </summary>
        protected DialogControlBase()
            : base(true, HtmlTextWriterTag.Div)
        {
        }

        #region Properties
        [Description("特殊标识")]
        [DefaultValue("")]
        public string Tag
        {
            get
            {
                return GetPropertyValue<string>("Tag", string.Empty);
            }
            set
            {
                SetPropertyValue("Tag", value);
            }
        }

        [DefaultValue("")]
        public string Category
        {
            get
            {
                return GetPropertyValue<string>("Category", string.Empty);
            }
            set
            {
                SetPropertyValue("Category", value);
            }
        }

        /// <summary>
        /// 对话框的标题
        /// </summary>
        [DefaultValue("")]
        [ScriptControlProperty(), ClientPropertyName("dialogTitle")]
        public virtual string DialogTitle
        {
            get
            {
                return GetPropertyValue<string>("DialogTitle", string.Empty);
            }
            set
            {
                SetPropertyValue("DialogTitle", value);
            }
        }

        /// <summary>
        /// 对话框的宽度
        /// </summary>
        [DefaultValue("360px")]
        [ScriptControlProperty(), ClientPropertyName("dialogWidth")]
        public virtual string DialogWidth
        {
            get
            {
                return GetPropertyValue<string>("DialogWidth", "360px");
            }
            set
            {
                SetPropertyValue("DialogWidth", value);
            }
        }

        /// <summary>
        /// 对话框的高度
        /// </summary>
        [DefaultValue("400px")]
        [ScriptControlProperty(), ClientPropertyName("dialogHeight")]
        public virtual string DialogHeight
        {
            get
            {
                return GetPropertyValue<string>("DialogHeight", "400px");
            }
            set
            {
                SetPropertyValue("DialogHeight", value);
            }
        }

        /// <summary>
        /// 是否显示取消按钮
        /// </summary>
        [DefaultValue(true)]
        [ScriptControlProperty(), ClientPropertyName("showCancelButton")]
        public bool ShowCancelButton
        {
            get
            {
                return GetPropertyValue<bool>("ShowCancelButton", true);
            }
            set
            {
                SetPropertyValue("ShowCancelButton", value);
            }
        }

        /// <summary>
        /// 取消按钮文字
        /// </summary>
        [DefaultValue("取消")]
        [ScriptControlProperty(), ClientPropertyName("cancelButtonText")]
        public string CancelButtonText
        {
            get
            {
                return this.Translate(GetPropertyValue<string>("CancelButtonText", "取消"));
            }
            set
            {
                SetPropertyValue("CancelButtonText", value);
            }
        }

        /// <summary>
        /// 确定按钮文字
        /// </summary>
        [DefaultValue("确定")]
        [ScriptControlProperty(), ClientPropertyName("confirmButtonText")]
        public string ConfirmButtonText
        {
            get
            {
                return this.Translate(GetPropertyValue<string>("ConfirmButtonText", "确定"));
            }
            set
            {
                SetPropertyValue("ConfirmButtonText", value);
            }
        }

        /// <summary>
        /// 是否显示确定按钮
        /// </summary>
        [DefaultValue(true)]
        [ScriptControlProperty(), ClientPropertyName("showConfirmButton")]
        public bool ShowConfirmButton
        {
            get
            {
                return GetPropertyValue<bool>("ShowConfirmButton", true);
            }
            set
            {
                SetPropertyValue("ShowConfirmButton", value);
            }
        }

        /// <summary>
        /// 当前的显示模式（弹出对话框还是普通页面）
        /// </summary>
        [Browsable(false)]
        [ScriptControlProperty(), ClientPropertyName("currentMode")]
        public ControlShowingMode CurrentMode
        {
            get
            {
                ControlShowingMode result = ControlShowingMode.Normal;

                if (this.RenderMode.OnlyRenderSelf)
                    result = ControlShowingMode.Dialog;

                return result;
            }
        }

        /// <summary>
        /// 控件是当作对话框使用，还是显示在普通页面上
        /// </summary>
        [DefaultValue(ControlShowingMode.Dialog)]
        [ScriptControlProperty(), ClientPropertyName("showingMode")]
        public ControlShowingMode ShowingMode
        {
            get
            {
                return GetPropertyValue("ShowingMode", ControlShowingMode.Dialog);
            }
            set
            {
                SetPropertyValue("ShowingMode", value);
            }
        }

        /// <summary>
        /// 客户端弹出对话框的url
        /// </summary>
        [ScriptControlProperty(), ClientPropertyName("dialogUrl")]
        [Browsable(false)]
        public string DialogUrl
        {
            get
            {
                return GetDialogUrl();
            }
        }

        /// <summary>
        /// 控件在父页面上的ID
        /// </summary>
        [ScriptControlProperty(), ClientPropertyName("parentPageControlID")]
        [Browsable(false)]
        public string ParentPageControlID
        {
            get
            {
                string id = "";
                var pageRenderMode = Request.GetRequestPageRenderMode();

                if (pageRenderMode != null && pageRenderMode.RenderControlUniqueID != null)
                {
                    return pageRenderMode.RenderControlUniqueID.Replace("$", "_");
                }

                return id;
            }
        }

        /// <summary>
        /// 控件在对话框页面上的ID
        /// </summary>
        [ScriptControlProperty(), ClientPropertyName("dialogControlID")]
        [Browsable(false)]
        public string DialogControlID
        {
            get { return this.ID; }
        }

        #endregion

        #region protected

        protected virtual void OnLoadingDialogContent(object sender, LoadingDialogContentEventArgs e)
        {
            if (LoadingDialogContent != null)
                LoadingDialogContent(sender, e);
        }

        /// <summary>
        /// 在OnLoad执行之前
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected override void OnPagePreLoad(object sender, EventArgs e)
        {
            EnsureChildControls();
            base.OnPagePreLoad(sender, e);
        }

        /// <summary>
        /// 页面渲染之前
        /// </summary>
        /// <param name="e"></param>
        protected override void OnPreRender(EventArgs e)
        {
            if (this.CurrentMode == ControlShowingMode.Normal)
            {
                if (this.BeforeNormalPreRender != null)
                    this.BeforeNormalPreRender(this, e);
            }

            base.OnPreRender(e);

            if (this.CurrentMode == ControlShowingMode.Dialog)
            {
                DialogContentAttribute contentAttr = AttributeHelper.GetCustomAttribute<DialogContentAttribute>(this.GetType());

                if (contentAttr != null)
                    this.EnableViewState = contentAttr.EnableViewState;
            }
        }

        /// <summary>
        /// 创建子控件
        /// </summary>
        protected override void CreateChildControls()
        {
            base.CreateChildControls();

            if (this.CurrentMode == ControlShowingMode.Dialog)
            {
                Page.Response.Cache.SetCacheability(HttpCacheability.NoCache);

                Control template = LoadDialogTemplate();

                Controls.Add(template);

                OnDialogContentControlLoaded(template);
            }
        }

        protected virtual void OnDialogContentControlLoaded(Control control)
        {
        }

        /// <summary>
        /// 获得客户端显示对话框的url
        /// </summary>
        /// <returns></returns>
        protected virtual string GetDialogUrl()
        {
            PageRenderMode originalPageRenderMode = Request.GetRequestPageRenderMode();

            string controlID = null;

            if (originalPageRenderMode != null)
                controlID = ScriptControlHelper.GetPageUniqueID(originalPageRenderMode.PageCache);

            if (controlID.IsNotEmpty())
                controlID = controlID + "$" + this.UniqueID;
            else
                controlID = this.UniqueID;

            PageRenderMode pageRenderMode = new PageRenderMode(controlID, "DialogControl");

            string url = Request.GetRequestExecutionUrl(pageRenderMode);

            NameValueCollection originalParams = UriHelper.GetUriParamsCollection(url);

            originalParams.Remove(TicketParamName);

            return UriHelper.CombineUrlParams(url, originalParams);
        }

        /// <summary>
        /// 加载对话框的模板
        /// </summary>
        /// <returns></returns>
        protected virtual Control LoadDialogTemplate()
        {
            DialogContentAttribute contentAttr = AttributeHelper.GetCustomAttribute<DialogContentAttribute>(this.GetType());

            var contentHtml = "";

            if (contentAttr != null && string.IsNullOrEmpty(contentAttr.ResourcePath) == false)
            {
                Assembly assembly = Assembly.GetExecutingAssembly();

                if (string.IsNullOrEmpty(contentAttr.AssemblyName) == false)
                    assembly = Assembly.Load(contentAttr.AssemblyName);

                LoadingDialogContentEventArgs e = new LoadingDialogContentEventArgs();

                OnLoadingDialogContent(this, e);

                contentHtml = e.Content;

                if (contentHtml.IsNullOrEmpty())
                    contentHtml = ResourceHelper.LoadStringFromResource(assembly, contentAttr.ResourcePath);

            }

            contentHtml = ReplaceDialogTemplateString(contentHtml);

            return Page.ParseControl(contentHtml);
        }

        /// <summary>
        /// 重载此方法，可以替换已经加载的模板的内容
        /// </summary>
        /// <param name="html"></param>
        /// <returns></returns>
        protected virtual string ReplaceDialogTemplateString(string html)
        {
            return html;
        }

        #endregion

        #region private

        private string Translate(string sourceText)
        {
            string category = Define.DefaultCategory;

            if (string.IsNullOrEmpty(this.Category) == false)
                category = this.Category;

            return Translator.Translate(category, sourceText);
        }
        #endregion
    }
}
