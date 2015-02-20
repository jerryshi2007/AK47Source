#region
// -------------------------------------------------
// Assembly	：	DeluxeWorks.Web.WebControls
// FileName	：	DeluxeTime.cs
// Remark	：	
// -------------------------------------------------
// VERSION  	AUTHOR		DATE			CONTENT
// 1.0		    吴伟	    20070720		创建
// -------------------------------------------------
#endregion


using System;
using System.Web;
using System.Web.UI.WebControls;
using System.Web.UI;
using System.ComponentModel;
using System.ComponentModel.Design;
using System.Web.UI.WebControls.WebParts;
using System.Collections;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using MCS.Web.Library.Script;
[assembly: System.Web.UI.WebResource("MCS.Web.WebControls.DeluxeTime.DeluxeTime.css", "text/css")]
[assembly: System.Web.UI.WebResource("MCS.Web.WebControls.DeluxeTime.DeluxeTime.js", "text/javascript")]
namespace MCS.Web.WebControls
{
	/// <summary>
	/// 时间控件类
	/// </summary>
	[DefaultProperty("MValue")]
	[RequiredScript(typeof(ControlBaseScript), 0)]
	[ClientCssResource("MCS.Web.WebControls.DeluxeTime.DeluxeTime.css")]
	[ClientScriptResource("MCS.Web.WebControls.DeluxeTime", "MCS.Web.WebControls.DeluxeTime.DeluxeTime.js")]
	public class DeluxeTime : ScriptControlBase
	{
		/// <summary>
		/// 构造函数
		/// </summary>
		/// <remarks></remarks>
		public DeluxeTime()
			: base(true, HtmlTextWriterTag.Input)
		{
			this.Attributes.Add("type", "text");
		}

		private string setTextBoxValue;
		private GenericInputExtender extender = new GenericInputExtender();

		/// <summary>
		/// 重写OnPreRender，附加样式可控制Button的输出
		/// </summary>
		/// <param name="e"></param>
		/// <remarks></remarks>
		protected override void OnPreRender(EventArgs e)
		{
			this.Attributes.Add("name", this.UniqueID);
			this.Attributes.Add("class", TextCssClass);
			this.Attributes.Add("value", MValue);

			if (ShowButton)
			{
				this.extender.TargetControlID = this.UniqueID;
				this.extender.Text = MValue;
				//this.extender.Items.Add(DataSource);
				this.Controls.Add(this.extender);
			}

			base.OnPreRender(e);
		}

		/// <summary>
		/// 重写Render，附加样式
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

				lb.Text = MValue;
				lb.ToolTip = this.ToolTip;
				lb.Visible = this.Visible;
				lb.Width = this.Width;

				lb.Style.Add("word-wrap", "break-word");

				lb.RenderControl(writer);

				base.Style["display"] = "none";
			}

			base.Render(writer);
		}

		/// <summary>
		/// 设置文本框的样式
		/// </summary>
		/// <remarks>设置文本框的样式</remarks>
		[Category("Appearance")]
		[Description("设置文本框的样式")]
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
		[Description("输入框样式")]
		[DesignerSerializationVisibility(DesignerSerializationVisibility.Content), NotifyParentProperty(true), PersistenceMode(PersistenceMode.InnerProperty)]
		public Style TextStyle
		{
			get { return this.style; }
			set { this.style = value; }
		}

		/// <summary>
		/// 时间格式串
		/// </summary>
		/// <remarks>时间格式串</remarks>
		[Category("Data")]
		[ScriptControlProperty]
		[Description("时间格式串")]
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
		[Description("掩码字符")]
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
		[Description("验证时间的提示信息")]
		public string CurrentMessageError
		{
			get { return GetPropertyValue("CurrentMessageError", string.Empty); }
			set { SetPropertyValue("CurrentMessageError", value); }
		}

		/// <summary>
		/// 是否提供按钮来选择自定义时间列表,若是则需设置数据源
		/// </summary>
		/// <remarks>是否提供按钮来选择自定义时间列表,若是则需设置数据源</remarks>
		[Category("Appearance")]
		[Description("是否提供按钮来选择自定义时间列表,若是则需设置数据源")]
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
		[Description("得到焦点时文本框的样式")]
		public string OnFocusCssClass
		{
			get { return GetPropertyValue("OnFocusCssClass", "MaskedEditFocus"); }
			set { SetPropertyValue("OnFocusCssClass", value); }
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
			get { return GetPropertyValue("OnInvalidCssClass", "MaskedEditError"); }
			set { SetPropertyValue("OnInvalidCssClass", value); }
		}

		/// <summary>
		/// 设置或是获取时间的值
		/// </summary>
		/// <remarks>设置或是获取时间的值</remarks>
		[Category("Default")]
		[Description("设置或是获取时间的值")]
		public string MValue
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
				try
				{
					if (string.IsNullOrEmpty(value) == false)
						this.setTextBoxValue = Convert.ToDateTime(value).ToString("HH:mm:ss");
					else
						this.setTextBoxValue = string.Empty;
				}
				catch
				{
					throw new InvalidCastException("输入框内的格式转换时间无效！");
				}
			}
		}

		/// <summary>
		/// 
		/// </summary>
		public string Value
		{
			get
			{
				if (Page.Request.Form[this.UniqueID] != null)
					return Page.Request.Form[this.UniqueID];
				else
					return null;
			}
		}

		/// <summary>
		/// 时间输入变化后触发的客户端事件
		/// </summary>
		/// <remarks>时间输入变化后触发的客户端事件</remarks>
		[DefaultValue("")]
		[Category("Action")]
		[ScriptControlEvent]
		[Description("时间输入变化后触发的客户端事件")]
		public string OnClientValueChanged
		{
			get { return GetPropertyValue("OnClientValueChanged", string.Empty); }
			set { SetPropertyValue("OnClientValueChanged", value); }
		}

		#region    列表部分
		private ListItem listItem = new ListItem();

		/// <summary>
		/// 数据源
		/// </summary>
		/// <remarks>ListItem类型的数据源</remarks>
		[Category("Data")]
		[Description("数据源")]
		public ListItemCollection DataSource
		{
			get { return this.extender.Items; }

		}
		#endregion
	}
}
