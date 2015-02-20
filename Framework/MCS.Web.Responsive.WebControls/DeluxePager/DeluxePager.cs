using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Data;
using System.Data.Sql;
using System.Globalization;
using System.IO;
using System.Reflection;
using System.Text;
using System.Text.RegularExpressions;
using System.Web;
using System.Web.UI;
using System.Web.UI.HtmlControls;
using System.Web.UI.WebControls;
using MCS.Library.Core;
using MCS.Library.Globalization;
using MCS.Web.Responsive.Library;
using MCS.Web.Responsive.Library.Script;

namespace MCS.Web.Responsive.WebControls
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
            : base(HtmlTextWriterTag.Div)
        {
        }

        /// <summary>
        /// ҳ����ʾ���ؿؼ�
        /// </summary>
        /// <remarks>
        ///  ҳ����ʾ���ؿؼ�
        /// </remarks>
        protected HtmlInputHidden _TxtPageCount = new HtmlInputHidden() { ID = "txtPageCount" };

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
        private BaseDataBoundControl _BoundControl = null;
        /// <summary>
        /// �ؼ�
        /// </summary>
        /// <remarks>
        ///  �ؼ�
        /// </remarks>
        private Control _Control = null;

        private TextBox _TxtPageCode = new TextBox();

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

        private HtmlGenericControl _List = new HtmlGenericControl("ul");

        private HtmlGenericControl _FirstListItem = new HtmlGenericControl("li");
        private HtmlGenericControl _PrevListItem = new HtmlGenericControl("li");
        private HtmlGenericControl _NextListItem = new HtmlGenericControl("li");
        private HtmlGenericControl _LastListItem = new HtmlGenericControl("li");

        HtmlGenericControl _PageCountArea = new HtmlGenericControl("div");
        DeluxePagerLinkButton _GotoBtn = null;
        Control _PageNumberContainer = new Control();

        private int _ItemDataBoundCount = 0;
        #endregion

        #region ˽������
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
                return this.ViewState.GetViewStateValue("GotoPageShow", true);
            }
            set
            {
                this.ViewState.SetViewStateValue("GotoPageShow", value);
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
                return this.ViewState.GetViewStateValue("PageIndex", 0);
            }
            set
            {
                this.ViewState.SetViewStateValue("PageIndex", value);
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
                return this.ViewState.GetViewStateValue("PageCount", 0);
            }
            set
            {
                this.ViewState.SetViewStateValue("PageCount", value);
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
                return this.ViewState.GetViewStateValue("PageSize", 0);
            }
            set
            {
                this.ViewState.SetViewStateValue("PageSize", value);
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
                return this.ViewState.GetViewStateValue("Category", string.Empty);
            }
            set
            {
                this.ViewState.SetViewStateValue("Category", value);
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
                return this.ViewState.GetViewStateValue("CascadeControlID", string.Empty);
            }
            set
            {
                this.ViewState.SetViewStateValue("CascadeControlID", value);
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
                return this.ViewState.GetViewStateValue("DataBoundControlID", string.Empty);
            }
            set
            {
                this.ViewState.SetViewStateValue("DataBoundControlID", value);
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
                PagerSettings pagerSettings = this.ViewState.GetViewStateValue<PagerSettings>("PagerSettings", null);
                if (pagerSettings == null)
                {
                    pagerSettings = new PagerSettings();
                    this.ViewState.SetViewStateValue<PagerSettings>("PagerSettings", pagerSettings);
                }
                return pagerSettings;
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
                return this.ViewState.GetViewStateValue("GotoButtonText",
                    Translator.Translate(Define.DefaultCategory, "��ת��"));
            }
            set
            {
                this.ViewState.SetViewStateValue("GotoButtonText", value);
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
        [DefaultValue(PagerCodeShowMode.Auto)]
        [Category("��չ")]
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
                return this.ViewState.GetViewStateValue("RecordCount", 0);
            }
            set
            {
                this.ViewState.SetViewStateValue("RecordCount", value);
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
                return this.ViewState.GetViewStateValue("IsDataSourceControl", true);
            }
            set
            {
                this.ViewState.SetViewStateValue("IsDataSourceControl", value);
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
                return this.ViewState.GetViewStateValue("IsPagedControl", true);
            }
            set
            {
                this.ViewState.SetViewStateValue("IsPagedControl", value);
            }
        }
        #endregion

        #region Override Method
        /// <summary>
        /// �����ӿؼ�
        /// </summary>
        /// <remarks>
        ///  �����ӿؼ�
        /// </remarks>
        private void CreatePagerContent()
        {
            this.Controls.Add(this._TxtPageCount);

            Control container = CreateSegmentDiv("col-lg-8 col-md-7 col-sm-8 col-xs-8");
            this.Controls.Add(container);

            HtmlGenericControl divTable = new HtmlGenericControl("div");
            container.Controls.Add(divTable);
            divTable.Attributes["class"] = "dataTables_paginate";

            divTable.Controls.Add(this._List);

            Control pageCountDiv = CreateSegmentDiv("col-lg-4 col-md-5 col-sm-4 col-xs-4");
            this.Controls.Add(pageCountDiv);

            pageCountDiv.Controls.Add(this._PageCountArea);
            this._PageCountArea.Attributes["class"] = "dataTables_info";

            this._List.Attributes["class"] = "pagination";

            //��ҳ����������
            this.DrawingPageNumberDesp(this._PageCountArea);

            //��ʼ��paging����
            InitializePager(this._List);

            //��ҳ����ת����
            DrawingGotoPaging(divTable);
        }

        private static Control CreateSegmentDiv(string cssClass)
        {
            HtmlGenericControl divSegment = new HtmlGenericControl("div");

            divSegment.Attributes["class"] = cssClass;

            return divSegment;
        }

        /// <summary>
        /// ��ʼ��Pager��ҳ����
        /// </summary>
        /// <param name="cell"></param>
        /// <remarks>
        ///  ��ʼ��Pager��ҳ����
        /// </remarks>
        private void InitializePager(HtmlGenericControl listContainer)
        {
            switch (PagerSettings.Mode)
            {
                case DeluxePagerMode.Numeric:
                    //���Numeric����תҳ���ֲ���ʾ
                    this.DrawingPaging(listContainer);
                    this.DrawingNumericPager(this._PageNumberContainer);
                    this.GotoPageShow = false;
                    break;

                case DeluxePagerMode.NextPreviousFirstLast:
                    this.DrawingPaging(listContainer);
                    break;
            }
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
                this._Control = (Control)this.Page.FindControlByID(this.DataBoundControlID, true);

                if (this._Control != null)
                {
                    //���ð󶨶�Ӧ�ؼ��ķ�ҳ����
                    IPageEventArgs ipea = new PageEventArgs();

                    IPagerBoundControlType pagerControlType = new PagerBoundControlType();
                    PagerBoundControlStatus pbControlStatus = pagerControlType.GetPagerBoundControl(this._Control.GetType());

                    ipea.SetBoundControlPagerSetting(this._Control, pbControlStatus.DataListControlType, this.PageSize);

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
                Control cascadeControl = (Control)this.Page.FindControlByID(this.CascadeControlID, true);

                if (cascadeControl != null && cascadeControl is ICascadePagedControl)
                    ((ICascadePagedControl)cascadeControl).SetPageIndex(this, pageIndex);
            }
        }

        /// <summary>
        /// �����ַ�ҳ����
        /// </summary>
        /// <param name="tc"></param>
        /// <remarks>
        ///  �����ַ�ҳ����
        /// </remarks>
        private void DrawingNumericPager(Control tc)
        {
            int pageCount = this.PageCount;

            if (!this.IsDataSourceControl && PageSize != 0)
                pageCount = ((int)this.RecordCount - 1) / this.PageSize + 1;

            int startIndex = (this.PageIndex / PagerSettings.PageButtonCount) * PagerSettings.PageButtonCount;
            int endIndex = Math.Min(startIndex + PagerSettings.PageButtonCount, pageCount);

            int countButtons = 0;
            for (int i = startIndex; i < endIndex; i++)
            {
                string codeText = (i + 1).ToString();

                HtmlGenericControl li = AddNavLinkButton(tc, "Number", codeText, codeText, string.Empty, string.Empty);

                li.Attributes["class"] = countButtons < 3 ? "hidden-sm hidden-xs" : "hidden-sm hidden-xs hidden-md";

                if (i == this.PageIndex)
                    li.Attributes["class"] += " active";

                countButtons++;
            }

            this.PageCount = pageCount;
        }

        /// <summary>
        /// ��ʾ����ҳ ��һҳ ��һҳ βҳ����ť
        /// </summary>
        /// <param name="tc">TableCell</param>
        /// <remarks>
        ///  ��ʾ����ҳ ��һҳ ��һҳ βҳ����ť
        /// </remarks>
        private void DrawingPaging(HtmlGenericControl tc)
        {
            HtmlGenericControl first = AddNavLinkButton(tc, "First", "First", string.Empty, string.Empty, "icon-double-angle-left");
            HtmlGenericControl prev = AddNavLinkButton(tc, "Prev", "Prev", string.Empty, string.Empty, "icon-angle-left");

            tc.Controls.Add(this._PageNumberContainer);

            HtmlGenericControl next = AddNavLinkButton(tc, "Next", "Next", string.Empty, string.Empty, "icon-angle-right");
            HtmlGenericControl last = AddNavLinkButton(tc, "Last", "Last", string.Empty, string.Empty, "icon-double-angle-right");

            SetNavLinkButtonEnabled(first, this.PageIndex > 0);
            SetNavLinkButtonEnabled(prev, this.PageIndex > 0);

            SetNavLinkButtonEnabled(next, this.PageIndex < PageCount - 1);
            SetNavLinkButtonEnabled(last, this.PageIndex < PageCount - 1);
        }

        private void SetNavLinkButtonEnabled(HtmlGenericControl listItem, bool enabled)
        {
            listItem.Attributes["class"] = GetEnabledCssClass(listItem.Attributes["class"], enabled);

            LinkButton btn = (LinkButton)listItem.Controls[0];

            btn.Enabled = enabled;
        }

        private string GetEnabledCssClass(string originalCss, bool enabled)
        {
            string result = originalCss;

            if (originalCss.IsNotEmpty())
                result = originalCss.Replace(" disabled", string.Empty);

            if (enabled == false)
                result = result + " disabled";

            return result;
        }

        private HtmlGenericControl AddNavLinkButton(Control container, string commandName, string commandArgument, string text, string btnClass, string iconClass)
        {
            HtmlGenericControl li = new HtmlGenericControl("li");
            container.Controls.Add(li);

            DeluxePagerLinkButton btn = new DeluxePagerLinkButton(this);
            li.Controls.Add(btn);

            btn.CommandName = commandName;
            btn.CommandArgument = commandArgument;

            btn.Text = text;

            if (text.IsNullOrEmpty())
            {
                HtmlGenericControl icon = new HtmlGenericControl("i");
                btn.Controls.Add(icon);

                icon.Attributes["class"] = iconClass;
            }

            return li;
        }

        private void DrawingGotoPaging(HtmlGenericControl container)
        {
            HtmlGenericControl group = new HtmlGenericControl("div");
            container.Controls.Add(group);

            group.Attributes["class"] = "input-group";

            this._TxtPageCode.CssClass = "form-control";
            group.Controls.Add(_TxtPageCode);

            this._TxtPageCode.Text = (PageIndex + 1).ToString();
            this._TxtPageCode.ID = "txtPageCode";

            HtmlGenericControl btnGroup = new HtmlGenericControl("span");
            group.Controls.Add(btnGroup);

            btnGroup.Attributes["class"] = "input-group-btn";

            this._GotoBtn = new DeluxePagerLinkButton(this);
            btnGroup.Controls.Add(this._GotoBtn);

            this._GotoBtn.Text = this.GotoButtonText;
            this._GotoBtn.CommandName = "Goto";
            this._GotoBtn.CausesValidation = false;
            this._GotoBtn.CssClass = "btn btn-default";
            this._GotoBtn.OnClientClick = GetButtonValidationScript();

            bool disabled = (this.PageCount == 0 || (this.PageIndex <= this.PageCount && this.PageCount == 1));

            group.Attributes["class"] = GetEnabledCssClass(group.Attributes["class"], !disabled);

            this._TxtPageCode.Enabled = !disabled;
            this._GotoBtn.Enabled = !disabled;
        }

        private string GetButtonValidationScript()
        {
            StringBuilder sbOnclickScript = new StringBuilder();
            sbOnclickScript.Append(" var bl = true; var inputVal = document.getElementById('" + this._TxtPageCode.ClientID + "').value;  var pageCount = document.getElementById('" + this._TxtPageCount.ClientID + "').value; if(pageCount.length==0) bl= false;");
            sbOnclickScript.Append("var oneChar;inputStr=inputVal.toString();for (var i=0;i<inputStr.length;i++){oneChar=inputStr.charAt(i);");
            sbOnclickScript.Append("if (oneChar<'0' || oneChar>'9'){ alert('" +
                Translator.Translate(Define.DefaultCategory, "�Ƿ��ַ�����������������") + "');bl= false;break;}} ");
            sbOnclickScript.Append("if((inputVal-pageCount)>0 || inputVal<1){alert('" +
                Translator.Translate(Define.DefaultCategory, "û�д�ҳ�룬������1��' + pageCount + '֮�������") +
                "');bl= false;} if(!bl)return false;");

            return sbOnclickScript.ToString();
        }

        /// <summary>
        /// ��ҳ����������
        /// </summary>
        /// <param name="tc"></param>
        private void DrawingPageNumberDesp(Control tc)
        {
            if (this.PageSize != 0)
            {
                if ((RecordCount % PageSize) > 0)
                    this.PageCount = ((int)(RecordCount / PageSize)) + 1;
                else
                    this.PageCount = (int)(RecordCount / PageSize);
            }

            //ҳ��ؼ���ֵ
            RefreshPageNumberInfo();
            PageInfo pageInfo = CalculatePageInfo();

            HtmlGenericControl allDespCtrl = CreateDescriptionControl(GetAllPageDesp(pageInfo, this.RecordCount));
            HtmlGenericControl currentDespCtrl = CreateDescriptionControl(GetPageDespWithCurrentPage(pageInfo, this.RecordCount));
            HtmlGenericControl recordCountDespCtrl = CreateDescriptionControl(GetPageDespOnlyRecourdCount(pageInfo, this.RecordCount));

            tc.Controls.Add(allDespCtrl);
            tc.Controls.Add(currentDespCtrl);
            tc.Controls.Add(recordCountDespCtrl);

            switch ((PagerCodeShowMode)PageCodeShowMode)
            {
                case PagerCodeShowMode.All:
                    allDespCtrl.Attributes["class"] = string.Empty;
                    currentDespCtrl.Attributes["class"] = "hidden";
                    recordCountDespCtrl.Attributes["class"] = "hidden";
                    break;
                case PagerCodeShowMode.CurrentRecordCount:
                    allDespCtrl.Attributes["class"] = "hidden";
                    currentDespCtrl.Attributes["class"] = string.Empty;
                    recordCountDespCtrl.Attributes["class"] = "hidden";
                    break;
                case PagerCodeShowMode.RecordCount:
                    allDespCtrl.Attributes["class"] = "hidden";
                    currentDespCtrl.Attributes["class"] = "hidden";
                    recordCountDespCtrl.Attributes["class"] = string.Empty;
                    break;
                case PagerCodeShowMode.Auto:
                    allDespCtrl.Attributes["class"] = "hidden-sm hidden-xs";
                    recordCountDespCtrl.Attributes["class"] = "hidden-md hidden-lg hidden-xs";
                    currentDespCtrl.Attributes["class"] = "hidden-md hidden-lg hidden-sm";
                    break;
            }
        }

        private static HtmlGenericControl CreateDescriptionControl(string description)
        {
            HtmlGenericControl result = new HtmlGenericControl();

            result.InnerHtml = description;

            return result;
        }

        private static string GetPageDespOnlyRecourdCount(PageInfo pageInfo, int? recordCount)
        {
            StringBuilder strB = new StringBuilder();

            if (recordCount.HasValue)
            {
                strB.Append("&nbsp;&nbsp;");
                strB.Append(Translator.Translate(Define.DefaultCategory, "�ܼ�¼��{0:#,##0}", pageInfo.RecordCount));
                strB.Append("&nbsp;&nbsp;");
                strB.Append(Translator.Translate(Define.DefaultCategory, "{0:#,##0}/ҳ", pageInfo.PageSize));
            }

            return strB.ToString();
        }

        private static string GetPageDespWithCurrentPage(PageInfo pageInfo, int? recordCount)
        {
            StringBuilder strB = new StringBuilder();

            if (recordCount.HasValue)
            {
                strB.Append(Translator.Translate(Define.DefaultCategory, "��{0:#,##0}ҳ", pageInfo.CurrentPage));
                strB.Append("/");
                strB.Append(Translator.Translate(Define.DefaultCategory, "��{0:#,##0}ҳ", pageInfo.PageCount));
                strB.Append("&nbsp;&nbsp;&nbsp;&nbsp;");
            }

            return strB.ToString();
        }

        private static string GetAllPageDesp(PageInfo pageInfo, int? recordCount)
        {
            StringBuilder strB = new StringBuilder();

            if (recordCount.HasValue)
            {
                strB.Append(Translator.Translate(Define.DefaultCategory, "�ܼ�¼��{0:#,##0}", pageInfo.RecordCount));
                strB.Append("&nbsp;&nbsp;");
                strB.Append(Translator.Translate(Define.DefaultCategory, "{0:#,##0}/ҳ", pageInfo.PageSize));
                strB.Append("&nbsp;&nbsp;");
            }

            strB.Append(Translator.Translate(Define.DefaultCategory, "��{0:#,##0}ҳ", pageInfo.CurrentPage));
            strB.Append("&nbsp;");
            strB.Append(Translator.Translate(Define.DefaultCategory, "��{0:#,##0}ҳ", pageInfo.PageCount));

            return strB.ToString();
        }

        /// <summary>
        /// �����ݿؼ�
        /// </summary>
        /// <remarks>
        ///  �����ݿؼ�
        /// </remarks>
        private void BindDataControl()
        {
            BaseDataBoundControl boundControl = (BaseDataBoundControl)this.Page.FindControlByID(this.DataBoundControlID, true);

            if (boundControl != null)
            {
                System.Type type = boundControl.GetType();
                IPagerBoundControlType pagerControlType = new PagerBoundControlType();
                PagerBoundControlStatus pbControlStatus = pagerControlType.GetPagerBoundControl(type);

                if (pbControlStatus.DataListControlType != DataListControlType.DataGrid)
                {
                    boundControl.DataBound += new EventHandler(BoundControl_DataBound);
                    this._BoundControl = boundControl;
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
                {
                    if (pi.CanRead)
                        result = (int)pi.GetValue(sender, null);
                }
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
            if (this._ItemDataBoundCount > 0)
                return;

            this.PageCount = this.GetPagerFromDataGridControl(sender, "PageCount");

            //ShenZheng
            PageIndex = this.GetPagerFromDataGridControl(sender, "CurrentPageIndex");
            PageSize = this.GetPagerFromDataGridControl(sender, "PageSize");

            if (this.Page.IsPostBack && PagerSettings.Mode == DeluxePagerMode.Numeric == false)
            {
                this._PageNumberContainer.Controls.Clear();
                this.DrawingNumericPager(this._PageNumberContainer);
            }

            this.RefreshPageNumberInfo();

            this._ItemDataBoundCount++;
        }

        /// <summary>
        /// DataGrid������
        /// </summary> 
        /// <remarks>
        ///  DataGrid������
        /// </remarks>
        private void DataGridDataSource()
        {
            DataGrid dg = (DataGrid)this.Page.FindControlByID(this.DataBoundControlID, true);
            dg.ItemDataBound += new DataGridItemEventHandler(this.dg_ItemDataBound);
        }

        private void RefreshPageNumberInfo()
        {
            this._TxtPageCount.Value = PageCount.ToString();
        }

        private PageInfo CalculatePageInfo()
        {
            PageInfo result = new PageInfo();

            result.RecordCount = (int)RecordCount;
            result.PageSize = this.PageSize;
            result.PageCount = this.PageCount;

            int currentPage = this.PageIndex + 1;

            if (currentPage == 0)
                currentPage = 1;

            result.CurrentPage = currentPage;

            return result;
        }

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

            if (this._BoundControl != null)
            {
                System.Type type = this._BoundControl.GetType();
                PropertyInfo pi = type.GetProperty("PageCount");

                if (pi != null)
                    if (pi.CanRead)
                        result = (int)pi.GetValue(this._BoundControl, null);
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

            if (this._BoundControl != null)
            {
                System.Type type = this._BoundControl.GetType();
                PropertyInfo pi = type.GetProperty("PageIndex");

                if (pi != null)
                    if (pi.CanRead)
                        result = (int)pi.GetValue(this._BoundControl, null);
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

            if (this._BoundControl != null)
            {
                System.Type type = this._BoundControl.GetType();

                //FormView�� DetailsView û��PageSize ����
                if (type.Name == "FormView" || type.Name == "DetailsView")
                    return 1;

                PropertyInfo pi = type.GetProperty("PageSize");

                if (pi != null)
                    if (pi.CanRead)
                        result = (int)pi.GetValue(this._BoundControl, null);
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
            if (this._BoundControl != null)
            {
                System.Type type = this._BoundControl.GetType();
                IPagerBoundControlType pagerControlType = new PagerBoundControlType();
                PagerBoundControlStatus pbControlStatus = pagerControlType.GetPagerBoundControl(type);

                MethodInfo mi = type.GetMethod("OnPageIndexChanging", BindingFlags.Instance | BindingFlags.NonPublic);

                IPageEventArgs eventArgs = new PageEventArgs();
                mi.Invoke(this._BoundControl, new object[] { eventArgs.GetPageEventArgs(pbControlStatus.DataListControlType, "EventPageIndexChanging", null, pageIndex) });

                PropertyInfo pi = type.GetProperty("PageIndex");

                if (pi != null)
                {
                    if (pi.CanWrite)
                    {
                        pi.SetValue(this._BoundControl, pageIndex, null);

                        MethodInfo miChanged = type.GetMethod("OnPageIndexChanged", BindingFlags.Instance | BindingFlags.NonPublic);

                        if (miChanged != null)
                            miChanged.Invoke(this._BoundControl, new object[] { new EventArgs() });
                    }
                }
            }

            //��������DataGrid 
            if (this._Control != null)
            {
                System.Type type = this._Control.GetType();
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
            if (this._Control != null)
            {
                System.Type type = this._Control.GetType();
                IPagerBoundControlType pagerControlType = new PagerBoundControlType();
                PagerBoundControlStatus pbControlStatus = new PagerBoundControlStatus();

                pbControlStatus = pagerControlType.GetPagerBoundControl(type);

                MethodInfo mi = type.GetMethod("OnPageIndexChanged", BindingFlags.Instance | BindingFlags.NonPublic);

                IPageEventArgs eventArgs = new PageEventArgs();

                mi.Invoke(this._Control, new object[] { eventArgs.GetPageEventArgs(pbControlStatus.DataListControlType, "EventPageIndexChanged", (object)this._Control, pageIndex) });
                PropertyInfo pi = type.GetProperty("CurrentPageIndex");

                if (pi != null)
                    if (pi.CanWrite)
                        pi.SetValue(this._Control, pageIndex, null);
            }
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
}