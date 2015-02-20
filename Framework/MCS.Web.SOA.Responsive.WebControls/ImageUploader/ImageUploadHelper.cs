using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Web;
using MCS.Library.SOA.DataObjects;
using MCS.Library.Core;

namespace MCS.Web.Responsive.WebControls
{
	public class ImageUploadHelper
	{
		public static void Save(MaterialContent content)
		{
			IMaterialContentPersistManager manager = MaterialContentSettings.GetConfig().PersistManager;
			string path = GetUploadRootPath("ImageUploadRootPath");
			string fileName = Path.Combine(path + @"Temp\", content.FileName);

			if (manager is FileMaterialContentPersistManager)
				manager.DestFileInfo = new System.IO.FileInfo(Path.Combine(path, content.FileName));

			manager.SourceFileInfo = new System.IO.FileInfo(fileName);
			manager.SaveMaterialContent(content);
		}

		public static void UploadFile(ImageUploader uploadControl, HttpPostedFile file, string originalName, string newName, out string filePath)
		{
            filePath = string.Empty;

			string path = GetUploadRootPath("ImageUploadRootPath");
            string tempPath = Path.Combine(path + @"Temp\", newName);
			AutoCreateUploadPath(path);

			var beforeArgs = new UploadEventArgs(originalName);
            //uploadControl.OnBeforeUploadFile(beforeArgs);
			file.SaveAs(Path.Combine(path + @"Temp\", newName));
			var afterArgs = new UploadEventArgs(newName);
            //uploadControl.OnAfterUploadFile(afterArgs);

            filePath = tempPath;
		}

        public static void UploadFile(HttpPostedFile file, string originalName, string newName, string rootPath, out string filePath)
        {
            filePath = string.Empty;
            string path = GetUploadRootPath(rootPath);
            string tempPath = Path.Combine(path + @"Temp\", newName);
            AutoCreateUploadPath(path);
            file.SaveAs(Path.Combine(path + @"Temp\", newName));
            filePath = tempPath;
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

		private static string GetFileName(string fileName)
		{
			string ext = Path.GetExtension(fileName);
			string noExtName = Path.GetFileNameWithoutExtension(fileName);

			return string.Format("{1}{2}", noExtName, Guid.NewGuid().ToString(), ext);
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
