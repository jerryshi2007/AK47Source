using System;
using System.Web.UI.WebControls;
using System.Web.UI;
using System.ComponentModel;
using System.ComponentModel.Design;
using System.Web.UI.WebControls.WebParts;
using ChinaCustoms.Framework.DeluxeWorks.Web.Library.Script;
#region [ Resources ]

[assembly: System.Web.UI.WebResource("ChinaCustoms.Framework.DeluxeWorks.Web.WebControls.Calendar.CalendarControl.js", "text/javascript")]
[assembly: System.Web.UI.WebResource("ChinaCustoms.Framework.DeluxeWorks.Web.WebControls.Calendar.CalendarControl.css", "text/css", PerformSubstitution = true)]
[assembly: System.Web.UI.WebResource("ChinaCustoms.Framework.DeluxeWorks.Web.WebControls.Calendar.Images.arrow-left.gif", "image/gif")]
[assembly: System.Web.UI.WebResource("ChinaCustoms.Framework.DeluxeWorks.Web.WebControls.Calendar.Images.arrow-right.gif", "image/gif")]
[assembly: System.Web.UI.WebResource("ChinaCustoms.Framework.DeluxeWorks.Web.WebControls.Calendar.Images.caption.gif", "image/gif")]
[assembly: System.Web.UI.WebResource("ChinaCustoms.Framework.DeluxeWorks.Web.WebControls.Calendar.Images.datePicker.gif", "image/gif")]
[assembly: System.Web.UI.WebResource("ChinaCustoms.Framework.DeluxeWorks.Web.WebControls.Calendar.Images.updown.gif", "image/gif")]
[assembly: System.Web.UI.WebResource("ChinaCustoms.Framework.DeluxeWorks.Web.WebControls.Calendar.Images.checked.gif", "image/gif")]
[assembly: System.Web.UI.WebResource("ChinaCustoms.Framework.DeluxeWorks.Web.WebControls.Calendar.Images.today.gif", "image/gif")]

#endregion

namespace ChinaCustoms.Framework.DeluxeWorks.Web.WebControls
{
    [RequiredScript(typeof(DeluxeAjaxScript), 0)]
    [RequiredScript(typeof(DateTimeScript), 1)]
    [RequiredScript(typeof(BlockingScript), 2)] //
    [RequiredScript(typeof(PopupControlScript), 3)]
    [RequiredScript(typeof(AnimationsScript), 4)]
    [RequiredScript(typeof(ThreadingScript), 5)]

    [ClientCssResource("ChinaCustoms.Framework.DeluxeWorks.Web.WebControls.Calendar.CalendarControl.css")]
    [ClientScriptResource("ChinaCustoms.Framework.DeluxeWorks.Web.WebControls.CalendarControl", "ChinaCustoms.Framework.DeluxeWorks.Web.WebControls.Calendar.CalendarControl.js")]
    public class PopupCalendarControl : Web.Library.Script.ScriptControlBase
    {
        // TODO: Add your property accessors here.
        //
        private string setTextBoxValue;
        public PopupCalendarControl()
            : base(false, HtmlTextWriterTag.Input)
        {
            this.Attributes.Add("type", "text");

        }

        /// <summary>
        /// 是否只显示当月
        /// </summary>
        ///<remarks>是否只显示当月</remarks>
        [Category("Default")]
        [ScriptControlProperty]
        [ClientPropertyName("isOnlyCurrentMonth")]
        [DescriptionAttribute("是否只显示当月")]
        public bool IsOnlyCurrentMonth
        {
            get { return GetPropertyValue("IsOnlyCurrentMonth", true); }
            set { SetPropertyValue("IsOnlyCurrentMonth", value); }
        }

        /// <summary>
        /// 日历的样式，不填为默认样式
        /// </summary>
        ///<remarks>日历的样式，不填为默认样式</remarks>
        [Category("Appearance")]
        [ScriptControlPropertyAttribute]
        [ClientPropertyName("cssClass")]
        [DescriptionAttribute("日历的样式，不填为默认样式")]
        public override string CssClass
        {
            get { return GetPropertyValue("CssClass", string.Empty); }
            set { SetPropertyValue("CssClass", value); }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <remarks></remarks>
        private Style style = new Style();
        [Category("Appearance")]
        [WebDescription("输入框样式")]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Content), NotifyParentProperty(true), PersistenceMode(PersistenceMode.InnerProperty)]
        public Style TextStyle
        {
            get { return style; }
            set { style = value; }
        }

        /// <summary>
        /// 日历文本框的Css
        /// </summary>
        /// <remarks>日历文本框的Css</remarks>
        [Category("Appearance")]
        [DescriptionAttribute("日历文本框的Css")]
        public string TextCssClass
        {
            get { return GetPropertyValue("TextCssClass", "ajax_calendartextbox"); }
            set { SetPropertyValue("TextCssClass", value); }
        }

        /// <summary>
        /// 图片的Style
        /// </summary>
        /// <remarks>图片的Style</remarks>
        //[ClientPropertyName]
        [Category("Appearance")]
        [DescriptionAttribute("图片的Style")]
        public string ImageStyle
        {
            get { return GetPropertyValue("ImageStyle", string.Empty); }
            set { SetPropertyValue("ImageStyle", value); }
        }

        /// <summary>
        /// 图片的CssClass
        /// </summary>
        /// <remarks>图片的CssClass</remarks>
        [Category("Appearance")]
        [DescriptionAttribute("图片的CssClass")]
        public string ImageCssClass
        {
            get { return GetPropertyValue("ButtonCssClass", "ajax_calendarimagebutton"); }
            set { SetPropertyValue("ButtonCssClass", value); }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <remarks></remarks>
        [Browsable(false)]
        [ScriptControlProperty]
        [DescriptionAttribute("按钮的ID")]
        public string MaskedEditButtonID
        {
            get { return GetPropertyValue("MaskedEditButtonID", this.UniqueID + "_image"); }
            set { SetPropertyValue("MaskedEditButtonID", value); }
        }

        /// <summary>
        /// 按钮图片的Src
        /// </summary>
        /// <remarks>按钮图片的Src</remarks>
        [Category("Appearance")]
        [DescriptionAttribute("按钮图片的Src")]
        public string ImageUrl
        {
            //get { return GetPropertyValue("ImageUrl", ""); }
            get { return GetPropertyValue("ImageUrl", Page.ClientScript.GetWebResourceUrl(typeof(PopupCalendarControl), "ChinaCustoms.Framework.DeluxeWorks.Web.WebControls.Calendar.Images.datePicker.gif")); }
            set { SetPropertyValue("ImageUrl", value); }
        }
        
        #region     日期掩码部分

        //[DefaultValue("9999-99-99")]
        //[ScriptControlProperty]
        //[DescriptionAttribute("日期格式串")]
        //public string Mask
        //{
        //    get { return GetPropertyValue("Mask", "9999-99-99"); }
        //    set { SetPropertyValue("Mask", value); }
        //}

        /// <summary>
        /// 掩码字符
        /// </summary>
        /// <remarks>掩码字符</remarks>
        [Category("Data")]
        [ScriptControlProperty]
        [DescriptionAttribute("掩码字符")]
        public string PromptCharacter
        {
            get { return GetPropertyValue("PromptCharacter","_"); }
            set { SetPropertyValue("PromptCharacter", value); }
        }

        /// <summary>
        /// 是否自动补齐日期
        /// </summary>
        /// <remarks>是否自动补齐日期</remarks>
        [Category("Default")]
        [ScriptControlProperty]
        [Description("是否自动补齐日期")]
        public bool AutoComplete
        {
            get { return GetPropertyValue("AutoComplete", true); }
            set { SetPropertyValue("AutoComplete", value); }
        }

        /// <summary>
        /// 提供自动补齐的时间串，不设置则取系统日期
        /// </summary>
        /// <remarks>提供自动补齐的时间串，不设置则取系统日期</remarks>
        [Category("Data")]
        [ScriptControlProperty]
        [Description("提供自动补齐的时间串，不设置则取系统日期")]
        public string AutoCompleteValue
        {
            get { return GetPropertyValue("AutoCompleteValue", string.Empty); }
            set { SetPropertyValue("AutoCompleteValue", value); }
        }

        /// <summary>
        /// 是否启用验证
        /// </summary>
        /// <remarks>是否启用验证</remarks>
        [Category("Default")]
        [ScriptControlProperty]
        [Description("是否启用验证")]
        public bool IsValidValue
        {
            get { return GetPropertyValue("IsValidValue", true); }
            set { SetPropertyValue("IsValidValue", value); }
        }

        /// <summary>
        /// 验证日期的提示信息
        /// </summary>
        /// <remarks>验证日期的提示信息</remarks>
        [Category("Data")]
        [ScriptControlProperty]
        [DescriptionAttribute("验证日期的提示信息")]
        public string CurrentMessageError
        {
            get { return GetPropertyValue("CurrentMessageError", "输入错误！"); }
            set { SetPropertyValue("CurrentMessageError", value); }
        }

        /// <summary>
        /// 得到焦点时文本框的样式
        /// </summary>
        /// <remarks>得到焦点时文本框的样式</remarks>
        [Category("Appearance")]
        [ScriptControlProperty]
        [DescriptionAttribute("得到焦点时文本框的样式")]
        public string OnFocusCssClass
        {

            get { return GetPropertyValue("OnFocusCssClass", "MaskedEditFocus"); }
            set
            {
                SetPropertyValue("OnFocusCssClass", value);
            }

        }

        /// <summary>
        /// 验证不通过时文本框的样式
        /// </summary>
        /// <remarks>验证不通过时文本框的样式</remarks>
        [Category("Appearance")]
        [ScriptControlProperty]
        [DescriptionAttribute("验证不通过时文本框的样式")]
        public string OnInvalidCssClass
        {

            get { return GetPropertyValue("OnInvalidCssClass", "MaskedEditError"); ; }
            set
            {

                SetPropertyValue("OnInvalidCssClass", value);
            }

        }


        #endregion
        protected override void OnPreRender(System.EventArgs e)
        {
            base.Attributes.Add("name", this.UniqueID);
            base.Attributes.Add("value", CValue);

            base.OnPreRender(e);
        }

        protected override void Render(HtmlTextWriter writer)
        {
            this.ApplyStyle(TextStyle);
            base.Attributes.Add("class", TextCssClass);
            base.Render(writer);
            writer.Write("<input type='image' id='{0}' src='{1}' class='{2}' style='{3}' align='absmiddle' />", this.UniqueID + "_image", ImageUrl, ImageCssClass,ImageStyle);           
        }

        /// <summary>
        /// 设置或是获取日历的值
        /// </summary>
        /// <remarks>设置或是获取日历的值</remarks>
        [Category("Default")]
        [ClientPropertyName("cValue")]
        [WebDisplayName("设置或是获取日历的值")]
        [DescriptionAttribute("设置或是获取日历的值")]
        public virtual string CValue
        {
            get
            {
                if (this.setTextBoxValue != null)
                {
                    return this.setTextBoxValue;
                }
                else
                {
                    if (Page.Request.Form[this.UniqueID] != null)
                        return Page.Request.Form[this.UniqueID];
                    else
                        return null;
                }
            }
            set
            {
                this.setTextBoxValue = value;
            }
        }

        //[DefaultValue("d")]
        //[ScriptControlPropertyAttribute]
        //[ClientPropertyName("format")]
        //[DescriptionAttribute("设置日历获得日期的格式")]
        //public string Format
        //{
        //    get { return GetPropertyValue("Format", "d"); }
        //    set { SetPropertyValue("Format", value); }
        //}

        /// <summary>
        /// 是否启用日历功能
        /// </summary>
        /// <remarks>是否启用日历功能</remarks>
        [Category("Default")]
        [ScriptControlPropertyAttribute]
        [ClientPropertyName("enabled")]
        [DescriptionAttribute("是否启用日历功能")]
        public bool EnabledOnClient
        {
            get { return GetPropertyValue("EnabledOnClient", true); }
            set { SetPropertyValue("EnabledOnClient", value); }
        }

        [DefaultValue(true)]
        [ScriptControlPropertyAttribute]
        [ClientPropertyName("animated")]
        [DescriptionAttribute("设置日历月份转换的动画效果")]
        public virtual bool Animated
        {
            get { return GetPropertyValue("Animated", true); }
            set { SetPropertyValue("Animated", value); }
        }

        /// <summary>
        /// 是否提供下拉框快捷选项
        /// </summary>
        /// <remarks>是否提供下拉框快捷选项</remarks>
        [Category("Default")]
        [ScriptControlPropertyAttribute]
        [ClientPropertyName("isComplexHeader")]
        [DescriptionAttribute("是否提供下拉框快捷选项")]
        public bool IsComplexHeader
        {
            get { return GetPropertyValue("IsComplexHeader", true); }
            set { SetPropertyValue("IsComplexHeader", value); }
        }

        /// <summary>
        /// 自定义第一列是从周几开始
        /// </summary>
        /// <remarks>自定义第一列是从周几开始</remarks>
        [Category("Data")]
        [ScriptControlPropertyAttribute]
        [ClientPropertyName("firstDayOfWeek")]
        [DescriptionAttribute("自定义第一列是从周几开始")]
        public FirstDayOfWeek FirstDayOfWeek
        {
            get { return GetPropertyValue("FirstDayOfWeek", FirstDayOfWeek.Default); }
            set { SetPropertyValue("FirstDayOfWeek", value); }
        }

        /// <summary>
        /// 弹出日历时触发的客户端事件
        /// </summary>
        /// <remarks>弹出日历时触发的客户端事件</remarks>
        [Category("Action")]
        [DefaultValue("")]
        [ScriptControEventAttribute]
        [ClientPropertyName("showing")]
        [DescriptionAttribute("弹出日历时触发的客户端事件")]
        public string OnClientShowing
        {
            get { return GetPropertyValue("OnClientShowing", string.Empty); }
            set { SetPropertyValue("OnClientShowing", value); }
        }

        /// <summary>
        /// 弹出日历时后触发的客户端事件
        /// </summary>
        /// <remarks>弹出日历时后触发的客户端事件</remarks>
        [Category("Action")]
        [DefaultValue("")]
        [ScriptControEventAttribute]
        [ClientPropertyName("shown")]
        [DescriptionAttribute("弹出日历时后触发的客户端事件")]
        public string OnClientShown
        {
            get { return GetPropertyValue("OnClientShown", string.Empty); }
            set { SetPropertyValue("OnClientShown", value); }
        }

        /// <summary>
        /// 隐藏日历时触发的客户端事件
        /// </summary>
        /// <remarks>隐藏日历时触发的客户端事件</remarks>
        [Category("Action")]
        [DefaultValue("")]
        [ScriptControEventAttribute]
        [ClientPropertyName("hiding")]
        [DescriptionAttribute("隐藏日历时触发的客户端事件")]
        public string OnClientHiding
        {
            get { return GetPropertyValue("OnClientHiding", string.Empty); }
            set { SetPropertyValue("OnClientHiding", value); }
        }

        /// <summary>
        /// 隐藏日历后触发的客户端事件
        /// </summary>
        /// <remarks>隐藏日历后触发的客户端事件</remarks>
        [Category("Action")]
        [DefaultValue("")]
        [ScriptControEventAttribute]
        [ClientPropertyName("hidden")]
        [DescriptionAttribute("隐藏日历后触发的客户端事件")]
        public string OnClientHidden
        {
            get { return GetPropertyValue("OnClientHidden", string.Empty); }
            set { SetPropertyValue("OnClientHidden", value); }
        }

        /// <summary>
        /// 日期选择变化后触发的客户端事件
        /// </summary>
        /// <remarks>日期选择变化后触发的客户端事件</remarks>
        [Category("Action")]
        [DefaultValue("")]
        [ScriptControEventAttribute]
        [ClientPropertyName("dateSelectionChanged")]
        [DescriptionAttribute("日期选择变化后触发的客户端事件")]
        public string OnClientDateSelectionChanged
        {
            get { return GetPropertyValue("OnClientDateSelectionChanged", string.Empty); }
            set { SetPropertyValue("OnClientDateSelectionChanged", value); }
        }
    }
}

