using System;
using MCS.Library.Core;
using MCS.Library.Data;
using MCS.Library.SOA.DataObjects.Workflow;

namespace MCS.Library.SOA.DataObjects
{
	/// <summary>
	/// 实现同步附件到文档中心
	/// </summary>
	public class SyncMaterialFileToDocCenterOperation : IMaterialFileOperation
	{
		public static string ShowFileUrlKey = "showUrl";

		private readonly string taskTitleTemplate = "同步附件到文档中心：{0};ID：{1}";
		private readonly string resourceUri = "syncMaterialService";

		public virtual void DoModifyFileOperations(string rootPath, MaterialFileOeprationInfo fileOperation, MaterialContent content)
		{
			var task = new InvokeServiceTask()
			{
				TaskID = UuidHelper.NewUuidString(),
				TaskTitle = string.Format(this.taskTitleTemplate, fileOperation.Material.OriginalName, fileOperation.Material.ID),
			};

			var parameters = new WfServiceOperationParameterCollection
			{
				new WfServiceOperationParameter("rootPath", rootPath),
				new WfServiceOperationParameter("fileOperation", fileOperation.Operation.ToString()),
				new WfServiceOperationParameter("materialConnectionName",
					DbConnectionMappingContext.GetMappedConnectionName(MaterialAdapter.Instance.GetConnectionName())),
				new WfServiceOperationParameter("contentConnectionName",
					DbConnectionMappingContext.GetMappedConnectionName(MaterialContentAdapter.Instance.ConnectionName)),
				new WfServiceOperationParameter("materialId", fileOperation.Material.ID),
				new WfServiceOperationParameter("url", fileOperation.Material.ShowFileUrl)
			};


			task.SvcOperationDefs.Add(new WfServiceOperationDefinition(
					new WfServiceAddressDefinition(WfServiceRequestMethod.Post, null, ResourceUriSettings.GetConfig().Paths[this.resourceUri].Uri.ToString()),
						"DoFileToDocService", parameters, "ReturnValue")
					);

			DbConnectionMappingContext.DoMappingAction(
				DbConnectionMappingContext.GetMappedConnectionName(InvokeServiceTaskAdapter.Instance.ConnectionName),
				InvokeServiceTaskAdapter.Instance.ConnectionName,
				() =>
				{
					InvokeServiceTaskAdapter.Instance.Update(task);
				});
		}

		public virtual void DecorateMaterialListAfterLoad(MaterialList materials)
		{
			materials.ForEach(i =>
			{
				i.ShowFileUrl = i.ExtraDataDictionary.GetValue("showUrl", i.ShowFileUrl);
			});
		}
	}
}
