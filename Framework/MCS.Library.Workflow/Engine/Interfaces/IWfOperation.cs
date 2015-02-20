using System;
using System.Collections.Generic;
using System.Text;
using MCS.Library.Workflow.Descriptors;

namespace MCS.Library.Workflow.Engine
{
    public interface IWfOperation
    {
        string ID
        {
            get;
        }

        IWfOperationDescriptor Descriptor
        {
            get;
        }

        IWfAnchorActivity AnchorActivity
        {
            get;
        }

        BranchesOperationalType OperationalType
        {
            get;
        }

        WfBranchProcessInfoCollection Branches
        {
            get;
        }

        State OpState
        {
            get;
        }

		WfOperationContext Context
		{
			get;
		}

		DataLoadingType LoadingType
		{
			get;
		}

		WfAssigneeCollection AutoTransferReceivers
		{
			get;
		}

        void AdjustBranches(WfAdjustBranchesParams adjustParams);
    }
}
