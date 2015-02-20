using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using MCS.Library.SOA.DataObjects;
using MCS.Web.WebControls;
using System.IO;
using MCS.Library.Core;

namespace MCS.Library.SOA.Web.WebControls.Test
{
    public partial class ImageUploadTest : System.Web.UI.Page
    {

        protected override void OnPreInit(EventArgs e)
        {
            base.OnPreInit(e);
            try
            {
                ImageProperty prop = ImagePropertyAdapter.Instance.Load("0045a55f-5df2-a978-499d-04c5f4658830");
                ImageProperty clone = prop.Clone();
                clone.Changed = true;
                //clone.Content.FileName = "1234.png";
                clone.EnsureMaterialContent();
                
                this.imgUploader.ImgProp = prop;

            }
            catch (Exception ex)
            {
                Response.Write(ex.Message);
            }
            if (!IsPostBack)
            {
                //ImageProperty prop = ImagePropertyAdapter.Instance.Load(p => p.AppendItem("RESOURCE_ID", "add06a1f-2b17-979b-4293-89ad3869da1a"))[0];
                //this.imgUploader.ImgProp = prop;
            }
            //this.imgUploader.ImageContent = new ImageUploader.ImageContentHandler(GetImageContent);
        }

        private Stream GetImageContent()
        {
            
            IMaterialContentPersistManager materialManager = MaterialContentSettings.GetConfig().PersistManager;
            //ImageProperty prop = ImagePropertyAdapter.Instance.Load(contentId);
            MemoryStream stream = (MemoryStream)materialManager.GetMaterialContent("14773f2f-2adf-4ddc-926f-eda3b1b47b11");
            return stream;
        }


        protected void Page_Load(object sender, EventArgs e)
        {
        }

        protected void Button1_Click(object sender, EventArgs e)
        {
            var a = this.imgUploader.ImgProp;
        }

        //public void imgUploader_BeforeUpload(object sender,UploadEventArgs e)
        //{
        //    var a = e.FileName;
        //}

        //public void imgUploader_AfterUpload(object sender, UploadEventArgs e)
        //{
        //    var a = e.FileName;
        //    var imgUploader = sender as ImageUploader;
        //    ImagePropertyAdapter.Instance.Update(imgUploader.ImgProp);
        //    MaterialContent content = imgUploader.ImgProp.GenerateMaterialContent();
        //    //MCS.Web.WebControls.ImageUpload.Save(content);
        //}

        protected void Button1_Click1(object sender, EventArgs e)
        {
            this.imgUploader.ReadOnly = false;
        }
    }
}