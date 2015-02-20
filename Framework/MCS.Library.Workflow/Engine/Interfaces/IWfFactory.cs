using System;
using System.Text;
using System.Collections.Generic;
using MCS.Library.Workflow.Descriptors;

namespace MCS.Library.Workflow.Engine
{
	/// <summary>
	/// ��������ʵ������Ĺ�����
	/// </summary>
	public interface IWfFactory
	{
		/// <summary>
		/// ��������
		/// </summary>
		/// <returns></returns>
		IWfProcess CreateProcess();

		/// <summary>
		/// 
		/// </summary>
		/// <param name="process"></param>
		/// <returns></returns>
		IWfActivity CreateActivity(IWfProcess process, IWfActivityDescriptor descriptor);

		/// <summary>
		/// 
		/// </summary>
		/// <param name="process"></param>
		/// <param name="transParams"></param>
		/// <returns></returns>
		IWfActivity CreateActivity(IWfProcess process, WfTransferParams transParams);

		/// <summary>
		/// 
		/// </summary>
		/// <param name="process"></param>
		/// <returns></returns>
		IWfAnchorActivity CreateAnchorActivity(IWfProcess process, IWfAnchorActivityDescriptor descriptor);
		
		/// <summary>
		/// 
		/// </summary>
		/// <param name="process"></param>
		/// <param name="transParams"></param>
		/// <returns></returns>
		IWfAnchorActivity CreateAnchorActivity(IWfProcess process, WfBranchesTransferParams transParams);

		/// <summary>
		/// 
		/// </summary>
		/// <param name="ownerActivity"></param>
		/// <param name="transferParams"></param>
		/// <returns></returns>
		IWfOperation CreateOperation(IWfAnchorActivity ownerActivity, WfBranchesTransferParams transferParams);
	}
}
