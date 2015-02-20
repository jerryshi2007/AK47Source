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
	/// Executor相关接口
	/// </summary>
    [ServiceContract(Name = "WfExecutorService", Namespace = "http://temp.org")]
	public interface IWfExecutorService
	{
		[OperationContract]
		WfClientProcess ExecuteStartupWorkflow(
			string operatorActivityID,
            WfClientProcessStartupParams startupParams);

		[OperationContract]
		void ExecuteMoveTo(
			string operatorActivityID,
			string originalActivityID,
			WfClientTransferParams transferParams);

		[OperationContract]
		void ExecuteWithdraw(
			string operatorActivityID,
			string originalActivityID);

		[OperationContract]
		void ExecuteConsign(
			string operatorActivityID,
			string originalActivityID,
			WfClientAssigneeCollection assignees,
            ClientOguUser[] consignUsers,
            ClientOguUser[] circulateUsers,
			WfClientBranchProcessBlockingType blockingType,
            WfClientBranchProcessExecuteSequence sequence);

		[OperationContract]
		void ExecuteCirculate(
			string operatorActivityID,
			string originalActivityID,
			ClientOguUser[] circulateUsers);

		[OperationContract]
		WfClientActivity ExecuteAddActivity(
			string operatorActivityID,
			string targetActivityID,
			WfClientActivityDescriptorCreateParams createParams);

		[OperationContract]
		WfClientActivity ExecuteEditActivity(string operatorActivityID,
			string targetActivityID,
			WfClientActivityDescriptorCreateParams createParams);

		[OperationContract]
		void ExecuteDeleteActivity(string operatorActivityID, string targetActivityID);

		[OperationContract]
		void ExecuteCancelProcess(string operatorActivityID, string cancelProcessID);

		[OperationContract]
		void ExecuteReturn(string operatorActivityID, string targetActivityID);
	}
}
