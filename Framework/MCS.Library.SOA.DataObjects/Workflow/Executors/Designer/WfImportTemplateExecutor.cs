using MCS.Library.Core;
using MCS.Library.SOA.DataObjects.Workflow.Importers;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MCS.Library.SOA.DataObjects.Workflow
{
    public class WfImportTemplateExecutor : WfDesignerExecutorBase
    {
        public const int MaxStep = WfProcessImporter.MaxStep;

        private readonly Stream _InputStream = null;
        private readonly Action<string> _NotifyEveryStep = null;
        private List<IWfProcessDescriptor> _ImportedProcesses = new List<IWfProcessDescriptor>();

        public WfImportTemplateExecutor(Stream inputStream, Action<string> notifyEveryStep = null)
            : base(WfDesignerOperationType.ImportTemplate)
        {
            inputStream.NullCheck("inputStream");

            this._InputStream = inputStream;
            this._NotifyEveryStep = notifyEveryStep;
        }

        public Stream InputStream
        {
            get
            {
                return this._InputStream;
            }
        }

        public Action<string> NotifyEveryStep
        {
            get
            {
                return this._NotifyEveryStep;
            }
        }

        public List<IWfProcessDescriptor> ImportedProcesses
        {
            get
            {
                return this._ImportedProcesses;
            }
        }

        protected override void OnExecute(WfDesignerExecutorDataContext dataContext)
        {
            this._ImportedProcesses = WfProcessImporter.ImportProcessDescriptors(this.InputStream, this._NotifyEveryStep);

            dataContext["ImportedProcesses"] = this._ImportedProcesses;
        }

        protected override void OnSaveUserOperationLogs(WfDesignerExecutorDataContext dataContext, UserOperationLogCollection logs)
        {
            List<IWfProcessDescriptor> importedProcesses = dataContext.GetValue("ImportedProcesses", new List<IWfProcessDescriptor>());

            importedProcesses.ForEach(processDesp => logs.Add(this.CreateImportLog(processDesp)));

            this.FillEnvironmentInfoToLogs(logs);
            base.OnSaveUserOperationLogs(dataContext, logs);
        }

        private UserOperationLog CreateImportLog(IWfProcessDescriptor processDesp)
        {
            UserOperationLog log = new UserOperationLog();

            log.ResourceID = processDesp.Key;
            log.Subject = string.Format("{0}:{1}", this.OperationType.ToDescription(), processDesp.Key);

            return log;
        }
    }
}
