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
		/// ��֧���̽�����ģ�Anchor���ܹ���ת����һ����
		/// </summary>
		IWfActivityDescriptor NextAutoTransferActivityDescriptor
		{
			get;
		}

		/// <summary>
		/// ��֧���̽�����ģ�Anchor�Զ����صĵ��Ƿ�����һ����
		/// </summary>
		bool AutoReturnToPreviousActivity
		{
			get;
		}

		/// <summary>
		/// �����µ�Operation
		/// </summary>
		/// <param name="transferParams"></param>
        IWfOperation AddNewOperation(WfBranchesTransferParams transferParams);

		/// <summary>
		/// ɾ��Operation
		/// </summary>
		/// <param name="operations"></param>
		void RemoveOperations(params IWfOperation[] operations);
    }
}
