using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using MCS.Web.Library;

namespace PermissionCenter.WebControls
{
	#region 枚举
	/// <summary>
	/// 表示通知的类型
	/// </summary>
	public enum NoticeType
	{
		/// <summary>
		/// 自动,当设置了Text，或者错误数不为0，则自动显示。
		/// </summary>
		Auto,

		/// <summary>
		/// 不显示
		/// </summary>
		None,

		/// <summary>
		/// 信息类提示
		/// </summary>
		Info,

		/// <summary>
		/// 警告消息
		/// </summary>
		Warning,

		/// <summary>
		/// 错误消息
		/// </summary>
		Error,
	}

	#endregion

	/// <summary>
	/// 标题消息控件
	/// </summary>
	/// <remarks>此控件仅供一次性操作，页面往返之间不保存状态，也不会响应用户操作。</remarks>
	[ParseChildren(false)]
	[DefaultProperty("Text")]
	[ToolboxData(@"<{0}:BannerNotice runat=""server""></{0}:BannerNotice>")]
	[Designer("MCS.Web.WebControls.GenericControlDesigner, MCS.Web.WebControls")]
	public class BannerNotice : WebControl, IScriptControl
	{
		#region 字段

		private static readonly object EventItemRendering = new object();

		private object renderObject = new object();
		private bool textSetByAddParsedSubObject = false;
		private string rendingContent = string.Empty;
		private string closeButtonText = "我已了解";
		private string reportButtonText = "发送错误报告";
		private string detailButtonText = "显示详情";
		private string mailAddr = null;
		private bool reportVisible = false;
		private int autoHideDuration = 10000;
		private string clientClose = null;
		private NoticeType renderingNoticeType = NoticeType.Auto;
		private bool encodeText = true;
		private IList<object> errs = null;
		private ScriptManager sm = null;
		private bool useConfigMail = true;

		#endregion

		#region 构造函数
		public BannerNotice()
			: base(HtmlTextWriterTag.Div)
		{
		}

		#endregion

		#region 事件
		[Category("Behavior")]
		[Description("当渲染时发生")]
		public event EventHandler<NoticeRenderEventArgs> ErrorItemRendering
		{
			add { this.Events.AddHandler(EventItemRendering, value); }

			remove { this.Events.RemoveHandler(EventItemRendering, value); }
		}

		#endregion

		#region 属性

		/// <summary>
		/// 此属性请无视
		/// </summary>
		[Browsable(false)]
		public override string CssClass
		{
			get
			{
				return base.CssClass;
			}

			set
			{
			}
		}

		/// <summary>
		/// 获取一个<see cref="IList&lt;string&gt;"/>，表示页面包含的错误集合
		/// </summary>
		[Bindable(false)]
		[Browsable(false)]
		[Localizable(false)]
		public IList<object> Errors
		{
			get
			{
				if (this.errs == null)
					this.errs = new List<object>();
				return this.errs;
			}
		}

		/// <summary>
		/// 获取一个值，表示是否包含了任何错误。
		/// </summary>
		/// <value><see langword="true"/>表示含有错误 或 <see langword="false"/>表示不含有错误。</value>
		[Browsable(false)]
		public bool HasErrors
		{
			get
			{
				return this.errs != null && this.errs.Count > 0;
			}
		}

		/// <summary>
		/// 获取或设置要显示的内容标题（可包含HTML）
		/// </summary>
		[Bindable(true)]
		[Category("Appearance")]
		[Description("内容标题")]
		[DefaultValue("")]
		[Localizable(true)]
		public string Text
		{
			get
			{
				return this.rendingContent;
			}

			set
			{
				if (this.HasControls())
				{
					// 如果有子控件，则清除子控件
					this.Controls.Clear();
				}

				this.rendingContent = value ?? string.Empty;
			}
		}

		/// <summary>
		/// 获取或设置一个值，表示是否对Text属性中的文字进行HTML编码
		/// </summary>
		[Bindable(true)]
		[Category("Appearance")]
		[Description("是否对标题文字进行HTML编码")]
		[DefaultValue(true)]
		[Localizable(true)]
		public bool EncodeText
		{
			get { return this.encodeText; }
			set { this.encodeText = value; }
		}

		/// <summary>
		/// 获取或设置显示在显示详情链接上的文字
		/// </summary>
		[Bindable(true)]
		[Category("Appearance")]
		[DefaultValue("显示详情")]
		[Description("显示在显示详情按钮上的文字")]
		[Localizable(true)]
		public string DetailButtonText
		{
			get { return this.detailButtonText; }
			set { this.detailButtonText = value; }
		}

		/// <summary>
		/// 获取或设置显示在关闭按钮上的文字
		/// </summary>
		[Bindable(true)]
		[Category("Appearance")]
		[DefaultValue("我已了解")]
		[Description("显示在关闭按钮上的文字")]
		[Localizable(true)]
		public string CloseButtonText
		{
			get { return this.closeButtonText; }
			set { this.closeButtonText = value; }
		}

		/// <summary>
		/// 获取或设置显示在错误报告按钮上的文字
		/// </summary>
		[Bindable(true)]
		[Category("Appearance")]
		[DefaultValue("发送错误报告")]
		[Description("显示在发送错误报告按钮上的文字")]
		[Localizable(true)]
		public string ReportButtonText
		{
			get { return this.reportButtonText; }
			set { this.reportButtonText = value; }
		}

		/// <summary>
		/// 获取或设置错误报告按钮是否可用
		/// </summary>
		[Bindable(true)]
		[Category("Appearance")]
		[Description("确定是否显示发送错误报告按钮")]
		[DefaultValue(false)]
		public bool ReportButtonVisible
		{
			get { return this.reportVisible; }
			set { this.reportVisible = value; }
		}

		/// <summary>
		/// 获取或设置发送错误报告的接收地址
		/// </summary>
		[Bindable(true)]
		[Category("Appearance")]
		[DefaultValue("")]
		[Description("接收错误报告的电子邮件地址")]
		public string ReportMailAddress
		{
			get
			{
				if (this.ReportMailAddressByConfig)
				{
					if (this.DesignMode)
					{
						return "从配置获取";
					}
					else
					{
						return ApplicationErrorLogSection.GetSection().NotifyMailAddress;
					}
				}

				return this.mailAddr;
			}

			set
			{
				this.mailAddr = value;
			}
		}

		/// <summary>
		/// 获取或设置一个值，表示是否从配置文件读取邮件地址
		/// </summary>
		[Bindable(true)]
		[Category("Appearance")]
		[DefaultValue(true)]
		[Description("从配置文件读取错误报告邮件地址")]
		public bool ReportMailAddressByConfig
		{
			get { return this.useConfigMail; }
			set { this.useConfigMail = value; }
		}

		/// <summary>
		/// 获取或设置在用户关闭通知时执行的客户端操作。
		/// </summary>
		[Bindable(true)]
		[Category("Behavior")]
		[Description("当单击关闭按钮时需要执行的客户端操作")]
		public string OnClientClose
		{
			get { return this.clientClose; }
			set { this.clientClose = value; }
		}

		/// <summary>
		/// 获取或设置呈现类型
		/// </summary>
		[Bindable(true)]
		[Category("Appearance")]
		[Description("呈现的类型")]
		[DefaultValue(NoticeType.Auto)]
		[Localizable(false)]
		public NoticeType RenderType
		{
			get
			{
				return this.renderingNoticeType;
			}

			set
			{
				if (Enum.IsDefined(typeof(NoticeType), value))
					this.renderingNoticeType = value;
				else
					throw new InvalidEnumArgumentException();
			}
		}

		public override bool SupportsDisabledAttribute
		{
			get
			{
				return false;
			}
		}

		/// <summary>
		/// 获取一个值，表示该控件是否应该被初始化为可见
		/// </summary>
		[Browsable(false)]
		public bool IsClientVisibble
		{
			get
			{
				if (this.renderingNoticeType == NoticeType.None)
					return false;
				else if (this.renderingNoticeType == NoticeType.Auto)
					return string.IsNullOrEmpty(this.Text) == false || this.HasErrors;
				else
					return true;
			}
		}

		/// <summary>
		/// 获取或设置在自动隐藏通知前需要持续的时间
		/// </summary>
		/// <value>0表示不隐藏，或一个自动隐藏前的时间的毫秒值</value>
		[Bindable(true)]
		[Category("Appearance")]
		[Description("自动隐藏的时间")]
		[DefaultValue(10000)]
		[Localizable(false)]
		public int AutoHideDuration
		{
			get
			{
				return this.autoHideDuration;
			}

			set
			{
				if (value < 0)
					value = 0;
				this.autoHideDuration = value;
			}
		}
		#endregion

		#region 公开的方法

		public void AddErrorInfo(Exception ex)
		{
			this.Errors.Add(ex);
		}

		public void AddErrorInfo(string message)
		{
			this.Errors.Add(message);
		}

		public IEnumerable<ScriptDescriptor> GetScriptDescriptors()
		{
			ScriptControlDescriptor descriptor = new ScriptControlDescriptor("PermissionCenter.BannerNotice", this.ClientID);
			descriptor.AddProperty("clientVisible", this.IsClientVisibble);
			descriptor.AddProperty("reportMailAddress", this.ReportMailAddress);
			descriptor.AddProperty("autoHideDuration", this.AutoHideDuration);

			return new ScriptDescriptor[] { descriptor };
		}

		public IEnumerable<ScriptReference> GetScriptReferences()
		{
			ScriptReference reference = new ScriptReference();
			reference.Path = this.ResolveClientUrl("~/scripts/BannerNotice.js");

			return new ScriptReference[] { reference };
		}
		#endregion

		#region 受保护的方法
		/// <summary>
		/// 引发<see cref="ErrorItemRendering"/>事件。
		/// </summary>
		/// <param name="e"></param>
		protected virtual void OnErrorItemRendering(NoticeRenderEventArgs e)
		{
			var handler = (EventHandler<NoticeRenderEventArgs>)this.Events[EventItemRendering];
			if (handler != null)
				handler(this, e);
		}

		protected override void OnPreRender(EventArgs e)
		{
			if (!this.DesignMode)
			{
				this.sm = ScriptManager.GetCurrent(this.Page);
				if (this.sm == null)
					throw new HttpException("页面上必须有ScriptManager控件");

				this.sm.RegisterScriptControl(this);
			}

			base.OnPreRender(e);

			if (this.IsClientVisibble && this.ReportButtonVisible && this.useConfigMail == false && string.IsNullOrEmpty(this.ReportMailAddress))
			{
				throw new HttpException("BannerNotice中设置了报告问题按钮可见，但未设置接收报告的地址。");
			}
		}

		protected override void Render(HtmlTextWriter writer)
		{
			if (!this.DesignMode)
				this.sm.RegisterScriptDescriptors(this);
			this.EnsureChildControls();
			base.Render(writer);
		}

		protected override void AddAttributesToRender(HtmlTextWriter writer)
		{
			string typeStyle;
			switch (this.renderingNoticeType)
			{
				case NoticeType.Auto:
					if (this.errs != null && this.errs.Count > 0)
					{
						typeStyle = "pc-error";
					}
					else if (string.IsNullOrEmpty(this.Text) == false)
					{
						typeStyle = "pc-info";
					}
					else
					{
						typeStyle = "pc-hidden";
					}

					break;
				case NoticeType.None:
					typeStyle = "pc-hidden";
					break;
				case NoticeType.Info:
					typeStyle = "pc-info";
					break;
				case NoticeType.Warning:
					typeStyle = "pc-warning";
					break;
				case NoticeType.Error:
					typeStyle = "pc-error";
					break;
				default:
					typeStyle = "pc-hidden";
					break;
			}

			writer.AddAttribute(HtmlTextWriterAttribute.Class, "pc-notice " + typeStyle);

			base.AddAttributesToRender(writer);
		}

		protected override void RenderContents(HtmlTextWriter writer)
		{
			this.RenderSummary(writer);

			this.RenderDetails(writer);
		}

		protected virtual void RenderDetails(HtmlTextWriter writer)
		{
			writer.AddAttribute("data-actrole", "details");
			writer.AddAttribute(HtmlTextWriterAttribute.Class, "pc-details");
			writer.RenderBeginTag(HtmlTextWriterTag.Div);
			if (this.HasErrors)
			{
				NoticeRenderEventArgs e = new NoticeRenderEventArgs(writer);
				writer.AddStyleAttribute(HtmlTextWriterStyle.Display, "none");
				writer.RenderBeginTag(HtmlTextWriterTag.Dir);
				string[] env = MCS.Library.Core.EnvironmentHelper.GetEnvironmentInfo().Split(new string[] { Environment.NewLine }, StringSplitOptions.RemoveEmptyEntries);
				foreach (string line in env)
				{
					writer.RenderBeginTag(HtmlTextWriterTag.P);
					writer.WriteEncodedText(line);
					writer.RenderEndTag();
				}

				writer.RenderEndTag();

				writer.RenderBeginTag(HtmlTextWriterTag.Ol);
				foreach (object o in this.Errors)
				{
					e.Item = o;
					this.OnErrorItemRendering(e); // 允许对呈现进行控制
					if (e.PreventDefault == false)
					{
						writer.AddAttribute(HtmlTextWriterAttribute.Class, "pc-item");
						writer.RenderBeginTag(HtmlTextWriterTag.Li);
						if (o is string)
						{
							this.RenderStringItem(o as string, writer);
						}
						else if (o is Exception)
						{
							this.RenderErrorItem(o as Exception, writer);
						}
						else
						{
							this.RenderObjectItem(o, writer);
						}

						writer.RenderEndTag();
					}
					else
					{
						e.PreventDefault = false;
					}
				}

				writer.RenderEndTag();
			}

			writer.RenderEndTag();
		}

		protected virtual void RenderObjectItem(object o, HtmlTextWriter writer)
		{
			writer.WriteEncodedText(o.ToString());
		}

		protected virtual void RenderStringItem(string message, HtmlTextWriter writer)
		{
			writer.WriteEncodedText(message);
		}

		protected virtual void RenderErrorItem(Exception err, HtmlTextWriter writer)
		{
			writer.AddAttribute("data-actrole", "toggle");
			writer.AddAttribute(HtmlTextWriterAttribute.Class, "pc-subject");
			writer.RenderBeginTag(HtmlTextWriterTag.Div);

			writer.AddAttribute(HtmlTextWriterAttribute.Class, "pc-handle");
			writer.RenderBeginTag(HtmlTextWriterTag.I);
			writer.RenderEndTag();
			writer.RenderBeginTag(HtmlTextWriterTag.Span);
			writer.WriteEncodedText(err.Message);
			writer.RenderEndTag();
			writer.RenderEndTag();

			writer.AddAttribute(HtmlTextWriterAttribute.Class, "pc-more");
			writer.RenderBeginTag(HtmlTextWriterTag.Div);
			writer.RenderBeginTag(HtmlTextWriterTag.Div);
			writer.WriteEncodedText(err.GetType().AssemblyQualifiedName);
			writer.RenderEndTag();
			writer.RenderBeginTag(HtmlTextWriterTag.Div);
			writer.WriteEncodedText(err.Source ?? string.Empty);
			writer.RenderEndTag();
			writer.AddAttribute(HtmlTextWriterAttribute.Class, "pc-stack");
			writer.RenderBeginTag(HtmlTextWriterTag.Div);
			if (err.StackTrace != null)
			{
				string[] lines = err.StackTrace.Split(new string[] { Environment.NewLine }, StringSplitOptions.RemoveEmptyEntries);
				foreach (string line in lines)
				{
					writer.RenderBeginTag(HtmlTextWriterTag.P);
					writer.WriteEncodedText(line);
					writer.RenderEndTag();
				}
			}

			writer.RenderEndTag();
			writer.RenderEndTag();
		}

		/// <summary>
		/// 如果控件包含子元素，则将其视为Text
		/// </summary>
		/// <param name="obj"></param>
		protected override void AddParsedSubObject(object obj)
		{
			if (this.HasControls())
			{
				base.AddParsedSubObject(obj);
			}
			else if (obj is LiteralControl)
			{
				if (this.textSetByAddParsedSubObject)
				{
					this.Text = this.Text + ((LiteralControl)obj).Text;
				}
				else
				{
					this.Text = ((LiteralControl)obj).Text;
				}

				this.textSetByAddParsedSubObject = true;
			}
			else
			{
				string text = this.Text;
				if (text.Length != 0)
				{
					this.Text = null;
					base.AddParsedSubObject(new LiteralControl(text));
				}

				base.AddParsedSubObject(obj);
			}
		}

		#endregion

		#region 私有的方法

		private void RenderSummary(HtmlTextWriter writer)
		{
			writer.AddAttribute("data-actrole", "summary");
			writer.AddAttribute(HtmlTextWriterAttribute.Class, "pc-summary");
			writer.RenderBeginTag(HtmlTextWriterTag.Div);
			if (string.IsNullOrEmpty(this.rendingContent) == false)
			{
				writer.AddAttribute(HtmlTextWriterAttribute.Class, "pc-message");
				writer.RenderBeginTag(HtmlTextWriterTag.Span);
				if (this.encodeText)
					writer.WriteEncodedText(this.rendingContent);
				else
					writer.Write(this.rendingContent);
				writer.RenderEndTag();
			}
			else if (this.HasErrors)
			{
				writer.AddAttribute(HtmlTextWriterAttribute.Class, "pc-message");
				writer.RenderBeginTag(HtmlTextWriterTag.Span);

				writer.WriteEncodedText(string.Format("请求的操作已经执行，但有{0}个错误或消息。", this.errs.Count));

				writer.RenderEndTag();
			}

			if (this.errs != null && this.errs.Count > 0)
			{
				writer.AddAttribute(HtmlTextWriterAttribute.Class, "pc-detaillink");
				writer.AddAttribute("data-actrole", "opendetails");
				writer.AddAttribute(HtmlTextWriterAttribute.Href, "javascript:void(0)");
				writer.RenderBeginTag(HtmlTextWriterTag.A);
				writer.WriteEncodedText(this.detailButtonText);
				writer.RenderEndTag();
			}

			writer.AddAttribute(HtmlTextWriterAttribute.Title, "关闭");

			// writer.AddAttribute(HtmlTextWriterAttribute.Href, "javascript:void(0)");
			if (string.IsNullOrEmpty(this.clientClose) == false)
			{
				writer.AddAttribute(HtmlTextWriterAttribute.Onclick, this.clientClose);
			}

			writer.AddAttribute(HtmlTextWriterAttribute.Class, "pc-button");
			writer.AddAttribute(HtmlTextWriterAttribute.Type, "button");
			writer.AddAttribute("data-actrole", "close");
			writer.AddAttribute(HtmlTextWriterAttribute.Value, this.closeButtonText ?? string.Empty);
			writer.RenderBeginTag(HtmlTextWriterTag.Input);

			// if (closeButtonText != null)
			//    writer.WriteEncodedText(closeButtonText);
			writer.RenderEndTag();

			if (this.reportVisible)
			{
				writer.AddAttribute(HtmlTextWriterAttribute.Title, "发送错误报告");

				// writer.AddAttribute(HtmlTextWriterAttribute.Href, "javascript:void(0)");
				writer.AddAttribute(HtmlTextWriterAttribute.Class, "pc-button");
				writer.AddAttribute(HtmlTextWriterAttribute.Type, "button");
				writer.AddAttribute("data-actrole", "report");
				writer.AddAttribute(HtmlTextWriterAttribute.Value, this.reportButtonText ?? string.Empty);
				writer.RenderBeginTag(HtmlTextWriterTag.Input);
				writer.RenderEndTag();
			}

			writer.RenderEndTag();
		}

		#endregion
	}

	/// <summary>
	/// 为<see cref="ErrorItemRendering"/>事件提供参数。
	/// </summary>
	public class NoticeRenderEventArgs : EventArgs
	{
		private HtmlTextWriter writer;

		internal NoticeRenderEventArgs(HtmlTextWriter writer)
		{
			this.writer = writer;
		}

		/// <summary>
		/// 获取或设置一个值，表示是否应该阻止默认的呈现方式。
		/// </summary>
		public bool PreventDefault { get; set; }

		/// <summary>
		/// 获取或设置正在呈现的项目
		/// </summary>
		public object Item { get; set; }

		/// <summary>
		/// 获取正在用于进行呈现的<see cref="HtmlTextWriter"/>
		/// </summary>
		public HtmlTextWriter Writer
		{
			get { return this.writer; }
		}
	}
}
