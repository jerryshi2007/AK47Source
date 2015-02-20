using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MCS.Library.SOA.DataObjects.Workflow
{
    public class WfDesignerExecutorDataContext : Dictionary<string, object>
    {
        private readonly WfDesignerOperationType _OperationType = WfDesignerOperationType.None;
        private readonly WfDesignerExecutorBase _Executor = null;
        private readonly UserOperationLogCollection _OperationLogs = new UserOperationLogCollection();

        public WfDesignerExecutorDataContext(WfDesignerExecutorBase executor, WfDesignerOperationType opType)
        {
            this._Executor = executor;
            this._OperationType = opType;
        }

        public WfDesignerOperationType OperationType
        {
            get
            {
                return this._OperationType;
            }
        }

        public WfDesignerExecutorBase Executor
        {
            get
            {
                return this._Executor;
            }
        }

        public UserOperationLogCollection OperationLogs
        {
            get
            {
                return this._OperationLogs;
            }
        }
    }
}
