using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MCS.Library.Core;
using MCS.Library.OGUPermission;

namespace MCS.Library.SOA.DataObjects.Workflow
{
    public class WfCirculateExecutor : WfMoveToExecutorBase
    {
        public IEnumerable<IUser> Circulators
        {
            get;
            private set;
        }

        public WfCirculateExecutor(IWfActivity operatorActivity,
                                IWfActivity targetActivity,
                                IEnumerable<IUser> circulators)
            : base(operatorActivity, targetActivity, WfControlOperationType.Circulate)
        {
            circulators.NullCheck("circulators");

            this.Circulators = circulators;
        }

        protected override void OnModifyWorkflow(WfExecutorDataContext dataContext)
        {
            this.TargetActivity.Process.Committed = true;

            WfBranchProcessTransferParams bptp = new WfBranchProcessTransferParams(
                WfTemplateBuilder.CreateDefaultCirculationTemplate("Circulate", this.Circulators));

            WfRuntime.ProcessContext.AfterStartupBranchProcess += new WfAfterStartupBranchProcessHandler(WfActivityRelativeExecutorBase.AfterStartupBranchProcess);

            this.TargetActivity.StartupBranchProcesses(bptp);
        }
    }
}
