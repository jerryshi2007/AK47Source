using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MCS.Library.Core;

namespace MCS.Library.SOA.DataObjects.Workflow
{
	/// <summary>
	/// 流程的状态
	/// </summary>
	public enum WfProcessStatus
	{
		/// <summary>
		/// 运行中
		/// </summary>
		[EnumItemDescription("运行中", WfHelper.CultureCategory)]
		Running,

		/// <summary>
		/// 已完成
		/// </summary>
		[EnumItemDescription("已完成", WfHelper.CultureCategory)]
		Completed,

		/// <summary>
		/// 被终止
		/// </summary>
		[EnumItemDescription("被终止", WfHelper.CultureCategory)]
		Aborted,

		/// <summary>
		/// 未运行
		/// </summary>
		[EnumItemDescription("未运行", WfHelper.CultureCategory)]
		NotRunning,

		/// <summary>
		/// 已暂停
		/// </summary>
		[EnumItemDescription("已暂停", WfHelper.CultureCategory)]
		Paused,

		/// <summary>
		/// 维护中
		/// </summary>
		[EnumItemDescription("维护中", WfHelper.CultureCategory)]
		Maintaining
	}

	/// <summary>
	/// 节点的状态
	/// </summary>
	public enum WfActivityStatus
	{
		[EnumItemDescription("未运行", WfHelper.CultureCategory)]
		NotRunning,

		/// <summary>
		/// 运行中
		/// </summary>
		[EnumItemDescription("运行中", WfHelper.CultureCategory)]
		Running,

		[EnumItemDescription("等待中", WfHelper.CultureCategory)]
		Pending,

		/// <summary>
		/// 已完成
		/// </summary>
		[EnumItemDescription("已完成", WfHelper.CultureCategory)]
		Completed,

		/// <summary>
		/// 被终止
		/// </summary>
		[EnumItemDescription("被终止", WfHelper.CultureCategory)]
		Aborted,
	}

	/// <summary>
	/// 对象的加载方式
	/// </summary>
	public enum DataLoadingType
	{
		/// <summary>
		/// 对象的内容从内存中加载（new）
		/// </summary>
		Memory,

		/// <summary>
		/// 对象的内容从外部加载（数据库）
		/// </summary>
		External,

		/// <summary>
		/// 从别的流程复制过来，也在内存中
		/// </summary>
		Cloned
	}

	/// <summary>
	/// 流程活动的类型
	/// </summary>
	public enum WfActivityType
	{
		NormalActivity = 0,
		InitialActivity = 1,
		CompletedActivity = 4
	}

	/// <summary>
	/// 分支流程的执行方式
	/// </summary>
	public enum WfBranchProcessExecuteSequence
	{
		[EnumItemDescription("并行")]
		Parallel,

		[EnumItemDescription("串行")]
		Serial,

		[EnumItemDescription("同一流程内串行")]
		SerialInSameProcess
	}

	/// <summary>
	/// 主流程和分支流程的等待关系
	/// </summary>
	public enum WfBranchProcessBlockingType
	{
		/// <summary>
		/// 等待全部分支流程完成
		/// </summary>
		[EnumItemDescription("等待全部分支流程完成")]
		WaitAllBranchProcessesComplete,

		/// <summary>
		/// 任何分支流程都不等待
		/// </summary>
		[EnumItemDescription("任何分支流程都不等待")]
		WaitNoneOfBranchProcessComplete,

		/// <summary>
		/// 等待任意一个分支流程完成
		/// </summary>
		[EnumItemDescription("等待任意一个分支流程完成")]
		WaitAnyoneBranchProcessComplete,

		/// <summary>
		/// 等待特定的某些分支流程完成
		/// </summary>
		[EnumItemDescription("等待特定的某些分支流程完成")]
		WaitSpecificBranchProcessesComplete
	}

	/// <summary>
	/// 主流程和分支流程组之间的关系
	/// </summary>
	public enum WfBranchGroupBlockingType
	{
		[EnumItemDescription("等待全部分支流程组完成")]
		WaitAllBranchGroupsComplete = 0,

		/// <summary>
		/// 等待任意一个分支流程完成
		/// </summary>
		[EnumItemDescription("等待任意一个分支流程组完成")]
		WaitAnyoneBranchGroupComplete = 2
	}

	/// <summary>
	/// 流程的类型
	/// </summary>
	public enum WfProcessType
	{
		[EnumItemDescription("审批流程", "Approval", 0)]
		Approval = 0,

		[EnumItemDescription("计划流程", "Schedule", 1)]
		Schedule = 1,
	}

	/// <summary>
	/// 分支流程的返回结果类型
	/// </summary>
	public enum BranchProcessReturnType
	{
		[EnumItemDescription("全部反对")]
		AllFalse = 0,

		[EnumItemDescription("全部同意")]
		AllTrue = 1,

		[EnumItemDescription("部分同意")]
		PartialTrue = 2
	}

	/// <summary>
	/// 流程活动在导航上显示模式
	/// </summary>
	public enum WfNavigatorDisplayMode
	{
		/// <summary>
		/// 取决于流程，如果是审批流，则显示人名，否则是环节名称
		/// </summary>
		[EnumItemDescription("取决于流程", "DependsOnProcess", 0)]
		DependsOnProcess = 0,

		/// <summary>
		/// 显示活动名称
		/// </summary>
		[EnumItemDescription("显示活动名称", "ShowActivityName", 1)]
		ShowActivityName = 1,

		/// <summary>
		/// 显示候选人
		/// </summary>
		[EnumItemDescription("显示候选人", "ShowCandidates", 2)]
		ShowCandidates = 2
	}

	/// <summary>
	/// 子流程ResourceID的生成模式
	/// </summary>
	public enum WfSubProcessResourceMode
	{
		/// <summary>
		/// 取决于流程
		/// </summary>
		[EnumItemDescription("取决于流程", "DependsOnProcess", 0)]
		DependsOnProcess = 0,

		/// <summary>
		/// 与主流程相同
		/// </summary>
		[EnumItemDescription("与主流程相同", "SameWithRoot", 1)]
		SameWithRoot = 1,

		/// <summary>
		/// 重新创建
		/// </summary>
		[EnumItemDescription("重新创建", "NewCreate", 2)]
		NewCreate = 2
	}

	/// <summary>
	/// 审批流审批方式
	/// </summary>
	public enum WfSubProcessApprovalMode
	{
		/// <summary>
		/// 审批流中领导选不同意后结束
		/// </summary>
		[EnumItemDescription("任意一步都不决定流程结束", "NoActivityDecide", 0)]
		NoActivityDecide = 0,

		/// <summary>
		/// 审批流中领导选不同意后结束
		/// </summary>
		[EnumItemDescription("任意一步决定流程结束", "AnyActivityDecide", 1)]
		AnyActivityDecide = 1,

		/// <summary>
		/// 审批流中领导选不同意后，流程继续，审批流最后一个人说了算
		/// </summary>
		[EnumItemDescription("最后一步决定流程结束", "LastActivityDecide", 2)]
		LastActivityDecide = 2,
	}

	/// <summary>
	/// 流程收集参数时将收集的值赋给哪个流程实例
	/// </summary>
	[Flags]
	public enum ProcessParameterEvalMode
	{
		CurrentProcess = 1,
		ApprovalRootProcess = 2,
		RootProcess = 4,
		SameResourceRootProcess = 8
	}

	/// <summary>
	/// 是否独立显示意见
	/// </summary>
	public enum WfOpinionMode
	{
		[EnumItemDescription("默认", "Default", 0)]
		Default = 0,
		[EnumItemDescription("独立", "Independent", 1)]
		Independent = 1,
		[EnumItemDescription("合并", "Merged", 2)]
		Merged = 2
	}

	/// <summary>
	/// 是否允许编辑流程活动
	/// </summary>
	public enum WfSubProcessActivityEditMode
	{
		[EnumItemDescription("默认", "Default", 0)]
		Default = 0,
		[EnumItemDescription("允许编辑活动点", "AllowEdit", 1)]
		AllowEdit = 1,
		[EnumItemDescription("禁止编辑活动点", "DisallowEdit", 2)]
		DisallowEdit = 2
	}

	/// <summary>
	/// 搜索ID生成方式
	/// </summary>
	public enum WfSearchIDMode
	{
		/// <summary>
		/// 与表单ID相同
		/// </summary>
		[EnumItemDescription("与表单ID相同", "SameAsResourceID", 0)]
		SameAsResourceID = 0,

		/// <summary>
		/// 与流程ID相同
		/// </summary>
		[EnumItemDescription("与流程ID相同", "SameAsProcessID ", 1)]
		SameAsProcessID = 1
	}

	/// <summary>
	/// 自动发送待办的模式，与AutoSendUserTask配合使用
	/// </summary>
	public enum WfAutoSendUserTaskMode
	{
		/// <summary>
		/// 默认形式。取决于流程是否从数据库加载，或者是子流程
		/// </summary>
		[EnumItemDescription("默认形式", "ByDefault", 0)]
		ByDefault = 0,

		[EnumItemDescription("由AutoSendUserTask属性决定", "ByAutoSendUserTaskProperty", 1)]
		ByAutoSendUserTaskProperty = 1
	}

	/// <summary>
	/// 加签模式
	/// </summary>
	[Flags]
	public enum WfAddApproverMode
	{
		[EnumItemDescription("仅添加审批人")]
		OnlyAddApprover = 0,

		[EnumItemDescription("将当前环节添加到最后")]
		AppendCurrentActivity = 1,

		[EnumItemDescription("添加的活动是否算关联活动")]
		AreAssociatedActivities = 2,

		[EnumItemDescription("标准模式，关联活动，且添加当前活动到最后")]
		StandardMode = 3,
	}

	/// <summary>
	/// 获取流程上下文参数时，是否递归查询父流程
	/// </summary>
	public enum WfProbeApplicationRuntimeParameterMode
	{
		/// <summary>
		/// 根据流程属性中ProbeParentProcessParams属性值决定是否递归查询父流程
		/// </summary>
		Auto = 0,

		/// <summary>
		/// 不递归
		/// </summary>
		NotRecursively = 1,

		/// <summary>
		/// 递归
		/// </summary>
		Recursively = 2
	}
}
