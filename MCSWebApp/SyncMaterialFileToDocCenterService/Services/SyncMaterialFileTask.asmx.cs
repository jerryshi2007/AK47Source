using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Services;

namespace SyncMaterialFileToDocCenterService.Services
{
	[WebService(Namespace = "http://tempuri.org/")]
	[WebServiceBinding(ConformsTo = WsiProfiles.BasicProfile1_1)]
	[System.ComponentModel.ToolboxItem(false)]
	public class SyncMaterialFileTask1 : System.Web.Services.WebService
	{

		[WebMethod]
		public void DoFileToDocService(string materialId, string materialConnectionName, string contentConnectionName, string rootPath, string fileOperation, string url)
		{
			SyncMaterialFileTaskConfigSetting.GetConfig().GetExecutor(fileOperation).Execute(materialId, materialConnectionName, contentConnectionName, rootPath, url);
		}
	}
}
