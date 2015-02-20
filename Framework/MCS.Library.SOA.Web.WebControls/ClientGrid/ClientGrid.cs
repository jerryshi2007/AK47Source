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
        /// �ڿͻ���ҳ��load��ʱ���Ƿ��Զ������ݡ����Ӧ����ͼ��loadʱ�����ݣ������Ƿ����������ã���ô�뽫����������Ϊfalse
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
        /// �ͻ�����Ⱦ������ʱÿһ��������
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
        [DefaultValue("û�м�¼")]
        public string EmptyDataText
        {
            get
            {
                return WebControlUtility.GetViewStateValue(ViewState, "EmptyDataText", Translator.Translate(Define.DefaultCulture, "û�м�¼"));
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
        [Description("�����ͣʱ��Ӧ��ӵ�CssClass")]
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
        [Description("ѡ����ʱ��Ӧ��ӵ�CssClass")]
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
        [PersistenceMode(PersistenceMode.InnerProperty), Description("�ͻ����ඨ��")]
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

        #region �̶������

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
            //Ĭ��Ϊ0,��Ϊ0��ȡ��ֵ����Ϊ0ΪӦ�����õĸ߶ȡ�ȡ��ֵ
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
            //�ƶ��е��ܿ��
            get;
            set;
        }

        [ScriptControlProperty]
        [ClientPropertyName("totleWidthOfNotFixeLines")]
        [DefaultValue(0)]
        private int TotleWidthOfNotFixeLines
        {
            //�ƶ��е��ܿ��
            get;
            set;
        }

        [ScriptControlProperty]
        [ClientPropertyName("autoWidthOfNotFixeLines")]
        [DefaultValue(true)]
        public bool AutoWidthOfNotFixeLines
        {
            //�ƶ��е��ܿ��
            get { return autoWidthOfNotFixeLines; }
            set { autoWidthOfNotFixeLines = value; }
        }

        [ScriptControlProperty]
        [ClientPropertyName("headRowHeightWithFixeLines")]
        [DefaultValue(0)]
        private int HeadRowHeightWithFixeLines
        {
            //�����еĸ߶�
            get { return headRowHeightWithFixeLines; }
            set { headRowHeightWithFixeLines = value; }
        }

        #endregion

        /// <summary>
        /// ��ʼ�����ͻ��˵�����
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
        [Bindable(true), Category("ClientEventsHandler"), Description("��ʼ�������¼�����ǰ��")]
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
        [Bindable(true), Category("ClientEventsHandler"), Description("�����������¼����󶨺�")]
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
        [Bindable(true), Category("ClientEventsHandler"), Description("�ͻ��˹���HeadCell���¼�")]
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
        [Bindable(true), Category("ClientEventsHandler"), Description("�ͻ��˹���FootCell���¼�")]
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
        [Bindable(true), Category("ClientEventsHandler"), Description("�ͻ������ݰ�")]
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
        [Bindable(true), Category("ClientEventsHandler"), Description("�ͻ�������ҳ��仯���¼�")]
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
        [Bindable(true), Category("ClientEventsHandler"), Description("�ͻ���������¼�")]
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
        [Bindable(true), Category("ClientEventsHandler"), Description("�ͻ��˴����ؼ�����ǰ���¼�")]
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
        [Bindable(true), Category("ClientEventsHandler"), Description("�ͻ��˴����ؼ�����ʱ���¼�")]
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
        [Bindable(true), Category("ClientEventsHandler"), Description("�ͻ��˴����ؼ���������¼�")]
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
        [Bindable(true), Category("ClientEventsHandler"), Description("�ͻ��˿ؼ�У���¼�")]
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
        [Bindable(true), Category("ClientEventsHandler"), Description("��ʼ��Editor�¼�")]
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
        [Bindable(true), Category("ClientEventsHandler"), Description("���ݸ�ʽ���¼�")]
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
        [Bindable(true), Category("ClientEventsHandler"), Description("����dataChanging�¼�")]
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
        [Bindable(true), Category("ClientEventsHandler"), Description("����Changed�¼�")]
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
        [Bindable(true), Category("ClientEventsHandler"), Description("�������ǰ�¼�")]
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
        [Bindable(true), Category("ClientEventsHandler"), Description("ɾ�����¼�")]
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
        [Bindable(true), Category("ClientEventsHandler"), Description("(ȫѡ)��ѡ�򵥻��¼�")]
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
        [Bindable(true), Category("ClientEventsHandler"), Description("��ѡ��(������ȫѡ)�����¼�")]
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
        [Bindable(true), Category("ClientEventsHandler"), Description("��ѡ��(����ȫѡ)�����¼�")]
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
        [Bindable(true), Category("ClientEventsHandler"), Description("saveClientStateǰ�¼�")]
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
        [Bindable(true), Category("ClientEventsHandler"), Description("editBarRowCreating����ʱ�¼�")]
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
        [Bindable(true), Category("ClientEventsHandler"), Description("HeaderRow����ǰ�¼�")]
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
        [Bindable(true), Category("ClientEventsHandler"), Description("�����д���ǰ�¼�")]
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
        [Bindable(true), Category("ClientEventsHandler"), Description("�����д������¼�")]
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
                //�����һ�����Ϊ�̶���֮ǰ���ж�������Ϊ�̶���(true)
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
        /// ȥ��visible==false���ж���
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
        /// ��ȡ�ж����б��������Ϊ�̶��е��±�
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
        /// ��ȡ�ǹ̶��е��ܿ��
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

                    //�õ��߶�
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
