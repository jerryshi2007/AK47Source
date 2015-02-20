using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web;
using System.Threading;
using MCS.Library.SOA.DataObjects;
using MCS.Web.Library.Script;
using MCS.Library.Core;
using System.IO;
using MCS.Library.Security;
using System.Web.Caching;
using System.Reflection;
using System.Web.UI;

namespace MCS.Web.Responsive.WebControls
{
    public class ImageUploadHandler : IHttpHandler
    {
        public bool IsReusable
        {
            get { return true; }
        }

        public void ProcessRequest(HttpContext context)
        {
            try
            {
                switch (context.Request.QueryString["action"])
                {
                    case "upload":
                        if (context.Request.Files.Count > 0)
                            this.ProcessFileUpload(context);
                        else
                            BadRequest(context.Response, "没有上传文件");
                        break;
                    case "getimage":
                        ProcessGetImage(context);
                        break;
                    case "getdefaultimage":
                        ProcessGetDefaultImage(context);
                        break;
                    default:
                        break;
                }

            }
            catch (Exception)
            {
                context.Response.StatusCode = 500;
            }
        }

        private void ProcessGetDefaultImage(HttpContext context)
        {
            byte[] content = ResourceHelper.GetResourceBytes(typeof(ImageUploadHandler).Assembly,
                        "MCS.Web.Responsive.WebControls.ImageUploader.cannotShow.png");
            context.Response.ContentType = "image/jpeg";
            context.Response.Cache.SetNoStore();
            context.Response.BinaryWrite(content);
        }

        private void ProcessGetImage(HttpContext context)
        {
            string contentID = context.Request.QueryString["imagePropID"];
            string filePath = context.Request.QueryString["filePath"];
            if (string.IsNullOrEmpty(contentID) == false)
            {
                if (string.IsNullOrEmpty(filePath) == false)
                {
                    filePath = MachineKeyEncryptor.Decrypt(filePath);
                    string filename = Path.GetFileName(filePath);

                    if (File.Exists(filePath))
                    {
                        context.Response.ContentType = "image/jpeg";
                        context.Response.AddFileDependency(filePath);
                        context.Response.Cache.SetLastModifiedFromFileDependencies();
                        context.Response.Cache.VaryByParams["contentID"] = true;
                        context.Response.Cache.VaryByParams["filePath"] = true;
                        context.Response.Cache.SetETagFromFileDependencies();
                        var cacheCtrl = context.Request.Headers["Cache-Control"];
                        var ifModifiedSince = context.Request.Headers["If-Modified-Since"];
                        string lastChangeSince;

                        try
                        {
                            lastChangeSince = context.Response.Headers["Last-Modified"];
                        }
                        catch (Exception)
                        {
                            lastChangeSince = null;
                        }

                        if (string.IsNullOrEmpty(ifModifiedSince) == false && ifModifiedSince == lastChangeSince)
                        {
                            context.Response.StatusCode = 302;
                        }
                        else
                        {
                            context.Response.WriteFile(filePath, false);
                        }
                    }
                    else
                    {
                        ProcessGetImageById(contentID, context);
                    }
                }
                else
                {
                    ProcessGetImageById(contentID, context);
                }
            }
            else
            {
                BadRequest(context.Response, "没有指定imagePropID参数");
            }
        }

        private void ProcessGetImageById(string propID, HttpContext context)
        {
            MaterialContentCollection mcs = MaterialContentAdapter.Instance.Load(builder => builder.AppendItem("CONTENT_ID", propID));
            if (mcs.Count > 0)
            {
                MaterialContent matirial = mcs[0];
                string fileName = matirial.FileName;
                byte[] content = matirial.ContentData;

                context.Response.ContentType = "image/jpeg";
                context.Response.BinaryWrite(content);
            }
            else
            {
                byte[] content = ResourceHelper.GetResourceBytes(typeof(ImageUploadHandler).Assembly,
                         "MCS.Web.Responsive.WebControls.ImageUploader.cannotShow.png");
                context.Response.ContentType = "image/jpeg";
                context.Response.Cache.SetNoStore();
                context.Response.BinaryWrite(content);
            }
        }

        private void ProcessFileUpload(HttpContext context)
        {
            HttpPostedFile file = context.Request.Files[0];
            if (file.ContentType == "image/jpeg")
            {
                string maxSizeString = context.Request.QueryString["maxSize"];
                int maxSize = string.IsNullOrEmpty(maxSizeString) ? -1 : int.Parse(maxSizeString);
                string imgInfoStr = context.Request.Form["imageInfo"];
                if (string.IsNullOrEmpty(imgInfoStr) == false)
                {
                    var imgProp = JSONSerializerExecute.Deserialize<ImageProperty>(imgInfoStr);

                    if (string.IsNullOrWhiteSpace(imgProp.ID))
                        imgProp.ID = Guid.NewGuid().ToString();

                    imgProp.NewName = UuidHelper.NewUuidString() + Path.GetExtension(imgProp.OriginalName);
                    imgProp.Changed = true;
                    imgProp.UpdateTime = DateTime.Now;

                    if (maxSize < 0 || file.ContentLength <= maxSize)
                    {
                        string filePath;
                        // ImageUploadHelper.UploadFile(file, imgProp.OriginalName, imgProp.NewName, out filePath);


                        string path = GetUploadRootPath("ImageUploadRootPath");
                        string tempPath = Path.Combine(path + @"Temp\", imgProp.NewName);
                        AutoCreateUploadPath(path);

                        // var beforeArgs = new UploadEventArgs(originalName);
                        // uploadControl.OnBeforeUploadFile(beforeArgs);
                        file.SaveAs(Path.Combine(path + @"Temp\", imgProp.NewName));
                        //var afterArgs = new UploadEventArgs(newName);
                        //uploadControl.OnAfterUploadFile(afterArgs);

                        filePath = tempPath;

                        imgProp.FilePath = MachineKeyEncryptor.Encrypt(filePath);

                        string imgPropJsonStr = JSONSerializerExecute.Serialize(imgProp);

                        string uploadImageShowenUrl = "some.imgupload?action=getimage&imagePropID=" + imgProp.ID + "&filePath=" + MachineKeyEncryptor.Encrypt(filePath);

                        context.Response.ContentType = "text/html";
                        var output = context.Response.Output;

                        System.Web.UI.HtmlTextWriter writer = new System.Web.UI.HtmlTextWriter(output);
                        BeginForm(writer);

                        AddInput(writer, "imgInfo", imgPropJsonStr);
                        AddInput(writer, "imgPath", uploadImageShowenUrl);

                        EndForm(writer);

                        output.Close();


                        //context.Response.Write(GetResponseTextScript(imgPropJsonStr));

                        //context.Response.Write(GetUploadImageUrlByFile(uploadImageShowenUrl));

                        //string paramsData = string.Format("['{0}','{1}']", "document.getElementById('responseInfo').value", uploadImageShowenUrl);

                        //context.Response.Write(GetClientControlInvokeStript(GetPostedControlID(), "uploadSuccess", "document.getElementById('responseInfo').value", "document.getElementById('uploadImageUrlByFile').value"));
                    }
                    else
                    {
                        BadRequest(context.Response, "上传文件太大");
                    }
                }
            }
        }

        private void AddInput(HtmlTextWriter writer, string name, string value)
        {

            writer.WriteBeginTag("input");
            writer.WriteAttribute("id", name);
            writer.WriteAttribute("type", "hidden");
            writer.WriteAttribute("value", value, true);
            writer.Write(HtmlTextWriter.SelfClosingTagEnd);
        }

        private static void EndForm(System.Web.UI.HtmlTextWriter writer)
        {
            writer.EndRender();
            writer.RenderEndTag();
            writer.RenderEndTag();
            writer.RenderEndTag();

            writer.Flush();
        }

        private static void BeginForm(System.Web.UI.HtmlTextWriter writer)
        {
            writer.WriteLine("<!DOCTYPE html>");
            writer.AddAttribute("xmlns", "http://www.w3.org/1999/xhtml");
            writer.RenderBeginTag("html");
            writer.RenderBeginTag("body");
            writer.AddAttribute("autocomplete", "off");
            writer.RenderBeginTag("form");
            writer.BeginRender();
        }

        private void BadRequest(HttpResponse httpResponse, string reason)
        {
            httpResponse.StatusCode = 400;

        }

        private static void AutoCreateUploadPath(string uploadRootPath)
        {
            ExceptionHelper.CheckStringIsNullOrEmpty(uploadRootPath, "uploadRootPath");

            if (Directory.Exists(uploadRootPath) == false)
                Directory.CreateDirectory(uploadRootPath);

            string uploadTempPath = uploadRootPath + @"Temp\";

            if (Directory.Exists(uploadTempPath) == false)
                Directory.CreateDirectory(uploadTempPath);
        }

        public static string GetUploadRootPath(string rootPathName)
        {
            ExceptionHelper.CheckStringIsNullOrEmpty(rootPathName, "rootPathName");

            ImagePathSettingsElement elem = (ImagePathSettingsElement)ImagePathConfigSettings.GetConfig().Paths[rootPathName];

            ExceptionHelper.FalseThrow(elem != null, "不能在配置节imagePathSettings下找到名称为\"{0}\"的路径定义", rootPathName);

            return elem.Dir;
        }

    }
}
