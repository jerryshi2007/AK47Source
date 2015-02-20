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
        /// �����ı������ʽ
        /// </summary>
        /// <remarks>�����ı������ʽ</remarks>
        [Category("Appearance")]
        [DescriptionAttribute("�����ı������ʽ")]
        public string TextCssClass
        {
            get { return GetPropertyValue("TextCssClass", string.Empty); }
            set { SetPropertyValue("TextCssClass", value); }
        }

        private Style style = new Style();
        /// <summary>
        /// �������ʽ
        /// </summary>
        /// <remarks>�������ʽ</remarks>
        [Category("Appearance")]
        [WebDescription("�������ʽ")]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Content), NotifyParentProperty(true), PersistenceMode(PersistenceMode.InnerProperty)]
        public Style TextStyle
        {
            get { return style; }
            set { style = value; }
        }

        /// <summary>
        /// ʱ���ʽ��
        /// </summary>
        /// <remarks>ʱ���ʽ��</remarks>
        [Category("Data")]
        [ScriptControlProperty]
        [DescriptionAttribute("ʱ���ʽ��")]
        public string Mask
        {
            get { return GetPropertyValue("Mask", "99:99:99"); }
            set { SetPropertyValue("Mask", value); }
        }

        /// <summary>
        /// �����ַ�
        /// </summary>
        /// <remarks>�����ַ�</remarks>
        [Category("Data")]
        [ScriptControlProperty]
        [DescriptionAttribute("�����ַ�")]
        public string PromptCharacter
        {
            get { return GetPropertyValue("PromptCharacter", "_"); }
            set { SetPropertyValue("PromptCharacter", value); }
        }

        /// <summary>
        /// �Ƿ��Զ�����ʱ��
        /// </summary>
        /// <remarks>�Ƿ��Զ�����ʱ��</remarks>
        [Category("Default")]
        [ScriptControlProperty]
        [Description("�Ƿ��Զ�����ʱ��")]
        public bool AutoComplete
        {
            get { return GetPropertyValue("AutoComplete", true); }
            set { SetPropertyValue("AutoComplete", value); }
        }

        /// <summary>
        /// �ṩ�Զ������ʱ�䴮����������ȡϵͳʱ��
        /// </summary>
        /// <remarks>�ṩ�Զ������ʱ�䴮����������ȡϵͳʱ��</remarks>
        [Category("Data")]
        [ScriptControlProperty]
        [Description("�ṩ�Զ������ʱ�䴮����������ȡϵͳʱ��")]
        public string AutoCompleteValue
        {
            get { return GetPropertyValue("AutoCompleteValue", string.Empty); }
            set { SetPropertyValue("AutoCompleteValue", value); }
        }

        /// <summary>
        /// �Ƿ�������֤
        /// </summary>
        /// <remarks>�Ƿ�������֤</remarks>
        [Category("Default")]
        [ScriptControlProperty]
        [Description("�Ƿ�������֤")]
        public bool IsValidValue
        {
            get { return GetPropertyValue("IsValidValue", true); }
            set { SetPropertyValue("IsValidValue", value); }
        }

        /// <summary>
        /// ��֤ʱ�����ʾ��Ϣ
        /// </summary>
        /// <remarks>��֤ʱ�����ʾ��Ϣ</remarks>
        [Category("Appearance")]
        [ScriptControlProperty]
        [DescriptionAttribute("��֤ʱ�����ʾ��Ϣ")]
        public string CurrentMessageError
        {
            get { return GetPropertyValue("CurrentMessageError", "�������"); }
            set { SetPropertyValue("CurrentMessageError", value); }
        }

        /// <summary>
        /// �Ƿ��ṩ��ť��ѡ���Զ���ʱ���б�,����������������Դ
        /// </summary>
        /// <remarks>�Ƿ��ṩ��ť��ѡ���Զ���ʱ���б�,����������������Դ</remarks>
        //[ScriptControlProperty]
        [Category("Appearance")]
        [DescriptionAttribute("�Ƿ��ṩ��ť��ѡ���Զ���ʱ���б�,����������������Դ")]
        public bool ShowButton
        {
            get { return GetPropertyValue("ShowButton", false); }
            set { SetPropertyValue("ShowButton", value); }
        }

        /// <summary>
        /// �õ�����ʱ�ı������ʽ
        /// </summary>
        /// <remarks>�õ�����ʱ�ı������ʽ</remarks>
        [Category("Appearance")]
        [ScriptControlProperty]
        [DescriptionAttribute("�õ�����ʱ�ı������ʽ")]
        public string OnFocusCssClass
        {

            get { return GetPropertyValue("OnFocusCssClass", "MaskedEditFocus"); }
            set
            {
                SetPropertyValue("OnFocusCssClass", value);
            }

        }

        /// <summary>
        /// ��֤��ͨ��ʱ�ı������ʽ
        /// </summary>
        /// <remarks>��֤��ͨ��ʱ�ı������ʽ</remarks>
        [Category("Appearance")]
        [ScriptControlProperty]
        [DescriptionAttribute("��֤��ͨ��ʱ�ı������ʽ")]
        public string OnInvalidCssClass
        {

            get { return GetPropertyValue("OnInvalidCssClass", "MaskedEditError"); ; }
            set
            {

                SetPropertyValue("OnInvalidCssClass", value);
            }

        }

        /// <summary>
        /// ���û��ǻ�ȡʱ���ֵ
        /// </summary>
        /// <remarks>���û��ǻ�ȡʱ���ֵ</remarks>
        [Category("Default")]
        [ClientPropertyName("mValue")]
        [DescriptionAttribute("���û��ǻ�ȡʱ���ֵ")]
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

        #region    �б���
        private ListItem listItem = new ListItem();
        /// <summary>
        /// ����Դ
        /// </summary>
        /// <remarks>ListItem���͵�����Դ</remarks>
        [Category("Data")]
        public ListItem DataSource
        {
            get { return this.listItem; }
            set { this.listItem = value; }
        }

        #endregion
        //[ScriptControlProperty]
        //[ClientPropertyName("DataArrayList")]
        //[DescriptionAttribute("�󶨵�����Դ")]
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
        //                    throw new ArgumentException("�������б���������ʽ����");
        //                }
        //            }
        //            SetPropertyValue("DataArrayList", value);

        //        }
        //        else
        //        {
        //            throw new ArgumentException("����δ�����б������Դ������Դ��ʽ����");
        //        }

        //    }

        //}

    }
}
