using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.Design;
using System.Drawing.Design;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Collections.Specialized;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.Script.Serialization;
using MCS.Library.Caching;
using MCS.Library.Core;
using MCS.Library.Security;
using MCS.Web.Library.Script;
using MCS.Web.Library;
using MCS.Library.Passport;
using MCS.Library.Globalization;
using MCS.Web.WebControls.Properties;
using System.Reflection;
using System.IO;
using MCS.Library.SOA.DataObjects;

[assembly: WebResource("MCS.Web.WebControls.UEditorWrapper.UEditorLogo.png", "image/x-png")]
[assembly: WebResource("MCS.Web.WebControls.UEditorWrapper.UEditorWrapper.js", "text/javascript")]
[assembly: WebResource("MCS.Web.WebControls.UEditorWrapper.UEditorWrapperBridge.js", "text/javascript")]

namespace MCS.Web.WebControls
{
    [DefaultProperty("InitialData")]
    [RequiredScript(typeof(ControlBaseScript), 1)]
    [RequiredScript(typeof(HBCommonScript), 2)]
    [RequiredScript(typeof(MaterialScript), 3)]
    [ClientScriptResource("MCS.Web.WebControls.UEditorWrapper", "MCS.Web.WebControls.UEditorWrapper.UEditorWrapper.js")]
    [ClientScriptResource("MCS.Web.WebControls.UEditorWrapper", "MCS.Web.WebControls.UEditorWrapper.UEditorWrapperBridge.js")]
    [ToolboxData("<{0}:UEditorWrapper runat=server></{0}:UEditorWrapper>")]
    public partial class UEditorWrapper : ScriptControlBase
    {
        private string _InitialData = string.Empty;

        private DocumentProperty _docProp;

        private CustomValidator _SubmitValidator = new CustomValidator();
        private UEditorConfigWrapper _EditorOptions = null;//new UEditorConfigWrapper() { Toolbars = new string[0]}; //;

        public UEditorWrapper()
            : base(true, HtmlTextWriterTag.Div)
        {
            JSONSerializerExecute.RegisterConverter(typeof(UEditorConfigWrapperConverter));
            JSONSerializerExecute.RegisterConverter(typeof(DocumentPropertyConverter));
        }

        #region Public properties
        /// <summary>
        /// 编辑器内容
        /// </summary>
        public string InitialData
        {
            get
            {
                return this._InitialData;
            }
            set
            {
                this._InitialData = value;
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

        [ScriptControlProperty, ClientPropertyName("autoDownloadUploadImages"), Description("是否自动下载并上传图片")]
        public bool AutoDownloadUploadImages
        {
            get
            {
                return GetPropertyValue<bool>("autoDownloadUploadImages", false);
            }
            set
            {
                SetPropertyValue<bool>("autoDownloadUploadImages", value);
            }
        }

        [ScriptControlProperty, ClientPropertyName("autoUploadImages"), Description("是否自动下载并上传图片")]
        public bool AutoUploadImages
        {
            get
            {
                return GetPropertyValue<bool>("autoUploadImages", false);
            }
            set
            {
                SetPropertyValue<bool>("autoUploadImages", value);
            }
        }


        [ScriptControlProperty, ClientPropertyName("readOnly"), Description("只读")]
        public new bool ReadOnly
        {
            get
            {
                return GetPropertyValue<bool>("ReadOnly", false);
            }
            set
            {
                SetPropertyValue<bool>("ReadOnly", value);
            }

        }
        #endregion

        #region Private properties
        [Browsable(false)]
        [ScriptControlProperty, ClientPropertyName("postedDataFormName")]
        private string PostedDataFormName
        {
            get
            {
                return this.ClientID + "_TextArea";
            }
        }

        [Browsable(false)]
        [ScriptControlProperty, ClientPropertyName("editorContainerClientID")]
        private string EditorContainerClientID
        {
            get
            {
                return this.ClientID + "_container";
            }
        }

        [Browsable(false)]
        [ScriptControlProperty, ClientPropertyName("editorOptions")]
        private UEditorConfigWrapper EditorOptions
        {
            get
            {
                return this._EditorOptions;
            }
        }

        [ScriptControlProperty(), ClientPropertyName("dialogUploadFileProcessControlUrl")]
        private string DialogUploadFileProcessControlUrl
        {
            get
            {
                PageRenderMode pageRenderMode
                   = new PageRenderMode(this.UniqueID, "DialogUploadFileProcessControl");

                return NormalizeDialogRequestUrl(WebUtility.GetRequestExecutionUrl(pageRenderMode));
            }
        }

        [ScriptControlProperty(), ClientPropertyName("showImageHandlerUrl")]
        private string showImageHandlerUrl
        {
            get
            {
                return UriHelper.ResolveUri(UEditorWrapperSettings.GetConfig().ShowImageHandlerUrl).ToString();
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
        /// 控件ID
        /// </summary>
        [ScriptControlProperty(), ClientPropertyName("controlID")]
        private string ControlID
        {
            get
            {
                return this.UniqueID;
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
                return lockID;
            }
        }


        /// <summary>
        /// 服务器上保存文件的根路径的配置节点名称
        /// </summary>
        [DefaultValue(""), Category("Image"), ScriptControlProperty, ClientPropertyName("rootPathName"), Description("服务器上保存文件的根路径的配置节点名称")]
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
        [DefaultValue(""), Category("Image"), ScriptControlProperty, ClientPropertyName("relativePath"), Description("服务器上保存文件的目录(相对路径)")]
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

        [PersistenceMode(PersistenceMode.Attribute), Description("UEditor工具栏")]
        [DefaultValue(@"|,Undo,Redo,|,
Bold,Italic,Underline,StrikeThrough,Superscript,Subscript,RemoveFormat,FormatMatch,|,
BlockQuote,|,PastePlain,|,ForeColor,BackColor,InsertOrderedList,InsertUnorderedList,|,CustomStyle,
Paragraph,RowSpacing,LineHeight,FontFamily,FontSize,|,
DirectionalityLtr,DirectionalityRtl,|,Indent,|,
JustifyLeft,JustifyCenter,JustifyRight,JustifyJustify,|,
Link,Unlink,Anchor,|,InsertFrame,PageBreak,HighlightCode,|,
Horizontal,Date,Time,Spechars,|,
InsertTable,DeleteTable,InsertParagraphBeforeTable,InsertRow,DeleteRow,InsertCol,DeleteCol,MergeCells,MergeRight,MergeDown,SplittoCells,SplittoRows,SplittoCols,|,
SelectAll,ClearDoc,SearchReplace,Print,Preview,CheckImage,WordImage,InsertImage,DownloadImage,|,
Source")]
        [Editor(typeof(MultilineStringEditor), typeof(UITypeEditor)), Category("UEditor Settings")]
        public string Toolbars
        {
            get
            {
                return GetPropertyValue("Toolbars", @"|,Undo,Redo,|,
Bold,Italic,Underline,StrikeThrough,Superscript,Subscript,RemoveFormat,FormatMatch,|,
BlockQuote,|,PastePlain,|,ForeColor,BackColor,InsertOrderedList,InsertUnorderedList,|,CustomStyle,
Paragraph,RowSpacing,LineHeight,FontFamily,FontSize,|,
DirectionalityLtr,DirectionalityRtl,|,Indent,|,
JustifyLeft,JustifyCenter,JustifyRight,JustifyJustify,|,
Link,Unlink,Anchor,|,InsertFrame,PageBreak,HighlightCode,|,
Horizontal,Date,Time,Spechars,|,
InsertTable,DeleteTable,InsertParagraphBeforeTable,InsertRow,DeleteRow,InsertCol,DeleteCol,MergeCells,MergeRight,MergeDown,SplittoCells,SplittoRows,SplittoCols,|,
SelectAll,ClearDoc,SearchReplace,Print,Preview,CheckImage,WordImage,InsertImage,DownloadImage,|,
Source");

            }
            set
            {
                SetPropertyValue("Toolbars", value);
            }
        }

        private DialogUploadFileProcessControl dialogUploadFileProcessControl = new DialogUploadFileProcessControl();
        private ComponentHelperWrapper componentHelperWrapper = new ComponentHelperWrapper();

        private static string NormalizeDialogRequestUrl(string url)
        {
            NameValueCollection originalParams = UriHelper.GetUriParamsCollection(url);

            originalParams.Remove(PassportManager.TicketParamName);

            return UriHelper.CombineUrlParams(url, originalParams);
        }
        /// <summary>
        /// 图片信息
        /// </summary>
        public DocumentProperty DocumentProperty
        {
            get
            {
                if (this._docProp == null)
                    this._docProp = new DocumentProperty();

                return this._docProp;
            }
            set
            {
                this._docProp = value;
            }
        }


        #endregion

        #region Protected methods

        protected override void OnInit(EventArgs e)
        {
            //if (ProcessShowImageRequest())
            //    return;
            if (ProcessRequest())
            {
                return;
            }

            base.OnInit(e);
        }

        protected override void CreateChildControls()
        {
            this.Controls.Add(this.componentHelperWrapper);

            if (this.Visible) //&& this.Page.Items.Contains("ueditorWrapperSubmitValidator") == false)
            {
                string clientValidateStr = string.Format("$find('{0}').check", this.ID);
                this._SubmitValidator.ClientValidationFunction = clientValidateStr;//"$HBRootNS.UEditorWrapper.ContentValidate";
                this.Controls.Add(this._SubmitValidator);

                //this.Page.Items.Add("ueditorWrapperSubmitValidator", "exist");
            }
            Controls.Add(dialogUploadFileProcessControl);
            base.CreateChildControls();
        }

        protected override void OnPreRender(EventArgs e)
        {
            RegisterScriptAndCss();
            RegisterHiddenFields();

            //this.Attributes.CssStyle[HtmlTextWriterStyle.Position] = "relative";
            //this._EditorOptions = PrepareConfigWrapper();
            this.componentHelperWrapper.Visible = ReadOnly ? false : true;
            this._EditorOptions = ReadOnly ? new UEditorConfigWrapper() { Toolbars = new string[0], TextArea = this.PostedDataFormName } : PrepareConfigWrapper(this.Toolbars);

            if (RenderMode.OnlyRenderSelf)
            {
                switch (RenderMode.RenderArgument)
                {
                    case "DialogUploadFileProcessControl":
                        dialogUploadFileProcessControl.Visible = true;
                        this.Page.Title = Translator.Translate(Define.DefaultCulture, "正在上传");
                        this.componentHelperWrapper.Visible = true;
                        break;
                    default:
                        break;
                }
            }
            else
            {
                if (this.RenderMode.UseNewPage == false)
                {
                    dialogUploadFileProcessControl.Visible = false;
                }
            }

            base.OnPreRender(e);

        }

        protected override bool LoadPostData(string postDataKey, NameValueCollection postCollection)
        {
            string data = Page.Request.Form[PostedDataFormName];
            this.DocumentProperty = JSONSerializerExecute.Deserialize<DocumentProperty>(data);

            if (DocumentProperty.InitialData.IsNotEmpty())
            {
                this._InitialData = HttpUtility.UrlDecode(DocumentProperty.InitialData);//.Replace("+", HttpUtility.UrlEncode("+")));
                DocumentProperty.InitialData = this._InitialData;
            }
            //if (data.IsNotEmpty())
            //    this._InitialData = HttpUtility.UrlDecode(data);

            return base.LoadPostData(postDataKey, postCollection);
        }

        protected override void Render(HtmlTextWriter writer)
        {
            if (this.DesignMode)
                writer.Write(string.Format("<img src=\"{0}\" style=\"display: block\"/>",
                    Page.ClientScript.GetWebResourceUrl(this.GetType(),
                        "MCS.Web.WebControls.UEditorWrapper.UEditorLogo.png")));
            else
            {
                RenderFakeElement(writer);
                RenderEditorContainer(writer);
                writer.AddStyleAttribute(HtmlTextWriterStyle.Display, "none");
                base.Render(writer);
            }
        }

        protected override void LoadClientState(string clientState)
        {
            base.LoadClientState(clientState);
        }

        protected override string SaveClientState()
        {
            if (this.DocumentProperty.Id.IsNullOrEmpty())
                this.DocumentProperty.Id = Guid.NewGuid().ToString();

            //this.DocumentProperty.Id = Guid.NewGuid().ToString();
            return JSONSerializerExecute.Serialize(this.DocumentProperty);
        }

        #endregion Protected methods

        #region Private Methods

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

                if (requestType == string.Empty)
                    return false;

                string controlID = WebUtility.GetRequestQueryValue("controlID", string.Empty);
                bool raiseEvent = false;

                if (this.Page.FindControl(control => control.UniqueID == controlID, true) != null)
                {
                    if (controlID != this.ControlID)
                        return raiseEvent;
                    else
                        raiseEvent = true;
                }

                result = true;

                HttpContext.Current.Response.Clear();

                if (requestType.ToLower() == "upload")
                {
                    UploadImage();
                    //if (request.Files.Count > 0)
                    //{
                    //    FileUpload.Upload();
                    //    string filePath = string.Empty;
                    //    var file = request.Files[0];

                    //    var newID = UuidHelper.NewUuid();
                    //    var fileName = file.FileName;

                    //    ImageUploadHelper.UploadFile(file, file.FileName, newID + fileName.Substring(fileName.LastIndexOf('.')), this.RootPathName, out filePath);
                    //    HttpContext.Current.Response.AppendHeader("newMaterialID", "message=" + HttpUtility.UrlEncode(newID.ToString()));
                    //    HttpContext.Current.Response.End();
                    //}
                }
            }

            return result;
        }

        private void GenerateErrorInformation(string errorInformation)
        {
            HttpResponse response = HttpContext.Current.Response;

            response.AppendHeader("errorMessage", HttpUtility.UrlEncode("message=" + errorInformation));

            if (HttpContext.Current.Request.QueryString["upmethod"] == "new")
            {
                response.ClearContent();
                var dialogControlID = WebUtility.GetRequestQueryValue("dialogControlID", string.Empty);
                string output = "<script type='text/javascript'>";
                output += "window.parent.$find('" + dialogControlID + "').onUploadFinish(2,'" + errorInformation + "')";
                output += "</script>";
                response.Write(output);
            }

        }


        /// <summary>
        /// 检查上传路径是否存在，如果不存在则创建该路径
        /// </summary>
        private void AutoCreateUploadPath(string uploadRootPath)
        {
            ExceptionHelper.CheckStringIsNullOrEmpty(uploadRootPath, "uploadRootPath");

            if (Directory.Exists(uploadRootPath) == false)
                Directory.CreateDirectory(uploadRootPath);

            string uploadTempPath = uploadRootPath + @"Temp\";

            if (Directory.Exists(uploadTempPath) == false)
                Directory.CreateDirectory(uploadTempPath);
        }

        private void UploadImage()
        {
            const int FileTooLargeError = -2147467259;
            ObjectContextCache.Instance["FileUploadProcessed"] = true;

            HttpRequest request = HttpContext.Current.Request;
            HttpResponse response = HttpContext.Current.Response;

            string lockID = WebUtility.GetRequestQueryValue<string>("lockID", string.Empty);
            string userID = WebUtility.GetRequestQueryValue<string>("userID", string.Empty);
            string fileName = WebUtility.GetRequestQueryValue<string>("fileName", string.Empty);
            int fileMaxSize = WebUtility.GetRequestQueryValue<int>("fileMaxSize", 0);
            string controlID = WebUtility.GetRequestQueryValue("controlID", string.Empty);

            ExceptionHelper.CheckStringIsNullOrEmpty(fileName, "fileName");

            try
            {
                if (fileMaxSize > 0 && request.ContentLength > fileMaxSize)
                {
                    GenerateErrorInformation(string.Format("文件超过了上传大小的限制{0}字节", fileMaxSize));
                }
                else
                {
                    //不检查锁，沈峥修改
                    //CheckLock(lockID, userID);

                    string uploadPath = ImageUploadHelper.GetUploadRootPath(this.RootPathName);

                    AutoCreateUploadPath(uploadPath);

                    string newID = UuidHelper.NewUuidString();
                    fileName = newID + fileName.Substring(fileName.LastIndexOf('.'));

                    if (WebUtility.GetRequestQueryString("upmethod", "") == "new")
                    {
                        var dialogControlID = WebUtility.GetRequestQueryValue("dialogControlID", string.Empty);
                        request.Files[0].SaveAs(uploadPath + @"Temp\" + fileName);

                        string output = "<script type='text/javascript'>";
                        output += "window.parent.$find('" + dialogControlID + "').onUploadFinish(1)";
                        output += "</script>";

                        response.Write(output);
                    }
                    else if (WebUtility.GetRequestQueryString("upmethod", "") == "ue")
                    {
                        request.Files[0].SaveAs(uploadPath + @"Temp\" + fileName);

                        string output = "<script type='text/javascript'>";
                        output += string.Format("window.parent.onUploadFinish('{0}','{1}')", newID, fileName);
                        output += "</script>";

                        response.Write(output);
                    }
                    else
                    {
                        request.SaveAs(uploadPath + @"Temp\" + fileName, false);
                    }

                    string fileIconPath = FileConfigHelper.GetFileIconPath(fileName);

                    response.AppendHeader("fileIconPath", HttpUtility.UrlEncode("message=" + fileIconPath));

                    string dateTimeNow = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss.fff");
                    response.AppendHeader("lastUploadTag", "message=" + dateTimeNow);
                    response.AppendHeader("newMaterialID", "message=" + HttpUtility.UrlEncode(newID));
                }
            }
            catch (Exception ex)
            {
                string errorMessage = ex.Message;

                if (ex is ExternalException && ((ExternalException)(ex)).ErrorCode == FileTooLargeError
                    && ex.Source == "System.Web")
                    errorMessage = "您上传的文件过大";

                GenerateErrorInformation(errorMessage);
            }
            finally
            {
                try
                {
                    response.End();
                }
                catch { }
            }
        }

        private bool ProcessShowImageRequest()
        {
            string imgName = WebUtility.GetRequestQueryValue("uploadedImageName", "");
            bool result = imgName.IsNotEmpty();

            if (result)
            {

                string filePath = "";
                bool cache = WebUtility.GetRequestQueryValue("cache", false);

                HttpContext.Current.Response.Clear();

                ShowImage(imgName, filePath, cache);
            }

            return result;
        }

        private void ShowImage(string imgName, string filePath, bool cache)
        {
            HttpResponse response = HttpContext.Current.Response;

            try
            {
                byte[] content = null;

                string fileName = string.Empty;

                if (imgName.IsNotEmpty())
                    content = GetImageBytesFromFilePath(imgName);
                //else
                //    content = GetImageBytesFromID(id, out fileName);

                if (content == null)
                {
                    fileName = "cannotShow.png";
                    content = ResourceHelper.GetResourceBytes(Assembly.GetExecutingAssembly(),
                        "MCS.Web.WebControls.ImageUploaderControl.cannotShow.png");
                }

                if (cache == false)
                    response.Cache.SetCacheability(HttpCacheability.NoCache);

                response.ContentType = WebUtility.GetContentTypeByFileName(fileName);
                response.BinaryWrite(content);
            }
            catch (System.Exception ex)
            {
                response.Write(ex.Message);
            }
            finally
            {
                response.End();
            }
        }

        private byte[] GetImageBytesFromFilePath(string fileName)
        {
            byte[] content = null;
            string filePath = ImageUploadHelper.GetUploadRootPath(this.RootPathName) + "Temp\\" + fileName;

            if (File.Exists(filePath))
            {
                using (FileStream fs = new FileStream(filePath, FileMode.Open, FileAccess.Read, FileShare.Read))
                {
                    content = fs.ToBytes();
                }
            }

            return content;
        }

        //private static string GetUploadRootPath(string rootPathName)
        //{
        //    ExceptionHelper.CheckStringIsNullOrEmpty(rootPathName, "rootPathName");

        //    AppPathSettingsElement elem = AppPathConfigSettings.GetConfig().Paths[rootPathName];

        //    ExceptionHelper.FalseThrow(elem != null, "不能在配置节appPathSettings下找到名称为\"{0}\"的路径定义", rootPathName);

        //    return elem.Dir;
        //}

        //ImagePathConfigSettings

        #endregion


    }
}
