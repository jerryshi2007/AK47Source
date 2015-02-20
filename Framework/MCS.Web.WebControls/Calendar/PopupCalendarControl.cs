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
        /// �Ƿ�ֻ��ʾ����
        /// </summary>
        ///<remarks>�Ƿ�ֻ��ʾ����</remarks>
        [Category("Default")]
        [ScriptControlProperty]
        [ClientPropertyName("isOnlyCurrentMonth")]
        [DescriptionAttribute("�Ƿ�ֻ��ʾ����")]
        public bool IsOnlyCurrentMonth
        {
            get { return GetPropertyValue("IsOnlyCurrentMonth", true); }
            set { SetPropertyValue("IsOnlyCurrentMonth", value); }
        }

        /// <summary>
        /// ��������ʽ������ΪĬ����ʽ
        /// </summary>
        ///<remarks>��������ʽ������ΪĬ����ʽ</remarks>
        [Category("Appearance")]
        [ScriptControlPropertyAttribute]
        [ClientPropertyName("cssClass")]
        [DescriptionAttribute("��������ʽ������ΪĬ����ʽ")]
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
        [WebDescription("�������ʽ")]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Content), NotifyParentProperty(true), PersistenceMode(PersistenceMode.InnerProperty)]
        public Style TextStyle
        {
            get { return style; }
            set { style = value; }
        }

        /// <summary>
        /// �����ı����Css
        /// </summary>
        /// <remarks>�����ı����Css</remarks>
        [Category("Appearance")]
        [DescriptionAttribute("�����ı����Css")]
        public string TextCssClass
        {
            get { return GetPropertyValue("TextCssClass", "ajax_calendartextbox"); }
            set { SetPropertyValue("TextCssClass", value); }
        }

        /// <summary>
        /// ͼƬ��Style
        /// </summary>
        /// <remarks>ͼƬ��Style</remarks>
        //[ClientPropertyName]
        [Category("Appearance")]
        [DescriptionAttribute("ͼƬ��Style")]
        public string ImageStyle
        {
            get { return GetPropertyValue("ImageStyle", string.Empty); }
            set { SetPropertyValue("ImageStyle", value); }
        }

        /// <summary>
        /// ͼƬ��CssClass
        /// </summary>
        /// <remarks>ͼƬ��CssClass</remarks>
        [Category("Appearance")]
        [DescriptionAttribute("ͼƬ��CssClass")]
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
        [DescriptionAttribute("��ť��ID")]
        public string MaskedEditButtonID
        {
            get { return GetPropertyValue("MaskedEditButtonID", this.UniqueID + "_image"); }
            set { SetPropertyValue("MaskedEditButtonID", value); }
        }

        /// <summary>
        /// ��ťͼƬ��Src
        /// </summary>
        /// <remarks>��ťͼƬ��Src</remarks>
        [Category("Appearance")]
        [DescriptionAttribute("��ťͼƬ��Src")]
        public string ImageUrl
        {
            //get { return GetPropertyValue("ImageUrl", ""); }
            get { return GetPropertyValue("ImageUrl", Page.ClientScript.GetWebResourceUrl(typeof(PopupCalendarControl), "ChinaCustoms.Framework.DeluxeWorks.Web.WebControls.Calendar.Images.datePicker.gif")); }
            set { SetPropertyValue("ImageUrl", value); }
        }
        
        #region     �������벿��

        //[DefaultValue("9999-99-99")]
        //[ScriptControlProperty]
        //[DescriptionAttribute("���ڸ�ʽ��")]
        //public string Mask
        //{
        //    get { return GetPropertyValue("Mask", "9999-99-99"); }
        //    set { SetPropertyValue("Mask", value); }
        //}

        /// <summary>
        /// �����ַ�
        /// </summary>
        /// <remarks>�����ַ�</remarks>
        [Category("Data")]
        [ScriptControlProperty]
        [DescriptionAttribute("�����ַ�")]
        public string PromptCharacter
        {
            get { return GetPropertyValue("PromptCharacter","_"); }
            set { SetPropertyValue("PromptCharacter", value); }
        }

        /// <summary>
        /// �Ƿ��Զ���������
        /// </summary>
        /// <remarks>�Ƿ��Զ���������</remarks>
        [Category("Default")]
        [ScriptControlProperty]
        [Description("�Ƿ��Զ���������")]
        public bool AutoComplete
        {
            get { return GetPropertyValue("AutoComplete", true); }
            set { SetPropertyValue("AutoComplete", value); }
        }

        /// <summary>
        /// �ṩ�Զ������ʱ�䴮����������ȡϵͳ����
        /// </summary>
        /// <remarks>�ṩ�Զ������ʱ�䴮����������ȡϵͳ����</remarks>
        [Category("Data")]
        [ScriptControlProperty]
        [Description("�ṩ�Զ������ʱ�䴮����������ȡϵͳ����")]
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
        /// ��֤���ڵ���ʾ��Ϣ
        /// </summary>
        /// <remarks>��֤���ڵ���ʾ��Ϣ</remarks>
        [Category("Data")]
        [ScriptControlProperty]
        [DescriptionAttribute("��֤���ڵ���ʾ��Ϣ")]
        public string CurrentMessageError
        {
            get { return GetPropertyValue("CurrentMessageError", "�������"); }
            set { SetPropertyValue("CurrentMessageError", value); }
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
        /// ���û��ǻ�ȡ������ֵ
        /// </summary>
        /// <remarks>���û��ǻ�ȡ������ֵ</remarks>
        [Category("Default")]
        [ClientPropertyName("cValue")]
        [WebDisplayName("���û��ǻ�ȡ������ֵ")]
        [DescriptionAttribute("���û��ǻ�ȡ������ֵ")]
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
        //[DescriptionAttribute("��������������ڵĸ�ʽ")]
        //public string Format
        //{
        //    get { return GetPropertyValue("Format", "d"); }
        //    set { SetPropertyValue("Format", value); }
        //}

        /// <summary>
        /// �Ƿ�������������
        /// </summary>
        /// <remarks>�Ƿ�������������</remarks>
        [Category("Default")]
        [ScriptControlPropertyAttribute]
        [ClientPropertyName("enabled")]
        [DescriptionAttribute("�Ƿ�������������")]
        public bool EnabledOnClient
        {
            get { return GetPropertyValue("EnabledOnClient", true); }
            set { SetPropertyValue("EnabledOnClient", value); }
        }

        [DefaultValue(true)]
        [ScriptControlPropertyAttribute]
        [ClientPropertyName("animated")]
        [DescriptionAttribute("���������·�ת���Ķ���Ч��")]
        public virtual bool Animated
        {
            get { return GetPropertyValue("Animated", true); }
            set { SetPropertyValue("Animated", value); }
        }

        /// <summary>
        /// �Ƿ��ṩ��������ѡ��
        /// </summary>
        /// <remarks>�Ƿ��ṩ��������ѡ��</remarks>
        [Category("Default")]
        [ScriptControlPropertyAttribute]
        [ClientPropertyName("isComplexHeader")]
        [DescriptionAttribute("�Ƿ��ṩ��������ѡ��")]
        public bool IsComplexHeader
        {
            get { return GetPropertyValue("IsComplexHeader", true); }
            set { SetPropertyValue("IsComplexHeader", value); }
        }

        /// <summary>
        /// �Զ����һ���Ǵ��ܼ���ʼ
        /// </summary>
        /// <remarks>�Զ����һ���Ǵ��ܼ���ʼ</remarks>
        [Category("Data")]
        [ScriptControlPropertyAttribute]
        [ClientPropertyName("firstDayOfWeek")]
        [DescriptionAttribute("�Զ����һ���Ǵ��ܼ���ʼ")]
        public FirstDayOfWeek FirstDayOfWeek
        {
            get { return GetPropertyValue("FirstDayOfWeek", FirstDayOfWeek.Default); }
            set { SetPropertyValue("FirstDayOfWeek", value); }
        }

        /// <summary>
        /// ��������ʱ�����Ŀͻ����¼�
        /// </summary>
        /// <remarks>��������ʱ�����Ŀͻ����¼�</remarks>
        [Category("Action")]
        [DefaultValue("")]
        [ScriptControEventAttribute]
        [ClientPropertyName("showing")]
        [DescriptionAttribute("��������ʱ�����Ŀͻ����¼�")]
        public string OnClientShowing
        {
            get { return GetPropertyValue("OnClientShowing", string.Empty); }
            set { SetPropertyValue("OnClientShowing", value); }
        }

        /// <summary>
        /// ��������ʱ�󴥷��Ŀͻ����¼�
        /// </summary>
        /// <remarks>��������ʱ�󴥷��Ŀͻ����¼�</remarks>
        [Category("Action")]
        [DefaultValue("")]
        [ScriptControEventAttribute]
        [ClientPropertyName("shown")]
        [DescriptionAttribute("��������ʱ�󴥷��Ŀͻ����¼�")]
        public string OnClientShown
        {
            get { return GetPropertyValue("OnClientShown", string.Empty); }
            set { SetPropertyValue("OnClientShown", value); }
        }

        /// <summary>
        /// ��������ʱ�����Ŀͻ����¼�
        /// </summary>
        /// <remarks>��������ʱ�����Ŀͻ����¼�</remarks>
        [Category("Action")]
        [DefaultValue("")]
        [ScriptControEventAttribute]
        [ClientPropertyName("hiding")]
        [DescriptionAttribute("��������ʱ�����Ŀͻ����¼�")]
        public string OnClientHiding
        {
            get { return GetPropertyValue("OnClientHiding", string.Empty); }
            set { SetPropertyValue("OnClientHiding", value); }
        }

        /// <summary>
        /// ���������󴥷��Ŀͻ����¼�
        /// </summary>
        /// <remarks>���������󴥷��Ŀͻ����¼�</remarks>
        [Category("Action")]
        [DefaultValue("")]
        [ScriptControEventAttribute]
        [ClientPropertyName("hidden")]
        [DescriptionAttribute("���������󴥷��Ŀͻ����¼�")]
        public string OnClientHidden
        {
            get { return GetPropertyValue("OnClientHidden", string.Empty); }
            set { SetPropertyValue("OnClientHidden", value); }
        }

        /// <summary>
        /// ����ѡ��仯�󴥷��Ŀͻ����¼�
        /// </summary>
        /// <remarks>����ѡ��仯�󴥷��Ŀͻ����¼�</remarks>
        [Category("Action")]
        [DefaultValue("")]
        [ScriptControEventAttribute]
        [ClientPropertyName("dateSelectionChanged")]
        [DescriptionAttribute("����ѡ��仯�󴥷��Ŀͻ����¼�")]
        public string OnClientDateSelectionChanged
        {
            get { return GetPropertyValue("OnClientDateSelectionChanged", string.Empty); }
            set { SetPropertyValue("OnClientDateSelectionChanged", value); }
        }
    }
}

