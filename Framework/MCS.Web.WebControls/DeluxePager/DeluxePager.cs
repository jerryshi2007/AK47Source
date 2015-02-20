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
	#region ö����
	/// <summary>
	/// ��ҳ�ؼ�֧�ֵĿؼ�����
	/// </summary>
	/// <remarks>
	///  ��ҳ�ؼ�֧�ֵĿؼ�����
	/// </remarks>
	public enum DataListControlType
	{
		/// <summary>
		/// Nothing�ؼ�
		/// </summary>
		/// <remarks>
		///  Nothing�ؼ�
		/// </remarks>
		Nothing,
		/// <summary>
		/// GridView�ؼ�
		/// </summary>
		/// <remarks>
		///  GridView�ؼ�
		/// </remarks>
		GridView,
		/// <summary>
		/// Table�ؼ�
		/// </summary>
		/// <remarks>
		///  Table�ؼ�
		/// </remarks>
		Table,
		/// <summary>
		/// DataGrid�ؼ�
		/// </summary>
		/// <remarks>
		///  DataGrid�ؼ�
		/// </remarks>
		DataGrid,
		/// <summary>
		/// DataList�ؼ�
		/// </summary>
		/// <remarks>
		///  DataList�ؼ�
		/// </remarks>
		DataList,
		/// <summary>
		/// DeluxeGrid�ؼ�
		/// </summary>
		/// <remarks>
		///  DeluxeGrid�ؼ�
		/// </remarks>
		DeluxeGrid,
		/// <summary>
		/// DetailsView�ؼ�
		/// </summary>
		/// <remarks>
		///  DetailsView�ؼ�
		/// </remarks>
		DetailsView,
		/// <summary>
		/// FormView�ؼ�
		/// </summary>
		/// <remarks>
		///  FormView�ؼ�
		/// </remarks>
		FormView,
		/// <summary>
		/// Repeater�ؼ�
		/// </summary>
		/// <remarks>
		/// Repeater�ؼ�
		/// </remarks>
		Repeater
	}
	#endregion
	/// <summary>
	/// DeluxePager�ؼ� �̳���ScriptControlBase
	/// </summary>    
	/// <remarks>
	///  DeluxePager�ؼ� �̳���ScriptControlBase
	/// </remarks>
	[DefaultProperty("DeluxePager"),
	ToolboxData("<{0}:DeluxePager runat=server Width=\"500\"></{0}:DeluxePager>")]
	[Designer(typeof(DeluxePagerDesigner))]
	[ParseChildren(true),
	PersistChildren(false)]
	public class DeluxePager : WebControl, IPostBackEventHandler, IPostBackContainer, INamingContainer, ICascadePagedControl //ScriptControlBase
	{
		/// <summary>
		/// ���캯��
		/// </summary>
		/// <remarks>
		///  ���캯��
		/// </remarks>
		public DeluxePager()
		{
			txtPageCount = new HtmlInputHidden();
			txtPageCount.ID = "txtPageCount";
		}
		/// <summary>
		/// ҳ����ʾ���ؿؼ�
		/// </summary>
		/// <remarks>
		///  ҳ����ʾ���ؿؼ�
		/// </remarks>
		protected HtmlInputHidden txtPageCount;

		#region Private
		/// <summary>
		/// �󶨿ؼ�ID
		/// </summary>
		/// <remarks>
		///  �󶨿ؼ�ID
		/// </remarks>
		private string dataBoundControlID = string.Empty;
		/// <summary>
		/// �������ݰ󶨿ؼ�����
		/// </summary>
		/// <remarks>
		///  �������ݰ󶨿ؼ�����
		/// </remarks>
		private BaseDataBoundControl boundControl = null;
		/// <summary>
		/// �ؼ�
		/// </summary>
		/// <remarks>
		///  �ؼ�
		/// </remarks>
		private Control control = null;

		private TextBox txtPageCode = new TextBox();

		private class PageInfo
		{
			/// <summary>
			/// ҳ������
			/// </summary>
			public int PageCount;

			/// <summary>
			/// ��ǰҳ��
			/// </summary>
			public int CurrentPage;

			/// <summary>
			/// ��¼����
			/// </summary>
			public int RecordCount;

			/// <summary>
			/// ҳ���С
			/// </summary>
			public int PageSize;
		}

		PageInfo _PageInfo = new PageInfo();

		/*
		/// <summary>
		/// ҳ������
		/// </summary>  
		/// <remarks>
		///  ҳ������
		/// </remarks>
		private Label pageCount = new Label();
		/// <summary>
		/// ��ǰҳ��
		/// </summary>
		/// <remarks>
		///  ��ǰҳ��
		/// </remarks>
		private Label currentPage = new Label();
		/// <summary>
		/// ��¼����
		/// </summary>
		/// <remarks>
		///  ��¼����
		/// </remarks>
		private Label recordCount = new Label();
		/// <summary>
		/// ҳ���С
		/// </summary>
		/// <remarks>
		///  ҳ���С
		/// </remarks>
		private Label pageSize = new Label();
		*/

		/// <summary>
		/// ҳ�벿��
		/// </summary>
		/// <remarks>
		///  ҳ�벿��
		/// </remarks>
		TableCell cellPageNumber = new TableCell();
		/// <summary>
		/// ��ҳ����
		/// </summary>
		/// <remarks>
		///  ��ҳ����
		/// </remarks>
		TableCell cellPager = new TableCell();
		/// <summary>
		/// ��תҳ����
		/// </summary>
		/// <remarks>
		///  ��תҳ����
		/// </remarks>
		TableCell cellGotoPage = new TableCell();
		/// <summary>
		/// ��תҳ��ť����
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
		/// DataGrid���ݰ��¼�������
		/// </summary>
		/// <remarks>
		///  DataGrid���ݰ��¼�������
		/// </remarks>
		int itemDataBoundCount = 0;
		#endregion

		#region ˽������
		/// <summary>
		/// ��һ����ʵ��ҳ����
		/// </summary>
		/// <remarks>
		///  ��һ����ʵ��ҳ����
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
		/// �Ƿ���ʾ��תҳ,Ĭ��true
		/// </summary>
		/// <remarks>
		///  �Ƿ���ʾ��תҳ,Ĭ��true
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

		#region �ڲ�����

		/// <summary>
		/// ��ǰҳ�����
		/// </summary>
		/// <remarks>
		///  ��ǰҳ�����
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
		/// ҳ������
		/// </summary>
		/// <remarks>
		///  ҳ������
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
		/// ��ҳ��¼����
		/// </summary>
		/// <remarks>
		///  ��ҳ��¼����
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

		#region  Properties ����
		/// <summary>
		/// Culture��Category
		/// </summary>
		[Browsable(true),
		Description("Culture��Category"),
		DefaultValue(""),
		Category("��չ")]
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
		/// �¼��ؼ�ID������ʵ����ICascadePagedControl�ӿ�
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
		/// ���ݰ󶨿ؼ�ID
		/// </summary>
		/// <remarks>
		///  ���ݰ󶨿ؼ�ID
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
		/// ��ҳ��������
		/// </summary>
		/// <remarks>
		///  ��ҳ��������
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
		/// ������תҳ��ťText
		/// </summary>
		/// <remarks>
		///  ������תҳ��ťText
		/// </remarks>
		[Browsable(true),
		Description("������תҳ��ť��Text"),
		DefaultValue("��ת��"),
		Category("��չ")]
		public string GotoButtonText
		{
			get
			{
				return WebControlUtility.GetViewStateValue<string>(ViewState,
					"GotoButtonText",
					Translator.Translate(Define.DefaultCategory, "��ת��"));
			}
			set
			{
				WebControlUtility.SetViewStateValue<string>(ViewState, "GotoButtonText", value);
			}
		}

		/// <summary>
		/// ҳ����ʾģʽ
		/// </summary>
		/// <remarks>
		///  ҳ����ʾģʽ
		/// </remarks>
		[Browsable(true)]
		[Description("ҳ����ʾģʽ")]
		[DefaultValue(PagerCodeShowMode.All)]
		[Category("��չ")]
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
		/// ��ȡ��¼����
		/// </summary>
		/// <remarks>
		///  ��ȡ��¼����
		/// </remarks>
		[Browsable(false)]
		[Description("���ݼ�¼����"),
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
		/// �Ƿ�ΪIDataSouce���͵�����Դ Ĭ��true
		/// </summary>
		/// <remarks>
		///  �Ƿ�ΪIDataSouce���͵�����Դ Ĭ��true
		/// </remarks>
		[Description("�Ƿ�ΪIDataSouce���͵�����Դ"),
		DefaultValue(true),
		Category("��չ")]
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
		/// ��ǰ������չʾ�ؼ��Ƿ���з�ҳ����,Ĭ��true
		/// </summary>
		/// <remarks>
		///  ��ǰ������չʾ�ؼ��Ƿ���з�ҳ����,Ĭ��true
		/// </remarks>
		[Description("��ǰ������չʾ�ؼ��Ƿ���з�ҳ����"),
		DefaultValue(true),
		Category("��չ")]
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
		/// ��ҳ�ؼ�style
		/// </summary>
		/// <remarks>
		///  ��ҳ�ؼ�style
		/// </remarks>
		[Description("��ҳ�ؼ�style"),
		Category("��չ")]
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
		/// ��ҳ�ؼ���תҳ�����CssClass
		/// </summary>
		/// <remarks>
		/// ��ҳ�ؼ���תҳ�����CssClass
		/// </remarks>
		[Description("��ҳ�ؼ���תҳ�����CssClass"),
		DefaultValue(""),
		Category("��չ")]
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
		/// ��ҳ�ؼ�CssClass
		/// </summary>
		/// <remarks>
		/// ��ҳ�ؼ�CssClass
		/// </remarks>
		[Description("��ҳ�ؼ�CssClass"),
		DefaultValue(""),
		Category("��չ")]
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
		/// ��תҳ�ؼ�CssClass
		/// </summary>
		/// <remarks>
		/// ��תҳ�ؼ�CssClass
		/// </remarks>
		[Description("��תҳ�ؼ�CssClass"),
		DefaultValue("portalButton"),
		Category("��չ")]
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
		/// �����ӿؼ�
		/// </summary>
		/// <remarks>
		///  �����ӿؼ�
		/// </remarks>
		private void CreatePagerContent()
		{
			this.table.ID = "pagerTable";
			table.CssClass = TableCssClass;
			this.ApplyStyle(TableStyle);

			this.tRow.ID = "pagerRow";
			//��ҳ�벿��
			this.DrawingPageNumber(this.cellPageNumber);
			//��ʼ��paging����
			InitializePager(this.cellPager);
			this.cellPager.ID = "cellPager";
			//��ʼ��GotoPaging����
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
		/// ��ʼ��Pager��ҳ����
		/// </summary>
		/// <param name="cell"></param>
		/// <remarks>
		///  ��ʼ��Pager��ҳ����
		/// </remarks>
		protected void InitializePager(TableCell cell)
		{
			switch (PagerSettings.Mode)
			{
				case DeluxePagerMode.Numeric:
					this.DrawingNumericPager(cell);
					//���Numeric����תҳ���ֲ���ʾ
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
		/// ��ʼ���ؼ�
		/// </summary>
		/// <param name="e"></param>
		/// <remarks>
		///  ��ʼ���ؼ�
		/// </remarks>
		protected override void OnInit(EventArgs e)
		{
			if (!this.DesignMode)
			{
				this.control = (Control)this.FindControlRecursivly(this.Page, DataBoundControlID);

				if (this.control != null)
				{
					//���ð󶨶�Ӧ�ؼ��ķ�ҳ����
					IPageEventArgs ipea = new PageEventArgs();

					IPagerBoundControlType pagerControlType = new PagerBoundControlType();
					PagerBoundControlStatus pbControlStatus = pagerControlType.GetPagerBoundControl(this.control.GetType());

					ipea.SetBoundControlPagerSetting(this.control, pbControlStatus.DataListControlType, this.PageSize);

					if (pbControlStatus.IsPagedControl && this.IsPagedControl && DataBoundControlID != string.Empty)
					{
						//DataGrid���������з�ҳ���ܿؼ��Ļ��Ʋ�ͬ�����ڼ̳еĻ��಻ͬ����������ų�DataGrid����
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
		/// ���·�ҳ
		/// </summary>
		/// <param name="pageIndex"></param>
		/// <remarks>
		///  ���·�ҳ
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
		/// ��תҳ��ť��ҳ����
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		/// <remarks>
		///  ��תҳ��ť��ҳ����
		/// </remarks>
		void BtnGoto_Click(object sender, EventArgs e)
		{
			this.PageGoto();
			//this.Page.RegisterClientScriptBlock("", "JudgePageGoto('"+this.this.txtPageCode.Text+"','"+txtPageCount.Value+"')");
			//�ػ���ҳ�ؼ�
			//this.ReDrawingControls(PageIndex);
		}


		/// <summary>
		/// �����ַ�ҳ����
		/// </summary>
		/// <param name="tc"></param>
		/// <remarks>
		///  �����ַ�ҳ����
		/// </remarks>
		private void DrawingNumericPager(TableCell tc)
		{
			if (!this.IsDataSourceControl && PageSize != 0)
				this.PageCount = RecordCount % PageSize == 0 ? (int)RecordCount / PageSize : ((int)RecordCount / PageSize) + 1;
			DeluxePagerLinkButton lButton;
			//��ҳ����
			int pageCount = this.PageCount;
			//��ǰҳ��
			int num2 = PageIndex;
			//if (!this.Page.IsPostBack)
			num2 = PageIndex + 1;
			//��ǰ��ҳ�ؼ��İ�ť���� ��pageSize
			int num4 = PagerSettings.PageButtonCount;
			//��ǰҪ��ʵ�ĵ�һ��ҳ����
			int num5 = this.FirstDisplayedPageIndex + 1;
			//��ҳ�İ�ť����
			int pageButtonCount = PagerSettings.PageButtonCount;
			//�ؼ����
			int btnIndex = 1;
			if (pageCount < num4)
			{
				num4 = pageCount;
			}
			//ѭ�����
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
		/// ��ʾ����ҳ ��һҳ ��һҳ βҳ����ť
		/// </summary>
		/// <param name="tc">TableCell</param>
		/// <remarks>
		///  ��ʾ����ҳ ��һҳ ��һҳ βҳ����ť
		/// </remarks>
		private void DrawingPaging(TableCell tc)
		{
			DeluxePagerLinkButton lbtnFirst = new DeluxePagerLinkButton(this);
			DeluxePagerLinkButton lbtnPrev = new DeluxePagerLinkButton(this);
			DeluxePagerLinkButton lbtnNext = new DeluxePagerLinkButton(this);
			DeluxePagerLinkButton lbtnLast = new DeluxePagerLinkButton(this);
			//�����ҳ����һҳ��ť
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

			//��ʾ����ҳ ��һҳ ��һҳ βҳ����ť
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

			//�����һҳ��βҳ��ť
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

			//�����ҳ����һҳ��ť
			if (this.PageIndex <= 0)
			{
				lbtnFirst.Enabled = lbtnPrev.Enabled = false;
			}
			else
			{
				lbtnFirst.Enabled = lbtnPrev.Enabled = true;
			}

			//�����һҳ��βҳ��ť
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
		/// ��ʾ��תҳ
		/// </summary>
		/// <param name="tc"></param>
		/// <param name="tcb"></param>
		/// <remarks>
		///  ��ʾ��תҳ
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
				Translator.Translate(Define.DefaultCategory, "�Ƿ��ַ�����������������") + "');bl= false;break;}} ");
			sbOnclickScript.Append("if((inputVal-pageCount)>0 || inputVal<1){alert('" +
				Translator.Translate(Define.DefaultCategory, "û�д�ҳ�룬������1��' + pageCount + '֮�������") +
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

			//ҳ��ؼ���ֵ
			RefreshPageNumberInfo();
			PageInfo pageInfo = this._PageInfo;

			StringBuilder strB = new StringBuilder();

			switch ((PagerCodeShowMode)PageCodeShowMode)
			{
				case PagerCodeShowMode.All:
					if (RecordCount.HasValue)
					{
						strB.Append("&nbsp;&nbsp;");
						strB.Append(Translator.Translate(Define.DefaultCategory, "�ܼ�¼��{0:#,##0}", pageInfo.RecordCount));
						strB.Append("&nbsp;&nbsp;");
						strB.Append(Translator.Translate(Define.DefaultCategory, "{0:#,##0}/ҳ", pageInfo.PageSize));
						strB.Append("&nbsp;&nbsp;");
					}

					strB.Append(Translator.Translate(Define.DefaultCategory, "��{0:#,##0}ҳ", pageInfo.CurrentPage));
					strB.Append("&nbsp;");
					strB.Append(Translator.Translate(Define.DefaultCategory, "��{0:#,##0}ҳ", pageInfo.PageCount));

					break;
				case PagerCodeShowMode.CurrentRecordCount:
					strB.Append(Translator.Translate(Define.DefaultCategory, "��{0:#,##0}ҳ", pageInfo.CurrentPage));
					strB.Append("/");
					strB.Append(Translator.Translate(Define.DefaultCategory, "��{0:#,##0}ҳ", pageInfo.PageCount));
					strB.Append("&nbsp;&nbsp;&nbsp;&nbsp;");
					break;
				case PagerCodeShowMode.RecordCount:
					if (RecordCount.HasValue)
					{
						strB.Append("&nbsp;&nbsp;");
						strB.Append(Translator.Translate(Define.DefaultCategory, "�ܼ�¼��{0:#,##0}", pageInfo.RecordCount));
						strB.Append("&nbsp;&nbsp;");
						strB.Append(Translator.Translate(Define.DefaultCategory, "{0:#,##0}/ҳ", pageInfo.PageSize));
					}
					break;
			}

			tc.Controls.Add(new LiteralControl(strB.ToString()));
		}

		/// <summary>
		/// �����ݿؼ�
		/// </summary>
		/// <remarks>
		///  �����ݿؼ�
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
		/// ��ȡDataGrid���Զ���ֵ
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="propertyName"></param>
		/// <returns></returns>
		/// <remarks>
		///  ��ȡDataGrid���Զ���ֵ
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
		/// DataGrid���ݰ��¼�
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		/// <remarks>
		///  DataGrid���ݰ��¼�
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
		/// DataGrid������
		/// </summary> 
		/// <remarks>
		///  DataGrid������
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
		/// ��ҳ�벿�ֵĿؼ���ֵ
		/// </summary>
		/// <remarks>
		///  ��ҳ�벿�ֵĿؼ���ֵ
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
		/// �󶨿ؼ��ķ���
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		/// <remarks>
		///  �󶨿ؼ��ķ���
		/// </remarks>
		private void BoundControl_DataBound(object sender, EventArgs e)
		{
			PageCount = this.GetTotalPageFromControl();

			//ShenZheng
			PageIndex = this.GetPageIndexFromControl();
			PageSize = this.GetPageSizeFromControl();
		}

		/// <summary>
		/// �ӿؼ���ȡ��ҳ��
		/// </summary>
		/// <returns></returns>
		/// <remarks>
		///  �ӿؼ���ȡ��ҳ��
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
		/// �ӿؼ���ȡ��ǰҳ��
		/// </summary>
		/// <returns></returns>
		/// <remarks>
		///  �ӿؼ���ȡ��ǰҳ��
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
		/// �ӿؼ���ȡ��ҳ�Ĵ�С
		/// </summary>
		/// <returns></returns>
		/// <remarks>
		///  �ӿؼ���ȡ��ҳ�Ĵ�С
		/// </remarks>
		private int GetPageSizeFromControl()
		{
			int result = 0;

			if (this.boundControl != null)
			{
				System.Type type = this.boundControl.GetType();
				//FormView�� DetailsView û��PageSize ����
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
		/// ���õ�ǰҳ����ؼ�
		/// </summary>
		/// <param name="pageIndex"></param>
		/// <remarks>
		///  ���õ�ǰҳ����ؼ�
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

			//��������DataGrid 
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
		/// ����DataGrid��ǰҳ���DataGrid�ؼ�
		/// </summary>
		/// <param name="pageIndex"></param>
		/// <remarks>
		///  ����DataGrid��ǰҳ���DataGrid�ؼ�
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
		/// ��ѯ�ؼ�ͨ���ؼ�ID
		/// </summary>
		/// <param name="parent"></param>
		/// <param name="controlID"></param>
		/// <returns></returns>
		/// <remarks>
		///  ��ѯ�ؼ�ͨ���ؼ�ID
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
		/// ��תҳʵ��
		/// </summary>
		/// <remarks>
		///  ��תҳʵ��
		/// </remarks>
		private void PageGoto()
		{
			int CurrentPageCode = 0;//��תҳ�������ֵ       
			CurrentPageCode = Convert.ToInt32(this.txtPageCode.Text);
			PageIndex = CurrentPageCode - 1;

			SetPageIndex(this.PageIndex);
		}
		#endregion

		#region  Events
		/// <summary>
		/// ͨ�÷�ҳ�ؼ��ķ�ҳ�¼�
		/// </summary>
		/// <remarks>
		///  ͨ�÷�ҳ�ؼ��ķ�ҳ�¼�
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

		#region ���̬ Pager Rendering
		/// <summary>
		/// ���ģʽ
		/// </summary>
		/// <param name="pager"></param>
		/// <returns></returns>
		/// <remarks>
		///  ���ģʽ
		/// </remarks>
		public string GetMenuDesignHTML(DeluxePager pager)
		{
			StringBuilder strB = new StringBuilder();
			int currentPageSize = pager.PageSize;
			strB.Append("<Table>");
			strB.AppendFormat("<tr><td align='left' width='18%'>&nbsp;&nbsp;�ܼ�¼��<span>");
			strB.Append(pager.PageCount);
			strB.Append("</span>&nbsp;&nbsp;<span>");
			strB.Append(currentPageSize);
			strB.Append("</span>/ҳ&nbsp;&nbsp;��<span>");
			strB.Append(pager.PageIndex + 1);
			strB.Append("</span>ҳ/��<span>");
			strB.Append(pager.RecordCount);
			strB.Append("</span>ҳ&nbsp;&nbsp;&nbsp;&nbsp;</td>");
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
				strB.AppendFormat("<a disabled=\"disabled\" style=\"text-decoration:none;\">��ҳ</a>&nbsp;&nbsp;<a disabled=\"disabled\" style=\"text-decoration:none;\">��һҳ</a>&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;<a href=\"javascript:__doPostBack('atrm','')\" style=\"text-decoration:none;\">��һҳ</a>&nbsp;&nbsp;<a href=\"javascript:__doPostBack('bt','')\" style=\"text-decoration:none;\">βҳ</a>");
			}
			strB.AppendFormat("&nbsp;&nbsp;</td>");
			if (pager.GotoPageShow)
			{
				strB.AppendFormat("<td align='right' width='4%'><input name=\"txtGoto\" type=\"text\" value=\"1\" id=\"txtGoto\" style=\"width:20px;\" />&nbsp;<input type=\"submit\" name=\"btn_goto\" value=\"��ת��\" />&nbsp;&nbsp;</td></tr>");
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
	/// render�ؼ�
	/// </summary>
	/// <remarks>
	///  render�ؼ�
	/// </remarks>
	public class RenderControllor
	{
		/// <summary>
		/// render�ؼ�
		/// </summary>
		/// <param name="control"></param>
		/// <returns></returns>
		/// <remarks>
		///  render�ؼ�
		/// </remarks>
		public static string RenderControl(System.Web.UI.Control control)
		{
			StringBuilder result = new StringBuilder(1024);
			control.RenderControl(new HtmlTextWriter(new StringWriter(result)));

			return result.ToString();
		}

		/// <summary>
		/// render�ؼ�
		/// </summary>
		/// <param name="control"></param>
		/// <returns></returns>
		/// <remarks>
		///  render�ؼ�
		/// </remarks>
		public static string RenderControl(System.Web.UI.TemplateControl control)
		{
			StringBuilder result = new StringBuilder(1024);
			control.RenderControl(new HtmlTextWriter(new StringWriter(result)));

			return result.ToString();
		}

		/// <summary>
		/// renderҳ��
		/// </summary>
		/// <param name="pageLocation"></param>
		/// <returns></returns>
		/// <remarks>
		///  renderҳ��
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