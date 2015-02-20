using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Drawing;
using System.Web;
using System.Web.UI;
using System.Web.UI.HtmlControls;
using System.Web.UI.WebControls;
using MCS.Library.Core;
using MCS.Library.Globalization;
using MCS.Web.Library.Script;
using MCS.Web.Responsive.Library;
using MCS.Web.Responsive.Library.Script;

[assembly: WebResource("MCS.Web.Responsive.WebControls.DeluxeGrid.DeluxeGrid.js", "application/x-javascript")]
[assembly: WebResource("MCS.Web.Responsive.WebControls.DeluxeGrid.DeluxeGrid.css", "text/css")]
[assembly: WebResource("MCS.Web.Responsive.WebControls.DeluxeGrid.excel.gif", "image/gif")]
[assembly: WebResource("MCS.Web.Responsive.WebControls.DeluxeGrid.word.gif", "image/gif")]

namespace MCS.Web.Responsive.WebControls
{
	/// <summary>
	/// DeluxeGrid 控件 继承自GridView
	/// </summary>
	/// <remarks>
	/// DeluxeGrid 控件 继承自GridView
	/// </remarks>
	//引用所需的Css
	[DefaultProperty("DeluxeGrid"),
	ToolboxData("<{0}:DeluxeGrid runat=server></{0}:DeluxeGrid>")]
	[ParseChildren(true),
	PersistChildren(false)]
	[RequiredScript(typeof(ControlBaseScript))]
	[ClientCssResource("MCS.Web.Responsive.WebControls.DeluxeGrid.DeluxeGrid.css")]
	[ClientScriptResource("MCS.Web.WebControls.DeluxeGrid", "MCS.Web.Responsive.WebControls.DeluxeGrid.DeluxeGrid.js")]
	public class DeluxeGrid : ScriptGridViewBase, ICascadePagedControl
	{
		/// <summary>
		/// 用于设计态
		/// </summary>
		internal List<IAttributeAccessor> regions;

		/// <summary>
		/// 用来搞设计态的东西用的容器
		/// </summary>
		private Panel cPanelContent;

		/// <summary>
		/// 构造函数
		/// </summary>
		/// <remarks>
		/// 构造函数
		/// </remarks>
		public DeluxeGrid()
		{
		}

		#region 私有变量
		/// <summary>
		/// 客户端导出Excel按钮
		/// </summary>
		/// <remarks>
		/// 客户端导出Excel按钮
		/// </remarks>
		private HtmlImage clientExportExcelButton = new HtmlImage() { ID = "exportFiles" };

		/// <summary>
		/// 客户端导出Word按钮
		/// </summary>
		/// <remarks>
		/// 客户端导出Word按钮
		/// </remarks>
		private HtmlImage clientExportWordButton = new HtmlImage() { ID = "exportWordFiles" };

		/// <summary>
		/// 客户端导出Excel顶端按钮
		/// </summary>
		/// <remarks>
		/// 客户端导出Excel顶端按钮
		/// </remarks>
		private HtmlImage clientExportExcelTopButton = new HtmlImage() { ID = "exportTopFiles" };

		/// <summary>
		/// 客户端导出Word顶端按钮
		/// </summary>
		/// <remarks>
		/// 客户端导出Word顶端按钮
		/// </remarks>
		private HtmlImage clientExportWordTopButton = new HtmlImage() { ID = "exportTopWordFiles" };

		/// <summary>
		/// checked总数
		/// </summary>
		int selectedCount = 0;

		/// <summary>
		/// 放置checkbox模版
		/// </summary>
		/// <remarks>
		/// 放置checkbox模版
		/// </remarks>
		TemplateField tempCheckBox = new TemplateField();

		/// <summary>
		/// 顶端翻页是否创建
		/// </summary>
		/// <remarks>
		/// 顶端翻页是否创建
		/// </remarks>
		private bool topPagerCreating = false;

		/// <summary>
		/// 导出excelbutton
		/// </summary>
		private Button btnExcelExport = new Button() { ID = "btnExcelServer", CommandName = "excel" };

		/// <summary>
		/// 导出wordbutton
		/// </summary>
		private Button btnWordExport = new Button() { ID = "btnWordServer", CommandName = "word" };

		/// <summary>
		/// 标题控件，在上边显示的
		/// </summary>
		Label lblUpTitle = new Label();
		/// <summary>
		/// 标题控件，在下边显示的（目前先预留吧）
		/// </summary>
		Label lblLowTitle = new Label();
		#endregion

		#region 私有属性

		/// <summary>
		/// 属性对象模型
		/// </summary>
		/// <remarks>
		/// 属性对象模型
		/// </remarks>
		private System.Web.UI.AttributeCollection properties;

		/// <summary>
		/// 所有属性对象访问模型
		/// </summary>
		/// <remarks>
		/// 所有属性对象访问模型
		/// </remarks>
		internal System.Web.UI.AttributeCollection Properties
		{
			get
			{
				if (this.properties == null)
				{
					StateBag oBag = new StateBag(true);
					this.properties = new System.Web.UI.AttributeCollection(oBag);
				}

				return this.properties;
			}
			set
			{
				this.properties = value;
			}
		}

		/// <summary>
		/// 导出功能excel的图标
		/// </summary>
		/// <remarks>
		/// 导出功能excel的图标
		/// </remarks>
		[Description("导出功能excel的图标")]
		private string ExcelImgUrl
		{
			get
			{
				return Page.ClientScript.GetWebResourceUrl(typeof(DeluxeGrid),
						"MCS.Web.Responsive.WebControls.DeluxeGrid.excel.gif");
			}
		}

		/// <summary>
		/// 导出功能word的图标
		/// </summary>
		/// <remarks>
		/// 导出功能word的图标
		/// </remarks>
		[Description("导出功能word的图标")]
		private string WordImgUrl
		{
			get
			{
				return Page.ClientScript.GetWebResourceUrl(typeof(DeluxeGrid),
						"MCS.Web.Responsive.WebControls.DeluxeGrid.word.gif");
			}
		}

		[Browsable(false)]
		[ScriptControlProperty()]
		[ClientPropertyName("openExportWordDocumentUrl")]
		private string OpenExportWordDocumentUrl
		{
			get
			{
				PageRenderMode pageRenderMode = new PageRenderMode(this.UniqueID, "openExportWordDocumentUrl");

				pageRenderMode.ContentTypeKey = ResponseContentTypeKey.WORD.ToString();
				pageRenderMode.DispositionType = ResponseDispositionType.Inline;
				pageRenderMode.AttachmentFileName = "exports.doc";
				pageRenderMode.UseNewPage = false;

				return Request.AppendToExecutionUrl(pageRenderMode);
			}
		}

		[Browsable(false)]
		[ScriptControlProperty()]
		[ClientPropertyName("checkItemName")]
		private string CheckItemName
		{
			get
			{
				return this.ClientID + "_" + "checkItem";
			}
		}

		[Browsable(false)]
		[ScriptControlProperty()]
		[ClientPropertyName("openExportExcelDocumentUrl")]
		private string OpenExportExcelDocumentUrl
		{
			get
			{
				PageRenderMode pageRenderMode = new PageRenderMode(this.UniqueID, "openExportExcelDocumentUrl");

				pageRenderMode.ContentTypeKey = ResponseContentTypeKey.EXCEL.ToString();
				pageRenderMode.DispositionType = ResponseDispositionType.Inline;
				pageRenderMode.AttachmentFileName = "exports.xls";
				pageRenderMode.UseNewPage = false;

				return Request.AppendToExecutionUrl(pageRenderMode);
			}
		}

		/// <summary>
		/// 导出功能excel的图标
		/// </summary>
		/// <remarks>
		/// 导出功能excel的图标
		/// </remarks> 
		[Category("Appearance")]
		[ScriptControlProperty]//设置此属性要输出到客户端
		[ClientPropertyName("btnExcelClientID")]//设置此属性对应客户端属性的名称
		[Description("ExcelButtonClientID服务器端导出按钮clientID")]
		private string BtnExcelClientID
		{
			get
			{
				return btnExcelExport.ClientID;
			}
		}

		/// <summary>
		/// 导出功能excel的图标
		/// </summary>
		/// <remarks>
		/// 导出功能excel的图标
		/// </remarks>
		[Category("Appearance")]
		[ScriptControlProperty]//设置此属性要输出到客户端
		[ClientPropertyName("btnWordClientID")]//设置此属性对应客户端属性的名称
		[Description("WordButtonClientID服务器端导出按钮clientID")]
		private string BtnWordClientID
		{
			get
			{
				return btnWordExport.ClientID;
			}
		}

		/// <summary>
		/// 客户端导出ButtonID
		/// </summary>
		/// <remarks>
		/// 客户端导出ButtonID
		/// </remarks> 
		[Category("Appearance")]
		[ScriptControlProperty]//设置此属性要输出到客户端
		[ClientPropertyName("excelTopButtonClientID")]//设置此属性对应客户端属性的名称
		[Description("ExportButton客户端导出按钮ID")]
		private string ExcelTopButtonClientID
		{
			get
			{
				return this.clientExportExcelTopButton.ClientID;
			}
		}

		/// <summary>
		/// 客户端导出ButtonID
		/// </summary>
		/// <remarks>
		/// 客户端导出ButtonID
		/// </remarks> 
		[Category("Appearance")]
		[ScriptControlProperty]//设置此属性要输出到客户端
		[ClientPropertyName("excelButtonClientID")]//设置此属性对应客户端属性的名称
		[Description("ClientExportExcelTopButton客户端导出按钮ID")]
		private string ExcelButtonClientID
		{
			get
			{
				return this.clientExportExcelButton.ClientID;
			}
		}

		/// <summary>
		/// 客户端导出wordButtonID
		/// </summary>
		/// <remarks>
		/// 客户端导出wordButtonID
		/// </remarks> 
		[Category("Appearance")]
		[ScriptControlProperty]//设置此属性要输出到客户端
		[ClientPropertyName("wordTopButtonClientID")]//设置此属性对应客户端属性的名称
		[Description("WordTopButtonClientID客户端导出按钮ID")]
		private string WordTopButtonClientID
		{
			get
			{
				return this.clientExportWordTopButton.ClientID;
			}
		}

		/// <summary>
		/// 客户端导出wordButtonID
		/// </summary>
		/// <remarks>
		/// 客户端导出wordButtonID
		/// </remarks> 
		[Category("Appearance")]
		[ScriptControlProperty]//设置此属性要输出到客户端
		[ClientPropertyName("wordButtonClientID")]//设置此属性对应客户端属性的名称
		[Description("WordButtonClientID客户端导出按钮ID")]
		private string WordButtonClientID
		{
			get
			{
				return this.clientExportWordButton.ClientID;
			}
		}

		/// <summary>
		/// 是否正在导出状态
		/// </summary>
		/// <remarks>
		/// 是否正在导出状态
		/// </remarks> 
		[Browsable(false), EditorBrowsable(EditorBrowsableState.Advanced), DefaultValue(false)]
		public bool ExportingDeluxeGrid
		{
			get
			{
				return this.ViewState.GetViewStateValue("ExportingDeluxeGrid", false);
			}
			set
			{
				this.ViewState.SetViewStateValue("ExportingDeluxeGrid", value);
			}
		}
		#endregion

		#region public ITemplate GridPagerTemplate
		/// <summary>
		/// DeluxePager模版
		/// </summary>
		/// <remarks>
		/// DeluxePager模版
		/// </remarks>
		[Browsable(false)]
		[Description("DeluxePager模版")]
		[MergableProperty(false)]
		[DefaultValue(null)]
		[PersistenceMode(PersistenceMode.InnerProperty)]
		[TemplateContainer(typeof(GridViewRow))]
		[TemplateInstance(TemplateInstance.Single)]
		public ITemplate GridPagerTemplate
		{
			get
			{
				return this.ViewState.GetViewStateValue("GridPagerTemplate", (ITemplate)null);
			}
			set
			{
				this.ViewState.SetViewStateValue("GridPagerTemplate", value);
			}
		}
		#endregion

		#region 内部属性
		/// <summary>
		/// 当页的checkbox是否全选
		/// </summary>
		/// <remarks>
		/// 当页的checkbox是否全选
		/// </remarks>
		[Browsable(false)]
		[Category("Appearance")]
		[ScriptControlProperty]//设置此属性要输出到客户端
		[ClientPropertyName("isSelectedAll")]//设置此属性对应客户端属性的名称
		[Description("是否全选")]
		private bool IsSelectedAll
		{
			get
			{
				return this.ViewState.GetViewStateValue("IsSelectedAll", false);
			}
			set
			{
				this.ViewState.SetViewStateValue("IsSelectedAll", value);
			}
		}

		/// <summary>
		/// checkall全选客户端名称
		/// </summary>
		/// <remarks>
		/// checkall全选客户端名称
		/// </remarks> 
		[Category("Appearance")]
		[ScriptControlProperty]//设置此属性要输出到客户端
		[ClientPropertyName("checkAll")]//设置此属性对应客户端属性的名称
		[Description("checkbox全选客户端名称")]
		[Browsable(false)]
		private string CheckAllID
		{
			get
			{
				return this.ViewState.GetViewStateValue("CheckAllID", string.Empty);
			}
			set
			{
				this.ViewState.SetViewStateValue("CheckAllID", value);
			}
		}

		/// <summary>
		/// (全选)复选框单击事件
		/// </summary>
		[DefaultValue("")]
		[ScriptControlEvent]
		[ClientPropertyName("selectAllCheckBoxClick")]
		[Bindable(true), Category("ClientEventsHandler"), Description("(全选)复选框单击事件")]
		public string OnSelectAllCheckBoxClick
		{
			get
			{
				return GetPropertyValue("OnSelectAllCheckBoxClick", string.Empty);
			}
			set
			{
				SetPropertyValue("OnSelectAllCheckBoxClick", value);
			}
		}

		/// <summary>
		/// 复选框(不包括全选)单击事件
		/// </summary>
		[DefaultValue("")]
		[ScriptControlEvent]
		[ClientPropertyName("selectCheckBoxClick")]
		[Bindable(true), Category("ClientEventsHandler"), Description("复选框(不包括全选)单击事件")]
		public string OnSelectCheckBoxClick
		{
			get
			{
				return GetPropertyValue("OnSelectCheckBoxClick", string.Empty);
			}
			set
			{
				SetPropertyValue("OnSelectCheckBoxClick", value);
			}
		}

		private List<string> selectedKeys = null;

		/// <summary>
		/// 选择后的Key集合
		/// </summary> 
		/// <remarks>
		/// 选择后的Key集合
		/// </remarks> 
		[ScriptControlProperty]//设置此属性要输出到客户端
		[ClientPropertyName("clientSelectedKeys")]//设置此属性对应客户端属性的名称
		[Description("选择后的Key集合")]
		[Browsable(false)]
		public List<string> SelectedKeys
		{
			get
			{
				if (this.selectedKeys == null)
				{
					this.selectedKeys = new List<string>();
					this.ViewState.SetViewStateValue("SelectedKeys", this.selectedKeys);
				}

				return this.selectedKeys;
			}
		}

		/// <summary>
		/// 当前页中的checkbox是否全选
		/// </summary>
		/// <remarks>
		/// 当前页中的checkbox是否全选
		/// </remarks>
		[Browsable(false), DefaultValue(false)]
		[Category("Appearance")]
		[ScriptControlProperty]//设置此属性要输出到客户端
		[ClientPropertyName("isChecked")]//设置此属性对应客户端属性的名称
		[Description("当前页中的checkbox是否全选")]
		public bool CheckedAll
		{
			get
			{
				return this.ViewState.GetViewStateValue("CheckedAll", false);
			}
			set
			{
				this.ViewState.SetViewStateValue("CheckedAll", value);
			}
		}

		/// <summary>
		/// 获取隐藏域中选中的checkbox的value值，逗号分隔
		/// </summary>
		/// <remarks>
		/// 获取隐藏域中选中的checkbox的value值，逗号分隔
		/// </remarks>
		[Browsable(false)]
		public string SelectedKeysValue
		{
			get
			{
				return string.Join(",", this.SelectedKeys.ToArray());
			}
		}
		#endregion

		#region 公共属性
		/// <summary>
		/// 下级控件ID。必须实现了ICascadePagedControl接口
		/// </summary>
		[IDReferenceProperty(typeof(ICascadePagedControl))]
		public string CascadeControlID
		{
			get
			{
				return this.ViewState.GetViewStateValue("CascadeControlID", string.Empty);
			}
			set
			{
				this.ViewState.SetViewStateValue("CascadeControlID", value);
			}
		}

		/// <summary>
		/// Culture的Category
		/// </summary>
		[Browsable(true),
		Description("Culture的Category"),
		DefaultValue(""),
		Category("扩展")]
		public string Category
		{
			get
			{
				return this.ViewState.GetViewStateValue("Category", string.Empty);
			}
			set
			{
				this.ViewState.SetViewStateValue("Category", value);
			}
		}

		/// <summary>
		/// 是否允许客户端的数据绑定
		/// </summary>
		[Browsable(true),
		Description("是否允许客户端的数据绑定"),
		DefaultValue(false),
		Category("扩展")]
		[ScriptControlProperty()]
		[ClientPropertyName("allowClientDataBind")]
		public bool AllowClientDataBind
		{
			get
			{
				return this.ViewState.GetViewStateValue("AllowClientDataBind", false);
			}
			set
			{
				this.ViewState.SetViewStateValue("AllowClientDataBind", value);
			}
		}

		/// <summary>
		/// 设置跳转页按钮Text
		/// </summary>
		/// <remarks>
		/// 设置跳转页按钮Text
		/// </remarks>
		[Browsable(true),
		Description("设置跳转页按钮的Text"),
		DefaultValue("跳转到"),
		Category("扩展")]
		public string GotoButtonText
		{
			get
			{
				return this.ViewState.GetViewStateValue("GotoButtonText", Translator.Translate(Define.DefaultCategory, "跳转到"));
			}
			set
			{
				this.ViewState.SetViewStateValue("GotoButtonText", value);
			}
		}

		/// <summary>
		/// 页码显示模式
		/// </summary>
		/// <remarks>
		/// 页码显示模式
		/// </remarks>
		[Browsable(true)]
		[Description("页码显示模式")]
		[DefaultValue(PagerCodeShowMode.Auto)]
		[Category("扩展")]
		public PagerCodeShowMode PageCodeShowMode
		{
			get
			{
				return this.ViewState.GetViewStateValue("PageCodeShowMode", PagerCodeShowMode.Auto);
			}
			set
			{
				this.ViewState.SetViewStateValue("PageCodeShowMode", value);
			}
		}

		/// <summary>
		/// 指定GridView上导出内容的数据源的最大行数
		/// </summary>
		/// <remarks>
		/// 指定GridView上导出内容的数据源的最大行数
		/// </remarks>
		[Browsable(false),
		Category("扩展"),
		DefaultValue(200),
		Description("指定DeluxeView上导出内容的数据源的最大行数。")]
		public int DataSourceMaxRow
		{
			get
			{
				return this.ViewState.GetViewStateValue("DataSourceMaxRow", 0);
			}
			set
			{
				this.ViewState.SetViewStateValue("DataSourceMaxRow", value);
			}
		}

		/// <summary>
		/// 指定GridView上显示标题内容
		/// </summary>
		/// <remarks>
		/// 指定GridView上显示标题内容
		/// </remarks>
		[Browsable(true),
		Category("扩展"),
		DefaultValue("标题"),
		Description("指定GridView上显示标题内容。")]
		public string GridTitle
		{
			get
			{
				return this.ViewState.GetViewStateValue("GridTitle", string.Empty);
			}
			set
			{
				this.ViewState.SetViewStateValue("GridTitle", value);
			}
		}

		///// <summary>
		///// 标题字体颜色
		///// </summary>
		///// <remarks>
		///// 标题字体颜色
		///// </remarks>
		//[Browsable(true),
		//Category("扩展"),
		//DefaultValue(typeof(System.Drawing.Color)),
		//Description("标题字体颜色")]
		//public Color TitleColor
		//{
		//    get
		//    {
		//        return this.ViewState.GetViewStateValue("TitleColor", Color.FromArgb(141, 143, 149));
		//    }
		//    set
		//    {
		//        this.ViewState.SetViewStateValue("TitleColor", value);
		//    }
		//}
		///// <summary>
		///// 标题字体大小
		///// </summary>
		///// <remarks>
		///// 标题字体大小
		///// </remarks>
		//[Browsable(true),
		//Category("扩展"),
		//DefaultValue(typeof(FontUnit)),
		//Description("标题字体")]
		//public FontUnit TitleFontSize
		//{
		//    get
		//    {
		//        return this.ViewState.GetViewStateValue<FontUnit>("TitleFontSize", FontUnit.Large);
		//    }
		//    set
		//    {
		//        this.ViewState.SetViewStateValue<FontUnit>("TitleFontSize", value);
		//    }
		//}

		/// <summary>
		/// 设置是否显示导出部分
		/// </summary>
		/// <remarks>
		/// 设置是否显示导出部分
		/// </remarks>
		[Browsable(true),
		Description("是否显示导出部分"),
		DefaultValue(true),
		ScriptControlProperty,//设置此属性要输出到客户端
		ClientPropertyName("showExportControl"),//设置此属性对应客户端属性的名称
		Category("扩展")]
		public bool ShowExportControl
		{
			get
			{
				return this.ViewState.GetViewStateValue("ShowExportControl", false);
			}
			set
			{
				this.ViewState.SetViewStateValue("ShowExportControl", value);
			}
		}

		[
		ScriptControlProperty,//设置此属性要输出到客户端
		ClientPropertyName("showPager"),//设置此属性对应客户端属性的名称
		]
		private bool ShowPager
		{
			get
			{
				return this.AllowPaging;
			}
		}

		/// <summary>
		/// 设置是否增加checkbox列
		/// </summary>
		/// <remarks>
		/// 设置是否增加checkbox列
		/// </remarks>
		[Browsable(true),
		Description("是否增加选择列"),
		DefaultValue(false),
		ScriptControlProperty,//设置此属性要输出到客户端
		ClientPropertyName("showCheckBoxes"),//设置此属性对应客户端属性的名称
		Category("扩展")]
		public bool ShowCheckBoxes
		{
			get
			{
				return this.ViewState.GetViewStateValue("ShowCheckBoxes", false);
			}
			set
			{
				this.ViewState.SetViewStateValue("ShowCheckBoxes", value);
			}
		}

		/// <summary>
		/// 如果显示选择列，是否是多选
		/// </summary>
		[Browsable(true),
		Description("如果显示选择列，是否是多选"),
		DefaultValue(true),
			//ScriptControlProperty,//设置此属性要输出到客户端
			//ClientPropertyName("showCheckBoxs"),//设置此属性对应客户端属性的名称
		Category("扩展")]
		public bool MultiSelect
		{
			get
			{
				return this.ViewState.GetViewStateValue("MultiSelect", true);
			}
			set
			{
				this.ViewState.SetViewStateValue("MultiSelect", value);
			}
		}

		/// <summary>
		/// 设置checkbox列的位置
		/// </summary> 
		/// <remarks>
		/// 设置checkbox列的位置
		/// </remarks>
		[Browsable(true)]
		[Description("选择列的位置")]
		[DefaultValue(RowPosition.Left)]
		[Category("扩展")]
		public RowPosition CheckBoxPosition
		{
			get
			{
				return this.ViewState.GetViewStateValue("CheckBoxPosition", RowPosition.Left);
			}
			set
			{
				this.ViewState.SetViewStateValue("CheckBoxPosition", value);
			}
		}

		/// <summary>
		/// checkbox模板头样式
		/// </summary>
		/// <remarks>
		/// checkbox模板头样式
		/// </remarks>
		[Browsable(true)]
		[Description("checkbox模板头样式")]
		[Category("扩展")]
		[DesignerSerializationVisibility(DesignerSerializationVisibility.Content), NotifyParentProperty(true), PersistenceMode(PersistenceMode.InnerProperty)]
		public Style CheckBoxTemplateHeaderStyle
		{
			get
			{
				Style style = this.GetPropertyValue<System.Web.UI.WebControls.Style>("CheckBoxTemplateHeaderStyle", null);

				if (style == null)
				{
					style = new Style();
					style.CssClass = "center";
					this.SetPropertyValue<System.Web.UI.WebControls.Style>("CheckBoxTemplateHeaderStyle", style);
				}

				return style;
			}
		}

		/// <summary>
		/// checkbox模板内容对象样式
		/// </summary>
		/// <remarks>
		/// checkbox模板内容对象样式
		/// </remarks> 
		[Browsable(true)]
		[Description("checkbox模板内容对象样式")]
		[Category("扩展")]
		[DesignerSerializationVisibility(DesignerSerializationVisibility.Content), NotifyParentProperty(true), PersistenceMode(PersistenceMode.InnerProperty)]
		public Style CheckBoxTemplateItemStyle
		{
			get
			{
				Style style = this.GetPropertyValue<System.Web.UI.WebControls.Style>("CheckBoxTemplateItemStyle", null);

				if (style == null)
				{
					style = new Style();
					style.CssClass = "center";
					this.SetPropertyValue<System.Web.UI.WebControls.Style>("CheckBoxTemplateItemStyle", style);
				}

				return style;
			}
		}

		/// <summary>
		/// PagerCss页码样式
		/// </summary> 
		[Description("数据源是否用的是DataSourceControl")]
		[Category("扩展")]
		private bool IsDataSourceControl
		{
			get
			{
				return string.IsNullOrEmpty(DataSourceID) == false;
			}
		}
		#endregion

		#region 事件
		/// <summary>
		/// 创建子控件
		/// </summary>
		/// <param name="dataSource"></param>
		/// <param name="dataBinding"></param>
		/// <returns></returns>
		/// <remarks>
		/// 创建子控件
		/// </remarks>
		protected override int CreateChildControls(IEnumerable dataSource, bool dataBinding)
		{
			if (!this.RenderMode.OnlyRenderSelf && this.RenderMode.IsHtmlRender)
			{
				if (this.ShowExportControl)
					this.PagerSettings.Position = PagerPosition.TopAndBottom;

				//是否加checkbox。
				if (this.ShowCheckBoxes)
				{
					//客户端记录
					this.CheckedAll = true;
				}

				this.topPagerCreating =
					this.PagerSettings.Position == PagerPosition.Top || this.PagerSettings.Position == PagerPosition.TopAndBottom;
			}

			return base.CreateChildControls(dataSource, dataBinding);
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="postDataKey"></param>
		/// <param name="postCollection"></param>
		/// <returns></returns>
		protected override bool LoadPostData(string postDataKey, NameValueCollection postCollection)
		{
			string targetID = postCollection["__EVENTTARGET"];

			if (!string.IsNullOrEmpty(targetID))
			{
				if (targetID == this.btnWordExport.ClientID)
				{
					this.ExportData_Click(this.btnWordExport, new EventArgs());
				}
				else
					if (targetID == this.btnExcelExport.ClientID)
					{
						this.ExportData_Click(this.btnExcelExport, new EventArgs());
					}
			}

			return base.LoadPostData(postDataKey, postCollection);
		}
		/// <summary>
		/// 创建子控件
		/// </summary>
		/// <remarks>
		/// 创建子控件
		/// </remarks>
		protected override void CreateChildControls()
		{
			this.cPanelContent = new Panel();

			if (this.GridPagerTemplate != null)
				this.GridPagerTemplate.InstantiateIn(this.cPanelContent);

			this.regions = new List<IAttributeAccessor>();
			this.regions.Add(this.cPanelContent);

			base.CreateChildControls();
		}

		/// <summary>
		/// InitializePager
		/// </summary>
		/// <param name="row"></param>
		/// <param name="columnSpan"></param>
		/// <param name="pagedDataSource"></param>
		/// <remarks>
		/// InitializePager
		/// </remarks>
		protected override void InitializePager(GridViewRow row, int columnSpan, PagedDataSource pagedDataSource)
		{
			row.CssClass = "pagertr";
			TableCell tc = new TableCell();

			row.Cells.Add(tc);

			if (columnSpan > 1)
				tc.ColumnSpan = columnSpan;

			CreatePagerRow(tc, pagedDataSource);
			//tc.CreateSubItems("div", container =>
			//{
			//    ((HtmlGenericControl)container).Attributes["class"] = "table-responsive";

			//    container.CreateSubItems("div", wrapper =>
			//        {
			//            ((HtmlGenericControl)wrapper).Attributes["class"] = "dataTables_wrapper";
			//            CreatePagerRow(wrapper, pagedDataSource);
			//        });
			//});


			//Table childTable = new Table();
			//TableRow childRow = new TableRow();
			//childTable.Width = Unit.Percentage(100);

			//if (this.topPagerCreating)
			//    this.DrawingShowTitle(childRow, true);
			//else
			//    this.DrawingShowTitle(childRow, false);

			//TableCell pagerCell = new TableCell();

			//DeluxePager pager = new DeluxePager();

			//pager.GotoButtonText = this.GotoButtonText;
			//pager.PageCodeShowMode = this.PageCodeShowMode;
			//pager.PagerSettings = new PagerSettings(this.PagerSettings);
			//pager.PageSize = this.PageSize;
			//pager.PageCount = this.PageCount;

			//pager.PageIndex = this.PageIndex;
			//pager.RecordCount = pagedDataSource.DataSourceCount;
			//pager.DataBoundControlID = this.ID;
			//pager.IsPagedControl = true;
			//pager.IsDataSourceControl = this.IsDataSourceControl;
			//pager.Category = this.Category;

			//pagerCell.Controls.Add(pager);

			//childRow.Cells.Add(pagerCell);
			//childRow.Style.Add(HtmlTextWriterStyle.TextAlign, "right"); //右对齐 add new by longmark 08-04-14

			////画导出控件
			//if (this.ShowExportControl)
			//    this.DrawingExportControl(childRow, this.topPagerCreating);

			//childTable.Rows.Add(childRow);
			//tc.Controls.Add(childTable);
			//row.Cells.Add(tc);

			this.topPagerCreating = false;
		}

		/// <summary>
		/// 创建Pager行
		/// </summary>
		/// <param name="container"></param>
		/// <returns></returns>
		private Control CreatePagerRow(Control container, PagedDataSource pagedDataSource)
		{
			HtmlGenericControl row = new HtmlGenericControl("div");
			container.Controls.Add(row);

			row.Attributes["class"] = "row";

			HtmlGenericControl pagerContainer = new HtmlGenericControl("div");
			row.Controls.Add(pagerContainer);

			CreatePager(pagerContainer, pagedDataSource);

			pagerContainer.Attributes["class"] = "col-xs-11 grid-pager-home";

			HtmlGenericControl right = new HtmlGenericControl("div");
            right.Attributes["class"] = "grid-pager-cell";
			row.Controls.Add(right);

			right.Attributes["class"] = "col-xs-1 hidden-xs grid-exporter";

			if (this.ShowExportControl)
				CreateExportControls(right, this.topPagerCreating);

			return row;
		}

		private DeluxePager CreatePager(Control container, PagedDataSource pagedDataSource)
		{
			DeluxePager pager = new DeluxePager();

			pager.GotoButtonText = this.GotoButtonText;
			pager.PageCodeShowMode = this.PageCodeShowMode;
			pager.PagerSettings = new PagerSettings(this.PagerSettings);
			pager.PageSize = this.PageSize;
			pager.PageCount = this.PageCount;

			pager.PageIndex = this.PageIndex;
			pager.RecordCount = pagedDataSource.DataSourceCount;
			pager.DataBoundControlID = this.ID;
			pager.IsPagedControl = true;
			pager.IsDataSourceControl = this.IsDataSourceControl;
			pager.Category = this.Category;

			container.Controls.Add(pager);

			return pager;
		}

		private void CreateExportControls(Control container, bool isTop)
		{
			this.clientExportExcelTopButton.Src = this.ExcelImgUrl;
			this.clientExportWordTopButton.Src = this.WordImgUrl;
			this.clientExportExcelButton.Src = this.ExcelImgUrl;
			this.clientExportWordButton.Src = this.WordImgUrl;

			this.btnWordExport.Click += new EventHandler(this.ExportData_Click);
			this.btnExcelExport.Click += new EventHandler(this.ExportData_Click);

			HtmlGenericControl filter = new HtmlGenericControl("div");

			container.Controls.Add(filter);
			filter.Attributes["class"] = "dataTables-filter";

			HtmlGenericControl ul = new HtmlGenericControl("ul");
			filter.Controls.Add(ul);

			//客户端button
			if (isTop)
			{
				CreateListButtonItem(ul, this.clientExportExcelTopButton);
				CreateListButtonItem(ul, this.clientExportWordTopButton);
			}
			else
			{
				CreateListButtonItem(ul, this.clientExportExcelButton);
				CreateListButtonItem(ul, this.clientExportWordButton);

				container.CreateSubItems("div", hidden =>
					{
						hidden.Attributes["class"] = "hidden";
						hidden.Controls.Add(this.btnWordExport);
						hidden.Controls.Add(this.btnExcelExport);
					});
			}
		}

		/// <summary>
		/// 创建
		/// </summary>
		/// <param name="ul"></param>
		/// <param name="innerButton"></param>
		private static void CreateListButtonItem(Control ul, Control innerButton)
		{
			ul.CreateSubItems("li", li =>
					li.CreateSubItems("a", a =>
					{
						//a.Attributes["class"] = "btn";
						a.Controls.Add(innerButton);
					})
				);
		}

		/// <summary>
		/// 设置模式
		/// </summary>
		/// <remarks>
		/// 设置模式
		/// </remarks>
		private void SetStateByRenderMode()
		{
			this.AllowSorting = false;
			this.AllowPaging = false;
			this.PagerSettings.Visible = false;
			this.EnableViewState = false;
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="e"></param>
		protected override void OnPageIndexChanged(EventArgs e)
		{
			base.OnPageIndexChanged(e);

			if (this.CascadeControlID.IsNotEmpty())
			{
				Control cascadeControl = (Control)this.Page.FindControlByID(this.CascadeControlID, true);

				if (cascadeControl != null && cascadeControl is ICascadePagedControl)
					((ICascadePagedControl)cascadeControl).SetPageIndex(this, this.PageIndex);
			}
		}

		protected override void PrepareControlHierarchy()
		{
			base.PrepareControlHierarchy();
			// 处理排序列头的样式问题
			if (this.Controls.Count > 0)
			{
				Table table = this.Controls[0] as Table;
				if (table != null)
				{
					var rows = table.Rows;
					foreach (GridViewRow r in rows)
					{
						if (r.RowType == DataControlRowType.Header)
						{
							if (this.ShowHeader)
							{
								foreach (TableCell cell in r.Cells)
								{
									DataControlFieldCell cell2 = cell as DataControlFieldCell;
									if (cell2 != null)
									{
										DataControlField containingField = cell2.ContainingField;
										if (containingField != null)
										{
											ChangeSortingStyle(cell, containingField);
										}
									}
								}
							}

							break;
						}
					}
				}
			}
		}

		private void ChangeSortingStyle(TableCell cell, DataControlField field)
		{
			if (string.IsNullOrEmpty(field.SortExpression))
			{
				cell.CssClass += " sorting-disabled";
			}
			else
			{
				if (string.Equals(field.SortExpression, this.SortExpression, StringComparison.OrdinalIgnoreCase))
				{
					if (this.SortDirection == SortDirection.Ascending)
					{
						cell.CssClass += " sorting-asc";
					}
					else
					{
						cell.CssClass += " sorting-desc";
					}
				}
			}
		}


		#region OnRowDataBound
		/// <summary>
		/// 
		/// </summary>
		/// <param name="e"></param>
		protected override void OnRowDataBound(GridViewRowEventArgs e)
		{
			switch (e.Row.RowType)
			{
				case DataControlRowType.Header:
					{
						e.Row.CssClass = "headertr";

						for (int i = 0; i < e.Row.Cells.Count; i++)
						{
							TableCell cell = e.Row.Cells[i];

							if (cell.Controls.Count == 0)
							{
								cell.Text = Translator.Translate(Category, cell.Text);
							}
							else
							{
								if (cell.Controls[0] is IButtonControl)
								{
									IButtonControl button = (IButtonControl)cell.Controls[0];
									button.Text = Translator.Translate(Category, button.Text);
								}
							}
						}

						if (this.ShowCheckBoxes)
						{
							if (this.CheckBoxPosition == RowPosition.Left)
								e.Row.Cells[0].CssClass = CheckBoxTemplateHeaderStyle.CssClass;
							else
								e.Row.Cells[e.Row.Cells.Count - 1].CssClass = CheckBoxTemplateHeaderStyle.CssClass;
						}
					}
					break;
				case DataControlRowType.EmptyDataRow:
					{
						if (e.Row.Cells.Count > 0)
						{
							string originalText = e.Row.Cells[0].Text;
							string targetText = originalText.Trim('\t', '\r', '\n');

							e.Row.Cells[0].Text = HttpUtility.HtmlEncode(Translator.Translate(this.Category, targetText));
						}
					}
					break;
			}

			if (this.ShowCheckBoxes)
			{
				if (e.Row.RowType == DataControlRowType.Header)
					CheckAllID = e.Row.ClientID + "_checkall";

				if (e.Row.RowType == DataControlRowType.DataRow)
				{
					string itemsValue = "";

					InputButton chk = ((InputButton)e.Row.FindControl("checkitem"));

					if (e.Row.DataItem.GetType().FullName == "System.Data.DataRowView")
					{
						if (chk != null)
						{
							itemsValue = ((System.Data.DataRowView)(e.Row.DataItem)).Row[DataKeyNames[0]].ToString();
							chk.Value = itemsValue;

							DeluxeGridCheckBoxEventArgs args = new DeluxeGridCheckBoxEventArgs();
							args.CheckBoxValue = itemsValue;
							args.RowEventArgs = e;
							DeluxeGrid_SetCheckBoxStatusHandler(e.Row.DataItem, args);
							chk.Visible = args.CheckBoxVisible;
						}
					}
					else
					{
						if (chk != null)
						{
							itemsValue = e.Row.DataItem.GetType().GetProperty(DataKeyNames[0]).GetGetMethod().Invoke(e.Row.DataItem, new Object[] { }).ToString();
							chk.Value = itemsValue;
							DeluxeGridCheckBoxEventArgs args = new DeluxeGridCheckBoxEventArgs();
							args.CheckBoxValue = itemsValue;
							args.RowEventArgs = e;
							DeluxeGrid_SetCheckBoxStatusHandler(e.Row.DataItem, args);
							chk.Visible = args.CheckBoxVisible;
						}
					}

					foreach (string keys in SelectedKeys)
					{
						if (itemsValue == keys && chk != null)
						{
							chk.Checked = true;

							this.selectedCount++;
						}
					}
				}
			}

			if (e.Row.RowType == DataControlRowType.DataRow)
				if (this.ExportingDeluxeGrid)
					for (int i = 0; i < e.Row.Cells.Count; i++)
					{
						if (this.ShowCheckBoxes && i == 0 && this.CheckBoxPosition == RowPosition.Left)
						{
							//e.Row.Cells[i].CssClass = "col-chekbox";
						}
						else if (this.ShowCheckBoxes && i == e.Row.Cells.Count - 1 && this.CheckBoxPosition == RowPosition.Right)
						{
							//e.Row.Cells[i].CssClass = "col-chekbox";
						}
						else
						{
							//e.Row.Cells[i].Style.Add("word-break", "break-all");
						}
					}
				else
					for (int i = 0; i < e.Row.Cells.Count; i++)
					{
						if (this.ShowCheckBoxes && i == 0 && this.CheckBoxPosition == RowPosition.Left)
						{
							//e.Row.Cells[i].CssClass = "col-chekbox";
						}
						else if (this.ShowCheckBoxes && i == e.Row.Cells.Count - 1 && this.CheckBoxPosition == RowPosition.Right)
						{
							//e.Row.Cells[i].CssClass = "col-chekbox";
						}
						else
						{
							//e.Row.Cells[i].Style.Add("word-wrap", "break-word");
						}
					}
			base.OnRowDataBound(e);
		}
		#endregion

		/// <summary>
		/// 客户端保存到服务器端
		/// </summary>
		/// <param name="clientState"></param>
		/// <remarks>
		/// 客户端保存到服务器端
		/// </remarks>
		protected override void LoadClientState(string clientState)
		{
			this.selectedKeys = JSONSerializerExecute.Deserialize<List<string>>(clientState);
			/*
			StringBuilder strB = new StringBuilder();

			foreach (string s in SelectedKeys)
			{
				if (strB.Length > 0)
					strB.Append(",");

				strB.Append(s);
			}

			this.SelectedKeysValue = strB.ToString();
			*/
		}

		/// <summary>
		/// 服务器端保存内容到客户端
		/// </summary>
		/// <returns></returns>
		/// <remarks>
		/// 服务器端保存内容到客户端
		/// </remarks>
		protected override string SaveClientState()
		{
			var result = new
			{
				selectedKeys = SelectedKeys,
				columnsInfo = GetClientColumnsInfo()
			};

			return JSONSerializerExecute.Serialize(result);
		}

		/// <summary>
		/// OnPreRender
		/// </summary>
		/// <param name="e"></param>
		/// <remarks>
		/// OnPreRender
		/// </remarks>
		protected override void OnPreRender(EventArgs e)
		{
			base.OnPreRender(e);

			//this.ReadOnly = this.Rows.Count == 0;

			//强制单页也显示Pager部分内容
			if (this.PagerSettings.Visible)
			{
				if (this.TopPagerRow != null && !this.TopPagerRow.Visible)
					this.TopPagerRow.Visible = true;
				if (this.BottomPagerRow != null && !this.BottomPagerRow.Visible)
					this.BottomPagerRow.Visible = true;
			}

			//打印的时候如果有checkbox则干掉它。
			if (this.ExportingDeluxeGrid && this.ShowCheckBoxes)
			{
				if (this.CheckBoxPosition == RowPosition.Left)
					this.Columns[0].Visible = false;
				else
					this.Columns[this.Columns.Count - 1].Visible = false;
			}
			else if (this.ExportingDeluxeGrid)
			{
				this.Columns[0].Visible = !this.Columns[0].Visible;
				this.Columns[0].Visible = !this.Columns[0].Visible;
			}
		}

		/// <summary>
		/// 页面加载前先绑定checkbox模版列
		/// </summary>
		/// <param name="e"></param>
		/// <remarks>
		/// 页面加载前先绑定checkbox模版列
		/// </remarks>
		protected override void OnInit(EventArgs e)
		{
			if (DesignMode == false)
			{
				if (Page.Request.UserAgent.IndexOf("Microsoft Office") >= 0)
					Page.Response.End();
			}

			base.OnInit(e);

			//增加checkbox模版列
			if (this.ShowCheckBoxes && !this.DesignMode)
				this.OnPagePreLoadCheckBox();
		}


		/// <summary>
		/// OnPagePreLoad
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		/// <remarks>
		/// OnPagePreLoad
		/// </remarks>
		protected override void OnPagePreLoad(object sender, EventArgs e)
		{
			EnsureChildControls();
			base.OnPagePreLoad(sender, e);
		}

		/// <summary>
		/// render
		/// </summary>
		/// <param name="writer"></param>
		/// <remarks>
		/// render
		/// </remarks>
		protected override void Render(HtmlTextWriter writer)
		{
			if (this.ShowCheckBoxes && !this.DesignMode)
				this.AddPageCheckBoxTemplateStyle();

			if (this.DesignMode)
			{
				if (this.cPanelContent != null)
					this.cPanelContent.RenderControl(writer);
			}
			else
			{
				this.CssClass = this.CssClass.IsNotEmpty() ? this.CssClass : "table table-striped table-bordered table-hover dataTable";

				if (this.Rows.Count == 0)
				{
					writer.WriteBeginTag("table");
					writer.WriteAttribute("id", this.ClientID);
					writer.WriteAttribute("style", "display:none");
					writer.Write(">");
					writer.WriteEndTag("table");
				}
				else
				{
					WriteGridTitle(writer);
				}
			}

			writer.WriteBeginTag("div");
			writer.WriteAttribute("class", "table-responsive");
			writer.Write(">");

			writer.WriteBeginTag("div");
			writer.WriteAttribute("class", "dataTables_wrapper");
			writer.WriteAttribute("role", "grid");
			writer.Write(">");

			base.Render(writer);

			writer.WriteEndTag("div");
			writer.WriteEndTag("div");
		}

		private void WriteGridTitle(HtmlTextWriter writer)
		{
			if (this.GridTitle.IsNotEmpty())
			{
				HtmlGenericControl divHeader = new HtmlGenericControl("div");

				divHeader.Attributes["class"] = "table-header";

				HtmlGenericControl divTitle = new HtmlGenericControl("div");

				divTitle.Attributes["class"] = "dataTables-title";
				divHeader.Controls.Add(divTitle);

				divHeader.CreateSubItems("span", span => span.InnerText = Translator.Translate(this.Category, this.GridTitle));

				writer.Write(divHeader.GetHtml());
			}
		}

		/// <summary>
		/// 增加checkbox样式
		/// </summary>		 
		/// <summary>
		/// 增加checkbox样式
		/// </summary> 
		private void AddPageCheckBoxTemplateStyle()
		{
			this.tempCheckBox.HeaderStyle.CopyFrom(CheckBoxTemplateHeaderStyle);
			this.tempCheckBox.ItemStyle.CopyFrom(CheckBoxTemplateItemStyle);
		}

		/// <summary>
		/// 加载checkbox列
		/// </summary>
		/// <remarks>
		/// 加载checkbox列
		/// </remarks>
		private void OnPagePreLoadCheckBox()
		{
			ExceptionHelper.FalseThrow(this.DataKeyNames.Length == 1, "增加CheckBox列时，DataKeys必须确定一个键值");

			this.DrawCheckAll(this.tempCheckBox);

			if (CheckBoxPosition == RowPosition.Left)
			{
				//this.Columns.RemoveAt(0);
				this.Columns.Insert(0, this.tempCheckBox);
			}
			else
			{
				//this.Columns.RemoveAt(this.Columns.Count-1);
				this.Columns.Add(this.tempCheckBox);
			}
		}

		/// <summary>
		/// 画checkbox全选按钮
		/// </summary>
		/// <param name="temp"></param>
		/// <remarks>
		/// 画checkbox全选按钮
		/// </remarks>
		private void DrawCheckAll(TemplateField temp)
		{
			temp.HeaderTemplate = new CheckBoxTemplate(DataControlRowType.Header, CheckItemName, MultiSelect);
			temp.ItemTemplate = new CheckBoxTemplate(DataControlRowType.DataRow, CheckItemName, MultiSelect);
		}

		private PrivateColumnInfo[] GetClientColumnsInfo()
		{
			List<PrivateColumnInfo> visibleFields = new List<PrivateColumnInfo>();

			for (var i = 0; i < this.Columns.Count; i++)
			{
				DataControlField field = this.Columns[i];

				if (field.Visible && (field is BoundField))
					visibleFields.Add(GetClientFieldInfo(i, (BoundField)field));
			}

			return visibleFields.ToArray();
		}

		private PrivateColumnInfo GetClientFieldInfo(int columnIndex, BoundField field)
		{
			PrivateColumnInfo result = new PrivateColumnInfo
			{
				ColumnIndex = columnIndex,
				DataField = field.DataField
			};

			ExceptionHelper.DoSilentAction(() => result.DataField = field.DataFormatString);
			ExceptionHelper.DoSilentAction(() => result.NullDisplayText = field.NullDisplayText);

			return result;
		}

		#endregion

		#region 公有事件
		/// <summary>
		/// 导出Click事件
		/// </summary>
		/// <remarks>
		/// 
		/// </remarks>
		[Category("Behavior")]
		public event EventHandler ExportData;

		private void ExportData_Click(object sender, System.EventArgs e)
		{
			this.ExportingDeluxeGrid = true;
			if (ExportData != null)
				ExportData(sender, e);

			SetStateByRenderMode();
		}

		/// <summary>
		/// 设置checkbox状态句柄
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="args"></param> 
		public delegate void SetCheckBoxStatusEventHandler(object sender, DeluxeGridCheckBoxEventArgs args);
		/// <summary>
		/// 设置checkbox状态句柄
		/// </summary>
		public event SetCheckBoxStatusEventHandler SetCheckBoxStatusHandler;

		/// <summary>
		/// 设置checkbox状态句柄
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="args"></param>
		public void DeluxeGrid_SetCheckBoxStatusHandler(object sender, DeluxeGridCheckBoxEventArgs args)
		{
			if (SetCheckBoxStatusHandler != null)
				SetCheckBoxStatusHandler(sender, args);
		}
		#endregion

		#region Private Columns Info
		private class PrivateColumnInfo
		{
			public int ColumnIndex { get; set; }
			public string DataField { get; set; }
			public string DataFormatString { get; set; }
			public string NullDisplayText { get; set; }
		}
		#endregion Private Columns Info

		#region ICascadePagedControl Members

		/// <summary>
		/// 设置分页索引
		/// </summary>
		/// <param name="source"></param>
		/// <param name="pageIndex"></param>
		public void SetPageIndex(object source, int pageIndex)
		{
			this.PageIndex = pageIndex;
		}

		#endregion
	}
}
