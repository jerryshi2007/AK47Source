using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using MCS.Web.Library;
using MCS.Library.Core;
using MCS.Library.SOA.DataObjects;
using System.Reflection;
using System.IO;

namespace MCS.Library.SOA.Web.WebControls.Test.UEditorWrapperTest
{
    public class UEditorModule : IHttpModule
    {
        public void Dispose()
        {
            
        }

        public void Init(HttpApplication app)
        {
            app.BeginRequest += new EventHandler(BeginRequest);
        }

        private void BeginRequest(object sender, EventArgs e)
        {
            HttpApplication objApp = (HttpApplication)sender;
            HttpContext context = objApp.Context;
            string imgName = WebUtility.GetRequestQueryValue("uploadedImageName", "");
            string rootPath = WebUtility.GetRequestQueryValue("rootPath", "");

            if (rootPath != "")
            {
                //string a = "213123";
            }
            if (string.IsNullOrEmpty(imgName))
            {
                return;
            }
            ShowImage(imgName, rootPath, false, objApp);

        }

        private void ShowImage(string imgName, string rootPath, bool cache, HttpApplication objApp)
        {
            HttpResponse response = objApp.Response;

            try
            {
                byte[] content = null;

                string fileName = string.Empty;

                if (imgName.IsNotEmpty())
                    content = GetImageBytesFromFilePath(imgName, rootPath);
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

        private byte[] GetImageBytesFromFilePath(string fileName, string rootPath)
        {
            byte[] content = null;
            string filePath = GetUploadRootPath(rootPath) + "Temp\\" + fileName;
            fileName = Path.GetFileName(filePath);

            if (File.Exists(filePath))
            {
                using (FileStream fs = new FileStream(filePath, FileMode.Open, FileAccess.Read, FileShare.Read))
                {
                    content = fs.ToBytes();
                }
            }

            return content;

            throw new NotImplementedException();
        }

        private static string GetUploadRootPath(string rootPathName)
        {
            ExceptionHelper.CheckStringIsNullOrEmpty(rootPathName, "rootPathName");

            AppPathSettingsElement elem = AppPathConfigSettings.GetConfig().Paths[rootPathName];

            ExceptionHelper.FalseThrow(elem != null, "不能在配置节appPathSettings下找到名称为\"{0}\"的路径定义", rootPathName);

            return elem.Dir;
        }

    }
}