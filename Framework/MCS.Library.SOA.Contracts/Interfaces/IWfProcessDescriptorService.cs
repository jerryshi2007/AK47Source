using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ServiceModel;
using MCS.Library.SOA.Contracts.DataObjects;
using MCS.Library.SOA.Contracts.DataObjects.Workflow;

namespace MCS.Library.SOA.Contracts
{
	/// <summary>
	/// 流程描述操作的接口
	/// </summary>
    [ServiceContract(Name = "WfProcessDescriptorService", Namespace = "MCS")]
    
	public interface IWfProcessDescriptorService
	{
		[OperationContract]
		WfClientProcessDescriptor LoadDescriptor(string processDespKey);

		[OperationContract]
		void SaveDescriptor(WfClientProcessDescriptor processDesp);

		[OperationContract]
        WfClientProcessDescriptor AppendActivityDescriptor(string processDespKey, string actDespKey, WfClientActivityDescriptor newActDesp);

		[OperationContract]
		void DeleteActivityDescriptor(string processDespKey, string actDespKey);
	}
}
