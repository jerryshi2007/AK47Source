using System;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.UI;
using System.ComponentModel;
using System.Web.UI.WebControls;
using System.Collections.Generic;
using System.Web.UI.HtmlControls;
using MCS.Web.Library.Script;
using MCS.Library.OGUPermission;
using MCS.Web.Library;
using System.Data;
using MCS.Web.Responsive.Library.Script;

[assembly: WebResource("MCS.Web.Responsive.WebControls.ImageUploader.ImageUploaderDialog.js", "application/x-javascript")]

namespace MCS.Web.Responsive.WebControls
{

    [RequiredScript(typeof(ControlBaseScript), 1)]
    [RequiredScript(typeof(HBCommonScript), 2)]
    [ClientScriptResource("MCS.Web.WebControls.ImageUploaderDialog", "MCS.Web.Responsive.WebControls.ImageUploader.ImageUploaderDialog.js")]
    [DialogContent("MCS.Web.Responsive.WebControls.ImageUploader.ImageUploaderDialog.htm", "MCS.Web.SOA.Responsive.WebControls")]
    [ToolboxData("<{0}:ImageUploaderDialog runat=server></{0}:ImageUploaderDialog>")]
    public class ImageUploaderDialog : DialogControlBase
    {
        #region private

        private ImageUploader _imageUploader = new ImageUploader()
        {
            InvokeWithoutViewState = true,
            ID = "imageUploaderDialog_uploader"
        };

        #endregion

        /// <summary>
        /// 构造函数
        /// </summary>
        public ImageUploaderDialog()
        {
        }

        #region 属性

        //[Browsable(false)]
        //[ScriptControlProperty()]
        //[ClientPropertyName("confirmButtonClientID")]
        //private string confirmButtonClientID
        //{
        //    get
        //    {
        //        if (this._confirmButton != null)
        //            return this._confirmButton.ClientID;
        //        else return "confirmButtonClientID";
        //    }
        //}

        [Browsable(false)]
        [ScriptControlProperty()]
        [ClientPropertyName("imageUploaderClientID")]
        private string imageUploaderClientID
        {
            get
            {
                if (this._imageUploader != null)
                    return this._imageUploader.ClientID;
                else return "imageUploaderClientID";
            }
        }

        #endregion

        #region 重写的方法

        protected override void CreateChildControls()
        {
            this.Controls.Clear();
            base.CreateChildControls();
        }
        protected override void OnInit(EventArgs e)
        {
            base.OnInit(e);
            //this.DialogFeature = this.GetDialogFeature();
            if (this.Page.IsCallback)
                EnsureChildControls();
        }

        protected override void OnPagePreLoad(object sender, EventArgs e)
        {
            EnsureChildControls();
            base.OnPagePreLoad(sender, e);
        }

        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);

        }

        protected override void OnPreRender(EventArgs e)
        {
            base.OnPreRender(e);
        }

        protected override System.Web.UI.Control LoadDialogTemplate()
        {
            this._imageUploader.Style[HtmlTextWriterStyle.Height] = "300px";
            this._imageUploader.ImgProp.ID = this.Page.Request.QueryString["imageID"];
            string path = this.Page.Request.QueryString["filePath"];
            if (string.IsNullOrEmpty(path) == false)
            {
                path = MCS.Library.Security.MachineKeyEncryptor.Decrypt(path);
                this._imageUploader.ImgProp.FilePath = path;
            }

            return this._imageUploader;
        }

        //protected override void InitDialogContent(Control container)
        //{
        //    base.InitDialogContent(container);
        //    this.ID = "ImageUploaderDialog";

        //    HtmlForm form = (HtmlForm)WebControlUtility.FindParentControl(this, typeof(HtmlForm), true);
        //    if (form != null)
        //    {
        //        form.Style["width"] = "100%";
        //        form.Style["height"] = "100%";
        //    }
        //    this.DialogTitle = "上传图片";
        //    this.Width = Unit.Percentage(100);
        //    this.Height = Unit.Percentage(100);

        //    InitChildControls(WebControlUtility.FindControlByHtmlIDProperty(container, "imageUploader_container", true));
        //}

        //protected override string GetDialogFeature()
        //{
        //    WindowFeature feature = new WindowFeature();

        //    feature.Width = 400;
        //    feature.Height = 400;
        //    feature.Resizable = false;
        //    feature.ShowStatusBar = false;
        //    feature.ShowScrollBars = false;
        //    feature.Center = true;

        //    return feature.ToDialogFeatureClientString();
        //}

        //protected override void InitConfirmButton(HtmlInputButton confirmButton)
        //{
        //    base.InitConfirmButton(confirmButton);
        //    confirmButton.Attributes["onclick"] = "$find('" + this.ClientID + "').onConfirmButtonClick();";

        //    this._confirmButton = confirmButton;
        //}
        #endregion

        #region 私有方法
        private void InitChildControls(Control container)
        {
            if (container != null)
            {
                //_imageUploader.ImageWidth = Unit.Pixel(280);
                //_imageUploader.ImageHeight = Unit.Pixel(250);
                container.Controls.Add(_imageUploader);
            }
        }

        #endregion

    }


    ///// <summary>
    ///// ImageUploaderDialog的参数
    ///// </summary>
    //[Serializable]
    //public class ImageUploaderDialogParams : DialogControlParamsBase
    //{
    //    public const string DefaultDialogTitle = "上传图片";
    //}
}
