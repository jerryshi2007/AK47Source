using System;
using System.Collections.Generic;
using System.Text;
using ChinaCustoms.Framework.DeluxeWorks.Web.WebControls;
using System.Web.UI.WebControls;
using System.Text.RegularExpressions;
using System.ComponentModel;
//using ChinaCustoms.Framework.DeluxeWorks.Web.WebControls;
using ChinaCustoms.Framework.DeluxeWorks.Web.Library.Script;

namespace ChinaCustoms.Framework.DeluxeWorks.Web.WebControls
{
    public class DateTimeControl : System.Web.UI.WebControls.WebControl
    {
        public DateTimeControl()
        { }

        private PopupCalendarControl calendar = new PopupCalendarControl();
        private MaskedEditControl maskededit = new MaskedEditControl();
        protected override void OnInit(EventArgs e)
        {
            if (!ReadOnly)
            {
                Controls.Add(calendar);
                Controls.Add(maskededit);
            }
            else
            {

                Controls.Add(datetimeValue);
            }

            base.OnInit(e);
        }
    
        #region 时间部分

        //[DescriptionAttribute("时间输入框的Style")]
        //public string TimeTextStyle
        //{
        //    get { return maskededit.TextStyle; }
        //    set { maskededit.TextStyle = value; }
        //}

        //[DescriptionAttribute("时间输入框的Css")]
        //public string TimeTextCss
        //{
        //    get { return maskededit.TextCssClass; }
        //    set { maskededit.TextCssClass = value; }
        //}

        [DescriptionAttribute("时间的掩码")]
        public string TimeMask
        {
            get { return maskededit.Mask; }
            set { maskededit.Mask = value; }
        }

        [DefaultValue("_")]
        [ScriptControlProperty]
        [DescriptionAttribute("掩码字符")]
        public string TimePromptCharacter
        {
            get { return maskededit.PromptCharacter; }
            set { maskededit.PromptCharacter = value; }
        }

        //[DescriptionAttribute("按钮的样式")]
        //public string ButtonCss
        //{
        //    get { return maskededit.ButtonCss; }
        //    set { maskededit.ButtonCss = value; }
        //}

        [DescriptionAttribute("是否自动补齐时间")]
        public bool TimeAutoComplete
        {
            get { return maskededit.AutoComplete; }
            set { maskededit.AutoComplete = value; }
        }

        [Description("提供自动补齐的时间串，不设置则取系统时间")]
        public string TimeAutoCompleteValue
        {
            get { return maskededit.AutoCompleteValue; }
            set { maskededit.AutoCompleteValue = value; }
        }

        [DescriptionAttribute("是否启用时间验证")]
        public bool IsValidTimeValue
        {
            get { return maskededit.IsValidValue; }
            set { maskededit.IsValidValue = value; }
        }

        [DescriptionAttribute("是否提供按钮来选择自定义时间列表,若是则需设置数据源")]
        public bool ShowButton
        {
            get { return maskededit.ShowButton; }
            set { maskededit.ShowButton = value; }
        }

        [DescriptionAttribute("验证时间的提示信息")]
        public string TimeCurrentMessageError
        {
            get { return maskededit.CurrentMessageError; }
            set { maskededit.CurrentMessageError = value; }
        }

        [DescriptionAttribute("得到焦点时时间文本框的样式")]
        public string OnTimeFocusCssClass
        {

            get { return maskededit.OnFocusCssClass; }
            set { maskededit.OnFocusCssClass = value; }

        }

        [ScriptControlProperty]
        [DescriptionAttribute("验证不通过时时间文本框的样式")]
        public string OnTimeInvalidCssClass
        {

            get { return maskededit.OnInvalidCssClass; }
            set
            {
                maskededit.OnInvalidCssClass = value;
            }

        }

        [ScriptControlProperty]
        [DescriptionAttribute("绑定时间的数据源")]
        public ListItem DataSource
        {

            get { return maskededit.DataSource; }
            set
            {
                maskededit.DataSource = value;
            }

        }

        #endregion
       
        #region 日期部分


        [DescriptionAttribute("是否只显示当月")]
        public virtual bool IsOnlyCurrentMonth
        {
            get { return calendar.IsOnlyCurrentMonth; }
            set { calendar.IsOnlyCurrentMonth = value; }
        }

        //[ClientPropertyName("cssClass")]
        [DescriptionAttribute("日历的样式，不填为默认样式")]
        public string PopupCalendarCssClass
        {
            get { return calendar.CssClass; }
            set { calendar.CssClass = value; }
        }

        //[DescriptionAttribute("日历的Style")]
        //public string DateTextStyle
        //{
        //    get { return calendar.TextStyle; }
        //    set { calendar.TextStyle = value; }
        //}

        [DescriptionAttribute("日历输入框的Css")]
        public string DateTextCssClass
        {
            get { return calendar.TextCssClass; }
            set { calendar.TextCssClass = value; }
        }

        [DescriptionAttribute("图片的Style")]
        public string DateImageStyle
        {
            get { return calendar.ImageStyle; }
            set { calendar.ImageStyle = value; }
        }

        [DescriptionAttribute("图片的CssClass")]
        public string DateImageCssClass
        {
            get { return calendar.ImageCssClass; }
            set { calendar.ImageCssClass = value; }
        }

        [DescriptionAttribute("按钮图片的Src")]
        public string DateImageUrl
        {
            //get { return GetPropertyValue("ImageUrl", ""); }
            get { return calendar.ImageUrl; }
            set { calendar.ImageUrl = value; }
        }

        //[DefaultValue("d")]
        //[ScriptControlPropertyAttribute]
        //[DescriptionAttribute("设置日历获得日期的格式")]
        //public virtual string DateFormat
        //{
        //    get { return calendar.Format; }
        //    set { calendar.Format = value; }
        //}

        #region     日期掩码部分

        //[DescriptionAttribute("日期格式串")]
        //public string DateMask
        //{
        //    get { return calendar.Mask; }
        //    set { calendar.Mask = value; }
        //}

        [DescriptionAttribute("日期掩码字符")]
        public string DatePromptCharacter
        {
            get { return calendar.PromptCharacter; }
            set { calendar.PromptCharacter = value; }
        }

        [ScriptControlProperty]
        [Description("是否自动补齐日期")]
        public bool DateAutoComplete
        {
            get { return calendar.AutoComplete; }
            set { calendar.AutoComplete = value; }
        }
		//private string autoCompleteValue;
        [ScriptControlProperty]
        [Description("提供自动补齐的时间串，不设置则取系统日期")]
        public string DateAutoCompleteValue
        {
            get { return calendar.AutoCompleteValue; }
            set { calendar.AutoCompleteValue = value; }
        }

        [ScriptControlProperty]
        [Description("是否启用日期验证")]
        public bool IsValidDateValue
        {
            get { return calendar.IsValidValue; }
            set { calendar.IsValidValue = value; }
        }

        [ScriptControlProperty]
        [DescriptionAttribute("验证日期的提示信息")]
        public string DateCurrentMessageError
        {
            get { return calendar.CurrentMessageError; }
            set { calendar.CurrentMessageError = value; }
        }

        private string dateFocusCss;
        [ScriptControlProperty]
        [DescriptionAttribute("日期得到焦点时文本框的样式")]
        public string OnFocusDateCssClass
        {

            get { return dateFocusCss; }
            set
            {
                dateFocusCss = value;
            }

        }

        private string dateInvalidCss;
        [ScriptControlProperty]
        [DescriptionAttribute("日期验证不通过时文本框的样式")]
        public string OnInvalidDateCssClass
        {

            get { return dateInvalidCss; }
            set
            {

                dateInvalidCss = value;
            }

        }


        #endregion

        [DefaultValue(true)]
        [ScriptControlPropertyAttribute]
        [DescriptionAttribute("是否启用日历功能")]
        public virtual bool EnabledOnClient
        {
            get { return calendar.EnabledOnClient; }
            set { calendar.EnabledOnClient = value; }
        }

        //[DefaultValue(true)]
        //[ExtenderControlProperty]
        //[ClientPropertyName("animated")]
        //[DescriptionAttribute("设置日历月份转换的动画效果")]
        //public virtual bool Animated
        //{
        //    get { return GetPropertyValue("Animated", true); }
        //    set { SetPropertyValue("Animated", value); }
        //}

        [DefaultValue(true)]
        [ScriptControlPropertyAttribute]
        [DescriptionAttribute("是否提供下拉框快捷选项")]
        public virtual bool IsComplexHeader
        {
            get { return calendar.IsComplexHeader; }
            set { calendar.IsComplexHeader = value; }
        }

        [DefaultValue(FirstDayOfWeek.Default)]
        [ScriptControlPropertyAttribute]
        [DescriptionAttribute("自定义第一列是从周几开始")]
        public virtual FirstDayOfWeek FirstDayOfWeek
        {
            get { return calendar.FirstDayOfWeek; }
            set { calendar.FirstDayOfWeek = value; }
        }


        [DefaultValue("")]
        [ScriptControEventAttribute]
        [DescriptionAttribute("弹出日历时触发的客户端事件")]
        public virtual string OnClientShowing
        {
            get { return calendar.OnClientShowing; }
            set { calendar.OnClientShowing = value; }
        }

        [DefaultValue("")]
        [ScriptControEventAttribute]
        [DescriptionAttribute("弹出日历时后触发的客户端事件")]
        public virtual string OnClientShown
        {
            get { return calendar.OnClientShown; }
            set { calendar.OnClientShown = value; }
        }

        [DefaultValue("")]
        [ScriptControEventAttribute]
        [DescriptionAttribute("隐藏日历时触发的客户端事件")]
        public virtual string OnClientHiding
        {
            get { return calendar.OnClientHiding; }
            set { calendar.OnClientHiding = value; }
        }

        [DefaultValue("")]
        [ScriptControEventAttribute]
        [DescriptionAttribute("隐藏日历后触发的客户端事件")]
        public virtual string OnClientHidden
        {
            get { return calendar.OnClientHidden; }
            set { calendar.OnClientHidden = value; }
        }

        [DefaultValue("")]
        [ScriptControEventAttribute]
        [DescriptionAttribute("日期选择变化后触发的客户端事件")]
        public virtual string OnClientDateSelectionChanged
        {
            get { return calendar.OnClientDateSelectionChanged; }
            set { calendar.OnClientDateSelectionChanged = value; }
        }

        #endregion


        //public string[] DataArrayList
        //{
        //    get { return maskededit.DataArrayList; }
        //    set { maskededit.DataArrayList = value; }
        //}
        //private DateTime dt;
        public DateTime Value
        {
            get
            {
                if (ReadOnly)
                {
                    return Convert.ToDateTime(datetimeValue.Text);
                }
                else
                {
                    string cv = calendar.CValue;
                    string mv = maskededit.MValue;
           
                    //if (mv.Length > 6 && mv.Length < 10)

                    if (cv != null && cv != string.Empty && mv != null && mv != string.Empty)
                    {
                        try
                        {
                            return Convert.ToDateTime(calendar.CValue + " " + maskededit.MValue);
                        }
                        catch { throw new InvalidCastException("输入框内的格式转换时间无效！"); }
                    }
                    else
                    {
                        //return dt;/*返回默认值*/
						return DateTime.MaxValue;
                    }
                    //else
                    //{
                    //    throw new InvalidCastException("输入框内的格式转换时间无效！");
                    //}
                }
            }

            //set { calendar.CValue = value.Date.ToShortDateString(); maskededit.MValue = value.ToShortTimeString(); }
            set
            {
                if (ReadOnly)
                {
                    datetimeValue.Text = value.ToString("yyyy-MM-dd HH:mm:ss");
                }
                else
                {
                    calendar.CValue = value.Date.ToShortDateString(); maskededit.MValue = value.ToShortTimeString();
                }
            }
        }

		//private bool readOnly;
        public bool ReadOnly
        {
            get 
			{
                if (ViewState["IsReadOnly"] != null)
                {
                    return (bool)ViewState["IsReadOnly"];
                }
                return false;
            }
            set 
			{ 
				ViewState["IsReadOnly"] = value; 
			}
        }

        Label datetimeValue = new Label();

    
         
    }
}
