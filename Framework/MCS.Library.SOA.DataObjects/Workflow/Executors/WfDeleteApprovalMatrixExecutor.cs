using MCS.Library.Core;
using MCS.Library.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Transactions;

namespace MCS.Library.SOA.DataObjects.Workflow
{
    /// <summary>
    /// 删除审批矩阵的执行器
    /// </summary>
    public class WfDeleteApprovalMatrixExecutor : WfDesignerExecutorBase
    {
        private readonly string _MatrixID = string.Empty;

        public WfDeleteApprovalMatrixExecutor(string matrixID)
            : base(WfDesignerOperationType.DeleteApprovalMatrix)
        {
            matrixID.CheckStringIsNullOrEmpty("matrixID");

            this._MatrixID = matrixID;
        }

        protected override void OnExecute(WfDesignerExecutorDataContext dataContext)
        {
            using (TransactionScope scope = TransactionScopeFactory.Create())
            {
                SOARolePropertyDefinitionAdapter.Instance.Delete(this.MatrixID);
                SOARolePropertiesAdapter.Instance.Delete(this.MatrixID);

                scope.Complete();
            }
        }

        public string MatrixID
        {
            get
            {
                return _MatrixID;
            }
        }

        protected override void OnPrepareUserOperationLog(WfDesignerExecutorDataContext dataContext, UserOperationLogCollection logs)
        {
            UserOperationLog log = new UserOperationLog();

            log.ResourceID = this.MatrixID;
            log.Subject = string.Format("{0}:{1}", this.OperationType.ToDescription(), this.MatrixID);

            logs.Add(log);

            base.OnPrepareUserOperationLog(dataContext, logs);

            this.FillEnvironmentInfoToLogs(logs);
        }
    }
}
