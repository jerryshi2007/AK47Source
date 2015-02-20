using System;
using System.Web.UI.WebControls;
using System.Web.UI;
using System.ComponentModel;
using System.ComponentModel.Design;
using System.Web.UI.WebControls.WebParts;
using System.Collections;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using ChinaCustoms.Framework.DeluxeWorks.Web.Library.Script;
[assembly: System.Web.UI.WebResource("ChinaCustoms.Framework.DeluxeWorks.Web.WebControls.MaskedEdit.MaskedEdit.css", "text/css", PerformSubstitution = true)]
[assembly: System.Web.UI.WebResource("ChinaCustoms.Framework.DeluxeWorks.Web.WebControls.MaskedEdit.MaskedEditBehavior.js", "text/javascript")]
namespace ChinaCustoms.Framework.DeluxeWorks.Web.WebControls
{
    /// <summary>
    /// 
    /// </summary>
    ///     [Designer("AjaxControlToolkit.MaskedEditDesigner, AjaxControlToolkit")]
    ///  
    [RequiredScript(typeof(PopupControlScript), 0)]
    [RequiredScript(typeof(DeluxeAjaxScript), 1)]
    [RequiredScript(typeof(BlockingScript), 2)] //
    [ClientCssResource("ChinaCustoms.Framework.DeluxeWorks.Web.WebControls.MaskedEdit.MaskedEdit.css")]
    [ClientScriptResource("ChinaCustoms.Framework.DeluxeWorks.Web.WebControls.MaskedEditBehavior", "ChinaCustoms.Framework.DeluxeWorks.Web.WebControls.MaskedEdit.MaskedEditBehavior.js")]
    public class MaskedEditControl : ScriptControlBase
    {
        /// <summary>
        /// 
        /// </summary>
        public MaskedEditControl()
            : base(true, HtmlTextWriterTag.Input)
        {
            this.Attributes.Add("type", "text");
        }
        private string setTextBoxValue;

        private GenericInputExtender extender = new GenericInputExtender();

        protected override void OnPreRender(EventArgs e)
        {
            this.Attributes.Add("name", this.UniqueID);
            this.Attributes.Add("value", MValue);

            this.Attributes.Add("class", TextCssClass);
            if (ShowButton)
            {
                extender.TargetControlID = this.UniqueID;
                this.extender.Items.Add(DataSource);
                this.Controls.Add(extender);
            }
            base.OnPreRender(e);
        }

        protected override void Render(HtmlTextWriter writer)
        {

            this.ApplyStyle(TextStyle);
            base.Render(writer);
            //    //ScriptManager.RegisterScriptDescriptors(this);
        }

        /// <summary>
        /// 设置文本框的样式
        /// </summary>
        /// <remarks>设置文本框的样式</remarks>
        [Category("Appearance")]
        [DescriptionAttribute("设置文本框的样式")]
        public string TextCssClass
        {
            get { return GetPropertyValue("TextCssClass", string.Empty); }
            set { SetPropertyValue("TextCssClass", value); }
        }

        private Style style = new Style();
        /// <summary>
        /// 输入框样式
        /// </summary>
        /// <remarks>输入框样式</remarks>
        [Category("Appearance")]
        [WebDescription("输入框样式")]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Content), NotifyParentProperty(true), PersistenceMode(PersistenceMode.InnerProperty)]
        public Style TextStyle
        {
            get { return style; }
            set { style = value; }
        }

        /// <summary>
        /// 时间格式串
        /// </summary>
        /// <remarks>时间格式串</remarks>
        [Category("Data")]
        [ScriptControlProperty]
        [DescriptionAttribute("时间格式串")]
        public string Mask
        {
            get { return GetPropertyValue("Mask", "99:99:99"); }
            set { SetPropertyValue("Mask", value); }
        }

        /// <summary>
        /// 掩码字符
        /// </summary>
        /// <remarks>掩码字符</remarks>
        [Category("Data")]
        [ScriptControlProperty]
        [DescriptionAttribute("掩码字符")]
        public string PromptCharacter
        {
            get { return GetPropertyValue("PromptCharacter", "_"); }
            set { SetPropertyValue("PromptCharacter", value); }
        }

        /// <summary>
        /// 是否自动补齐时间
        /// </summary>
        /// <remarks>是否自动补齐时间</remarks>
        [Category("Default")]
        [ScriptControlProperty]
        [Description("是否自动补齐时间")]
        public bool AutoComplete
        {
            get { return GetPropertyValue("AutoComplete", true); }
            set { SetPropertyValue("AutoComplete", value); }
        }

        /// <summary>
        /// 提供自动补齐的时间串，不设置则取系统时间
        /// </summary>
        /// <remarks>提供自动补齐的时间串，不设置则取系统时间</remarks>
        [Category("Data")]
        [ScriptControlProperty]
        [Description("提供自动补齐的时间串，不设置则取系统时间")]
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
        /// 验证时间的提示信息
        /// </summary>
        /// <remarks>验证时间的提示信息</remarks>
        [Category("Appearance")]
        [ScriptControlProperty]
        [DescriptionAttribute("验证时间的提示信息")]
        public string CurrentMessageError
        {
            get { return GetPropertyValue("CurrentMessageError", "输入错误！"); }
            set { SetPropertyValue("CurrentMessageError", value); }
        }

        /// <summary>
        /// 是否提供按钮来选择自定义时间列表,若是则需设置数据源
        /// </summary>
        /// <remarks>是否提供按钮来选择自定义时间列表,若是则需设置数据源</remarks>
        //[ScriptControlProperty]
        [Category("Appearance")]
        [DescriptionAttribute("是否提供按钮来选择自定义时间列表,若是则需设置数据源")]
        public bool ShowButton
        {
            get { return GetPropertyValue("ShowButton", false); }
            set { SetPropertyValue("ShowButton", value); }
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

        /// <summary>
        /// 设置或是获取时间的值
        /// </summary>
        /// <remarks>设置或是获取时间的值</remarks>
        [Category("Default")]
        [ClientPropertyName("mValue")]
        [DescriptionAttribute("设置或是获取时间的值")]
        public virtual string MValue
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

        #region    列表部分
        private ListItem listItem = new ListItem();
        /// <summary>
        /// 数据源
        /// </summary>
        /// <remarks>ListItem类型的数据源</remarks>
        [Category("Data")]
        public ListItem DataSource
        {
            get { return this.listItem; }
            set { this.listItem = value; }
        }

        #endregion
        //[ScriptControlProperty]
        //[ClientPropertyName("DataArrayList")]
        //[DescriptionAttribute("绑定的数据源")]
        //public string[] DataArrayList
        //{

        //    get { return GetPropertyValue("DataArrayList", new string[] { }); }
        //    set
        //    {
        //        if (value != null && value.Length != 0)
        //        {
        //            Regex rg = new Regex(@"^([01][0-9]|2[0-3]):[0-5]\d$");
        //            Regex rgs = new Regex(@"^([01]\d|2[0-3]):[0-5]\d:[0-5]\d$");
        //            foreach (string v in value)
        //            {
        //                if (rg.IsMatch(v) || rgs.IsMatch(v))
        //                { }
        //                else
        //                {
        //                    throw new ArgumentException("您设置列表的数据项格式错误");
        //                }
        //            }
        //            SetPropertyValue("DataArrayList", value);

        //        }
        //        else
        //        {
        //            throw new ArgumentException("您还未设置列表的数据源或数据源格式错误");
        //        }

        //    }

        //}

    }
}
