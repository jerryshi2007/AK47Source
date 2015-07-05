using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MCS.Library.Core;

namespace MCS.Library.SOA.DataObjects
{
    public enum WfPrivilegeMode
    {
        Normal = 0,
        Admin = 1,
    }

    /// <summary>
    /// 仿真操作的类型
    /// </summary>
    public enum WfSimulationOperationType
    {
        [EnumItemDescription("流转")]
        MoveTo = 1,

        /// <summary>
        /// 启动流程
        /// </summary>
        [EnumItemDescription("启动流程")]
        Startup = 2048
    }

    /// <summary>
    /// 工作流控件的辅助操作类型
    /// </summary>
    public enum WfControlSubOperationType
    {
        /// <summary>
        /// 未指定
        /// </summary>
        [EnumItemDescription("未指定")]
        None = 0,

        /// <summary>
        /// 进入活动
        /// </summary>
        [EnumItemDescription("进入")]
        Enter = 1,

        /// <summary>
        /// 离开活动
        /// </summary>
        [EnumItemDescription("离开")]
        Leave = 2
    }

    /// <summary>
    /// 工作流控件的操作类型
    /// </summary>
    public enum WfControlOperationType
    {
        /// <summary>
        /// 未指定
        /// </summary>
        [EnumItemDescription("未指定")]
        None = 0,

        /// <summary>
        /// 流转
        /// </summary>
        [EnumItemDescription("流转")]
        MoveTo = 1,

        /// <summary>
        /// 保存
        /// </summary>
        [EnumItemDescription("保存")]
        Save = 2,

        /// <summary>
        /// 撤回
        /// </summary>
        [EnumItemDescription("撤回")]
        Withdraw = 4,

        /// <summary>
        /// 作废
        /// </summary>
        [EnumItemDescription("作废")]
        CancelProcess = 8,

        /// <summary>
        /// 调整流程
        /// </summary>
        [EnumItemDescription("加签")]
        AdjustProcess = 16,

        /// <summary>
        /// 强制结束
        /// </summary>
        [EnumItemDescription("强制结束")]
        ObligeEnd = 32,

        /// <summary>
        /// 转签
        /// </summary>
        [EnumItemDescription("转签")]
        ChangeApprover = 64,

        /// <summary>
        /// 退件
        /// </summary>
        [EnumItemDescription("退件")]
        Return = 128,

        /// <summary>
        /// 增加活动
        /// </summary>
        [EnumItemDescription("增加活动")]
        AddActivity = 256,

        /// <summary>
        /// 修改活动
        /// </summary>
        [EnumItemDescription("修改活动")]
        EditActivity = 512,

        /// <summary>
        /// 删除活动
        /// </summary>
        [EnumItemDescription("删除活动")]
        DeleteActivity = 1024,

        /// <summary>
        /// 启动流程
        /// </summary>
        [EnumItemDescription("启动流程")]
        Startup = 2048,

        /// <summary>
        /// 会签
        /// </summary>
        [EnumItemDescription("会签")]
        Consign = 4096,

        /// <summary>
        /// 加签
        /// </summary>
        [EnumItemDescription("加签")]
        AddApprover = 8192,

        /// <summary>
        /// 传阅
        /// </summary>
        [EnumItemDescription("传阅")]
        Circulate = 16384,

        [EnumItemDescription("待办转出")]
        ReplaceAssignees = 32768,

        [EnumItemDescription("暂停")]
        PauseProcess = 65536,

        [EnumItemDescription("恢复")]
        ResumeProcess = 2 ^ 17,

        /// <summary>
        /// 还原作废的流程
        /// </summary>
        [EnumItemDescription("还原")]
        RestoreProcess = 2 ^ 18,

        /// <summary>
        /// 编辑流程属性
        /// </summary>
        [EnumItemDescription("编辑流程属性")]
        EditProcessProperties = 2 ^ 19,

        /// <summary>
        /// 编辑活动属性
        /// </summary>
        [EnumItemDescription("编辑活动属性")]
        EditActivityProperties = 2 ^ 20,

        /// <summary>
        /// 编辑线属性
        /// </summary>
        [EnumItemDescription("编辑线属性")]
        EditTransitionProperties = 2 ^ 21,

        [EnumItemDescription("管理增加连线")]
        AdminAddTransition = 2 ^ 22,

        [EnumItemDescription("管理增加活动")]
        AdminAddActivity = 2 ^ 23,

        [EnumItemDescription("管理删除活动")]
        AdminDeleteActivity = 2 ^ 24,

        [EnumItemDescription("管理删除连线")]
        AdminDeleteTransition = 2 ^ 25,

        [EnumItemDescription("启动分支流程")]
        StartBranchProcess = 2 ^ 26,

        [EnumItemDescription("退出维护模式")]
        ExitMaintainingStatus = 2 ^ 27,

        [EnumItemDescription("异步撤回")]
        AsyncWithdraw = 2 ^ 28,

        [EnumItemDescription("异步作废")]
        AsyncCancelProcess = 2 ^ 29
    }
}
