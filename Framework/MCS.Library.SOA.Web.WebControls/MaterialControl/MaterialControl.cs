using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Text;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using MCS.Library.Core;
using MCS.Library.Globalization;
using MCS.Library.OGUPermission;
using MCS.Library.Passport;
using MCS.Library.Principal;
using MCS.Library.SOA.DataObjects;
using MCS.Library.SOA.DataObjects.Workflow;
using MCS.Web.Library;
using MCS.Web.Library.MVC;
using MCS.Web.Library.Script;
using MCS.Web.WebControls.Properties;

#region assembly

[assembly: WebResource("MCS.Web.WebControls.MaterialControl.MaterialControl.js", "application/x-javascript")]
[assembly: WebResource("MCS.Web.WebControls.MaterialControl.Material.js", "application/x-javascript")]
[assembly: WebResource("MCS.Web.WebControls.MaterialControl.MaterialControl.css", "text/css")]
[assembly: WebResource("MCS.Web.WebControls.Images.empty.gif", "image/gif")]
[assembly: WebResource("MCS.Web.WebControls.Images.edit.gif", "image/gif")]
[assembly: WebResource("MCS.Web.WebControls.Images.upload.gif", "image/gif")]
[assembly: WebResource("MCS.Web.WebControls.Images.version.gif", "image/gif")]
[assembly: WebResource("MCS.Web.WebControls.Images.setOpenType.gif", "image/gif")]
[assembly: WebResource("MCS.Web.WebControls.Images.default.gif", "image/gif")]
[assembly: WebResource("MCS.Web.WebControls.Images.material.gif", "image/gif")]

#endregion

namespace MCS.Web.WebControls
{
    #region 枚举

    /// <summary>
    /// 文件选择方式
    /// TraditionalSingle　: 传统单文件
    /// MultiSelectUseActiveX：ActiveX多选
    /// </summary>
    public enum FileSelectMode
    {
        /// <summary>
        /// 传统单文件
        /// </summary>
        TraditionalSingle = 1,
        /// <summary>
        /// ActiveX多选
        /// </summary>
        MultiSelectUseActiveX = 2,
    }

    /// <summary>
    /// 控件在不同流程环节下的控制模式
    /// </summary>
    public enum MaterialActivityControlMode
    {
        None = 0,

        /// <summary>
        /// 允许删除不属于此环节的附件
        /// </summary>
        AllowDelete = 1,

        /// <summary>
        /// 允许编辑不属于此环节的附件的属性
        /// </summary>
        AllowEdit = 2,

        /// <summary>
        /// 允许编辑不属于此环节的附件的内容
        /// </summary>
        AllowEditContent = 4,

        /// <summary>
        /// All
        /// </summary>
        All = 7
    }

    /// <summary>
    /// 显示样式
    /// InLine　: 在一行中显示
    /// Vertical　:　多行显示
    /// </summary>
    public enum MaterialTableShowMode
    {
        /// <summary>
        /// 单行显示
        /// </summary>
        Inline = 1,
        /// <summary>
        /// 分行显示
        /// </summary>
        Vertical = 2
    }

    /// <summary>
    /// 使用方式
    /// SingleDraft　: 起草单个文件
    /// DraftAndUpload　:　起草一个文件，上传多个文件
    /// UploadFile : 上传文件
    /// </summary>
    public enum MaterialUseMode
    {
        /// <summary>
        /// 起草
        /// </summary>
        SingleDraft = 1,
        /// <summary>
        /// 起草并可上传
        /// </summary>
        DraftAndUpload = 2,
        /// <summary>
        /// 只能上传
        /// </summary>
        UploadFile = 3,
    }

    /// <summary>
    /// "添加或修改" 显示为文字还是图片
    /// </summary>
    public enum LinkShowMode
    {
        /// <summary>
        /// 显示为文字链接
        /// </summary>
        Text = 1,
        /// <summary>
        /// 显示为图片
        /// </summary>
        Image = 2,
        /// <summary>
        /// 图片在前 文字在后
        /// </summary>
        ImageAndText = 3
    }

    #endregion

    #region	控件
    [RequiredScript(typeof(ControlBaseScript), 1)]
    [RequiredScript(typeof(HBCommonScript), 3)]
    [RequiredScript(typeof(MaterialScript), 5)]
    [ClientScriptResource("MCS.Web.WebControls.MaterialControl",
        "MCS.Web.WebControls.MaterialControl.MaterialControl.js")]
    [ClientCssResource("MCS.Web.WebControls.MaterialControl.MaterialControl.css")]
    public class MaterialControl : ScriptControlBase, IDeltaDataControl
    {
        private const string MATERIALCONTROL_COMMON_HIDDENFIELD = "MaterialControlCommonHiddenField";

        public static CommonDeltaMaterialsContainer GetCommonDeltaMaterials()
        {
            Dictionary<string, DeltaMaterialList> resultData = new Dictionary<string, DeltaMaterialList>();
            var postData = HttpContext.Current.Request.Form[MATERIALCONTROL_COMMON_HIDDENFIELD];

            if (postData.IsNotEmpty())
            {
                var dicObjs = JSONSerializerExecute.Deserialize<Dictionary<string, object>>(postData);
                foreach (var key in dicObjs.Keys)
                {
                    var deltaMaterials = JSONSerializerExecute.Deserialize<DeltaMaterialList>(dicObjs[key]);
                    if (deltaMaterials != null && deltaMaterials.Inserted != null)
                    {
                        for (int i = 0; i < deltaMaterials.Inserted.Count; i++)
                        {
                            deltaMaterials.Inserted[i].Creator = GetCurrentUser();
                            deltaMaterials.Inserted[i].Department = GetCurrentDepartment();
                        }
                    }
                    resultData.Add(key, deltaMaterials);
                }
            }

            return new CommonDeltaMaterialsContainer(resultData);
        }

        private IAttributeAccessor controlToShowDialog = null;

        public MaterialControl()
            : base(true, HtmlTextWriterTag.Div)
        {
            JSONSerializerExecute.RegisterConverter(typeof(OguObjectConverter));
            JSONSerializerExecute.RegisterConverter(typeof(MaterialConverter));
            JSONSerializerExecute.RegisterConverter(typeof(DeltaMaterialConverter));
            JSONSerializerExecute.RegisterConverter(typeof(MultiMaterialConverter));
        }

        #region 私有变量

        private CustomValidator submitValidator = new CustomValidator();
        private DialogUploadFileControl dialogUploadFileControl = new DialogUploadFileControl();
        private DialogUploadFileTraditionalControl dialogUploadFileTraditonalControl = new DialogUploadFileTraditionalControl();
        private DialogMaterialVersionControl dialogMaterialVersionControl = new DialogMaterialVersionControl();
        private DialogFileOpenTypeControl dialogFileOpenTypeControl = new DialogFileOpenTypeControl();
        private DialogUploadFileProcessControl dialogUploadFileProcessControl = new DialogUploadFileProcessControl();
        //private DialogEditDocumentControl dialogEditDocumentControl = new DialogEditDocumentControl();
        private OfficeViewerWrapper officeViewerWrapper = new OfficeViewerWrapper();
        private DeltaMaterialList deltaMaterials = new DeltaMaterialList();
        private MaterialList materials = new MaterialList();
        private ComponentHelperWrapper componentHelperWrapper = new ComponentHelperWrapper();

        private bool delayProcessDownload = false;
        #endregion

        #region	属性

        #region 设计态显示的属性

        /// <summary>
        /// 是否在浏览器内编辑文档
        /// </summary>
        [DefaultValue(false), Category("Behavior"), ScriptControlProperty, ClientPropertyName("editDocumentInCurrentPage"), Description("是否在浏览器内编辑文档")]
        public bool EditDocumentInCurrentPage
        {
            get
            {
                return GetPropertyValue("EditDocumentInCurrentPage", false);
            }
            set
            {
                SetPropertyValue("EditDocumentInCurrentPage", value);
            }
        }

        /// <summary>
        /// 是否自动打开文档，仅当SingleDraft模式下生效
        /// </summary>
        [DefaultValue(false), Category("Behavior"), ScriptControlProperty, ClientPropertyName("autoOpenDocument"), Description("是否自动打开文档")]
        public bool AutoOpenDocument
        {
            get
            {
                return GetPropertyValue("AutoOpenDocument", false);
            }
            set
            {
                SetPropertyValue("AutoOpenDocument", value);
            }
        }

        /// <summary>
        /// 附件在流转环节方面的控制
        /// </summary>
        [DefaultValue(MaterialActivityControlMode.AllowEditContent), Category("Appearance"), ScriptControlProperty, ClientPropertyName("activityControlMode"), Description("附件在流转环节方面的控制")]
        public MaterialActivityControlMode ActivityControlMode
        {
            get
            {
                return GetPropertyValue<MaterialActivityControlMode>("ActivityControlMode", MaterialActivityControlMode.AllowEditContent);
            }
            set
            {
                SetPropertyValue<MaterialActivityControlMode>("ActivityControlMode", value);
            }
        }

        [DefaultValue(true), Category("Appearance"), Description("是否根据流程状态自动控制编辑状态")]
        public bool AutoControlEditStatusByActivity
        {
            get
            {
                return GetPropertyValue("AutoControlEditStatusByActivity", true);
            }
            set
            {
                SetPropertyValue("AutoControlEditStatusByActivity", value);
            }
        }

        /// <summary>
        /// 弹出上传文件窗口的按钮文字
        /// </summary>
        [DefaultValue("添加或修改"), Category("Appearance"), ScriptControlProperty, ClientPropertyName("caption"), Description("弹出上传文件窗口的按钮文字")]
        public string Caption
        {
            get
            {
                return GetPropertyValue<string>("Caption", Translator.Translate(Define.DefaultCulture, "添加或修改"));
            }
            set
            {
                SetPropertyValue<string>("Caption", value);
            }
        }

        /// <summary>
        /// 起草时显示的文字
        /// </summary>
        [DefaultValue("起草"), Category("Appearance"), ScriptControlProperty, ClientPropertyName("draftText"), Description("起草时显示的文字")]
        public string DraftText
        {
            get
            {
                return GetPropertyValue<string>("DraftText", Translator.Translate(Define.DefaultCulture, "起草"));
            }
            set
            {
                SetPropertyValue<string>("DraftText", value);
            }
        }

        /// <summary>
        /// 保存原始草稿
        /// </summary>
        [DefaultValue(true), Category("Behavior"), ScriptControlProperty, ClientPropertyName("saveOriginalDraft"), Description("是否保存原始草稿")]
        public bool SaveOriginalDraft
        {
            get
            {
                return GetPropertyValue<bool>("SaveOriginalDraft", true);
            }
            set
            {
                SetPropertyValue<bool>("SaveOriginalDraft", value);
            }
        }

        /// <summary>
        /// 新起草文件的默认名字
        /// </summary>
        [DefaultValue("正文"), Category("Appearance"), ScriptControlProperty, ClientPropertyName("materialTitle"), Description("新起草文件的默认名字")]
        public string MaterialTitle
        {
            get
            {
                return GetPropertyValue<string>("MaterialTitle", Translator.Translate(Define.DefaultCulture, "正文"));
            }
            set
            {
                SetPropertyValue<string>("MaterialTitle", value);
            }
        }


        /// <summary>
        /// 编辑时显示的文字
        /// </summary>
        [DefaultValue("编辑"), Category("Appearance"), ScriptControlProperty, ClientPropertyName("editText"), Description("编辑时显示的文字")]
        public string EditText
        {
            get
            {
                return GetPropertyValue<string>("EditText", Translator.Translate(Define.DefaultCulture, "编辑"));
            }
            set
            {
                SetPropertyValue<string>("EditText", value);
            }
        }

        /// <summary>
        /// 显示状态的动词显示
        /// </summary>
        [DefaultValue("显示"), Category("Appearance"), ScriptControlProperty, ClientPropertyName("displayText"), Description("显示状态的动词显示")]
        public string DisplayText
        {
            get
            {
                return GetPropertyValue<string>("DisplayText", Translator.Translate(Define.DefaultCulture, "显示"));
            }
            set
            {
                SetPropertyValue<string>("DisplayText", value);
            }
        }

        /// <summary>
        /// 是否允许修改文件内容
        /// </summary>
        [DefaultValue(true), Category("Document"), Description("是否允许修改文件内容")]
        public bool AllowEditContent
        {
            get
            {
                return GetPropertyValue<bool>("AllowEditContent", true);
            }
            set
            {
                SetPropertyValue<bool>("AllowEditContent", value);
            }
        }

        /// <summary>
        /// 是否可以添加、删除文件、修改文件标题
        /// </summary>
        [DefaultValue(true), Category("Document"), Description("是否允许修改文件标题等其他属性")]
        public bool AllowEdit
        {
            get
            {
                return GetPropertyValue<bool>("AllowEdit", true);
            }
            set
            {
                SetPropertyValue<bool>("AllowEdit", value);
            }
        }

        [Browsable(false)]
        [ScriptControlProperty, ClientPropertyName("allowEditContent")]
        private bool ClientAllowEditContent
        {
            get
            {
                return this.AllowEditContent && this.Enabled && this.ReadOnly == false;
            }
        }

        [Browsable(false)]
        [ScriptControlProperty, ClientPropertyName("allowEdit")]
        private bool ClientAllowEdit
        {
            get
            {
                return this.AllowEdit && this.Enabled && this.ReadOnly == false;
            }
        }

        [ScriptControlProperty, ClientPropertyName("requestContext")]
        public string RequestContext
        {
            get
            {
                return GetPropertyValue("RequestContext", string.Empty);
            }
            set
            {
                SetPropertyValue("RequestContext", value);
            }
        }

        /// <summary>
        /// 是否只读
        /// </summary>
        [DefaultValue(false), Category("Document"), Description("是否只读")]
        public new bool ReadOnly
        {
            get
            {
                return GetPropertyValue("OnlyRead", false);
            }
            set
            {
                SetPropertyValue("OnlyRead", value);

                base.ReadOnly = false;
            }
        }

        /// <summary>
        /// 是否有效
        /// </summary>
        [DefaultValue(true), Category("Document"), ScriptControlProperty, ClientPropertyName("enabled"), Description("是否可用")]
        public override bool Enabled
        {
            get
            {
                return base.Enabled;
            }
            set
            {
                base.Enabled = value;
            }
        }

        /// <summary>
        /// 是否保留修改痕迹
        /// </summary>
        [DefaultValue(false), Category("Document"), ScriptControlProperty, ClientPropertyName("trackRevisions"), Description("是否保留修改痕迹")]
        public bool TrackRevisions
        {
            get
            {
                return GetPropertyValue<bool>("TrackRevisions", true);
            }
            set
            {
                SetPropertyValue<bool>("TrackRevisions", value);
            }
        }

        /// <summary>
        /// 是否显示文件标题
        /// </summary>
        [DefaultValue(true), Category("Appearance"), ScriptControlProperty, ClientPropertyName("showFileTitle"), Description("是否显示文件标题")]
        public bool ShowFileTitle
        {
            get
            {
                return GetPropertyValue<bool>("ShowFileTitle", true);
            }
            set
            {
                SetPropertyValue<bool>("ShowFileTitle", value);
            }
        }

        /// <summary>
        /// 是否检查文件已上传
        /// </summary>
        [DefaultValue(true), Category("Document"), ScriptControlProperty, ClientPropertyName("modifyCheck"), Description("是否检查文件已上传")]
        public bool ModifyCheck
        {
            get
            {
                return GetPropertyValue<bool>("ModifyCheck", true);
            }
            set
            {
                SetPropertyValue<bool>("ModifyCheck", value);
            }
        }

        /// <summary>
        /// 是否显示所有版本
        /// </summary>
        [DefaultValue(false), Category("Appearance"), ScriptControlProperty, ClientPropertyName("showAllVersions"), Description("是否显示所有版本")]
        public bool ShowAllVersions
        {
            get
            {
                return GetPropertyValue<bool>("ShowAllVersions", false);
            }
            set
            {
                SetPropertyValue<bool>("ShowAllVersions", value);
            }
        }

        /// <summary>
        /// 模板路径
        /// </summary>
        [DefaultValue(""), Category("Document"), ScriptControlProperty, UrlProperty, ClientPropertyName("templateUrl"), Description("模板路径")]
        public string TemplateUrl
        {
            get
            {
                return GetPropertyValue<string>("TemplateUrl", string.Empty);
            }
            set
            {
                SetPropertyValue<string>("TemplateUrl", value);
            }
        }

        /// <summary>
        /// 服务器上保存文件的根路径的配置节点名称
        /// </summary>
        [DefaultValue(""), Category("Document"), ScriptControlProperty, ClientPropertyName("rootPathName"), Description("服务器上保存文件的根路径的配置节点名称")]
        public string RootPathName
        {
            get
            {
                return GetPropertyValue<string>("RootPathName", string.Empty);
            }
            set
            {
                SetPropertyValue<string>("RootPathName", value);
            }
        }

        /// <summary>
        /// 服务器上保存文件的目录(相对路径)
        /// </summary>
        [DefaultValue(""), Category("Document"), ScriptControlProperty, ClientPropertyName("relativePath"), Description("服务器上保存文件的目录(相对路径)")]
        public string RelativePath
        {
            get
            {
                return GetPropertyValue<string>("RelativePath", string.Empty);
            }
            set
            {
                ExceptionHelper.TrueThrow<ArgumentNullException>(value == null, "RelativePath");
                ExceptionHelper.TrueThrow(value.IndexOfAny(new char[] { ':', '*', '?', '"', '<', '>', '|' }) != -1, string.Format(Resources.IllegalPath, value));

                SetPropertyValue<string>("RelativePath", value);
            }
        }

        /// <summary>
        /// 文档被打开时的客户端事件代码
        /// </summary>
        [DefaultValue(""), ScriptControlEvent, ClientPropertyName("onDocumentOpen"), Bindable(true), Category("ClientEventsHandler"), Description("文档被打开时的客户端事件代码")]
        public string OnDocumentOpen
        {
            get
            {
                if (!string.IsNullOrEmpty(RenderMode.RenderArgument))
                    return string.Empty;

                return GetPropertyValue("OnDocumentOpen", string.Empty);
            }
            set
            {
                SetPropertyValue("OnDocumentOpen", value);
            }
        }

        /// <summary>
        /// 客户端附件改变后触发的事件
        /// </summary>
        [DefaultValue("")]
        [ScriptControlEvent]
        [ClientPropertyName("materialsChanged")]
        [Bindable(true), Category("ClientEventsHandler"), Description("客户端附件改变后触发的事件")]
        public string OnClientMaterialsChanged
        {
            get
            {
                if (!string.IsNullOrEmpty(RenderMode.RenderArgument))
                    return string.Empty;

                return GetPropertyValue("OnClientMaterialsChanged", string.Empty);
            }

            set
            {
                SetPropertyValue("OnClientMaterialsChanged", value);
            }
        }

        /// <summary>
        /// 文档下载完成时的客户端事件代码
        /// </summary>
        [DefaultValue(""), ScriptControlEvent, ClientPropertyName("onDocumentDownload"), Bindable(true), Category("ClientEventsHandler"), Description("文档下载完成时的客户端事件代码")]
        public string OnDocumentDownload
        {
            get
            {
                if (!string.IsNullOrEmpty(RenderMode.RenderArgument))
                    return string.Empty;

                return GetPropertyValue("OnDocumentDownload", string.Empty);
            }
            set
            {
                SetPropertyValue("OnDocumentDownload", value);
            }
        }

        /// <summary>
        /// 使用方式
        /// </summary>
        [DefaultValue(MaterialUseMode.UploadFile), Category("Appearance"), ScriptControlProperty, ClientPropertyName("materialUseMode"), Description("使用方式")]
        public MaterialUseMode MaterialUseMode
        {
            get
            {
                return GetPropertyValue<MaterialUseMode>("MaterialUseMode", MaterialUseMode.UploadFile);
            }
            set
            {
                SetPropertyValue<MaterialUseMode>("MaterialUseMode", value);
            }
        }

        /// <summary>
        /// 文件选择方式
        /// </summary>
        [DefaultValue(FileSelectMode.MultiSelectUseActiveX), Category("Behavior"), ScriptControlProperty, ClientPropertyName("fileSelectMode"), Description("文件选择方式")]
        public FileSelectMode FileSelectMode
        {
            get
            {
                return GetPropertyValue<FileSelectMode>("FileSelectMode", FileSelectMode.MultiSelectUseActiveX);
            }
            set
            {
                SetPropertyValue<FileSelectMode>("FileSelectMode", value);
            }
        }

        /// <summary>
        /// 附件列表显示模式
        /// </summary>
        [DefaultValue(MaterialTableShowMode.Vertical), Category("Appearance"), ScriptControlProperty, ClientPropertyName("materialTableShowMode"), Description("附件列表显示模式")]
        public MaterialTableShowMode MaterialTableShowMode
        {
            get
            {
                return GetPropertyValue<MaterialTableShowMode>("MaterialTableShowMode", MaterialTableShowMode.Vertical);
            }
            set
            {
                SetPropertyValue<MaterialTableShowMode>("MaterialTableShowMode", value);
            }
        }

        /// <summary>
        /// "添加或修改" 显示为文字还是图片
        /// </summary>
        [DefaultValue(LinkShowMode.Text), Category("Appearance"), ScriptControlProperty, ClientPropertyName("linkShowMode"), Description("\"添加或修改\" 显示为文字还是图片")]
        public LinkShowMode LinkShowMode
        {
            get
            {
                return GetPropertyValue<LinkShowMode>("LinkShowMode", LinkShowMode.Text);
            }
            set
            {
                SetPropertyValue<LinkShowMode>("LinkShowMode", value);
            }
        }

        /// <summary>
        /// 是否显示设置文件打开方式的图标
        /// </summary>
        [DefaultValue(false), Category("Appearance"), ScriptControlProperty, ClientPropertyName("showFileOpenType"), Description("是否显示设置文件打开方式的图标")]
        public bool ShowFileOpenType
        {
            get
            {
                return GetPropertyValue<bool>("ShowFileOpenType", false);
            }
            set
            {
                SetPropertyValue<bool>("ShowFileOpenType", value);
            }
        }

        /// <summary>
        /// 是否在页面关闭的时候自动校验文件正在打开或者曾经修改
        /// </summary>
        [DefaultValue(true), Category("Appearance"), ScriptControlProperty, ClientPropertyName("autoCheck"), Description("是否在页面关闭的时候自动校验文件正在打开或者曾经修改")]
        public bool AutoCheck
        {
            get
            {
                return GetPropertyValue<bool>("AutoCheck", true);
            }
            set
            {
                SetPropertyValue<bool>("AutoCheck", value);
            }
        }

        /// <summary>
        /// 点击后弹出上传文件爱你对话框的控件ID
        /// </summary>
        [DefaultValue(""), IDReferenceProperty(), TypeConverter(typeof(DialogStartUpControlConverter))]
        public string ControlIDToShowDialog
        {
            get
            {
                return GetPropertyValue<string>("ControlIDToShowDialog", string.Empty);
            }
            set
            {
                SetPropertyValue<string>("ControlIDToShowDialog", value);
                this.controlToShowDialog = null;
            }
        }

        /// <summary>
        /// 可以上传的文件的拓展名(格式示例: 文本文件(txt)|*.txt|office文档(.doc,.xls)|*.doc;*.xls||。 注意　末尾是||
        /// </summary>
        [DefaultValue(""), Category("Document"), ScriptControlProperty, ClientPropertyName("fileExts")]
        [Description("可以上传的文件的拓展名(格式示例: 文本文件(txt)|*.txt|office文档(.doc,.xls)|*.doc;*.xls||。 注意　末尾是||) ")]
        public string FileExts
        {
            get
            {
                return GetPropertyValue<string>("FileExts", string.Empty);
            }
            set
            {
                SetPropertyValue<string>("FileExts", value);
            }
        }

        /// <summary>
        /// 文件的大小限制  单位是字节
        /// </summary>
        [DefaultValue(0), Category("Document"), ScriptControlProperty, ClientPropertyName("fileMaxSize"), Description(" 文件的大小限制  单位是字节")]
        public int FileMaxSize
        {
            get
            {
                return GetPropertyValue<int>("FileMaxSize", 0);
            }
            set
            {
                SetPropertyValue<int>("FileMaxSize", value);
            }
        }

        /// <summary>
        /// 文件的个数限制
        /// </summary>
        [DefaultValue(0), Category("Document"), ScriptControlProperty, ClientPropertyName("fileCountLimited"), Description("文件的个数限制")]
        public int FileCountLimited
        {
            get
            {
                return GetPropertyValue<int>("FileCountLimited", 0);
            }
            set
            {
                SetPropertyValue<int>("FileCountLimited", value);
            }
        }

        /// <summary>
        /// OfficeViewer宽
        /// </summary>
        [DefaultValue("100%"), Category("Document")]
        public Unit OfficeViewerWidth
        {
            get
            {
                return GetPropertyValue<Unit>("OfficeViewerWidth", Unit.Percentage(100));
            }
            set
            {
                SetPropertyValue<Unit>("OfficeViewerWidth", value);
            }
        }

        /// <summary>
        /// OfficeViewer高
        /// </summary>
        [Category("Document")]
        public Unit OfficeViewerHeight
        {
            get
            {
                return GetPropertyValue<Unit>("OfficeViewerHeight", Unit.Empty);
            }
            set
            {
                SetPropertyValue<Unit>("OfficeViewerHeight", value);
            }
        }

        /// <summary>
        /// OfficeViewer是否显示工具栏
        /// </summary>
        //[Category("Document"), ScriptControlProperty, ClientPropertyName("officeViewerShowToolBars"), Description("OfficeViewer是否显示工具栏")]
        //public bool OfficeViewerShowToolBars
        //{
        //    get
        //    {
        //        return GetPropertyValue<bool>("OfficeViewerShowToolBars", false);
        //    }
        //    set
        //    {
        //        SetPropertyValue<bool>("OfficeViewerShowToolBars", value);
        //    }
        //}

        #endregion

        #region 暴露的属性

        /// <summary>
        /// 附件所属表单
        /// </summary>
        [ScriptControlProperty, ClientPropertyName("defaultResourceID"), Browsable(false)]
        public string DefaultResourceID
        {
            get
            {
                string defaultResourceID = GetPropertyValue("DefaultResourceID", string.Empty);

                if (string.IsNullOrEmpty(defaultResourceID))
                {
                    if (this.AllowEdit || this.AllowEditContent)
                    {
                        if (WfClientContext.Current.OriginalActivity != null)
                            defaultResourceID = WfClientContext.Current.OriginalActivity.Process.ResourceID;

                        if (string.IsNullOrEmpty(defaultResourceID) == false)
                            SetPropertyValue("DefaultResourceID", defaultResourceID);
                    }
                }

                return defaultResourceID;
            }
            set
            {
                SetPropertyValue<string>("DefaultResourceID", value);
            }
        }

        /// <summary>
        /// 附件在应用中的类别
        /// </summary>
        [ScriptControlProperty, ClientPropertyName("defaultClass"), Browsable(true)]
        public string DefaultClass
        {
            get
            {
                return GetPropertyValue<string>("DefaultClass", string.Empty);
            }
            set
            {
                SetPropertyValue<string>("DefaultClass", value);
            }
        }

        /// <summary>
        /// 文件对象集合
        /// </summary>
        [Browsable(false)]
        public MaterialList Materials
        {
            get
            {
                return this.materials;
            }
            set
            {
                value.NullCheck("Materials");
                this.materials = value;
            }
        }

        /// <summary>
        /// 保存文件操作结果的集合
        /// </summary>
        [Browsable(false)]
        public DeltaMaterialList DeltaMaterials
        {
            get
            {
                if (this.deltaMaterials.RootPathName != RootPathName)
                    this.deltaMaterials.RootPathName = RootPathName;

                return this.deltaMaterials;
            }
        }

        /// <summary>
        /// 工作流流程ID
        /// </summary>
        [ScriptControlProperty, ClientPropertyName("wfProcessID"), Browsable(false)]
        public string WfProcessID
        {
            get
            {
                string wfProcessID = GetPropertyValue("WfProcessID", string.Empty);

                if (string.IsNullOrEmpty(wfProcessID))
                {
                    if (this.AllowEdit || this.AllowEditContent)
                    {
                        if (WfClientContext.Current.OriginalActivity != null)
                            wfProcessID = WfClientContext.Current.OriginalActivity.Process.ID;

                        if (string.IsNullOrEmpty(wfProcessID) == false)
                            SetPropertyValue("WfProcessID", wfProcessID);
                    }
                }

                return wfProcessID;
            }
            set
            {
                SetPropertyValue<string>("WfProcessID", value);
            }
        }

        /// <summary>
        ///工作流活动ID
        /// </summary>
        [ScriptControlProperty, ClientPropertyName("wfActivityID"), Browsable(false)]
        public string WfActivityID
        {
            get
            {
                string wfActivityID = GetPropertyValue("WfActivityID", string.Empty);

                if (string.IsNullOrEmpty(wfActivityID))
                {
                    if (this.AllowEdit || this.AllowEditContent)
                    {
                        if (WfClientContext.Current.OriginalActivity != null)
                            wfActivityID = WfClientContext.Current.OriginalActivity.ID;

                        if (string.IsNullOrEmpty(wfActivityID) == false)
                            SetPropertyValue("WfProcessID", wfActivityID);
                    }
                }

                return wfActivityID;
            }
            set
            {
                SetPropertyValue<string>("WfActivityID", value);
            }
        }

        /// <summary>
        ///工作流活动名称
        /// </summary>
        [ScriptControlProperty, ClientPropertyName("wfActivityName"), Browsable(false)]
        public string WfActivityName
        {
            get
            {
                string wfActivityName = GetPropertyValue("WfActivityName", string.Empty);

                if (string.IsNullOrEmpty(wfActivityName))
                {
                    if (this.AllowEdit || this.AllowEditContent)
                    {
                        if (WfClientContext.Current.OriginalActivity != null && WfClientContext.Current.OriginalActivity.Descriptor != null)
                            wfActivityName = WfClientContext.Current.OriginalActivity.Descriptor.Name;

                        if (string.IsNullOrEmpty(wfActivityName) == false)
                            SetPropertyValue("WfActivityName", wfActivityName);
                    }
                }

                return wfActivityName;
            }
            set
            {
                SetPropertyValue<string>("WfActivityName", value);
            }
        }

        /// <summary>
        /// 下载模板文件请求的时候，是否带着ViewState
        /// </summary>
        [DefaultValue(false)]
        [ScriptControlProperty, ClientPropertyName("downloadTemplateWithViewState")]
        public bool DownloadTemplateWithViewState
        {
            get
            {
                return GetPropertyValue("DownloadTemplateWithViewState", false);
            }
            set
            {
                SetPropertyValue("DownloadTemplateWithViewState", value);
            }
        }

        /// <summary>
        /// 下载文件请求的时候，是否带着ViewState
        /// </summary>
        [DefaultValue(false)]
        [ScriptControlProperty, ClientPropertyName("downloadWithViewState")]
        public bool DownloadWithViewState
        {
            get
            {
                return GetPropertyValue("DownloadWithViewState", false);
            }
            set
            {
                SetPropertyValue("DownloadWithViewState", value);
            }
        }

        /// <summary>
        /// 弹出对话框的控件的实例
        /// </summary>
        [Browsable(false)]
        public IAttributeAccessor ControlToShowDialog
        {
            get
            {
                if (this.controlToShowDialog == null)
                {
                    if (string.IsNullOrEmpty(ControlIDToShowDialog) == false)
                        this.controlToShowDialog = (IAttributeAccessor)WebControlUtility.FindControlByID(Page, ControlIDToShowDialog, true);
                }

                return this.controlToShowDialog;
            }
            set
            {
                this.controlToShowDialog = value;
            }
        }

        /// <summary>
        /// 控件使用的锁ID
        /// </summary>
        [ScriptControlProperty, ClientPropertyName("lockID")]
        internal string LockID
        {
            get
            {
                string lockID = GetPropertyValue("LockID", string.Empty);

                if (string.IsNullOrEmpty(lockID))
                {
                    //Lock lockEntity = LockContext.Current.Locks[LockType.FormLock];
                    Lock lockEntity = WfClientContext.Current.Locks[LockType.FormLock];
                    if (lockEntity != null)
                    {
                        SetPropertyValue("LockID", lockEntity.LockID);
                        lockID = lockEntity.LockID;
                    }
                }

                return lockID;
            }
            set
            {
                SetPropertyValue<string>("LockID", value);
            }
        }

        private static readonly object DownloadEventKey = new object();
        private static readonly object BeforeUploadEventKey = new object();
        private static readonly object AfterUploadEventKey = new object();

        /// <summary>
        /// 下载文件的事件
        /// </summary>
        public event DownloadHandler Download
        {
            add
            {
                this.Events.AddHandler(DownloadEventKey, value);
            }
            remove
            {
                this.Events.RemoveHandler(DownloadEventKey, value);
            }
        }

        /// <summary>
        /// 上传文件之前的事件
        /// </summary>
        public event UploadHandler BeforeUpload
        {
            add
            {
                this.Events.AddHandler(BeforeUploadEventKey, value);
            }
            remove
            {
                this.Events.RemoveHandler(BeforeUploadEventKey, value);
            }
        }

        /// <summary>
        /// 上传文件之后的事件
        /// </summary>
        public event UploadHandler AfterUpload
        {
            add
            {
                this.Events.AddHandler(AfterUploadEventKey, value);
            }
            remove
            {
                this.Events.RemoveHandler(AfterUploadEventKey, value);
            }
        }

        private MaterialControl dynamicControl;

        /// <summary>
        /// 动态产生的控件
        /// </summary>
        [Browsable(false)]
        public MaterialControl DynamicControl
        {
            get
            {
                return this.dynamicControl;
            }
            set
            {
                this.dynamicControl = value;
            }
        }

        #endregion

        #region 内部使用

        /// <summary>
        /// 用户
        /// </summary>
        [ScriptControlProperty, ClientPropertyName("user")]
        private IUser User
        {
            get
            {
                return GetCurrentUser();
            }
        }

        private static IUser GetCurrentUser()
        {
            if (DeluxePrincipal.IsAuthenticated)
            {
                var user = new OguUser(DeluxeIdentity.CurrentRealUser);
                return user;
            }
            else
            {
                var newUser = CreateNewUser();
                return newUser;
            }
        }

        private static IUser CreateNewUser()
        {
            return new OguUser
            {
                DisplayName = "newuser",
                Email = "",
                FullPath = "newuser",
                GlobalSortID = "",
                ID = "newuser",
                IsSideline = false,
                Levels = -1,
                LogOnName = "newuser",
                Name = "newuser",
                ObjectType = MCS.Library.OGUPermission.SchemaType.Users,
                Rank = 0,
                SortID = ""
            };
        }

        /// <summary>
        /// 部门
        /// </summary>
        [ScriptControlProperty, ClientPropertyName("department")]
        private IOrganization Department
        {
            get
            {
                return GetCurrentDepartment();
            }
        }

        private static IOrganization GetCurrentDepartment()
        {
            if (DeluxePrincipal.IsAuthenticated)
            {
                return new OguOrganization(DeluxeIdentity.CurrentRealUser.TopOU);
            }
            else
            {
                return null;
            }
        }

        /// <summary>
        /// 文件上传弹出对话框的路径
        /// </summary>
        [ScriptControlProperty(), ClientPropertyName("dialogUploadFileControlUrl")]
        private string DialogUploadFileControlUrl
        {
            get
            {
                if (this.MaterialUseMode != MaterialUseMode.SingleDraft)
                {
                    PageRenderMode pageRenderMode
                        = new PageRenderMode(this.dynamicControl == null ? this.UniqueID : this.dynamicControl.UniqueID, "DialogUploadFileControl");

                    return NormalizeDialogRequestUrl(WebUtility.GetRequestExecutionUrl(pageRenderMode));
                }
                else
                {
                    return string.Empty;
                }
            }
        }

        /// <summary>
        /// 文件上传弹出对话框(传统方式)的路径
        /// </summary>
        [ScriptControlProperty(), ClientPropertyName("dialogUploadFileTraditionalControlUrl")]
        private string DialogUploadFileTraditionalControlUrl
        {
            get
            {
                if (this.MaterialUseMode != MaterialUseMode.SingleDraft)
                {
                    PageRenderMode pageRenderMode
                        = new PageRenderMode(this.dynamicControl == null ? this.UniqueID : this.dynamicControl.UniqueID, "DialogUploadFileTraditionalControl");

                    return NormalizeDialogRequestUrl(WebUtility.GetRequestExecutionUrl(pageRenderMode));
                }
                else
                {
                    return string.Empty;
                }
            }
        }

        /// <summary>
        /// 设置默认在IE中打开的文件类型对话框的路径
        /// </summary>
        [ScriptControlProperty(), ClientPropertyName("dialogFileOpenTypeControlUrl")]
        private string DialogFileOpenTypeControlUrl
        {
            get
            {
                if (this.MaterialUseMode != MaterialUseMode.SingleDraft)
                {
                    PageRenderMode pageRenderMode
                       = new PageRenderMode(this.dynamicControl == null ? this.UniqueID : this.dynamicControl.UniqueID, "DialogFileOpenTypeControl");

                    return NormalizeDialogRequestUrl(WebUtility.GetRequestExecutionUrl(pageRenderMode));
                }
                else
                {
                    return string.Empty;
                }
            }
        }

        /// <summary>
        /// 版本弹出对话框的路径
        /// </summary>
        [ScriptControlProperty(), ClientPropertyName("dialogVersionControlUrl")]
        private string DialogVersionControlUrl
        {
            get
            {
                PageRenderMode pageRenderMode
                    = new PageRenderMode(this.dynamicControl == null ? this.UniqueID : this.dynamicControl.UniqueID, "DialogVersionControl");

                return NormalizeDialogRequestUrl(WebUtility.GetRequestExecutionUrl(pageRenderMode));
            }
        }

        /// <summary>
        /// 等待文件上传弹出对话框的路径
        /// </summary>
        [ScriptControlProperty(), ClientPropertyName("dialogUploadFileProcessControlUrl")]
        private string DialogUploadFileProcessControlUrl
        {
            get
            {
                PageRenderMode pageRenderMode
                   = new PageRenderMode(this.dynamicControl == null ? this.UniqueID : this.dynamicControl.UniqueID, "DialogUploadFileProcessControl");

                return NormalizeDialogRequestUrl(WebUtility.GetRequestExecutionUrl(pageRenderMode));
            }
        }

        /// <summary>
        /// 文档编辑传弹出对话框的路径
        /// </summary>
        [ScriptControlProperty(), ClientPropertyName("dialogEditDocumentControlUrl")]
        private string DialogEditDocumentControlUrl
        {
            get
            {
                if (!this.EditDocumentInCurrentPage || this.MaterialUseMode != MaterialUseMode.SingleDraft)
                {
                    PageRenderMode pageRenderMode
                        = new PageRenderMode(
                            this.dynamicControl == null ? this.UniqueID : this.dynamicControl.UniqueID,
                            "DialogEditDocumentControl");

                    return NormalizeDialogRequestUrl(WebUtility.GetRequestExecutionUrl(pageRenderMode));
                }
                else
                {
                    return string.Empty;
                }
            }
        }

        /// <summary>
        /// 控件ID
        /// </summary>
        [ScriptControlProperty(), ClientPropertyName("controlID")]
        private string ControlID
        {
            get
            {
                return this.ClientID;
            }
        }

        /// <summary>
        /// 控件ID
        /// </summary>
        [ScriptControlProperty(), ClientPropertyName("uniqueID")]
        private string ControlUniqueID
        {
            get
            {
                return this.UniqueID;
            }
        }

        /// <summary>
        /// OfficeViewerWrapper控件ID
        /// </summary>
        [ScriptControlProperty(), ClientPropertyName("officeViewerWrapperID")]
        private string OfficeViewerWrapperID
        {
            get
            {
                return this.officeViewerWrapper.ClientID;
            }
        }

        /// <summary>
        /// 表示编辑状态的图标路径
        /// </summary>
        [ScriptControlProperty, ClientPropertyName("editImagePath")]
        private string EditImagePath
        {
            get
            {
                return this.Page.ClientScript.GetWebResourceUrl(typeof(MCS.Web.WebControls.MaterialControl),
                    "MCS.Web.WebControls.Images.edit.gif");
            }
        }

        /// <summary>
        /// 表示未上传状态的图标路径
        /// </summary>
        [ScriptControlProperty, ClientPropertyName("uploadImagePath")]
        private string UploadImagePath
        {
            get
            {
                return this.Page.ClientScript.GetWebResourceUrl(typeof(MCS.Web.WebControls.MaterialControl),
                    "MCS.Web.WebControls.Images.upload.gif");
            }
        }

        /// <summary>
        /// 空白图片的路径
        /// </summary>
        [ScriptControlProperty, ClientPropertyName("emptyImagePath")]
        private string EmptyImagePath
        {
            get
            {
                return this.Page.ClientScript.GetWebResourceUrl(typeof(MCS.Web.WebControls.MaterialControl),
                    "MCS.Web.WebControls.Images.empty.gif");
            }
        }

        /// <summary>
        /// 默认文件图标的路径
        /// </summary>
        [ScriptControlProperty, ClientPropertyName("defaultFileIconPath")]
        private string DefaultFileIconPath
        {
            get
            {
                ContentTypeConfigElement elem = ContentTypesSection.GetConfig().DefaultElement;

                if (elem != null)
                    return elem.LogoImage;
                else
                    return this.Page.ClientScript.GetWebResourceUrl(typeof(MCS.Web.WebControls.MaterialControl),
                        "MCS.Web.WebControls.Images.default.gif");
            }
        }

        /// <summary>
        /// 显示版本的图标路径
        /// </summary>
        [ScriptControlProperty, ClientPropertyName("showVersionImagePath")]
        private string ShowVersionImagePath
        {
            get
            {
                return this.Page.ClientScript.GetWebResourceUrl(typeof(MCS.Web.WebControls.MaterialControl),
                    "MCS.Web.WebControls.Images.version.gif");
            }
        }

        /// <summary>
        /// 显示默认打开方式页面的图标路径
        /// </summary>
        [ScriptControlProperty, ClientPropertyName("setOpenTypeImagePath")]
        private string SetOpenTypeImagePath
        {
            get
            {
                return this.Page.ClientScript.GetWebResourceUrl(typeof(MCS.Web.WebControls.MaterialControl),
                    "MCS.Web.WebControls.Images.setOpenType.gif");
            }
        }

        /// <summary>
        /// "添加或修改"显示为图片时的 图片路径
        /// </summary>
        [ScriptControlProperty, ClientPropertyName("captionImagePath")]
        private string CaptionImagePath
        {
            get
            {
                return this.Page.ClientScript.GetWebResourceUrl(typeof(MCS.Web.WebControls.MaterialControl),
                    "MCS.Web.WebControls.Images.material.gif");
            }
        }

        /// <summary>
        /// 处理请求的页面地址
        /// </summary>
        [ScriptControlProperty, ClientPropertyName("currentPageUrl")]
        private string CurrentPageUrl
        {
            get
            {
                Page page = (Page)HttpContext.Current.CurrentHandler;

                return page.ResolveUrl(page.AppRelativeVirtualPath);
            }
        }

        /// <summary>
        /// 是否指定了弹出对话框的控件
        /// </summary>
        [ScriptControlProperty, ClientPropertyName("designatedControlToShowDialog")]
        private bool DesignatedControlToShowDialog
        {
            get
            {
                return this.ControlToShowDialog != null;
            }
        }

        [ScriptControlProperty, ClientPropertyName("openInlineFileExt")]
        private string OpenInlineFileExt
        {
            get
            {
                if (this.User == null)
                {
                    return "SOAUserID";
                }
                return FileConfigHelper.GetOpenInlineFileExts(this.User.ID);
            }
        }

        #endregion

        #region 事件

        /// <summary>
        /// 下载文件时发生
        /// </summary>
        [Category("Behavior"), Description("下载文件时发生")]
        public delegate void DownloadHandler(object sender, DownloadEventArgs e);

        [Category("Behavior"), Description("下载文件前发生")]
        public event BeforeFileDownloadHandler BeforeFileDownload;

        [Category("Behavior"), Description("准备下载文件流下载文件时发生")]
        public event PrepareDownloadStreamHandler PrepareDownloadStream;

        internal virtual void OnDownloadFile(DownloadEventArgs e)
        {
            DownloadHandler handler = (DownloadHandler)Events[DownloadEventKey];

            if (handler != null)
                handler(this, e);
        }

        /// <summary>
        /// 上传的文件保存在服务器之前或之后发生
        /// </summary>
        [Category("Behavior"), Description("上传的文件保存在服务器之前或之后发生")]
        public delegate void UploadHandler(object sender, UploadEventArgs e);

        internal virtual void OnBeforeUploadFile(UploadEventArgs e)
        {
            UploadHandler handler = (UploadHandler)Events[BeforeUploadEventKey];

            if (handler != null)
                handler(this, e);
        }

        internal virtual void OnAfterUploadFile(UploadEventArgs e)
        {
            UploadHandler handler = (UploadHandler)Events[AfterUploadEventKey];

            if (handler != null)
                handler(this, e);
        }

        #endregion

        #endregion

        #region 公共方法

        /// <summary>
        /// 处理客户端的请求
        /// </summary>
        /// <returns>操作是否成功</returns>
        private bool ProcessRequest()
        {
            bool result = FileUpload.Processed;

            if (result == false)
            {
                HttpRequest request = HttpContext.Current.Request;
                string requestType = HttpUtility.UrlEncode(WebUtility.GetRequestQueryValue<string>("requestType", string.Empty));

                if (requestType.IsNotEmpty())
                {
                    string controlID = WebUtility.GetRequestQueryValue("controlID", string.Empty);
                    bool raiseEvent = false;

                    if (this.Page.FindControl(control => control.UniqueID == controlID, true) != null)
                    {
                        //如果找到controlID所对应的控件，则判断是否和当前控件ID匹配，如果不匹配，则退出，否则由此控件处理事件
                        if (controlID != this.ControlUniqueID)
                            return false;
                        else
                            raiseEvent = true;
                    }

                    result = true;

                    HttpContext.Current.Response.Clear();

                    if (requestType.ToLower() == "download")
                    {
                        ProcessDownloadRequest(raiseEvent);
                    }
                    else if (requestType.ToLower() == "upload")
                    {
                        FileUpload.Upload();
                    }
                }
            }

            return result;
        }

        private void ProcessDownloadRequest(bool raiseEvent)
        {
            string opType = WebUtility.GetRequestQueryString("opType", string.Empty);

            if (opType == "Document")
            {
                if (this.DownloadWithViewState == false)
                    FileUpload.Download(this, BeforeFileDownload, PrepareDownloadStream, raiseEvent);
                else
                    this.delayProcessDownload = true;
            }
            else
            {
                if (this.DownloadTemplateWithViewState == false)
                    FileUpload.Download(this, BeforeFileDownload, PrepareDownloadStream, raiseEvent);
                else
                    this.delayProcessDownload = true;
            }
        }

        /// <summary>
        /// 入库后，应用调用此方法清除deltaMaterials,并还原material的ShowFileUrl
        /// </summary>
        public void ClearDeltaMaterials()
        {
            this.deltaMaterials.Clear();

            foreach (Material material in this.materials)
            {
                material.ShowFileUrl = string.Empty;
            }
        }

        #endregion

        #region 重写的方法

        protected override void CreateChildControls()
        {
            Controls.Clear();

            this.Controls.Add(this.dialogUploadFileControl);
            this.Controls.Add(this.dialogUploadFileTraditonalControl);
            this.Controls.Add(this.dialogMaterialVersionControl);
            this.Controls.Add(this.dialogFileOpenTypeControl);
            this.Controls.Add(this.dialogUploadFileProcessControl);
            //this.Controls.Add(this.dialogEditDocumentControl);
            this.Controls.Add(this.officeViewerWrapper);
            this.Controls.Add(this.componentHelperWrapper);
        }

        protected override void OnInit(EventArgs e)
        {
            Page.LoadComplete += new EventHandler(Page_LoadComplete);

            if (ProcessGetNewMaterialInfoRequest() || ProcessGetLastUploadTagRequest() || ProcessRequest())
            {
                return;
            }

            DeltaDataControlHelper.RegisterDeltaDataControl(this);

            base.OnInit(e);
        }

        private void Page_LoadComplete(object sender, EventArgs e)
        {
            if (this.delayProcessDownload)
                FileUpload.Download(this, BeforeFileDownload, PrepareDownloadStream, true);
        }

        private bool ProcessGetLastUploadTagRequest()
        {
            HttpRequest request = HttpContext.Current.Request;
            string requestType = HttpUtility.UrlEncode(WebUtility.GetRequestQueryValue<string>("requestType", string.Empty));

            if (requestType == "getlastuploadtag")
            {
                HttpResponse response = HttpContext.Current.Response;
                response.Clear();
                response.ContentType = "application/javascript";
                string dateTimeNow = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss.fff");
                response.Write(dateTimeNow);
                response.End();

                return true;
            }

            return false;
        }

        private bool ProcessGetNewMaterialInfoRequest()
        {
            HttpRequest request = HttpContext.Current.Request;
            string requestType = HttpUtility.UrlEncode(WebUtility.GetRequestQueryValue<string>("requestType", string.Empty));

            if (requestType == "getnewmaterialinfo")
            {
                HttpResponse response = HttpContext.Current.Response;
                response.Clear();
                response.ContentType = "application/javascript";

                string newID = UuidHelper.NewUuidString();

                string fileName = HttpUtility.UrlEncode(WebUtility.GetRequestQueryValue<string>("fileName", string.Empty));
                string IconPath = FileConfigHelper.GetFileIconPath(fileName);

                var obj = new { materialID = newID, fileIconPath = IconPath, uploadTag = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss.fff") };

                response.Write(JSONSerializerExecute.Serialize(obj));
                response.End();

                return true;
            }

            return false;
        }

        protected override void OnLoad(EventArgs e)
        {
            EnsureChildControls();

            base.OnLoad(e);
        }

        protected override void Render(HtmlTextWriter writer)
        {
            if (DesignMode)
            {
                Controls.Clear();
                LinkButton link = new LinkButton();
                link.Text = "添加或修改";
                Controls.Add(link);
                base.Render(writer);
            }
            else
            {
                officeViewerWrapper.Visible = false;
                if (!RenderMode.OnlyRenderSelf)
                {
                    base.Render(writer);
                    if (this.Enabled && !this.ReadOnly && this.AllowEdit && this.AllowEditContent && this.MaterialUseMode == WebControls.MaterialUseMode.SingleDraft && this.EditDocumentInCurrentPage)
                    {
                        officeViewerWrapper.Width = OfficeViewerWidth;
                        officeViewerWrapper.Height = OfficeViewerHeight;
                        officeViewerWrapper.Visible = true;
                        this.officeViewerWrapper.RenderControl(writer);
                    }
                }
                else
                {
                    base.Render(writer);
                }
            }
        }

        protected override void OnPreRender(EventArgs e)
        {
            base.OnPreRender(e);
            this.componentHelperWrapper.Visible = false;
            this.officeViewerWrapper.Visible = false;
            if (RenderMode.OnlyRenderSelf)
            {
                this.ReadOnly = true;
                this.Enabled = true;

                switch (RenderMode.RenderArgument)
                {
                    case "DialogUploadFileControl":
                        this.dialogMaterialVersionControl.Visible = false;
                        this.dialogFileOpenTypeControl.Visible = false;
                        this.dialogUploadFileTraditonalControl.Visible = false;
                        this.dialogUploadFileProcessControl.Visible = false;
                        //this.dialogEditDocumentControl.Visible = false;
                        this.Style.Clear();
                        this.Page.Title = Translator.Translate(Define.DefaultCulture, "批量上传文件");
                        break;
                    case "DialogUploadFileTraditionalControl":
                        this.dialogMaterialVersionControl.Visible = false;
                        this.dialogFileOpenTypeControl.Visible = false;
                        this.dialogUploadFileControl.Visible = false;
                        this.dialogUploadFileProcessControl.Visible = false;
                        //this.dialogEditDocumentControl.Visible = false;
                        this.Style.Clear();
                        this.Page.Title = Translator.Translate(Define.DefaultCulture, "上传文件");
                        break;
                    case "DialogVersionControl":
                        this.dialogUploadFileControl.Visible = false;
                        this.dialogUploadFileTraditonalControl.Visible = false;
                        this.dialogFileOpenTypeControl.Visible = false;
                        this.dialogUploadFileProcessControl.Visible = false;
                        //this.dialogEditDocumentControl.Visible = false;
                        this.Page.Title = Translator.Translate(Define.DefaultCulture, "文件版本");
                        break;
                    case "DialogFileOpenTypeControl":
                        HttpContext.Current.Response.Cache.SetCacheability(HttpCacheability.NoCache);
                        this.dialogMaterialVersionControl.Visible = false;
                        this.dialogUploadFileControl.Visible = false;
                        this.dialogUploadFileTraditonalControl.Visible = false;
                        this.dialogUploadFileProcessControl.Visible = false;
                        //this.dialogEditDocumentControl.Visible = false;
                        this.Page.Title = Translator.Translate(Define.DefaultCulture, "设置以内嵌方式打开的文档类型");
                        break;
                    case "DialogUploadFileProcessControl":
                        this.dialogMaterialVersionControl.Visible = false;
                        this.dialogUploadFileControl.Visible = false;
                        this.dialogUploadFileTraditonalControl.Visible = false;
                        this.dialogFileOpenTypeControl.Visible = false;
                        //this.dialogEditDocumentControl.Visible = false;
                        this.Style.Clear();
                        this.Page.Title = Translator.Translate(Define.DefaultCulture, "正在上传文件");
                        break;
                    //case "DialogEditDocumentControl":
                    //    this.dialogMaterialVersionControl.Visible = false;
                    //    this.dialogUploadFileControl.Visible = false;
                    //    this.dialogUploadFileTraditonalControl.Visible = false;
                    //    this.dialogFileOpenTypeControl.Visible = false;
                    //    this.dialogUploadFileProcessControl.Visible = false;
                    //    this.officeViewerWrapper.Visible = false;
                    //    this.Style.Clear();
                    //    this.Width = Unit.Empty;
                    //    this.Page.Title = Translator.Translate(Define.DefaultCulture, "编辑文档");
                    //    break;
                    default:
                        this.dialogMaterialVersionControl.Visible = false;
                        this.dialogUploadFileControl.Visible = false;
                        this.dialogUploadFileTraditonalControl.Visible = false;
                        this.dialogFileOpenTypeControl.Visible = false;
                        this.dialogUploadFileProcessControl.Visible = false;
                        //this.dialogEditDocumentControl.Visible = false;
                        this.officeViewerWrapper.Visible = false;
                        break;
                }
            }
            else
            {
                if (this.RenderMode.UseNewPage == false)
                {
                    ExceptionHelper.TrueThrow(this.MaterialUseMode == MaterialUseMode.SingleDraft && this.Materials.Count > 1,
                        string.Format(Resources.MaterialControlCheckMode, "SingleDraft", "Materials不允许包含多个文件"));

                    ExceptionHelper.TrueThrow(
                        this.MaterialUseMode != MaterialUseMode.UploadFile && this.Materials.Count == 0 && string.IsNullOrEmpty(this.TemplateUrl),
                        string.Format(Resources.MaterialControlCheckMode, "SingleDraft或DraftAndUpload", "TemplateUrl不允许为空"));

                    if (this.Visible && this.Page.Items.Contains("materialSubmitValidator") == false && !RenderMode.OnlyRenderSelf)
                    {
                        this.Controls.Add(this.submitValidator);
                        this.Page.Items.Add("materialSubmitValidator", "exist");

                        this.submitValidator.ClientValidationFunction = "$HBRootNS.MaterialControl.materialClientValidate";
                    }

                    this.dialogMaterialVersionControl.Visible = false;
                    this.dialogUploadFileControl.Visible = false;
                    this.dialogUploadFileTraditonalControl.Visible = false;
                    this.dialogFileOpenTypeControl.Visible = false;
                    this.dialogUploadFileProcessControl.Visible = false;
                    //this.dialogEditDocumentControl.Visible = false;

                    if (!this.DesignMode)
                    {
                        //根据流程定义来控制附件控件
                        if (AutoControlEditStatusByActivity && this.MaterialUseMode != MaterialUseMode.SingleDraft)
                        {
                            if (WfClientContext.Current.CurrentActivity != null &&
                                WfClientContext.Current.CurrentActivity.Status == WfActivityStatus.Running)
                            {
                                IWfActivityDescriptor actDesp = WfClientContext.Current.CurrentActivity.Descriptor;

                                this.AllowEdit = actDesp.Properties.GetValue("AllowAddAttachments", this.AllowEdit);
                                this.AllowEditContent = actDesp.Properties.GetValue("AllowEditAttachments",
                                                                                    this.AllowEditContent);
                            }
                        }

                        this.PreloadAllImages();

                        if (this.ClientAllowEdit && this.ClientAllowEditContent)
                        {
                            this.componentHelperWrapper.Visible = true;
                            if (this.EditDocumentInCurrentPage)
                            {
                                this.officeViewerWrapper.Visible = true;
                            }
                        }
                    }
                }
                RegisterOfficeViewerClientEventHandler("BeforeDocumentSaved()", GetBeforDocumentSavedScript());
            }

            DeluxeNameTableContextCache.Instance.Add(Define.DefaultCulture, "您有正在打开的文档文件，当前的页面不能关闭，请您保存文件后再关闭页面！");
            DeluxeNameTableContextCache.Instance.Add(Define.DefaultCulture, "您编辑了文件但没有保存当前文件，请您保存文件后再关闭页面！");
        }

        protected override void OnPagePreRenderComplete(object sender, EventArgs e)
        {
            base.OnPagePreRenderComplete(sender, e);

            this.InitShowDialogControl();

            this.RegisterCommonField();
        }

        private void RegisterCommonField()
        {
            Page.ClientScript.RegisterHiddenField(MATERIALCONTROL_COMMON_HIDDENFIELD, "");
        }

        protected override void LoadViewState(object savedState)
        {
            base.LoadViewState(savedState);

            this.materials = (MaterialList)ViewState["Materials"];
            this.deltaMaterials = (DeltaMaterialList)ViewState["DeltaMaterials"];
        }

        protected override object SaveViewState()
        {
            ViewState["Materials"] = this.materials;
            ViewState["DeltaMaterials"] = this.deltaMaterials;

            return base.SaveViewState();
        }

        protected override void LoadClientState(string clientState)
        {
            MultiMaterialList multiMaterialList = JSONSerializerExecute.Deserialize<MultiMaterialList>(clientState);
            if (multiMaterialList != null && multiMaterialList.DeltaMaterials != null && multiMaterialList.DeltaMaterials.Inserted != null)
            {
                for (int i = 0; i < multiMaterialList.DeltaMaterials.Inserted.Count; i++)
                {
                    multiMaterialList.DeltaMaterials.Inserted[i].Creator = this.User;
                    multiMaterialList.DeltaMaterials.Inserted[i].Department = this.Department;
                }
            }

            multiMaterialList.DeltaMaterials.GenerateTempPhysicalFilePath();
            multiMaterialList.Materials.GenerateTempPhysicalFilePath(this.RootPathName);

            this.materials = multiMaterialList.Materials;
            this.deltaMaterials = multiMaterialList.DeltaMaterials;
        }

        protected override string SaveClientState()
        {
            MultiMaterialList multiMaterialList = new MultiMaterialList();
            multiMaterialList.Materials = this.materials;
            multiMaterialList.DeltaMaterials = this.deltaMaterials;

            return JSONSerializerExecute.Serialize(multiMaterialList);
        }

        private static string NormalizeDialogRequestUrl(string url)
        {
            NameValueCollection originalParams = UriHelper.GetUriParamsCollection(url);

            originalParams.Remove(PassportManager.TicketParamName);

            return UriHelper.CombineUrlParams(url, originalParams);
        }

        private void InitShowDialogControl()
        {
            if (this.ControlToShowDialog != null)
            {
                if (this.AllowEdit == true)
                    this.controlToShowDialog.SetAttribute("onclick", string.Format("$find('{0}').showDialog();return false;", this.ClientID));
                else
                    this.controlToShowDialog.SetAttribute("onclick", string.Format("return false;", this.ClientID));
            }
        }

        /// <summary>
        /// 预先显示所有图片
        /// </summary>
        private void PreloadAllImages()
        {
            this.PreloadImage(this.UploadImagePath, this.UploadImagePath);
            this.PreloadImage(this.EditImagePath, this.EditImagePath);
            this.PreloadImage(this.EmptyImagePath, this.EmptyImagePath);
            this.PreloadImage(this.DefaultFileIconPath, this.DefaultFileIconPath);
            this.PreloadImage(this.ShowVersionImagePath, this.ShowVersionImagePath);
            this.PreloadImage(this.CaptionImagePath, this.CaptionImagePath);

            string fileIconPath = string.Empty;
            foreach (Material material in this.materials)
            {
                fileIconPath = FileConfigHelper.GetFileIconPath(material.OriginalName);
                this.PreloadImage(fileIconPath, fileIconPath);
            }

        }

        /// <summary>
        /// 预先显示图片
        /// </summary>
        /// <param name="key">标识</param>
        /// <param name="imgSrc">路径</param>
        private void PreloadImage(string key, string imgSrc)
        {
            if (string.IsNullOrEmpty(imgSrc) == false)
            {
                Page.ClientScript.RegisterClientScriptBlock(this.GetType(), key,
                    string.Format("<img src=\"{0}\" style=\"display:none\"/>", HttpUtility.UrlPathEncode(imgSrc)));
            }
        }

        #endregion

        /// <summary>
        /// 清空deltaMaterials中的数据
        /// </summary>
        public void AcceptDeltaData()
        {
            this.deltaMaterials.Clear();
        }

        /// <summary>
        /// 与deltaMaterials相同
        /// </summary>
        [Browsable(false)]
        public DeltaDataCollectionBase DeltaData
        {
            get
            {
                return this.DeltaMaterials;
            }
        }

        private void RegisterOfficeViewerClientEventHandler(string activeXEventName, string script)
        {
            StringBuilder result = new StringBuilder();

            result.AppendFormat("<script language=\"javascript\" for=\"{0}\" event=\"{1}\">\n",
                                this.OfficeViewerWrapperID + "_Viewer",
                                activeXEventName);

            result.Append(script);
            result.Append("\n</script>\n");

            Page.ClientScript.RegisterClientScriptBlock(this.GetType(), this.OfficeViewerWrapperID + "_beforeSave",
                                                        result.ToString());
        }

        private string GetBeforDocumentSavedScript()
        {
            return string.Format("$find(\"{0}\").raiseBeforDocumentSaved();", this.ClientID);
        }
    }

    #endregion
}
