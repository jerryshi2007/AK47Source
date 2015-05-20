using MCS.Library.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MCS.Library.SOA.DataObjects.Workflow.Actions
{
    /// <summary>
    /// 调用服务的Action的基类
    /// </summary>
    public abstract class InvokeServiceActionBase : IWfAction
    {
        public virtual void PrepareAction(WfActionParams actionParams)
        {
            if (WfRuntime.ProcessContext.EnableServiceCall)
            {
                //调用需要在非持久化时调用的服务
                this.InvokeServiceOperations(this.GetAllOperationsBeforePersist());

                //将需要在持久化时调用的服务记录下来
                WfRuntime.ProcessContext.ServiceOperations.CopyFrom(this.GetAllOperationsWhenPersist());
            }
        }

        public virtual void PersistAction(WfActionParams actionParams)
        {
            this.InvokeServiceOperations(WfRuntime.ProcessContext.ServiceOperations);

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
        /// 得到服务调用时的流程上下文参数
        /// </summary>
        /// <returns></returns>
        protected abstract WfApplicationRuntimeParameters GetApplicationRuntimeParameters();

        /// <summary>
        /// 得到持久化时需要调用的服务在配置文件中的Key
        /// </summary>
        /// <returns></returns>
        protected virtual string GetInvokeServiceKeys()
        {
            return string.Empty;
        }

        /// <summary>
        /// 得到在持久化之前需要调用的服务，包括配置文件中定义的
        /// </summary>
        /// <returns></returns>
        private WfServiceOperationDefinitionCollection GetAllOperationsBeforePersist()
        {
            WfServiceOperationDefinitionCollection result = this.GetOperationsBeforePersist();

            string opKeys = this.GetInvokeServiceKeys();

            if (opKeys.IsNotEmpty())
                result.CopyFrom(WfServiceDefinitionSettings.GetSection().GetOperations(false, opKeys));

            return result;
        }

        /// <summary>
        /// 得到在持久化时需要调用的服务，包括配置文件中定义的
        /// </summary>
        /// <returns></returns>
        private WfServiceOperationDefinitionCollection GetAllOperationsWhenPersist()
        {
            WfServiceOperationDefinitionCollection result = this.GetOperationsWhenPersist();

            string opKeys = this.GetInvokeServiceKeys();

            if (opKeys.IsNotEmpty())
                result.CopyFrom(WfServiceDefinitionSettings.GetSection().GetOperations(true, opKeys));

            return result;
        }

        /// <summary>
        /// 调用远程服务
        /// </summary>
        /// <param name="operations"></param>
        protected void InvokeServiceOperations(WfServiceOperationDefinitionCollection operations)
        {
            foreach (WfServiceOperationDefinition svcDefinition in operations)
            {
                WfServiceInvoker svcInvoker = new WfServiceInvoker(svcDefinition);

                svcInvoker.Invoke(this.GetApplicationRuntimeParameters());
            }
        }
    }
}
