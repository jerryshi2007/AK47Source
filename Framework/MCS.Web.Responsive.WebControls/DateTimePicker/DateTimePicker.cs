using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Web.UI;
using System.Web.UI.WebControls;
using MCS.Library.Globalization;
using MCS.Web.Library.Script;
using MCS.Web.Responsive.Library;
using MCS.Web.Responsive.Library.Script;

[assembly: WebResource("MCS.Web.Responsive.WebControls.DateTimePicker.bootstrap-datetimepicker.js", "application/x-javascript")]
[assembly: WebResource("MCS.Web.Responsive.WebControls.DateTimePicker.moment.min.js", "application/x-javascript")]
[assembly: WebResource("MCS.Web.Responsive.WebControls.DateTimePicker.DateTimePicker.js", "application/x-javascript")]
[assembly: WebResource("MCS.Web.Responsive.WebControls.DateTimePicker.bootstrap-datetimepicker.css", "text/css")]
[assembly: WebResource("MCS.Web.Responsive.WebControls.DateTimePicker.bootstrap-daterangepicker.css", "text/css")]

namespace MCS.Web.Responsive.WebControls
{
    [ToolboxData("<{0}:DateTimePicker runat=server></{0}:DateTimePicker>")]
    [RequiredScript(typeof(ControlBaseScript))]
    [ParseChildren(true)]
    [PersistChildren(false)]

    //[DefaultEvent("SelectionChanged")]
    [DefaultProperty("Value")]
    [ControlValueProperty("Value", typeof(DateTime), "1/1/0001")]

    [Designer(typeof(Design.GenericDesigner))]
    [RequiredScript(typeof(DateTimeScriptReference))]
    [ClientScriptResource("MCS.Web.WebControls.DateTimePicker", "MCS.Web.Responsive.WebControls.DateTimePicker.DateTimePicker.js")]
    [ClientCssResource("MCS.Web.Responsive.WebControls.DateTimePicker.bootstrap-datetimepicker.css")]

    public class DateTimePicker : ScriptControlBase
    {
        private const string vsKeyMode = "PickerMode";
        private const string vsKeyPlaceHolder = "PlaceHolder";
        private const string vsKeyApplyDefaultCssClass = "ApplyDefaultCssClass";
        private const string vsKeyStartDateTime = "StartDateTime";
        private const string vsKeyEndDateTime = "EndDateTime";
        private const string vsKeyDisplayFormat = "DisplayFormat";
        private const string vsKeyFirstDayOfWeek = "FirstDayOfWeek";
        private const string vsKeyOnClientSelectionChanged = "OnClientSelectionChanged";
        private const string vsKeyOnClientErrorDate = "OnClientErrorDate";
        private const string vsKeyOnClientValueChanged = "OnClientValueChanged";
        private const string vsKeyValue = "Value";
        private const string vsKeyDayOfToday = "DayOfToday";
        private const string vsKeyAsComponent = "AsComponent";
        private const string vsKeyAsFormControl = "AsFormControl";
        private const string vsKeyAutoComplete = "AutoComplete";

        public DateTimePicker()
            : base(true, "div")
        {
        }

        #region 属性

        /// <summary>
        /// 不允许修改高度
        /// </summary>
        [EditorBrowsable(EditorBrowsableState.Never)]
        [Browsable(false)]
        public override System.Web.UI.WebControls.Unit Height
        {
            get
            {
                return base.Height;
            }
            set
            {
                base.Height = value;
            }
        }

        [Description("此控件的显示模式")]
        [Category("Behavior")]
        [DefaultValue(DateTimePickerMode.DatePicker)]
        [ScriptControlProperty(true)]
        [ClientPropertyName("mode")]
        public DateTimePickerMode Mode
        {
            get
            {
                return this.ViewState.GetViewStateValue<DateTimePickerMode>(vsKeyMode, DateTimePickerMode.DatePicker);
            }
            set
            {
                this.ViewState.SetViewStateValue(vsKeyMode, value);
            }
        }

        [Description("此控件的输入占位符（HTML5时有效）")]
        [Category("Appearance")]
        [DefaultValue("")]
        [ScriptControlProperty(true)]
        [ClientPropertyName("placeHolder")]
        public string PlaceHolder
        {
            get
            {
                return this.ViewState.GetViewStateValue(vsKeyPlaceHolder, string.Empty);
            }
            set
            {
                this.ViewState.SetViewStateValue(vsKeyPlaceHolder, value);
            }
        }

        [Description("表示是否应用默认的class")]
        [Category("Appearance")]
        [DefaultValue(true)]
        public bool ApplyDefaultCssClass
        {
            get
            {
                return this.ViewState.GetViewStateValue(vsKeyApplyDefaultCssClass, true);
            }
            set
            {
                this.ViewState.SetViewStateValue(vsKeyApplyDefaultCssClass, value);
            }
        }

        /// <summary>
        /// 控件允许输入的最小日期时间 ，如果为默认值则忽略此参数
        /// </summary>
        [Description("控件允许输入的最小日期时间")]
        [Category("Behavior")]
        [DefaultValue(typeof(DateTime), "0001-01-01")]
        [ScriptControlProperty(true)]
        [ClientPropertyName("startDateTime")]
        public DateTime StartDateTime
        {
            get
            {
                return this.ViewState.GetViewStateValue(vsKeyStartDateTime, DateTime.MinValue);
            }
            set
            {
                this.ViewState.SetViewStateValue(vsKeyStartDateTime, value);
            }
        }

        /// <summary>
        /// 控件允许输入的最大日期时间 ，如果为默认值则忽略此参数
        /// </summary>
        [Description("控件允许输入的最大日期时间")]
        [Category("Behavior")]
        [DefaultValue(typeof(DateTime), "0001-01-01")]
        [ScriptControlProperty(true)]
        [ClientPropertyName("endDateTime")]
        public DateTime EndDateTime
        {
            get
            {
                return this.ViewState.GetViewStateValue(vsKeyEndDateTime, DateTime.MinValue);
            }
            set
            {
                this.ViewState.SetViewStateValue(vsKeyEndDateTime, value);
            }
        }

        /// <summary>
        /// 只读状况下显示格式
        /// </summary>
        [Description("此控件在只读情况下的显示模式")]
        [Category("Behavior")]
        [DefaultValue(typeof(string), "")]
        [ScriptControlProperty(true)]
        [ClientPropertyName("displayFormat")]
        public string DisplayFormat
        {
            get
            {
                return (string)this.ViewState[vsKeyDisplayFormat] ?? string.Empty;
            }
            set
            {
                this.ViewState.SetViewStateValue<string>(vsKeyDisplayFormat, value ?? string.Empty);
            }
        }

        /// <summary>
        /// 设置true时文本框为只读
        /// </summary>
        [Description("设置true时文本框为只读")]
        [Category("Behavior")]
        [DefaultValue(false)]
        [ScriptControlProperty(true)]
        [ClientPropertyName("asComponent")]
        public bool AsComponent
        {
            get
            {
                return this.ViewState.GetViewStateValue(vsKeyAsComponent, false);
            }
            set
            {
                this.ViewState.SetViewStateValue(vsKeyAsComponent, value);
            }
        }

        /// <summary>
        /// 设置true时文本框为只读
        /// </summary>
        [Description("设置是否作为表单控件（带name）")]
        [Category("Behavior")]
        [DefaultValue(false)]
        [ScriptControlProperty(true)]
        [ClientPropertyName("asFormControl")]
        public bool AsFormControl
        {
            get
            {
                return this.ViewState.GetViewStateValue(vsKeyAsFormControl, false);
            }
            set
            {
                this.ViewState.SetViewStateValue(vsKeyAsFormControl, value);
            }
        }

        /// <summary>
        /// 获取或设置每周的第一天
        /// </summary>
        [Description("设置true时，自动补全日期")]
        [Category("Behavior")]
        [DefaultValue(true)]
        [ScriptControlProperty(true)]
        [ClientPropertyName("autoComplete")]
        public bool AutoComplete
        {
            get
            {
                return this.ViewState.GetViewStateValue(vsKeyAutoComplete, true);
            }
            set
            {
                this.ViewState.SetViewStateValue(vsKeyAutoComplete, value);
            }
        }

        /// <summary>
        /// 获取或设置每周的第一天
        /// </summary>
        [Description("设置显示每周的第一天")]
        [Category("Behavior")]
        [DefaultValue(DayOfWeek.Sunday)]
        [ScriptControlProperty(true)]
        [ClientPropertyName("firstDayOfWeek")]
        public DayOfWeek FirstDayOfWeek
        {
            get
            {
                return this.ViewState.GetViewStateValue(vsKeyFirstDayOfWeek, DayOfWeek.Sunday);
            }
            set
            {
                this.ViewState.SetViewStateValue(vsKeyFirstDayOfWeek, value);
            }
        }

        [Description("当客户端选择一个值时执行的方法")]
        [Category("Behavior")]
        [DefaultValue("")]
        [ScriptControlEvent(true)]
        [ClientPropertyName("onClientSelectionChanged")]
        public string OnClientSelectionChanged
        {
            get
            {
                return this.ViewState.GetViewStateValue(vsKeyOnClientSelectionChanged, string.Empty);
            }
            set
            {
                this.ViewState.SetViewStateValue(vsKeyOnClientSelectionChanged, string.Empty);
            }
        }

        [Description("当客户端的值发生改变时发生")]
        [Category("Behavior")]
        [DefaultValue("")]
        [ScriptControlEvent(true)]
        [ClientPropertyName("onClientValueChanged")]
        public string OnClientValueChanged
        {
            get { return this.ViewState.GetViewStateValue(vsKeyOnClientValueChanged, string.Empty); }

            set { this.ViewState.SetViewStateValue(vsKeyOnClientValueChanged, string.Empty); }
        }

        [Description("当客户端日期输入错误时")]
        [Category("Behavior")]
        [DefaultValue("")]
        [ScriptControlEvent(true)]
        [ClientPropertyName("onClientErrorDate")]
        public string OnClientErrorDate
        {
            get { return this.ViewState.GetViewStateValue(vsKeyOnClientErrorDate, string.Empty); }

            set { this.ViewState.SetViewStateValue(vsKeyOnClientErrorDate, value); }
        }

        [Description("控件的值")]
        [Category("Behavior")]
        [DefaultValue(typeof(DateTime), "0001-01-01")]
        [ScriptControlProperty(true)]
        [ClientPropertyName("value")]
        public DateTime Value
        {
            get
            {
                return this.ViewState.GetViewStateValue(vsKeyValue, DateTime.MinValue);
            }
            set
            {
                this.ViewState.SetViewStateValue(vsKeyValue, value);
            }
        }

        [Description("时间的值")]
        [Category("Behavior")]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        [EditorBrowsable(EditorBrowsableState.Never)]
        [Browsable(false)]
        public TimeSpan TimeValue
        {
            get
            {

                return Value != DateTime.MinValue ? Value.TimeOfDay : TimeSpan.MinValue;
            }

            set
            {
                this.Value = DateTime.Now.Date + value;
            }
        }

        [Description("日期的值")]
        [Category("Behavior")]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        [EditorBrowsable(EditorBrowsableState.Never)]
        [Browsable(false)]
        public DateTime DateValue
        {
            get
            {

                return Value != DateTime.MinValue ? Value.Date : DateTime.MinValue;
            }

            set
            {
                this.Value = value;
            }
        }

        /// <summary>
        /// 获取或设置用于表示当天日期的值，如果未显式设置此属性，则默认为服务器日期
        /// </summary>
        [Description("用于表示当天日期的日期")]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        [Browsable(false)]
        [ScriptControlProperty(true)]
        [ClientPropertyName("dayOfToday")]
        public DateTime DayOfToday
        {
            get
            {
                object o = this.ViewState[vsKeyDayOfToday];

                return o != null ? (DateTime)o : DateTime.Now;
            }

            set { this.ViewState[vsKeyDayOfToday] = value; }
        }

        /// <summary>
        /// 获取或设置一个值，该值指示是否启用 Web 服务器控件。
        /// </summary>
        [Description("控件的已启用状态")]
        [Bindable(true)]
        [Themeable(false)]
        [DefaultValue(true)]
        [ScriptControlProperty(true)]
        [ClientPropertyName("enabled")]
        public override bool Enabled
        {
            get
            {
                return base.Enabled;
            }
            set
            {
                base.Enabled = value;
            }
        }

        /// <summary>
        /// 获取或设置一个值，该值指示是否启用 Web 服务器控件。
        /// </summary>
        [Description("控件的只读状态")]
        [Bindable(true)]
        [Themeable(false)]
        [DefaultValue(false)]
        [ScriptControlProperty(true)]
        [ClientPropertyName("readOnly")]
        public override bool ReadOnly
        {
            get
            {
                return base.ReadOnly;
            }
            set
            {
                base.ReadOnly = value;
            }
        }
        #endregion

        protected override void AddAttributesToRender(HtmlTextWriter writer)
        {
            if (this.ApplyDefaultCssClass)
            {
                this.ControlStyle.CssClass = "input-group " + this.ControlStyle.CssClass;
            }

            base.AddAttributesToRender(writer);
        }

        protected override void LoadClientState(string clientState)
        {
            if (string.IsNullOrEmpty(clientState) == false)
            {
                var obj = JSONSerializerExecute.Deserialize<DateTimePickerDataPack>(clientState);
                if (this.Mode != obj.Mode)
                    this.Mode = obj.Mode;

                if (this.Value != obj.Value)
                    this.Value = obj.Value;
            }
        }

        protected override string SaveClientState()
        {
            return JSONSerializerExecute.Serialize(new DateTimePickerDataPack(this.Mode, this.Value) { });
        }
    }

    /// <summary>
    /// 选择模式
    /// </summary>
    public enum DateTimePickerMode
    {
        /// <summary>
        /// 日期选择器
        /// </summary>
        DatePicker,

        /// <summary>
        /// 时间选择器
        /// </summary>
        TimePicker,

        /// <summary>
        /// 时间日期选择器
        /// </summary>
        DateTimePicker,
    }

    internal class DateTimePickerDataPack
    {
        private DateTimePickerMode mode;
        private DateTime dateVal;

        public DateTimePickerDataPack(DateTimePickerMode dateTimePickerMode, DateTime dateTime)
        {
            this.mode = dateTimePickerMode;
            this.dateVal = dateTime;
        }

        public DateTimePickerDataPack()
        {
        }

        public DateTimePickerMode Mode
        {
            get { return this.mode; }
            set { this.mode = value; }
        }

        public DateTime Value
        {
            get { return this.dateVal; }
            set { this.dateVal = value; }
        }
    }
}
