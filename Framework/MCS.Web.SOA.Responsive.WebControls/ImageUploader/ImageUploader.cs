using System;
using System.IO;
using System.Collections.Generic;
using System.ComponentModel;
using System.Reflection;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.UI;
using System.Web.UI.HtmlControls;
using System.Web.UI.WebControls;

using MCS.Web.Library.Script;
using MCS.Library.SOA.DataObjects;
using MCS.Library.Core;
using MCS.Library.Security;
using MCS.Library.Globalization;
using MCS.Web.Responsive.Library;
using MCS.Web.Responsive.Library.Script;

[assembly: WebResource("MCS.Web.Responsive.WebControls.ImageUploader.ImageUploader.js", "application/x-javascript")]
[assembly: WebResource("MCS.Web.Responsive.WebControls.ImageUploader.blank.png", "image/png")]
[assembly: WebResource("MCS.Web.Responsive.WebControls.ImageUploader.ImageUploader.css", "text/css", PerformSubstitution = true)]
//[assembly: WebResource("MCS.Web.Responsive.WebControls.ImageUploader.noLogo.png", "image/png")]
//[assembly: WebResource("MCS.Web.Responsive.WebControls.ImageUploader.defaultImage_bw.jpg", "image/jpeg")]

namespace MCS.Web.Responsive.WebControls
{
    [RequiredScript(typeof(HBCommonScript), 2)]
    [RequiredScript(typeof(SubmitButtonScript), 8)]
    [ClientScriptResource("MCS.Web.WebControls.ImageUploader", "MCS.Web.Responsive.WebControls.ImageUploader.ImageUploader.js")]
    [ClientCssResource("MCS.Web.Responsive.WebControls.ImageUploader.ImageUploader.css")]
    public class ImageUploader : ScriptControlBase
    {
        private ImageProperty imgProp = null;

        public ImageUploader()
            : base(true, HtmlTextWriterTag.Div)
        { }

        protected override void OnInit(EventArgs e)
        {
            base.OnInit(e);
        }

        protected override void Render(HtmlTextWriter writer)
        {
            if (this.DesignMode)
            {
                writer.AddStyleAttribute(HtmlTextWriterStyle.BackgroundColor, "#ccc");
                writer.RenderBeginTag("div");
                writer.Write("Image Uploader Control");
                writer.RenderEndTag();
            }
            else
                base.Render(writer);
        }

        protected override void OnPreRender(EventArgs e)
        {
            base.OnPreRender(e);
        }

        protected override string SaveClientState()
        {
            if (this.ResourceID.IsNotEmpty())
                this.ImgProp.ResourceID = this.ResourceID;

            if (this.Class.IsNotEmpty())
                this.ImgProp.Class = this.Class;

            return JSONSerializerExecute.Serialize(this.ImgProp);
        }

        protected override void LoadClientState(string clientState)
        {
            if (string.IsNullOrEmpty(clientState) == false)
                this.imgProp = JSONSerializerExecute.Deserialize<ImageProperty>(clientState);
        }

        public ImageProperty ImgProp
        {
            get
            {
                if (this.imgProp == null)
                    this.imgProp = new ImageProperty();

                return this.imgProp;
            }
            set
            {
                this.imgProp = value;
            }
        }

        public string UploadImageShowenUrl
        {
            get;
            set;

        }

        public delegate void UploadHandler(object sender, UploadEventArgs e);

        #region 事件

        //public event ImageContentHandler ImageContent;
        #endregion

        //internal protected void OnBeforeUploadFile(UploadEventArgs e)
        //{
        //    if (this.BeforeUpload != null)
        //        this.BeforeUpload(this, e);
        //}

        //internal protected void OnAfterUploadFile(UploadEventArgs e)
        //{
        //    if (this.AfterUpload != null)
        //        this.AfterUpload(this, e);
        //}

        //internal protected void OnBeforeDownload()
        //{
        //    if (this.BeforeDownload != null)
        //    {
        //        EventArgs e = new EventArgs();

        //        this.BeforeDownload(this, e);
        //    }
        //}

        #region 客户端属性

        //[Browsable(false)]
        //[ScriptControlProperty()]
        //[ClientPropertyName("inputFileID")]
        //private string InputControlID
        //{
        //    get
        //    {
        //        return uploader.ClientID;
        //    }

        //}

        //[Browsable(false)]
        //[ScriptControlProperty()]
        //[ClientPropertyName("imageID")]
        //private string ImageID
        //{
        //    get
        //    {
        //        return imageControl.ClientID;
        //    }

        //}

        //public Unit ImageWidth
        //{
        //    get
        //    {
        //        return ViewState.GetViewStateValue("ImageWidth", Unit.Pixel(120));
        //    }
        //    set
        //    {
        //        ViewState.SetViewStateValue("ImageWidth", value);
        //    }
        //}

        //[DefaultValue("")]
        //[ScriptControlProperty]
        //[ClientPropertyName("imageWidth")]
        //public string ClientImageWidth
        //{
        //    get
        //    {
        //        return ImageWidth.ToString();
        //    }
        //}

        //public Unit ImageHeight
        //{
        //    get
        //    {
        //        return ViewState.GetViewStateValue("ImageHeight", Unit.Pixel(120));
        //    }
        //    set
        //    {
        //        ViewState.SetViewStateValue("ImageHeight", value);
        //    }
        //}

        //[ScriptControlProperty]
        //[ClientPropertyName("imageHeight")]
        //public string ClientImageHeight
        //{
        //    get
        //    {
        //        return ImageHeight.ToString();
        //    }
        //}

        //[DefaultValue("")]
        //[ScriptControlProperty]
        //[ClientPropertyName("defaultImg")]
        //public string DefaultImg
        //{
        //    get
        //    {
        //        string defaultImg = "MCS.Web.Responsive.WebControls.ImageUploader.noLogo.png";

        //        if (WebAppSettings.IsWebApplicationCompilationDebug())
        //            defaultImg = "MCS.Web.Responsive.WebControls.ImageUploader.defaultImage_bw.jpg";

        //        return ViewState.GetViewStateValue("DefaultImg",
        //            this.Page.ClientScript.GetWebResourceUrl(this.GetType(), defaultImg));
        //    }
        //    set
        //    {
        //        ViewState.SetViewStateValue("DefaultImg", value);
        //    }
        //}

        //[DefaultValue("")]
        //[ScriptControlProperty]
        //[ClientPropertyName("uploadButtonId")]
        //private string UploadButtonID
        //{
        //    get
        //    {
        //        return this.btnUpload.ClientID;
        //    }
        //}

        //[DefaultValue("")]
        //[ScriptControlProperty]
        //[ClientPropertyName("deleteButtonID")]
        //private string DeleteButtonID
        //{
        //    get
        //    {
        //        return this.btnDelete.ClientID;
        //    }
        //}

        //[DefaultValue("")]
        //[ScriptControlProperty]
        //[ClientPropertyName("innerFrameID")]
        //private string InnerFrameID
        //{
        //    get
        //    {
        //        return INNERFRAMEID;
        //    }
        //}

        //[DefaultValue("")]
        //[ScriptControlProperty]
        //[ClientPropertyName("innerFrameContainerID")]
        //private string InnerFrameContainer
        //{
        //    get
        //    {
        //        return INNERFRAME_CONTAINER;
        //    }
        //}

        [DefaultValue("")]
        [ScriptControlProperty]
        [ClientPropertyName("fileMaxSize")]
        public int FileMaxSize
        {
            get
            {
                return GetPropertyValue<int>("FileMaxSize", 4194304);
            }
            set
            {
                SetPropertyValue<int>("FileMaxSize", value);
            }
        }

        //[DefaultValue("")]
        //[ScriptControlProperty]
        //[ClientPropertyName("controlID")]
        //public string ControlID
        //{
        //    get
        //    {
        //        return this.ClientID;
        //    }
        //}

        public string Class
        {
            get;
            set;
        }

        public string ResourceID
        {
            get;
            set;
        }

        /// <summary>
        /// 上传后触发的客户端事件
        /// </summary>
        [DefaultValue("")]
        [Category("Action")]
        [ScriptControlEvent()]
        [ClientPropertyName("imageUploaded")]
        [Description("上传后触发的客户端事件")]
        public string OnClientImageUploaded
        {
            get { return GetPropertyValue("OnClientImageUploaded", string.Empty); }
            set { SetPropertyValue("OnClientImageUploaded", value); }
        }

        /// <summary>
        /// 客户端删除图片后事件
        /// </summary>
        [DefaultValue("")]
        [Category("Action")]
        [ScriptControlEvent()]
        [ClientPropertyName("imageDeleted")]
        [Description("客户端删除图片后事件")]
        public string OnClientImageDeleted
        {
            get { return GetPropertyValue("OnClientImageDeleted", string.Empty); }
            set { SetPropertyValue("OnClientImageDeleted", value); }
        }

        #endregion
    }
}
