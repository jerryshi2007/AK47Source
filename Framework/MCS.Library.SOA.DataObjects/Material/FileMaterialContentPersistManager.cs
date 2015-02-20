using System;
using System.IO;
using System.Linq;
using System.Text;
using System.Collections.Generic;
using MCS.Library.Core;
using MCS.Library.SOA.DataObjects.Properties;

namespace MCS.Library.SOA.DataObjects
{
	/// <summary>
	/// 基于文件的附件内容管理
	/// </summary>
	public class FileMaterialContentPersistManager : MaterialContentPersistManagerBase
	{
		public FileMaterialContentPersistManager()
		{
		}

		#region IMaterialContentPersistManager
		public override void SaveMaterialContent(MaterialContent content)
		{
			SourceFileInfo.NullCheck("SourceFileInfo");
			DestFileInfo.NullCheck("DestFileInfo");

			if (CheckSourceFileExists)
				ExceptionHelper.FalseThrow(SourceFileInfo.Exists, string.Format(Resource.FileNotFound, SourceFileInfo.Name));

			MoveFile(SourceFileInfo, DestFileInfo);
		}

		public override Stream GetMaterialContent(string contentID)
		{
			DestFileInfo.NullCheck("DestFileInfo");

			ExceptionHelper.FalseThrow<FileNotFoundException>(File.Exists(DestFileInfo.FullName), Resource.FileNotFound, DestFileInfo.FullName);

			return DestFileInfo.OpenRead();
		}

		public override bool ExistsContent(string contentID)
		{
			DestFileInfo.NullCheck("DestFileInfo");

			return DestFileInfo.Exists;
		}

		public override void DeleteMaterialContent(MaterialContent content)
		{
			DestFileInfo.NullCheck("DestFileInfo");

			DeleteFile(DestFileInfo);
		}
		#endregion IMaterialContentPersistManager
	}
}
