using MCS.Library.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MCS.Library.SOA.DataObjects.Workflow
{
    /// <summary>
    /// 清空所有流程模板的执行器
    /// </summary>
    public class WfClearAllTemplatesExecutor : WfDesignerExecutorBase
    {
        /// <summary>
        /// 构造方法
        /// </summary>
        public WfClearAllTemplatesExecutor()
            : base(WfDesignerOperationType.ClearAll)
        {
        }

        protected override void OnExecute(WfDesignerExecutorDataContext dataContext)
        {
            WfProcessDescriptorManager.ClearAll();
        }

        protected override void OnPrepareUserOperationLog(WfDesignerExecutorDataContext dataContext, UserOperationLogCollection logs)
        {
            UserOperationLog log = new UserOperationLog();

            log.Subject = string.Format("{0}", this.OperationType.ToDescription());

            logs.Add(log);

            base.OnPrepareUserOperationLog(dataContext, logs);

            this.FillEnvironmentInfoToLogs(logs);
        }
    }
}
