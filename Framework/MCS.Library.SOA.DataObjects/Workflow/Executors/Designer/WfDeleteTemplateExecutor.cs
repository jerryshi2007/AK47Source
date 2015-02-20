using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MCS.Library.Core;

namespace MCS.Library.SOA.DataObjects.Workflow
{
    public class WfDeleteTemplateExecutor : WfDesignerExecutorBase
    {
        /// <summary>
        /// 删除流程模板的执行器
        /// </summary>
        /// <param name="processDespKey"></param>
        public WfDeleteTemplateExecutor(string processDespKey)
            : base(WfDesignerOperationType.DeleteTemplate)
        {
            processDespKey.NullCheck("processDespKey");

            this.ProcessDescriptorKey = processDespKey;
        }

        public string ProcessDescriptorKey
        {
            get;
            private set;
        }

        protected override void OnExecute(WfDesignerExecutorDataContext dataContext)
        {
            WfProcessDescriptorManager.DeleteDescriptor(this.ProcessDescriptorKey);
        }

        protected override void OnPrepareUserOperationLog(WfDesignerExecutorDataContext dataContext, UserOperationLogCollection logs)
        {
            UserOperationLog log = new UserOperationLog();

            log.ResourceID = this.ProcessDescriptorKey;
            log.Subject = string.Format("{0}:{1}", this.OperationType.ToDescription(), this.ProcessDescriptorKey);

            logs.Add(log);

            base.OnPrepareUserOperationLog(dataContext, logs);

            this.FillEnvironmentInfoToLogs(logs);
        }
    }
}
