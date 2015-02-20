using MCS.Library.WF.Contracts.Workflow.Descriptors;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CIIC.HSR.TSP.WF.Bizlet.Contract
{
    /// <summary>
    /// 业务流程描述转换器
    /// </summary>
    public interface IWfMatrixDescriptorTransformation
    {
        /// <summary>
        /// 将业务描述转换为流程引擎的流程描述
        /// </summary>
        /// <param name="process">业务流程描述</param>
        /// <returns>流程引擎流程描述</returns>
        WfClientProcessDescriptor Transform(IWfMatrixProcess process);

        /// <summary>
        /// 从流程引擎中找回流程描述
        /// </summary>
        /// <param name="key">流程Key</param>
        /// <returns>流程描述</returns>
        IWfMatrixProcess TransformBack(WfClientProcessDescriptor clientProcessDescriptor);
    }
}
