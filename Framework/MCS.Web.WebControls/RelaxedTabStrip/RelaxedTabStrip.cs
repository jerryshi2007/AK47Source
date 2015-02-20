using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using MCS.Web.Library.Script;

[assembly: WebResource("MCS.Web.WebControls.RelaxedTabStrip.RelaxedTabStrip.js", "text/javascript")]
namespace MCS.Web.WebControls
{
	/// <summary>
	/// 提供选项卡式视图，不是命名容器
	/// </summary>
	[DefaultProperty("Text")]
	//[RequiredScript(typeof(ControlBaseScript), 1)]
	[ToolboxData("<{0}:RelaxedTabStrip runat=server></{0}:RelaxedTabStrip>")]
	[ParseChildren(typeof(RelaxedTabPage))]
	[ControlBuilder(typeof(RelaxedTabStripControlBuilder))]
	[Designer("System.Web.UI.Design.WebControls.MultiViewDesigner, System.Design, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a")]
	[Themeable(true)]
	public class RelaxedTabStrip : Control, IScriptControl
	{
		#region 私有字段

		private static readonly object eventActiveTabPageChanged = new object();

		private ScriptManager sm = null;
		private int _activeTabPageIndex = 0;
		private int _cachedActiveTabPageIndex = 0;
		private bool _controlStateApplied = false;
		private bool _ignoreBubbleEvents = false;
		private bool _autoPostBack = false;
		private string _cssClass = "pc-tabs";
		private string _tabsHeaderCss = "pc-tabs-header";
		private string _hoverCssClass = "pc-hover";
		private string _activeCssClass = "pc-active";
		#endregion

		#region 构造函数

		/// <summary>
		/// 初始化<see cref="RelaxedTabStrip"/>的新实例
		/// </summary>
		public RelaxedTabStrip()
		{
			this._activeTabPageIndex = -1;
			this._cachedActiveTabPageIndex = -1;
		}

		#endregion

		#region 事件

		/// <summary>
		/// 当ActiveTabPage改变时发生
		/// </summary>
		[Category("Action"), Description("当ActiveTabPage改变时发生")]
		public event EventHandler ActiveTabPageChanged
		{
			add
			{
				this.Events.AddHandler(eventActiveTabPageChanged, value);
			}

			remove
			{
				this.Events.RemoveHandler(eventActiveTabPageChanged, value);
			}
		}

		#endregion

		#region 属性

		/// <summary>
		/// 获取或设置一个值，表示当客户端激活标签页时是否引发回调
		/// </summary>
		[DefaultValue(false), Description("当客户端激活标签页时是否引发回调"), Category("Behavior")]
		public bool PostBackWhenClientActive
		{
			get { return this._autoPostBack; }

			set { this._autoPostBack = value; }
		}

		/// <summary>
		/// 表示活动RelaxedTabPage的索引
		/// </summary>
		[DefaultValue(-1), Description("表示活动RelaxedTabStrip的索引"), Category("Behavior")]
		public virtual int ActiveTabPageIndex
		{
			get
			{
				if (this._cachedActiveTabPageIndex > -1)
				{
					return this._cachedActiveTabPageIndex;
				}

				return this._activeTabPageIndex;
			}

			set
			{
				if (value < -1)
				{
					throw new ArgumentOutOfRangeException("value", string.Format("ActiveTabPageIndex{0}不能小于-1.", new object[] { value }));
				}

				if (this.TabPages.Count == 0)
				{
					this._cachedActiveTabPageIndex = value;
				}
				else
				{
					if (value >= this.TabPages.Count)
					{
						throw new ArgumentOutOfRangeException("value", string.Format("ActiveTabPageIndex不能等于或大于{0}。", this.TabPages.Count));
					}

					int num = (this._cachedActiveTabPageIndex != -1) ? -1 : this._activeTabPageIndex;
					this._activeTabPageIndex = value;
					this._cachedActiveTabPageIndex = -1;
					if (((num != value) && (num != -1)) && (num < this.TabPages.Count))
					{
						this.TabPages[num].Active = false;
						if (this.ShouldTriggerTabPageEvent)
						{
							// this.TabPages[num].OnDeactivate(EventArgs.Empty);
						}
					}

					if ((num != value && this.TabPages.Count != 0) && value != -1)
					{
						this.TabPages[value].Active = true;
						if (this.ShouldTriggerTabPageEvent)
						{
							// this.TabPages[value].OnActivate(EventArgs.Empty);
							this.OnActiveTabPageChanged(EventArgs.Empty);
						}
					}
				}
			}
		}

		/// <summary>
		/// 获取标签页的集合
		/// </summary>
		[PersistenceMode(PersistenceMode.InnerDefaultProperty), Browsable(false), Description("标签页的集合")]
		public virtual RelaxedTabPageCollection TabPages
		{
			get
			{
				return (RelaxedTabPageCollection)this.Controls;
			}
		}

		/// <summary>
		/// 获取或设置控件的CssClass样式规则，该规则应用于div元素
		/// </summary>
		[Description("控件的CSS样式规则"), Category("Appearance"), CssClassProperty, DefaultValue("pc-tabs")]
		public string CssClass
		{
			get
			{
				return this._cssClass;
			}

			set
			{
				this._cssClass = value ?? string.Empty;
			}
		}

		/// <summary>
		/// 获取或设置标签栏的CssClass样式规则，该规则应用于ul元素
		/// </summary>
		[Description("Tab栏的CSS样式规则"), Category("Appearance"), CssClassProperty, DefaultValue("pc-tabs-header")]
		public string TabsHeaderCssClass
		{
			get { return this._tabsHeaderCss; }
			set { this._tabsHeaderCss = value; }
		}

		/// <summary>
		/// 获取或设置鼠标悬停在Tab上的CssClass样式规则，该规则应用于li元素
		/// </summary>
		[Description("悬停的Tab的CSS样式规则"), Category("Appearance"), CssClassProperty, DefaultValue("pc-hover")]
		public string HoverTabCssCass
		{
			get { return this._hoverCssClass; }
			set { this._hoverCssClass = value; }
		}

		/// <summary>
		/// 获取或设置激活的Tab的CssClass样式规则，该规则应用于li元素
		/// </summary>
		[Description("激活的TabPage的Tab的CSS样式规则"), Category("Appearance"), CssClassProperty, DefaultValue("pc-active")]
		public string ActiveTabCssClass
		{
			get { return this._activeCssClass; }
			set { this._activeCssClass = value; }
		}

		#endregion

		#region 私有属性

		private bool ShouldTriggerTabPageEvent
		{
			get
			{
				return this._controlStateApplied || (this.Page != null && !this.Page.IsPostBack);
			}
		}

		#endregion

		#region 公开的方法

		/// <summary>
		/// 获取当前活动的<see cref="RelaxedTabPage"/>
		/// </summary>
		/// <returns></returns>
		public RelaxedTabPage GetActivePage()
		{
			int activeTabPageIndex = this.ActiveTabPageIndex;
			if (activeTabPageIndex >= this.TabPages.Count)
			{
				throw new Exception("活动页索引超出范围");
			}

			if (activeTabPageIndex < 0)
			{
				return null;
			}

			RelaxedTabPage tabPage = this.TabPages[activeTabPageIndex];

			return tabPage;
		}

		/// <summary>
		/// 设置当前活动的<see cref="RelaxedTabPage"/>对象。
		/// </summary>
		/// <param name="tabPage">要设置活动的<see cref="RelaxedTabPage"/>对象</param>
		public void SetActiveTabPage(RelaxedTabPage tabPage)
		{
			int index = this.TabPages.IndexOf(tabPage);
			if (index < 0)
			{
				throw new HttpException(string.Format("在RelaxedTabStrip:{1}中没有找到RelaxedTabPage:{0}", new object[] { (tabPage == null) ? "null" : tabPage.ID, this.ID }));
			}

			this.ActiveTabPageIndex = index;
		}
		#endregion

		#region 显式接口实现

		IEnumerable<ScriptReference> IScriptControl.GetScriptReferences()
		{
			return this.GetScriptReferences();
		}

		IEnumerable<ScriptDescriptor> IScriptControl.GetScriptDescriptors()
		{
			return this.GetScriptDescriptors();
		}

		#endregion

		#region 引发事件

		/// <summary>
		/// 引发<see cref="ActiveTabPageChanged"/>事件
		/// </summary>
		/// <param name="e">表示事件参数的<see cref="EventArgs"/></param>
		protected virtual void OnActiveTabPageChanged(EventArgs e)
		{
			EventHandler handler = (EventHandler)this.Events[eventActiveTabPageChanged];
			if (handler != null)
			{
				handler(this, e);
			}
		}

		/// <summary>
		/// 确定服务器控件的事件是否沿页的 UI 服务器控件层次结构向上传递。
		/// </summary>
		/// <param name="source">事件源</param>
		/// <param name="e">包含事件数据的 <see cref="System.EventArgs"/> 对象。</param>
		/// <returns>如果事件已被取消，则为 true；否则为 false。默认值为 false。</returns>
		protected override bool OnBubbleEvent(object source, EventArgs e)
		{
			if (!this._ignoreBubbleEvents && (e is CommandEventArgs))
			{
				CommandEventArgs args = (CommandEventArgs)e;
				string commandName = args.CommandName;
				// TODO: 这里未实现
				base.OnBubbleEvent(source, e);
			}

			return false;
		}

		/// <summary>
		/// 引发 <see cref="System.Web.UI.Control.Init"/> 事件。
		/// </summary>
		/// <param name="e">一个包含事件数据的 <see cref="System.EventArgs"/> 对象。</param>
		protected override void OnInit(EventArgs e)
		{
			base.OnInit(e);
			this.Page.RegisterRequiresControlState(this); // 使用控件状态保存选定的页
			if (this._cachedActiveTabPageIndex > -1)
			{
				this.ActiveTabPageIndex = this._cachedActiveTabPageIndex;
				this._cachedActiveTabPageIndex = -1;
				this.GetActivePage();
			}
		}

		/// <summary>
		/// 引发<see cref="System.Web.UI.Control.PreRender"/>事件。
		/// </summary>
		/// <param name="e">一个包含事件数据的 <see cref="System.EventArgs"/> 对象。</param>
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
		}

		#endregion

		#region 受保护的方法

		/// <summary>
		/// 在子控件从 System.Web.UI.Control 对象的 System.Web.UI.Control.Controls 集合中移除后调用。
		/// </summary>
		/// <param name="ctl">已移除的 System.Web.UI.Control。</param>
		/// <exception cref="System.InvalidOperationException"> 该控件为 System.Web.UI.WebControls.Substitution 控件。</exception>
		protected override void RemovedControl(Control ctl)
		{
			if (((RelaxedTabPage)ctl).Active && (this.ActiveTabPageIndex < this.TabPages.Count))
			{
				this.GetActivePage();
			}

			base.RemovedControl(ctl);
		}

		/// <summary>
		/// 将服务器控件内容发送到提供的 <see cref="System.Web.UI.HtmlTextWriter"/> 对象，此对象编写将在客户端呈现的内容。
		/// </summary>
		/// <param name="writer">接收服务器控件内容的 <see cref="System.Web.UI.HtmlTextWriter"/> 对象。</param>
		protected override void Render(HtmlTextWriter writer)
		{
			if (!this.DesignMode)
				this.sm.RegisterScriptDescriptors(this);
			this.EnsureChildControls();
			writer.AddAttribute(HtmlTextWriterAttribute.Class, this._cssClass ?? string.Empty);
			writer.AddAttribute(HtmlTextWriterAttribute.Id, this.ClientID);
			writer.RenderBeginTag(HtmlTextWriterTag.Div); // <div>

			// 设置隐藏域，用于保存客户端激活的标签索引
			writer.AddAttribute(HtmlTextWriterAttribute.Type, "hidden");
			writer.AddAttribute(HtmlTextWriterAttribute.Name, "__rts_cs_" + this.ClientID);
			writer.AddAttribute(HtmlTextWriterAttribute.Value, this.ActiveTabPageIndex.ToString());
			writer.RenderBeginTag(HtmlTextWriterTag.Input); // <input />
			writer.RenderEndTag();

			writer.AddAttribute(HtmlTextWriterAttribute.Type, "hidden");
			writer.AddAttribute(HtmlTextWriterAttribute.Name, "__rts_cstag_" + this.ClientID);
			if (this.ActiveTabPageIndex >= 0 && this.ActiveTabPageIndex < this.TabPages.Count && this.GetActivePage().TagKey != null)
			{
				writer.AddAttribute(HtmlTextWriterAttribute.Value, this.GetActivePage().TagKey);
			}

			writer.RenderBeginTag(HtmlTextWriterTag.Input); // </input>
			writer.RenderEndTag(); // </input>

			base.Render(writer);
			writer.RenderEndTag(); // </div>
		}

		/// <summary>
		/// 获取用于定义控件需要的脚本资源的 <see cref="System.Web.UI.ScriptReference"/> 对象的集合。
		/// </summary>
		/// <returns><see cref="System.Web.UI.ScriptReference"/> 对象的 <see cref="System.Collections.IEnumerable"/> 集合。</returns>
		protected virtual IEnumerable<ScriptReference> GetScriptReferences()
		{
			ScriptReference reference0 = new ScriptReference() { Assembly = typeof(ScriptControlBase).Assembly.FullName, Name = "MCS.Web.Library.Script.Resources.ControlBase.js" };

			ScriptReference reference1 = new ScriptReference();
			reference1.Path = this.Page.ClientScript.GetWebResourceUrl(typeof(RelaxedTabStrip), "MCS.Web.WebControls.RelaxedTabStrip.RelaxedTabStrip.js");

			return new ScriptReference[] { reference0, reference1 };
		}

		/// <summary>
		/// 获取表示 ECMAScript (JavaScript) 客户端组件的脚本说明符的集合。
		/// </summary>
		/// <returns><see cref="System.Web.UI.ScriptDescriptor"/> 对象的 <see cref="System.Collections.IEnumerable"/> 集合。</returns>
		protected virtual IEnumerable<ScriptDescriptor> GetScriptDescriptors()
		{
			ScriptControlDescriptor descriptor = new ScriptControlDescriptor("MCS.Web.WebControls.RelaxedTabStrip", this.ClientID);
			descriptor.AddProperty("activeTabCssClass", this.ActiveTabCssClass);
			descriptor.AddProperty("hoverTabCssCass", this.HoverTabCssCass);
			descriptor.AddProperty("activeTabPageIndex", this.ActiveTabPageIndex);
			return new ScriptDescriptor[] { descriptor };
		}

		/// <summary>
		/// 将服务器控件子级的内容输出到提供的 <see cref="System.Web.UI.HtmlTextWriter"/> 对象，此对象编写将在客户端呈现的内容。
		/// </summary>
		/// <param name="writer">接收呈现内容的 <see cref="System.Web.UI.HtmlTextWriter"/> 对象。</param>
		protected override void RenderChildren(HtmlTextWriter writer)
		{
			this.RenderTabsHeader(writer);
			base.RenderChildren(writer);
		}

		/// <summary>
		/// 呈现标签页头部
		/// </summary>
		///<param name="writer">接收呈现内容的 <see cref="System.Web.UI.HtmlTextWriter"/> 对象。</param>
		protected virtual void RenderTabsHeader(HtmlTextWriter writer)
		{
			writer.AddAttribute(HtmlTextWriterAttribute.Class, this._tabsHeaderCss);
			writer.RenderBeginTag(HtmlTextWriterTag.Ul); // <ul>
			foreach (RelaxedTabPage tabPage in this.TabPages)
			{
				string liCss = tabPage.TabCssClass;
				if (tabPage.Active)
				{
					liCss += " " + this._activeCssClass;
				}

				if (!string.IsNullOrWhiteSpace(liCss))
				{
					writer.AddAttribute(HtmlTextWriterAttribute.Class, liCss);
				}

				if (string.IsNullOrEmpty(tabPage.TagKey) == false)
					writer.AddAttribute("data-tag", tabPage.TagKey);

				writer.RenderBeginTag(HtmlTextWriterTag.Li); // <li>
				writer.AddAttribute(HtmlTextWriterAttribute.Href, "javascript:void(0);");
				writer.RenderBeginTag(HtmlTextWriterTag.A); // <a>
				writer.AddAttribute(HtmlTextWriterAttribute.Class, tabPage.IconCssClass);
				writer.RenderBeginTag(HtmlTextWriterTag.Span); // <span>
				writer.RenderEndTag(); // </span>
				writer.WriteEncodedText(tabPage.Title);
				writer.RenderEndTag(); // </a>
				writer.RenderEndTag(); // </li>
			}

			writer.RenderEndTag(); // </ul>
			writer.WriteLine("<!-- 此位置包含的任何div将视作标签页面板，请勿随意触动此处的DOM -->");
		}

		/// <summary>
		/// 保存自页回发到服务器后发生的任何服务器控件状态更改。
		/// </summary>
		/// <returns>返回服务器控件的当前状态。如果没有与控件关联的状态，则此方法返回 null。</returns>
		protected override object SaveControlState()
		{
			int activeTabPageIndex = this.ActiveTabPageIndex;
			object x = base.SaveControlState();
			if ((x == null) && (activeTabPageIndex == -1))
			{
				return null;
			}

			return new Pair(x, activeTabPageIndex);
		}

		/// <summary>
		/// 从 <see cref="System.Web.UI.Control.SaveControlState"/> 方法保存的上一个页请求还原控件状态信息。
		/// </summary>
		/// <param name="state">表示要还原的控件状态的 <see cref="System.Object"/>。</param>
		protected override void LoadControlState(object state)
		{
			Pair pair = state as Pair;
			if (pair != null)
			{
				base.LoadControlState(pair.First);
				this.ActiveTabPageIndex = (int)pair.Second;
			}

			this._controlStateApplied = true;
		}

		/// <summary>
		/// 通知服务器控件某个元素（XML 或 HTML）已经过语法分析，并将该元素添加到服务器控件的 <see cref="System.Web.UI.ControlCollection"/> 对象。
		/// </summary>
		/// <param name="obj">表示已经过语法分析的元素的 <see cref="System.Object"/> 。</param>
		protected override void AddParsedSubObject(object obj)
		{
			if (obj is RelaxedTabPage)
			{
				this.Controls.Add((Control)obj);
			}
			else if (!(obj is LiteralControl))
			{
				{
					throw new HttpException("容器内不能添加" + obj.GetType().Name + "类型的对象");
				}
			}
		}

		/// <summary>
		/// 创建一个新的 <see cref="System.Web.UI.ControlCollection"/> 对象来保存服务器控件的子控件（包括文本控件和服务器控件）。
		/// </summary>
		/// <returns>包含当前服务器控件的子服务器控件的 <see cref="System.Web.UI.ControlCollection"/> 对象。</returns>
		protected override ControlCollection CreateControlCollection()
		{
			return new RelaxedTabPageCollection(this);
		}

		#endregion
	}
}
