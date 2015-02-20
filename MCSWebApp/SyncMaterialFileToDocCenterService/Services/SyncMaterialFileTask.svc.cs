using System.ServiceModel.Web;

namespace SyncMaterialFileToDocCenterService.Services
{
	[System.ComponentModel.ToolboxItem(false)]
	public class SyncMaterialFileTask : ISyncMaterialFileTask
	{
		[WebInvoke(Method = "POST", RequestFormat = WebMessageFormat.Json, ResponseFormat = WebMessageFormat.Json, BodyStyle = WebMessageBodyStyle.WrappedRequest)]
		public void DoFileToDocService(string materialId, string materialConnectionName, string contentConnectionName, string rootPath, string fileOperation, string url)
		{
			SyncMaterialFileTaskConfigSetting.GetConfig().GetExecutor(fileOperation).Execute(materialId, materialConnectionName, contentConnectionName, rootPath, url);
		}
	}
}
