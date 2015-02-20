using System;
using System.Collections.Generic;
using System.Text;
using System.ComponentModel;
using System.Web.UI;
using System.Web;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;
using ChinaCustoms.Framework.DeluxeWorks.Web.Library.Script;
using System.Data;
using System.ComponentModel;
using System.Collections;
using System.Collections.ObjectModel;
using System.Collections.Generic;

[assembly: WebResource("ChinaCustoms.Framework.DeluxeWorks.Web.WebControls.MaterialControl.MaterialControl.js", "application/x-javascript")]
[assembly: WebResource("ChinaCustoms.Framework.DeluxeWorks.Web.WebControls.MaterialControl.MaterialControl.css", "text/css", PerformSubstitution = true)]
[assembly: WebResource("ChinaCustoms.Framework.DeluxeWorks.Web.WebControls.MaterialControl.UploadFiles.htm", "text/html", PerformSubstitution = true)]
[assembly: WebResource("ChinaCustoms.Framework.DeluxeWorks.Web.WebControls.MaterialControl.HBWebHelperControl.CAB", "application/x-shockwave-flash")]

namespace ChinaCustoms.Framework.DeluxeWorks.Web.WebControls
{
    /// <summary>
    /// 显示样式
    /// InOneLine　: 在一行中显示
    /// MultiLine　:　多行显示
    /// </summary>
    public enum ShowMode
    {
        Inline = 1,
        Vertical = 2
    }

    #region 控件

    [RequiredScript(typeof(ControlBaseScript))]
    [ClientScriptResource("ChinaCustoms.Framework.DeluxeWorks.Web.WebControls.MaterialControl",
          "ChinaCustoms.Framework.DeluxeWorks.Web.WebControls.MaterialControl.MaterialControl.js")]
    [ClientCssResource("ChinaCustoms.Framework.DeluxeWorks.Web.WebControls.MaterialControl.MaterialControl.css")]
    public class MaterialControl : ScriptControlBase
    {
        private Style materialTableCellStyle = null;
        private Style materialTableStyle = null;
        private Style popUpMaterialTableStyle = null;
        private Style popUpMaterialTableCellStyle = null;
        private Style popUpMaterialTableHeadCellStyle = null;
        private Style popUpMaterialTableCellInputStyle = null;
        private Style popUpBodyStyle = null;

        [Description("显示文件之前，可以进行权限检查或重定向。此事件仅当页面为HB.Framework.Web.BaseWebForm的派生类时才触发")]
        public event EventHandler BeforeShowFile;

        //[Description("生成显示附件的Url时触发此事件。此事件仅当页面为HB.Framework.Web.BaseWebForm的派生类时才触发")]
        //public event CalculateFileUrlEventHandler CalculateFileUrl;

        public MaterialControl()
            : base(true, HtmlTextWriterTag.Div)
        {
            this.EnableViewState = true;
        }

        #region   Appearance

        [DefaultValue(""),Category("Appearance"),ScriptControlProperty]
        [ClientPropertyName("displayText"),WebDescription("显示文本")]
        public string DisplayText
        {
            set
            {
                SetPropertyValue("DisplayText", value);
            }
            get
            {
                return GetPropertyValue("DisplayText", "添加或修改");
            }
        }

        [DefaultValue(""),Category("Appearance"),ScriptControlProperty]
        [ClientPropertyName("materialTableShowMode"),WebDescription("附件列表显示模式")]
        public ShowMode MaterialTableShowMode
        {
            set
            {
                SetPropertyValue("MaterialTableShowMode", value);
            }
            get
            {
                return GetPropertyValue("MaterialTableShowMode", ShowMode.Vertical);
            }
        }

        [DefaultValue(""),Category("Appearance"),ScriptControlProperty]
        [ClientPropertyName("materialTableStyle"), WebDescription("附件列表样式")]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Content), NotifyParentProperty(true), PersistenceMode(PersistenceMode.InnerProperty)]
        [JavaScriptConverter(typeof(StyleConverter))]
        public Style MaterialTableStyle
        {
            get
            {
                if (this.materialTableStyle == null)
                {
                    this.materialTableStyle = new Style();
                    if (base.IsTrackingViewState)
                    {
                        ((IStateManager)this.materialTableStyle).TrackViewState();
                    }
                }
                return this.materialTableStyle;
            }
        }

        [DefaultValue(""),Category("Appearance"),ScriptControlProperty]
        [ClientPropertyName("materialTableCellStyle"),WebDescription("附件列表单元格样式")]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Content), NotifyParentProperty(true), PersistenceMode(PersistenceMode.InnerProperty)]
        [JavaScriptConverter(typeof(StyleConverter))]
        public Style MaterialTableCellStyle
        {
            get
            {
                if (this.materialTableCellStyle == null)
                {
                    this.materialTableCellStyle = new Style();
                    if (base.IsTrackingViewState)
                    {
                        ((IStateManager)this.materialTableCellStyle).TrackViewState();
                    }
                }
                return this.materialTableCellStyle;
            }
        }

        [DefaultValue(""), Category("Appearance"), ScriptControlProperty]
        [ClientPropertyName("popUpMaterialTableStyle"), WebDescription("弹出窗口的表格样式")]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Content), NotifyParentProperty(true), PersistenceMode(PersistenceMode.InnerProperty)]
        [JavaScriptConverter(typeof(StyleConverter))]
        public Style PopUpMaterialTableStyle
        {
            get
            {
                if (this.popUpMaterialTableStyle == null)
                {
                    this.popUpMaterialTableStyle = new Style();
                    if (base.IsTrackingViewState)
                    {
                        ((IStateManager)this.popUpMaterialTableStyle).TrackViewState();
                    }
                }
                return this.popUpMaterialTableStyle;
            }
        }

        [DefaultValue(""), Category("Appearance"), ScriptControlProperty]
        [ ClientPropertyName("popUpMaterialTableCellStyle"), WebDescription("弹出窗口的表格的单元格样式")]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Content), NotifyParentProperty(true), PersistenceMode(PersistenceMode.InnerProperty)]
        [JavaScriptConverter(typeof(StyleConverter))]
        public Style PopUpMaterialTableCellStyle
        {
            get
            {
                if (this.popUpMaterialTableCellStyle == null)
                {
                    this.popUpMaterialTableCellStyle = new Style();
                    if (base.IsTrackingViewState)
                    {
                        ((IStateManager)this.popUpMaterialTableCellStyle).TrackViewState();
                    }
                }
                return this.popUpMaterialTableCellStyle;
            }
        }

        [DefaultValue(""), Category("Appearance"), ScriptControlProperty]
        [ ClientPropertyName("popUpMaterialTableHeadCellStyle"), WebDescription("弹出窗口的表格的单元格表头样式")]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Content), NotifyParentProperty(true), PersistenceMode(PersistenceMode.InnerProperty)]
        [JavaScriptConverter(typeof(StyleConverter))]
        public Style PopUpMaterialTableHeadCellStyle
        {
            get
            {
                if (this.popUpMaterialTableHeadCellStyle == null)
                {
                    this.popUpMaterialTableHeadCellStyle = new Style();
                    if (base.IsTrackingViewState)
                    {
                        ((IStateManager)this.popUpMaterialTableHeadCellStyle).TrackViewState();
                    }
                }
                return this.popUpMaterialTableHeadCellStyle;
            }
        }

        [DefaultValue(""), Category("Appearance"), ScriptControlProperty]
        [ ClientPropertyName("popUpMaterialTableCellInputStyle"), WebDescription("弹出窗口的输入框样式")]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Content), NotifyParentProperty(true), PersistenceMode(PersistenceMode.InnerProperty)]
        [JavaScriptConverter(typeof(StyleConverter))]
        public Style PopUpMaterialTableCellInputStyle
        {
            get
            {
                if (this.popUpMaterialTableCellInputStyle == null)
                {
                    this.popUpMaterialTableCellInputStyle = new Style();
                    if (base.IsTrackingViewState)
                    {
                        ((IStateManager)this.popUpMaterialTableCellInputStyle).TrackViewState();
                    }
                }
                return this.popUpMaterialTableCellInputStyle;
            }
        }

        [DefaultValue(""), Category("Appearance"), ScriptControlProperty]
        [ClientPropertyName("popUpBodyStyle"), WebDescription("弹出窗口的body样式")]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Content), NotifyParentProperty(true), PersistenceMode(PersistenceMode.InnerProperty)]
        [JavaScriptConverter(typeof(StyleConverter))]
        public Style PopUpBodyStyle
        {
            get
            {
                if (this.popUpBodyStyle == null)
                {
                    this.popUpBodyStyle = new Style();
                    if (base.IsTrackingViewState)
                    {
                        ((IStateManager)this.popUpBodyStyle).TrackViewState();
                    }
                }
                return this.popUpBodyStyle;
            }
        }
        

        #endregion

        #region other

        [ScriptControlProperty,ClientPropertyName("uploadFilePageUrl"),WebDescription("内置上传附件的页面地址")]
        public string UploadFilePageUrl
        {
            get
            {
                return this.Page.ClientScript.GetWebResourceUrl(typeof(ChinaCustoms.Framework.DeluxeWorks.Web.WebControls.MaterialControl),
              "ChinaCustoms.Framework.DeluxeWorks.Web.WebControls.MaterialControl.UploadFiles.htm");
            }
        }

        [ScriptControlProperty,ClientPropertyName("hBWebHelperControl"),WebDescription("内置上传附件的插件")]
        public string HBWebHelperControl
        {
            get
            {
                return this.Page.ClientScript.GetWebResourceUrl(typeof(ChinaCustoms.Framework.DeluxeWorks.Web.WebControls.MaterialControl),
                "ChinaCustoms.Framework.DeluxeWorks.Web.WebControls.MaterialControl.HBWebHelperControl.CAB");
            }
        }

        [ScriptControlProperty,ClientPropertyName("cssUrl"),WebDescription("内置css样式表文件路径")]
        public string CssUrl
        {
            get
            {
                return this.Page.ClientScript.GetWebResourceUrl(typeof(ChinaCustoms.Framework.DeluxeWorks.Web.WebControls.MaterialControl),
                 "ChinaCustoms.Framework.DeluxeWorks.Web.WebControls.MaterialControl.MaterialControl.css");
            }
        }

        [DefaultValue(""),Category("Document"),WebDescription("服务器上的文件存储目录,物理路径")]
        public string UpLoadPath
        {
            set
            {
                SetPropertyValue("UpLoadPath", value);
            }
            get
            {
                return GetPropertyValue("UpLoadPath", string.Empty);
            }
        }

        [DefaultValue(""),Category("Document"),ScriptControlProperty]
        [ClientPropertyName("materialList"),WebDescription("附件列表")]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Content), NotifyParentProperty(true), PersistenceMode(PersistenceMode.InnerProperty)]
        public MaterialList MaterialList
        {
            set
            {
                SetPropertyValue("MaterialList", value);
            }
            get
            {
                return GetPropertyValue("MaterialList", new MaterialList());
            }
        }

        [DefaultValue(""),Category("Document"),ScriptControlProperty]
        [ClientPropertyName("fileUrlRoot"),WebDescription("文件主路径")]
        public string FileUrlRoot
        {
            set
            {
                SetPropertyValue("FileUrlRoot", value);
            }
            get
            {
                return GetPropertyValue("FileUrlRoot", string.Empty);
            }
        }

        [DefaultValue(""), Category("Document"), ScriptControlProperty]
        [ClientPropertyName("applicationName"),WebDescription("应用名称")]
        public string ApplicationName
        {
            set
            {
                SetPropertyValue("ApplicationName", value);
            }
            get
            {
                return GetPropertyValue("ApplicationName", string.Empty);
            }
        }

        [DefaultValue(""), Category("Document"), ScriptControlProperty]
        [ClientPropertyName("programName"),WebDescription("模块名称")]
        public string ProgramName
        {
            set
            {
                SetPropertyValue("ProgramName", value);
            }
            get
            {
                return GetPropertyValue("ProgramName", string.Empty);
            }
        }

        [DefaultValue(""), Category("Document"), ScriptControlProperty]
        [ClientPropertyName("user"),WebDescription("当前用户")]
        public string User
        {
            set
            {
                SetPropertyValue("User", value);
            }
            get
            {
                return GetPropertyValue("User", string.Empty);
            }
        } 

        [DefaultValue(false),ScriptControlProperty,ClientPropertyName("editable")]
        [Category("Document"),WebDescription("是否允许编辑")]
        public bool Editable
        {
            set
            {
                SetPropertyValue("Editable", value);
            }
            get
            {
                return GetPropertyValue("Editable", false );
            }
        }

        [DefaultValue(true),Category("Document"),ScriptControlProperty]
        [ClientPropertyName("allowUpload"),WebDescription("是否允许上传")]
        public bool AllowUpload
        {
            set
            {
                SetPropertyValue("AllowUpload", value);
            }
            get
            {
                return GetPropertyValue("AllowUpload", true);
            }
        }

        [DefaultValue(true),Category("Document"),ScriptControlProperty]
        [ClientPropertyName("allowDelete"),WebDescription("是否允许删除")]
        public bool AllowDelete
        {
            set
            {
                SetPropertyValue("AllowDelete", value);
            }
            get
            {
                return GetPropertyValue("AllowDelete", true);
            }
        }

        [DefaultValue(true),Category("Document"),ScriptControlProperty]
        [ClientPropertyName("allowAdjustOrder"),WebDescription("是否允许调整顺序")]
        public bool AllowAdjustOrder
        {
            set
            {
                SetPropertyValue("AllowAdjustOrder", value);
            }
            get
            {
                return GetPropertyValue("AllowAdjustOrder", true);
            }
        }

        [DefaultValue(true),Category("Document"),ScriptControlProperty]
        [ClientPropertyName("trackRevisions"),WebDescription("是否在microsoft office文档中保留打开痕迹")]
        public bool TrackRevisions
        {
            set
            {
                SetPropertyValue("TrackRevisions", value);
            }
            get
            {
                return GetPropertyValue("TrackRevisions", true);
            }
        }

        [DefaultValue(""),Category("Document"),ScriptControlProperty]
        [ClientPropertyName("resourceID"),WebDescription("附件所属文件的ID")]
        public string ResourceID
        {
            set
            {
                SetPropertyValue("ResourceID", value);
            }
            get
            {
                return GetPropertyValue("ResourceID", string.Empty );
            }
        }

        [DefaultValue(""),Category("Document"),ScriptControlProperty]
        [ClientPropertyName("class"),WebDescription("附件在应用中的类别")]
        public string Class
        {
            set
            {
                SetPropertyValue("Class", value);
            }
            get
            {
                return GetPropertyValue("Class", string.Empty);
            }
        }

        [Category("Document")]
        [ScriptControlProperty,ClientPropertyName("wfActivity"),WebDescription("工作流活动的对象")]
        public string WfActivity
        {
            set
            {
                SetPropertyValue("WfActivity", value);
            }
            get
            {
                return GetPropertyValue("WfActivity", string.Empty);
            }
        }

        [DefaultValue(""),Category("Document"),ScriptControlProperty]
        [ClientPropertyName("forbidUploadExpandName"),WebDescription("禁止上传的文件拓展名(用逗号间隔)")]
        public string ForbidUploadExpandName
        {
            set
            {
                SetPropertyValue("ForbidUploadExpandName", value);
            }
            get
            {
                return GetPropertyValue("ForbidUploadExpandName", string.Empty);
            }
        }

        [DefaultValue(""),Category("Document"),ScriptControlProperty]
        [ClientPropertyName("mainVersionIDs"),WebDescription("设置为主版本的ID形成的数组")]
        public string[] MainVersionIDs
        {
            set
            {
                SetPropertyValue("MainVersionIDs", value);
            }
            get
            {
                return GetPropertyValue("MainVersionIDs", new string[] { });
            }
        }

        [DefaultValue(""),Category("Document"),ScriptControlProperty]
        [ClientPropertyName("showAllVersion"),WebDescription("是否显示所有版本")]
        public bool ShowAllVersion
        {
            set
            {
                SetPropertyValue("ShowAllVersion", value);
            }
            get
            {
                return GetPropertyValue("ShowAllVersion", true );
            }
        }

        [DefaultValue(""),Category("Document"),ScriptControlProperty]
        [ClientPropertyName("department"),WebDescription("附件所属的部门对象")]
        public string Department
        {
            set
            {
                SetPropertyValue("Department", value);
            }
            get
            {
                return GetPropertyValue("Department", string.Empty);
            }
        }

        #endregion

        #region 上传文件

        /// <summary>
        /// 接收文件
        /// </summary>
        /// <param name="e"></param>
        /// <returns></returns>
        public bool ProcessSpecialRequest()
        {
            // 在此处放置用户代码以初始化页面
            this.Page.Server.ScriptTimeout = 9999;

            string strFileName = string.Empty;

            strFileName = this.Page.Server.UrlDecode(this.Page.Request.QueryString["FileName"]);

            CheckUploadPath();

            return IOSaveFile(strFileName);

            /*
              string strParentID = string.Empty;
             上传文件所属的文件夹ID
            strParentID = this.Page.Request.QueryString["ParentID"];
           
            如使用ADODB.Stream保存流到文件，将下面的注释去掉
            ADOSaveFile(strFileName,strGUID);
             将上传文件信息保存到数据库
             SaveToDB(strParentID, strFileName, strGUID); 
             */
        }

        //检查文件名是否存在，如果存在则返回一个新的文件名（格式如"xxx\文件名(1)"）
        private string CheckFileName(string iParentPath, string iFileName)
        {
            bool NoSameName = false;
            string tmpExt = GetFileExtension(iFileName);
            string tmpName = GetFileName(iFileName);
            int i = 1;

            //
            //循环判断是否存在文件重名，并重新生成文件名

            return tmpName + tmpExt;

        }

        //获得文件扩展名(包含.)
        private string GetFileExtension(string iFileName)
        {
            int i;
            i = iFileName.LastIndexOf('.');

            //如果存在扩展名则返回，否则返回空字符串
            if (i > 0)
            {
                return iFileName.Substring(i);
            }
            else
            {
                return string.Empty;
            }
        }

        //获得文件名(不包含扩展名的部分)
        private string GetFileName(string iFileName)
        {
            int i;
            i = iFileName.LastIndexOf('.');

            //如果存在扩展名则返回扩展名前面的部分，否则全部返回
            if (i > 0)
            {
                return iFileName.Substring(0, i);
            }
            else
            {
                return iFileName;
            }
        }

        //使用IO流保存文件到指定目录
        private bool IOSaveFile(string iFileName)
        {
            //获得页面传入的流
            System.IO.Stream fs = this.Page.Request.InputStream;
            System.IO.BinaryReader br = new System.IO.BinaryReader(fs);
            Byte[] postArray = br.ReadBytes(Convert.ToInt32(fs.Length));

            //使用政研资料库地址保存上传文件/使用GUID文件名 + 文件扩展名
            System.IO.Stream postStream = System.IO.File.OpenWrite(UpLoadPath + "\\" + iFileName);

            //保存流到文件中
            if (postStream.CanWrite)
            {
                postStream.Write(postArray, 0, postArray.Length);
                postStream.Close();
                fs.Close();
                return true;
            }
            else
            {
                return false;
            }
        }

        //检查上传路径是否存在，如果不存在则创建该路径
        private void CheckUploadPath()
        {
            if (System.IO.Directory.Exists(this.UpLoadPath) == false)
            {
                throw new Exception("指定的上传文件根目录不存在。");
            }

            if (this.UpLoadPath.EndsWith("\\") == false)
            {
                this.UpLoadPath += "\\";
            }

            string iPath = this.UpLoadPath;

            if (System.IO.Directory.Exists(iPath) == false)
            {
                System.IO.Directory.CreateDirectory(iPath);
            }
        }

        #endregion

        protected override void OnInit(EventArgs e)
        {
            HttpRequest request = HttpContext.Current.Request;

            if (request.QueryString["_uploadFile"] != null)
                if (ProcessSpecialRequest())
                    throw new Exception();

            base.OnInit(e);
        }

        protected override void Render(HtmlTextWriter writer)
        {
            if (DesignMode)
            {
                LinkButton link = new LinkButton();
                link.Text = "添加或修改";
                Controls.Add(link);             
                base.Render(writer);
            }
            else
            {
                base.Render(writer);
            }
        }

        /// <summary>
        /// 重新设定文件列表
        /// </summary>
        /// <param name="materialList"></param>
        /// <returns></returns>
        [ScriptControlMethod]
        public string ReinitMaterial(MaterialList materialList)
        {
            this.MaterialList = materialList;
            return "_MaterialList包含：" + this.MaterialList.Count.ToString();
        }

        /// <summary>
        /// 保存副本
        /// </summary>
        public void SaveCopyVersion()
        { 
        
        }

        /// <summary>
        /// 形成新的版本
        /// </summary>
        public void SaveDepartmentVersion()
        {

        }

    }

    #endregion

    //用于测试的辅助类
    #region  Material
    [Serializable]
    public class Material
    {
        private string _ID = string.Empty;
        private string _ResourceID = string.Empty;
        private int _SortID = 0;
        private string _Class = string.Empty;
        private string _Title = string.Empty;
        private int _PageQuantity = 0;
        private string _FilePath = string.Empty;
        private string _OriginalName = string.Empty;

        public Material(string id, string resourceID, int sortID, string class_, string title, int pageQuantity, string filePath, string originalName)
        {
            this._ID = id;
            this._ResourceID = resourceID;
            this._SortID = sortID;
            this._Class = class_;
            this._Title = title;
            this._PageQuantity = pageQuantity;
            this._FilePath = filePath;
            this._OriginalName = originalName;
        }

        public Material()
        {

        }

        public string ID
        {
            get { return this._ID; }
            set { this._ID = value; }
        }

        public string ResourceID
        {
            get { return this._ResourceID; }
            set { this._ResourceID = value; }
        }

        public int SortID
        {
            get { return this._SortID; }
            set { this._SortID = value; }
        }

        public string Class
        {
            get { return this._Class; }
            set { this._Class = value; }
        }

        public string Title
        {
            get { return this._Title; }
            set { this._Title = value; }
        }

        public int PageQuantity
        {
            get { return this._PageQuantity; }
            set { this._PageQuantity = value; }
        }

        public string FilePath
        {
            get { return this._FilePath; }
            set { this._FilePath = value; }
        }

        public string OriginalName
        {
            get { return this._OriginalName; }
            set { this._OriginalName = value; }
        }

    }

    #endregion

    #region  MaterialList

    [Serializable]
    public class MaterialList : System.Collections.CollectionBase
    { 
        public MaterialList()
        { 
        }

        #region 操作集合

        public Material this[int index]
        {
            get
            {
                return (Material)this.InnerList[index];
            }
            set
            {
                this.InnerList[index] = value;
            }
        }

        public int IndexOf(Material material )
        {
            return this.InnerList.IndexOf(material);
        }

        public int Add(Material material)
        {
            return this.InnerList.Add(material);
        }

        public void Insert(int index, Material material )
        {
            this.Insert(index, material);
        }

        public void Remove(Material material )
        {
            this.InnerList.Remove(m);
        }

        public void RemoveAt(int index)
        {
            this.InnerList.RemoveAt(index);
        }

        public bool Contains(Material material )
        {
            return this.InnerList.Contains(material);
        }

        #endregion

        #region 重载基类的方法
        protected override void OnValidate(object value)
        {
            if (value.GetType() != Type.GetType("ChinaCustoms.Framework.DeluxeWorks.Web.WebControls.Material"))
                throw new ArgumentException("value must be of type ChinaCustoms.Framework.DeluxeWorks.Web.WebControls.Material", "value");
        }
        #endregion

    }

    #endregion

}