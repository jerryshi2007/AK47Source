using MCS.Library.Core;
using MCS.Library.WF.Contracts.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MCS.Library.WF.Contracts.Workflow.Runtime
{
    public enum WfClientAssigneeType
    {
        Normal,
        Delegated
    }

    /// <summary>
    /// 流程的状态
    /// </summary>
    public enum WfClientProcessStatus
    {
        /// <summary>
        /// 运行中
        /// </summary>
        [EnumItemDescription("运行中", Define.WorkflowCulture)]
        Running,

        /// <summary>
        /// 已完成
        /// </summary>
        [EnumItemDescription("已完成", Define.WorkflowCulture)]
        Completed,

        /// <summary>
        /// 被终止
        /// </summary>
        [EnumItemDescription("被终止", Define.WorkflowCulture)]
        Aborted,

        /// <summary>
        /// 未运行
        /// </summary>
        [EnumItemDescription("未运行", Define.WorkflowCulture)]
        NotRunning,

        /// <summary>
        /// 已暂停
        /// </summary>
        [EnumItemDescription("已暂停", Define.WorkflowCulture)]
        Paused,

        /// <summary>
        /// 维护中
        /// </summary>
        [EnumItemDescription("维护中", Define.WorkflowCulture)]
        Maintaining
    }

    /// <summary>
    /// 节点的状态
    /// </summary>
    public enum WfClientActivityStatus
    {
        /// <summary>
        /// 未运行
        /// </summary>
        [EnumItemDescription("未运行", Define.WorkflowCulture)]
        NotRunning,

        /// <summary>
        /// 运行中
        /// </summary>
        [EnumItemDescription("运行中", Define.WorkflowCulture)]
        Running,

        /// <summary>
        /// 等待中
        /// </summary>
        [EnumItemDescription("等待中", Define.WorkflowCulture)]
        Pending,

        /// <summary>
        /// 已完成
        /// </summary>
        [EnumItemDescription("已完成", Define.WorkflowCulture)]
        Completed,

        /// <summary>
        /// 被终止
        /// </summary>
        [EnumItemDescription("被终止", Define.WorkflowCulture)]
        Aborted,
    }

    /// <summary>
    /// 对象的加载方式
    /// </summary>
    public enum WfClientDataLoadingType
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
    /// 流程信息的筛选条件
    /// </summary>
    [Flags]
    public enum WfClientProcessInfoFilter
    {
        /// <summary>
        /// 仅有实例
        /// </summary>
        InstanceOnly = 0,

        /// <summary>
        /// 流程描述信息
        /// </summary>
        Descriptor = 1,

        /// <summary>
        /// 绑定活动的描述信息
        /// </summary>
        BindActivityDescriptors = 2,

        /// <summary>
        /// 运行时上下文参数
        /// </summary>
        ApplicationRuntimeParameters = 4,

        /// <summary>
        /// 流程上下文
        /// </summary>
        ProcessContext = 8,

        /// <summary>
        /// 主线流程信息
        /// </summary>
        MainStream = 16,

        /// <summary>
        /// 意见
        /// </summary>
        CurrentOpinion = 31,

        /// <summary>
        /// 默认
        /// </summary>
        Default = WfClientProcessInfoFilter.Descriptor | WfClientProcessInfoFilter.BindActivityDescriptors | WfClientProcessInfoFilter.ApplicationRuntimeParameters | WfClientProcessInfoFilter.ProcessContext,

        /// <summary>
        /// 全部
        /// </summary>
        All = WfClientProcessInfoFilter.Descriptor | WfClientProcessInfoFilter.ApplicationRuntimeParameters
            | WfClientProcessInfoFilter.BindActivityDescriptors | WfClientProcessInfoFilter.MainStream
            | WfClientProcessInfoFilter.ProcessContext | WfClientProcessInfoFilter.CurrentOpinion
    }

    /// <summary>
    /// 获取流程上下文参数时，是否递归查询父流程
    /// </summary>
    public enum WfClientProbeApplicationRuntimeParameterMode
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