using MCS.Library.Core;
using MCS.Library.Office.OpenXml.Excel;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MCS.Library.SOA.DataObjects.Workflow
{
    public class WfImportApprovalMatrixExecutor : WfDesignerExecutorBase
    {
        private readonly Stream _InputStream = null;
        private readonly string _MatrixID = string.Empty;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="matrixID"></param>
        /// <param name="inputStream"></param>
        public WfImportApprovalMatrixExecutor(string matrixID, Stream inputStream)
            : base(WfDesignerOperationType.ImportApprovalMatrix)
        {
            matrixID.CheckStringIsNullOrEmpty("matrixID");
            inputStream.NullCheck("inputStream");

            this._MatrixID = matrixID;
            this._InputStream = inputStream;
        }

        public string MatrixID
        {
            get
            {
                return _MatrixID;
            }
        }

        protected override void OnExecute(WfDesignerExecutorDataContext dataContext)
        {
            this._InputStream.Seek(0, SeekOrigin.Begin);

            WorkBook workBook = WorkBook.Load(this._InputStream);

            WfApprovalMatrix matrix = workBook.ToApprovalMatrix();

            matrix.ID = this.MatrixID;

            WfApprovalMatrixAdapter.Instance.Update(matrix);
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
