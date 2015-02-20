using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SyncMaterialFileToDocCenterService
{
	public interface ISyncMaterialFileTaskExecutor
	{
		void Execute(string materialId, string materialConnectionName, string contentConnectionName, string rootPath, string url);
	}
}