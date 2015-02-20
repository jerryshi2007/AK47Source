#region
// -------------------------------------------------
// Assembly	：	DeluxeWorks.Web.WebControls
// FileName	：	DeluxeDateTime.cs
// Remark	：	
// -------------------------------------------------
// VERSION  	AUTHOR		DATE			CONTENT
// 1.0		    吴伟	    20070720		创建
// -------------------------------------------------
#endregion

using System;
using System.Collections.Generic;
using System.Text;
using MCS.Web.WebControls;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Text.RegularExpressions;
using System.ComponentModel;
using MCS.Web.Library.Script;

[assembly: System.Web.UI.WebResource("MCS.Web.WebControls.DeluxeDateTime.DeluxeDateTime.js", "text/javascript")]

namespace MCS.Web.WebControls
{
	/// <summary>
	/// 日期时间类
	/// </summary>
	[DefaultProperty("Value")]
	[RequiredScript(typeof(DeluxeCalendar), 1)]
	[RequiredScript(typeof(DeluxeTime), 2)]
	[ClientScriptResource("MCS.Web.WebControls.DeluxeDateTime", "MCS.Web.WebControls.DeluxeDateTime.DeluxeDateTime.js")]
	public class DeluxeDateTime : Web.Library.Script.ScriptControlBase
	{
		/// <summary>
		/// DeluxeDateTime的构造函数
		/// </summary>
		///<remarks></remarks>
		public DeluxeDateTime()
			: base(false, HtmlTextWriterTag.Span)
		{
		}

		private DeluxeCalendar calendar = new DeluxeCalendar();
		private DeluxeTime maskededit = new DeluxeTime();

		/// <summary>
		/// 
		/// </summary>
		/// <param name="e"></param>
		/// <remarks></remarks>
		protected override void OnInit(EventArgs e)
		{
			Controls.Add(this.calendar);
			Controls.Add(this.maskededit);
			Controls.Add(datetimeValue);

			base.OnInit(e);
		}

		/// <summary>
		/// 控制是否只读
		/// </summary>
		/// <param name="e"></param>
		protected override void OnPreRender(EventArgs e)
		{
			this.calendar.ReadOnly = this.ReadOnly;
			this.maskededit.ReadOnly = this.ReadOnly;
			//this.datetimeValue.Visible = this.ReadOnly;
			if (this.ReadOnly)
			{
				this.maskededit.Style.Add(HtmlTextWriterStyle.MarginLeft, "2px");
			}
			base.OnPreRender(e);
		}

		#region 时间部分

		/// <summary>
		/// 时间输入框的Style
		/// </summary>
		///<remarks>时间输入框的Style</remarks>
		[Description("时间输入框的Style")]
		public Style TimeTextStyle
		{
			get { return this.maskededit.TextStyle; }
			set { this.maskededit.TextStyle = value; }
		}

		/// <summary>
		/// 时间输入框的Css
		/// </summary>
		/// <remarks>时间输入框的Css</remarks>
		[Description("时间输入框的Css")]
		public string TimeTextCssClass
		{
			get { return this.maskededit.TextCssClass; }
			set { this.maskededit.TextCssClass = value; }
		}

		/// <summary>
		/// 时间的掩码
		/// </summary>
		/// <remarks>时间的掩码</remarks>
		[Description("时间的分隔符")]
		public string TimeMask
		{
			get { return this.maskededit.Mask; }
			set { this.maskededit.Mask = value; }
		}

		/// <summary>
		/// 掩码字符
		/// </summary>
		/// <remarks>掩码字符</remarks>
		[DefaultValue("_")]
		[Description("掩码字符")]
		public string TimePromptCharacter
		{
			get { return this.maskededit.PromptCharacter; }
			set { this.maskededit.PromptCharacter = value; }
		}

		/// <summary>
		/// 是否自动补齐时间
		/// </summary>
		/// <remarks>是否自动补齐时间</remarks>
		[Description("是否自动补齐时间")]
		public bool TimeAutoComplete
		{
			get { return this.maskededit.AutoComplete; }
			set { this.maskededit.AutoComplete = value; }
		}

		/// <summary>
		/// 提供自动补齐的时间串，不设置则取系统时间
		/// </summary>
		/// <remarks>提供自动补齐的时间串，不设置则取系统时间</remarks>
		[Description("提供自动补齐的时间串，不设置则取系统时间")]
		public string TimeAutoCompleteValue
		{
			get { return this.maskededit.AutoCompleteValue; }
			set { this.maskededit.AutoCompleteValue = value; }
		}

		/// <summary>
		/// 是否启用时间验证
		/// </summary>
		/// <remarks>是否启用时间验证</remarks>
		[Description("是否启用时间验证")]
		public bool IsValidTimeValue
		{
			get { return this.maskededit.IsValidValue; }
			set { this.maskededit.IsValidValue = value; }
		}

		/// <summary>
		/// 是否提供按钮来选择自定义时间列表,若是则需设置数据源
		/// </summary>
		/// <remarks>是否提供按钮来选择自定义时间列表,若是则需设置数据源</remarks>
		[Description("是否提供按钮来选择自定义时间列表,若是则需设置数据源")]
		public bool ShowButton
		{
			get { return this.maskededit.ShowButton; }
			set { this.maskededit.ShowButton = value; }
		}

		/// <summary>
		/// 验证时间的提示信息
		/// </summary>
		/// <remarks>验证时间的提示信息</remarks>
		[Description("验证时间的提示信息")]
		public string TimeCurrentMessageError
		{
			get { return this.maskededit.CurrentMessageError; }
			set { this.maskededit.CurrentMessageError = value; }
		}

		/// <summary>
		/// 得到焦点时时间文本框的样式
		/// </summary>
		/// <remarks>得到焦点时时间文本框的样式</remarks>
		[Description("得到焦点时时间文本框的样式")]
		public string OnTimeFocusCssClass
		{
			get { return this.maskededit.OnFocusCssClass; }
			set { this.maskededit.OnFocusCssClass = value; }
		}

		/// <summary>
		/// 验证不通过时时间文本框的样式
		/// </summary>
		/// <remarks>验证不通过时时间文本框的样式</remarks>
		[Description("验证不通过时时间文本框的样式")]
		public string OnTimeInvalidCssClass
		{
			get { return this.maskededit.OnInvalidCssClass; }
			set { this.maskededit.OnInvalidCssClass = value; }
		}

		/// <summary>
		/// 绑定时间的数据源
		/// </summary>
		/// <remarks>ListItem类型的数据源</remarks>
		[Description("绑定时间的数据源")]
		public ListItemCollection DataSource
		{
			get { return this.maskededit.DataSource; }
		}

		#endregion

		#region 日期部分
		/// <summary>
		/// 是否只显示当月
		/// </summary>
		/// <remarks>是否只显示当月</remarks>
		[Description("是否只显示当月")]
		public bool IsOnlyCurrentMonth
		{
			get { return this.calendar.IsOnlyCurrentMonth; }
			set { this.calendar.IsOnlyCurrentMonth = value; }
		}

		/// <summary>
		/// 设置日历月份转换的动画效果
		/// </summary>
		/// <remarks>设置日历月份转换的动画效果</remarks>
		[Description("设置日历月份转换的动画效果")]
		public bool Animated
		{
			get { return this.calendar.Animated; }
			set { this.calendar.Animated = value; }
		}

		/// <summary>
		/// 日历的样式，不填为默认样式
		/// </summary>
		/// <remarks>日历的样式，不填为默认样式</remarks>
		[Description("日历的样式，不填为默认样式")]
		public string PopupCalendarCssClass
		{
			get { return this.calendar.CssClass; }
			set { this.calendar.CssClass = value; }
		}

		/// <summary>
		/// 日历的Style
		/// </summary>
		/// <remarks>日历的Style</remarks>
		[Description("日历的Style")]
		public Style DateTextStyle
		{
			get { return this.calendar.TextStyle; }
			set { this.calendar.TextStyle = value; }
		}

		/// <summary>
		/// 日历输入框的Css
		/// </summary>
		/// <remarks>日历输入框的Css</remarks>
		[Description("日历输入框的Css")]
		public string DateTextCssClass
		{
			get { return this.calendar.TextCssClass; }
			set { this.calendar.TextCssClass = value; }
		}

		/// <summary>
		/// 图片的Style
		/// </summary>
		/// <remarks>图片的Style</remarks>
		[Description("图片的Style")]
		public string DateImageStyle
		{
			get { return this.calendar.ImageStyle; }
			set { this.calendar.ImageStyle = value; }
		}

		/// <summary>
		/// 图片的CssClass
		/// </summary>
		/// <remarks>图片的CssClass</remarks>
		[Description("图片的CssClass")]
		public string DateImageCssClass
		{
			get { return this.calendar.ImageCssClass; }
			set { this.calendar.ImageCssClass = value; }
		}

		/// <summary>
		/// 按钮图片的Src
		/// </summary>
		/// <remarks>按钮图片的Src</remarks>
		[Description("按钮图片的Src")]
		public string DateImageUrl
		{
			get { return this.calendar.ImageUrl; }
			set { this.calendar.ImageUrl = value; }
		}

		#region     日期掩码部分
		/// <summary>
		/// 日期掩码字符
		/// </summary>
		/// <remarks>日期掩码字符</remarks>
		[Description("日期掩码字符")]
		public string DatePromptCharacter
		{
			get { return this.calendar.PromptCharacter; }
			set { this.calendar.PromptCharacter = value; }
		}

		/// <summary>
		/// 是否自动补齐日期
		/// </summary>
		/// <remarks>是否自动补齐日期</remarks>
		[Description("是否自动补齐日期")]
		public bool DateAutoComplete
		{
			get { return this.calendar.AutoComplete; }
			set { this.calendar.AutoComplete = value; }
		}

		/// <summary>
		/// 提供自动补齐的时间串，不设置则取系统日期
		/// </summary>
		/// <remarks>提供自动补齐的时间串，不设置则取系统日期</remarks>
		[Description("提供自动补齐的时间串，不设置则取系统日期")]
		public string DateAutoCompleteValue
		{
			get { return this.calendar.AutoCompleteValue; }
			set { this.calendar.AutoCompleteValue = value; }
		}

		/// <summary>
		/// 是否启用日期验证
		/// </summary>
		/// <remarks>是否启用日期验证</remarks>
		[Description("是否启用日期验证")]
		public bool IsValidDateValue
		{
			get { return this.calendar.IsValidValue; }
			set { this.calendar.IsValidValue = value; }
		}

		/// <summary>
		/// 验证日期的提示信息
		/// </summary>
		/// <remarks>验证日期的提示信息</remarks>
		[Description("验证日期的提示信息")]
		public string DateCurrentMessageError
		{
			get { return this.calendar.CurrentMessageError; }
			set { this.calendar.CurrentMessageError = value; }
		}

		/// <summary>
		/// 日期得到焦点时文本框的样式
		/// </summary>
		/// <remarks>日期得到焦点时文本框的样式</remarks>
		[Description("日期得到焦点时文本框的样式")]
		public string OnFocusDateCssClass
		{
			get { return this.calendar.OnFocusCssClass; }
			set { this.calendar.OnFocusCssClass = value; }
		}

		/// <summary>
		/// 日期验证不通过时文本框的样式
		/// </summary>
		/// <remarks>日期验证不通过时文本框的样式</remarks>
		[Description("日期验证不通过时文本框的样式")]
		public string OnInvalidDateCssClass
		{
			get { return this.calendar.OnInvalidCssClass; }
			set { this.calendar.OnInvalidCssClass = value; }
		}
		#endregion

		/// <summary>
		/// 是否启用日历功能
		/// </summary>
		/// <remarks>是否启用日历功能</remarks>
		[DefaultValue(true)]
		[Description("是否启用日历功能")]
		public virtual bool EnabledOnClient
		{
			get { return this.calendar.EnabledOnClient; }
			set { this.calendar.EnabledOnClient = value; }
		}

		/// <summary>
		/// 是否提供下拉框快捷选项
		/// </summary>
		/// <remarks>是否提供下拉框快捷选项</remarks>
		[Description("是否提供下拉框快捷选项")]
		public bool IsComplexHeader
		{
			get { return this.calendar.IsComplexHeader; }
			set { this.calendar.IsComplexHeader = value; }
		}

		/// <summary>
		/// 自定义第一列是从周几开始
		/// </summary>
		/// <remarks>自定义第一列是从周几开始</remarks>
		[Description("自定义第一列是从周几开始")]
		public FirstDayOfWeek FirstDayOfWeek
		{
			get { return this.calendar.FirstDayOfWeek; }
			set { this.calendar.FirstDayOfWeek = value; }
		}

		/// <summary>
		/// 弹出日历时触发的客户端事件
		/// </summary>
		/// <remarks>弹出日历时触发的客户端事件</remarks>
		[Description("弹出日历时触发的客户端事件")]
		public string OnClientShowing
		{
			get { return this.calendar.OnClientShowing; }
			set { this.calendar.OnClientShowing = value; }
		}

		/// <summary>
		/// 弹出日历时后触发的客户端事件
		/// </summary>
		/// <remarks>弹出日历时后触发的客户端事件</remarks>
		[Description("弹出日历时后触发的客户端事件")]
		public string OnClientShown
		{
			get { return this.calendar.OnClientShown; }
			set { this.calendar.OnClientShown = value; }
		}

		/// <summary>
		/// 隐藏日历时触发的客户端事件
		/// </summary>
		/// <remarks>隐藏日历时触发的客户端事件</remarks>
		[Description("隐藏日历时触发的客户端事件")]
		public string OnClientHiding
		{
			get { return this.calendar.OnClientHiding; }
			set { this.calendar.OnClientHiding = value; }
		}

		/// <summary>
		/// 隐藏日历后触发的客户端事件
		/// </summary>
		/// <remarks>隐藏日历后触发的客户端事件</remarks>
		[Description("隐藏日历后触发的客户端事件")]
		public string OnClientHidden
		{
			get { return this.calendar.OnClientHidden; }
			set { this.calendar.OnClientHidden = value; }
		}

		/// <summary>
		/// 日期选择变化后触发的客户端事件
		/// </summary>
		/// <remarks>日期选择变化后触发的客户端事件</remarks>
		[Description("日期选择变化后触发的客户端事件")]
		public string OnClientDateSelectionChanged
		{
			get { return this.calendar.OnClientDateSelectionChanged; }
			set { this.calendar.OnClientDateSelectionChanged = value; }
		}

		/// <summary>
		/// 日期或时间输入变化后触发的客户端事件
		/// </summary>
		[DefaultValue("")]
		[Category("Action")]
		[ScriptControlEvent()]
		[ClientPropertyName("clientValueChanged")]
		[Description("日期或时间输入变化后触发的客户端事件")]
		public string OnClientValueChanged
		{
			get { return GetPropertyValue("OnClientValueChanged", string.Empty); }
			set { SetPropertyValue("OnClientValueChanged", value); }
		}

		/// <summary>
		/// 控件使用时一些额外的参数
		/// </summary>
		[ScriptControlProperty]
		[ClientPropertyName("tag")]
		[DefaultValue("")]
		[Description("控件使用时一些额外的参数")]
		public string Tag
		{
			get { return GetPropertyValue("Tag", string.Empty); }
			set { SetPropertyValue("Tag", value); }
		}
		#endregion

		/// <summary>
		/// 日期数据
		/// </summary>
		/// <remarks>日期数据</remarks>
		public DateTime Value
		{
			get
			{
				if (this.ReadOnly)
				{
					return string.IsNullOrEmpty(datetimeValue.Text) ? DateTime.MinValue : Convert.ToDateTime(datetimeValue.Text);
				}
				else
				{
					string cv = this.calendar.CValue;
					string mv = this.maskededit.MValue;

					if (string.IsNullOrEmpty(cv) == false)
					{
						string data = cv;

						if (string.IsNullOrEmpty(mv) == false)
							data += " " + mv;

						try
						{
							return Convert.ToDateTime(data);
						}
						catch
						{
							throw new InvalidCastException("输入框内的格式转换时间无效！");
						}
					}
					else
					{
						return DateTime.MinValue;/*返回默认值*/
					}
				}
			}
			set
			{
				if (this.ReadOnly)
				{
					if (value == DateTime.MinValue)
						datetimeValue.Text = string.Empty;
					else
						datetimeValue.Text = value.ToString("yyyy-MM-dd HH:mm:ss");
				}
				else
				{
					if (value == DateTime.MinValue)
					{
						this.calendar.CValue = string.Empty;
						this.maskededit.MValue = string.Empty;
					}
					else
					{
						this.calendar.CValue = value.Date.ToString("yyyy-MM-dd");
						this.maskededit.MValue = value.ToString("HH:mm:ss");
					}
				}
			}
		}

		private Label datetimeValue = new Label();

		[ScriptControlProperty]
		[ClientPropertyName("calendarControlID")]
		private string CalendarControlID
		{
			get
			{
				return this.calendar.ClientID;
			}
		}

		[ScriptControlProperty]
		[ClientPropertyName("timeControlID")]
		private string TimeControlID
		{
			get
			{
				return this.maskededit.ClientID;
			}
		}
	}
}
