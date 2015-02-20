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
    /// �Ի�����Ŀؼ����ࡣ
    /// �ÿؼ������˻����Ի���Ĳ������������˻�����ģ��
    /// </summary>
    public abstract class DialogControlBase : ScriptControlBase, INamingContainer
    {
        private const string TicketParamName = "t";

        public event EventHandler BeforeNormalPreRender;
        public event LoadingDialogContentEventHander LoadingDialogContent;

        /// <summary>
        /// ���췽��
        /// </summary>
        protected DialogControlBase()
            : base(true, HtmlTextWriterTag.Div)
        {
        }

        #region Properties
        [Description("�����ʶ")]
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
        /// �Ի���ı���
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
        /// �Ի���Ŀ��
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
        /// �Ի���ĸ߶�
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
        /// �Ƿ���ʾȡ����ť
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
        /// ȡ����ť����
        /// </summary>
        [DefaultValue("ȡ��")]
        [ScriptControlProperty(), ClientPropertyName("cancelButtonText")]
        public string CancelButtonText
        {
            get
            {
                return this.Translate(GetPropertyValue<string>("CancelButtonText", "ȡ��"));
            }
            set
            {
                SetPropertyValue("CancelButtonText", value);
            }
        }

        /// <summary>
        /// ȷ����ť����
        /// </summary>
        [DefaultValue("ȷ��")]
        [ScriptControlProperty(), ClientPropertyName("confirmButtonText")]
        public string ConfirmButtonText
        {
            get
            {
                return this.Translate(GetPropertyValue<string>("ConfirmButtonText", "ȷ��"));
            }
            set
            {
                SetPropertyValue("ConfirmButtonText", value);
            }
        }

        /// <summary>
        /// �Ƿ���ʾȷ����ť
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
        /// ��ǰ����ʾģʽ�������Ի�������ͨҳ�棩
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
        /// �ؼ��ǵ����Ի���ʹ�ã�������ʾ����ͨҳ����
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
        /// �ͻ��˵����Ի����url
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
        /// �ؼ��ڸ�ҳ���ϵ�ID
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
        /// �ؼ��ڶԻ���ҳ���ϵ�ID
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
        /// ��OnLoadִ��֮ǰ
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected override void OnPagePreLoad(object sender, EventArgs e)
        {
            EnsureChildControls();
            base.OnPagePreLoad(sender, e);
        }

        /// <summary>
        /// ҳ����Ⱦ֮ǰ
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
        /// �����ӿؼ�
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
        /// ��ÿͻ�����ʾ�Ի����url
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
        /// ���ضԻ����ģ��
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
        /// ���ش˷����������滻�Ѿ����ص�ģ�������
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
