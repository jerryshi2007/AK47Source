using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MCS.Library.OGUPermission;

namespace MCS.Library.SOA.DataObjects.Workflow
{
    public interface IWfActivity
    {
        /// <summary>
        /// 节点的ID
        /// </summary>
        string ID
        {
            get;
        }

        /// <summary>
        /// 节点的描述信息
        /// </summary>
        IWfActivityDescriptor Descriptor
        {
            get;
            set;
        }

        /// <summary>
        /// 节点的流程信息
        /// </summary>
        IWfProcess Process
        {
            get;

        }

        /// <summary>
        /// 分支流程组
        /// </summary>
        WfBranchProcessGroupCollection BranchProcessGroups
        {
            get;
        }

        /// <summary>
        /// 节点状态
        /// </summary>
        WfActivityStatus Status
        {
            get;
        }

        /// <summary>
        /// 节点起始时间
        /// </summary>
        DateTime StartTime
        {
            get;
        }

        /// <summary>
        /// 节点结束时间
        /// </summary>
        DateTime EndTime
        {
            get;
        }

        /// <summary>
        /// 加载方式
        /// </summary>
        DataLoadingType LoadingType
        {
            get;
        }

        /// <summary>
        /// 节点的上下文
        /// </summary>
        WfActivityContext Context
        {
            get;
        }

        /// <summary>
        /// 任务分派人
        /// </summary>
        WfAssigneeCollection Assignees
        {
            get;
        }

        IUser Operator
        {
            get;
        }

        /// <summary>
        /// 是否可以流传走
        /// </summary>
        bool CanMoveTo
        {
            get;
        }

        /// <summary>
        /// 本活动对应着主线活动的描述的Key
        /// </summary>
        string MainStreamActivityKey
        {
            get;
        }

        /// <summary>
        /// 该环节的候选人
        /// </summary>
        WfAssigneeCollection Candidates
        {
            get;
        }

        BranchProcessReturnType BranchProcessReturnValue
        {
            get;
        }

        /// <summary>
        /// 得到对应的主线流程活动的描述。如果没有找到，则返回False
        /// </summary>
        /// <returns></returns>
        IWfActivityDescriptor GetMainStreamActivityDescriptor();

        /// <summary>
        /// 取消所有的分支流程
        /// </summary>
        /// <param name="recursive">是否递归</param>
        void CancelBranchProcesses(bool recursive);

        /// <summary>
        /// 暂停所有分支流程
        /// </summary>
        /// <param name="recursive"></param>
        void PauseBranchProcesses(bool recursive);

        /// <summary>
        /// 恢复所有暂停的分支流程
        /// </summary>
        /// <param name="recursive"></param>
        void ResumeBranchProcesses(bool recursive);

        /// <summary>
        /// 结束所有分支流程
        /// </summary>
        /// <param name="recursive"></param>
        void CompleteBranchProcesses(bool recursive);

        /// <summary>
        /// 启动分支流程
        /// </summary>
        /// <param name="branchParams"></param>
        void StartupBranchProcesses(WfBranchProcessTransferParams branchParams);

        /// <summary>
        /// 在当前活动之前插入活动。当前活动不能是起始点。
        /// </summary>
        /// <param name="actDesp"></param>
        IWfActivity InsertBefore(IWfActivityDescriptor actDesp);

        /// <summary>
        /// 在当前活动实例后，添加一个新的活动
        /// </summary>
        /// <param name="actDesp"></param>
        /// <returns></returns>
        IWfActivity Append(IWfActivityDescriptor newActDesp);

        /// <summary>
        /// 在当前活动之后添加活动。当前活动不能是结束点。
        /// 自动创建一条当前点和新节点的连线，而当前点的向前的出线，将作为新点的出线。
        /// </summary>
        /// <param name="newActDesp">需要添加的新活动</param>
        /// <param name="moveReturnLine">是否将当前点的退回线也复制到新的活动之后</param>
        IWfActivity Append(IWfActivityDescriptor newActDesp, bool moveReturnLine);

        /// <summary>
        /// 在当前活动实例后，添加一些被克隆的活动。
        /// 即从指定的点截至到当前点
        /// </summary>
        /// <param name="actDesp">被克隆的活动</param>

        /// <summary>
        /// 在当前活动实例后，添加一些被克隆的活动。
        /// 即从指定的点截至到当前点
        /// </summary>
        /// <param name="activity">被克隆的活动</param>
        /// <param name="entryTransition"></param>
        /// <param name="operationType">操作类型(目前退件、加签）</param>
        /// <returns>返回的第一个Clone活动的出线</returns>
        IWfTransitionDescriptor CopyMainStreamActivities(IWfActivity activity, IWfTransitionDescriptor entryTransition, WfControlOperationType operationType);

        /// <summary>
        /// 在指定的活动点之后，添加从startActivity与endActivity之间复制出来的
        /// </summary>
        /// <param name="activityToAppend">在哪个节点之后添加需要复制的节点</param>
        /// <param name="startActivity">需要复制的开始节点</param>
        /// <param name="endActivity">需要复制的结束节点</param>
        /// <param name="entryTransition"></param>
        /// <param name="operationType"></param>
        /// <returns>返回的第一个Clone活动的出线</returns>
        IWfTransitionDescriptor CopyMainStreamActivities(IWfActivity activityToAppend, IWfActivity startActivity, IWfActivity endActivity, IWfTransitionDescriptor entryTransition, WfControlOperationType operationType);

        /// <summary>
        /// 删除自身实例，同时删除所有进出线
        /// </summary>
        void Remove();

        /// <summary>
        /// 删除自身实例
        /// </summary>
        void Delete();

        /// <summary>
        /// 从当前资源中生成Candidates
        /// </summary>
        void GenerateCandidatesFromResources();

        /// <summary>
        /// 从哪条线定义产生的活动点。有可能为null，例如起始点。
        /// </summary>
        IWfTransitionDescriptor FromTransitionDescriptor
        {
            get;
        }

        /// <summary>
        /// 从哪条线流转出去。有可能为null，例如结束点或者没有走到的点。如果活动被撤回，这个属性保持最后一次流转的线
        /// </summary>
        IWfTransitionDescriptor ToTransitionDescriptor
        {
            get;
        }

        WfActionCollection EnterActions
        {
            get;
        }

        WfActionCollection LeaveActions
        {
            get;
        }

        WfActionCollection WithdrawActions
        {
            get;
        }

        WfActionCollection BeWithdrawnActions
        {
            get;
        }

        IWfActivity RootActivity
        {
            get;
        }

        /// <summary>
        /// 相同资源的根活动（相同的ResourceID）
        /// </summary>
        IWfActivity SameResourceRootActivity
        {
            get;

        }

        /// <summary>
        /// 审批流的根活动。从当前流程找到审批流的边界的OwnerActivity。
        /// </summary>
        IWfActivity ApprovalRootActivity
        {
            get;

        }

        /// <summary>
        /// 计划（业务）流程的根活动。如果当前流程是计划（业务）流程，则立即返回。如果都不是，则返回根流程的活动
        /// </summary>
        IWfActivity ScheduleRootActivity
        {
            get;
        }

        /// <summary>
        /// 意见控件显示流程对应的活动
        /// </summary>
        IWfActivity OpinionRootActivity
        {
            get;
        }

        /// <summary>
        /// 流程活动的创建者的ID，默认为空。通常是其它活动实例的ID
        /// </summary>
        string CreatorInstanceID
        {
            get;
        }
    }
}
