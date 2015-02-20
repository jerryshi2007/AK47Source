using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.Design;
using System.Drawing;
using System.Drawing.Design;
using System.Reflection;
using System.Web.UI;
using System.Web.UI.HtmlControls;
using System.Web.UI.WebControls;
using MCS.Library.Core;
using MCS.Library.Data.Builder;
using MCS.Library.Data.Mapping;
using MCS.Library.Globalization;
using MCS.Library.OGUPermission;
using MCS.Library.Principal;
using MCS.Library.SOA.DataObjects;
using MCS.Web.Library;
using MCS.Web.Library.Script;

[assembly: WebResource("MCS.Web.WebControls.DeluxeSearch.DeluxeSearch.js", "text/javascript")]
[assembly: WebResource("MCS.Web.WebControls.DeluxeSearch.DeluxeSearch.css", "text/css", PerformSubstitution = true)]

[assembly: WebResource("MCS.Web.WebControls.DeluxeSearch.Images.search_icon.png", "image/png")]
[assembly: WebResource("MCS.Web.WebControls.DeluxeSearch.Images.search_btn.png", "image/png")]
[assembly: WebResource("MCS.Web.WebControls.DeluxeSearch.Images.btn_left.png", "image/png")]
[assembly: WebResource("MCS.Web.WebControls.DeluxeSearch.Images.btn_right.png", "image/png")]
[assembly: WebResource("MCS.Web.WebControls.DeluxeSearch.Images.del.png", "image/png")]
[assembly: WebResource("MCS.Web.WebControls.DeluxeSearch.Images.line.png", "image/png")]
namespace MCS.Web.WebControls
{
    /// <summary>
    /// 用于通用查询界面的控件
    /// </summary>
    [RequiredScript(typeof(ControlBaseScript), 1)]
    [RequiredScript(typeof(HBCommonScript), 2)]
    [ClientCssResource("MCS.Web.WebControls.DeluxeSearch.DeluxeSearch.css")]
    [ClientScriptResource("MCS.Web.WebControls.DeluxeSearch", "MCS.Web.WebControls.DeluxeSearch.DeluxeSearch.js")]
    [Designer(typeof(DeluxeSearchDesigner), typeof(IDesigner))]
    public class DeluxeSearch : ScriptControlBase
    {
        public DeluxeSearch()
            : base(true, HtmlTextWriterTag.Div)
        {
            JSONSerializerExecute.RegisterConverter(typeof(WhereSqlClauseBuilderConverter));
        }

        #region Fields

        private const string DEFAULTCSSNAME = "deluxe-search deluxe-center";
        private Control _advanceSearchTargetControl = new Control();
        private TextBox _inputBox;
        private HtmlGenericControl _lblSearchTip;
        private Button _searchButton;
        private HtmlGenericControl _advancedSearchButton;
        private HtmlGenericControl _advancedSearchContainer;
        private HtmlGenericControl _customConditonContainer;
        private HtmlGenericControl _categoryContainer;
        private HtmlGenericControl _divSearchBar;
        private HtmlGenericControl _divSearchPanel;
        private HtmlGenericControl _divSearchFiled;
        private HtmlGenericControl _divSearchTips;
        private HtmlGenericControl _divClear;
        private HtmlGenericControl _contentTitle;
        private HtmlGenericControl _categoryTable;
        private HtmlButton _showButton = new HtmlButton() { ID = "showBtn" };
        private HtmlAnchor _closeLinkButton = new HtmlAnchor() { ID = "closeLinkButton", HRef = "###", InnerText = "关闭" };
        private HtmlButton _hideButton = new HtmlButton() { ID = "hideBtn" };

        private CheckBox _saveConditionCheckBox = new CheckBox();
        private TextBox _conditionNameBox = new TextBox();
        private ClientGrid _clientGridConditions = new ClientGrid();

        public event EventHandler InitCategories;
        public event EventHandler<SearchEventArgs> Searching;
        private readonly DeluxeSearchCategoryCollection _categories = new DeluxeSearchCategoryCollection();

        private readonly List<WhereSqlClauseBuilder> _whereSqlClauses = new List<WhereSqlClauseBuilder>();
        private readonly UserCustomSearchConditionCollection _UserCustomSearchConditions = new UserCustomSearchConditionCollection();

        internal static readonly string ClearWhereCondition = "ClearWhereCondition";
        #endregion

        #region Properties

        /// <summary>
        /// 控件ClientID
        /// </summary>
        [Browsable(false)]
        [ScriptControlProperty]
        [ClientPropertyName("controlClientID")]
        private string ControlClientID
        {
            get { return this.ClientID; }
        }

        /// <summary>
        /// 搜索文本框提示内容
        /// </summary>
        [Browsable(false)]
        [ScriptControlProperty]
        [ClientPropertyName("labelSearchTipClientID")]
        private string LabelSearchTipClientID
        {
            get { return this._lblSearchTip.ClientID; }
        }

        /// <summary>
        /// 搜索文本框ClientID
        /// </summary>
        [Browsable(false)]
        [ScriptControlProperty]
        [ClientPropertyName("searchInputClientID")]
        private string SearchInputClientID
        {
            get { return this._inputBox.ClientID; }
        }

        /// <summary>
        /// 显示搜索条件列表按钮客户端ID
        /// </summary>
        [Browsable(false)]
        [ScriptControlProperty]
        [ClientPropertyName("showButtonClientID")]
        private string ShowButtonClientID
        {
            get { return this._showButton.ClientID; }
        }

        /// <summary>
        /// 隐藏搜索条件列表按钮客户端ID
        /// </summary>
        [Browsable(false)]
        [ScriptControlProperty]
        [ClientPropertyName("hideButtonClientID")]
        private string HideButtonClientID
        {
            get { return this._hideButton.ClientID; }
        }

        /// <summary>
        /// 自定义搜索条件容器客户端ID
        /// </summary>
        [Browsable(false)]
        [ScriptControlProperty]
        [ClientPropertyName("customConditionClientID")]
        private string CustomConditionClientID
        {
            get { return _customConditonContainer == null ? "" : this._customConditonContainer.ClientID; }
        }

        /// <summary>
        /// 高级搜索按钮ClientID
        /// </summary>
        [Browsable(false)]
        [ScriptControlProperty]
        [ClientPropertyName("advancedSearchClientID")]
        private string AdvancedSearchClientID
        {
            get { return this._advancedSearchButton.ClientID; }
        }

        /// <summary>
        /// 高级搜索关闭按钮ClientID
        /// </summary>
        [Browsable(false)]
        [ScriptControlProperty]
        [ClientPropertyName("closeLinkButtonClientID")]
        private string CloseLinkButtonClientID
        {
            get { return this._closeLinkButton.ClientID; }
        }

        /// <summary>
        /// 搜索按钮ClientID
        /// </summary>
        [Browsable(false)]
        [ScriptControlProperty]
        [ClientPropertyName("searchButtonClientID")]
        private string SearchButtonClientID
        {
            get { return this._searchButton.ClientID; }
        }

        /// <summary>
        /// ClientGridClientID
        /// </summary>
        [Browsable(false)]
        [ScriptControlProperty]
        [ClientPropertyName("clientGridClientID")]
        private string ClientGridClientID
        {
            get { return this._clientGridConditions.ClientID; }
        }

        /// <summary>
        /// 搜索框中输入的值
        /// </summary>
        [Browsable(true), Description("搜索框中输入的值"), Category("Default"), DefaultValue("")]
        public string InputText
        {
            get { return GetPropertyValue("InputText", string.Empty); }
            set { SetPropertyValue("InputText", value); }
        }

        /// <summary>
        /// 搜索框对应的字段
        /// </summary>
        [DefaultValue("")]
        [Browsable(true), Description("搜索框对应的字段"), Category("Default")]
        public string SearchField
        {
            get { return GetPropertyValue("SearchField", string.Empty); }
            set { SetPropertyValue("SearchField", value); }
        }

        /// <summary>
        /// 高级搜索链接文本
        /// </summary>        
        [DefaultValue("高级搜索")]
        [Browsable(true), Description("高级搜索链接文本"), Category("Default")]
        public string AdvancedSearchText
        {
            get { return GetPropertyValue("AdvancedSearchText", Translator.Translate(Define.DefaultCulture, "高级搜索")); }
            set { SetPropertyValue("AdvancedSearchButtonText", value); }
        }

        /// <summary>
        /// 高级搜索按钮的样式
        /// </summary>
        [ScriptControlProperty]
        [DefaultValue("advancedSearchbutton")]
        [ClientPropertyName("advancedSearchCss")]
        [Browsable(true), Description("高级搜索按钮的样式"), Category("Appearance")]
        public string AdvancedSearchCss
        {
            get { return GetPropertyValue("AdvancedSearchCss", "advancedSearchbutton"); }
            set { SetPropertyValue("AdvancedSearchCss", value); }
        }

        [DefaultValue("")]
        [ScriptControlEvent]
        [ClientPropertyName("conditionClick")]
        [Bindable(true), Category("ClientEventsHandler"), Description("自定义搜索条件单击事件")]
        public string OnConditionClick
        {
            get
            {
                return GetPropertyValue("OnConditionClick", string.Empty);
            }
            set
            {
                SetPropertyValue("OnConditionClick", value);
            }
        }

        /// <summary>
        /// 删除搜索条件按钮Url
        /// </summary>
        [Browsable(false)]
        [ScriptControlProperty]
        [ClientPropertyName("deleteButtonImageUrl")]
        private string DeleteButtonImageUrl
        {
            get { return Page.ClientScript.GetWebResourceUrl(typeof(DeluxeSearch), "MCS.Web.WebControls.DeluxeSearch.Images.del.png"); }
        }

        /// <summary>
        /// 搜索按钮的背景图片
        /// </summary>
        [Browsable(true), Description("搜索按钮的背景图片"), Category("Appearance")]
        [Editor("System.Web.UI.Design.ImageUrlEditor, System.Design", typeof(UITypeEditor))]
        public string SearchImageUrl
        {
            get
            {
                return GetPropertyValue("SearchImageUrl", string.Empty);
            }
            set
            {
                SetPropertyValue("SearchImageUrl", value);
            }
        }

        /// <summary>
        /// 选中标签的背景色
        /// </summary>
        [DefaultValue(typeof(Color))]
        [Browsable(true), Description("选中标签的背景色"), Category("Appearance")]
        public Color SelectedLinkColor
        {
            get
            {
                return GetPropertyValue("SelectedLinkColor", Color.FromArgb(0, 200, 200, 200));
            }
            set
            {
                SetPropertyValue("SelectedLinkColor", value);
            }
        }

        /// <summary>
        /// 提示图片
        /// </summary>
        [Browsable(true), Description("提示图片"), Category("Appearance")]
        [Editor("System.Web.UI.Design.ImageUrlEditor, System.Design", typeof(UITypeEditor))]
        public string TipImageUrl
        {
            get
            {
                return GetPropertyValue("TipImageUrl", string.Empty);
            }
            set
            {
                SetPropertyValue("TipImageUrl", value);
            }
        }

        /// <summary>
        /// 搜索按钮的样式 
        /// </summary>        
        [DefaultValue("deluxe-search-button")]
        [Browsable(true), Description("搜索按钮的样式"), Category("Appearance")]
        public string SearchCss
        {
            get { return GetPropertyValue("SearchCss", "deluxe-search-button"); }
            set { SetPropertyValue("SearchCss", value); }
        }

        /// <summary>
        /// 搜索输入框的样式 
        /// </summary>        
        [DefaultValue("deluxe-search-content")]
        [Browsable(true), Description("搜索输入框的样式"), Category("Appearance")]
        public string InputCss
        {
            get { return GetPropertyValue("InputCss", "deluxe-search-content"); }
            set { SetPropertyValue("InputCss", value); }
        }

        /// <summary>
        /// 搜索框容器的样式 
        /// </summary>        
        [DefaultValue("deluxe-search-searchbar")]
        [Browsable(true), Description("搜索框容器的样式"), Category("Appearance")]
        public string SearchbarCss
        {
            get { return GetPropertyValue("SearchbarCss", "deluxe-search-searchbar"); }
            set { SetPropertyValue("SearchbarCss", value); }
        }

        /// <summary>
        /// 搜索框的样式 
        /// </summary>        
        [DefaultValue("deluxe-search-panel")]
        [Browsable(true), Description("搜索框的样式"), Category("Appearance")]
        public string SearchPanelCss
        {
            get { return GetPropertyValue("SearchPanelCss", "deluxe-search-panel"); }
            set { SetPropertyValue("SearchPanelCss", value); }
        }

        /// <summary>
        /// 输入搜索区域的样式 
        /// </summary>
        [ScriptControlProperty]
        [ClientPropertyName("searchFieldCss")]
        [DefaultValue("deluxe-search-panel-fields")]
        [Browsable(true), Description("输入搜索区域的样式"), Category("Appearance")]
        public string SearchFieldCss
        {
            get { return GetPropertyValue("SearchFieldCss", "deluxe-search-panel-fields"); }
            set { SetPropertyValue("SearchFieldCss", value); }
        }

        /// <summary>
        /// 高级搜索区域的样式 
        /// </summary>        
        [DefaultValue("deluxe-search-tips")]
        [Browsable(true), Description("高级搜索区域的样式"), Category("Appearance")]
        public string AdvanceSearchFieldCss
        {
            get { return GetPropertyValue("AdvanceSearchFieldCss", "deluxe-search-tips"); }
            set { SetPropertyValue("AdvanceSearchFieldCss", value); }
        }

        /// <summary>
        /// 分类查搜索中每一个表可的样式 
        /// </summary>        
        [DefaultValue("item")]
        [Browsable(true), Description("分类查搜索中每一个表格的样式 "), Category("Appearance")]
        public string CellCss
        {
            get { return GetPropertyValue("CellCss", "item"); }
            set { SetPropertyValue("CellCss", value); }
        }

        /// <summary>
        /// 分类查搜索中每一个表格单元头的样式 
        /// </summary>        
        [DefaultValue("head")]
        [Browsable(true), Description("分类查搜索中每一个表格单元头的样式 "), Category("Appearance")]
        public string CellHeaderCss
        {
            get { return GetPropertyValue("CellHeaderCss", "head"); }
            set { SetPropertyValue("CellHeaderCss", value); }
        }

        /// <summary>
        /// 分类搜索中超链接的样式 
        /// </summary>        
        [DefaultValue("")]
        [Browsable(true), Description("分类搜索中超链接的样式"), Category("Appearance")]
        public string LinkCss
        {
            get { return GetPropertyValue("LinkCss", ""); }
            set { SetPropertyValue("LinkCss", value); }
        }

        /// <summary>
        /// 数据源最外DiV的样式 
        /// </summary>        
        [DefaultValue("deluxe-search-category-content")]
        [Browsable(true), Description("数据源最外DiV的样式"), Category("Appearance")]
        public string CategoryContainerCss
        {
            get { return GetPropertyValue("CategoryContainerCss", "deluxe-search-category-content"); }
            set { SetPropertyValue("CategoryContainerCss", value); }
        }

        /// <summary>
        /// 数据源分类头样式
        /// </summary>        
        [DefaultValue("deluxe-search-classify")]
        [Browsable(true), Description("数据源分类头样式"), Category("Appearance")]
        public string CategoryHeadCss
        {
            get { return GetPropertyValue("CategoryHeadCss", "deluxe-search-classify"); }
            set { SetPropertyValue("CategoryHeadCss", value); }
        }

        /// <summary>
        /// 数据源分类样式
        /// </summary>        
        [DefaultValue("deluxe-search-tab")]
        [Browsable(true), Description("数据源分类头样式"), Category("Appearance")]
        public string CategoryCss
        {
            get { return GetPropertyValue("CategoryCss", "deluxe-search-tab"); }
            set { SetPropertyValue("CategoryCss", value); }
        }

        /// <summary>
        /// 高级搜索容器的样式 
        /// </summary>
        [DefaultValue("deluxe-search-advance-container")]
        [Browsable(true), Description("高级搜索容器的样式"), Category("Appearance")]
        public string AdvanceSearchContainerCss
        {
            get { return GetPropertyValue("AdvanceSearchContainerCss", "deluxe-search-advance-container"); }
            set { SetPropertyValue("AdvanceSearchContainerCss", value); }
        }

        /// <summary>
        /// 高级搜索盒子样式
        /// </summary>
        [DefaultValue("deluxe-search-advanced-box")]
        [Browsable(true), Description("高级搜索盒子样式"), Category("Appearance")]
        public string AdvanceSearchBoxCss
        {
            get { return GetPropertyValue("AdvanceSearchBoxCss", "deluxe-search-advanced-box"); }
            set { SetPropertyValue("AdvanceSearchBoxCss", value); }
        }

        /// <summary>
        /// OfficeViewer宽
        /// </summary>
        [Category("Appearance")]
        public Unit AdvanceSearchContainerWidth
        {
            get
            {
                return GetPropertyValue<Unit>("AdvanceSearchContainerWidth", Unit.Pixel(500));
            }
            set
            {
                SetPropertyValue<Unit>("AdvanceSearchContainerWidth", value);
            }
        }

        /// <summary>
        /// OfficeViewer高
        /// </summary>
        [Category("Appearance")]
        public Unit AdvanceSearchContainerHeight
        {
            get
            {
                return GetPropertyValue<Unit>("AdvanceSearchContainerHeight", Unit.Empty);
            }
            set
            {
                SetPropertyValue<Unit>("AdvanceSearchContainerHeight", value);
            }
        }

        /// <summary>
        /// 不限链接的默认显示文本（默认为不限）
        /// </summary>        
        [DefaultValue("不限")]
        [Browsable(true), Description("不限链接的默认显示文本"), Category("Appearance")]
        public string AllText
        {
            get { return GetPropertyValue("AllText", Translator.Translate(Define.DefaultCulture, "不限")); }
            set { SetPropertyValue("AllText", value); }
        }

        /// <summary>
        ///默认提示
        /// </summary>                
        [ScriptControlProperty]
        [DefaultValue("请输入内容")]
        [ClientPropertyName("defaultTip")]
        [Browsable(true), Description("默认提示"), Category("Appearance")]
        public string DefaultTip
        {
            get { return GetPropertyValue("DefaultTip", Translator.Translate(Define.DefaultCulture, "请输入内容")); }
            set { SetPropertyValue("DefaultTip", value); }
        }

        /// <summary>
        /// 每个列的分隔符
        /// </summary>
        [DefaultValue(":")]
        [Browsable(true), Description("每个列的分隔符"), Category("Appearance")]
        public string Separator
        {
            get { return GetPropertyValue("Separator", ":"); }
            set { SetPropertyValue("Separator", value); }
        }

        /// <summary>
        /// 是否显示分类搜索
        /// </summary>        
        [DefaultValue(true)]
        [Browsable(true), Description("是否显示分类搜索"), Category("Appearance")]
        public bool HasCategory
        {
            get { return GetPropertyValue("HasCategory", true); }
            set { SetPropertyValue("HasCategory", value); }
        }

        /// <summary>
        /// 是否显示高级搜索
        /// </summary>        
        [DefaultValue(false)]
        [Browsable(true), Description("是否显示高级搜索"), Category("Appearance")]
        public bool HasAdvanced
        {
            get { return GetPropertyValue("HasAdvanced", false); }
            set { SetPropertyValue("HasAdvanced", value); }
        }

        protected override bool AutoClearClientStateFieldValue
        {
            get
            {
                return false;
            }
        }

        /// <summary>
        /// Where语句集合
        /// </summary>
        [Browsable(false), Description("Where SQL语句集合")]
        private List<WhereSqlClauseBuilder> WhereSqlClauses
        {
            get { return GetPropertyValue("WhereSqlClauses", this._whereSqlClauses); }
            set { SetPropertyValue("WhereSqlClauses", value); }
        }

        /// <summary>
        /// 自定义搜索条件集合
        /// </summary>
        [Browsable(false), Description("自定义搜索条件集合")]
        public UserCustomSearchConditionCollection UserCustomSearchConditions
        {
            get { return GetPropertyValue("UserCustomSearchConditions", this._UserCustomSearchConditions); }
            set { SetPropertyValue("UserCustomSearchConditions", value); }
        }

        /// <summary>
        /// 高级搜索条件对象
        /// </summary>
        public bool IsAdvanceSearching
        {
            get { return GetPropertyValue("IsAdvanceSearching", false); }
            private set { SetPropertyValue("IsAdvanceSearching", value); }
        }

        /// <summary>
        /// 用户
        /// </summary>
        [ClientPropertyName("user")]
        public IUser User
        {
            get
            {
                if (DeluxePrincipal.IsAuthenticated)
                {
                    var user = new OguUser(DeluxeIdentity.CurrentRealUser);
                    return user;
                }
                else
                {
                    return new OguUser() { ID = "ID", DisplayName = "Name" };//todo deluxe search test
                    //return null;
                }
            }
        }

        /// <summary>
        /// 高级搜索容器客户端ID
        /// </summary>
        [DefaultValue("")]
        [ScriptControlProperty, ClientPropertyName("advanceSearchContainerClientID")]
        private string AdvanceSearchContainerClientID
        {
            get
            {
                return this._advancedSearchContainer == null ? "" : this._advancedSearchContainer.ClientID;
            }
        }

        [ScriptControlProperty, ClientPropertyName("categoryContainerClientID")]
        private string CategoryContainerClientID
        {
            get { return this._categoryContainer == null ? "" : this._categoryContainer.ClientID; }
        }

        /// <summary>
        /// 自定义搜索容器客户端ID
        /// </summary>
        [DefaultValue("")]
        [Browsable(true), Bindable(true), Description("自定义搜索容器客户端ID")]
        [IDReferenceProperty()]
        public string CustomSearchContainerControlID
        {
            get
            {
                return GetPropertyValue("CustomSearchContainerControlID", "");
            }
            set
            {
                SetPropertyValue("CustomSearchContainerControlID", value);
            }
        }

        [ScriptControlProperty, ClientPropertyName("customSearchContainerClientID")]
        private string CustomSearchContainerClientID
        {
            get { return this._advanceSearchTargetControl == null ? "" : this._advanceSearchTargetControl.ClientID; }
        }

        /// <summary>
        /// 高级搜索条件类型
        /// </summary>
        [Browsable(false), Bindable(true), Description("高级搜索条件类型")]
        public string AdvanceSearchConditionTypeName
        {
            get { return GetPropertyValue("AdvanceSearchConditionTypeName", string.Empty); }
            set { SetPropertyValue("AdvanceSearchConditionTypeName", value); }
        }

        [DefaultValue("")]
        [Description("如果指定了SearchField,并且当且的SearchMethod是FullText，那么这个属性可以指定生成Condition的Pattern，主要用于全文索引时设置CONTAINS(${DataField}$, ${Data}$)")]
        [Bindable(true)]
        public string SearchFieldTemplate
        {
            get { return GetPropertyValue("SearchFieldTemplate", string.Empty); }
            set { SetPropertyValue("SearchFieldTemplate", value); }
        }

        [DefaultValue(DeluxeSearchMethodType.FullText)]
        [Description("搜索区域搜索方式类型，有=、LIKE，和全文索引三种方式")]
        [Bindable(true)]
        public DeluxeSearchMethodType SearchMethod
        {
            get { return GetPropertyValue("SearchMethod", DeluxeSearchMethodType.FullText); }
            set { SetPropertyValue("SearchMethod", value); }
        }

        /// <summary>
        /// 搜索分类
        /// </summary>
        [PersistenceMode(PersistenceMode.InnerProperty), Description("搜索分类")]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
        [Browsable(false)]
        public DeluxeSearchCategoryCollection Categories
        {
            get
            {
                return this._categories;
            }
        }
        #endregion

        #region Events

        /// <summary>
        /// 处理Repeater控件中的事件
        /// </summary>
        /// <param name="source"></param>
        /// <param name="e"></param>
        private void RepeaterItemCommand(object source, RepeaterCommandEventArgs e)
        {
            var args = new SearchEventArgs(e.CommandName, e.CommandArgument);

            if (args.ConditionKey.IsNotEmpty() && null != args.ConditionValue)
            {
                ReplaceWhereItem(args.ConditionKey, args.ConditionValue, false);
            }

            DealWithSearchField();

            OnSearching(source, args);
        }

        /// <summary>
        /// 初始化数据
        /// </summary>
        /// <param name="eventArgs"></param>
        protected override void OnInit(EventArgs eventArgs)
        {
            OnInitCategories(this, eventArgs);

            base.OnInit(eventArgs);
        }

        /// <summary>
        /// 初始化标签类数据源
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="eventArgs"></param>
        protected virtual void OnInitCategories(object sender, EventArgs eventArgs)
        {
            if (InitCategories != null)
            {
                InitCategories(sender, eventArgs);
            }
        }

        /// <summary>
        /// 搜索事件方法
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="eventArgs"></param>
        private void SearchButtonClick(object sender, EventArgs eventArgs)
        {
            if (this._inputBox != null)
            {
                if (SearchField.IsNotEmpty() && InputText.IsNotEmpty())
                    ReplaceWhereItem(SearchField, InputText, SearchFieldTemplate, true);
            }

            DealWithSearchField();

            var newArgs = new SearchEventArgs();

            if (this.HasAdvanced)
            {
                newArgs.IsSaveCondition = this._saveConditionCheckBox.Checked;
                newArgs.ConditionName = this._conditionNameBox.Text;
            }

            OnSearching(sender, newArgs);
        }

        private void DealWithSearchField()
        {
            InputText = this._inputBox.Text;

            if (SearchField.IsNotEmpty())
            {
                if (InputText.IsNotEmpty())
                    ReplaceWhereItem(SearchField, InputText, SearchFieldTemplate, true);
                else
                {
                    WhereSqlClauses.RemoveAll(item =>
                    {
                        var curItem = (WhereSqlClauseBuilder)item;
                        if (curItem.Count > 0 && ((SqlClauseBuilderItemIUW)curItem[0]).DataField == SearchField)
                        {
                            return true;
                        }
                        return false;
                    });
                }
            }
        }

        public override void RenderBeginTag(HtmlTextWriter writer)
        {
            var cssClass = string.IsNullOrEmpty(this.CssClass) ? DEFAULTCSSNAME : this.CssClass;
            writer.AddAttribute(HtmlTextWriterAttribute.Class, cssClass);
            base.RenderBeginTag(writer);
        }

        protected override void OnPreRender(EventArgs e)
        {
            base.OnPreRender(e);
            SetChildControlStyle();
            this._saveConditionCheckBox.Checked = false;
            this._conditionNameBox.Text = "";
            SetSelectedLinkStyle();

            _categoryContainer.Visible = HasCategory;
        }

        /// <summary>
        /// 设置选中的标签样式
        /// </summary>
        private void SetSelectedLinkStyle()
        {
            foreach (var categoryItem in Categories)
            {
                var currentCategory = FindControl(categoryItem.DataValueField);
                if (null != currentCategory)
                {
                    foreach (var sqlClause in WhereSqlClauses)
                    {
                        var whereClause = sqlClause as WhereSqlClauseBuilder;
                        if (whereClause != null)
                        {
                            if (whereClause.Count > 0)
                            {
                                foreach (var whereItem in whereClause)
                                {
                                    var builderItemIuw = ((SqlClauseBuilderItemIUW)whereItem);
                                    Control linkButton = currentCategory.FindControlByID(string.Format("{0}_{1}", builderItemIuw.DataField, builderItemIuw.Data), true);
                                    if (null != linkButton) (linkButton as LinkButton).BackColor = SelectedLinkColor;
                                }
                            }
                        }
                    }
                }
            }
        }

        protected virtual void OnSearching(object sender, SearchEventArgs eventArgs)
        {
            if (null != Searching)
                Searching(sender, eventArgs);
        }

        /// <summary>
        /// OnLoad
        /// </summary>
        /// <param name="e"></param>
        protected override void OnLoad(EventArgs e)
        {
            foreach (var category in Categories)
            {
                var repeater = FindControl(category.DataValueField) as Repeater;
                if (repeater != null)
                {
                    repeater.ItemCommand += RepeaterItemCommand;
                    repeater.DataBind();
                }
            }
        }

        private void SetChildControlStyle()
        {
            _inputBox.Attributes["class"] = InputCss;
            _searchButton.Attributes["class"] = SearchCss;
            _divSearchBar.Attributes["class"] = SearchbarCss;
            _divSearchPanel.Attributes["class"] = SearchPanelCss;
            _divSearchFiled.Attributes["class"] = SearchFieldCss;

            if (_divSearchTips != null)
            {
                _divSearchTips.Attributes["class"] = AdvanceSearchFieldCss;
            }

            if (_divClear != null)
            {
                _divClear.Attributes["class"] = AdvanceSearchBoxCss;
            }

            if (_advancedSearchContainer != null)
            {
                _advancedSearchContainer.Style.Add(HtmlTextWriterStyle.Display, "none");
                _advancedSearchContainer.Style.Add(HtmlTextWriterStyle.Position, "absolute");
                _advancedSearchContainer.Style.Add(HtmlTextWriterStyle.Width, this.AdvanceSearchContainerWidth.ToString());

                if (this.AdvanceSearchContainerHeight != Unit.Empty)
                {
                    _advancedSearchContainer.Style.Add(HtmlTextWriterStyle.Height, this.AdvanceSearchContainerHeight.ToString());
                }

                _advancedSearchContainer.Attributes["class"] = this.AdvanceSearchContainerCss;
            }

            if (_customConditonContainer != null)
            {
                _customConditonContainer.Style.Add(HtmlTextWriterStyle.Display, "none");
                _customConditonContainer.Style.Add(HtmlTextWriterStyle.Position, "absolute");
                _customConditonContainer.Style.Add("float", "left");
                _customConditonContainer.Style.Add(HtmlTextWriterStyle.TextAlign, "center");
                _customConditonContainer.Attributes["class"] = this.AdvanceSearchContainerCss + " bg_white";
            }

            if (_categoryContainer != null)
            {
                _categoryContainer.Attributes.Add("class", CategoryContainerCss);
            }

            if (_contentTitle != null)
            {
                _contentTitle.Attributes.Add("class", CategoryHeadCss);
            }

            if (_categoryTable != null)
            {
                _categoryTable.Attributes.Add("class", CategoryCss);
            }
        }

        /// <summary>
        /// 创建子控件
        /// </summary>
        protected override void CreateChildControls()
        {
            _inputBox = new TextBox { ID = "searchInput" };
            _searchButton = new Button { ID = "searchButton" };
            _searchButton.Click += SearchButtonClick;
            _advancedSearchButton = new HtmlGenericControl { TagName = "a", InnerText = AdvancedSearchText, ID = "advancedSearch" };
            _advancedSearchButton.Attributes.Add("href", "#");
            _advanceSearchTargetControl = this.Parent.FindControl(this.CustomSearchContainerControlID);

            CreateSearchBar();
            Controls.Add(this._divSearchBar);

            GenerateCategoryContainer();
            Controls.Add(this._categoryContainer);


            if (this.HasAdvanced)
            {
                _divClear = new HtmlGenericControl() { TagName = "div" };
                this._divSearchBar.Controls.Add(_divClear);
                GenerateAdvanceContainer();
                _divClear.Controls.Add(this._advancedSearchContainer);

                if (this.User != null)
                {
                    GenarateCustomContainer();
                    _divClear.Controls.Add(this._customConditonContainer);
                }
            }
        }

        private void CreateSearchBar()
        {
            _divSearchBar = new HtmlGenericControl() { TagName = "div" };
            _divSearchPanel = new HtmlGenericControl() { TagName = "div" };
            _divSearchBar.Controls.Add(_divSearchPanel);

            _divSearchFiled = new HtmlGenericControl() { TagName = "div" };
            _divSearchPanel.Controls.Add(_divSearchFiled);
            _divSearchPanel.Controls.Add(_searchButton);

            _lblSearchTip = new HtmlGenericControl() { TagName = "label", ID = "searchTip", InnerText = DefaultTip };
            _divSearchFiled.Controls.Add(_lblSearchTip);
            _divSearchFiled.Controls.Add(_inputBox);

            _divSearchFiled.Controls.Add(new HtmlGenericControl("s"));

            if (HasAdvanced)
            {
                _divSearchTips = new HtmlGenericControl() { TagName = "div" };
                _divSearchTips.Controls.Add(_advancedSearchButton);
                _divSearchBar.Controls.Add(_divSearchTips);
            }
        }

        private void GenerateAdvanceContainer()
        {
            _advancedSearchContainer = new HtmlGenericControl() { TagName = "div", ID = "advancedSearchContainer" };
            var mainDiv = new HtmlGenericControl() { TagName = "div", ID = "containerMain" };
            _advancedSearchContainer.Controls.Add(mainDiv);

            if (this.User != null)
            {
                var hr = new HtmlGenericControl("hr");
                hr.Attributes["size"] = "1";
                hr.Attributes["class"] = "line";

                var footerDiv = new HtmlGenericControl("div");
                footerDiv.Attributes.Add("class", "search_save");
                var conditionTable = new HtmlGenericControl("table");
                conditionTable.Style.Add(HtmlTextWriterStyle.Width, "100%");

                GenarateConditionRow(conditionTable);

                footerDiv.Controls.Add(conditionTable);
                _advancedSearchContainer.Controls.Add(hr);
                _advancedSearchContainer.Controls.Add(footerDiv);
            }
        }

        private void GenarateCustomContainer()
        {
            _customConditonContainer = new HtmlGenericControl() { TagName = "div", ID = "customConditonContainer" };

            HtmlGenericControl h2 = new HtmlGenericControl() { TagName = "h2", InnerText = "-- 保存搜索列表 --" };
            _customConditonContainer.Controls.Add(h2);
            _customConditonContainer.Controls.Add(this._clientGridConditions);
            _clientGridConditions.AllowPaging = true;
            _clientGridConditions.AutoPaging = true;
            _clientGridConditions.PageSize = 5;
            _clientGridConditions.ID = "gridConditions";

            _clientGridConditions.CssClass = "tb_condition";

            _clientGridConditions.Columns.Add(new ClientGridColumn()
            {
                HeaderText = "搜索名称",
                DataField = "ConditionName",
                HeaderStyle = "{textAlign: 'center'}",
                ItemStyle = "{textAlign: 'center'}",
                EditorStyle = "{width:'90%',textAlign: 'center'}",
                EditTemplate = new ClientGridColumnEditTemplate()
                {
                    EditMode = ClientGridColumnEditMode.A,
                    HrefFieldOfA = "#",
                    TargetOfA = "_self"
                }

            });
            _clientGridConditions.Columns.Add(new ClientGridColumn()
            {
                HeaderText = "操作",
                DataField = "ID",
                HeaderStyle = "{textAlign: 'center',width:'50px'}",
                ItemStyle = "{textAlign: 'center'}",
                EditorStyle = "{width:'90%',textAlign: 'center'}",
                EditTemplate = new ClientGridColumnEditTemplate()
                {
                    EditMode = ClientGridColumnEditMode.A,
                    HrefFieldOfA = "#",
                    TargetOfA = "_self"
                }
            });

            HtmlGenericControl divfoot = new HtmlGenericControl() { TagName = "div" };
            divfoot.Style.Add(HtmlTextWriterStyle.TextAlign, "left");
            divfoot.Attributes["class"] = "hidefooter";
            _hideButton.Attributes["onclick"] = "return false;";
            _hideButton.Attributes["class"] = "hide";

            divfoot.Controls.Add(_hideButton);
            _customConditonContainer.Controls.Add(divfoot);
        }

        private void GenarateConditionRow(HtmlGenericControl table)
        {
            var tableRow = new HtmlTableRow();
            var cell1 = new HtmlTableCell();
            var cell2 = new HtmlTableCell();

            Label label = new Label { Text = "命名搜索名称" };

            _saveConditionCheckBox.Text = "搜索后保存";
            cell1.Controls.Add(this._saveConditionCheckBox);
            cell2.Controls.Add(label);
            cell2.Controls.Add(this._conditionNameBox);

            tableRow.Controls.Add(cell1);
            tableRow.Controls.Add(cell2);

            var tableRow1 = new HtmlTableRow();

            var tableCellClose = new HtmlTableCell();
            tableRow1.Controls.Add(tableCellClose);
            tableCellClose.Controls.Add(_closeLinkButton);

            var tableCellShow = new HtmlTableCell();
            tableCellShow.Attributes.Add("class", "align_r");
            tableRow1.Controls.Add(tableCellShow);


            _showButton.Attributes["class"] = "show";
            tableCellShow.Controls.Add(_showButton);

            table.Controls.Add(tableRow);
            table.Controls.Add(tableRow1);
        }

        private void GenerateCategoryContainer()
        {
            this._categoryContainer = new HtmlGenericControl() { TagName = "div", ID = "categoryContainer" };

            _contentTitle = new HtmlGenericControl() { TagName = "div" };
            _contentTitle.InnerText = "所有分类";
            this._categoryContainer.Controls.Add(_contentTitle);

            _categoryTable = new HtmlGenericControl("table");
            _categoryContainer.Controls.Add(_categoryTable);

            foreach (var category in Categories)
            {
                HtmlGenericControl repeaterRow = new HtmlGenericControl("tr");
                _categoryTable.Controls.Add(repeaterRow);

                var repeater = new Repeater
                {
                    ID = category.DataValueField,
                    DataSourceID = category.DataSourceID,
                    HeaderTemplate = new CategoryColumn(ListItemType.Header, category, CellHeaderCss, CellCss, LinkCss, Separator, AllText),
                    ItemTemplate = new CategoryColumn(ListItemType.Item, category, CellHeaderCss, CellCss, LinkCss, Separator, AllText),
                    ViewStateMode = ViewStateMode.Disabled
                };

                repeaterRow.Controls.Add(repeater);
            }
        }

        /// <summary>
        /// 加载ClientState
        /// </summary>
        /// <param name="clientState">客户端状态</param>
        protected override void LoadClientState(string clientState)
        {
            if (string.IsNullOrEmpty(clientState) == false)
            {
                var data = JSONSerializerExecute.Deserialize<object[]>(clientState);
                WhereSqlClauses = new List<WhereSqlClauseBuilder>();

                //这样反序列化居然有问题,LogicOperator居然没反序列化成功
                WhereSqlClauses = JSONSerializerExecute.Deserialize<List<WhereSqlClauseBuilder>>(data[0]);

                foreach (var clause in WhereSqlClauses)
                    clause.LogicOperator = LogicOperatorDefine.Or;

                if (this._inputBox != null)
                    InputText = this._inputBox.Text;

                //if (this._inputBox != null)
                //{
                //    InputText = this._inputBox.Text;

                //    if (SearchField.IsNotEmpty())
                //    {
                //        if (InputText.IsNotEmpty())
                //            ReplaceWhereItem(SearchField, InputText, SearchFieldTemplate);
                //    }s
                //}

                if (HasAdvanced)
                {
                    this.IsAdvanceSearching = JSONSerializerExecute.Deserialize<bool>(data[1]);

                    this.UserCustomSearchConditions = JSONSerializerExecute.Deserialize<UserCustomSearchConditionCollection>(data[2]);
                }
            }
        }

        /// <summary>
        /// 保存ClientState
        /// </summary>
        /// <returns>条件序列化的字符串</returns>
        protected override string SaveClientState()
        {
            string result = string.Empty;

            if (Page.IsCallback == false)
            {
                ArrayList array = new ArrayList { WhereSqlClauses, IsAdvanceSearching, UserCustomSearchConditions };
                result = JSONSerializerExecute.Serialize(array);
            }

            return result;
        }

        /// <summary>
        /// 增加Where子句的项
        /// </summary>
        /// <param name="searchField">Field</param>
        /// <param name="searchValue">Value</param>
        private void ReplaceWhereItem(string searchField, object searchValue, bool isInput)
        {
            ReplaceWhereItem(searchField, searchValue, string.Empty, isInput);
        }

        private void ReplaceWhereItem(string searchField, object searchValue, string template, bool isInput)
        {
            WhereSqlClauseBuilder whereSqlClause = WhereSqlClauses.Find(builder => builder.Exists(item => ((SqlClauseBuilderItemIUW)item).DataField == searchField));

            string searchText = searchValue.ToString();
            string formattedSearchText = TSqlBuilder.Instance.FormatFullTextString(LogicOperatorDefine.And, searchText);

            //找到了已经存在的和searchField字段匹配的WhereSqlClauseBuilder
            if (whereSqlClause != null)
            {
                if (searchText == DeluxeSearch.ClearWhereCondition)
                {
                    WhereSqlClauses.Remove(whereSqlClause);
                }
                else
                {
                    if (searchText.IsNotEmpty())
                    {
                        if (searchField == SearchField)
                        {
                            whereSqlClause.Clear();
                        }
                        else
                        {
                            whereSqlClause.Remove(item =>
                            {
                                var curItem = (SqlClauseBuilderItemIUW)item;
                                bool compareResult = false;

                                //如果是全文检索（有Template），则按照字段匹配删除，否则附加上值的匹配
                                if (this.SearchMethod == DeluxeSearchMethodType.FullText && template.IsNotEmpty())
                                    compareResult = curItem.DataField == searchField;
                                else
                                    compareResult = curItem.DataField == searchField && (curItem.Data.ToString() == searchText ||
                                                                        curItem.Data.ToString() == formattedSearchText);
                                return compareResult;
                            });
                        }

                        FillSearchFieldWhereSqlClauseBuilder(whereSqlClause, searchField, searchText, template, isInput);
                    }
                }
            }
            else if (searchText.IsNotEmpty() && searchText != DeluxeSearch.ClearWhereCondition)
            {
                whereSqlClause = new WhereSqlClauseBuilder();

                FillSearchFieldWhereSqlClauseBuilder(whereSqlClause, searchField, searchText, template, isInput);

                WhereSqlClauses.Add(whereSqlClause);
            }
        }

        private void FillSearchFieldWhereSqlClauseBuilder(WhereSqlClauseBuilder builder, string searchField, string searchText, string template, bool isInput)
        {
            builder.LogicOperator = LogicOperatorDefine.Or;

            if (isInput)
            {
                if (SearchMethod == DeluxeSearchMethodType.FullText && template.IsNotEmpty())
                {
                    string formattedSearchText = TSqlBuilder.Instance.FormatFullTextString(LogicOperatorDefine.And, searchText);

                    if (formattedSearchText.IsNotEmpty())
                    {
                        builder.AppendItem(searchField,
                                              TSqlBuilder.Instance.FormatFullTextString(LogicOperatorDefine.And, formattedSearchText), string.Empty, template);
                    }
                    else
                        if (formattedSearchText != searchText)
                        {
                            //formattedSearchText为空，但是原始的SearchText有内容，则执行空查询
                            builder.AppendItem(searchField, searchField, "<>", true);
                        }
                }
                else if (SearchMethod == DeluxeSearchMethodType.Equal)
                {
                    builder.AppendItem(searchField, searchText, "=", template);
                }
                else if (SearchMethod == DeluxeSearchMethodType.Like)
                {
                    builder.AppendItem(searchField, "%" + searchText + "%", "LIKE");
                }
                else if (SearchMethod == DeluxeSearchMethodType.PrefixLike)
                {
                    builder.AppendItem(searchField, searchText + "%", "LIKE");
                }
                else if (SearchMethod == DeluxeSearchMethodType.SurffixLike)
                {
                    builder.AppendItem(searchField, "%" + searchText, "LIKE");
                }
            }
            else
            {
                builder.AppendItem(searchField, searchText, "=", template);
            }
        }

        /// <summary>
        /// 获得条件
        /// </summary>
        /// <returns></returns>
        public ConnectiveSqlClauseCollection GetCondition()
        {
            ConnectiveSqlClauseCollection result = new ConnectiveSqlClauseCollection(LogicOperatorDefine.And);

            foreach (var whereSqlClause in WhereSqlClauses)
                result.Add(whereSqlClause);

            return result;
        }

        /// <summary>
        /// 清空条件
        /// </summary>
        public void ClearWhereSqlClauses()
        {
            this.WhereSqlClauses.Clear();
        }

        /// <summary>
        /// 删除保存的条件
        /// </summary>
        [ScriptControlMethod]
        public string DeleteCustomCondition(string conditionID)
        {
            UserCustomSearchConditionAdapter.Instance.Delete(c => c.AppendItem("ID", conditionID));
            return conditionID;
        }

        #endregion
    }

    /// <summary>
    /// 搜索区域搜索方式类型，有=、LIKE，和全文索引三种方式
    /// </summary>
    public enum DeluxeSearchMethodType
    {
        /// <summary>
        /// 相等，使用=操作符
        /// </summary>
        Equal,
        /// <summary>
        /// 模糊查询，使用LIKE操作符
        /// </summary>
        Like,
        /// <summary>
        /// 模糊查询，使用LIKE操作符，关键字在前
        /// </summary>
        PrefixLike,
        /// <summary>
        /// 模糊查询，使用LIKE操作符，关键字在尾
        /// </summary>
        SurffixLike,
        /// <summary>
        /// 使用全文索引
        /// </summary>
        FullText,
    }
}
