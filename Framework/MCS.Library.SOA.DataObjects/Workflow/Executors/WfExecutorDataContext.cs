using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MCS.Library.SOA.DataObjects.Workflow
{
    /// <summary>
    /// 流程操作执行者的上下文
    /// </summary>
    [Serializable]
    public class WfExecutorDataContext : Dictionary<string, object>
    {
        private readonly WfControlOperationType _OperationType = WfControlOperationType.None;
        private readonly WfExecutorBase _Executor = null;
        private readonly UserOperationLogCollection _OperationLogs = new UserOperationLogCollection();
        private UserTaskCollection _MoveToTasks = null;
        private UserTaskCollection _NotifyTasks = null;

        public WfExecutorDataContext(WfExecutorBase executor, WfControlOperationType opType)
        {
            this._Executor = executor;
            this._OperationType = opType;
        }

        public WfExecutorBase Executor
        {
            get
            {
                return this._Executor;
            }
        }

        public WfControlOperationType OperationType
        {
            get { return this._OperationType; }
        }

        public IWfProcess CurrentProcess
        {
            get;
            internal set;
        }

        public UserOperationLogCollection OperationLogs
        {
            get
            {
                return this._OperationLogs;
            }
        }

        public UserTaskCollection MoveToTasks
        {
            get
            {
                return this._MoveToTasks;
            }
            internal set
            {
                this._MoveToTasks = value;
            }
        }

        public UserTaskCollection NotifyTasks
        {
            get
            {
                return this._NotifyTasks;
            }
            internal set
            {
                this._NotifyTasks = value;
            }
        }
    }
}
