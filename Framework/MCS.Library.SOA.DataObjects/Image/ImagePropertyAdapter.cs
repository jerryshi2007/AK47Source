using System;
using System.IO;
using System.Data;
using System.Text;
using System.Collections.Generic;
using System.Collections;
using System.Transactions;
using MCS.Library.Data;
using MCS.Library.Data.Builder;
using MCS.Library.Data.Mapping;
using MCS.Library.Core;
using System.Data.Common;
using MCS.Library.SOA.DataObjects.Properties;
using System.Data.SqlClient;
using MCS.Library.SOA.DataObjects;
using MCS.Library.SOA.DataObjects.Workflow;

namespace MCS.Library.SOA.DataObjects
{
	public class ImagePropertyAdapter : UpdatableAndLoadableAdapterBase<ImageProperty, ImagePropertyCollection>
	{
		private ImagePropertyAdapter()
		{ }

		public static readonly ImagePropertyAdapter Instance = new ImagePropertyAdapter();

		public ImageProperty Load(string imageID)
		{
			imageID.CheckStringIsNullOrEmpty("imageID");

			ImagePropertyCollection imgProp = Load(builder => builder.AppendItem("ID", imageID));

			(imgProp.Count > 0).FalseThrow("不能找到imageID为{0}的记录", imageID);

			return imgProp[0];
		}

		public ImagePropertyCollection LoadByResourceID(string resourceID)
		{
			resourceID.CheckStringIsNullOrEmpty("resourceID");

			return Load(builder => builder.AppendItem("RESOURCE_ID", resourceID));
		}

		public string GetUploadRootPath(string rootPathName)
		{
			ExceptionHelper.CheckStringIsNullOrEmpty(rootPathName, "rootPathName");

			ImagePathSettingsElement elem = (ImagePathSettingsElement)ImagePathConfigSettings.GetConfig().Paths[rootPathName];

			ExceptionHelper.FalseThrow(elem != null, "不能在配置节imagePathSettings下找到名称为\"{0}\"的路径定义", rootPathName);

			return elem.Dir;
		}

		public void UpdateWithContent(ImageProperty image)
		{
			image.NullCheck("image");

			using (TransactionScope scope = TransactionScopeFactory.Create())
			{
				if (image.IsEmpty())
					Delete(image);
				else
					Update(image);

				UpdateContent(image);

				scope.Complete();
			}
		}

		/// <summary>
		/// 得到上传图片的临时路径的物理目录
		/// </summary>
		/// <param name="fileName"></param>
		/// <returns></returns>
		public System.IO.FileInfo GetPhysicalUploadImagePath(string fileName)
		{
			fileName.NullCheck("fileName");

			string path = GetUploadRootPath("ImageUploadRootPath");

			return new System.IO.FileInfo(Path.Combine(path + @"Temp\", fileName));
		}

		/// <summary>
		/// 得到图片最终的存放目录。当且仅当使用文件系统存放图片时才有效
		/// </summary>
		/// <param name="fileName"></param>
		/// <returns></returns>
		public System.IO.FileInfo GetPhysicalDestinationImagePath(string fileName)
		{
			fileName.NullCheck("fileName");

			string path = GetUploadRootPath("ImageUploadRootPath");

			return new System.IO.FileInfo(Path.Combine(path, fileName));
		}

		public void UpdateContent(ImageProperty image)
		{
			image.NullCheck("image");

			IMaterialContentPersistManager manager = MaterialContentSettings.GetConfig().PersistManager;

			image.EnsureMaterialContent();

			using (TransactionScope scope = TransactionScopeFactory.Create())
			{
				if (image.Content.IsEmpty())
				{
					manager.DeleteMaterialContent(image.Content);
				}
				else
				{
					manager.DestFileInfo = GetPhysicalDestinationImagePath(image.Content.FileName);
					manager.SourceFileInfo = image.Content.PhysicalSourceFilePath;

					manager.SaveMaterialContent(image.Content);
                    image.FilePath = string.Empty;
                    image.Changed = false;
				}

				scope.Complete();
			}
		}

		protected override string GetConnectionName()
		{
			return WorkflowSettings.GetConfig().ConnectionName;
		}
	}
}
