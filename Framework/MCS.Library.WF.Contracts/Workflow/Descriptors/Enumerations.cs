using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MCS.Library.WF.Contracts.Workflow.Descriptors
{
    /// <summary>
    /// 流程活动的类型
    /// </summary>
    public enum WfClientActivityType
    {
        /// <summary>
        /// 一般活动
        /// </summary>
        NormalActivity = 0,

        /// <summary>
        /// 起始活动
        /// </summary>
        InitialActivity = 1,

        /// <summary>
        /// 结束活动
        /// </summary>
        CompletedActivity = 4
    }

    /// <summary>
    /// 分支流程的执行方式
    /// </summary>
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
    /// 主流程和分支流程的等待关系
    /// </summary>
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

    /// <summary>
    /// 主流程和分支流程组之间的关系
    /// </summary>
    public enum WfClientBranchGroupBlockingType
    {
        /// <summary>
        /// 等待全部分支流程组完成
        /// </summary>
        WaitAllBranchGroupsComplete = 0,

        /// <summary>
        /// 等待任意一个分支流程完成
        /// </summary>
        WaitAnyoneBranchGroupComplete = 2
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

    /// <summary>
    /// 分支流程的返回结果类型
    /// </summary>
    public enum WfClientBranchProcessReturnType
    {
        /// <summary>
        /// 全部反对
        /// </summary>
        AllFalse = 0,

        /// <summary>
        /// 全部同意
        /// </summary>
        AllTrue = 1,

        /// <summary>
        /// 部分同意
        /// </summary>
        PartialTrue = 2
    }

    /// <summary>
    /// 流程活动在导航上显示模式
    /// </summary>
    public enum WfClientNavigatorDisplayMode
    {
        /// <summary>
        /// 取决于流程，如果是审批流，则显示人名，否则是环节名称
        /// </summary>
        DependsOnProcess = 0,

        /// <summary>
        /// 显示活动名称
        /// </summary>
        ShowActivityName = 1,

        /// <summary>
        /// 显示候选人
        /// </summary>
        ShowCandidates = 2
    }

    /// <summary>
    /// 子流程ResourceID的生成模式
    /// </summary>
    public enum WfClientSubProcessResourceMode
    {
        /// <summary>
        /// 取决于流程
        /// </summary>
        DependsOnProcess = 0,

        /// <summary>
        /// 与主流程相同
        /// </summary>
        SameWithRoot = 1,

        /// <summary>
        /// 重新创建
        /// </summary>
        NewCreate = 2
    }

    /// <summary>
    /// 审批流审批方式
    /// </summary>
    public enum WfClientSubProcessApprovalMode
    {
        /// <summary>
        /// 审批流中领导选不同意后结束
        /// </summary>
        NoActivityDecide = 0,

        /// <summary>
        /// 审批流中领导选不同意后结束
        /// </summary>
        AnyActivityDecide = 1,

        /// <summary>
        /// 审批流中领导选不同意后，流程继续，审批流最后一个人说了算
        /// </summary>
        LastActivityDecide = 2,
    }

    /// <summary>
    /// 流程收集参数时将收集的值赋给哪个流程实例
    /// </summary>
    [Flags]
    public enum WfClientProcessParameterEvalMode
    {
        CurrentProcess = 1,
        ApprovalRootProcess = 2,
        RootProcess = 4,
        SameResourceRootProcess = 8
    }

    /// <summary>
    /// 是否独立显示意见
    /// </summary>
    public enum WfClientOpinionMode
    {
        /// <summary>
        /// 默认
        /// </summary>
        Default = 0,

        /// <summary>
        /// 独立
        /// </summary>
        Independent = 1,

        /// <summary>
        /// 合并
        /// </summary>
        Merged = 2
    }

    /// <summary>
    /// 是否允许编辑流程活动
    /// </summary>
    public enum WfClientSubProcessActivityEditMode
    {
        /// <summary>
        /// 默认
        /// </summary>
        Default = 0,

        /// <summary>
        /// 允许编辑活动点
        /// </summary>
        AllowEdit = 1,

        /// <summary>
        /// 禁止编辑活动点
        /// </summary>
        DisallowEdit = 2
    }

    /// <summary>
    /// 搜索ID生成方式
    /// </summary>
    public enum WfClientSearchIDMode
    {
        /// <summary>
        /// 与表单ID相同
        /// </summary>
        SameAsResourceID = 0,

        /// <summary>
        /// 与流程ID相同
        /// </summary>
        SameAsProcessID = 1
    }

    /// <summary>
    /// 自动发送待办的模式，与AutoSendUserTask配合使用
    /// </summary>
    public enum WfClientAutoSendUserTaskMode
    {
        /// <summary>
        /// 默认形式。取决于流程是否从数据库加载，或者是子流程
        /// </summary>
        ByDefault = 0,

        ByAutoSendUserTaskProperty = 1
    }

    /// <summary>
    /// 加签模式
    /// </summary>
    [Flags]
    public enum WfClientAddApproverMode
    {
        /// <summary>
        /// 仅添加审批人
        /// </summary>
        OnlyAddApprover = 0,

        /// <summary>
        /// 将当前环节添加到最后
        /// </summary>
        AppendCurrentActivity = 1,

        /// <summary>
        /// 添加的活动是否算关联活动
        /// </summary>
        AreAssociatedActivities = 2,

        /// <summary>
        /// 标准模式，关联活动，且添加当前活动到最后
        /// </summary>
        StandardMode = 3,
    }
}
