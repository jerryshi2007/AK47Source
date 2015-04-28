using MCS.Library.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MCS.Library.SOA.DataObjects.Workflow.Actions
{
    /// <summary>
    /// 活动进出需要调用的外部服务的Action基类
    /// </summary>
    public abstract class ActivityInvokeServiceActionBase : IWfAction
    {
        public void PrepareAction(WfActionParams actionParams)
        {
            if (WfRuntime.ProcessContext.EnableServiceCall)
            {
                //调用需要在非持久化时调用的服务
                InvokeServiceOperations(GetOperationsBeforePersist());

                //将需要在持久化时调用的服务记录下来
                WfRuntime.ProcessContext.ServiceOperations.CopyFrom(GetOperationsWhenPersist());
            }
        }

        public void PersistAction(WfActionParams actionParams)
        {
            InvokeServiceOperations(WfRuntime.ProcessContext.ServiceOperations);

            this.ClearCache();
        }

        public virtual void ClearCache()
        {
            WfRuntime.ProcessContext.ServiceOperations.Clear();
        }

        /// <summary>
        /// 得到在持久化之前需要调用的服务
        /// </summary>
        /// <returns></returns>
        protected abstract WfServiceOperationDefinitionCollection GetOperationsBeforePersist();

        /// <summary>
        /// 得到持久化时需要调用的服务
        /// </summary>
        /// <returns></returns>
        protected abstract WfServiceOperationDefinitionCollection GetOperationsWhenPersist();

        /// <summary>
        /// 调用远程服务
        /// </summary>
        /// <param name="operations"></param>
        private static void InvokeServiceOperations(WfServiceOperationDefinitionCollection operations)
        {
            if (WfRuntime.ProcessContext.CurrentActivity != null)
            {
                foreach (WfServiceOperationDefinition svcDefinition in operations)
                {
                    WfServiceInvoker svcInvoker = new WfServiceInvoker(svcDefinition);

                    svcInvoker.Invoke();
                }
            }
        }
    }
}
