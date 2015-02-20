using CIIC.HSR.TSP.WF.Bizlet.Common.Metrix;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CIIC.HSR.TSP.WF.Bizlet.Contract
{
    /// <summary>
    /// 候选人
    /// </summary>
    public interface IWfMatrixCandidate : IWfMatrixConditionConvertor
    {
        /* 沈峥注释
        /// <summary>
        /// 唯一标识
        /// </summary>
        Guid ID { get; set; }
        */

        /// <summary>
        /// 资源类型
        /// </summary>
        string ResourceType { get; set; }

        /// <summary>
        /// 相关的动态角色
        /// </summary>
        IWfMatrixParameterDefinition Candidate { get; set; }
    }
}
