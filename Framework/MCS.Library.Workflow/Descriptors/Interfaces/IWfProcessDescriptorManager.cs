using System;
using System.IO;
using System.Text;
using System.Collections.Generic;

namespace MCS.Library.Workflow.Descriptors
{
    /// <summary>
    /// ���غͱ���ProcessDescriptorManager�Ľӿڶ���
    /// </summary>
    public interface IWfProcessDescriptorManager
    {
        /// <summary>
        /// ����ProcessDescriptor���ڴ�
        /// </summary>
        /// <param name="processKey">Process Descriptor��Key</param>
        /// <returns></returns>
        IWfProcessDescriptor LoadProcessDescriptor(string processKey);

        /// <summary>
        /// ����ProcessDescriptor
        /// </summary>
        /// <param name="processKey">Process Descriptor��Key</param>
        /// <param name="processDesp">Process Descriptor</param>
        void SaveProcessDescriptor(string processKey, IWfProcessDescriptor processDesp);

		/// <summary>
		/// 
		/// </summary>
		/// <param name="processKey"></param>
		/// <returns></returns>
		IWfProcessDescriptor GetProcessDescriptor(string processKey);
    }

    /// <summary>
    /// �����������л���
    /// </summary>
    public interface IProcessDescriptorSerializer
    {
        /// <summary>
        /// ���л���������
        /// </summary>
        /// <param name="stream">������</param>
        /// <param name="descriptor">������������</param>
        void Serialize(Stream stream, IWfProcessDescriptor descriptor);

        /// <summary>
        /// ���������������л���������������
        /// </summary>
        /// <param name="stream">������</param>
        /// <returns>������������</returns>
        IWfProcessDescriptor Deserialize(Stream stream);
    }
}
