using System;
using System.ComponentModel;
using System.Web.UI;
using System.Runtime;

namespace MCS.Web.WebControls
{
	/// <summary>
	/// 表示TabPage
	/// </summary>
	[ParseChildren(false), ToolboxData("<{0}:RelaxedTabPage runat=\"server\" Title=\"新标签\"></{0}:RelaxedTabPage>")]
	[Designer("System.Web.UI.Design.WebControls.MultiViewDesigner, System.Design, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a")]
	[Themeable(true)]
	public class RelaxedTabPage : Control
	{
		#region 字段

		private bool _active = false;
		private string _title = null;
		private string _iconCss = null;
		private string _tabCssClass = null;
		private string _contentCssClass = null;
		private string _tag = null;

		#endregion

		#region 构造函数
		/// <summary>
		/// 初始化<see cref="RelaxedTabPage"/>的新实例。
		/// </summary>
		public RelaxedTabPage()
		{
			this._title = "新标签";
			this._iconCss = "pc-tabs-icon";
			this._tabCssClass = "pc-tabs-tab";
			this._contentCssClass = "pc-tabs-content";
		}

		#endregion

		#region 属性
		/// <summary>
		/// 获取或设置标题
		/// </summary>
		public string Title
		{
			get
			{
				return this._title;
			}

			set
			{
				if (string.IsNullOrEmpty(value))
					throw new ArgumentNullException("value");
				this._title = value;
			}
		}

		/// <summary>
		/// 获取或设置在Tab上应用的Css样式
		/// </summary>
		[CssClassProperty]
		[DefaultValue("pc-tabs-tab")]
		public string TabCssClass
		{
			get { return this._tabCssClass; }

			set { this._tabCssClass = value ?? string.Empty; }
		}

		/// <summary>
		/// 获取或设置在Icon上应用的Css样式
		/// </summary>
		[CssClassProperty]
		[DefaultValue("pc-tabs-icon")]
		public string IconCssClass
		{
			get { return this._iconCss; }

			set { this._iconCss = value ?? string.Empty; }
		}

		/// <summary>
		/// 获取或设置在内容容器上应用的Css样式
		/// </summary>
		[CssClassProperty]
		[DefaultValue("pc-tabs-content")]
		public string ContentCssClass
		{
			get { return this._contentCssClass; }

			set { this._contentCssClass = value ?? string.Empty; }
		}

		/// <summary>
		/// 获取或设置在TabPage上定义的自定义数据
		/// </summary>
		public string TagKey
		{
			get { return this._tag; }

			set { this._tag = value; }
		}

		/// <summary>
		/// 此页的可见性，总是可见，不可设置为不可见
		/// </summary>
		[Browsable(false)]
		public override bool Visible
		{
			get
			{
				return true;
			}

			set
			{
				throw new InvalidOperationException("无法设置可见性，控件永远会被Render");
			}
		}

		internal bool Active
		{
			get
			{
				return this._active;
			}

			set
			{
				this._active = value;
			}
		}

		#endregion

		/// <summary>
		/// 将服务器控件内容发送到提供的 <see cref="System.Web.UI.HtmlTextWriter"/> 对象，此对象编写将在客户端呈现的内容。
		/// </summary>
		/// <param name="writer">接收服务器控件内容的 <see cref="System.Web.UI.HtmlTextWriter"/> 对象。</param>
		[TargetedPatchingOptOut("Performance critical to inline this type of method across NGen image boundaries")]
		protected override void Render(HtmlTextWriter writer)
		{
			writer.AddAttribute(HtmlTextWriterAttribute.Id, this.ClientID);
			string cssClass = this.ContentCssClass;
			if (this.Active)
			{
				RelaxedTabStrip strip = this.Parent as RelaxedTabStrip;
				if (strip != null)
				{
					cssClass += " " + strip.ActiveTabCssClass;
				}
			}

			cssClass = cssClass.Trim();
			if (string.IsNullOrWhiteSpace(cssClass) == false)
			{
				writer.AddAttribute(HtmlTextWriterAttribute.Class, cssClass);
			}

			if (this.TagKey != null)
			{
				writer.AddAttribute("data-tag", this.TagKey);
			}

			writer.RenderBeginTag(HtmlTextWriterTag.Div);
			base.Render(writer);
			writer.RenderEndTag();
		}
	}
}