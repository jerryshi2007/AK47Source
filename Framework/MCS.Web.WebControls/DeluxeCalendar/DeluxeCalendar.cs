#region
// -------------------------------------------------
// Assembly	：	DeluxeWorks.Web.WebControls
// FileName	：	DeluxeDate.cs
// Remark	：	
// -------------------------------------------------
// VERSION  	AUTHOR		DATE			CONTENT
// 1.0		    吴伟	    20070720		创建
// -------------------------------------------------
#endregion

using System;
using System.Web.UI.WebControls;
using System.Web.UI;
using System.ComponentModel;
using System.ComponentModel.Design;
using System.Web.UI.WebControls.WebParts;
using MCS.Web.Library.Script;
using System.Web;
using MCS.Library.Globalization;
using System.Web.UI.HtmlControls;
#region [ Resources ]

[assembly: System.Web.UI.WebResource("MCS.Web.WebControls.DeluxeCalendar.DeluxeCalendar.js", "text/javascript")]
[assembly: System.Web.UI.WebResource("MCS.Web.WebControls.DeluxeCalendar.DeluxeCalendar.css", "text/css", PerformSubstitution = true)]
[assembly: System.Web.UI.WebResource("MCS.Web.WebControls.DeluxeCalendar.Images.arrow-left.gif", "image/gif")]
[assembly: System.Web.UI.WebResource("MCS.Web.WebControls.DeluxeCalendar.Images.arrow-right.gif", "image/gif")]
[assembly: System.Web.UI.WebResource("MCS.Web.WebControls.DeluxeCalendar.Images.caption.gif", "image/gif")]
[assembly: System.Web.UI.WebResource("MCS.Web.WebControls.DeluxeCalendar.Images.datePicker.gif", "image/gif")]
[assembly: System.Web.UI.WebResource("MCS.Web.WebControls.DeluxeCalendar.Images.updown.gif", "image/gif")]
[assembly: System.Web.UI.WebResource("MCS.Web.WebControls.DeluxeCalendar.Images.checked.gif", "image/gif")]
[assembly: System.Web.UI.WebResource("MCS.Web.WebControls.DeluxeCalendar.Images.today.gif", "image/gif")]

#endregion

namespace MCS.Web.WebControls
{
	/// <summary>
	/// 日期控件类
	/// </summary>
	[DefaultProperty("Value")]
	[RequiredScript(typeof(AnimationsScript), 4)]
	[ClientCssResource("MCS.Web.WebControls.DeluxeCalendar.DeluxeCalendar.css")]
	[ClientScriptResource("MCS.Web.WebControls.DeluxeCalendar", "MCS.Web.WebControls.DeluxeCalendar.DeluxeCalendar.js")]
	public class DeluxeCalendar : Web.Library.Script.ScriptControlBase
	{
		private string setTextBoxValue;
		
		/// <summary>
		/// DeluxeCalendar构造函数
		/// </summary>
		/// <remarks></remarks>
		public DeluxeCalendar()
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
		[Description("是否只显示当月")]
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
		[Description("日历的样式，不填为默认样式")]
		public override string CssClass
		{
			get { return GetPropertyValue("CssClass", "ajax__calendar"); }
			set { SetPropertyValue("CssClass", value); }
		}

		private Style style = new Style();
		/// <summary>
		/// 输入框样式
		/// </summary>
		/// <remarks>输入框样式</remarks>
		[Category("Appearance")]
		[Description("输入框样式")]
		[DesignerSerializationVisibility(DesignerSerializationVisibility.Content), NotifyParentProperty(true), PersistenceMode(PersistenceMode.InnerProperty)]
		public Style TextStyle
		{
			get { return style; }
			set { style = value; }
		}

		/// <summary>
		/// 
		/// </summary>
		[Category("Appearance")]
		[ScriptControlProperty]
		[ClientPropertyName("readOnly")]
		public new bool ReadOnly
		{
			get { return GetPropertyValue("InnerReadOnly", false); }
			set { SetPropertyValue("InnerReadOnly", value); }
		}

		/// <summary>
		/// 日历文本框的Css
		/// </summary>
		/// <remarks>日历文本框的Css</remarks>
		[Category("Appearance")]
		[Description("日历文本框的Css")]
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
		[Description("图片的Style")]
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
		[Description("图片的CssClass")]
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
		[Description("按钮的ID")]
		public string MaskedEditButtonID
		{
			get { return GetPropertyValue("MaskedEditButtonID", this.UniqueID + "_image"); }
			set { SetPropertyValue("MaskedEditButtonID", value); }
		}

		/// <summary>
		/// 按钮默认图片的Src
		/// </summary>
		/// <remarks>按钮默认图片的Src</remarks>
		[Browsable(false)]
		public string DefaultImageUrl
		{
			get { return Page.ClientScript.GetWebResourceUrl(typeof(DeluxeCalendar), "MCS.Web.WebControls.DeluxeCalendar.Images.datePicker.gif"); }
		}

		/// <summary>
		/// 按钮图片的Src
		/// </summary>
		/// <remarks>按钮图片的Src</remarks>
		[Category("Appearance")]
		[Description("按钮图片的Src")]
		[Editor("System.Web.UI.Design.ImageUrlEditor, System.Design", typeof(System.Drawing.Design.UITypeEditor))]
		public string ImageUrl
		{
			get { return GetPropertyValue("ImageUrl", string.Empty); }
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
		[Description("掩码字符")]
		public string PromptCharacter
		{
			get { return GetPropertyValue("PromptCharacter", "_"); }
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
		[Description("验证日期的提示信息")]
		public string CurrentMessageError
		{
			get { return GetPropertyValue("CurrentMessageError", Translator.Translate(Define.DefaultCategory, "请输入正确的日期！")); }
			set { SetPropertyValue("CurrentMessageError", value); }
		}

		/// <summary>
		/// 得到焦点时文本框的样式
		/// </summary>
		/// <remarks>得到焦点时文本框的样式</remarks>
		[Category("Appearance")]
		[ScriptControlProperty]
		[Description("得到焦点时文本框的样式")]
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
		[Description("验证不通过时文本框的样式")]
		public string OnInvalidCssClass
		{

			get { return GetPropertyValue("OnInvalidCssClass", "MaskedEditError"); ; }
			set
			{

				SetPropertyValue("OnInvalidCssClass", value);
			}

		}

		[ScriptControlProperty]
		[ClientPropertyName("imageButtonPath")]
		private string ImageButtonPath
		{
			get
			{
				return (ImageUrl == string.Empty) ? DefaultImageUrl : ResolveUrl(ImageUrl);
			}
		}


		#endregion
		/// <summary>
		/// 在页面输出文本框前分配控件id和绑定值
		/// </summary>
		/// <param name="e"></param>
		protected override void OnPreRender(System.EventArgs e)
		{
			base.Attributes.Add("name", this.UniqueID);
			base.Attributes.Add("value", CValue);

			//处理readonly
			if (this.ReadOnly)
			{
				Attributes.Add("readonly", "readonly");
			}
			base.OnPreRender(e);
		}

		/// <summary>
		/// 向页面输出文本框
		/// </summary>
		/// <param name="writer"></param>
		protected override void Render(HtmlTextWriter writer)
		{
			this.ApplyStyle(TextStyle);

			if (this.ReadOnly)
			{
				Label lb = new Label();
				lb.AccessKey = this.AccessKey;
				lb.AppRelativeTemplateSourceDirectory = this.AppRelativeTemplateSourceDirectory;

				foreach (string s in this.Attributes.Keys)
					lb.Attributes.Add(s, this.Attributes[s]);

				foreach (string s in this.Style.Keys)
					lb.Style.Add(s, this.Style[s]);

				lb.ForeColor = this.ForeColor;
				lb.Font.CopyFrom(this.Font);

				lb.ControlStyle.CopyFrom(this.ControlStyle);
				lb.Style["min-height"] = this.Height.ToString();
				lb.Style["height"] = string.Empty;
				//lb.ID = this.ID;
				lb.TabIndex = this.TabIndex;
				lb.TemplateControl = this.TemplateControl;

				string txt = this.Value != DateTime.MinValue ?
					HttpUtility.HtmlEncode(string.Format(this.DisplayFormat, this.Value)) :
					string.Empty;

				txt = txt.Replace("\r\n", "<br>");
				lb.Text = txt;
				lb.ToolTip = this.ToolTip;
				lb.Visible = this.Visible;
				lb.Width = this.Width;

				lb.Style.Add("word-wrap", "break-word");

				lb.RenderControl(writer);

				base.Style["display"] = "none";
				base.Render(writer);
			}
			else
			{
				base.Attributes.Add("class", TextCssClass);
				base.Render(writer);

				writer.Write(this.PlaceholderHtmlNode);

				//writer.Write("<img id='{0}' src='{1}' class='{2}' style='{3}' tabindex='-1' align='absmiddle' />",
				//    this.UniqueID + "_image", (ImageUrl == string.Empty) ? DefaultImageUrl : ResolveUrl(ImageUrl), ImageCssClass, ImageStyle,
				//    this.ReadOnly ? "onclick='return false;'" : string.Empty);
			}
		}

		/// <summary>
		/// 设置或是获取日历的值
		/// </summary>
		/// <remarks>设置或是获取日历的值</remarks>
		[Category("Default")]
		[ClientPropertyName("cValue")]
		[WebDisplayName("设置或是获取日历的值")]
		[Description("设置或是获取日历的值")]
		public string CValue
		{
			get
			{
				string result = null;

				if (this.setTextBoxValue != null)
				{
					result = this.setTextBoxValue;
				}
				else
				{
					if (Page.Request.Form[this.UniqueID] != null)
						result = Page.Request.Form[this.UniqueID];
					else
						result = null;
				}

				if (string.IsNullOrEmpty(result) == false)
					result = result.Trim('_', '-');

				return result;
			}
			set
			{
				try
				{
					if (string.IsNullOrEmpty(value) == false)
						this.setTextBoxValue = Convert.ToDateTime(value).ToString("yyyy-MM-dd");
					else
						this.setTextBoxValue = string.Empty;
				}
				catch
				{
					throw new InvalidCastException(Translator.Translate(Define.DefaultCategory, "输入框内的格式转换时间无效！"));
				}
			}
		}

		/// <summary>
		/// 只读状态下日期格式
		/// </summary>
		[DefaultValue("{0:yyyy-MM-dd}")]
		[Description("只读状态下日期格式")]
		public string DisplayFormat
		{
			get { return GetPropertyValue("DisplayFormat", "{0:yyyy-MM-dd}"); }
			set { SetPropertyValue("DisplayFormat", value); }
		}

		/// <summary>
		/// 是否启用日历功能
		/// </summary>
		/// <remarks>是否启用日历功能</remarks>
		[Category("Default")]
		[ScriptControlPropertyAttribute]
		[ClientPropertyName("enabled")]
		[Description("是否启用日历功能")]
		public bool EnabledOnClient
		{
			get { return GetPropertyValue("EnabledOnClient", true); }
			set { SetPropertyValue("EnabledOnClient", value); }
		}

		/// <summary>
		/// 是否启用日历动画效果
		/// </summary>
		/// <remarks>是否启用日历动画效果</remarks>
		[DefaultValue(true)]
		[ScriptControlPropertyAttribute]
		[ClientPropertyName("animated")]
		[Description("设置日历月份转换的动画效果")]
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
		[Description("是否提供下拉框快捷选项")]
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
		[Description("自定义第一列是从周几开始")]
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
		[ScriptControlEvent]
		[ClientPropertyName("onClientShowing")]
		[Description("弹出日历时触发的客户端事件")]
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
		[ScriptControlEvent]
		[ClientPropertyName("onClientShown")]
		[Description("弹出日历时后触发的客户端事件")]
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
		[ScriptControlEvent]
		[ClientPropertyName("onClientHiding")]
		[Description("隐藏日历时触发的客户端事件")]
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
		[ScriptControlEvent]
		[ClientPropertyName("onClientHidden")]
		[Description("隐藏日历后触发的客户端事件")]
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
		[ScriptControlEvent]
		[ClientPropertyName("onClientDateSelectionChanged")]
		[Description("日期选择变化后触发的客户端事件")]
		public string OnClientDateSelectionChanged
		{
			get { return GetPropertyValue("OnClientDateSelectionChanged", string.Empty); }
			set { SetPropertyValue("OnClientDateSelectionChanged", value); }
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
		/// 日期数据
		/// </summary>
		/// <remarks>日期数据</remarks>
		public DateTime Value
		{
			get
			{
				string cv = this.CValue;

				if (string.IsNullOrEmpty(cv) == false)
				{
					try
					{
						return Convert.ToDateTime(cv);
					}
					catch
					{
						throw new InvalidCastException(Translator.Translate(Define.DefaultCategory, "输入框内的格式转换时间无效！"));
					}
				}
				else
				{
					return DateTime.MinValue;/*返回默认值*/
				}
			}
			set
			{
				if (value == DateTime.MinValue)
					CValue = null;
				else
					CValue = value.Date.ToString("yyyy-MM-dd");
			}
		}

		/// <summary>
		/// 占位符ID
		/// </summary>
		[DefaultValue("div_placeholder")]
		[ScriptControlProperty()]
		[ClientPropertyName("placeholderID")]
		[Description("占位符ID")]
		public string PlaceholderID
		{
			get { return GetPropertyValue("PlaceholderID", this.ClientID + "_div_placeholder"); }
			set { SetPropertyValue("PlaceholderID", value); }
		}

		[Description("占位符Html")]
		private string PlaceholderHtmlNode
		{
			get
			{
				return "<div id='" + HttpUtility.HtmlAttributeEncode(PlaceholderID) + "' style='display:none;cursor: pointer'></div>";
			}
		}

		/// <summary>
		/// 
		/// </summary>
		[DefaultValue(1900)]
		[ScriptControlProperty()]
		[ClientPropertyName("startYear")]
		[Description("起始年")]
		public int StartYear
		{
			get { return GetPropertyValue("StartYear", 1900); }
			set { SetPropertyValue("StartYear", value); }
		}

		/// <summary>
		/// 
		/// </summary>
		[DefaultValue(2500)]
		[ScriptControlProperty()]
		[ClientPropertyName("endYear")]
		[Description("结束年")]
		public int EndYear
		{
			get { return GetPropertyValue("EndYear", 2500); }
			set { SetPropertyValue("EndYear", value); }
		}
	}
}

