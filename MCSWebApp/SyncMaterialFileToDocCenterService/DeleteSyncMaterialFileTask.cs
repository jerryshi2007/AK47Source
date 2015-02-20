using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using MCS.Library.Core;
using MCS.Library.Data;
using MCS.Library.SOA.DataObjects;
using MCS.Library.SOA.DocServiceClient;

namespace SyncMaterialFileToDocCenterService
{
	public class DeleteSyncMaterialFileTask : ISyncMaterialFileTaskExecutor
	{
		public void Execute(string materialId, string materialConnectionName, string contentConnectionName, string rootPath, string url)
		{
			if (url.IsNullOrWhiteSpace() || url.StartsWith(@"temp\\", StringComparison.OrdinalIgnoreCase)) return;

			var config = SyncMaterialFileTaskConfigSetting.GetConfig().Servers[contentConnectionName];
			var client = DCSClient.Create(config.DocumentLibraryName, config.MossServerName);

			var uri = new Uri(url);
			var file = client.GetFile(uri.AbsolutePath);
			file.Client = client;
			file.Delete();
		}
	}
}