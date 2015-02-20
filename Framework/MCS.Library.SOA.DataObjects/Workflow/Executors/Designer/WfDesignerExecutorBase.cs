using MCS.Library.Core;
using MCS.Library.Data;
using MCS.Library.Principal;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Transactions;

namespace MCS.Library.SOA.DataObjects.Workflow
{
    /// <summary>
    /// 流程描述操作的执行器的基类
    /// </summary>
    public abstract class WfDesignerExecutorBase
    {
        public event DesignerExecutorEventHandler BeforeExecute;
        public event DesignerExecutorEventHandler AfterExecute;
        public event DesignerPrepareUserOperationLogEventHandler PrepareUserOperationLog;
        public event DesignerErrorEventHandler Error;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="opType"></param>
        protected WfDesignerExecutorBase(WfDesignerOperationType opType)
        {
            this.OperationType = opType;
        }

        public WfDesignerOperationType OperationType
        {
            get;
            protected set;
        }

        /// <summary>
        /// 执行操作
        /// </summary>
        public void Execute()
        {
            this.InternalExecute();
        }

        private void InternalExecute()
        {
            WfDesignerExecutorDataContext dataContext = new WfDesignerExecutorDataContext(this, this.OperationType);

            try
            {
                ExecutionWrapper("BeforeExecute", () => this.OnBeforeExecute(dataContext));
                ExecutionWrapper("PrepareLogs", () => OnPrepareUserOperationLog(dataContext, dataContext.OperationLogs));

                using (TransactionScope scope = TransactionScopeFactory.Create())
                {
                    ExecutionWrapper("Execute", () => this.OnExecute(dataContext));
                    ExecutionWrapper("SaveOperationLogs", () => this.OnSaveUserOperationLogs(dataContext, dataContext.OperationLogs));

                    scope.Complete();
                }
            }
            catch (System.Exception ex)
            {
                bool autoThrow = true;

                this.OnError(ex, dataContext, ref autoThrow);

                if (autoThrow)
                    throw;
            }
            finally
            {
                ExecutionWrapper("AfterExecute", () => this.OnAfterExecute(dataContext));
            }
        }

        protected virtual void OnBeforeExecute(WfDesignerExecutorDataContext dataContext)
        {
            if (this.BeforeExecute != null)
                this.BeforeExecute(dataContext);
        }

        /// <summary>
        /// 执行操作
        /// </summary>
        /// <param name="dataContext"></param>
        protected abstract void OnExecute(WfDesignerExecutorDataContext dataContext);

        protected virtual void OnAfterExecute(WfDesignerExecutorDataContext dataContext)
        {
            if (this.AfterExecute != null)
                this.AfterExecute(dataContext);
        }

        protected virtual void FirePrepareUserOperationLog(WfDesignerExecutorDataContext dataContext, UserOperationLogCollection logs)
        {
            if (this.PrepareUserOperationLog != null)
                this.PrepareUserOperationLog(dataContext, logs);
        }

        protected virtual void OnPrepareUserOperationLog(WfDesignerExecutorDataContext dataContext, UserOperationLogCollection logs)
        {
            this.FirePrepareUserOperationLog(dataContext, logs);
        }

        protected virtual void OnError(System.Exception ex, WfDesignerExecutorDataContext dataContext, ref bool autoThrow)
        {
            if (this.Error != null)
                this.Error(ex, dataContext, ref autoThrow);
        }

        /// <summary>
        /// 填充日志中的环境信息
        /// </summary>
        /// <param name="logs"></param>
        protected void FillEnvironmentInfoToLogs(UserOperationLogCollection logs)
        {
            logs.ForEach(log =>
            {
                log.OperationName = this.OperationType.ToDescription();

                if (DeluxePrincipal.IsAuthenticated)
                {
                    log.Operator = DeluxeIdentity.CurrentUser;
                    log.RealUser = DeluxeIdentity.CurrentRealUser;

                    log.TopDepartment = DeluxeIdentity.CurrentUser.TopOU;
                }
            });
        }

        protected virtual void OnSaveUserOperationLogs(WfDesignerExecutorDataContext dataContext, UserOperationLogCollection logs)
        {
            logs.ForEach(log => UserOperationLogAdapter.Instance.InsertData(log));
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
