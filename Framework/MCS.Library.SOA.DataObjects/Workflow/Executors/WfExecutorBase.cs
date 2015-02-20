using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Transactions;
using MCS.Library.Core;
using MCS.Library.Data;

namespace MCS.Library.SOA.DataObjects.Workflow
{
    /// <summary>
    /// 业务功能执行者的基类。所谓业务功能执行者，就是将一组工作操作组合起来，
    /// 组成一个业务行为，例如“流转”、“保存”、“撤回”、“作废”、“退回”。
    /// 有些操作对于工作流引擎是是一个单一行为，例如流转；
    /// 有些就是一些组合操作，例如“退回”，本质上是修改流程定义，复制出新的活动，然后再流转。
    /// </summary>
    public abstract class WfExecutorBase
    {
        private WfExecutorDataContext _DataContext = null;

        public event MoveToEventHandler BeforeMoveTo;
        public event MoveToEventHandler AfterMoveTo;
        public event ExecutorEventHandler BeforeExecute;
        public event ExecutorEventHandler PrepareApplicationData;
        public event PrepareTasksEventHandler PrepareMoveToTasks;
        public event PrepareTasksEventHandler PrepareNotifyTasks;
        public event PrepareUserOperationLogEventHandler PrepareUserOperationLog;
        public event ExecutorEventHandler SaveApplicationData;
        public event ExecutorEventHandler AfterSaveApplicationData;
        public event ExecutorEventHandler AfterModifyWorkflow;
        public event ErrorEventHandler Error;

        private Dictionary<string, object> _ExtendedProperties = null;
        private WfExecutorActionCollection _Actions = new WfExecutorActionCollection();

        /// <summary>
        /// 
        /// </summary>
        /// <param name="operatorActivity">操作人所在的流程活动</param>
        /// <param name="opType"></param>
        protected WfExecutorBase(IWfActivity operatorActivity, WfControlOperationType opType)
        {
            OperatorActivity = operatorActivity;
            OperationType = opType;

            WfExecutorActionCollection actions = this._Actions;

            actions.Add(new WfExecutorAction("BeforeExecute", new Action<WfExecutorDataContext>(this.OnBeforeExecute)));
            actions.Add(new WfExecutorAction("ModifyWorkflow", new Action<WfExecutorDataContext>(this.CallModifyWorkflow)));
            actions.Add(new WfExecutorAction("AfterModifyWorkflow", new Action<WfExecutorDataContext>(this.OnAfterModifyWorkflow)));

            actions.Add(new WfExecutorAction("PrepareApplicationData", new Action<WfExecutorDataContext>(this.OnPrepareApplicationData)));
            actions.Add(new WfExecutorAction("PrepareUserTaskTitles", new Action<WfExecutorDataContext>(this.OnPrepareUserTaskTitles)));
            actions.Add(new WfExecutorAction("PrepareMoveToTasks", new Action<WfExecutorDataContext>(this.OnPrepareMoveToTasks)));
            actions.Add(new WfExecutorAction("PrepareNotifyTasks", new Action<WfExecutorDataContext>(this.OnPrepareNotifyTasks)));
            actions.Add(new WfExecutorAction("PrepareUserOperationLog", new Action<WfExecutorDataContext>(this.OnPrepareUserOperationLog)));

            actions.Add(new WfExecutorAction("SaveApplicationData", new Action<WfExecutorDataContext>(this.OnSaveApplicationData)));
            actions.Add(new WfExecutorAction("SaveUserOperationLog", new Action<WfExecutorDataContext>(this.OnSaveUserOperationLog)));
            actions.Add(new WfExecutorAction("PersistWorkflow", new Action<WfExecutorDataContext>(this.OnPersistWorkflow)));
        }

        /// <summary>
        /// 扩展的属性
        /// </summary>
        public Dictionary<string, object> ExtendedProperties
        {
            get
            {
                if (this._ExtendedProperties == null)
                    this._ExtendedProperties = new Dictionary<string, object>();

                return this._ExtendedProperties;
            }
        }

        /// <summary>
        /// 操作人所在的流程活动
        /// </summary>
        public IWfActivity OperatorActivity
        {
            get;
            protected set;
        }

        public WfControlOperationType OperationType
        {
            get;
            protected set;
        }

        public IWfProcess Execute()
        {
            IWfProcess result = null;

            try
            {
                ExecutionWrapper(EnumItemDescriptionAttribute.GetDescription(OperationType),
                    () => result = InternalExecute(true));

                return result;
            }
            finally
            {
                WfExecutorLogContextInfo.CommitInfoToLogger();
            }
        }

        /// <summary>
        /// 执行但是不真正保存，只是准备数据
        /// </summary>
        /// <returns></returns>
        public IWfProcess ExecuteNotPersist()
        {
            IWfProcess result = null;

            try
            {
                ExecutionWrapper(EnumItemDescriptionAttribute.GetDescription(OperationType),
                    () => result = InternalExecute(false));

                return result;
            }
            finally
            {
                WfExecutorLogContextInfo.CommitInfoToLogger();
            }
        }

        /// <summary>
        /// 如何获取当前流程
        /// </summary>
        /// <returns></returns>
        protected virtual IWfProcess OnGetCurrentProcess()
        {
            return WfRuntime.ProcessContext.CurrentProcess;
        }

        private IWfProcess InternalExecute(bool persist)
        {
            ProcessProgress.Current.CurrentStep = 0;
            ProcessProgress.Current.MinStep = 0;
            ProcessProgress.Current.MaxStep = this._Actions.Count;
            ProcessProgress.Current.Response();

            WfExecutorDataContext dataContext = new WfExecutorDataContext(this, OperationType);
            this._DataContext = dataContext;
            try
            {
                //准备数据阶段
                dataContext.CurrentProcess = OnGetCurrentProcess();

                ExecuteAction("BeforeExecute", dataContext);

                ExecuteAction("ModifyWorkflow", dataContext);

                if (WfRuntime.ProcessContext.CurrentProcess != null)
                    dataContext.CurrentProcess = WfRuntime.ProcessContext.CurrentProcess;

                ExecuteAction("AfterModifyWorkflow", dataContext);
                ExecuteAction("PrepareApplicationData", dataContext);

                if (WfRuntime.ProcessContext != null)
                {
                    dataContext.MoveToTasks = WfRuntime.ProcessContext.MoveToUserTasks;
                    dataContext.NotifyTasks = WfRuntime.ProcessContext.NotifyUserTasks;
                }
                else
                {
                    dataContext.MoveToTasks = new UserTaskCollection();
                    dataContext.NotifyTasks = new UserTaskCollection();
                }

                ExecuteAction("PrepareUserTaskTitles", dataContext);
                ExecuteAction("PrepareMoveToTasks", dataContext);
                ExecuteAction("PrepareNotifyTasks", dataContext);

                ExecuteAction("PrepareUserOperationLog", dataContext);

                if (persist)
                {
                    OnBeforeTransaction(dataContext);

                    try
                    {
                        //保存数据阶段，带Transaction
                        using (TransactionScope scope = TransactionScopeFactory.Create())
                        {
                            ExecuteAction("SaveApplicationData", dataContext);

                            ExecuteAction("SaveUserOperationLog", dataContext);

                            ExecuteAction("PersistWorkflow", dataContext);

                            scope.Complete();
                        }
                    }
                    finally
                    {
                        OnAfterTransaction(dataContext);
                    }

                    ExecutionWrapper("AfterSaveApplicationData", () => OnAfterSaveApplicationData(dataContext));
                }

                return dataContext.CurrentProcess;
            }
            catch (System.Exception ex)
            {
                bool autoThrow = true;

                OnError(ex, dataContext, ref autoThrow);

                if (autoThrow)
                    throw;
                else
                    return dataContext.CurrentProcess;
            }
            finally
            {
                OnAfterExecute(dataContext);
                this._DataContext = null;
            }
        }

        protected virtual void OnAfterExecute(WfExecutorDataContext dataContext)
        {
        }

        protected virtual void OnAfterTransaction(WfExecutorDataContext dataContext)
        {
        }

        protected virtual void OnBeforeTransaction(WfExecutorDataContext dataContext)
        {
        }

        protected virtual void OnBeforeExecute(WfExecutorDataContext dataContext)
        {
            if (BeforeExecute != null)
                BeforeExecute(dataContext);
        }

        protected abstract void OnModifyWorkflow(WfExecutorDataContext dataContext);

        protected virtual void OnAfterModifyWorkflow(WfExecutorDataContext dataContext)
        {
            if (AfterModifyWorkflow != null)
                AfterModifyWorkflow(dataContext);
        }

        protected virtual void OnPrepareApplicationData(WfExecutorDataContext dataContext)
        {
            if (PrepareApplicationData != null)
                PrepareApplicationData(dataContext);
        }

        protected virtual void OnPrepareUserTaskTitles(WfExecutorDataContext dataContext)
        {
            WfRuntime.ProcessContext.NormalizeTaskTitles();
        }

        protected virtual void OnPrepareMoveToTasks(WfExecutorDataContext dataContext)
        {
            UserTaskCollection tasks = dataContext.MoveToTasks;

            if (PrepareMoveToTasks != null)
                PrepareMoveToTasks(dataContext, tasks);
        }

        protected virtual void OnPrepareNotifyTasks(WfExecutorDataContext dataContext)
        {
            UserTaskCollection tasks = dataContext.NotifyTasks;

            if (PrepareNotifyTasks != null)
                PrepareNotifyTasks(dataContext, tasks);
        }

        protected virtual void OnPrepareUserOperationLogDescription(WfExecutorDataContext dataContext, UserOperationLog log)
        {
            if (log.RealUser != null && log.OperationDescription.IsNullOrEmpty())
            {
                log.OperationDescription = string.Format("{0}:{1}->'{2}' {3:yyyy-MM-dd HH:mm:ss}",
                            log.OperationName, log.RealUser.DisplayName, log.Subject, DateTime.Now);
            }
        }

        protected virtual void OnPrepareUserOperationLog(WfExecutorDataContext dataContext)
        {
            UserOperationLogCollection logs = dataContext.OperationLogs;

            if (OperatorActivity != null)
            {
                UserOperationLog log = UserOperationLog.FromActivity(OperatorActivity);

                log.OperationType = DataObjects.OperationType.Update;
                log.OperationName = EnumItemDescriptionAttribute.GetDescription(this.OperationType);

                logs.Add(log);
            }

            FirePrepareUserOperationLog(dataContext, logs);

            foreach (UserOperationLog log in logs)
            {
                OnPrepareUserOperationLogDescription(dataContext, log);
            }

            List<string> accomplishedUserTaskIDs = new List<string>();
            foreach (UserTask item in WfRuntime.ProcessContext.AccomplishedUserTasks)
            {
                accomplishedUserTaskIDs.Add(item.TaskID);
            }

            UserOperationTasksLogCollection userOperationlogs = new UserOperationTasksLogCollection();
            foreach (UserTask userTaskItem in WfRuntime.ProcessContext.MoveToUserTasks)
            {
                if (accomplishedUserTaskIDs.Exists(taskID => string.Equals(taskID, userTaskItem.TaskID)) == false)
                    userOperationlogs.Add(UserOperationTasksLog.FromUserTask(userTaskItem));
            }

            if (userOperationlogs.Count > 0)
                dataContext.Add("USER_OPERATION_TASKSLOGS", userOperationlogs);
            else
                dataContext.Remove("USER_OPERATION_TASKSLOGS");
        }

        protected virtual void FirePrepareUserOperationLog(WfExecutorDataContext dataContext, UserOperationLogCollection logs)
        {
            if (PrepareUserOperationLog != null)
                PrepareUserOperationLog(dataContext, logs);
        }

        protected virtual void OnSaveApplicationData(WfExecutorDataContext dataContext)
        {
            if (SaveApplicationData != null)
                SaveApplicationData(dataContext);
        }

        protected virtual void OnPersistWorkflow(WfExecutorDataContext dataContext)
        {
            WfRuntime.PersistWorkflows();
        }

        protected virtual void OnAfterSaveApplicationData(WfExecutorDataContext dataContext)
        {
            if (AfterSaveApplicationData != null)
                AfterSaveApplicationData(dataContext);
        }

        protected virtual void OnError(System.Exception ex, WfExecutorDataContext dataContext, ref bool autoThrow)
        {
            if (Error != null)
                Error(ex, dataContext, ref autoThrow);
        }

        protected virtual void OnBeforeMoveTo(WfExecutorDataContext dataContext, WfMoveToEventArgs eventArgs)
        {
            if (BeforeMoveTo != null)
                BeforeMoveTo(dataContext, eventArgs);
        }

        protected virtual void OnAfterMoveTo(WfExecutorDataContext dataContext, WfMoveToEventArgs eventArgs)
        {
            if (AfterMoveTo != null)
                AfterMoveTo(dataContext, eventArgs);
        }

        private void OnSaveUserOperationLog(WfExecutorDataContext dataContext)
        {
            UserOperationTasksLogCollection userTasks = null;

            if (dataContext.ContainsKey("USER_OPERATION_TASKSLOGS") == true)
            {
                userTasks = dataContext["USER_OPERATION_TASKSLOGS"] as UserOperationTasksLogCollection;
            }

            foreach (UserOperationLog userLog in dataContext.OperationLogs)
            {
                int identityID = UserOperationLogAdapter.Instance.InsertData(userLog);
                if (userTasks != null)
                {
                    foreach (UserOperationTasksLog item in userTasks)
                    {
                        item.ID = identityID;
                    }
                }
            }

            if (userTasks != null)
                UserOperationTasksLogAdapter.Instance.AddOperationTasksLogs(userTasks);
        }

        private void CallModifyWorkflow(WfExecutorDataContext dataContext)
        {
            WfRuntime.ProcessContext.BeforeMoveTo += new WfMoveToHandler(ProcessContext_BeforeMoveTo);
            WfRuntime.ProcessContext.AfterMoveTo += new WfMoveToHandler(ProcessContext_AfterMoveTo);

            try
            {
                OnModifyWorkflow(dataContext);
            }
            finally
            {
                WfRuntime.ProcessContext.BeforeMoveTo -= new WfMoveToHandler(ProcessContext_BeforeMoveTo);
                WfRuntime.ProcessContext.AfterMoveTo -= new WfMoveToHandler(ProcessContext_AfterMoveTo);
            }
        }

        private void ProcessContext_BeforeMoveTo(WfMoveToEventArgs eventArgs)
        {
            OnBeforeMoveTo(this._DataContext, eventArgs);
        }

        private void ProcessContext_AfterMoveTo(WfMoveToEventArgs eventArgs)
        {
            OnAfterMoveTo(this._DataContext, eventArgs);
        }

        private void ExecuteAction(string operationName, WfExecutorDataContext dataContext)
        {
            operationName.CheckStringIsNullOrEmpty("operationName");

            WfExecutorLogContextInfo.Writer.WriteLine("\t\t{0}开始：{1:yyyy-MM-dd HH:mm:ss.fff}",
                    operationName, DateTime.Now);

            Stopwatch sw = new Stopwatch();

            sw.Start();
            try
            {
                this._Actions.ExecuteActionByName(operationName, dataContext);
            }
            finally
            {
                sw.Stop();
                WfExecutorLogContextInfo.Writer.WriteLine("\t\t{0}结束：{1:yyyy-MM-dd HH:mm:ss.fff}；经过时间：{2:#,##0}毫秒",
                    operationName, DateTime.Now, sw.ElapsedMilliseconds);

                ProcessProgress.Current.Increment();
                ProcessProgress.Current.Response();
            }
        }

        /// <summary>
        /// 操作执行的包装类，在操作的开始和结束输出时间信息
        /// </summary>
        /// <param name="operationName"></param>
        /// <param name="action"></param>
        private static void ExecutionWrapper(string operationName, Action action)
        {
            operationName.CheckStringIsNullOrEmpty("operationName");
            action.NullCheck("action");

            WfExecutorLogContextInfo.Writer.WriteLine("\t\t{0}开始：{1:yyyy-MM-dd HH:mm:ss.fff}",
                    operationName, DateTime.Now);

            Stopwatch sw = new Stopwatch();

            sw.Start();
            try
            {
                action();
            }
            finally
            {
                sw.Stop();
                WfExecutorLogContextInfo.Writer.WriteLine("\t\t{0}结束：{1:yyyy-MM-dd HH:mm:ss.fff}；经过时间：{2:#,##0}毫秒",
                    operationName, DateTime.Now, sw.ElapsedMilliseconds);
            }
        }
    }
}
