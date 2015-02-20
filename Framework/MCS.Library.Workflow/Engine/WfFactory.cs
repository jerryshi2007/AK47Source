using System;
using System.Collections.Generic;
using System.Text;
using MCS.Library.Workflow.Descriptors;

namespace MCS.Library.Workflow.Engine
{
    /// <summary>
    /// 
    /// </summary>
	public class WfFactory : IWfFactory
	{
		#region IWfFactory ≥…‘±

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
		public IWfProcess CreateProcess()
		{
			return new WfProcess();
		}

        /// <summary>
        /// 
        /// </summary>
        /// <param name="process"></param>
        /// <param name="descriptor"></param>
        /// <returns></returns>
		public IWfActivity CreateActivity(IWfProcess process, IWfActivityDescriptor descriptor)
		{
			WfActivity activity = new WfActivity(descriptor);
			activity.Process = process;

			return activity;
		}

        /// <summary>
        /// 
        /// </summary>
        /// <param name="process"></param>
        /// <param name="transParams"></param>
        /// <returns></returns>
		public IWfActivity CreateActivity(IWfProcess process, WfTransferParams transParams)
		{
			WfActivity activity = new WfActivity(transParams.NextActivityDescriptor);
            activity.Operator = transParams.Operator;
			activity.Process = process;

			return activity;
		}

        /// <summary>
        /// 
        /// </summary>
        /// <param name="process"></param>
        /// <param name="descriptor"></param>
        /// <returns></returns>
		public IWfAnchorActivity CreateAnchorActivity(IWfProcess process, IWfAnchorActivityDescriptor descriptor)
		{
			WfAnchorActivity activity = new WfAnchorActivity(descriptor);
			activity.Process = process;

			return activity;
		}

        /// <summary>
        /// 
        /// </summary>
        /// <param name="process"></param>
        /// <param name="transParams"></param>
        /// <returns></returns>
		public IWfAnchorActivity CreateAnchorActivity(IWfProcess process, WfBranchesTransferParams transParams)
		{
            WfAnchorActivity activity = new WfAnchorActivity((IWfAnchorActivityDescriptor)transParams.NextActivityDescriptor);
			activity.Process = process;

			activity.Operations.Add(CreateOperation(activity, transParams));
            

			return activity;
		}

        /// <summary>
        /// 
        /// </summary>
        /// <param name="ownerActivity"></param>
        /// <param name="transferParams"></param>
        /// <returns></returns>
		public IWfOperation CreateOperation(IWfAnchorActivity ownerActivity, WfBranchesTransferParams transferParams)
		{
			WfOperation operation = new WfOperation(ownerActivity, transferParams);

			operation.AutoTransferReceivers.CopyFrom(transferParams.AutoTransferReceivers);

			return operation;
		}

		#endregion
	}
}
