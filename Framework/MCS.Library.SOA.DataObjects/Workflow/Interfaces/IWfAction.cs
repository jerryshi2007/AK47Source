using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MCS.Library.Data.DataObjects;
using MCS.Library.Expression;
using MCS.Library.Core;

namespace MCS.Library.SOA.DataObjects.Workflow
{
    /// <summary>
    /// 流转时的委托
    /// </summary>
    /// <param name="eventArgs"></param>
    public delegate void WfMoveToHandler(WfMoveToEventArgs eventArgs);

    /// <summary>
    /// 执行Action的委托
    /// </summary>
    public delegate void WfActionHandler();

    /// <summary>
    /// 准备分支流程启动前的委托
    /// </summary>
    /// <param name="group">分支流程组信息，里面包括分支流程模板和已经启动的流程实例。在此事件中是没有实例的</param>
    /// <param name="branchParams"></param>
    public delegate void WfPrepareBranchProcessParamsHandler(IWfBranchProcessGroup group, WfBranchProcessStartupParamsCollection branchParams);

    /// <summary>
    /// 启动分支流程后的事件委托
    /// </summary>
    /// <param name="process">分支流程</param>
    public delegate void WfAfterStartupBranchProcessHandler(IWfProcess process);

    /// <summary>
    /// 得到流程描述的事件委托。WfProcessDescriptorManager的GetProcessDescriptor会使用此事件
    /// </summary>
    /// <param name="processDespKey">流程描述的Key</param>
    /// <returns>如果返回不是null，那么WfProcessDescriptorManager的GetProcessDescriptor就使用此流程描述，否则使用缺省的加载方式</returns>
    public delegate IWfProcessDescriptor WfGetProcessDescriptorHandler(string processDespKey);

    /// <summary>
    /// 角色矩阵，合并行时，抛出的事件
    /// </summary>
    /// <param name="rowsUsers"></param>
    /// <param name="eventArgs"></param>
    public delegate void WfRemoveMatrixMergeableRowsHandler(SOARolePropertyRowUsersCollection rowsUsers, WfMergeMatrixRowParams eventArgs);

    public enum WfMergeMatrixRowMethod
    {
        KeepTheLastestRow,
        KeepTheEarliestRow,
        NoChange
    }

    public class WfMergeMatrixRowParams
    {
        public WfMergeMatrixRowMethod Method;
    }

    /// <summary>
    /// 动作相关的参数
    /// </summary>
    public class WfActionParams
    {
        /// <summary>
        /// 构造方法
        /// </summary>
        /// <param name="context"></param>
        public WfActionParams(WfProcessActionContext context)
        {
            this.Context = context;
        }

        public WfProcessActionContext Context
        {
            get;
            set;
        }
    }

    public interface IWfAction
    {
        /// <summary>
        /// 发生在流程持久化的事务之前
        /// </summary>
        /// <param name="actionParams"></param>
        void PrepareAction(WfActionParams actionParams);

        /// <summary>
        /// 发生在流程持久化的事务之中，但是发生在流程本身的持久化操作之前
        /// </summary>
        /// <param name="actionParams"></param>
        void PersistAction(WfActionParams actionParams);

        /// <summary>
        /// 发生在流程持久化的事务之中，但是发生在流程本身的持久化操作之后
        /// </summary>
        /// <param name="actionParams"></param>
        void AfterWorkflowPersistAction(WfActionParams actionParams);

        /// <summary>
        /// 清理上下文缓存中的内容
        /// </summary>
        void ClearCache();
    }

    [Serializable]
    public class WfActionCollection : EditableDataObjectCollectionBase<IWfAction>
    {
        public void PrepareActions(WfActionParams actionParams)
        {
            ForEach(action => action.PrepareAction(actionParams));
        }

        public void PersistActions(WfActionParams actionParams)
        {
            this.ForEach(action =>
                {
                    PerformanceMonitorHelper.GetDefaultMonitor().WriteExecutionDuration(
                        string.Format("Persist Action: {0}", action.GetType().FullName),
                        () => action.PersistAction(actionParams));
                });
        }

        public void AfterWorkflowPersistAction(WfActionParams actionParams)
        {
            ProcessProgress.Current.MaxStep += this.Count;
            ProcessProgress.Current.Response();

            this.ForEach(action =>
            {
                PerformanceMonitorHelper.GetDefaultMonitor().WriteExecutionDuration(
                    string.Format("AfterWorkflowPersist Action: {0}", action.GetType().FullName),
                    () => action.AfterWorkflowPersistAction(actionParams));

                ProcessProgress.Current.Increment();
                ProcessProgress.Current.Response();
            });
        }

        public void ClearCache()
        {
            ForEach(action => action.ClearCache());
        }
    }

    /// <summary>
    /// 当流程不同时，保存的原始流程Context的状态
    /// </summary>
    internal class WfProcessActionContextState
    {
        /// <summary>
        /// 已经保存的OrginalActivity
        /// </summary>
        public IWfActivity SavedOriginalActivity
        {
            get;
            set;
        }

        /// <summary>
        /// 是否需要恢复（是否是同一流程）
        /// </summary>
        public bool NeedToRestore
        {
            get;
            set;
        }
    }
}
