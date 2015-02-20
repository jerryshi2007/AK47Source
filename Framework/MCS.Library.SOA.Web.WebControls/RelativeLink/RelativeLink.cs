using System;
using System.ComponentModel;
using System.Drawing.Design;
using System.Web.UI;
using MCS.Library.Core;
using MCS.Library.SOA.DataObjects.Workflow;
using MCS.Web.Library.MVC;
using MCS.Web.Library.Script;

[assembly: WebResource("MCS.Web.WebControls.RelativeLink.RelativeLink.js", "text/javascript", PerformSubstitution = true)]
[assembly: WebResource("MCS.Web.WebControls.RelativeLink.RelativeLink.css", "text/css", PerformSubstitution = true)]
[assembly: WebResource("MCS.Web.WebControls.RelativeLink.Images.blue_down.png", "Image/png")]
[assembly: WebResource("MCS.Web.WebControls.RelativeLink.Images.blue_line.png", "Image/png")]
[assembly: WebResource("MCS.Web.WebControls.RelativeLink.Images.blue_up.png", "Image/png")]
[assembly: WebResource("MCS.Web.WebControls.RelativeLink.Images.down_btn.png", "Image/png")]
[assembly: WebResource("MCS.Web.WebControls.RelativeLink.Images.grap_bg.png", "Image/png")]
[assembly: WebResource("MCS.Web.WebControls.RelativeLink.Images.gray_down.png", "Image/png")]
[assembly: WebResource("MCS.Web.WebControls.RelativeLink.Images.gray_line.png", "Image/png")]
[assembly: WebResource("MCS.Web.WebControls.RelativeLink.Images.gray_up.png", "Image/png")]
[assembly: WebResource("MCS.Web.WebControls.RelativeLink.Images.icon1.png", "Image/png")]
[assembly: WebResource("MCS.Web.WebControls.RelativeLink.Images.line.png", "Image/png")]
[assembly: WebResource("MCS.Web.WebControls.RelativeLink.Images.standard_l.png", "Image/png")]
[assembly: WebResource("MCS.Web.WebControls.RelativeLink.Images.standard_m.png", "Image/png")]
[assembly: WebResource("MCS.Web.WebControls.RelativeLink.Images.standard_r.png", "Image/png")]
[assembly: WebResource("MCS.Web.WebControls.RelativeLink.Images.standard_title.png", "Image/png")]
[assembly: WebResource("MCS.Web.WebControls.RelativeLink.Images.hand1.cur", "Image/cur")]
[assembly: WebResource("MCS.Web.WebControls.RelativeLink.Images.standard_l1.png", "Image/png")]
[assembly: WebResource("MCS.Web.WebControls.RelativeLink.Images.standard_r1.png", "Image/png")]
[assembly: WebResource("MCS.Web.WebControls.RelativeLink.Images.standard_title1.png", "Image/png")]

namespace MCS.Web.WebControls
{
	/// <summary>
	/// 控件位置枚举
	/// </summary>
	public enum RelativeLinkPosition
	{
		Left = 0,
		Right = 1
	}

	/// <summary>
	/// 初始控件状态枚举
	/// </summary>
	public enum RelativeLinkStatus
	{
		Expanded = 0,
		Collapsed = 1,
		ByConfig = 2
	}

	[ClientCssResource("MCS.Web.WebControls.RelativeLink.RelativeLink.css")]
	[ClientScriptResource("MCS.Web.WebControls.RelativeLink", "MCS.Web.WebControls.RelativeLink.RelativeLink.js")]
	[RequiredScript(typeof(ControlBaseScript), 1)]
	[RequiredScript(typeof(HBCommonScript), 2)]
	[Designer(typeof(RelativeDesigner))]
	public class RelativeLink : ScriptControlBase
	{
		private WfRelativeLinkDescriptorCollection _relativeLinks;
		private WfRelativeLinkDescriptorCollection _moreLinks;

		public RelativeLink()
			: base(false, HtmlTextWriterTag.Div)
		{
		}
		/// <summary>
		/// 相关链接的标题,默认为”标准、规范、制度“。
		/// </summary>
		[Category("Default")]
		[ScriptControlProperty]
		[ClientPropertyName("title")]
		[Description("相关链接的标题")]
		public string Title
		{
			get { return GetPropertyValue("Title", "标准、规范、制度"); }
			set { SetPropertyValue("Title", value); }
		}

		/// <summary>
		/// 箭头移动步长默认为10
		/// </summary>
		[Category("Appearance")]
		[ScriptControlProperty]
		[ClientPropertyName("moveStep")]
		[Description("箭头移动步长默认为50")]
		public int MoveStep
		{
			get { return GetPropertyValue("MoveStep", 50); }
			set { SetPropertyValue("MoveStep", value); }
		}

		/// <summary>
		/// 停靠窗口
		/// </summary>
		[Category("Default")]
		[ScriptControlProperty]
		[ClientPropertyName("dockContainer")]
		[Description("停靠窗口")]
		public string DockContainer
		{
			get { return GetPropertyValue("DockContainer", string.Empty); }
			set { SetPropertyValue("DockContainer", value); }
		}

		/// <summary>
		/// 分类
		/// </summary>
		[Category("Default")]
		[Description("分类")]
		public string Category
		{
			get { return GetPropertyValue("Category", string.Empty); }
			set { SetPropertyValue("Category", value); }
		}

		/// <summary>
		/// 更多链接
		/// </summary>
		[Category("Default")]
		[Description("更多链接")]
		public string MoreLinkCategory
		{
			get
			{
				return GetPropertyValue("MoreLinkCategory", string.Empty);
			}
			set
			{
				SetPropertyValue("MoreLinkCategory", value);
			}
		}

		/// <summary>
		/// 链接打开目标
		/// </summary>
		[Category("Default")]
		[ScriptControlProperty]
		[ClientPropertyName("linkTarget")]
		[Description("链接打开目标")]
		public string LinkTarget
		{
			get { return GetPropertyValue("LinkTarget", "_blank"); }
			set { SetPropertyValue("LinkTarget", value); }
		}

		/// <summary>
		/// 控件外层的Div客户端ID
		/// </summary>       
		[ScriptControlProperty]
		[ClientPropertyName("outerContainerClientID")]
		[Description("控件外层的Div客户端ID")]
		private string OuterContainerClientID
		{
			get { return GetPropertyValue("OuterContainerClientID", string.Format("{0}_container", ClientID)); }
			set { SetPropertyValue("OuterContainerClientID", value); }
		}

		/// <summary>
		/// 可拖放点的客户端ID
		/// </summary>    
		[ScriptControlProperty]
		[ClientPropertyName("dragAndDropPointClientID")]
		[Description("可拖放点的客户端ID")]
		private string DragAndDropPointClientID
		{
			get { return GetPropertyValue("DragAndDropPointClientID", string.Format("{0}_line", ClientID)); }
			set { SetPropertyValue("DragAndDropPointClientID", value); }
		}


		/// <summary>
		/// 左/右则标题客户端ID
		/// </summary>    
		[ScriptControlProperty]
		[ClientPropertyName("titleClientID")]
		[Description("左/右则标题客户端ID")]
		private string TitleClientID
		{
			get { return GetPropertyValue("TitleClientID", string.Format("{0}_title", ClientID)); }
			set { SetPropertyValue("TitleClientID", value); }
		}

		/// <summary>
		/// 上移按钮客户端ID
		/// </summary>   
		[ScriptControlProperty]
		[ClientPropertyName("upArrowClientID")]
		[Description("上移按钮客户端ID")]
		private string UpArrowClientID
		{
			get { return GetPropertyValue("UpArrowClientID", string.Format("{0}_upArrow", ClientID)); }
			set { SetPropertyValue("UpArrowClientID", value); }
		}

		/// <summary>
		/// 下移按钮客户端ID
		/// </summary> 
		[ScriptControlProperty]
		[ClientPropertyName("downArrowClientID")]
		[Description("下移按钮客户端ID")]
		private string DownArrowClientID
		{
			get { return GetPropertyValue("DownArrowClientID", string.Format("{0}_downArrow", ClientID)); }
			set { SetPropertyValue("DownArrowClientID", value); }
		}

		/// <summary>
		/// 标题的宽度
		/// </summary>
		[Category("Appearance")]
		[ScriptControlProperty]
		[ClientPropertyName("titleWidth")]
		[Description("标题的宽度")]
		public int TitleWidth
		{
			get { return GetPropertyValue("TitleWidth", 32); }//26
			set { SetPropertyValue("TitleWidth", value); }
		}

		/// <summary>
		/// 标题的高度
		/// </summary>
		[Category("Appearance")]
		[ScriptControlProperty]
		[ClientPropertyName("titleHeight")]
		[Description("标题的高度")]
		public int TitleHeight
		{
			get { return GetPropertyValue("TitleHeight", 168); }//99
			set { SetPropertyValue("TitleHeight", value); }
		}

		/// <summary>
		/// 容器的宽度
		/// </summary>
		[Category("Appearance")]
		[Browsable(false)]
		[ScriptControlProperty]
		[ClientPropertyName("containerWidth")]
		[Description("容器的宽度")]
		public int ContainerWidth
		{
			get { return GetPropertyValue("ContainerWidth", 200); }
			set { SetPropertyValue("ContainerWidth", value); }
		}

		/// <summary>
		/// 容器的高度
		/// </summary>
		[Category("Appearance")]
		[Browsable(false)]
		[ScriptControlProperty]
		[ClientPropertyName("containerHeight")]
		[Description("容器的高度")]
		public int ContainerHeight
		{
			get { return GetPropertyValue("ContainerHeight", 260); }
			set { SetPropertyValue("ContainerHeight", value); }
		}

		/// <summary>
		/// 控件定位
		/// </summary>
		[Category("Appearance")]
		[ScriptControlProperty]
		[ClientPropertyName("relativeLinkPosition")]
		[Description("控件定位")]
		public RelativeLinkPosition RelativeLinkPosition
		{
			get { return GetPropertyValue("RelativeLinkPosition", RelativeLinkPosition.Right); }
			set { SetPropertyValue("RelativeLinkPosition", value); }
		}

		/// <summary>
		/// 控件初始状态
		/// </summary>
		[Category("Appearance")]
		[ScriptControlProperty]
		[ClientPropertyName("relativeLinkStatus")]
		[Description("控件初始状态")]
		public RelativeLinkStatus RelativeLinkStatus
		{
			get { return GetPropertyValue("RelativeLinkStatus", RelativeLinkStatus.ByConfig); }
			set { SetPropertyValue("RelativeLinkStatus", value); }
		}

		/// <summary>
		/// 控件默认状态为打开还是关闭
		/// </summary>
		[Browsable(false)]
		[ScriptControlProperty]
		[ClientPropertyName("isOpen")]
		private bool IsOpen
		{
			get
			{
				if (RelativeLinkStatus == RelativeLinkStatus.ByConfig)
				{
					var showingMode = RelativeLinkControlConfigSetting.GetConfig().DefaultShowingMode;
					return string.Equals(showingMode, "Expanded", StringComparison.OrdinalIgnoreCase);
				}
				return RelativeLinkStatus == RelativeLinkStatus.Expanded;
			}
		}

		/// <summary>
		/// 容器的样式
		/// </summary>
		[Category("Appearance")]
		[ScriptControlProperty]
		[ClientPropertyName("containerStyle")]
		[Description("容器的样式")]
		public string ContainerStyle
		{
			get { return GetPropertyValue("ContainerStyle", string.Empty); }
			set { SetPropertyValue("ContainerStyle", value); }
		}

		/// <summary>
		/// 容器是否自适应
		/// </summary>
		[Category("Appearance")]
		[ScriptControlProperty]
		[ClientPropertyName("isSelfAdaption")]
		[Description("容器是否自适应")]
		public bool IsSelfAdaption
		{
			get { return GetPropertyValue("IsSelfAdaption", true); }
			set { SetPropertyValue("IsSelfAdaption", value); }
		}

		/// <summary>
		/// 是否一直垂直居中显示控件，不建议自己设置该值。
		/// </summary>
		[Category("Default")]
		[ScriptControlProperty]
		[ClientPropertyName("alwaysVerticalCenter")]
		[Description("是否一直垂直居中显示控件，不建议自己设置该值。")]
		public bool AlwaysVerticalCenter
		{
			get { return GetPropertyValue("AlwaysVerticalCenter", true); }
			set { SetPropertyValue("AlwaysVerticalCenter", value); }
		}

		/// <summary>
		/// 左侧标题的图标
		/// </summary>
		[Category("Appearance")]
		[ScriptControlProperty]
		[ClientPropertyName("titleRelativeLink")]
		[Description("左侧标题的图标")]
		[Editor("System.Web.UI.Design.ImageUrlEditor, System.Design", typeof(UITypeEditor))]
		public string TitleImg
		{
			get
			{
				return GetPropertyValue("TitleImg",
										Page.ClientScript.GetWebResourceUrl(typeof(RelativeLink),
																			"MCS.Web.WebControls.RelativeLink.Images.standard_title.png"));
			}
			set { SetPropertyValue("TitleImg", value); }
		}

		/// <summary>
		/// 左侧标题的图标
		/// </summary>
		[Category("Appearance")]
		[ScriptControlProperty]
		[ClientPropertyName("rightTitleRelativeLink")]
		[Description("右侧标题的图标")]
		[Editor("System.Web.UI.Design.ImageUrlEditor, System.Design", typeof(UITypeEditor))]
		public string RightTitleImg
		{
			get
			{
				return GetPropertyValue("RightTitleImg",
										Page.ClientScript.GetWebResourceUrl(typeof(RelativeLink),
																			"MCS.Web.WebControls.RelativeLink.Images.standard_title1.png"));
			}
			set { SetPropertyValue("RightTitleImg", value); }
		}

		/// <summary>
		/// 正文中的图标
		/// </summary>
		[Category("Appearance")]
		[Description("正文中的图标")]
		[Editor("System.Web.UI.Design.ImageUrlEditor, System.Design", typeof(UITypeEditor))]
		public string ImgIcon
		{
			get
			{
				return GetPropertyValue("ImgIcon",
										Page.ClientScript.GetWebResourceUrl(typeof(RelativeLink),
																			"MCS.Web.WebControls.RelativeLink.Images.icon1.png"));
			}
			set { SetPropertyValue("ImgIcon", value); }
		}

		/// <summary>
		/// 上箭头图标
		/// </summary>
		[Category("Appearance")]
		[ScriptControlProperty]
		[ClientPropertyName("grayUpArrowImg")]
		[Description("上箭头图标")]
		[Editor("System.Web.UI.Design.ImageUrlEditor, System.Design", typeof(UITypeEditor))]
		public string GrayUpArrowImg
		{
			get
			{
				return GetPropertyValue("GrayUpArrowImg",
										Page.ClientScript.GetWebResourceUrl(typeof(RelativeLink),
																			"MCS.Web.WebControls.RelativeLink.Images.gray_up.png"));
			}
			set { SetPropertyValue("GrayUpArrowImg", value); }
		}
		/// <summary>
		/// 下箭头图标
		/// </summary>
		[Category("Appearance")]
		[ScriptControlProperty]
		[ClientPropertyName("downArrowImg")]
		[Description("下箭头图标")]
		[Editor("System.Web.UI.Design.ImageUrlEditor, System.Design", typeof(UITypeEditor))]
		public string GrayDownArrowImg
		{
			get
			{
				return GetPropertyValue("GrayDownArrowImg",
										Page.ClientScript.GetWebResourceUrl(typeof(RelativeLink),
																			"MCS.Web.WebControls.RelativeLink.Images.gray_down.png"));
			}
			set { SetPropertyValue("GrayDownArrowImg", value); }
		}

		/// <summary>
		/// 上箭头图标
		/// </summary>
		[Category("Appearance")]
		[ScriptControlProperty]
		[ClientPropertyName("lightUpArrowImg")]
		[Description("上箭头图标")]
		[Editor("System.Web.UI.Design.ImageUrlEditor, System.Design", typeof(UITypeEditor))]
		public string LightUpArrowImg
		{
			get
			{
				return GetPropertyValue("LightUpArrowImg",
										Page.ClientScript.GetWebResourceUrl(typeof(RelativeLink),
																			"MCS.Web.WebControls.RelativeLink.Images.blue_up.png"));
			}
			set { SetPropertyValue("LightUpArrowImg", value); }
		}
		/// <summary>
		/// 下箭头图标
		/// </summary>
		[Category("Appearance")]
		[ScriptControlProperty]
		[ClientPropertyName("lightDownArrowImg")]
		[Description("下箭头图标")]
		[Editor("System.Web.UI.Design.ImageUrlEditor, System.Design", typeof(UITypeEditor))]
		public string LightDownArrowImg
		{
			get
			{
				return GetPropertyValue("LightDownArrowImg",
										Page.ClientScript.GetWebResourceUrl(typeof(RelativeLink),
																			"MCS.Web.WebControls.RelativeLink.Images.blue_down.png"));
			}
			set { SetPropertyValue("LightDownArrowImg", value); }
		}

		/// <summary>
		/// 上箭头中间分隔线图标
		/// </summary>
		[Category("Appearance")]
		[ScriptControlProperty]
		[ClientPropertyName("grayLineImg")]
		[Description("上箭头中间分隔线图标")]
		[Editor("System.Web.UI.Design.ImageUrlEditor, System.Design", typeof(UITypeEditor))]
		public string GrayLineImg
		{
			get
			{
				return GetPropertyValue("GrayLineImg",
										Page.ClientScript.GetWebResourceUrl(typeof(RelativeLink),
																			"MCS.Web.WebControls.RelativeLink.Images.gray_line.png"));
			}
			set { SetPropertyValue("GrayLineImg", value); }
		}

		/// <summary>
		/// 上箭头中间分隔线图标
		/// </summary>
		[Category("Appearance")]
		[ScriptControlProperty]
		[ClientPropertyName("lightLineImg")]
		[Description("上箭头中间分隔线图标")]
		[Editor("System.Web.UI.Design.ImageUrlEditor, System.Design", typeof(UITypeEditor))]
		public string LightLineImg
		{
			get
			{
				return GetPropertyValue("LightLineImg",
										Page.ClientScript.GetWebResourceUrl(typeof(RelativeLink),
																			"MCS.Web.WebControls.RelativeLink.Images.blue_line.png"));
			}
			set { SetPropertyValue("LightLineImg", value); }
		}
		/// <summary>
		/// 滑出界面的时间间隔单位是毫秒（默认为80ms）
		/// </summary>
		[Category("Default")]
		[ScriptControlProperty]
		[ClientPropertyName("timeInterval")]
		[Description("滑出界面的时间间隔单位是毫秒")]
		public int TimeInterval
		{
			get
			{
				return GetPropertyValue("TimeInterval", 80);
			}
			set { SetPropertyValue("TimeInterval", value); }
		}

		/// <summary>
		/// 热点光标
		/// </summary>
		[Category("Appearance")]
		[ScriptControlProperty]
		[ClientPropertyName("hotPointCursor")]
		[Description("热点光标")]
		[Editor("System.Web.UI.Design.ImageUrlEditor, System.Design", typeof(UITypeEditor))]
		public string HotPointCursor
		{
			get
			{
				return GetPropertyValue("HotPointCursor", string.Format("url({0}),auto", Page.ClientScript.GetWebResourceUrl(typeof(RelativeLink),
																			"MCS.Web.WebControls.RelativeLink.Images.hand1.cur")));

			}
			set { SetPropertyValue("HotPointCursor", value); }
		}

		/// <summary>
		/// 拖动光标
		/// </summary>
		[DefaultValue("move")]
		[Category("Appearance")]
		[ScriptControlProperty]
		[ClientPropertyName("movingCursor")]
		[Description("拖动光标")]
		public string MovingCursor
		{
			get
			{
				return GetPropertyValue("MovingCursor", "move");
			}
			set { SetPropertyValue("MovingCursor", value); }
		}

		/// <summary>
		/// 停止拖动光标
		/// </summary>
		[Category("Appearance")]
		[DefaultValue("auto")]
		[ScriptControlProperty]
		[ClientPropertyName("stoppingCursor")]
		[Description("停止拖动光标")]
		public string StoppingCursor
		{
			get
			{
				return GetPropertyValue("StoppingCursor", "auto");
			}
			set { SetPropertyValue("StoppingCursor", value); }
		}

		/// <summary>
		/// 扩展内容
		/// </summary>
		[Category("Default")]
		[ScriptControlProperty]
		[ClientPropertyName("extendPanel")]
		[Description("扩展内容")]
		public string ExtendContent
		{
			get { return GetPropertyValue("ExtendContent", string.Empty); }
			set { SetPropertyValue("ExtendContent", value); }
		}

		/// <summary>
		/// 扩展内容的显示样式 
		/// </summary>
		[Category("Appearance")]
		[ScriptControlProperty]
		[ClientPropertyName("extendContentStyle")]
		[Description("扩展内容的显示样式 ")]
		public string ExtendContentStyle
		{
			get { return GetPropertyValue("ExtendPanelStyle", string.Empty); }
			set { SetPropertyValue("ExtendPanelStyle", value); }
		}


		[Browsable(false)]
		protected IWfActivity GetDefaultActivity()
		{
			IWfActivity result = null;

			if (WfClientContext.Current.OriginalActivity != null)
				result = WfClientContext.Current.OriginalActivity.ApprovalRootActivity;

			return result;
		}

		[Browsable(false)]
		public WfRelativeLinkDescriptorCollection RelativeLinks
		{
			get
			{
				if (_relativeLinks == null)
				{
					var currentActivity = GetDefaultActivity();

					if (currentActivity != null)
					{
						this._relativeLinks = currentActivity.Descriptor.RelativeLinks;

						if (this._relativeLinks.Count == 0)
						{
							this._relativeLinks = currentActivity.Process.Descriptor.RelativeLinks;

							if (this._relativeLinks.Count == 0)
							{
								if (currentActivity.Process.EntryInfo != null)
									this._relativeLinks = currentActivity.Process.EntryInfo.ProcessTemplate.RelativeLinks;

								if (this._relativeLinks.Count == 0)
									this._relativeLinks = currentActivity.SameResourceRootActivity.Process.Descriptor.RelativeLinks;
							}
						}
					}
					else
					{
						this._relativeLinks = new WfRelativeLinkDescriptorCollection();
					}

					if (Category.IsNotEmpty())
						this._relativeLinks = this._relativeLinks.FilterByCategory(Category);
				}

				return this._relativeLinks;
			}
		}

		[Browsable(false)]
		public WfRelativeLinkDescriptorCollection MoreLinks
		{
			get
			{
				if (this._moreLinks == null)
				{
					var currentActivity = GetDefaultActivity();

					if (currentActivity != null)
					{
						this._moreLinks = currentActivity.Descriptor.RelativeLinks;

						if (this._moreLinks.Count == 0)
						{
							this._moreLinks = currentActivity.Process.Descriptor.RelativeLinks;

							if (this._moreLinks.Count == 0)
							{
								if (currentActivity.Process.EntryInfo != null)
									this._moreLinks = currentActivity.Process.EntryInfo.ProcessTemplate.RelativeLinks;

								if (this._moreLinks.Count == 0)
									this._moreLinks = currentActivity.SameResourceRootActivity.Process.Descriptor.RelativeLinks;
							}
						}
					}
					else
					{
						this._moreLinks = new WfRelativeLinkDescriptorCollection();
					}

					this._moreLinks = MoreLinkCategory != "" ? this._moreLinks.FilterByCategory(MoreLinkCategory) : new WfRelativeLinkDescriptorCollection();
				}

				return this._moreLinks;
			}
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="writer"></param>
		protected override void RenderContents(HtmlTextWriter writer)
		{
			if (!DesignMode)
			{
				switch (RelativeLinkPosition)
				{
					case RelativeLinkPosition.Left:
						LeftRender(writer);
						break;
					case RelativeLinkPosition.Right:
						RightRender(writer);
						break;
				}
			}
			base.RenderContents(writer);
		}

		/// <summary>
		/// 居左
		/// </summary>
		/// <param name="writer"></param>
		protected void LeftRender(HtmlTextWriter writer)
		{
			writer.AddAttribute(HtmlTextWriterAttribute.Id, OuterContainerClientID);
			writer.AddAttribute(HtmlTextWriterAttribute.Class, "container");
			writer.RenderBeginTag(HtmlTextWriterTag.Div);

			RenderBlockLayer(writer);
			writer.AddAttribute(HtmlTextWriterAttribute.Id, TitleClientID);
			writer.AddAttribute(HtmlTextWriterAttribute.Class, "rightTitleRelativeLink");
			writer.RenderBeginTag(HtmlTextWriterTag.Div);
			writer.AddAttribute(HtmlTextWriterAttribute.Src, RightTitleImg);
			writer.AddAttribute(HtmlTextWriterAttribute.Class, "titleRelativeLink");
			writer.AddAttribute(HtmlTextWriterAttribute.Title, "流程相关信息");
			writer.AddAttribute("rollout", "false");
			writer.AddStyleAttribute(HtmlTextWriterStyle.BorderStyle, "none");
			writer.AddStyleAttribute(HtmlTextWriterStyle.Cursor, "hand");
			writer.RenderBeginTag(HtmlTextWriterTag.Img);
			writer.RenderEndTag();
			writer.RenderEndTag();
			writer.AddAttribute(HtmlTextWriterAttribute.Class, "rightBoxR");
			writer.RenderBeginTag(HtmlTextWriterTag.Div);
			writer.AddAttribute(HtmlTextWriterAttribute.Class, "rightStandardLRelativeLink");
			writer.RenderBeginTag(HtmlTextWriterTag.Div);
			writer.RenderEndTag();
			writer.AddAttribute(HtmlTextWriterAttribute.Class, "standardM");
			writer.RenderBeginTag(HtmlTextWriterTag.Div);
			writer.AddAttribute(HtmlTextWriterAttribute.Id, "content");
			writer.AddAttribute(HtmlTextWriterAttribute.Class, "content");
			writer.RenderBeginTag(HtmlTextWriterTag.Div);
			writer.AddAttribute(HtmlTextWriterAttribute.Class, "titlepan");
			writer.RenderBeginTag(HtmlTextWriterTag.Span);
			writer.Write(Title);
			writer.RenderEndTag();
			writer.AddAttribute(HtmlTextWriterAttribute.Class, "grab_btn");
			writer.RenderBeginTag(HtmlTextWriterTag.Div);

			writer.AddAttribute(HtmlTextWriterAttribute.Href, "#");
			writer.RenderBeginTag(HtmlTextWriterTag.A);
			writer.AddAttribute(HtmlTextWriterAttribute.Src, GrayUpArrowImg);
			writer.AddAttribute(HtmlTextWriterAttribute.Id, UpArrowClientID);
			writer.RenderBeginTag(HtmlTextWriterTag.Img);
			writer.RenderEndTag();
			writer.RenderEndTag();

			writer.AddAttribute(HtmlTextWriterAttribute.Href, "#");
			writer.RenderBeginTag(HtmlTextWriterTag.A);
			writer.AddAttribute(HtmlTextWriterAttribute.Src, GrayLineImg);
			writer.AddAttribute(HtmlTextWriterAttribute.Id, DragAndDropPointClientID);
			writer.AddStyleAttribute(HtmlTextWriterStyle.Cursor, HotPointCursor);
			writer.RenderBeginTag(HtmlTextWriterTag.Img);
			writer.RenderEndTag();
			writer.RenderEndTag();

			writer.AddAttribute(HtmlTextWriterAttribute.Href, "#");
			writer.RenderBeginTag(HtmlTextWriterTag.A);
			writer.AddAttribute(HtmlTextWriterAttribute.Src, GrayDownArrowImg);
			writer.AddAttribute(HtmlTextWriterAttribute.Id, DownArrowClientID);
			writer.RenderBeginTag(HtmlTextWriterTag.Img);
			writer.RenderEndTag();
			writer.RenderEndTag();

			writer.RenderEndTag();
			writer.AddAttribute(HtmlTextWriterAttribute.Class, "line");
			writer.RenderBeginTag(HtmlTextWriterTag.Ul);
			writer.RenderEndTag();
			writer.AddAttribute(HtmlTextWriterAttribute.Class, "olContent");
			writer.RenderBeginTag(HtmlTextWriterTag.Ol);

			foreach (var item in RelativeLinks)
			{
				writer.AddAttribute(HtmlTextWriterAttribute.Class, "liContent");
				writer.RenderBeginTag(HtmlTextWriterTag.Li);
				writer.AddAttribute(HtmlTextWriterAttribute.Href, item.Url);
				writer.AddAttribute(HtmlTextWriterAttribute.Target, LinkTarget);
				writer.AddAttribute(HtmlTextWriterAttribute.Title, item.Description);
				writer.AddAttribute(HtmlTextWriterAttribute.Class, "aContent");
				writer.RenderBeginTag(HtmlTextWriterTag.A);
				writer.Write(item.Name);
				writer.RenderEndTag();
				writer.RenderEndTag();
			}

			if (!string.IsNullOrEmpty(ExtendContent))
			{
				writer.AddAttribute(HtmlTextWriterAttribute.Class, "liContent");
				writer.AddAttribute(HtmlTextWriterAttribute.Style, ExtendContentStyle);
				writer.RenderBeginTag(HtmlTextWriterTag.Li);
				writer.Write(ExtendContent);
				writer.RenderEndTag();
			}

			writer.RenderEndTag();

			foreach (var item in MoreLinks)
			{
				writer.AddAttribute(HtmlTextWriterAttribute.Class, "more");
				writer.RenderBeginTag(HtmlTextWriterTag.P);
				writer.AddAttribute(HtmlTextWriterAttribute.Href, item.Url);
				writer.AddAttribute(HtmlTextWriterAttribute.Target, "_blank");
				writer.AddAttribute(HtmlTextWriterAttribute.Title, item.Name);
				writer.AddAttribute(HtmlTextWriterAttribute.Class, "liContent");
				writer.RenderBeginTag(HtmlTextWriterTag.A);
				writer.Write(item.Name);
				writer.RenderEndTag();
				writer.RenderEndTag();
			}

			writer.RenderEndTag();

			writer.RenderEndTag();
			writer.AddAttribute(HtmlTextWriterAttribute.Id, "rightStandardRRelativeLink");
			writer.AddAttribute(HtmlTextWriterAttribute.Class, "rightStandardR");
			writer.RenderBeginTag(HtmlTextWriterTag.Div);
			writer.RenderEndTag();
			writer.RenderEndTag();
			writer.RenderEndTag();
		}

		/// <summary>
		/// 居右
		/// </summary>
		/// <param name="writer"></param>
		private void RightRender(HtmlTextWriter writer)
		{
			writer.AddAttribute(HtmlTextWriterAttribute.Id, OuterContainerClientID);
			writer.AddAttribute(HtmlTextWriterAttribute.Class, "container");
			writer.RenderBeginTag(HtmlTextWriterTag.Div);

			RenderBlockLayer(writer);
			writer.AddAttribute(HtmlTextWriterAttribute.Id, TitleClientID);
			writer.AddAttribute(HtmlTextWriterAttribute.Class, "titleRelativeLink");
			writer.RenderBeginTag(HtmlTextWriterTag.Div);
			writer.AddAttribute(HtmlTextWriterAttribute.Src, TitleImg);
			writer.AddAttribute(HtmlTextWriterAttribute.Class, "titleRelativeLink");
			writer.AddAttribute(HtmlTextWriterAttribute.Title, "流程相关信息");
			writer.AddAttribute("rollout", "false");
			writer.AddStyleAttribute(HtmlTextWriterStyle.BorderStyle, "none");
			writer.AddStyleAttribute(HtmlTextWriterStyle.Cursor, "hand");
			writer.RenderBeginTag(HtmlTextWriterTag.Img);
			writer.RenderEndTag();
			writer.RenderEndTag();
			writer.AddAttribute(HtmlTextWriterAttribute.Class, "boxR");
			writer.RenderBeginTag(HtmlTextWriterTag.Div);
			writer.AddAttribute(HtmlTextWriterAttribute.Class, "standardLRelativeLink");
			writer.RenderBeginTag(HtmlTextWriterTag.Div);
			writer.RenderEndTag();
			writer.AddAttribute(HtmlTextWriterAttribute.Class, "standardM");
			writer.RenderBeginTag(HtmlTextWriterTag.Div);
			writer.AddAttribute(HtmlTextWriterAttribute.Class, "content");
			writer.RenderBeginTag(HtmlTextWriterTag.Div);
			writer.AddAttribute(HtmlTextWriterAttribute.Class, "titlepan");
			writer.RenderBeginTag(HtmlTextWriterTag.Span);
			writer.Write(Title);
			writer.RenderEndTag();
			writer.AddAttribute(HtmlTextWriterAttribute.Class, "grab_btn");
			writer.RenderBeginTag(HtmlTextWriterTag.Div);

			writer.AddAttribute(HtmlTextWriterAttribute.Href, "#");
			writer.RenderBeginTag(HtmlTextWriterTag.A);
			writer.AddAttribute(HtmlTextWriterAttribute.Src, GrayUpArrowImg);
			writer.AddAttribute(HtmlTextWriterAttribute.Id, UpArrowClientID);
			writer.RenderBeginTag(HtmlTextWriterTag.Img);
			writer.RenderEndTag();
			writer.RenderEndTag();

			writer.AddAttribute(HtmlTextWriterAttribute.Href, "#");
			writer.RenderBeginTag(HtmlTextWriterTag.A);
			writer.AddAttribute(HtmlTextWriterAttribute.Src, GrayLineImg);
			writer.AddAttribute(HtmlTextWriterAttribute.Id, DragAndDropPointClientID);
			writer.AddStyleAttribute(HtmlTextWriterStyle.Cursor, HotPointCursor);
			writer.RenderBeginTag(HtmlTextWriterTag.Img);
			writer.RenderEndTag();
			writer.RenderEndTag();

			writer.AddAttribute(HtmlTextWriterAttribute.Href, "#");
			writer.RenderBeginTag(HtmlTextWriterTag.A);
			writer.AddAttribute(HtmlTextWriterAttribute.Src, GrayDownArrowImg);
			writer.AddAttribute(HtmlTextWriterAttribute.Id, DownArrowClientID);
			writer.RenderBeginTag(HtmlTextWriterTag.Img);
			writer.RenderEndTag();
			writer.RenderEndTag();

			writer.RenderEndTag();
			writer.AddAttribute(HtmlTextWriterAttribute.Class, "line");
			writer.RenderBeginTag(HtmlTextWriterTag.Ul);
			writer.RenderEndTag();
			writer.AddAttribute(HtmlTextWriterAttribute.Class, "olContent");
			writer.RenderBeginTag(HtmlTextWriterTag.Ol);

			foreach (var item in RelativeLinks)
			{
				writer.AddAttribute(HtmlTextWriterAttribute.Class, "liContent");
				writer.RenderBeginTag(HtmlTextWriterTag.Li);
				writer.AddAttribute(HtmlTextWriterAttribute.Href, item.Url);
				writer.AddAttribute(HtmlTextWriterAttribute.Target, LinkTarget);
				writer.AddAttribute(HtmlTextWriterAttribute.Title, item.Description);
				writer.AddAttribute(HtmlTextWriterAttribute.Class, "aContent");
				writer.RenderBeginTag(HtmlTextWriterTag.A);
				writer.Write(item.Name);
				writer.RenderEndTag();
				writer.RenderEndTag();
			}
			if (!string.IsNullOrEmpty(ExtendContent))
			{
				writer.AddAttribute(HtmlTextWriterAttribute.Class, "liContent");
				writer.AddAttribute(HtmlTextWriterAttribute.Style, ExtendContentStyle);
				writer.RenderBeginTag(HtmlTextWriterTag.Li);
				writer.Write(ExtendContent);
				writer.RenderEndTag();
			}

			writer.RenderEndTag();

			foreach (var item in MoreLinks)
			{
				writer.AddAttribute(HtmlTextWriterAttribute.Class, "more");
				writer.RenderBeginTag(HtmlTextWriterTag.P);
				writer.AddAttribute(HtmlTextWriterAttribute.Href, item.Url);
				writer.AddAttribute(HtmlTextWriterAttribute.Target, "_blank");
				writer.AddAttribute(HtmlTextWriterAttribute.Title, item.Name);
				writer.AddAttribute(HtmlTextWriterAttribute.Class, "amore");
				writer.RenderBeginTag(HtmlTextWriterTag.A);
				writer.Write(item.Name);
				writer.RenderEndTag();
				writer.RenderEndTag();
			}

			writer.RenderEndTag();

			writer.RenderEndTag();

			writer.AddAttribute(HtmlTextWriterAttribute.Class, "standardR");
			writer.RenderBeginTag(HtmlTextWriterTag.Div);
			writer.RenderEndTag();
			writer.RenderEndTag();
			writer.RenderEndTag();
		}

		/// <summary>
		/// 打底裤层
		/// </summary>
		/// <param name="writer"></param>
		private void RenderBlockLayer(HtmlTextWriter writer)
		{
			writer.AddAttribute(HtmlTextWriterAttribute.Style,
								"position: absolute; z-index: -1; width:100%;top: 0px;left: 4px;scrolling: no;margin-top:10px;");
			writer.AddAttribute("height", string.Format("{0}px", TitleHeight - 20));
			writer.AddAttribute("frameborder", "0");
			writer.AddAttribute(HtmlTextWriterAttribute.Src, "about:blank");
			writer.RenderBeginTag(HtmlTextWriterTag.Iframe);
			writer.RenderEndTag();
		}
	}
}
