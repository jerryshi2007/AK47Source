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
	/// 流程运行时的接口
	/// </summary>
    [ServiceContract(Name = "WfProcessRuntimeService", Namespace = "MCS")]
	public interface IWfProcessRuntimeService
	{
		/// <summary>
		/// 根据流程的ID得到流程实例
		/// </summary>
		/// <param name="processID"></param>
		/// <returns></returns>
		[OperationContract]
		WfClientProcess GetProcessByID(string processgID);

		/// <summary>
		/// 根据流程的活动的ID得到流程实例
		/// </summary>
		/// <param name="activityID"></param>
		/// <returns></returns>
		[OperationContract]
		WfClientProcess GetProcessByActivityID(string activityID);

		/// <summary>
		/// 根据流程对应的资源ID得到所有的相关流程
		/// </summary>
		/// <param name="resourceID"></param>
		/// <returns></returns>
		[OperationContract]
		WfClientProcessCollection GetProcessesByResourceID(string resourceID);

		/// <summary>
		/// 更新指定的流程实例
		/// </summary>
		/// <param name="processes"></param>
		[OperationContract]
		void UpdateProcesses(WfClientProcessCollection processes);

		/// <summary>
		/// 启动一个新的流程
		/// </summary>
		/// <param name="startupParams">流程启动参数，包括需要启动的流程的Key和起始点的处理人</param>
		/// <returns>已经启动的流程</returns>
		[OperationContract]
        WfClientProcess StartupWorkflow(WfClientProcessStartupParams startupParams);

		/// <summary>
		/// 流转操作
		/// </summary>
		/// <param name="moveToParams">流转参数，包括需要流转到的活动和相应的处理人</param>
		[OperationContract]
        void MoveTo(WfClientTransferParams moveToParams);

		/// <summary>
		/// 作废流程
		/// </summary>
		/// <param name="processID">流程实例的ID</param>
		/// <param name="cancelAllBranches">是否作废所有子流程</param>
		[OperationContract]
		void CancelProcess(string processID, bool cancelAllBranches);

		/// <summary>
		/// 撤回一步流程
		/// </summary>
		/// <param name="processID">流程实例的ID</param>
        [OperationContract]
        void Withdraw(WfClientActivity destinationActivity, bool cancelAllBranchProcesses);

		/// <summary>
		/// 在某个活动后添加一个活动
		/// </summary>
		/// <param name="activityID">在此之后添加活动的活动实例</param>
		/// <param name="actDesp">新活动的描述</param>
		/// <returns>新添加的活动实例</returns>
		[OperationContract]
		WfClientActivity AppendActivity(string activityID, WfClientActivityDescriptor actDesp);

		/// <summary>
		/// 删除一个活动实例
		/// </summary>
		/// <param name="activityID">被删除的活动实例ID</param>
		/// <returns></returns>
		[OperationContract]
		void DeleteActivity(string activityID);

		/// <summary>
		/// 在活动上启动分支流程
		/// </summary>
		/// <param name="activityID">需要启动分支流程的活动ID</param>
		/// <param name="branchStartupParams">分支流程启动参数</param>
		[OperationContract]
		void StartupBranchProcesses(string activityID, WfClientBranchProcessTransferParams branchStartupParams);

		/// <summary>
		/// 在活动上取消分支流程
		/// </summary>
		/// <param name="activityID">需要作废分支流程的活动ID</param>
		/// <param name="recursive">是否需要递归作废子分支流程</param>
		[OperationContract]
		void CancelBranchProcesses(string activityID, bool recursive);

		/// <summary>
		/// 强制完成分支流程
		/// </summary>
		/// <param name="activityID">需要强制完成的分支流程的活动ID</param>
		/// <param name="recursive">是否需要递归强制完成子分支流程/param>
		[OperationContract]
		void CompleteBranchProcesses(string activityID, bool recursive);
	}
}
