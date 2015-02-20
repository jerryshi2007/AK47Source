using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MCS.Library.Core;

namespace MCS.Library.SOA.DataObjects.Workflow
{
    /// <summary>
    /// Activity相关的Executor的虚基类
    /// </summary>
    public abstract class WfActivityRelativeExecutorBase : WfExecutorBase
    {
        public IWfActivity TargetActivity
        {
            get;
            private set;
        }

        protected override IWfProcess OnGetCurrentProcess()
        {
            IWfProcess process = base.OnGetCurrentProcess();

            if (process == null && this.TargetActivity != null)
                process = this.TargetActivity.Process;

            return process;
        }

        protected WfActivityRelativeExecutorBase(IWfActivity operatorActivity, IWfActivity targetActivity, WfControlOperationType operationType) :
            base(operatorActivity, operationType)
        {
            targetActivity.NullCheck("targetActivity");
            TargetActivity = targetActivity;

            if (OperatorActivity == null)
                OperatorActivity = targetActivity;
        }

        protected static void AfterStartupBranchProcess(IWfProcess process)
        {
            StringBuilder strB = new StringBuilder();

            foreach (WfAssignee assignee in process.BranchStartupParams.Assignees)
            {
                if (strB.Length > 0)
                    strB.Append(",");

                strB.Append(assignee.User.DisplayName);
            }

            if (strB.Length > 0)
            {
                string opName = string.Empty;

                switch (process.Descriptor.Key)
                {
                    case WfProcessDescriptorManager.DefaultConsignProcessKey:
                        opName = EnumItemDescriptionAttribute.GetDescription(WfControlOperationType.Consign);
                        break;
                    case WfProcessDescriptorManager.DefaultCirculationProcessKey:
                        opName = EnumItemDescriptionAttribute.GetDescription(WfControlOperationType.Circulate);
                        break;
                }

                if (opName.IsNotEmpty())
                    opName = opName + ": " + strB.ToString();
                else
                    opName = strB.ToString();

                ((WfProcessDescriptor)process.Descriptor).Name = opName;
            }
        }
    }
}
