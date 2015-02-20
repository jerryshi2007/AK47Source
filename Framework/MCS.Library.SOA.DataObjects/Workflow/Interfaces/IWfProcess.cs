using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections.Specialized;
using MCS.Library.OGUPermission;

namespace MCS.Library.SOA.DataObjects.Workflow
{
	/// <summary>
	/// 流程的接口定义
	/// </summary>
	public interface IWfProcess
	{
		/// <summary>
		/// 流程的ID
		/// </summary>
		string ID
		{
			get;
		}

		/// <summary>
		/// 检索ID
		/// </summary>
		string SearchID
		{
			get;
		}


		string RelativeID
		{
			get;
		}

		string RelativeURL
		{
			get;
		}

		/// <summary>
		/// 
		/// </summary>
		string ResourceID
		{
			get;
		}

		/// <summary>
		/// 主线流程信息。创建时，原始的流程活动
		/// </summary>
		IWfProcessDescriptor MainStream
		{
			get;
		}

		/// <summary>
		/// 流程的描述信息
		/// </summary>
		IWfProcessDescriptor Descriptor
		{
			get;
			set;
		}

		/// <summary>
		/// 流程的状态
		/// </summary>
		WfProcessStatus Status
		{
			get;
		}

		/// <summary>
		/// 流程启动时间
		/// </summary>
		DateTime StartTime
		{
			get;
		}

		/// <summary>
		/// 流程结束信息
		/// </summary>
		DateTime EndTime
		{
			get;
		}

		/// <summary>
		/// 操作人
		/// </summary>
		IUser Creator
		{
			get;
			set;
		}

		/// <summary>
		/// 修改的标记
		/// </summary>
		int UpdateTag
		{
			get;
		}

		/// <summary>
		/// 流程是否是提交的。如果为False，则表示是用户打开表单，启动了流程，但是没有保存和流转
		/// </summary>
		bool Committed
		{
			get;
			set;
		}

		/// <summary>
		/// 流程的部门
		/// </summary>
		IOrganization OwnerDepartment
		{
			get;
			set;
		}

		/// <summary>
		/// 流程上下文
		/// </summary>
		WfProcessContext Context
		{
			get;
		}

		/// <summary>
		/// 流程的加载方式
		/// </summary>
		DataLoadingType LoadingType
		{
			get;
		}

		/// <summary>
		/// 活动的实例集合
		/// </summary>
		WfActivityCollection Activities
		{
			get;
		}

		/// <summary>
		/// 顺序返回流程中已执行过的一组活动节点集合
		/// </summary>
		WfActivityCollection ElapsedActivities
		{
			get;
		}

		IWfActivity InitialActivity
		{
			get;
		}

		IWfActivity CurrentActivity
		{
			get;
		}

		IWfActivity CompletedActivity
		{
			get;
		}

		/// <summary>
		/// 运行时应用在流程中的参数
		/// </summary>
		WfApplicationRuntimeParameters ApplicationRuntimeParameters
		{
			get;
		}

		/// <summary>
		/// 如果是分支流程，则EntryInfo是父流程的信息，否则是Null
		/// </summary>
		IWfBranchProcessGroup EntryInfo
		{
			get;
		}

		/// <summary>
		/// 根流程。如果没有父流程，则RootProcess等于此流程对象
		/// </summary>
		IWfProcess RootProcess
		{
			get;
		}

		/// <summary>
		/// 同一资源的根流程（相同的ResourceID）
		/// </summary>
		IWfProcess SameResourceRootProcess
		{
			get;
		}

		/// <summary>
		/// 当前流程是否是审批流的根流程
		/// </summary>
		bool IsApprovalRootProcess
		{
			get;
		}

		/// <summary>
		/// 如果当前流程是审批流程，则返回审批流程中的根流程，否则返回自己
		/// </summary>
		IWfProcess ApprovalRootProcess
		{
			get;
		}

		/// <summary>
		/// 计划（业务）流程的根流程。如果当前流程是计划（业务）流程，则立即返回。如果都不是，则返回根流程
		/// </summary>
		IWfProcess ScheduleRootProcess
		{
			get;
		}

		/// <summary>
		/// 如果当前活动处于挂起状态，那么进行处理
		/// </summary>
		void ProcessPendingActivity();

		/// <summary>
		/// 退出维护模式
		/// </summary>
		/// <param name="processPendingActivity">是否随之执行ProcessPendingActivity</param>
		void ExitMaintainingStatus(bool processPendingActivity);

		/// <summary>
		/// 流转
		/// </summary>
		/// <param name="transferParams"></param>
		/// <returns></returns>
		IWfActivity MoveTo(WfTransferParams transferParams);

		/// <summary>
		/// 流转到下一个默认活动
		/// </summary>
		/// <returns></returns>
		IWfActivity MoveToNextDefaultActivity();

		/// <summary>
		/// 取消（中止）流程
		/// </summary>
		/// <param name="cancelAllBranchProcesses">中止流程时，是否中止所有的分支流程</param>
		void CancelProcess(bool cancelAllBranchProcesses);

		/// <summary>
		/// 恢复已经取消（中止）的流程
		/// </summary>
		void RestoreProcess();

		/// <summary>
		/// 暂停流程
		/// </summary>
		/// <param name="pauseAllBranchProcesses">暂停流程时，暂停中止所有的分支流程</param>
		void PauseProcess(bool pauseAllBranchProcesses);

		/// <summary>
		/// 恢复暂停的流程
		/// </summary>
		/// <param name="resumeAllBranchProcesses"></param>
		void ResumeProcess(bool resumeAllBranchProcesses);

		/// <summary>
		/// 使流程直接结束
		/// </summary>
		/// <param name="completeAllBranchProcesses">是否直接结束所有子流程</param>
		void CompleteProcess(bool completeAllBranchProcesses);

		/// <summary>
		/// 撤回到指定的节点
		/// </summary>
		/// <param name="destinationActivity"></param>
		/// <param name="cancelAllBranchProcesses">是否需要取消子流程</param>
		void Withdraw(IWfActivity destinationActivity, bool cancelAllBranchProcesses);

		/// <summary>
		/// 进入到维护模式
		/// </summary>
		void EntrtMaintainingStatus();

		/// <summary>
		/// 分支流程的启动序号
		/// </summary>
		int Sequence
		{
			get;
		}

		/// <summary>
		/// 分支流程的启动参数
		/// </summary>
		WfBranchProcessStartupParams BranchStartupParams
		{
			get;
		}

		/// <summary>
		/// 取消流程的相关动作
		/// </summary>
		WfActionCollection CancelProcessActions
		{
			get;
		}

		/// <summary>
		/// 撤回流程的相关动作
		/// </summary>
		WfActionCollection WithdrawActions
		{
			get;
		}

		/// <summary>
		/// 流程状态变化的相关动作
		/// </summary>
		WfActionCollection ProcessStatusChangeActions
		{
			get;
		}

		/// <summary>
		/// 流程逻辑上是否可以撤回。包括流程的状态判断以及活动点的状态判断（不含业务逻辑和管理逻辑）
		/// </summary>
		bool CanWithdraw
		{
			get;
		}

		/// <summary>
		/// 是否有父流程
		/// </summary>
		bool HasParentProcess
		{
			get;
		}

		/// <summary>
		/// 流程相关的参数，通常是流程启动时指定或表单项。会保存到数据库中。
		/// </summary>
		NameValueCollection RelativeParams
		{
			get;
		}

		/// <summary>
		/// 意见控件显示流程
		/// </summary>
		IWfProcess OpinionRootProcess
		{
			get;
		}

		/// <summary>
		/// 得到权限矩阵
		/// </summary>
		/// <returns></returns>
		WfMatrix GetMatrix();

		/// <summary>
		/// 从当前资源中生成未流转的Activity的Candidates
		/// </summary>
		void GenerateCandidatesFromResources();

		/// <summary>
		/// 得到主线的活动点
		/// </summary>
		/// <param name="includeAllElapsedActivities">是否包含所有经过的活动</param>
		WfMainStreamActivityDescriptorCollection GetMainStreamActivities(bool includeAllElapsedActivities);
	}
}
