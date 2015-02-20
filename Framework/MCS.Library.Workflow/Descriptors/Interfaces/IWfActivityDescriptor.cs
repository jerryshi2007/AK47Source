using System;
using System.Collections.Generic;
using System.Text;

namespace MCS.Library.Workflow.Descriptors
{
    /// <summary>
    /// 
    /// </summary>
    public interface IWfActivityDescriptor : IWfDescriptor
    {
        /// <summary>
        /// 
        /// </summary>
        IWfProcessDescriptor Process
        {
            get;
        }

        /// <summary>
        /// 
        /// </summary>
        FromTransitionsDescriptorCollection FromTransitions
        {
            get;
        }

        /// <summary>
        /// 
        /// </summary>
        ToTransitionsDescriptorCollection ToTransitions
        {
            get;
        }

		/// <summary>
		/// 
		/// </summary>
		/// <param name="activityDespKey"></param>
		/// <returns></returns>
		bool CanReachTo(string activityDespKey);

		/// <summary>
		/// 
		/// </summary>
		/// <param name="activityDespKey"></param>
		/// <returns></returns>
		bool CanReachTo(IWfActivityDescriptor targetAct);

		/// <summary>
        /// 
        /// </summary>
        WfResourceDescriptorCollection Resources
        {
            get;
        }

		/// <summary>
		/// 流程对应的环节名称(多个节点可能对应一个环节名称)
		/// </summary>
		string LevelName
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
        WfExtendedPropertyDictionary ExtendedProperties
        {
            get;
        }

		IWfActivityDescriptor CloneDescriptor();
    }

    /// <summary>
    /// 
    /// </summary>
    public interface IWfInitialActivityDescriptor : IWfActivityDescriptor
    {
    }

    /// <summary>
    /// 
    /// </summary>
    public interface IWfCompletedActivityDescriptor : IWfActivityDescriptor
    {
    }

    /// <summary>
    /// 
    /// </summary>
    public interface IWfAnchorActivityDescriptor : IWfActivityDescriptor
    {
        WfOperationDescriptorCollection Operations
        {
            get;
        }
    }

    public interface IWfRegularActivityDescriptor : IWfActivityDescriptor
    {
    }
}
