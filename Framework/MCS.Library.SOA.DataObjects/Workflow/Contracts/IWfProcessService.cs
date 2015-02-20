using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
using System.Text;

namespace MCS.Library.SOA.DataObjects.Workflow.Contracts
{
	[ServiceContract]
	public interface IWfProcessService
	{
		#region
		/// <summary>
		/// 根据分支流程模板，将模板中的资源中的每一个人员进行分解，分发出不同的启动分支流程的任务。返回分发的任务数（不含处理挂起的活动）
		/// </summary>
		/// <param name="ownerActivityID">分支流程所挂接的活动</param>
		/// <param name="template">分支流程模板</param>
		/// <param name="autoAddExitMaintainingStatusTask">是否在最后补充一个退出维护模式的任务</param>
		/// <returns>分发的启动分支流程的任务个数</returns>
		[OperationContract]
		int DispatchStartBranchProcessTasks(string ownerActivityID, IWfBranchProcessTemplateDescriptor template, bool autoAddExitMaintainingStatusTask);

		/// <summary>
		/// 分发作废流程的任务
		/// </summary>
		/// <param name="ownerActivityID">分支流程所挂接的活动</param>
		/// <param name="ownerActivityID"></param>
		/// <param name="cancelSelf">是否包含作废自己</param>
		/// <param name="cancelAllBranchProcesses"></param>
		/// <returns>启动的活动的个数</returns>
		[OperationContract]
		int DispatchCancelBranchProcessesTasks(string ownerSysActivityID, string ownerActivityID, bool cancelAllBranchProcesses);

		/// <summary>
		/// 分发作废分支流程的任务
		/// </summary>
		/// <param name="ownerSysActivityID"></param>
		/// <param name="processID"></param>
		/// <param name="cancelAllBranchProcesses"></param>
		/// <returns></returns>
		[OperationContract]
		int DispatchCancelProcessTask(string ownerSysActivityID, string processID, bool cancelAllBranchProcesses);

		/// <summary>
		/// 分发撤回流程的任务
		/// </summary>
		/// <param name="ownerActivityID">分支流程所挂接的活动</param>
		/// <param name="processID"></param>
		/// <param name="cancelAllBranchProcesses"></param>
		/// <returns>启动的活动的个数</returns>
		[OperationContract]
		int DispatchWithdrawProcessTask(string ownerSysActivityID, string processID, bool cancelAllBranchProcesses);
		#endregion

		#region 流程操作
		/// <summary>
		/// 作废流程
		/// </summary>
		/// <param name="processID">需要作废的流程ID</param>
		/// <param name="cancelAllBranchProcesses">是否递归作废所有子流程</param>
		[OperationContract]
		void CancelProcess(string processID, bool cancelAllBranchProcesses);

		/// <summary>
		/// 撤回流程的步骤
		/// </summary>
		/// <param name="processID">需要撤回的流程ID</param>
		/// <param name="cancelAllBranchProcesses">是否递归作废所有子流程</param>
		[OperationContract]
		void WithdrawProcess(string processID, bool cancelAllBranchProcesses);

		/// <summary>
		/// 启动分支流程
		/// </summary>
		/// <param name="ownerActivityID">分支流程所挂接的活动</param>
		/// <param name="branchTransferParams">分支流程启动参数</param>
		/// <returns>已经启动的分支流程的实例ID</returns>
		[OperationContract]
		string[] StartBranchProcesses(string ownerActivityID, WfBranchProcessTransferParams branchTransferParams);

		/// <summary>
		/// 退出流程的维护模式
		/// </summary>
		/// <param name="processID">流程ID</param>
		/// <param name="processPendingActivity">是否处理挂起的活动</param>
		[OperationContract]
		void ExitMaintainingStatus(string processID, bool processPendingActivity);
		#endregion
	}
}
