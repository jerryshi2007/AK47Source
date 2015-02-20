using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MCS.Library.Core;
using System.IO;

namespace MCS.Library.SOA.DataObjects
{
	/// <summary>
	/// 默认的附件文件操作的实现类
	/// </summary>
	public class DefaultMaterialFileOperation : IMaterialFileOperation
	{
		public readonly static DefaultMaterialFileOperation Instance = new DefaultMaterialFileOperation();

		public void DecorateMaterialListAfterLoad(MaterialList materials)
		{
			//这里可以调整附件用于显示的Url
			//materials.ForEach(m => m.ShowFileUrl = "http://localhost/MCSWebApp/WebTestProject/OfficeViewer/pki.doc");
		}

		public void DoModifyFileOperations(string rootPathName, MaterialFileOeprationInfo fileOp, MaterialContent content)
		{
			string uploadRootPath = AppPathConfigSettings.GetConfig().Paths[rootPathName].Dir;

			ExceptionHelper.CheckStringIsNullOrEmpty(uploadRootPath, "uploadRootPath");

			IMaterialContentPersistManager persistManager = GetMaterialContentPersistManager(rootPathName, fileOp);

			if (content == null)
				content = fileOp.Material.GenerateMaterialContent();

			switch (fileOp.Operation)
			{
				case FileOperation.Add:
				case FileOperation.Update:
					persistManager.SaveMaterialContent(content);
					break;
				case FileOperation.Delete:
					persistManager.DeleteMaterialContent(content);
					break;
				default:
					break;
			}
		}

		private static IMaterialContentPersistManager GetMaterialContentPersistManager(string rootPathName, MaterialFileOeprationInfo fileOp)
		{
			string uploadRootPath = AppPathConfigSettings.GetConfig().Paths[rootPathName].Dir;

			FileInfo sourceFile = new FileInfo(uploadRootPath + @"Temp\" + Path.GetFileName(fileOp.Material.RelativeFilePath));
			FileInfo destFile = new FileInfo(uploadRootPath + fileOp.Material.RelativeFilePath);

			IMaterialContentPersistManager persistManager = MaterialContentSettings.GetConfig().PersistManager;

			persistManager.SourceFileInfo = sourceFile;
			persistManager.DestFileInfo = destFile;

			if (fileOp.Operation == FileOperation.Update)
				persistManager.CheckSourceFileExists = false;

			return persistManager;
		}
	}
}
