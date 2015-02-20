using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Serialization;
namespace MCS.Library.SOA.Contracts.DataObjects
{
	/// <summary>
	/// 流程的状态
	/// </summary>   
	public enum WfClientProcessStatus
	{
		/// <summary>
		/// 运行中
		/// </summary>
		[DataMember]
        Running,

		/// <summary>
		/// 已完成
		/// </summary>
       
        Completed,

		/// <summary>
		/// 被终止
		/// </summary>
       
        Aborted,

		/// <summary>
		/// 未运行
		/// </summary>
       
        NotRunning
	}

	/// <summary>
	/// 节点的状态
	/// </summary>   
	public enum WfClientActivityStatus
	{
		/// <summary>
		/// 未运行
		/// </summary>
       
        NotRunning,

		/// <summary>
		/// 运行中
		/// </summary>
       
        Running,

		/// <summary>
		/// 等待中
		/// </summary>
       
        Pending,

		/// <summary>
		/// 已完成
		/// </summary>
       
        Completed,

		/// <summary>
		/// 被终止
		/// </summary>
       
        Aborted,
	}

	/// <summary>
	/// 流程活动的类型
	/// </summary>
	public enum WfClientActivityType
	{
		/// <summary>
		/// 正常活动
		/// </summary>
       
        NormalActivity = 0,

		/// <summary>
		/// 开始活动
		/// </summary>
       
        InitialActivity = 1,

		/// <summary>
		/// 结束活动
		/// </summary>
       
        CompletedActivity = 4
	}

	public enum WfClientBranchProcessBlockingType
	{
		/// <summary>
		/// 等待全部分支流程完成
		/// </summary>
       
        WaitAllBranchProcessesComplete,

		/// <summary>
		/// 任何分支流程都不等待
		/// </summary>
       
        WaitNoneOfBranchProcessComplete,

		/// <summary>
		/// 等待任意一个分支流程完成
		/// </summary>
       
        WaitAnyoneBranchProcessComplete,

		/// <summary>
		/// 等待特定的某些分支流程完成
		/// </summary>
       
        WaitSpecificBranchProcessesComplete
	}

    public enum WfClientBranchProcessExecuteSequence
	{
		/// <summary>
		/// 并行
		/// </summary>
       
       Parallel,

		/// <summary>
		/// 串行
		/// </summary>
       
       Serial,

		/// <summary>
		/// 同一流程内串行
		/// </summary>
       
       
       SerialInSameProcess
	}

	/// <summary>
	/// 流程的类型
	/// </summary>
	public enum WfClientProcessType
	{
		/// <summary>
		/// 审批流程
		/// </summary>
       
        Approval = 0,

		/// <summary>
		/// 计划流程
		/// </summary>
       
        Schedule = 1,
	}

    public enum ClientDataType
    {
       
        String,
       
        Int,
       
        Double,
       
        Float,
       
        Boolean,
       
        DateTime
    }

    public enum WfClientAssigneeType
    {
        Normal,
        Delegated
    }

    /// <summary>
    /// 对象的加载方式
    /// </summary>
    public enum ClientDataLoadingType
    {
        /// <summary>
        /// 对象的内容从内存中加载（new）
        /// </summary>
        Memory,

        /// <summary>
        /// 对象的内容从外部加载（数据库）
        /// </summary>
        External
    }
}
