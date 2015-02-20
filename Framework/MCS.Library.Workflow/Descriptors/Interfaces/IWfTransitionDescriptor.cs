using System;
using System.Collections.Generic;
using System.Text;

namespace MCS.Library.Workflow.Descriptors
{
    /// <summary>
    /// 
    /// </summary>
    public interface IWfTransitionDescriptor : IWfDescriptor, IComparable<IWfTransitionDescriptor>
    {
        /// <summary>
        /// 
        /// </summary>
        int Priority
        {
            get;
        }

        /// <summary>
        /// 
        /// </summary>
        IWfActivityDescriptor ToActivity
        {
            get;
        }

        /// <summary>
        /// 
        /// </summary>
        IWfActivityDescriptor FromActivity
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
        /// 该路径是够有效
        /// </summary>
        /// <returns></returns>
        bool CanTransit();

		/// <summary>
		/// 默认选择
		/// </summary>
		bool DefaultSelect { get; set; }
    }

    /// <summary>
    /// 
    /// </summary>
    public interface IWfBackwardTransitionDescriptor : IWfTransitionDescriptor
    {
    }

    /// <summary>
    /// 
    /// </summary>
    public interface IWfForwardTransitionDescriptor : IWfTransitionDescriptor
    {
        WfConditionDescriptor Condition
        {
            get;
        }
    }
}
