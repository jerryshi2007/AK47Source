using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using System.Collections.Specialized;

namespace MCS.Library.Workflow.Engine
{
    public interface IEnqueueWorkItem
    {
        void EnqueueInitialWfWorkItem(IWfProcess process);

        void EnqueueMoveToWorkItem(IWfActivity originalActivity, IWfActivity destinationActivity);

        void EnqueueAddNewOperationWorkItem(IWfOperation operation);

		void EnqueueAdjustBranchesWorkItem(IWfOperation operation, WfBranchProcessInfoCollection deletedBranchProcesses);

		void EnqueueRemoveOperationsWorkItem(params IWfOperation[] operations);

        void EnqueueWithdrawWorkItem(IWfActivity destinationActivity, WfActivityCollection deletedActivities, WfProcessCollection deletedProcesses);

		void EnqueueCancelProcessWorkItem(IWfProcess process);
    }

}
