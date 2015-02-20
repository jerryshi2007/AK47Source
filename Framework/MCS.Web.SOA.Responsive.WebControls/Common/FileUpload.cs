using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Data;
using System.IO;
using System.Runtime.InteropServices;
using System.Text;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using MCS.Library.Caching;
using MCS.Library.Core;
using MCS.Library.SOA.DataObjects;
using MCS.Web.Library;
using MCS.Web.Library.Script;
using MCS.Web.Responsive.Library;
using MCS.Web.Responsive.WebControls.Properties;

namespace MCS.Web.Responsive.WebControls
{
	#region 上传下载
	public enum PathType
	{
		relative = 0,
		absolute = 1
	}

	/// <summary>
	/// 下载类型
	/// </summary>
	public enum DownloadTypeDefine
	{
		Document,
		Template,
		Print
	}

	public class FileDownloadInfo
	{
		public string RequestContext
		{
			get;
			internal set;
		}

		public string FilePath
		{
			get;
			internal set;
		}

		public string UserID
		{
			get;
			internal set;
		}

		public PathType PathType
		{
			get;
			internal set;
		}

		public string FileName
		{
			get;
			internal set;
		}

		public string RootPathName
		{
			get;
			internal set;
		}

		public string FileFullPath
		{
			get;
			internal set;
		}

		public DownloadTypeDefine DownloadType
		{
			get;
			internal set;
		}

		public static FileDownloadInfo CollectInfoFromRequest()
		{
			FileDownloadInfo info = new FileDownloadInfo();

			info.RequestContext = Request.GetRequestQueryValue("requestContext", string.Empty);

			info.FilePath = Request.GetRequestQueryValue("filePath", string.Empty);
			ExceptionHelper.CheckStringIsNullOrEmpty(info.FilePath, "filePath");

			info.UserID = Request.GetRequestQueryValue("userID", string.Empty);
			ExceptionHelper.CheckStringIsNullOrEmpty(info.UserID, "userID");

			info.PathType = Request.GetRequestQueryValue("pathType", PathType.relative);

			info.FileName = Request.GetRequestQueryValue("fileName", string.Empty);
			ExceptionHelper.CheckStringIsNullOrEmpty(info.FileName, "fileName");

			info.RootPathName = HttpUtility.UrlEncode(Request.GetRequestQueryValue<string>("rootPathName", string.Empty));
			ExceptionHelper.CheckStringIsNullOrEmpty(info.RootPathName, "rootPathName");

			info.FileFullPath = FileUpload.GetFileFullPath(info.PathType, info.RootPathName, info.FilePath);

			info.DownloadType = Request.GetRequestQueryValue("opType", DownloadTypeDefine.Document);

			return info;
		}
	}

	public delegate bool BeforeFileDownloadHandler(object sender, FileDownloadInfo info);
	public delegate void PrepareDownloadStreamHandler(object sender, PrepareDownloadStreamEventArgs args);

	internal class FileUpload
	{
		private const string NewMaterialID = "newMaterialID";

		const string OfficeDocExtensions = "dot,dotm,dotx,doc,docm,docx,rtf,wiz,xlsx,xlsm,xlsb,xls,xltx,xltm,xlt,csv,xlam,xla,xml,pptx,pptm,ppt,potx,potm,pot,ppsx,ppsm,pps,ppam,ppa,pwz,vsd,vss,vst,vsx,vdx,vsw,vtx";
		const int FileTooLargeError = -2147467259;

		public static void Download()
		{
			Download(null, null, null, false);
		}

		public static bool Processed
		{
			get
			{
				return ObjectContextCache.Instance.ContainsKey("FileUploadProcessed");
			}
		}

		/// <summary>
		/// 下载模板或已存在的文件
		/// </summary>
		public static void Download(object sender, BeforeFileDownloadHandler beforeDownloadHandler, PrepareDownloadStreamHandler prepareDownloadStreamHandler, bool raiseEvent)
		{
			ObjectContextCache.Instance["FileUploadProcessed"] = true;

			HttpResponse response = HttpContext.Current.Response;

			bool fileReadonly = Request.GetRequestQueryValue<bool>("fileReadonly", false);

			string materialID = string.Empty;

			try
			{
				string controlID = Request.GetRequestQueryValue("controlID", string.Empty);

				FileDownloadInfo downloadInfo = FileDownloadInfo.CollectInfoFromRequest();

				if (File.Exists(downloadInfo.FileFullPath) == false)
				{
					//是否需要启用映射机制（归档后，文件根目录的映射）
					string mappedRootPath = AppPathMappingContext.GetMappedPathName(downloadInfo.RootPathName);

					downloadInfo.FileFullPath = GetFileFullPath(downloadInfo.PathType, mappedRootPath, downloadInfo.FilePath);
				}

				response.ContentType = FileConfigHelper.GetFileContentType(downloadInfo.FileName);

				materialID = Request.GetRequestQueryValue("materialID", string.Empty);

				if (materialID.IsNullOrEmpty())
					materialID = UuidHelper.NewUuidString();

				if (fileReadonly)
				{
					WebFileOpenMode openMode = FileConfigHelper.GetFileOpenMode(downloadInfo.FileName, downloadInfo.UserID);

					response.AppendHeader("content-disposition",
						string.Format("{0};filename={1}", openMode == WebFileOpenMode.Inline ? "inline" : "attachment", response.EncodeFileNameInContentDisposition(downloadInfo.FileName)));
				}
				else
				{
					string fileIconPath = FileConfigHelper.GetFileIconPath(downloadInfo.FileName);

					response.AppendHeader("fileIconPath", HttpUtility.UrlEncode("message=" + fileIconPath));
					response.AppendHeader("content-disposition", "attachment;fileName=" + response.EncodeFileNameInContentDisposition(downloadInfo.FileName));

					if (downloadInfo.PathType != PathType.relative)
						response.AppendHeader("materialID", "message=" + materialID);
				}

				bool responseFile = true;

				if (beforeDownloadHandler != null && raiseEvent)
					responseFile = beforeDownloadHandler(sender, downloadInfo);

				if (responseFile)
				{
					IMaterialContentPersistManager persistManager =
						GetMaterialContentPersistManager(materialID, new FileInfo(downloadInfo.FileFullPath));

					using (Stream stream = persistManager.GetMaterialContent(materialID))
					{
						if (prepareDownloadStreamHandler != null && raiseEvent)
						{
							PrepareDownloadStreamEventArgs args = new PrepareDownloadStreamEventArgs();

							args.DownloadInfo = downloadInfo;
							args.InputStream = stream;
							args.OutputStream = response.OutputStream;

							prepareDownloadStreamHandler(sender, args);
						}
						else
						{
							stream.CopyTo(response.OutputStream);
						}
					}
				}

				Control control = null;

				if (controlID.IsNotEmpty())
					control = ((Page)HttpContext.Current.CurrentHandler).FindControlByID(controlID, true);

				//todo,MaterialControl
				//if (control != null && control is MaterialControl)
				//{
				//    DownloadEventArgs args = new DownloadEventArgs(materialID, downloadInfo.FilePath);
				//    ((MaterialControl)control).OnDownloadFile(args);
				//}
			}
			catch (Exception ex)
			{
				if (fileReadonly)
					response.Write(ex.Message);
				else
					GenerateErrorInformation(ex.ToString());
			}
			finally
			{
				try
				{
					response.End();
				}
				catch
				{
				}
			}
		}

		private static IMaterialContentPersistManager GetMaterialContentPersistManager(string contentID, FileInfo destFile)
		{
			IMaterialContentPersistManager persistManager = MaterialContentSettings.GetConfig().PersistManager;

			if ((persistManager is FileMaterialContentPersistManager) == false)
			{
				if (persistManager.ExistsContent(contentID) == false)
				{
					persistManager = new FileMaterialContentPersistManager();
				}
			}

			persistManager.DestFileInfo = destFile;

			return persistManager;
		}

		/// <summary>
		/// </summary>
		/// <param name="pType"></param>
		/// <param name="fileRelativePath"></param>
		/// <returns></returns>
		internal static string GetFileFullPath(PathType pType, string rootPathName, string fileRelativePath)
		{
			string fileFullPath = string.Empty;
			string uploadRootPath = GetUploadRootPath(rootPathName);

			if (pType == PathType.relative)
			{
				fileFullPath = uploadRootPath + fileRelativePath;

				AppPathConfigSettings.GetConfig().CheckPathIsConfiged(fileFullPath);
			}
			else
			{
				fileFullPath = HttpContext.Current.Server.MapPath(fileRelativePath);

				CheckTemplateFile(fileFullPath);
			}

			return fileFullPath;
		}

		/// <summary>
		/// 检查拓展名是否合法
		/// </summary>
		/// <param name="fileName"></param>
		/// <returns></returns>
		private static void CheckTemplateFile(string fileName)
		{
			string fileExt = Path.GetExtension(fileName);

			ExceptionHelper.TrueThrow(string.IsNullOrEmpty(fileExt), string.Format(Resources.IllegalPath, fileName));

			ExceptionHelper.TrueThrow(OfficeDocExtensions.IndexOf(fileExt.Trim('.').ToLower()) == -1, string.Format(Resources.IllegalPath, fileName));
		}

		/// <summary>
		/// 上传
		/// </summary>
		public static void Upload()
		{
			ObjectContextCache.Instance["FileUploadProcessed"] = true;

			HttpRequest request = HttpContext.Current.Request;
			HttpResponse response = HttpContext.Current.Response;

			string lockID = Request.GetRequestQueryValue<string>("lockID", string.Empty);
			string userID = Request.GetRequestQueryValue<string>("userID", string.Empty);
			string fileName = Request.GetRequestQueryValue<string>("fileName", string.Empty);
			int fileMaxSize = Request.GetRequestQueryValue<int>("fileMaxSize", 0);
			string controlID = Request.GetRequestQueryValue("controlID", string.Empty);

			ExceptionHelper.CheckStringIsNullOrEmpty(fileName, "fileName");

			try
			{
				if (fileMaxSize > 0 && request.ContentLength > fileMaxSize)
				{
					GenerateErrorInformation(string.Format("文件超过了上传大小的限制{0}字节", fileMaxSize));
				}
				else
				{
					//不检查锁，沈峥修改
					//CheckLock(lockID, userID);

					string uploadPath = GetUploadRootPath();

					AutoCreateUploadPath(uploadPath);

					Control control = null;

					if (string.IsNullOrEmpty(controlID) == false)
						control = ((Page)HttpContext.Current.CurrentHandler).FindControlByID(controlID, true);

					string newID = UuidHelper.NewUuidString();

					if (fileName.IndexOf(NewMaterialID) == 0)
						fileName = fileName.Replace(NewMaterialID, newID);
					else
						newID = Path.GetFileNameWithoutExtension(fileName);

					//todo,MaterialControl
					//if (control != null && control is MaterialControl)
					//{
					//    UploadEventArgs args = new UploadEventArgs(fileName);
					//    ((MaterialControl)control).OnBeforeUploadFile(args);
					//}

					if (request.QueryString["upmethod"] == "new")
					{
						var dialogControlID = Request.GetRequestQueryValue("dialogControlID", string.Empty);
						request.Files[0].SaveAs(uploadPath + @"Temp\" + fileName);

						string output = "<script type='text/javascript'>";
						output += "window.parent.$find('" + dialogControlID + "').onUploadFinish(1)";
						output += "</script>";

						response.Write(output);
					}
					else
					{
						request.SaveAs(uploadPath + @"Temp\" + fileName, false);
					}

					//todo,MaterialControl
					//if (control != null && control is MaterialControl)
					//{
					//    UploadEventArgs args = new UploadEventArgs(fileName);
					//    ((MaterialControl)control).OnAfterUploadFile(args);
					//}

					string fileIconPath = FileConfigHelper.GetFileIconPath(fileName);

					response.AppendHeader("fileIconPath", HttpUtility.UrlEncode("message=" + fileIconPath));

					string dateTimeNow = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss.fff");
					response.AppendHeader("lastUploadTag", "message=" + dateTimeNow);
					response.AppendHeader("newMaterialID", "message=" + HttpUtility.UrlEncode(newID));
				}
			}
			catch (Exception ex)
			{
				string errorMessage = ex.ToString();

				if (ex is ExternalException && ((ExternalException)(ex)).ErrorCode == FileTooLargeError
					&& ex.Source == "System.Web")
					errorMessage = "您上传的文件过大";

				GenerateErrorInformation(errorMessage);
			}
			finally
			{
				try
				{
					response.End();
				}
				catch { }
			}
		}

		/// <summary>
		/// 检查上传路径是否存在，如果不存在则创建该路径
		/// </summary>
		private static void AutoCreateUploadPath(string uploadRootPath)
		{
			ExceptionHelper.CheckStringIsNullOrEmpty(uploadRootPath, "uploadRootPath");

			if (Directory.Exists(uploadRootPath) == false)
				Directory.CreateDirectory(uploadRootPath);

			string uploadTempPath = uploadRootPath + @"Temp\";

			if (Directory.Exists(uploadTempPath) == false)
				Directory.CreateDirectory(uploadTempPath);
		}

		/// <summary>
		/// 检查锁
		/// </summary>
		private static void CheckLock(string lockID, string personID)
		{
			if (!string.IsNullOrEmpty(lockID) && !string.IsNullOrEmpty(personID))
			{
				CheckLockResult result = LockAdapter.CheckLock(lockID, personID);

				//如果不是本人加锁　则不允许上传
				ExceptionHelper.FalseThrow(
					(result.CurrentLockStatus == LockStatus.LockedByRight
					|| result.CurrentLockStatus == LockStatus.LockedByRightAndExpire),
					CheckLockOperation.GetCheckLockStatusText(result));
			}
		}

		private static string GetUploadRootPath(string rootPathName)
		{
			ExceptionHelper.CheckStringIsNullOrEmpty(rootPathName, "rootPathName");

			AppPathSettingsElement elem = AppPathConfigSettings.GetConfig().Paths[rootPathName];

			ExceptionHelper.FalseThrow(elem != null, "不能在配置节appPathSettings下找到名称为\"{0}\"的路径定义", rootPathName);

			return elem.Dir;
		}

		/// <summary>
		/// 获得pathName对应的配置信息
		/// </summary>
		private static string GetUploadRootPath()
		{
			string rootPathName = HttpUtility.UrlEncode(Request.GetRequestQueryValue<string>("rootPathName", string.Empty));

			return GetUploadRootPath(rootPathName);
		}

		/// <summary>
		/// 向客户端传递错误信息
		/// </summary>
		/// <param name="string">错误信息内容</param>
		private static void GenerateErrorInformation(string errorInformation)
		{
			HttpResponse response = HttpContext.Current.Response;

			response.AppendHeader("errorMessage", HttpUtility.UrlEncode("message=" + errorInformation));

			if (HttpContext.Current.Request.QueryString["upmethod"] == "new")
			{
				response.ClearContent();
				var dialogControlID = Request.GetRequestQueryValue("dialogControlID", string.Empty);
				string output = "<script type='text/javascript'>";
				output += "window.parent.$find('" + dialogControlID + "').onUploadFinish(2,'" + errorInformation + "')";
				output += "</script>";
				response.Write(output);
			}

		}
	}
	#endregion

	#region DownloadEventArgs
	/// <summary>
	/// 准备下载流的事件参数
	/// </summary>
	public class PrepareDownloadStreamEventArgs : EventArgs
	{
		private Stream _InputStream = null;
		private Stream _OutputStream = null;
		private FileDownloadInfo _DownloadInfo = null;

		internal PrepareDownloadStreamEventArgs()
		{
		}

		public FileDownloadInfo DownloadInfo
		{
			get { return this._DownloadInfo; }
			internal set { this._DownloadInfo = value; }
		}

		public Stream InputStream
		{
			get { return this._InputStream; }
			internal set { this._InputStream = value; }
		}

		public Stream OutputStream
		{
			get { return this._OutputStream; }
			internal set { this._OutputStream = value; }
		}
	}

	/// <summary>
	/// 下载事件
	/// </summary>
	public class DownloadEventArgs : EventArgs
	{
		private string materialID = string.Empty;
		private string filePath = string.Empty;

		public string FilePath
		{
			get
			{
				return this.filePath;
			}
		}

		internal DownloadEventArgs(string materialID, string filePath)
		{
			this.materialID = materialID;
			this.filePath = filePath;
		}

		public string MaterialID
		{
			get
			{
				return this.materialID;
			}
		}
	}

	/// <summary>
	/// 上传事件
	/// </summary>
	public class UploadEventArgs : EventArgs
	{
		private string fileName = string.Empty;

		public string FileName
		{
			get
			{
				return this.fileName;
			}
		}

		internal UploadEventArgs(string fileName)
		{
			this.fileName = fileName;
		}
	}

	#endregion

	#region 获得文件相关配置

	internal class FileConfigHelper
	{
		private const string AppName = "HB2008Portal";
		private const string ProgName = "Portal";
		private const string PropName = "FileOpenWay";
		private const string CategoryName = "CommonSettings";
		/// <summary>
		/// 获得文件图标
		/// </summary>
		/// <param name="originalName">文件名</param>
		/// <returns></returns>
		public static string GetFileIconPath(string originalName)
		{
			string fileIconPath = string.Empty;

			ContentTypeConfigElement elem = GetConfigElement(originalName);

			if (elem != null)
				fileIconPath = elem.LogoImage;

			return fileIconPath;
		}

		/// <summary>
		/// 获得ContentType
		/// </summary>
		/// <param name="originalName">文件名</param>
		/// <returns></returns>
		public static string GetFileContentType(string originalName)
		{
			string contentType = "application/octet-stream";

			ContentTypeConfigElement elem = GetConfigElement(originalName);

			if (elem != null)
				contentType = elem.ContentType;

			return contentType;
		}

		internal static string GetOpenInlineFileExts(string userID)
		{
			//return UserSettings.GetUserSettings(userID).GetPropertyValue(AppName, ProgName, PropName, GetOpenInLineFileExtensionNames()).ToString().ToLower();
			return UserSettings.GetSettings(userID).GetPropertyValue(CategoryName, PropName, GetOpenInlineFileExtensionNames()).ToString().ToLower();
		}

		internal static string GetOpenInlineFileExtensionNames()
		{
			StringBuilder sb = new StringBuilder(256);

			foreach (ContentTypeConfigElement elem in ContentTypesSection.GetConfig().ContentTypes)
			{
				if (sb.Length != 0 && sb[sb.Length - 1] != ',')
					sb.Append(",");

				if (elem.OpenMode == WebFileOpenMode.Inline)
				{
					for (int i = 0; i < elem.FileExtensionNames.Count; i++)
					{
						if (i != 0)
							sb.Append(",");

						sb.Append(elem.FileExtensionNames[i]);
					}
				}
			}

			return sb.ToString();
		}

		/// <summary>
		/// 获得openMode
		/// </summary>
		/// <param name="originalName">文件名</param>
		/// <returns></returns>
		internal static WebFileOpenMode GetFileOpenMode(string originalName, string userID)
		{
			WebFileOpenMode openMode = WebFileOpenMode.Attachment;

			string fileExt = Path.GetExtension(originalName);

			if (string.IsNullOrEmpty(fileExt) == false)
			{
				string openInlineFileExts = GetOpenInlineFileExts(userID);

				if (CheckInFileExts(openInlineFileExts, fileExt))
					openMode = WebFileOpenMode.Inline;
			}

			return openMode;
		}

		static bool CheckInFileExts(string openInlineFileExts, string fileExt)
		{
			string[] openInlineFileExt = openInlineFileExts.Split(new char[] { ';', ',' });

			fileExt = fileExt.Trim('.').ToLower();

			foreach (string str in openInlineFileExt)
			{
				if (fileExt.ToLower().Trim() == str.ToLower().Trim())
					return true;
			}

			return false;
		}

		/// <summary>
		/// 获得配置节点
		/// </summary>
		/// <param name="originalName">文件名</param>
		/// <returns></returns>
		private static ContentTypeConfigElement GetConfigElement(string originalName)
		{
			ContentTypeConfigElement elem =
				ContentTypesSection.GetConfig().ContentTypes.FindElementByFileName(originalName);

			if (elem != null)
				return elem;
			else
				return ContentTypesSection.GetConfig().DefaultElement;
		}
	}

	#endregion
}
