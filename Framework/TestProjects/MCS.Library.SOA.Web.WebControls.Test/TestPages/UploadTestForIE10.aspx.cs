using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace MCS.Library.SOA.Web.WebControls.Test.TestPages
{
    public partial class UploadTestForIE10 : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            var fileName=Request.QueryString["fileName"];
            if (!string.IsNullOrEmpty(fileName))
            {
                try
                {
                    fileName = fileName.Substring(fileName.LastIndexOf('.'));
                    Request.SaveAs(Server.MapPath("~/TestPages/" + Guid.NewGuid() + fileName), false);
                    Response.Clear();
                    Response.AppendHeader("uploadInfo", HttpUtility.UrlEncode("上传成功!"));
                }
                catch (Exception ex)
                {
                    Response.AppendHeader("uploadInfo", HttpUtility.UrlEncode("上传失败!" + ex.Message));
                }
              
            }
        }
    }
}