using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using MCS.Library.SOA.DataObjects;
using System.IO;

namespace MCS.Library.SOA.Web.WebControls.Test.ImageUpload
{
    public partial class ImageView : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            string id = "14773f2f-2adf-4ddc-926f-eda3b1b47b11";
            //ImageProperty imgProp = ImagePropertyAdapter.Instance.Load(id);
            MaterialContent imgContents = MaterialContentAdapter.Instance.Load(p => p.AppendItem("CONTENT_ID", id))[0];
            MemoryStream stream = new MemoryStream(imgContents.ContentData);
            stream.Position = 0;
            System.Drawing.Image img = System.Drawing.Image.FromStream(stream);
            Response.ContentType = "image/jpeg";
            Response.BinaryWrite(imgContents.ContentData);
            //Image img = System.Drawing.ima
            
        }
    }
}