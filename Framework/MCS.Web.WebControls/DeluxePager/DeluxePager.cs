using System;
using System.IO;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Text;
using System.ComponentModel;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Reflection;
using System.Text.RegularExpressions;
using System.Data;
using System.Data.Sql;
using System.Web.UI.HtmlControls;

using MCS.Web.Library;
using MCS.Web.Library.Script;
using System.Globalization;
using MCS.Library.Globalization;
using MCS.Library.Core;

namespace MCS.Web.WebControls
{
	#region 枚举类
	/// <summary>
	/// 翻页控件支持的控件类型
	/// </summary>
	/// <remarks>
	///  翻页控件支持的控件类型
	/// </remarks>
	public enum DataListControlType
	{
		/// <summary>
		/// Nothing控件
		/// </summary>
		/// <remarks>
		///  Nothing控件
		/// </remarks>
		Nothing,
		/// <summary>
		/// GridView控件
		/// </summary>
		/// <remarks>
		///  GridView控件
		/// </remarks>
		GridView,
		/// <summary>
		/// Table控件
		/// </summary>
		/// <remarks>
		///  Table控件
		/// </remarks>
		Table,
		/// <summary>
		/// DataGrid控件
		/// </summary>
		/// <remarks>
		///  DataGrid控件
		/// </remarks>
		DataGrid,
		/// <summary>
		/// DataList控件
		/// </summary>
		/// <remarks>
		///  DataList控件
		/// </remarks>
		DataList,
		/// <summary>
		/// DeluxeGrid控件
		/// </summary>
		/// <remarks>
		///  DeluxeGrid控件
		/// </remarks>
		DeluxeGrid,
		/// <summary>
		/// DetailsView控件
		/// </summary>
		/// <remarks>
		///  DetailsView控件
		/// </remarks>
		DetailsView,
		/// <summary>
		/// FormView控件
		/// </summary>
		/// <remarks>
		///  FormView控件
		/// </remarks>
		FormView,
		/// <summary>
		/// Repeater控件
		/// </summary>
		/// <remarks>
		/// Repeater控件
		/// </remarks>
		Repeater
	}
	#endregion
	/// <summary>
	/// DeluxePager控件 继承子ScriptControlBase
	/// </summary>    
	/// <remarks>
	///  DeluxePager控件 继承子ScriptControlBase
	/// </remarks>
	[DefaultProperty("DeluxePager"),
	ToolboxData("<{0}:DeluxePager runat=server Width=\"500\"></{0}:DeluxePager>")]
	[Designer(typeof(DeluxePagerDesigner))]
	[ParseChildren(true),
	PersistChildren(false)]
	public class DeluxePager : WebControl, IPostBackEventHandler, IPostBackContainer, INamingContainer, ICascadePagedControl //ScriptControlBase
	{
		/// <summary>
		/// 构造函数
		/// </summary>
		/// <remarks>
		///  构造函数
		/// </remarks>
		public DeluxePager()
		{
			txtPageCount = new HtmlInputHidden();
			txtPageCount.ID = "txtPageCount";
		}
		/// <summary>
		/// 页码显示隐藏控件
		/// </summary>
		/// <remarks>
		///  页码显示隐藏控件
		/// </remarks>
		protected HtmlInputHidden txtPageCount;

		#region Private
		/// <summary>
		/// 绑定控件ID
		/// </summary>
		/// <remarks>
		///  绑定控件ID
		/// </remarks>
		private string dataBoundControlID = string.Empty;
		/// <summary>
		/// 用作数据绑定控件基类
		/// </summary>
		/// <remarks>
		///  用作数据绑定控件基类
		/// </remarks>
		private BaseDataBoundControl boundControl = null;
		/// <summary>
		/// 控件
		/// </summary>
		/// <remarks>
		///  控件
		/// </remarks>
		private Control control = null;

		private TextBox txtPageCode = new TextBox();

		private class PageInfo
		{
			/// <summary>
			/// 页码总数
			/// </summary>
			public int PageCount;

			/// <summary>
			/// 当前页码
			/// </summary>
			public int CurrentPage;

			/// <summary>
			/// 记录总数
			/// </summary>
			public int RecordCount;

			/// <summary>
			/// 页码大小
			/// </summary>
			public int PageSize;
		}

		PageInfo _PageInfo = new PageInfo();

		/*
		/// <summary>
		/// 页码总数
		/// </summary>  
		/// <remarks>
		///  页码总数
		/// </remarks>
		private Label pageCount = new Label();
		/// <summary>
		/// 当前页码
		/// </summary>
		/// <remarks>
		///  当前页码
		/// </remarks>
		private Label currentPage = new Label();
		/// <summary>
		/// 记录总数
		/// </summary>
		/// <remarks>
		///  记录总数
		/// </remarks>
		private Label recordCount = new Label();
		/// <summary>
		/// 页码大小
		/// </summary>
		/// <remarks>
		///  页码大小
		/// </remarks>
		private Label pageSize = new Label();
		*/

		/// <summary>
		/// 页码部分
		/// </summary>
		/// <remarks>
		///  页码部分
		/// </remarks>
		TableCell cellPageNumber = new TableCell();
		/// <summary>
		/// 翻页部分
		/// </summary>
		/// <remarks>
		///  翻页部分
		/// </remarks>
		TableCell cellPager = new TableCell();
		/// <summary>
		/// 跳转页部分
		/// </summary>
		/// <remarks>
		///  跳转页部分
		/// </remarks>
		TableCell cellGotoPage = new TableCell();
		/// <summary>
		/// 跳转页按钮部分
		/// </summary>
		TableCell cellGotoButton = new TableCell();
		/// <summary>
		/// table in html
		/// </summary>
		/// <remarks>
		///  table in html
		/// </remarks>
		Table table = new Table();
		/// <summary>
		/// tr in html
		/// </summary>
		/// <remarks>
		///  tr in html
		/// </remarks>
		TableRow tRow = new TableRow();
		/// <summary>
		/// DataGrid数据绑定事件计数器
		/// </summary>
		/// <remarks>
		///  DataGrid数据绑定事件计数器
		/// </remarks>
		int itemDataBoundCount = 0;
		#endregion

		#region 私有属性
		/// <summary>
		/// 第一个现实的页码数
		/// </summary>
		/// <remarks>
		///  第一个现实的页码数
		/// </remarks>
		private int FirstDisplayedPageIndex
		{
			get
			{
				return WebControlUtility.GetViewStateValue<int>(ViewState, "FirstDisplayedPageIndex", -1);
			}
			set
			{
				WebControlUtility.SetViewStateValue<int>(ViewState, "FirstDisplayedPageIndex", value);
			}
		}


		/// <summary>
		/// 是否显示跳转页,默认true
		/// </summary>
		/// <remarks>
		///  是否显示跳转页,默认true
		/// </remarks>
		private bool GotoPageShow
		{
			get
			{
				return WebControlUtility.GetViewStateValue<bool>(ViewState, "GotoPageShow", true);
			}
			set
			{
				WebControlUtility.SetViewStateValue<bool>(ViewState, "GotoPageShow", value);
			}
		}
		#endregion

		#region 内部属性

		/// <summary>
		/// 当前页码序号
		/// </summary>
		/// <remarks>
		///  当前页码序号
		/// </remarks>
		public int PageIndex
		{
			get
			{
				return WebControlUtility.GetViewStateValue<int>(ViewState, "PageIndex", 0);
			}
			set
			{
				WebControlUtility.SetViewStateValue<int>(ViewState, "PageIndex", value);
			}
		}

		/// <summary>
		/// 页码总数
		/// </summary>
		/// <remarks>
		///  页码总数
		/// </remarks>
		public int PageCount
		{
			get
			{
				return WebControlUtility.GetViewStateValue<int>(ViewState, "PageCount", 0);
			}
			set
			{
				WebControlUtility.SetViewStateValue<int>(ViewState, "PageCount", value);
			}
		}

		/// <summary>
		/// 单页记录数量
		/// </summary>
		/// <remarks>
		///  单页记录数量
		/// </remarks>
		public int PageSize
		{
			get
			{
				return WebControlUtility.GetViewStateValue<int>(ViewState, "PageSize", 0);
			}
			set
			{
				WebControlUtility.SetViewStateValue<int>(ViewState, "PageSize", value);
			}
		}


		#endregion

		#region  Properties 属性
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
				return WebControlUtility.GetViewStateValue(ViewState, "Category", string.Empty);
			}
			set
			{
				WebControlUtility.SetViewStateValue(ViewState, "Category", value);
			}
		}

		/// <summary>
		/// 下级控件ID。必须实现了ICascadePagedControl接口
		/// </summary>
		[IDReferenceProperty(typeof(ICascadePagedControl))]
		public string CascadeControlID
		{
			get
			{
				return WebControlUtility.GetViewStateValue(ViewState, "CascadeControlID", string.Empty);
			}
			set
			{
				WebControlUtility.SetViewStateValue(ViewState, "CascadeControlID", value);
			}
		}

		/// <summary>
		/// 数据绑定控件ID
		/// </summary>
		/// <remarks>
		///  数据绑定控件ID
		/// </remarks>
		[IDReferenceProperty(typeof(BaseDataBoundControl))]
		public string DataBoundControlID
		{
			get
			{
				return WebControlUtility.GetViewStateValue<string>(ViewState, "DataBoundControlID", string.Empty);
			}
			set
			{
				WebControlUtility.SetViewStateValue<string>(ViewState, "DataBoundControlID", value);
			}
		}

		/// <summary>
		/// 翻页属性设置
		/// </summary>
		/// <remarks>
		///  翻页属性设置
		/// </remarks>
		[Category("Paging"), Description("DeluxePager_PagerSettings"), DesignerSerializationVisibility(DesignerSerializationVisibility.Content), NotifyParentProperty(true), PersistenceMode(PersistenceMode.InnerProperty)]
		public PagerSettings PagerSettings
		{
			get
			{
				PagerSettings pagerSettings = WebControlUtility.GetViewStateValue<PagerSettings>(ViewState, "PagerSettings", null);
				if (pagerSettings == null)
				{
					pagerSettings = new PagerSettings();
					WebControlUtility.SetViewStateValue<PagerSettings>(ViewState, "PagerSettings", pagerSettings);
				}
				return pagerSettings;
				//object o = ViewState["PagerSettings"];
				//return o != null ? (PagerSettings)o : new PagerSettings(); 

			}
			set
			{
				ViewState["PagerSettings"] = value;
			}

		}
		/// <summary>
		/// 设置跳转页按钮Text
		/// </summary>
		/// <remarks>
		///  设置跳转页按钮Text
		/// </remarks>
		[Browsable(true),
		Description("设置跳转页按钮的Text"),
		DefaultValue("跳转到"),
		Category("扩展")]
		public string GotoButtonText
		{
			get
			{
				return WebControlUtility.GetViewStateValue<string>(ViewState,
					"GotoButtonText",
					Translator.Translate(Define.DefaultCategory, "跳转到"));
			}
			set
			{
				WebControlUtility.SetViewStateValue<string>(ViewState, "GotoButtonText", value);
			}
		}

		/// <summary>
		/// 页码显示模式
		/// </summary>
		/// <remarks>
		///  页码显示模式
		/// </remarks>
		[Browsable(true)]
		[Description("页码显示模式")]
		[DefaultValue(PagerCodeShowMode.All)]
		[Category("扩展")]
		public PagerCodeShowMode PageCodeShowMode
		{
			get
			{
				return WebControlUtility.GetViewStateValue<PagerCodeShowMode>(ViewState, "PageCodeShowMode", PagerCodeShowMode.All);
			}
			set
			{
				WebControlUtility.SetViewStateValue<PagerCodeShowMode>(ViewState, "PageCodeShowMode", value);
			}
		}

		/// <summary>
		/// 获取记录总数
		/// </summary>
		/// <remarks>
		///  获取记录总数
		/// </remarks>
		[Browsable(false)]
		[Description("数据记录总数"),
		DefaultValue("0")]
		public int? RecordCount
		{
			get
			{
				return WebControlUtility.GetViewStateValue<int?>(ViewState, "RecordCount", 0);
			}
			set
			{
				WebControlUtility.SetViewStateValue<int?>(ViewState, "RecordCount", value);
			}
		}

		/// <summary>
		/// 是否为IDataSouce类型的数据源 默认true
		/// </summary>
		/// <remarks>
		///  是否为IDataSouce类型的数据源 默认true
		/// </remarks>
		[Description("是否为IDataSouce类型的数据源"),
		DefaultValue(true),
		Category("扩展")]
		public bool IsDataSourceControl
		{
			get
			{
				return WebControlUtility.GetViewStateValue<bool>(ViewState, "IsDataSourceControl", true);
			}
			set
			{
				WebControlUtility.SetViewStateValue<bool>(ViewState, "IsDataSourceControl", value);
			}
		}

		/// <summary>
		/// 当前的数据展示控件是否具有翻页功能,默认true
		/// </summary>
		/// <remarks>
		///  当前的数据展示控件是否具有翻页功能,默认true
		/// </remarks>
		[Description("当前的数据展示控件是否具有翻页功能"),
		DefaultValue(true),
		Category("扩展")]
		public bool IsPagedControl
		{
			get
			{
				return WebControlUtility.GetViewStateValue<bool>(ViewState, "IsPagedControl", true);
			}
			set
			{
				WebControlUtility.SetViewStateValue<bool>(ViewState, "IsPagedControl", value);
			}
		}
		private Style tableStyle = new Style();
		/// <summary>
		/// 翻页控件style
		/// </summary>
		/// <remarks>
		///  翻页控件style
		/// </remarks>
		[Description("翻页控件style"),
		Category("扩展")]
		[DesignerSerializationVisibility(DesignerSerializationVisibility.Content), NotifyParentProperty(true), PersistenceMode(PersistenceMode.InnerProperty)]
		public Style TableStyle
		{
			get
			{
				return tableStyle;
			}
			set
			{
				tableStyle = value;
			}
		}

		/// <summary>
		/// 翻页控件跳转页输入框CssClass
		/// </summary>
		/// <remarks>
		/// 翻页控件跳转页输入框CssClass
		/// </remarks>
		[Description("翻页控件跳转页输入框CssClass"),
		DefaultValue(""),
		Category("扩展")]
		public string TxtBoxCssClass
		{
			get
			{
				return WebControlUtility.GetViewStateValue<string>(ViewState, "TxtBoxCssClass", "inputStyle");
			}
			set
			{
				WebControlUtility.SetViewStateValue<string>(ViewState, "TxtBoxCssClass", value);
			}
		}

		/// <summary>
		/// 翻页控件CssClass
		/// </summary>
		/// <remarks>
		/// 翻页控件CssClass
		/// </remarks>
		[Description("翻页控件CssClass"),
		DefaultValue(""),
		Category("扩展")]
		public string TableCssClass
		{
			get
			{
				return WebControlUtility.GetViewStateValue<string>(ViewState, "TableCssClass", string.Empty);
			}
			set
			{
				WebControlUtility.SetViewStateValue<string>(ViewState, "TableCssClass", value);
			}
		}

		/// <summary>
		/// 跳转页控件CssClass
		/// </summary>
		/// <remarks>
		/// 跳转页控件CssClass
		/// </remarks>
		[Description("跳转页控件CssClass"),
		DefaultValue("portalButton"),
		Category("扩展")]
		public string GotoPageButtonCssClass
		{
			get
			{
				return WebControlUtility.GetViewStateValue<string>(ViewState, "GotoPageButtonCssClass", "portalButton");
			}
			set
			{
				WebControlUtility.SetViewStateValue<string>(ViewState, "GotoPageButtonCssClass", value);
			}
		}
		#endregion

		#region  Override Method

		/// <summary>
		/// 创建子控件
		/// </summary>
		/// <remarks>
		///  创建子控件
		/// </remarks>
		private void CreatePagerContent()
		{
			this.table.ID = "pagerTable";
			table.CssClass = TableCssClass;
			this.ApplyStyle(TableStyle);

			this.tRow.ID = "pagerRow";
			//画页码部分
			this.DrawingPageNumber(this.cellPageNumber);
			//初始化paging部分
			InitializePager(this.cellPager);
			this.cellPager.ID = "cellPager";
			//初始化GotoPaging部分
			this.DrawingGotoPaging(this.cellGotoPage, this.cellGotoButton);

			this.tRow.Controls.Add(this.cellPageNumber);
			this.tRow.Controls.Add(this.cellPager);
			this.tRow.Controls.Add(this.cellGotoPage);
			this.tRow.Controls.Add(this.cellGotoButton);

			this.cellPageNumber.ApplyStyle(TableStyle);
			this.cellPager.ApplyStyle(TableStyle);
			this.cellGotoPage.ApplyStyle(TableStyle);


			this.table.Controls.Add(this.tRow);
			this.table.Width = this.Width;
			Controls.Add(this.table);
		}

		/// <summary>
		/// 初始化Pager翻页部分
		/// </summary>
		/// <param name="cell"></param>
		/// <remarks>
		///  初始化Pager翻页部分
		/// </remarks>
		protected void InitializePager(TableCell cell)
		{
			switch (PagerSettings.Mode)
			{
				case DeluxePagerMode.Numeric:
					this.DrawingNumericPager(cell);
					//如果Numeric则跳转页部分不显示
					this.GotoPageShow = false;
					break;

				case DeluxePagerMode.NextPreviousFirstLast:
					this.DrawingPaging(cell);
					break;
			}
			Controls.Add(cell);
		}

		private void Page_PreRenderComplete(Object sender, EventArgs e)
		{
			CreatePagerContent();
		}

		/// <summary>
		/// 初始化控件
		/// </summary>
		/// <param name="e"></param>
		/// <remarks>
		///  初始化控件
		/// </remarks>
		protected override void OnInit(EventArgs e)
		{
			if (!this.DesignMode)
			{
				this.control = (Control)this.FindControlRecursivly(this.Page, DataBoundControlID);

				if (this.control != null)
				{
					//设置绑定对应控件的分页属性
					IPageEventArgs ipea = new PageEventArgs();

					IPagerBoundControlType pagerControlType = new PagerBoundControlType();
					PagerBoundControlStatus pbControlStatus = pagerControlType.GetPagerBoundControl(this.control.GetType());

					ipea.SetBoundControlPagerSetting(this.control, pbControlStatus.DataListControlType, this.PageSize);

					if (pbControlStatus.IsPagedControl && this.IsPagedControl && DataBoundControlID != string.Empty)
					{
						//DataGrid与其它具有翻页功能控件的机制不同，由于继承的基类不同，因此这里排除DataGrid类型
						if (pbControlStatus.DataListControlType != DataListControlType.DataGrid)
							this.BindDataControl();
						else
							this.DataGridDataSource();
					}
				}

				if (this.Page != null)
				{
					this.Page.PreRenderComplete += new EventHandler(Page_PreRenderComplete);
				}
				base.OnInit(e);
			}
		}

		#endregion override

		#region Private Method

		/// <summary>
		/// 上下翻页
		/// </summary>
		/// <param name="pageIndex"></param>
		/// <remarks>
		///  上下翻页
		/// </remarks>
		private void SetPageIndex(int pageIndex)
		{
			if (IsPagedControl)
				this.SetPageIndexToControl(pageIndex);
			else
			{
				PageIndex = pageIndex;
				this.OnCommonPageIndexChanged();
			}

			if (this.CascadeControlID.IsNotEmpty())
			{
				Control cascadeControl = (Control)this.FindControlRecursivly(this.Page, this.CascadeControlID);

				if (cascadeControl != null && cascadeControl is ICascadePagedControl)
					((ICascadePagedControl)cascadeControl).SetPageIndex(this, pageIndex);
			}
		}

		/// <summary>
		/// 跳转页按钮翻页方法
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		/// <remarks>
		///  跳转页按钮翻页方法
		/// </remarks>
		void BtnGoto_Click(object sender, EventArgs e)
		{
			this.PageGoto();
			//this.Page.RegisterClientScriptBlock("", "JudgePageGoto('"+this.this.txtPageCode.Text+"','"+txtPageCount.Value+"')");
			//重画翻页控件
			//this.ReDrawingControls(PageIndex);
		}


		/// <summary>
		/// 画数字翻页部分
		/// </summary>
		/// <param name="tc"></param>
		/// <remarks>
		///  画数字翻页部分
		/// </remarks>
		private void DrawingNumericPager(TableCell tc)
		{
			if (!this.IsDataSourceControl && PageSize != 0)
				this.PageCount = RecordCount % PageSize == 0 ? (int)RecordCount / PageSize : ((int)RecordCount / PageSize) + 1;
			DeluxePagerLinkButton lButton;
			//总页码数
			int pageCount = this.PageCount;
			//当前页码
			int num2 = PageIndex;
			//if (!this.Page.IsPostBack)
			num2 = PageIndex + 1;
			//当前翻页控件的按钮数量 ＝pageSize
			int num4 = PagerSettings.PageButtonCount;
			//当前要现实的第一个页码数
			int num5 = this.FirstDisplayedPageIndex + 1;
			//翻页的按钮数量
			int pageButtonCount = PagerSettings.PageButtonCount;
			//控件序号
			int btnIndex = 1;
			if (pageCount < num4)
			{
				num4 = pageCount;
			}
			//循环序号
			int num6 = 1;
			int pageIndex = num4;
			if (num2 > pageIndex)
			{
				int num8 = PageIndex / pageButtonCount;
				bool flag = ((num2 - num5) >= 0) && ((num2 - num5) < pageButtonCount);
				if ((num5 > 0) && flag)
				{
					num6 = num5;
				}
				else
				{
					num6 = (num8 * pageButtonCount) + 1;
				}
				pageIndex = (num6 + pageButtonCount) - 1;
				if (pageIndex > pageCount)
				{
					pageIndex = pageCount;
				}
				if (((pageIndex - num6) + 1) < pageButtonCount)
				{
					num6 = Math.Max(1, (pageIndex - pageButtonCount) + 1);
				}
				this.FirstDisplayedPageIndex = num6 - 1;
			}

			if ((num2 != 1) && (num6 != 1))
			{
				DeluxePagerLinkButton lbtnFirst = new DeluxePagerLinkButton(this);
				lbtnFirst.ID = "lbtnFirst";
				tc.Controls.Add(lbtnFirst);
				tc.Controls.Add(new LiteralControl("&nbsp;&nbsp;"));
				lbtnFirst.Text = PagerSettings.FirstPageText;
				lbtnFirst.CommandName = "Number";
				lbtnFirst.CommandArgument = "1";
				//lbtnFirst.Click += new EventHandler(First_Click);
			}

			if (num6 != 1)
			{
				DeluxePagerLinkButton lbtnPrev = new DeluxePagerLinkButton(this);
				lbtnPrev.ID = "lbtnPrev";
				tc.Controls.Add(lbtnPrev);
				tc.Controls.Add(new LiteralControl("&nbsp;&nbsp;"));
				lbtnPrev.CommandName = "Number";
				lbtnPrev.Text = "...";
				lbtnPrev.CommandArgument = (num6 - 1).ToString();
				//lbtnPrev.Click += new EventHandler(Prev_Click);
			}

			for (int i = num6; i <= pageIndex; i++)
			{
				string codeText = i.ToString();
				if (i == num2)
				{
					Label label = new Label();
					label.Text = codeText;
					tc.Controls.Add(label);
				}
				else
				{
					lButton = new DeluxePagerLinkButton(this);
					lButton.ID = btnIndex.ToString();
					lButton.Text = codeText;
					lButton.CommandName = "Number";
					lButton.CommandArgument = codeText;
					//lButton.Click += new EventHandler(LButton_Click);
					tc.Controls.Add(lButton);
				}

				tc.Controls.Add(new LiteralControl("&nbsp;&nbsp;"));
				btnIndex++;
			}

			if (pageCount > pageIndex)
			{
				DeluxePagerLinkButton lbtnNext = new DeluxePagerLinkButton(this);
				lbtnNext.ID = "lbtnNext";
				tc.Controls.Add(lbtnNext);
				tc.Controls.Add(new LiteralControl("&nbsp;&nbsp;"));
				lbtnNext.Text = "...";
				lbtnNext.CommandName = "Number";
				lbtnNext.CommandArgument = (pageIndex + 1).ToString();
				//lbtnNext.Click += new EventHandler(Next_Click);
			}

			bool flag2 = pageIndex == pageCount;

			if ((num2 != pageCount) && !flag2)
			{
				DeluxePagerLinkButton lbtnLast = new DeluxePagerLinkButton(this);
				lbtnLast.ID = "lbtnLast";
				tc.Controls.Add(lbtnLast);
				lbtnLast.Text = PagerSettings.LastPageText;
				lbtnLast.CommandName = "Number";
				lbtnLast.CommandArgument = pageCount.ToString();
				//lbtnLast.Click += new EventHandler(Last_Click);
			}
		}

		/// <summary>
		/// 显示“首页 上一页 下一页 尾页”按钮
		/// </summary>
		/// <param name="tc">TableCell</param>
		/// <remarks>
		///  显示“首页 上一页 下一页 尾页”按钮
		/// </remarks>
		private void DrawingPaging(TableCell tc)
		{
			DeluxePagerLinkButton lbtnFirst = new DeluxePagerLinkButton(this);
			DeluxePagerLinkButton lbtnPrev = new DeluxePagerLinkButton(this);
			DeluxePagerLinkButton lbtnNext = new DeluxePagerLinkButton(this);
			DeluxePagerLinkButton lbtnLast = new DeluxePagerLinkButton(this);
			//添加首页，上一页按钮
			if (this.PageIndex <= 0)
			{
				lbtnFirst.Enabled = lbtnPrev.Enabled = false;
			}
			else
			{
				lbtnFirst.Enabled = lbtnPrev.Enabled = true;
			}

			tc.Controls.Add(lbtnFirst);
			tc.Controls.Add(new LiteralControl("&nbsp;&nbsp;"));
			tc.Controls.Add(lbtnPrev);
			tc.Controls.Add(new LiteralControl("&nbsp;&nbsp;"));

			//显示“首页 上一页 下一页 尾页”按钮
			if (!String.IsNullOrEmpty(PagerSettings.FirstPageImageUrl))
			{
				lbtnFirst.Text = "<img src='" + ResolveUrl(PagerSettings.FirstPageImageUrl) + "' border='0'/>";
			}
			else
			{
				lbtnFirst.Text = Translator.Translate(Define.DefaultCategory, PagerSettings.FirstPageText);
			}

			lbtnFirst.CommandName = "First";
			lbtnFirst.CommandArgument = "First";
			lbtnFirst.Font.Underline = false;

			if (!String.IsNullOrEmpty(PagerSettings.PreviousPageImageUrl))
			{
				lbtnPrev.Text = "<img src='" + ResolveUrl(PagerSettings.PreviousPageImageUrl) + "' border='0'/>";
			}
			else
			{
				lbtnPrev.Text = Translator.Translate(Define.DefaultCategory, PagerSettings.PreviousPageText);
			}

			lbtnPrev.CommandName = "Prev";
			lbtnPrev.CommandArgument = "Prev";
			lbtnPrev.Font.Underline = false;

			if (!String.IsNullOrEmpty(PagerSettings.NextPageImageUrl))
			{
				lbtnNext.Text = "<img src='" + ResolveUrl(PagerSettings.NextPageImageUrl) + "' border='0'/>";
			}
			else
			{
				lbtnNext.Text = Translator.Translate(Define.DefaultCategory, PagerSettings.NextPageText);
			}

			lbtnNext.CommandName = "Next";
			lbtnNext.CommandArgument = "Next";
			lbtnNext.Font.Underline = false;

			if (!String.IsNullOrEmpty(PagerSettings.LastPageImageUrl))
			{
				lbtnLast.Text = "<img src='" + ResolveUrl(PagerSettings.LastPageImageUrl) + "' border='0'/>";
			}
			else
			{
				lbtnLast.Text = Translator.Translate(Define.DefaultCategory, PagerSettings.LastPageText);
			}

			lbtnLast.CommandName = "Last";
			lbtnLast.CommandArgument = "Last";
			lbtnLast.Font.Underline = false;

			tc.Controls.Add(lbtnFirst);
			tc.Controls.Add(new LiteralControl("&nbsp;&nbsp;"));
			tc.Controls.Add(lbtnPrev);
			tc.Controls.Add(new LiteralControl("&nbsp;&nbsp;"));
			tc.Controls.Add(lbtnNext);
			tc.Controls.Add(new LiteralControl("&nbsp;&nbsp;"));
			tc.Controls.Add(lbtnLast);
			tc.Controls.Add(new LiteralControl("&nbsp;&nbsp;"));

			//添加下一页，尾页按钮
			if (this.PageIndex >= PageCount - 1)
			{
				lbtnNext.Enabled = lbtnLast.Enabled = false;
			}
			else
			{
				lbtnNext.Enabled = lbtnLast.Enabled = true;
			}

			tc.Controls.Add(lbtnNext);
			tc.Controls.Add(new LiteralControl("&nbsp;&nbsp;"));
			tc.Controls.Add(lbtnLast);
			tc.Controls.Add(new LiteralControl("&nbsp;&nbsp;"));

			//添加首页，上一页按钮
			if (this.PageIndex <= 0)
			{
				lbtnFirst.Enabled = lbtnPrev.Enabled = false;
			}
			else
			{
				lbtnFirst.Enabled = lbtnPrev.Enabled = true;
			}

			//添加下一页，尾页按钮
			if (this.PageIndex >= PageCount - 1)
			{
				lbtnNext.Enabled = lbtnLast.Enabled = false;
			}
			else
			{
				lbtnNext.Enabled = lbtnLast.Enabled = true;
			}
		}

		/// <summary>
		/// 显示跳转页
		/// </summary>
		/// <param name="tc"></param>
		/// <param name="tcb"></param>
		/// <remarks>
		///  显示跳转页
		/// </remarks>
		private void DrawingGotoPaging(TableCell tc, TableCell tcb)
		{
			if (this.GotoPageShow)
			{
				DeluxePagerButton btnGoto = new DeluxePagerButton(this);

				this.txtPageCode.Text = (PageIndex + 1).ToString();
				this.txtPageCode.ID = "txtPageCode";
				btnGoto.PreRender += new EventHandler(btnGoto_PreRender);
				this.txtPageCode.Style.Value = "";
				Unit u = new Unit(20);
				this.txtPageCode.Width = u;

				btnGoto.Text = GotoButtonText;
				btnGoto.CommandName = "Goto";

				btnGoto.CausesValidation = false;

				tc.Controls.Add(this.txtPageCode);
				//tc.Controls.Add(new LiteralControl("&nbsp;"));
				tcb.Controls.Add(btnGoto);
				//tcb.Controls.Add(new LiteralControl("&nbsp;&nbsp;"));
				btnGoto.CssClass = this.GotoPageButtonCssClass;
				this.txtPageCode.CssClass = this.TxtBoxCssClass;

				this.cellGotoPage.Controls.Add(txtPageCount);

				if (this.PageCount == 0 || (this.PageIndex <= this.PageCount && this.PageCount == 1))
				{
					btnGoto.Enabled = false;
					this.txtPageCode.Enabled = false;
				}
				else
				{
					btnGoto.Enabled = true;
					this.txtPageCode.Enabled = true;
				}
			}
		}

		private void btnGoto_PreRender(object sender, EventArgs e)
		{
			StringBuilder sbOnclickScript = new StringBuilder();
			sbOnclickScript.Append(" var bl = true; var inputVal = document.getElementById('" + this.txtPageCode.ClientID + "').value;  var pageCount =document.getElementById('" + this.txtPageCount.ClientID + "').value; if(pageCount.length==0) bl= false;");
			sbOnclickScript.Append("var oneChar;inputStr=inputVal.toString();for (var i=0;i<inputStr.length;i++){oneChar=inputStr.charAt(i);");
			sbOnclickScript.Append("if (oneChar<'0' || oneChar>'9'){ alert('" +
				Translator.Translate(Define.DefaultCategory, "非法字符，请输入整数数字") + "');bl= false;break;}} ");
			sbOnclickScript.Append("if((inputVal-pageCount)>0 || inputVal<1){alert('" +
				Translator.Translate(Define.DefaultCategory, "没有此页码，请输入1－' + pageCount + '之间的数字") +
				"');bl= false;} if(!bl)return false;");

			((DeluxePagerButton)sender).OnClientClick = sbOnclickScript.ToString();
		}

		private void DrawingPageNumber(TableCell tc)
		{
			if (PageSize != 0)
			{
				if ((RecordCount % PageSize) > 0)
					PageCount = ((int)(RecordCount / PageSize)) + 1;
				else
					PageCount = (int)(RecordCount / PageSize);
			}

			//页码控件赋值
			RefreshPageNumberInfo();
			PageInfo pageInfo = this._PageInfo;

			StringBuilder strB = new StringBuilder();

			switch ((PagerCodeShowMode)PageCodeShowMode)
			{
				case PagerCodeShowMode.All:
					if (RecordCount.HasValue)
					{
						strB.Append("&nbsp;&nbsp;");
						strB.Append(Translator.Translate(Define.DefaultCategory, "总记录数{0:#,##0}", pageInfo.RecordCount));
						strB.Append("&nbsp;&nbsp;");
						strB.Append(Translator.Translate(Define.DefaultCategory, "{0:#,##0}/页", pageInfo.PageSize));
						strB.Append("&nbsp;&nbsp;");
					}

					strB.Append(Translator.Translate(Define.DefaultCategory, "第{0:#,##0}页", pageInfo.CurrentPage));
					strB.Append("&nbsp;");
					strB.Append(Translator.Translate(Define.DefaultCategory, "共{0:#,##0}页", pageInfo.PageCount));

					break;
				case PagerCodeShowMode.CurrentRecordCount:
					strB.Append(Translator.Translate(Define.DefaultCategory, "第{0:#,##0}页", pageInfo.CurrentPage));
					strB.Append("/");
					strB.Append(Translator.Translate(Define.DefaultCategory, "共{0:#,##0}页", pageInfo.PageCount));
					strB.Append("&nbsp;&nbsp;&nbsp;&nbsp;");
					break;
				case PagerCodeShowMode.RecordCount:
					if (RecordCount.HasValue)
					{
						strB.Append("&nbsp;&nbsp;");
						strB.Append(Translator.Translate(Define.DefaultCategory, "总记录数{0:#,##0}", pageInfo.RecordCount));
						strB.Append("&nbsp;&nbsp;");
						strB.Append(Translator.Translate(Define.DefaultCategory, "{0:#,##0}/页", pageInfo.PageSize));
					}
					break;
			}

			tc.Controls.Add(new LiteralControl(strB.ToString()));
		}

		/// <summary>
		/// 绑定数据控件
		/// </summary>
		/// <remarks>
		///  绑定数据控件
		/// </remarks>
		private void BindDataControl()
		{
			BaseDataBoundControl boundControl = (BaseDataBoundControl)this.FindControlRecursivly(this.Page, DataBoundControlID);

			if (boundControl != null)
			{
				System.Type type = boundControl.GetType();
				IPagerBoundControlType pagerControlType = new PagerBoundControlType();
				PagerBoundControlStatus pbControlStatus = pagerControlType.GetPagerBoundControl(type);

				if (pbControlStatus.DataListControlType != DataListControlType.DataGrid)
				{
					boundControl.DataBound += new EventHandler(BoundControl_DataBound);
					this.boundControl = boundControl;
				}
			}
		}

		/// <summary>
		/// 获取DataGrid属性对象值
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="propertyName"></param>
		/// <returns></returns>
		/// <remarks>
		///  获取DataGrid属性对象值
		/// </remarks>
		private int GetPagerFromDataGridControl(object sender, string propertyName)
		{
			int result = 0;

			if (sender != null)
			{
				System.Type type = sender.GetType();
				PropertyInfo pi = type.GetProperty(propertyName);

				if (pi != null)
					if (pi.CanRead)
						result = (int)pi.GetValue(sender, null);
			}

			return result;
		}

		/// <summary>
		/// DataGrid数据绑定事件
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		/// <remarks>
		///  DataGrid数据绑定事件
		/// </remarks>
		private void dg_ItemDataBound(object sender, DataGridItemEventArgs e)
		{
			if (this.itemDataBoundCount > 0)
				return;

			PageCount = this.GetPagerFromDataGridControl(sender, "PageCount");

			//ShenZheng
			PageIndex = this.GetPagerFromDataGridControl(sender, "CurrentPageIndex");
			PageSize = this.GetPagerFromDataGridControl(sender, "PageSize");

			if (!this.Page.IsPostBack && PagerSettings.Mode == DeluxePagerMode.Numeric)
			{
				this.cellPager.Controls.Clear();
				this.DrawingNumericPager(this.cellPager);
				this.tRow.Controls.Add(this.cellPager);
			}
			this.RefreshPageNumberInfo();

			this.itemDataBoundCount++;
		}

		/// <summary>
		/// DataGrid绑定数据
		/// </summary> 
		/// <remarks>
		///  DataGrid绑定数据
		/// </remarks>
		private void DataGridDataSource()
		{
			DataGrid dg = (DataGrid)this.FindControlRecursivly(this.Page, DataBoundControlID);
			dg.ItemDataBound += new DataGridItemEventHandler(this.dg_ItemDataBound);
		}

		private void RefreshPageNumberInfo()
		{
			PageInfo result = this._PageInfo;

			txtPageCount.Value = PageCount.ToString();

			result.RecordCount = (int)RecordCount;
			result.PageSize = PageSize;
			result.PageCount = PageCount;

			int currentPage = 0;
			if (PagerSettings.Mode == DeluxePagerMode.NextPreviousFirstLast)
				currentPage = PageIndex + 1;
			else
			{
				if (!this.Page.IsPostBack)
					currentPage = PageIndex + 1;
				else
				{
					currentPage = PageIndex + 1;
				}
			}

			if (currentPage == 0)
				currentPage = 1;

			result.CurrentPage = currentPage;
		}

		/*
		/// <summary>
		/// 给页码部分的控件赋值
		/// </summary>
		/// <remarks>
		///  给页码部分的控件赋值
		/// </remarks>
		private void GetPageNumber()
		{
			txtPageCount.Value = PageCount.ToString();
			//this.ClientPageCount = PageCount;
			this.recordCount.Text = RecordCount.ToString();
			this.pageSize.Text = PageSize.ToString();
			this.pageCount.Text = PageCount.ToString();

			int currentPage = 0;
			if (PagerSettings.Mode == DeluxePagerMode.NextPreviousFirstLast)
				currentPage = PageIndex + 1;
			else
			{
				if (!this.Page.IsPostBack)
					currentPage = PageIndex + 1;
				else
				{
					currentPage = PageIndex + 1;
				}
			}

			if (currentPage == 0)
				currentPage = 1;
			this.currentPage.Text = currentPage.ToString();
		}*/

		/// <summary>
		/// 绑定控件的方法
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		/// <remarks>
		///  绑定控件的方法
		/// </remarks>
		private void BoundControl_DataBound(object sender, EventArgs e)
		{
			PageCount = this.GetTotalPageFromControl();

			//ShenZheng
			PageIndex = this.GetPageIndexFromControl();
			PageSize = this.GetPageSizeFromControl();
		}

		/// <summary>
		/// 从控件获取总页数
		/// </summary>
		/// <returns></returns>
		/// <remarks>
		///  从控件获取总页数
		/// </remarks>
		private int GetTotalPageFromControl()
		{
			int result = 0;

			if (this.boundControl != null)
			{
				System.Type type = this.boundControl.GetType();
				PropertyInfo pi = type.GetProperty("PageCount");

				if (pi != null)
					if (pi.CanRead)
						result = (int)pi.GetValue(this.boundControl, null);
			}

			return result;
		}

		/// <summary>
		/// 从控件获取当前页码
		/// </summary>
		/// <returns></returns>
		/// <remarks>
		///  从控件获取当前页码
		/// </remarks>
		private int GetPageIndexFromControl()
		{
			int result = 0;

			if (this.boundControl != null)
			{
				System.Type type = this.boundControl.GetType();
				PropertyInfo pi = type.GetProperty("PageIndex");

				if (pi != null)
					if (pi.CanRead)
						result = (int)pi.GetValue(this.boundControl, null);
			}

			return result;
		}

		/// <summary>
		/// 从控件获取单页的大小
		/// </summary>
		/// <returns></returns>
		/// <remarks>
		///  从控件获取单页的大小
		/// </remarks>
		private int GetPageSizeFromControl()
		{
			int result = 0;

			if (this.boundControl != null)
			{
				System.Type type = this.boundControl.GetType();
				//FormView和 DetailsView 没有PageSize 属性
				if (type.Name == "FormView" || type.Name == "DetailsView")
					return 1;
				PropertyInfo pi = type.GetProperty("PageSize");

				if (pi != null)
					if (pi.CanRead)
						result = (int)pi.GetValue(this.boundControl, null);
			}

			return result;
		}

		/// <summary>
		/// 设置当前页码给控件
		/// </summary>
		/// <param name="pageIndex"></param>
		/// <remarks>
		///  设置当前页码给控件
		/// </remarks>
		private void SetPageIndexToControl(int pageIndex)
		{
			if (this.boundControl != null)
			{
				System.Type type = this.boundControl.GetType();
				IPagerBoundControlType pagerControlType = new PagerBoundControlType();
				PagerBoundControlStatus pbControlStatus = pagerControlType.GetPagerBoundControl(type);

				MethodInfo mi = type.GetMethod("OnPageIndexChanging", BindingFlags.Instance | BindingFlags.NonPublic);

				IPageEventArgs eventArgs = new PageEventArgs();
				mi.Invoke(this.boundControl, new object[] { eventArgs.GetPageEventArgs(pbControlStatus.DataListControlType, "EventPageIndexChanging", null, pageIndex) });

				PropertyInfo pi = type.GetProperty("PageIndex");

				if (pi != null)
				{
					if (pi.CanWrite)
					{
						pi.SetValue(this.boundControl, pageIndex, null);

						MethodInfo miChanged = type.GetMethod("OnPageIndexChanged", BindingFlags.Instance | BindingFlags.NonPublic);

						if (miChanged != null)
							miChanged.Invoke(this.boundControl, new object[] { new EventArgs() });
					}
				}
			}

			//单独处理DataGrid 
			if (this.control != null)
			{
				System.Type type = this.control.GetType();
				IPagerBoundControlType pagerControlType = new PagerBoundControlType();
				PagerBoundControlStatus pbControlStatus = pagerControlType.GetPagerBoundControl(type);

				if (pbControlStatus.DataListControlType == DataListControlType.DataGrid)
				{
					if (PagerSettings.Mode == DeluxePagerMode.Numeric && pageIndex == PageCount)
						pageIndex = pageIndex - 1;

					this.SetDataGridPageIndexToControl(pageIndex);
				}
			}
		}

		/// <summary>
		/// 设置DataGrid当前页码给DataGrid控件
		/// </summary>
		/// <param name="pageIndex"></param>
		/// <remarks>
		///  设置DataGrid当前页码给DataGrid控件
		/// </remarks>
		private void SetDataGridPageIndexToControl(int pageIndex)
		{
			if (this.control != null)
			{
				System.Type type = this.control.GetType();
				IPagerBoundControlType pagerControlType = new PagerBoundControlType();
				PagerBoundControlStatus pbControlStatus = new PagerBoundControlStatus();

				pbControlStatus = pagerControlType.GetPagerBoundControl(type);

				MethodInfo mi = type.GetMethod("OnPageIndexChanged", BindingFlags.Instance | BindingFlags.NonPublic);

				IPageEventArgs eventArgs = new PageEventArgs();

				mi.Invoke(this.control, new object[] { eventArgs.GetPageEventArgs(pbControlStatus.DataListControlType, "EventPageIndexChanged", (object)this.control, pageIndex) });
				PropertyInfo pi = type.GetProperty("CurrentPageIndex");

				if (pi != null)
					if (pi.CanWrite)
						pi.SetValue(this.control, pageIndex, null);
			}
		}


		/// <summary>
		/// 查询控件通过控件ID
		/// </summary>
		/// <param name="parent"></param>
		/// <param name="controlID"></param>
		/// <returns></returns>
		/// <remarks>
		///  查询控件通过控件ID
		/// </remarks>
		private Control FindControlRecursivly(Control parent, string controlID)
		{
			Control result = parent.FindControl(controlID);

			if (result == null)
			{
				foreach (Control ctrl in parent.Controls)
				{
					result = this.FindControlRecursivly(ctrl, controlID);
					if (result != null)
						break;
				}
			}
			return result;
		}

		/// <summary>
		/// 跳转页实现
		/// </summary>
		/// <remarks>
		///  跳转页实现
		/// </remarks>
		private void PageGoto()
		{
			int CurrentPageCode = 0;//跳转页中输入的值       
			CurrentPageCode = Convert.ToInt32(this.txtPageCode.Text);
			PageIndex = CurrentPageCode - 1;

			SetPageIndex(this.PageIndex);
		}
		#endregion

		#region  Events
		/// <summary>
		/// 通用翻页控件的翻页事件
		/// </summary>
		/// <remarks>
		///  通用翻页控件的翻页事件
		/// </remarks>
		[Category("Behavior")]
		public event EventHandler CommonPageIndexChanged;

		private void OnCommonPageIndexChanged()
		{
			if (CommonPageIndexChanged != null)
				CommonPageIndexChanged(this, new EventArgs());
			//ReDrawingControls(PageIndex);
		}
		#endregion

		#region 设计态 Pager Rendering
		/// <summary>
		/// 设计模式
		/// </summary>
		/// <param name="pager"></param>
		/// <returns></returns>
		/// <remarks>
		///  设计模式
		/// </remarks>
		public string GetMenuDesignHTML(DeluxePager pager)
		{
			StringBuilder strB = new StringBuilder();
			int currentPageSize = pager.PageSize;
			strB.Append("<Table>");
			strB.AppendFormat("<tr><td align='left' width='18%'>&nbsp;&nbsp;总记录数<span>");
			strB.Append(pager.PageCount);
			strB.Append("</span>&nbsp;&nbsp;<span>");
			strB.Append(currentPageSize);
			strB.Append("</span>/页&nbsp;&nbsp;第<span>");
			strB.Append(pager.PageIndex + 1);
			strB.Append("</span>页/共<span>");
			strB.Append(pager.RecordCount);
			strB.Append("</span>页&nbsp;&nbsp;&nbsp;&nbsp;</td>");
			strB.Append("<td align='center' width='16%'>&nbsp;&nbsp;&nbsp;&nbsp;");
			if (pager.PagerSettings.Mode == DeluxePagerMode.Numeric)
			{
				strB.Append("<span style='color:red;font-weight:bold'>1</span>&nbsp;&nbsp;");
				for (int i = 2; i <= currentPageSize; i++)
				{
					strB.AppendFormat(" <a href=\"javascript:__doPostBack('ctl{0}','')\">{0}</a>&nbsp;&nbsp;", i);
				}
			}
			else
			{
				strB.AppendFormat("<a disabled=\"disabled\" style=\"text-decoration:none;\">首页</a>&nbsp;&nbsp;<a disabled=\"disabled\" style=\"text-decoration:none;\">上一页</a>&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;<a href=\"javascript:__doPostBack('atrm','')\" style=\"text-decoration:none;\">下一页</a>&nbsp;&nbsp;<a href=\"javascript:__doPostBack('bt','')\" style=\"text-decoration:none;\">尾页</a>");
			}
			strB.AppendFormat("&nbsp;&nbsp;</td>");
			if (pager.GotoPageShow)
			{
				strB.AppendFormat("<td align='right' width='4%'><input name=\"txtGoto\" type=\"text\" value=\"1\" id=\"txtGoto\" style=\"width:20px;\" />&nbsp;<input type=\"submit\" name=\"btn_goto\" value=\"跳转到\" />&nbsp;&nbsp;</td></tr>");
			}
			strB.Append("</Table>");
			return strB.ToString();
		}

		#endregion

		//internal void ProcessPostBackEvent()
		//{
		//    string eventTarget = HttpContext.Current.Request.Form["__EVENTTARGET"];
		//    string eventArgument = HttpContext.Current.Request.Form["__EVENTARGUMENT"];

		//    if (eventTarget.IsNotEmpty() && HttpContext.Current.Items.Contains("Loaded") == false)
		//    {
		//        HttpContext.Current.Items["Loaded"] = true;

		//        if (eventTarget.IndexOf(this.UniqueID) == 0)
		//        {
		//            RaisePostBackEvent(eventArgument);
		//        }
		//    }
		//}

		void IPostBackEventHandler.RaisePostBackEvent(string eventArgument)
		{
			this.RaisePostBackEvent(eventArgument);
		}

		private void RaisePostBackEvent(string eventArgument)
		{
			int index = eventArgument.IndexOf('$');
			if (index >= 0)
			{
				CommandEventArgs args = new CommandEventArgs(eventArgument.Substring(0, index), eventArgument.Substring(index + 1));
				this.HandleEvent(args);
			}
		}

		private void HandleEvent(CommandEventArgs args)
		{
			if (args != null)
			{
				string commandName = args.CommandName;

				switch (commandName)
				{
					case "Next":
						this.PageIndex++;
						break;
					case "Prev":
						this.PageIndex--;
						break;
					case "First":
						this.PageIndex = 0;
						break;
					case "Last":
						this.PageIndex = this.PageCount - 1;
						break;
					case "Number":
						{
							int pageIndex = 0;
							if (int.TryParse((string)args.CommandArgument, out pageIndex))
								this.PageIndex = pageIndex - 1;
						}
						break;
					case "Goto":
						{
							int pageIndex = 0;

							if (int.TryParse(Page.Request.Form[this.UniqueID + "$txtPageCode"], out pageIndex))
								this.PageIndex = pageIndex - 1;
						}
						break;
				}

				SetPageIndex(this.PageIndex);
			}
		}

		PostBackOptions IPostBackContainer.GetPostBackOptions(IButtonControl buttonControl)
		{
			PostBackOptions options = new PostBackOptions(this, buttonControl.CommandName + "$" + buttonControl.CommandArgument);
			options.RequiresJavaScriptProtocol = true;
			return options;
		}

		#region ICascadePagedControl Members

		void ICascadePagedControl.SetPageIndex(object source, int pageIndex)
		{
			this.SetPageIndex(pageIndex);
		}

		#endregion
	}

	/// <summary>
	/// render控件
	/// </summary>
	/// <remarks>
	///  render控件
	/// </remarks>
	public class RenderControllor
	{
		/// <summary>
		/// render控件
		/// </summary>
		/// <param name="control"></param>
		/// <returns></returns>
		/// <remarks>
		///  render控件
		/// </remarks>
		public static string RenderControl(System.Web.UI.Control control)
		{
			StringBuilder result = new StringBuilder(1024);
			control.RenderControl(new HtmlTextWriter(new StringWriter(result)));

			return result.ToString();
		}

		/// <summary>
		/// render控件
		/// </summary>
		/// <param name="control"></param>
		/// <returns></returns>
		/// <remarks>
		///  render控件
		/// </remarks>
		public static string RenderControl(System.Web.UI.TemplateControl control)
		{
			StringBuilder result = new StringBuilder(1024);
			control.RenderControl(new HtmlTextWriter(new StringWriter(result)));

			return result.ToString();
		}

		/// <summary>
		/// render页面
		/// </summary>
		/// <param name="pageLocation"></param>
		/// <returns></returns>
		/// <remarks>
		///  render页面
		/// </remarks>
		public static string RenderPage(string pageLocation)
		{
			HttpContext context = HttpContext.Current;
			StringBuilder result = new StringBuilder(1024);

			context.Server.Execute(
				pageLocation,
				new HtmlTextWriter(new StringWriter(result)));

			return result.ToString();
		}
	}

	internal class DeluxePagerLinkButton : LinkButton
	{
		private IPostBackContainer container;

		public DeluxePagerLinkButton(IPostBackContainer container)
		{
			this.container = container;
		}

		protected override PostBackOptions GetPostBackOptions()
		{
			if (this.container != null)
				return this.container.GetPostBackOptions(this);

			return base.GetPostBackOptions();
		}
	}

	internal class DeluxePagerButton : Button
	{
		private IPostBackContainer container;

		public DeluxePagerButton(IPostBackContainer container)
		{
			this.container = container;
		}

		protected override PostBackOptions GetPostBackOptions()
		{
			if (this.container != null)
				return this.container.GetPostBackOptions(this);

			return base.GetPostBackOptions();
		}
	}
}