using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ServiceModel;
using MCS.Library.SOA.DataObjects.Workflow;

namespace WfFormTemplate.Services
{
	/// <summary>
	/// 流程操作的服务接口
	/// </summary>
	[ServiceContract]
	public interface IWfProcessService
	{
		[OperationContract]
		DateTime GetServerTime();

		[OperationContract]
		string[] StartBranchProcesses(string ownerActivityID, WfBranchProcessTransferParams branchTransferParams);
	}
}
