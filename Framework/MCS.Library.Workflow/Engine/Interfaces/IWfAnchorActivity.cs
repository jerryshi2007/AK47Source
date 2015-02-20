using System;
using System.Text;
using System.Collections.Generic;
using MCS.Library.Workflow.Descriptors;

namespace MCS.Library.Workflow.Engine
{
    /// <summary>
    /// 
    /// </summary>
    public interface IWfAnchorActivity : IWfActivity
    {
        /// <summary>
        /// 
        /// </summary>
        WfOperationCollection Operations
        {
            get;
        }

		/// <summary>
		/// 分支流程结束后的，Anchor点能够流转的下一个点
		/// </summary>
		IWfActivityDescriptor NextAutoTransferActivityDescriptor
		{
			get;
		}

		/// <summary>
		/// 分支流程结束后的，Anchor自动返回的点是否是上一个点
		/// </summary>
		bool AutoReturnToPreviousActivity
		{
			get;
		}

		/// <summary>
		/// 增加新的Operation
		/// </summary>
		/// <param name="transferParams"></param>
        IWfOperation AddNewOperation(WfBranchesTransferParams transferParams);

		/// <summary>
		/// 删除Operation
		/// </summary>
		/// <param name="operations"></param>
		void RemoveOperations(params IWfOperation[] operations);
    }
}
