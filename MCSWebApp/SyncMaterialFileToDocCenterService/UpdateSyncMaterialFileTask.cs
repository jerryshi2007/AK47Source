using System;
using System.IO;
using System.Linq;
using MCS.Library.Core;
using MCS.Library.Data;
using MCS.Library.SOA.DataObjects;
using MCS.Library.SOA.DocServiceClient;

namespace SyncMaterialFileToDocCenterService
{
	public class UpdateSyncMaterialFileTask : ISyncMaterialFileTaskExecutor
	{
		public void Execute(string materialId, string materialConnectionName, string contentConnectionName, string rootPath, string url)
		{
			var materialContent = MaterialHelper.LoadMaterialContent(materialId, contentConnectionName);
			materialContent.NullCheck<FileNotFoundException>("无法找到此文件：{0}", materialId);

			var config = SyncMaterialFileTaskConfigSetting.GetConfig().Servers[contentConnectionName];
			var client = DCSClient.Create(config.DocumentLibraryName, config.MossServerName);

			var file = Save(
				client,
				materialContent.ContentData,
				GetPath(rootPath, materialContent.CreateTime),
				GetNewFileName(materialId, materialContent.FileName)
				);

			UpdateInfo(materialId, file.AbsoluteUri, materialConnectionName, rootPath);
		}

		private string GetPath(string rootPath, DateTime createTime)
		{
			return rootPath + "\\" + createTime.ToString("yyyyMM");
		}

		private string GetNewFileName(string materialId, string oldName)
		{
			return string.Format("{0}[{1}]{2}", Path.GetFileNameWithoutExtension(oldName), materialId, Path.GetExtension(oldName));
		}

		private DCTClientFile Save(DCSClient client, byte[] content, string path, string fileName)
		{
			var folder = client.RootFolder;
			var paths = path.Split('\\');

			foreach (var folderName in paths)
			{
				if (folderName.IsNullOrWhiteSpace()) continue;
				folder = folder.GetFolder(folderName) ?? folder.CreateFolder(folderName);
				folder.Client = client;
			}

			return folder.Save(content, fileName, true);
		}

		private void UpdateInfo(string materialId, string url, string connectionName, string rootPath)
		{
			materialId.CheckStringIsNullOrEmpty("materialId");
			url.CheckStringIsNullOrEmpty("url");
			connectionName.CheckStringIsNullOrEmpty("connectionName");

			var material = MaterialHelper.LoadMaterial(materialId, connectionName);
			var extraData = material.ExtraDataDictionary;

			if (extraData.ContainsKey(SyncMaterialFileToDocCenterOperation.ShowFileUrlKey))
			{
				extraData[SyncMaterialFileToDocCenterOperation.ShowFileUrlKey] = url;
			}
			else
			{
				extraData.Add(SyncMaterialFileToDocCenterOperation.ShowFileUrlKey, url);
			}

			var deltaMaterialList = new DeltaMaterialList();
			deltaMaterialList.RootPathName = rootPath;
			deltaMaterialList.Updated.Add(material);

			DbConnectionMappingContext.DoMappingAction(
				MaterialAdapter.Instance.GetConnectionName(),
				connectionName,
				() =>
				{
					MaterialAdapter.Instance.SaveDeltaMaterials(deltaMaterialList, false);
				});
		}
	}
}