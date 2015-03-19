using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MCS.Library.SOA.DataObjects.Workflow
{
    /// <summary>
    /// 流程定义数据访问的接口
    /// </summary>
    public interface IWfProcessDescriptorManager
    {
        /// <summary>
        /// 加载流程模板
        /// </summary>
        /// <param name="processKey"></param>
        /// <returns></returns>
        IWfProcessDescriptor LoadDescriptor(string processKey);

        /// <summary>
        /// 读取流程模板。这里会试图从缓存中加载
        /// </summary>
        /// <param name="processKey"></param>
        /// <returns></returns>
        IWfProcessDescriptor GetDescriptor(string processKey);

        /// <summary>
        /// 删除流程模板
        /// </summary>
        /// <param name="processKey"></param>
        void DeleteDescriptor(string processKey);

        /// <summary>
        /// 保存流程模板
        /// </summary>
        /// <param name="processDesp"></param>
        void SaveDescriptor(IWfProcessDescriptor processDesp);

        /// <summary>
        /// 流程模板是否已经存在
        /// </summary>
        /// <param name="processKey"></param>
        /// <returns></returns>
        bool ExsitsProcessKey(string processKey);

        /// <summary>
        /// 清空所有流程模板
        /// </summary>
        void ClearAll();
    }
}
