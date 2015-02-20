using System.ServiceModel;
using System.ServiceModel.Web;

namespace SyncMaterialFileToDocCenterService
{
	[ServiceContract]
	public interface ISyncMaterialFileTask
	{
		[OperationContract]
		[WebInvoke(Method = "POST", RequestFormat = WebMessageFormat.Json, ResponseFormat = WebMessageFormat.Json, BodyStyle = WebMessageBodyStyle.WrappedRequest)]
		void DoFileToDocService(string materialId, string materialConnectionName, string contentConnectionName, string rootPath, string fileOperation, string url);
	}
}
