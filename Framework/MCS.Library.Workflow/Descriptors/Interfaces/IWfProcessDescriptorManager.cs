using System;
using System.IO;
using System.Text;
using System.Collections.Generic;

namespace MCS.Library.Workflow.Descriptors
{
    /// <summary>
    /// 加载和保存ProcessDescriptorManager的接口定义
    /// </summary>
    public interface IWfProcessDescriptorManager
    {
        /// <summary>
        /// 加载ProcessDescriptor到内存
        /// </summary>
        /// <param name="processKey">Process Descriptor的Key</param>
        /// <returns></returns>
        IWfProcessDescriptor LoadProcessDescriptor(string processKey);

        /// <summary>
        /// 保存ProcessDescriptor
        /// </summary>
        /// <param name="processKey">Process Descriptor的Key</param>
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
    /// 流程描述序列化器
    /// </summary>
    public interface IProcessDescriptorSerializer
    {
        /// <summary>
        /// 序列化流程描述
        /// </summary>
        /// <param name="stream">流对象</param>
        /// <param name="descriptor">流程描述对象</param>
        void Serialize(Stream stream, IWfProcessDescriptor descriptor);

        /// <summary>
        /// 从流程描述反序列化成流程描述对象
        /// </summary>
        /// <param name="stream">流对象</param>
        /// <returns>流程描述对象</returns>
        IWfProcessDescriptor Deserialize(Stream stream);
    }
}
