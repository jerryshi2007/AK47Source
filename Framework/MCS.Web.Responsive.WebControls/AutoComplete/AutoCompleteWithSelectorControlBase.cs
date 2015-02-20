using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web.UI;
using System.Collections;
using System.ComponentModel;
using System.Web.UI.HtmlControls;
using MCS.Web.Library.Script;
using System.Web.UI.WebControls;
using MCS.Library.Globalization;
using MCS.Web.Responsive.Library.Script;

[assembly: WebResource("MCS.Web.WebControls.AutoComplete.selectObjectDialog.htm", "text/html")]

namespace MCS.Web.Responsive.WebControls
{
    /// <summary>
    /// 自动完成以及附加选择器控件的基类。
    /// 自动完成部分是一个Html Editor。会附加一个图标，用于弹出窗口选择数据
    /// </summary>
    public abstract class AutoCompleteWithSelectorControlBase : ScriptControlBase
    {
        private HtmlGenericControl ctlHtmlSpan = new HtmlGenericControl("DIV");
        private HtmlImage foImg = new HtmlImage();
        private HtmlImage foHourGlass = new HtmlImage();

        private HtmlImage foBtn = new HtmlImage();
        /// <summary>
        /// 
        /// </summary>
        protected AutoCompleteExtender _AutoCompleteExtender = new AutoCompleteExtender();

        /// <summary>
        /// 构造方法
        /// </summary>
        /// <param name="enableClientState"></param>
        /// <param name="tag"></param>
        protected AutoCompleteWithSelectorControlBase(bool enableClientState, HtmlTextWriterTag tag) :
            base(enableClientState, tag)
        {
        }

        /// <summary>
        /// autocomplete客户端ID
        /// </summary>
        /// <remarks>
        ///autocomplete客户端ID
        /// </remarks>
        [Browsable(false)]
        [Description("autocomplete客户端ID")]
        [ScriptControlProperty]//设置此属性要输出到客户端
        [ClientPropertyName("autoCompleteID")]//对应的客户端属性
        protected string AutoCompleteID
        {
            get
            {
                return _AutoCompleteExtender.ClientID;
            }
        }

        /// <summary>
        /// 控件显示的文本
        /// </summary>
        /// <remarks>
        /// 控件显示的文本
        /// </remarks>
        [Description("控件显示的文本")]
        [ScriptControlProperty]//设置此属性要输出到客户端
        [ClientPropertyName("text")]//对应的客户端属性
        public string Text
        {
            get { return GetPropertyValue<string>("Text", string.Empty); }
            set { SetPropertyValue<string>("Text", value); }
        }

        /// <summary>
        /// 回调时的上下文，由使用者提供
        /// </summary>
        [ScriptControlProperty]
        [DefaultValue("")]
        [ClientPropertyName("callBackContext")]//对应的客户端属性
        [Bindable(true), Description("回调时的上下文，由使用者提供")]
        public virtual string CallBackContext
        {
            get { return GetPropertyValue<string>("callBackContext", null); }
            set
            {
                SetPropertyValue<string>("callBackContext", value);
                if (this._AutoCompleteExtender != null)
                {
                    this._AutoCompleteExtender.CallBackContext = value;
                    this._AutoCompleteExtender.EventContext = value;
                }
            }
        }

        /// <summary>
        /// 是否只读
        /// </summary>
        [DefaultValue(false)]
        [Category("Appearance")]
        [Description("是否只读")]
        [ClientPropertyName("readOnly")]
        [ScriptControlProperty]
        public override bool ReadOnly
        {
            get { return base.ReadOnly; }
            set { base.ReadOnly = value; }
        }

        /// <summary>
        /// 控件是否可用
        /// </summary>
        /// <remarks>
        /// 控件是否可用
        /// </remarks>
        [Description("控件是否禁用")]
        [ScriptControlProperty]//设置此属性要输出到客户端
        [ClientPropertyName("enabled")]//对应的客户端属性
        public override bool Enabled
        {
            get { return GetPropertyValue<bool>("enabled", true); }
            set { SetPropertyValue<bool>("enabled", value); }
        }

        /// <summary>
        /// 控件整体应用的CSS类名
        /// </summary>
        /// <remarks>
        /// 控件整体应用的CSS类名
        /// </remarks>
        [Description("控件整体应用的CSS类名")]
        [ScriptControlProperty]//设置此属性要输出到客户端
        [ClientPropertyName("className")]//对应的客户端属性
        public string ClassName
        {
            get { return GetPropertyValue<string>("ClassName", ""); }
            set { SetPropertyValue<string>("ClassName", value); }
        }

        /// <summary>
        /// 输入错误的项目应用的CSS
        /// </summary>
        /// <remarks>
        /// 输入错误的项目应用的CSS
        /// </remarks>
        [Description("输入错误的项目应用的CSS")]
        [ScriptControlProperty]//设置此属性要输出到客户端
        [ClientPropertyName("itemErrorCssClass")]//对应的客户端属性
        public string ItemErrorCssClass
        {
            get { return GetPropertyValue<string>("ItemErrorCssClass", ""); }
            set { SetPropertyValue<string>("ItemErrorCssClass", value); }
        }

        /// <summary>
        /// 正常的已验证项目应用的CSS
        /// </summary>
        /// <remarks>
        /// 正常的已验证项目应用的CSS
        /// </remarks>
        [Description("正常的已验证项目应用的CSS")]
        [ScriptControlProperty]//设置此属性要输出到客户端
        [ClientPropertyName("itemCssClass")]//对应的客户端属性
        public string ItemCssClass
        {
            get { return GetPropertyValue<string>("ItemCssClass", ""); }
            set { SetPropertyValue<string>("ItemCssClass", value); }
        }

        /// <summary>
        /// 被选择的项目应用的CSS
        /// </summary>
        /// <remarks>
        /// 被选择的项目应用的CSS
        /// </remarks>
        [Description("被选择的项目应用的CSS")]
        [ScriptControlProperty]//设置此属性要输出到客户端
        [ClientPropertyName("selectItemCssClass")]//对应的客户端属性
        public string SelectItemCssClass
        {
            get { return GetPropertyValue<string>("SelectItemCssClass", ""); }
            set { SetPropertyValue<string>("SelectItemCssClass", value); }
        }

        /// <summary>
        /// 是否显示检查按钮
        /// </summary>
        [Browsable(true)]
        [Description("是否显示检查按钮")]
        [DefaultValue(true)]
        [ScriptControlProperty]
        [ClientPropertyName("showCheckIcon")]//对应的客户端属性
        public bool ShowCheckButton
        {
            get
            {
                return GetPropertyValue("ShowCheckButton", true);
            }

            set
            {
                SetPropertyValue("ShowCheckButton", value);
            }
        }

        /// <summary>
        /// 是否显示选择按钮
        /// </summary>
        [Browsable(true)]
        [Description("是否显示选择按钮")]
        [DefaultValue(true)]
        [ScriptControlProperty]
        [ClientPropertyName("showSelector")]//对应的客户端属性
        public bool ShowSelector
        {
            get
            {
                return GetPropertyValue("ShowSelector", true);
            }

            set
            {
                SetPropertyValue("ShowSelector", value);
            }
        }

        /// <summary>
        /// 弹出下拉列表宽度
        /// </summary>
        [Browsable(true)]
        [Description("弹出下拉列表宽度")]
        [DefaultValue(true)]
        [ScriptControlProperty]
        [ClientPropertyName("popupListWidth")]//对应的客户端属性
        public int PopupListWidth
        {
            get
            {
                return GetPropertyValue("PopupListWidth", 0);
            }

            set
            {
                SetPropertyValue("PopupListWidth", value);
            }
        }

        /// <summary>
        /// 
        /// </summary>
        [Browsable(false)]
        [ScriptControlProperty]
        [ClientPropertyName("inputAreaClientID")]
        protected string InputAreaClientID
        {
            get
            {
                return this.ctlHtmlSpan.ClientID;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        [Browsable(false)]
        [Description("检查对象合法性的图片的客户端ID")]
        [ScriptControlProperty]
        [ClientPropertyName("checkOguUserImageClientID")]
        protected string CheckOguUserImageClientID
        {
            get
            {
                return foImg.ClientID;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        [Browsable(false)]
        [Description("HourGlass图片的客户端ID")]
        [ScriptControlProperty]//设置此属性要输出到客户端
        [ClientPropertyName("hourglassImageClientID")]//对应的客户端属性
        protected string HourglassImageClientID
        {
            get
            {
                return foHourGlass.ClientID;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        [Browsable(false)]
        [ScriptControlProperty]
        [ClientPropertyName("ouBtnClientID")]
        protected string OuBtnClientID
        {
            get
            {
                return this.foBtn.ClientID;
            }
        }
        /// <summary>
        /// 执行检查录入项目的图标
        /// </summary>
        /// <remarks>
        /// 执行检查录入项目的图标
        /// </remarks>
        [Description("执行检查录入项目的图标")]
        [ScriptControlProperty]
        [ClientPropertyName("checkImg")]
        protected virtual string CheckImg
        {
            get
            {
                return GetPropertyValue<string>("CheckImg",
                    Page.ClientScript.GetWebResourceUrl(typeof(AutoCompleteWithSelectorControlBase),
                        "MCS.Web.Responsive.WebControls.AutoComplete.check.gif"));
            }
        }

        /// <summary>
        /// SelectObjectDialogUrl
        /// </summary>
        [ScriptControlProperty]
        [ClientPropertyName("selectObjectDialogUrl")]
        protected virtual string SelectObjectDialogUrl
        {
            get
            {
                return Page.ClientScript.GetWebResourceUrl(typeof(AutoCompleteWithSelectorControlBase),
                    "MCS.Web.Responsive.WebControls.AutoComplete.selectObjectDialog.htm");
            }
        }

        /// <summary>
        /// 选择数据的图标
        /// </summary>
        [Description("选择数据的图标")]
        [ScriptControlProperty]//设置此属性要输出到客户端
        [ClientPropertyName("selectorImg")]//对应的客户端属性
        protected virtual string SelectorImg
        {
            get
            {
                return GetPropertyValue<string>("SelectorImg",
                    Page.ClientScript.GetWebResourceUrl(typeof(AutoCompleteWithSelectorControlBase),
                        "MCS.Web.Responsive.WebControls.AutoComplete.ou.gif"));
            }
        }

        /// <summary>
        /// 
        /// </summary>
        [Description("沙漏的图标")]
        [ScriptControlProperty]
        [ClientPropertyName("hourglassImg")]
        protected string HourglassImg
        {
            get
            {
                return GetPropertyValue<string>("HourglassImg",
                    Page.ClientScript.GetWebResourceUrl(typeof(AutoCompleteWithSelectorControlBase),
                        "MCS.Web.Responsive.WebControls.AutoComplete.hourglass.gif"));
            }
        }

        /// <summary>
        /// 
        /// </summary>
        [ScriptControlProperty]
        [ClientPropertyName("checkText")]
        protected string CheckText
        {
            get
            {
                return GetPropertyValue<string>("CheckText",
                    Translator.Translate(Define.DefaultCategory, "检查..."));
            }
        }

        /// <summary>
        /// 
        /// </summary>
        [ScriptControlProperty]
        [ClientPropertyName("checkingText")]
        protected string CheckingText
        {
            get
            {
                return GetPropertyValue<string>("CheckingText",
                    Translator.Translate(Define.DefaultCategory, "正在检查"));
            }
        }

        /// <summary>
        /// 数据类型
        /// </summary>
        [Description("数据类型")]
        [ScriptControlProperty]//设置此属性要输出到客户端
        [ClientPropertyName("dataType")]//对应的客户端属性
        public virtual string DataType
        {
            get { return GetPropertyValue<string>("DataType", ""); }
            set
            {
                SetPropertyValue<string>("DataType", value);
            }
        }

        /// <summary>
        /// 是否可以多选
        /// </summary>
        /// <remarks>
        /// 是否可以多选
        /// </remarks>
        [Description("是否可以多选  Single:单选   Multiple:多选")]
        [ScriptControlProperty]//设置此属性要输出到客户端
        [ClientPropertyName("multiSelect")]//对应的客户端属性
        public virtual bool MultiSelect
        {
            get { return GetPropertyValue<bool>("MultiSelect", true); }
            set
            {
                SetPropertyValue<bool>("MultiSelect", value);
            }
        }

        /// <summary>
        /// 是否允许选择重复的对象
        /// </summary>
        [Description("是否允许选择重复的对象")]
        [ScriptControlProperty]//设置此属性要输出到客户端
        [ClientPropertyName("allowSelectDuplicateObj")]//对应的客户端属性
        public bool AllowSelectDuplicateObj
        {
            get { return GetPropertyValue<bool>("AllowSelectDuplicateObj", false); }
            set { SetPropertyValue<bool>("AllowSelectDuplicateObj", value); }
        }

        /// <summary>
        /// 客户端选择的对象改变时的时间
        /// </summary>
        [DefaultValue("")]
        [ScriptControlEvent]
        [ClientPropertyName("selectedDataChanged")]
        [Bindable(true), Category("ClientEventsHandler"), Description("客户端选择的数据改变后触发的事件")]
        public string OnClientSelectedDataChanged
        {
            get
            {
                return GetPropertyValue("OnClientSelectedDataChanged", string.Empty);
            }

            set
            {
                SetPropertyValue("OnClientSelectedDataChanged", value);
            }
        }


        /// <summary>
        /// 客户端选择数据时的事件
        /// </summary>
        [DefaultValue("")]
        [ScriptControlEvent]
        [ClientPropertyName("selectData")]
        [Bindable(true), Category("ClientEventsHandler"), Description("客户端选择数据时的事件")]
        public string OnClientSelectData
        {
            get
            {
                return GetPropertyValue("OnClientSelectData", string.Empty);
            }

            set
            {
                SetPropertyValue("OnClientSelectData", value);
            }
        }

        /// <summary>
        /// 数据检索字段名称，支持逗号分割
        /// </summary>
        [DefaultValue("")]
        [Bindable(true), Description("数据检索字段名称")]
        protected virtual string DataCompareFields
        {
            get
            {
                return "";
            }
        }

        /// <summary>
        /// 数据显示字段名称，支持逗号分割
        /// </summary>
        [Bindable(true), Description("数据显示字段名称")]
        public virtual string DataTextFields
        {
            get
            {
                return GetPropertyValue("DataTextField", ClientDataDisplayPropName);
            }

            set
            {
                SetPropertyValue("DataTextField", value);
            }
        }

        /// <summary>
        /// 客户端数据对象Key名称
        /// </summary>
        [DefaultValue("id")]
        [ScriptControlProperty]
        [ClientPropertyName("dataKeyName")]
        [Bindable(true), Description("客户端数据对象Key名称")]
        public virtual string ClientDataKeyName
        {
            get
            {
                return GetPropertyValue("ClientDataKeyName", "id");
            }

            set
            {
                SetPropertyValue("ClientDataKeyName", value);
            }
        }

        /// <summary>
        /// 客户端数据对象显示属性名称
        /// </summary>
        [DefaultValue("displayName")]
        [ScriptControlProperty]
        [ClientPropertyName("dataDisplayPropName")]
        [Bindable(true), Description("客户端数据对象显示属性名称")]
        public virtual string ClientDataDisplayPropName
        {
            get
            {
                return GetPropertyValue("ClientDataDisplayPropName", "displayName");
            }

            set
            {
                SetPropertyValue("ClientDataDisplayPropName", value);
            }
        }

        /// <summary>
        /// 客户端数据对象显示属性名称
        /// </summary>
        [DefaultValue("description")]
        [ScriptControlProperty]
        [ClientPropertyName("dataDescriptionPropName")]
        [Bindable(true), Description("客户端数据对象显示属性名称")]
        public virtual string ClientDataDescriptionPropName
        {
            get
            {
                return GetPropertyValue("ClientDataDescriptionPropName", "description");
            }

            set
            {
                SetPropertyValue("ClientDataDescriptionPropName", value);
            }
        }

        /// <summary>
        /// 检查输入的回调方法名称
        /// </summary>
        [ScriptControlProperty]
        [ClientPropertyName("checkInputCallBackMethod")]
        public abstract string CheckInputCallBackMethod
        {
            get;
        }


        /// <summary>
        /// 
        /// </summary>
        protected IAttributeAccessor TreeSelectorButton
        {
            get
            {
                return this.foBtn;
            }
        }

        /// <summary>
        /// OnInit
        /// </summary>
        /// <param name="e"></param>
        protected override void OnInit(EventArgs e)
        {
            StaticCallBackProxy.Instance.TargetControlLoaded += new StaticCallBackProxyControlLoadedEventHandler(InnerAutoCompleteControl_TargetControlLoaded);
            base.OnInit(e);

            EnsureChildControls();
        }

        /// <summary>
        /// OnPreRender
        /// </summary>
        /// <param name="e"></param>
        protected override void OnPreRender(EventArgs e)
        {
            if (this.ReadOnly)
            {
                ctlHtmlSpan.Attributes["contentEditable"] = "false";
                ctlHtmlSpan.Style["border-width"] = "0px";
            }
            else
                ctlHtmlSpan.Attributes["contentEditable"] = "true";

            _AutoCompleteExtender.IsAutoComplete = !this.ReadOnly;

            if (this.Enabled == false || this.ReadOnly || this.ShowCheckButton == false)
            {
                foImg.Style["display"] = "none";
            }

            if (this.Enabled == false || this.ReadOnly || ShowSelector == false)
            {
                foBtn.Style["display"] = "none";
            }

            base.OnPreRender(e);
        }

        /// <summary>
        /// Render
        /// </summary>
        /// <param name="writer"></param>
        protected override void Render(HtmlTextWriter writer)
        {
            if (DesignMode)
            {
                Controls.Clear();
                TextBox tb = new TextBox();
                Controls.Add(tb);

                Image checkImg = new Image();
                checkImg.ImageUrl = CheckImg;
                Controls.Add(checkImg);

                Image selectorImg = new Image();
                selectorImg.ImageUrl = SelectorImg;
                Controls.Add(selectorImg);

                base.Render(writer);
            }
            else
            {
                writer.AddAttribute(HtmlTextWriterAttribute.Class, "autodrop propertyeditor");
                base.Render(writer);
            }
        }

        /// <summary>
        /// 创建子控件
        /// </summary>
        protected override void CreateChildControls()
        {
            if (!DesignMode)
                InitInputTable();

            InitAutoCompleteExtender();

            base.CreateChildControls();
        }

        private void InitInputTable()
        {
            var container = new HtmlGenericControl("div");
            container.Attributes["class"] = "autocomplete-layout ";
            this.Controls.Add(container);
            HtmlGenericControl cell;

            ctlHtmlSpan.ID = "inputArea";
            ctlHtmlSpan.Attributes["class"] = "autocomplete-input";


            cell = new HtmlGenericControl("div");
            cell.Attributes["class"] = "autocomplete-cell";

            //ctlHtmlSpan.Attributes["class"] = sClass;
            //ctlHtmlSpan.Style["width"] = "98%";

            HtmlGenericControl htmlInputContainer = new HtmlGenericControl("div");
            htmlInputContainer.Attributes["class"] = "autocomplete-input-container form-control";
            cell.Controls.Add(htmlInputContainer);
            htmlInputContainer.Controls.Add(ctlHtmlSpan);
            container.Controls.Add(cell);

            foImg.Src = this.CheckImg;
            foImg.ID = "chkUser";
            foImg.Style["cursor"] = "pointer";
            foImg.Attributes["title"] = Translator.Translate(Define.DefaultCategory, "检查...");
            //foImg.Attributes["onclick"] = string.Format("$find('{0}')._checkInput();", this.ClientID);

            if (ShowCheckButton)
            {
                cell = new HtmlGenericControl("div");
                cell.Attributes["class"] = "autocomplete-cell";
                cell.Controls.Add(foImg);
                cell.Style["width"] = "17px";

                container.Controls.Add(cell);
            }

            //沈峥添加
            foHourGlass.Src = this.Page.ClientScript.GetWebResourceUrl(typeof(AutoCompleteWithSelectorControlBase),
                "MCS.Web.Responsive.WebControls.AutoComplete.hourglass.gif");

            foHourGlass.Attributes["title"] = Translator.Translate(Define.DefaultCategory, "正在检查");
            foHourGlass.ID = "hourglass";
            foHourGlass.Style["display"] = "none";
            foImg.Style["cursor"] = "pointer";

            cell.Controls.Add(foHourGlass);

            foBtn.Src = this.SelectorImg;
            foBtn.ID = "lnkbtn";
            foBtn.Style["cursor"] = "pointer";

            if (this.Enabled == false || this.ReadOnly || ShowSelector == false)
            {
                foBtn.Style["display"] = "none";
            }

            container.Controls.Add(cell);

            cell = new HtmlGenericControl("div");
            cell.Attributes["class"] = "autocomplete-cell";
            cell.Controls.Add(foBtn);
            cell.Style["width"] = "17px";
            container.Controls.Add(cell);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sPrefix"></param>
        /// <param name="iCount"></param>
        /// <param name="context"></param>
        /// <param name="result"></param>
        protected abstract void AutoCompleteExtender_GetDataSource(string sPrefix, int iCount, object context, ref IEnumerable result);

        private void InnerAutoCompleteControl_TargetControlLoaded(Control targetControl)
        {
            if (targetControl is AutoCompleteExtender && targetControl.ID.IndexOf("oACE") > 0)
            {
                AutoCompleteExtender oAutoComp = (AutoCompleteExtender)targetControl;
                oAutoComp.GetDataSource += new AutoCompleteExtender.GetDataSourceDelegate(AutoCompleteExtender_GetDataSource);
            }
        }

        private void InitAutoCompleteExtender()
        {
            _AutoCompleteExtender.ID = "oACE";
            _AutoCompleteExtender.TargetControlID = "inputArea";
            //_AutoCompleteExtender.OnItemSelected = string.Format("function(sender, e){{$find('{0}')._autoCompleteItemClick(sender, e);}}", this.ClientID);
            _AutoCompleteExtender.IsAutoComplete = true;
            _AutoCompleteExtender.RequireValidation = false;
            _AutoCompleteExtender.CompareFieldName = DataCompareFields.Split(',');
            _AutoCompleteExtender.MinimumPrefixLength = 1;
            _AutoCompleteExtender.DataValueField = "ID";
            _AutoCompleteExtender.DataTextFieldList = DataTextFields.Split(',');
            _AutoCompleteExtender.DataTextFormatString = "{0}  {1}";
            _AutoCompleteExtender.AutoCallBack = true;

            _AutoCompleteExtender.GetDataSource += new AutoCompleteExtender.GetDataSourceDelegate(AutoCompleteExtender_GetDataSource);

            this.Controls.Add(_AutoCompleteExtender);
        }
    }
}
