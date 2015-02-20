using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MCS.Library.Core;

namespace MCS.Library.SOA.DataObjects.Workflow
{
    /// <summary>
    /// 保存流程模板的执行器
    /// </summary>
    public class WfSaveTemplateExecutor : WfDesignerExecutorBase
    {
        public WfSaveTemplateExecutor(IWfProcessDescriptor processDesp)
            : base(WfDesignerOperationType.CreateTemplate)
        {
            processDesp.NullCheck("processDesp");

            if (WfProcessDescriptorManager.ExsitsProcessKey(processDesp.Key))
                this.OperationType = WfDesignerOperationType.ModifyTemplate;
            else
                this.OperationType = WfDesignerOperationType.CreateTemplate;

            this.ProcessDescriptor = processDesp;
        }

        public IWfProcessDescriptor ProcessDescriptor
        {
            get;
            private set;
        }

        protected override void OnExecute(WfDesignerExecutorDataContext dataContext)
        {
            WfProcessDescriptorManager.SaveDescriptor(this.ProcessDescriptor);
        }

        protected override void OnPrepareUserOperationLog(WfDesignerExecutorDataContext dataContext, UserOperationLogCollection logs)
        {
            UserOperationLog log = new UserOperationLog();

            log.ResourceID = this.ProcessDescriptor.Key;
            log.Subject = string.Format("{0}:{1}", this.OperationType.ToDescription(), this.ProcessDescriptor.Key);

            logs.Add(log);

            base.OnPrepareUserOperationLog(dataContext, logs);

            this.FillEnvironmentInfoToLogs(logs);
        }
    }
}
