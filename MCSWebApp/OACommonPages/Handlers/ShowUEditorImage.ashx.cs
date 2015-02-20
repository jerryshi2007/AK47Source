using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using MCS.Web.Library;
using MCS.Library.Core;
using MCS.Library.SOA.DataObjects;
using System.Reflection;
using System.IO;
using MCS.Web.WebControls;

namespace MCS.Library.SOA.Web.WebControls.Test.Handlers
{
    /// <summary>
    /// Summary description for ShowUEditorImage
    /// </summary>
    public class ShowUEditorImage : IHttpHandler
    {
		public void ProcessRequest(HttpContext context)
		{
			string id = WebUtility.GetRequestQueryValue("id", "");
			string imgName = WebUtility.GetRequestQueryValue("imageName", "");
			string rootPath = WebUtility.GetRequestQueryValue("rootPath", "");

			if (string.IsNullOrEmpty(id) || string.IsNullOrEmpty(imgName))
			{
				return;
			}

			ShowImage(id, imgName, rootPath, false, context);
		}

		public bool IsReusable
		{
			get
			{
				return false;
			}
		}

		private void ShowImage(string id, string imgName, string rootPath, bool cache, HttpContext context)
		{
			try
			{
				byte[] content = null;

				string fileName = string.Empty;

				if (imgName.IsNotEmpty())
					content = GetImageBytesFromFilePath(imgName, rootPath);

				if (content == null)
					content = GetImageBytesFromID(id, out fileName);

				if (content == null)
				{
					fileName = "cannotShow.png";
					content = ResourceHelper.GetResourceBytes(Assembly.GetExecutingAssembly(),
															  "MCS.Web.WebControls.ImageUploaderControl.cannotShow.png");
				}

				if (cache == false)
					context.Response.Cache.SetCacheability(HttpCacheability.NoCache);

				context.Response.ContentType = WebUtility.GetContentTypeByFileName(fileName);
				context.Response.BinaryWrite(content);
			}
			catch (System.Exception ex)
			{
				context.Response.Write(ex.Message);
			}
			finally
			{
				context.Response.End();
			}
		}

		private static byte[] GetImageBytesFromID(string id, out string fileName)
		{
			MaterialContentCollection mcs = MaterialContentAdapter.Instance.Load(builder => builder.AppendItem("CONTENT_ID", id));

			byte[] content = null;
			fileName = string.Empty;

			if (mcs.Count > 0)
			{
				fileName = mcs[0].FileName;
				content = mcs[0].ContentData;
			}

			return content;
		}

		private byte[] GetImageBytesFromFilePath(string fileName, string rootPath)
		{
			byte[] content = null;
			string filePath = ImageUploadHelper.GetUploadRootPath(rootPath) + "Temp\\" + fileName;

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

    }
}