using System;
using System.Collections.Generic;
using System.Text;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.ComponentModel;
using System.Collections;
using System.Web.UI.HtmlControls;

using MCS.Library.Core;
using MCS.Web.Library;
using MCS.Web.Library.Script;
using MCS.Web.Library.Resources;
using MCS.Library.Globalization;

[assembly: WebResource("MCS.Web.WebControls.ClientGrid.NewClientGrid.js", "application/x-javascript")]
[assembly: WebResource("MCS.Web.WebControls.ClientGrid.ClientGrid.css", "text/css")]

namespace MCS.Web.WebControls
{
    [RequiredScript(typeof(ControlBaseScript), 1)]
    [RequiredScript(typeof(HBCommonScript), 2)]
    [RequiredScript(typeof(ClientMsgResources), 3)]
    [ClientScriptResource("MCS.Web.WebControls.ClientGrid", "MCS.Web.WebControls.ClientGrid.NewClientGrid.js")]
    [ClientCssResource("MCS.Web.WebControls.ClientGrid.ClientGrid.css")]
    public class ClientGrid : ScriptControlBase
    {
        private ClientGridColumnCollection columns = new ClientGridColumnCollection();
        private IList initialData = null;
        private DeluxeCalendar deluxeCalendarControl = new DeluxeCalendar();
        private DeluxeDateTime deluxeDateTimeControl = new DeluxeDateTime();
        private OuUserInputControl ouUserInputControl = new OuUserInputControl();
        //private MaterialControl materialControl = new MaterialControl();
        private int fixeLines = 0;
        private int notFixeLines = 0;
        private int rowHeightWithFixeLines = 30;
        private int headRowHeightWithFixeLines = 24;
        private int widthOfNotFixeLines = 500;
        private int heightOfNotFixeLines = 0;
        private bool autoWidthOfNotFixeLines = true;

        public ClientGrid()
            : base(true, HtmlTextWriterTag.Table)
        {
            JSONSerializerExecute.RegisterConverter(typeof(ClientGridColumnConverter));
        }

        protected override void OnInit(EventArgs e)
        {
            InitTemplateControls();
            base.OnInit(e);
        }

        private void InitTemplateControls()
        {
            var tbody = new HtmlGenericControl("tbody");

            HtmlGenericControl div = new HtmlGenericControl("div");
            div.ID = "div_sub_controls";

            div.Controls.Add(deluxeCalendarControl);
            div.Controls.Add(deluxeDateTimeControl);
            div.Controls.Add(ouUserInputControl);
            InitMaterialControls(div);

            var tr = new HtmlGenericControl("tr");
            tr.Style["display"] = "none";
            tbody.Controls.Add(tr);
            var td = new HtmlGenericControl("td");
            td.Attributes["colSpan"] = "2";
            tr.Controls.Add(td);
            td.Controls.Add(div);

            this.Controls.Add(tbody);
        }

        private void InitMaterialControls(HtmlGenericControl div)
        {
            for (int i = 0; i < this.Columns.Count; i++)
            {
                var column = this.columns[i];
                if (column.EditTemplate.EditMode == ClientGridColumnEditMode.Material)
                {
                    if (column.EditTemplate.TemplateControlID.IsNullOrEmpty() && column.EditTemplate.TemplateControlClientID.IsNullOrEmpty())
                    {
                        MaterialControl materialControl = new MaterialControl() { ID = "ClientGrid_MaterialControl_Template_" + i };
                        div.Controls.Add(materialControl);
                        column.EditTemplate.TemplateControlID = materialControl.ID;

                        if (column.EditTemplate.TemplateControlSettings.IsNotEmpty())
                        {
                            Dictionary<string, string> dictionary = JSONSerializerExecute.Deserialize<Dictionary<string, string>>(column.EditTemplate.TemplateControlSettings);
                            foreach (var key in dictionary.Keys)
                            {
                                switch (key)
                                {
                                    case "rootPathName":
                                        materialControl.RootPathName = dictionary[key];
                                        break;
                                    case "materialUseMode":
                                        materialControl.MaterialUseMode = (MaterialUseMode)Enum.Parse(typeof(MaterialUseMode), dictionary[key]);
                                        break;
                                    case "fileSelectMode":
                                        materialControl.FileSelectMode = (FileSelectMode)Enum.Parse(typeof(FileSelectMode), dictionary[key]);
                                        break;
                                    case "templateUrl":
                                        materialControl.TemplateUrl = dictionary[key];
                                        break;
                                    case "allowEdit":
                                        materialControl.AllowEdit = bool.Parse(dictionary[key]);
                                        break;
                                    case "allowEditContent":
                                        materialControl.AllowEditContent = bool.Parse(dictionary[key]);
                                        break;
                                    case "caption":
                                        materialControl.Caption = dictionary[key];
                                        break;
                                    case "materialTitle":
                                        materialControl.MaterialTitle = dictionary[key];
                                        break;
                                    case "displayText":
                                        materialControl.DisplayText = dictionary[key];
                                        break;
                                    case "editText":
                                        materialControl.EditText = dictionary[key];
                                        break;
                                    case "draftText":
                                        materialControl.DraftText = dictionary[key];
                                        break;
                                    case "editDocumentInCurrentPage":
                                        materialControl.EditDocumentInCurrentPage = bool.Parse(dictionary[key]);
                                        break;
                                    default:
                                        break;
                                }
                            }
                        }
                    }
                }
            }
        }

        #region Properties
        [ScriptControlProperty]
        [ClientPropertyName("readOnly")]
        [DefaultValue(false)]
        public new bool ReadOnly
        {
            get
            {
                return WebControlUtility.GetViewStateValue(ViewState, "OverrideReadOnly", false);
            }
            set
            {
                WebControlUtility.SetViewStateValue(ViewState, "OverrideReadOnly", value);
            }
        }

        /// <summary>
        /// 在客户端页面load的时候，是否自动绑定数据。如果应用企图在load时绑定数据，而不是服务器端设置，那么请将此属性设置为false
        /// </summary>
        [ScriptControlProperty]
        [ClientPropertyName("autoBindOnLoad")]
        [DefaultValue(true)]
        public bool AutoBindOnLoad
        {
            get
            {
                return WebControlUtility.GetViewStateValue(ViewState, "autoBindOnLoad", true);
            }
            set
            {
                WebControlUtility.SetViewStateValue(ViewState, "autoBindOnLoad", value);
            }
        }

        /// <summary>
        /// 客户端渲染数据行时每一批的行数
        /// </summary>
        [ScriptControlProperty]
        [ClientPropertyName("renderBatchSize")]
        [DefaultValue(false)]
        public int RenderBatchSize
        {
            get
            {
                return WebControlUtility.GetViewStateValue(ViewState, "renderBatchSize", 20);
            }
            set
            {
                WebControlUtility.SetViewStateValue(ViewState, "renderBatchSize", value);
            }
        }

        [ScriptControlProperty]
        [ClientPropertyName("showEditBar")]
        [DefaultValue(false)]
        public bool ShowEditBar
        {
            get
            {
                return WebControlUtility.GetViewStateValue(ViewState, "showEditBar", false);
            }
            set
            {
                WebControlUtility.SetViewStateValue(ViewState, "showEditBar", value);
            }
        }

        [ScriptControlProperty]
        [ClientPropertyName("showFooter")]
        [DefaultValue(false)]
        public bool ShowFooter
        {
            get
            {
                return WebControlUtility.GetViewStateValue(ViewState, "showFooter", false);
            }
            set
            {
                WebControlUtility.SetViewStateValue(ViewState, "showFooter", value);
            }
        }

        [ScriptControlProperty]
        [ClientPropertyName("showCheckBoxColumn")]
        [DefaultValue(true)]
        public bool ShowCheckBoxColumn
        {
            get
            {
                return WebControlUtility.GetViewStateValue(ViewState, "ShowCheckBoxColumn", true);
            }
            set
            {
                WebControlUtility.SetViewStateValue(ViewState, "ShowCheckBoxColumn", value);
            }
        }

        [ScriptControlProperty]
        [ClientPropertyName("selectedByDefault")]
        [DefaultValue(false)]
        public bool SelectedByDefault
        {
            get
            {
                return WebControlUtility.GetViewStateValue(ViewState, "selectedByDefault", false);
            }
            set
            {
                WebControlUtility.SetViewStateValue(ViewState, "selectedByDefault", value);
            }
        }

        [ScriptControlProperty]
        [ClientPropertyName("pageSize")]
        [DefaultValue(1024000)]
        public int PageSize
        {
            get
            {
                return WebControlUtility.GetViewStateValue(ViewState, "PageSize", 1024000);
            }
            set
            {
                WebControlUtility.SetViewStateValue(ViewState, "PageSize", value);
            }
        }

        [ScriptControlProperty]
        [ClientPropertyName("caption")]
        [DefaultValue("")]
        public string Caption
        {
            get
            {
                return WebControlUtility.GetViewStateValue(ViewState, "Caption", string.Empty);
            }
            set
            {
                WebControlUtility.SetViewStateValue(ViewState, "Caption", value);
            }
        }

        [ScriptControlProperty]
        [ClientPropertyName("emptyDataText")]
        [DefaultValue("没有记录")]
        public string EmptyDataText
        {
            get
            {
                return WebControlUtility.GetViewStateValue(ViewState, "EmptyDataText", Translator.Translate(Define.DefaultCulture, "没有记录"));
            }
            set
            {
                WebControlUtility.SetViewStateValue(ViewState, "EmptyDataText", value);
            }
        }

        [ScriptControlProperty]
        [ClientPropertyName("emptyDataHTML")]
        [DefaultValue("")]
        public string EmptyDataHTML
        {
            get
            {
                return WebControlUtility.GetViewStateValue(ViewState, "EmptyDataHTML", Translator.Translate(Define.DefaultCulture, ""));
            }
            set
            {
                WebControlUtility.SetViewStateValue(ViewState, "EmptyDataHTML", value);
            }
        }

        [ScriptControlProperty]
        [ClientPropertyName("allowPaging")]
        [DefaultValue(false)]
        public bool AllowPaging
        {
            get
            {
                return WebControlUtility.GetViewStateValue(ViewState, "AllowPaging", false);
            }
            set
            {
                WebControlUtility.SetViewStateValue(ViewState, "AllowPaging", value);
            }
        }

        [ScriptControlProperty]
        [ClientPropertyName("autoPaging")]
        [DefaultValue(false)]
        public bool AutoPaging
        {
            get
            {
                return WebControlUtility.GetViewStateValue(ViewState, "AutoPaging", false);
            }
            set
            {
                WebControlUtility.SetViewStateValue(ViewState, "AutoPaging", value);
            }
        }

        [ScriptControlProperty]
        [ClientPropertyName("cellPadding")]
        [DefaultValue("0px")]
        public string CellPadding
        {
            get
            {
                return WebControlUtility.GetViewStateValue(ViewState, "CellPadding", "0px");
            }
            set
            {
                WebControlUtility.SetViewStateValue(ViewState, "CellPadding", value);
            }
        }

        [ScriptControlProperty]
        [ClientPropertyName("cellSpacing")]
        [DefaultValue("0px")]
        public string CellSpacing
        {
            get
            {
                return WebControlUtility.GetViewStateValue(ViewState, "CellSpacing", "0px");
            }
            set
            {
                WebControlUtility.SetViewStateValue(ViewState, "CellSpacing", value);
            }
        }

        [ScriptControlProperty]
        [ClientPropertyName("cssClass")]
        [DefaultValue("clientGrid")]
        [Category("Appearance")]
        [CssClassProperty]
        public override string CssClass
        {
            get
            {

                return WebControlUtility.GetViewStateValue(ViewState, "CssClass", "clientGrid");
            }
            set
            {
                WebControlUtility.SetViewStateValue(ViewState, "CssClass", value);
            }
        }

        [ScriptControlProperty]
        [ClientPropertyName("hoveringItemCssClass")]
        [DefaultValue("hoveringItem")]
        [Description("鼠标悬停时，应添加的CssClass")]
        [Category("Appearance")]
        [CssClassProperty]
        public string HoveringItemCssClass
        {
            get
            {
                return WebControlUtility.GetViewStateValue(ViewState, "HoveringItemCssClass", "hoveringItem");
            }
            set
            {
                WebControlUtility.SetViewStateValue(ViewState, "HoveringItemCssClass", value);
            }
        }

        [ScriptControlProperty]
        [ClientPropertyName("selectedItemCssClass")]
        [DefaultValue("selectedItem")]
        [Description("选定项时，应添加的CssClass")]
        [Category("Appearance")]
        [CssClassProperty]
        public string SelectedItemCssClass
        {
            get
            {
                return WebControlUtility.GetViewStateValue(ViewState, "SelectedItemCssClass", "selectedItem");
            }
            set
            {
                WebControlUtility.SetViewStateValue(ViewState, "SelectedItemCssClass", value);
            }
        }

        [ScriptControlProperty]
        [ClientPropertyName("columns")]
        [PersistenceMode(PersistenceMode.InnerProperty), Description("客户端类定义")]
        [MergableProperty(false)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
        [DefaultValue((string)null)]
        [Browsable(false)]
        public ClientGridColumnCollection Columns
        {
            get
            {
                return this.columns;
            }
        }

        #region 固定列相关

        [ScriptControlProperty]
        [ClientPropertyName("fixeLines")]
        [DefaultValue(0)]
        private int FixeLines
        {
            get
            {
                return fixeLines;
            }
            set
            {
                fixeLines = value;
            }
        }

        [ScriptControlProperty]
        [ClientPropertyName("notFixeLines")]
        [DefaultValue(0)]
        private int NotFixeLines
        {
            get
            {
                return notFixeLines;
            }
            set
            {
                notFixeLines = value;
            }
        }

        [ScriptControlProperty]
        [ClientPropertyName("rowHeightWithFixeLines")]
        [DefaultValue(30)]
        public int RowHeightWithFixeLines
        {
            get
            {
                return rowHeightWithFixeLines;
            }
            set
            {
                rowHeightWithFixeLines = value;
            }
        }

        [ScriptControlProperty]
        [ClientPropertyName("widthOfNotFixeLines")]
        [DefaultValue(30)]
        public int WidthOfNotFixeLines
        {
            get
            {
                return widthOfNotFixeLines;
            }
            set
            {
                widthOfNotFixeLines = value;
            }
        }

        [ScriptControlProperty]
        [ClientPropertyName("heightOfNotFixeLines")]
        [DefaultValue(0)]
        public int HeightOfNotFixeLines
        {
            //默认为0,如为0不取该值。不为0为应用设置的高度。取该值
            get
            {
                return heightOfNotFixeLines;
            }
            set
            {
                heightOfNotFixeLines = value;
            }
        }

        [ScriptControlProperty]
        [ClientPropertyName("totleWidthOfFixeLines")]
        [DefaultValue(0)]
        private int TotleWidthOfFixeLines
        {
            //移动列的总宽度
            get;
            set;
        }

        [ScriptControlProperty]
        [ClientPropertyName("totleWidthOfNotFixeLines")]
        [DefaultValue(0)]
        private int TotleWidthOfNotFixeLines
        {
            //移动列的总宽度
            get;
            set;
        }

        [ScriptControlProperty]
        [ClientPropertyName("autoWidthOfNotFixeLines")]
        [DefaultValue(true)]
        public bool AutoWidthOfNotFixeLines
        {
            //移动列的总宽度
            get { return autoWidthOfNotFixeLines; }
            set { autoWidthOfNotFixeLines = value; }
        }

        [ScriptControlProperty]
        [ClientPropertyName("headRowHeightWithFixeLines")]
        [DefaultValue(0)]
        private int HeadRowHeightWithFixeLines
        {
            //标题行的高度
            get { return headRowHeightWithFixeLines; }
            set { headRowHeightWithFixeLines = value; }
        }

        #endregion

        /// <summary>
        /// 初始化到客户端的数据
        /// </summary>
        [Browsable(false)]
        public IList InitialData
        {
            get
            {
                return this.initialData;
            }
            set
            {
                this.initialData = value;
            }
        }

        [Browsable(false)]
        [ScriptControlProperty()]
        [ClientPropertyName("deluxeCalendarControlClientID")]
        public string deluxeCalendarControlClientID
        {
            get { return this.deluxeCalendarControl.ClientID; }
        }

        [Browsable(false)]
        [ScriptControlProperty()]
        [ClientPropertyName("deluxeDateTimeControlClientID")]
        public string deluxeDateTimeControlClientID
        {
            get { return this.deluxeDateTimeControl.ClientID; }
        }
        #endregion

        #region Client Events
        [DefaultValue("")]
        [ScriptControlEvent]
        [ClientPropertyName("beforeDataBind")]
        [Bindable(true), Category("ClientEventsHandler"), Description("开始绑定数据事件【绑定前】")]
        public string OnBeforeDataBind
        {
            get
            {
                return GetPropertyValue("OnBeforeDataBind", string.Empty);
            }
            set
            {
                SetPropertyValue("OnBeforeDataBind", value);
            }
        }

        [DefaultValue("")]
        [ScriptControlEvent]
        [ClientPropertyName("afterDataBind")]
        [Bindable(true), Category("ClientEventsHandler"), Description("结束绑定数据事件【绑定后】")]
        public string OnAfterDataBind
        {
            get
            {
                return GetPropertyValue("OnAfterDataBind", string.Empty);
            }
            set
            {
                SetPropertyValue("OnAfterDataBind", value);
            }
        }

        [DefaultValue("")]
        [ScriptControlEvent]
        [ClientPropertyName("headCellCreating")]
        [Bindable(true), Category("ClientEventsHandler"), Description("客户端构建HeadCell的事件")]
        public string OnHeadCellCreating
        {
            get
            {
                return GetPropertyValue("OnHeadCellCreating", string.Empty);
            }
            set
            {
                SetPropertyValue("OnHeadCellCreating", value);
            }
        }

        [DefaultValue("")]
        [ScriptControlEvent]
        [ClientPropertyName("footCellCreating")]
        [Bindable(true), Category("ClientEventsHandler"), Description("客户端构建FootCell的事件")]
        public string OnFootCellCreating
        {
            get
            {
                return GetPropertyValue("OnFootCellCreating", string.Empty);
            }
            set
            {
                SetPropertyValue("OnFootCellCreating", value);
            }
        }

        [DefaultValue("")]
        [ScriptControlEvent]
        [ClientPropertyName("cellDataBound")]
        [Bindable(true), Category("ClientEventsHandler"), Description("客户端数据绑定")]
        public string OnClientCellDataBound
        {
            get
            {
                return GetPropertyValue("OnClientCellDataBound", string.Empty);
            }
            set
            {
                SetPropertyValue("OnClientCellDataBound", value);
            }
        }

        [DefaultValue("")]
        [ScriptControlEvent]
        [ClientPropertyName("pageIndexChanged")]
        [Bindable(true), Category("ClientEventsHandler"), Description("客户端数据页码变化的事件")]
        public string OnClientPageIndexChanged
        {
            get
            {
                return GetPropertyValue("OnClientPageIndexChanged", string.Empty);
            }
            set
            {
                SetPropertyValue("OnClientPageIndexChanged", value);
            }
        }

        [DefaultValue("")]
        [ScriptControlEvent]
        [ClientPropertyName("sorted")]
        [Bindable(true), Category("ClientEventsHandler"), Description("客户端排序的事件")]
        public string OnClientSorted
        {
            get
            {
                return GetPropertyValue("OnClientSorted", string.Empty);
            }
            set
            {
                SetPropertyValue("OnClientSorted", value);
            }
        }

        [DefaultValue("")]
        [ScriptControlEvent]
        [ClientPropertyName("preCellCreatEditor")]
        [Bindable(true), Category("ClientEventsHandler"), Description("客户端创建控件发生前的事件")]
        public string OnPreCellCreatEditor
        {
            get
            {
                return GetPropertyValue("OnPreCellCreatEditor", string.Empty);
            }
            set
            {
                SetPropertyValue("OnPreCellCreatEditor", value);
            }
        }

        [DefaultValue("")]
        [ScriptControlEvent]
        [ClientPropertyName("cellCreatingEditor")]
        [Bindable(true), Category("ClientEventsHandler"), Description("客户端创建控件发生时的事件")]
        public string OnCellCreatingEditor
        {
            get
            {
                return GetPropertyValue("OnCellCreatingEditor", string.Empty);
            }
            set
            {
                SetPropertyValue("OnCellCreatingEditor", value);
            }
        }

        [DefaultValue("")]
        [ScriptControlEvent]
        [ClientPropertyName("cellCreatedEditor")]
        [Bindable(true), Category("ClientEventsHandler"), Description("客户端创建控件发生后的事件")]
        public string OnCellCreatedEditor
        {
            get
            {
                return GetPropertyValue("OnCellCreatedEditor", string.Empty);
            }
            set
            {
                SetPropertyValue("OnCellCreatedEditor", value);
            }
        }

        [DefaultValue("")]
        [ScriptControlEvent]
        [ClientPropertyName("editorValidate")]
        [Bindable(true), Category("ClientEventsHandler"), Description("客户端控件校验事件")]
        public string OnEditorValidate
        {
            get
            {
                return GetPropertyValue("OnEditorValidate", string.Empty);
            }
            set
            {
                SetPropertyValue("OnEditorValidate", value);
            }
        }


        [DefaultValue("")]
        [ScriptControlEvent]
        [ClientPropertyName("initializeEditor")]
        [Bindable(true), Category("ClientEventsHandler"), Description("初始化Editor事件")]
        public string OnInitializeEditor
        {
            get
            {
                return GetPropertyValue("OnInitializeEditor", string.Empty);
            }
            set
            {
                SetPropertyValue("OnInitializeEditor", value);
            }
        }

        [DefaultValue("")]
        [ScriptControlEvent]
        [ClientPropertyName("dataFormatting")]
        [Bindable(true), Category("ClientEventsHandler"), Description("数据格式化事件")]
        public string OnDataFormatting
        {
            get
            {
                return GetPropertyValue("OnDataFormatting", string.Empty);
            }
            set
            {
                SetPropertyValue("OnDataFormatting", value);
            }
        }

        [DefaultValue("")]
        [ScriptControlEvent]
        [ClientPropertyName("dataChanging")]
        [Bindable(true), Category("ClientEventsHandler"), Description("数据dataChanging事件")]
        public string OnDataChanging
        {
            get
            {
                return GetPropertyValue("OnDataChanging", string.Empty);
            }
            set
            {
                SetPropertyValue("OnDataChanging", value);
            }
        }

        [DefaultValue("")]
        [ScriptControlEvent]
        [ClientPropertyName("dataChanged")]
        [Bindable(true), Category("ClientEventsHandler"), Description("数据Changed事件")]
        public string OnDataChanged
        {
            get
            {
                return GetPropertyValue("OnDataChanged", string.Empty);
            }
            set
            {
                SetPropertyValue("OnDataChanged", value);
            }
        }

        [DefaultValue("")]
        [ScriptControlEvent]
        [ClientPropertyName("preRowAdd")]
        [Bindable(true), Category("ClientEventsHandler"), Description("添加新行前事件")]
        public string OnPreRowAdd
        {
            get
            {
                return GetPropertyValue("OnPreRowAdd", string.Empty);
            }
            set
            {
                SetPropertyValue("OnPreRowAdd", value);
            }
        }

        [DefaultValue("")]
        [ScriptControlEvent]
        [ClientPropertyName("rowDelete")]
        [Bindable(true), Category("ClientEventsHandler"), Description("删除行事件")]
        public string OnRowDelete
        {
            get
            {
                return GetPropertyValue("OnRowDelete", string.Empty);
            }
            set
            {
                SetPropertyValue("OnRowDelete", value);
            }
        }

        [DefaultValue("")]
        [ScriptControlEvent]
        [ClientPropertyName("allSelectCheckboxClicked")]
        [Bindable(true), Category("ClientEventsHandler"), Description("(全选)复选框单击事件")]
        public string OnAllSelectCheckboxClicked
        {
            get
            {
                return GetPropertyValue("OnAllSelectCheckboxClicked", string.Empty);
            }
            set
            {
                SetPropertyValue("OnAllSelectCheckboxClicked", value);
            }
        }

        [DefaultValue("")]
        [ScriptControlEvent]
        [ClientPropertyName("selectCheckboxClick")]
        [Bindable(true), Category("ClientEventsHandler"), Description("复选框(不包括全选)单击事件")]
        public string OnSelectCheckboxClick
        {
            get
            {
                return GetPropertyValue("OnSelectCheckboxClick", string.Empty);
            }
            set
            {
                SetPropertyValue("OnSelectCheckboxClick", value);
            }
        }

        [DefaultValue("")]
        [ScriptControlEvent]
        [ClientPropertyName("selectCheckboxCreated")]
        [Bindable(true), Category("ClientEventsHandler"), Description("复选框(包括全选)创建事件")]
        public string OnSelectCheckboxCreated
        {
            get
            {
                return GetPropertyValue("OnSelectCheckboxCreated", string.Empty);
            }
            set
            {
                SetPropertyValue("OnSelectCheckboxCreated", value);
            }
        }

        [DefaultValue("")]
        [ScriptControlEvent]
        [ClientPropertyName("beforeSaveClientState")]
        [Bindable(true), Category("ClientEventsHandler"), Description("saveClientState前事件")]
        public string OnBeforeSaveClientState
        {
            get
            {
                return GetPropertyValue("OnBeforeSaveClientState", string.Empty);
            }
            set
            {
                SetPropertyValue("OnBeforeSaveClientState", value);
            }
        }

        [DefaultValue("")]
        [ScriptControlEvent]
        [ClientPropertyName("editBarRowCreating")]
        [Bindable(true), Category("ClientEventsHandler"), Description("editBarRowCreating创建时事件")]
        public string OnEditBarRowCreating
        {
            get
            {
                return GetPropertyValue("OnEditBarRowCreating", string.Empty);
            }
            set
            {
                SetPropertyValue("OnEditBarRowCreating", value);
            }
        }

        [DefaultValue("")]
        [ScriptControlEvent]
        [ClientPropertyName("preHeaderRowCreate")]
        [Bindable(true), Category("ClientEventsHandler"), Description("HeaderRow创建前事件")]
        public string OnPreHeaderRowCreate
        {
            get
            {
                return GetPropertyValue("OnPreHeaderRowCreate", string.Empty);
            }
            set
            {
                SetPropertyValue("OnPreHeaderRowCreate", value);
            }
        }

        [DefaultValue("")]
        [ScriptControlEvent]
        [ClientPropertyName("beforeDataRowCreate")]
        [Bindable(true), Category("ClientEventsHandler"), Description("数据行创建前事件")]
        public string OnBeforeDataRowCreate
        {
            get
            {
                return GetPropertyValue("OnBeforeDataRowCreate", string.Empty);
            }
            set
            {
                SetPropertyValue("OnBeforeDataRowCreate", value);
            }
        }

        [DefaultValue("")]
        [ScriptControlEvent]
        [ClientPropertyName("afterDataRowCreate")]
        [Bindable(true), Category("ClientEventsHandler"), Description("数据行创建后事件")]
        public string OnAfterDataRowCreate
        {
            get
            {
                return GetPropertyValue("OnAfterDataRowCreate", string.Empty);
            }
            set
            {
                SetPropertyValue("OnAfterDataRowCreate", value);
            }
        }


        #endregion Client Events

        #region Protected

        protected override string SaveClientState()
        {
            string result = string.Empty;

            if (this.initialData != null)
            {
                object[] state = new object[] { this.initialData, this.initialData.GetType().AssemblyQualifiedName };

                PerformanceMonitorHelper.GetDefaultMonitor().WriteExecutionDuration(this.UniqueID + "_JSON Serializer",
                    () => result = JSONSerializerExecute.Serialize(state));
            }

            return result;
        }

        protected override void LoadClientState(string clientState)
        {
            object[] state = JSONSerializerExecute.Deserialize<object[]>(HttpUtility.UrlDecode(clientState));

            if (state[1] != null)
            {
                string typeDesp = (string)state[1];

                if (string.IsNullOrEmpty(typeDesp) == false && state[0] != null)
                {
                    Type type = TypeCreator.GetTypeInfo(typeDesp);

                    this.initialData = (IList)JSONSerializerExecute.DeserializeObject(state[0], type);
                }
            }
        }

        protected override void OnPreRender(EventArgs e)
        {
            if (!this.SettleOuUserInput())
            {
                this.ouUserInputControl.Visible = false;
            }

            base.OnPreRender(e);

            //this.RemoveUnVisibleColumns();

            int lastFixedLineIndexOfColumns = GetLastFixedLineIndexOfColumns();
            for (int i = 0; i < this.Columns.Count; i++)
            {
                //将最后一个标记为固定列之前的列定义设置为固定列(true)
                if (i < lastFixedLineIndexOfColumns)
                    this.Columns[i].IsFixedLine = true;

                if (this.Columns[i].EditTemplate.TemplateControlID.IsNotEmpty())
                {
                    Control templeteControl = this.Page.FindControlByID(this.Columns[i].EditTemplate.TemplateControlID, true);

                    if (templeteControl != null)
                        this.Columns[i].EditTemplate.TemplateControlClientID = templeteControl.ClientID;
                }
            }
            this.RegisterClientCss();
            this.SettleTotleWidthOfNotFixeLines();
        }

        #endregion

        #region private

        private bool SettleOuUserInput()
        {
            bool result = false;

            for (int i = 0; i < this.Columns.Count; i++)
            {
                var column = this.columns[i];
                if (column.EditTemplate.EditMode == ClientGridColumnEditMode.OuUserInput)
                {
                    if (column.EditTemplate.TemplateControlID.IsNullOrEmpty() && column.EditTemplate.TemplateControlClientID.IsNullOrEmpty())
                    {
                        result = true;
                        column.EditTemplate.TemplateControlClientID = this.ouUserInputControl.ClientID;
                    }
                }
            }

            return result;
        }

        /// <summary>
        /// 去除visible==false的列定义
        /// </summary>
        private void RemoveUnVisibleColumns()
        {
            ClientGridColumnCollection c = new ClientGridColumnCollection();
            foreach (var item in Columns)
            {
                if (item.Visible)
                    c.Add(item);
            }
            this.columns = c;
        }

        /// <summary>
        /// 获取列定义列表中最后标记为固定列的下标
        /// </summary>
        /// <returns></returns>
        private int GetLastFixedLineIndexOfColumns()
        {
            int index = this.Columns.Count;
            for (int i = this.Columns.Count - 1; i >= 0; i--)
            {
                if (this.Columns[i].IsFixedLine)
                {
                    index = i;
                    break;
                }
            }

            FixeLines = index;
            NotFixeLines = this.Columns.Count - index;

            return index;
        }

        /// <summary>
        /// 获取非固定列的总宽度
        /// </summary>
        private void SettleTotleWidthOfNotFixeLines()
        {
            foreach (var item in this.Columns)
            {
                if (item.Visible)
                {
                    IDictionary currentItemStyle = (IDictionary)JSONSerializerExecute.DeserializeObject(item.ItemStyle);

                    string temp = "100";
                    if (currentItemStyle != null)
                        temp = DictionaryHelper.GetValue(currentItemStyle, "width", "100");

                    if (temp.IndexOf("px") > 0)
                        temp = temp.Substring(0, temp.Length - 2);

                    if (temp.IndexOf("%") > 0)
                        temp = "160";

                    if (!item.IsFixedLine)
                        this.TotleWidthOfNotFixeLines += int.Parse(temp);
                    else
                        this.TotleWidthOfFixeLines += int.Parse(temp);

                    //得到高度
                    IDictionary currentHeaderStyle = (IDictionary)JSONSerializerExecute.DeserializeObject(item.HeaderStyle);
                    string temp2 = "24";
                    if (currentHeaderStyle != null)
                        temp2 = DictionaryHelper.GetValue(currentHeaderStyle, "height", "24");

                    if (temp2.IndexOf("px") > 0)
                        temp2 = temp2.Substring(0, temp2.Length - 2);

                    if (temp.IndexOf("%") > 0)
                        this.HeadRowHeightWithFixeLines = 24;
                    else
                        if (int.Parse(temp2) > HeadRowHeightWithFixeLines)
                            this.HeadRowHeightWithFixeLines = int.Parse(temp2);

                }
            }
        }

        private void RegisterClientCss()
        {
            if (this.NotFixeLines > 0)
                Page.ClientScript.RegisterClientScriptBlock(this.GetType(), "StatusCSS",
                    string.Format("<style type='text/css'>{0}</style>", ".clientGrid.tbody tr { height: " + this.RowHeightWithFixeLines.ToString() + "px;}"));
        }
        #endregion
    }
}
