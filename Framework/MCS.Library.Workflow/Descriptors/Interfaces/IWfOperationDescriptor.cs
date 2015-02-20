using System;
using System.Collections.Generic;
using System.Text;

namespace MCS.Library.Workflow.Descriptors
{
    public interface IWfOperationDescriptor : IWfDescriptor
    {
        /// <summary>
        /// 
        /// </summary>
        BranchesOperationalType OperationalType
        {
            get;
        }

        /// <summary>
        /// 
        /// </summary>
        string DefaultBranchProcessDescKey
        {
            get;
        }

        /// <summary>
        /// 
        /// </summary>
		AnchorOperationCompleteCondition CompleteCondition
		{
			get;
			set;
		}

        /// <summary>
        /// 
        /// </summary>
        WfResourceDescriptorCollection Resources
        {
            get;
        }

        /// <summary>
        /// 
        /// </summary>
        WfVariableDescriptorCollection Variables
        {
            get;
        }

        /// <summary>
        /// 
        /// </summary>
        bool AutoTransferWhenCompleted
        {
            get;
        }
    }
}
