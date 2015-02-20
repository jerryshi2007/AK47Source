using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Drawing.Design;
using System.Reflection;
using System.Text;
using System.Web;
using System.Web.UI;
using System.Web.UI.HtmlControls;
using MCS.Library.Core;
using MCS.Library.Globalization;
using MCS.Library.Passport;
using MCS.Web.Library;
using MCS.Web.Library.Script;

[assembly: WebResource("MCS.Web.WebControls.Script.Resources.dialogLogo.gif", "image/gif")]

namespace MCS.Web.WebControls
{
	/// <summary>
	/// 对话框类的控件基类。
	/// 该控件定义了基本对话框的参数处理，加载了基本的模板
	/// </summary>
	public abstract class DialogControlBase<T> : ScriptControlBase, INamingContainer where T : DialogControlParamsBase, new()
	{
		private delegate void InitControlDelegate<TControl>(TControl control);

		private T controlParams = null;
		private string confirmButtonClientID = string.Empty;
		private string cancelButtonClientID = string.Empty;
		private string middleButtonClientID = string.Empty;

		private Dictionary<string, string> clientVariables = new Dictionary<string, string>();

		public event EventHandler BeforeNormalPreRender;
		public event LoadingDialogContentEventHander LoadingDialogContent;

		/// <summary>
		/// 构造方法
		/// </summary>
		protected DialogControlBase()
			: base(true, HtmlTextWriterTag.Div)
		{
		}

		#region Properties
		[Description("特殊标识")]
		[DefaultValue("")]
		public string Tag
		{
			get
			{
				return ControlParams.Tag;
			}
			set
			{
				ControlParams.Tag = value;
			}
		}

		[DefaultValue("")]
		public string Category
		{
			get
			{
				return ControlParams.Category;
			}
			set
			{
				ControlParams.Category = value;
			}
		}

		/// <summary>
		/// 对话框窗口的标题
		/// </summary>
		[DefaultValue("")]
		public string DialogTitle
		{
			get
			{
				return ControlParams.DialogTitle;
			}
			set
			{
				ControlParams.DialogTitle = value;
			}
		}

		/// <summary>
		/// 对话框顶端的标题（不是窗口标题）
		/// </summary>
		[DefaultValue("")]
		public string DialogHeaderText
		{
			get
			{
				return ControlParams.DialogHeaderText;
			}
			set
			{
				ControlParams.DialogHeaderText = value;
			}
		}

		/// <summary>
		/// 当前的显示模式（弹出对话框还是普通页面）
		/// </summary>
		[Browsable(false)]
		[ScriptControlProperty(), ClientPropertyName("currentMode")]
		public ControlShowingMode CurrentMode
		{
			get
			{
				ControlShowingMode result = ControlShowingMode.Normal;

				if (this.RenderMode.OnlyRenderSelf)
					result = ControlShowingMode.Dialog;

				return result;
			}
		}

		/// <summary>
		/// 控件是当作对话框使用，还是显示在普通页面上
		/// </summary>
		[DefaultValue(ControlShowingMode.Dialog)]
		[ScriptControlProperty(), ClientPropertyName("showingMode")]
		public ControlShowingMode ShowingMode
		{
			get
			{
				return GetPropertyValue("ShowingMode", ControlShowingMode.Dialog);
			}
			set
			{
				SetPropertyValue("ShowingMode", value);
			}
		}

		/// <summary>
		/// 客户端弹出对话框的url
		/// </summary>
		[ScriptControlProperty(), ClientPropertyName("dialogUrl")]
		[Browsable(false)]
		public string DialogUrl
		{
			get
			{
				return GetDialogUrl();
			}
		}

		/// <summary>
		/// 客户端弹出对话框的窗口特性（尺寸等）
		/// </summary>
		[DefaultValue("")]
		[ScriptControlProperty(), ClientPropertyName("dialogFeature")]
		public string DialogFeature
		{
			get
			{
				string defaultFeature = string.Empty;

				if (this.DesignMode == false)
					defaultFeature = GetDialogFeature();

				return GetPropertyValue("DialogFeature", defaultFeature);
			}
			set
			{
				SetPropertyValue("DialogFeature", value);
			}
		}

		/// <summary>
		/// 对话框内容中，标题左侧的图标
		/// </summary>
		[Bindable(true), Category("Appearance"), Description("对话框的缺省Logo")]
		[Editor("System.Web.UI.Design.ImageUrlEditor, System.Design", typeof(UITypeEditor))]
		public string LogoImageUrl
		{
			get
			{
				return ControlParams.LogoImageUrl;
			}
			set
			{
				ControlParams.LogoImageUrl = value;
			}
		}

		[ScriptControlProperty()]
		[ClientPropertyName("confirmButtonClientID")]
		protected string ConfirmButtonClientID
		{
			get
			{
				return this.confirmButtonClientID;
			}
		}

		[ScriptControlProperty()]
		[ClientPropertyName("cancelButtonClientID")]
		protected string CancelButtonClientID
		{
			get
			{
				return this.cancelButtonClientID;
			}
		}

		[ScriptControlProperty]
		[ClientPropertyName("middleButtonClientID")]
		protected string MiddleButtonClientID
		{
			get
			{
				return this.middleButtonClientID;
			}
		}
		#endregion

		#region protected
		protected override void OnInit(EventArgs e)
		{
			if (this.ControlParams.LoadedFromQueryString == false && CurrentMode == ControlShowingMode.Dialog)
				this.ControlParams.LoadDataFromQueryString();

			base.OnInit(e);
		}

		/// <summary>
		/// 需要传递到对话框中的参数
		/// </summary>
		protected T ControlParams
		{
			get
			{
				if (this.controlParams == null)
				{
					if (this.DesignMode == false && Page != null)
						if (Page.IsPostBack && !Page.IsCallback)
							this.controlParams = GetPropertyValue("ControlParams", (T)null);

					if (this.controlParams == null)
						this.controlParams = new T();
				}

				return this.controlParams;
			}
		}

		protected virtual void OnLoadingDialogContent(object sender, LoadingDialogContentEventArgs eventArgs)
		{
			if (LoadingDialogContent != null)
				LoadingDialogContent(sender, eventArgs);
		}

		/// <summary>
		/// 在OnLoad执行之前
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		protected override void OnPagePreLoad(object sender, EventArgs e)
		{
			EnsureChildControls();
			base.OnPagePreLoad(sender, e);
		}

		/// <summary>
		/// 页面渲染之前
		/// </summary>
		/// <param name="e"></param>
		protected override void OnPreRender(EventArgs e)
		{
			if (this.CurrentMode == ControlShowingMode.Normal)
			{
				if (this.BeforeNormalPreRender != null)
					this.BeforeNormalPreRender(this, e);
			}

			base.OnPreRender(e);

			ViewState["ControlParams"] = ControlParams;
			RenderClientVariables();

			if (this.CurrentMode == ControlShowingMode.Dialog)
			{
				DialogContentAttribute contentAttr = AttributeHelper.GetCustomAttribute<DialogContentAttribute>(this.GetType());

				if (contentAttr != null)
					this.EnableViewState = contentAttr.EnableViewState;
			}
		}

		/// <summary>
		/// 创建子控件
		/// </summary>
		protected override void CreateChildControls()
		{
			base.CreateChildControls();

			if (this.CurrentMode == ControlShowingMode.Dialog)
			{
				Page.Response.Cache.SetCacheability(HttpCacheability.NoCache);

				Control template = LoadDialogTemplate();
				Controls.Add(template);

				DealChildControl(template, typeof(HtmlHead), new InitControlDelegate<HtmlHead>(InitDialogTitle));
				DealChildControl(template, "dialogHeaderText", new InitControlDelegate<HtmlGenericControl>(InitDialogHeader));
				DealChildControl(template, "logoImage", new InitControlDelegate<HtmlImage>(InitDialogLogo));
				DealChildControl(template, "dialogContent", new InitControlDelegate<Control>(InitDialogContent));

				Control confirmButton = DealChildControl(template, "confirmButton", new InitControlDelegate<HtmlInputButton>(InitConfirmButton));

				if (confirmButton != null)
					this.confirmButtonClientID = confirmButton.ClientID;

				Control middleButton = DealChildControl(template, "middleButton", new InitControlDelegate<HtmlInputButton>(InitMiddleButton));

				if (middleButton != null)
					this.middleButtonClientID = middleButton.ClientID;

				Control cancelButton = DealChildControl(template, "cancelButton", new InitControlDelegate<HtmlInputButton>(InitCancelButton));

				if (cancelButton != null)
					this.cancelButtonClientID = cancelButton.ClientID;
			}
		}

		/// <summary>
		/// 获得客户端显示对话框的url
		/// </summary>
		/// <returns></returns>
		protected virtual string GetDialogUrl()
		{
			PageRenderMode originalPageRenderMode = WebUtility.GetRequestPageRenderMode();

			string controlID = null;

			if (originalPageRenderMode != null)
				controlID = ScriptControlHelper.GetPageUniqueID(originalPageRenderMode.PageCache);

			if (controlID.IsNotEmpty())
				controlID = controlID + "$" + this.UniqueID;
			else
				controlID = this.UniqueID;

			PageRenderMode pageRenderMode = new PageRenderMode(controlID, "DialogControl");

			string url = WebUtility.GetRequestExecutionUrl(pageRenderMode);

			NameValueCollection originalParams = UriHelper.GetUriParamsCollection(url);

			originalParams.Remove(PassportManager.TicketParamName);

			return UriHelper.CombineUrlParams(url,
					originalParams,
					UriHelper.GetUriParamsCollection(this.ControlParams.ToRequestParams()));
		}

		/// <summary>
		/// 获得客户端弹出对话框窗口的描述
		/// </summary>
		/// <returns></returns>
		protected virtual string GetDialogFeature()
		{
			return "dialogWidth:360px;dialogHeight:400px;center:yes;help:no;resizable:yes;scroll:no;status:no";
		}

		/// <summary>
		/// 加载对话框的模板
		/// </summary>
		/// <returns></returns>
		protected virtual Control LoadDialogTemplate()
		{
			DialogTemplateAttribute templateAttr = AttributeHelper.GetCustomAttribute<DialogTemplateAttribute>(this.GetType());

			string pageHtml = string.Empty;

			if (templateAttr != null && string.IsNullOrEmpty(templateAttr.ResourcePath) == false)
				pageHtml = ResourceHelper.LoadStringFromResource(Assembly.GetExecutingAssembly(), templateAttr.ResourcePath);
			else
			{
				Assembly assembly = Assembly.GetExecutingAssembly();

				if (templateAttr != null && string.IsNullOrEmpty(templateAttr.AssemblyName) == false)
					assembly = Assembly.Load(templateAttr.AssemblyName);

				pageHtml = ResourceHelper.LoadStringFromResource(assembly,
					"MCS.Web.WebControls.Script.Resources.standardDialogControlTemplate.htm");
			}

			DialogContentAttribute contentAttr = AttributeHelper.GetCustomAttribute<DialogContentAttribute>(this.GetType());

			if (contentAttr != null && string.IsNullOrEmpty(contentAttr.ResourcePath) == false)
			{
				Assembly assembly = Assembly.GetExecutingAssembly();

				if (string.IsNullOrEmpty(contentAttr.AssemblyName) == false)
					assembly = Assembly.Load(contentAttr.AssemblyName);

				LoadingDialogContentEventArgs eventArgs = new LoadingDialogContentEventArgs();

				OnLoadingDialogContent(this, eventArgs);

				string html = eventArgs.Content;

				if (html.IsNullOrEmpty())
					html = ResourceHelper.LoadStringFromResource(assembly, contentAttr.ResourcePath);

				pageHtml = pageHtml.Replace("<!--dialogContent-->", html);
			}

			pageHtml = ReplaceDialogTemplateString(pageHtml);

			return Page.ParseControl(pageHtml);
		}

		/// <summary>
		/// 重载此方法，可以替换已经加载的模板的内容
		/// </summary>
		/// <param name="html"></param>
		/// <returns></returns>
		protected virtual string ReplaceDialogTemplateString(string html)
		{
			return html;
		}

		/// <summary>
		/// 加载模板后，初始化对话框中的控件
		/// </summary>
		/// <param name="container">对话框中内容部分的容器控件</param>
		protected virtual void InitDialogContent(Control container)
		{
		}

		/// <summary>
		/// 初始化对话框窗口标题
		/// </summary>
		/// <param name="header"></param>
		protected virtual void InitDialogTitle(HtmlHead header)
		{
			if (this.Page.IsCallback == false)
			{
				HtmlTitle title = new HtmlTitle();

				title.Text = Translate(this.DialogTitle);
				header.Controls.Add(title);
			}
		}

		/// <summary>
		/// 初始化对话框页面顶端的标题
		/// </summary>
		/// <param name="titleContainer"></param>
		protected virtual void InitDialogHeader(HtmlGenericControl titleContainer)
		{
			if (string.IsNullOrEmpty(DialogHeaderText))
				titleContainer.InnerText = Translate(DialogTitle);
			else
				titleContainer.InnerText = Translate(DialogHeaderText);

			HtmlGenericControl baseTag = new HtmlGenericControl("base");
			baseTag.Attributes["target"] = "_self";

			this.Page.Header.Controls.Add(baseTag);
		}

		/// <summary>
		/// 初始化对话框页面顶端的标题左侧图标
		/// </summary>
		/// <param name="logo"></param>
		protected virtual void InitDialogLogo(HtmlImage logo)
		{
			if (string.IsNullOrEmpty(this.LogoImageUrl))
				logo.Src = Page.ClientScript.GetWebResourceUrl(typeof(DialogControlBase<>), "MCS.Web.WebControls.Script.Resources.dialogLogo.gif");
			else
				logo.Src = this.LogoImageUrl;
		}

		/// <summary>
		/// 初始化“确认”按钮。可以通过重载此方法，定制按下按钮的客户端事件响应
		/// </summary>
		/// <param name="confirmButton"></param>
		protected virtual void InitConfirmButton(HtmlInputButton confirmButton)
		{
		}

		/// <summary>
		/// 初始化中间按钮。可以通过重载此方法，定制按下按钮的客户端事件响应。默认此按钮时不显示的
		/// </summary>
		/// <param name="middleButton"></param>
		protected virtual void InitMiddleButton(HtmlInputButton middleButton)
		{
		}

		/// <summary>
		/// 初始化“取消”按钮。可以通过重载此方法，定制按下按钮的客户端事件响应
		/// </summary>
		/// <param name="cancelButton"></param>
		protected virtual void InitCancelButton(HtmlInputButton cancelButton)
		{
		}

		/// <summary>
		/// 注册客户端的变量
		/// </summary>
		/// <param name="variableName"></param>
		/// <param name="elementClientID"></param>
		protected void RegisterClientVariable(string variableName, string elementClientID)
		{
			this.clientVariables[variableName] = elementClientID;
		}
		#endregion

		#region private
		private TControl DealChildControl<TControl>(Control template, string controlID, InitControlDelegate<TControl> initDelegate) where TControl : Control
		{
			TControl ctrl = (TControl)WebControlUtility.FindControlByHtmlIDProperty(template, controlID, true);

			if (ctrl != null && initDelegate != null)
				initDelegate(ctrl);

			return ctrl;
		}

		private TControl DealChildControl<TControl>(Control template, Type childControlType, InitControlDelegate<TControl> initDelegate) where TControl : Control
		{
			TControl ctrl = (TControl)WebControlUtility.FindControl(this.Page, typeof(HtmlHead), true);

			if (ctrl != null && initDelegate != null)
				initDelegate(ctrl);

			return ctrl;
		}

		private void RenderClientVariables()
		{
			if (this.clientVariables.Count > 0)
			{
				StringBuilder strB = new StringBuilder(512);

				strB.Append("<script language=\"javascript\" type=\"text/javascript\">");
				foreach (KeyValuePair<string, string> kv in this.clientVariables)
					strB.AppendFormat("\nvar {0} = document.getElementById(\"{1}\");", kv.Key, kv.Value.Replace("\"", "\\\""));

				strB.Append("\n</script>");

				Page.ClientScript.RegisterStartupScript(this.GetType(), this.ID + "_ClientVariables", strB.ToString());
			}
		}

		private string Translate(string sourceText)
		{
			string category = Define.DefaultCulture;

			if (string.IsNullOrEmpty(this.Category) == false)
				category = this.Category;

			return Translator.Translate(category, sourceText);
		}
		#endregion
	}
}
