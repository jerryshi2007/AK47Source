using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CIIC.HSR.TSP.WF.Bizlet.Contract
{
    /// <summary>
    /// 流程绑定的人员或其他资源集合
    /// </summary>
    public interface IWfMatrixCandidateCollection : IList<IWfMatrixCandidate>, IWfMatrixConditionConvertor
    {
        /*沈峥注释
        /// <summary>
        /// 删除一个动态角色
        /// </summary>
        /// <param name="id">动态角色Id</param>
        void Remove(Guid id);

        /// <summary>
        /// 根据Id获取动态角色
        /// </summary>
        /// <param name="id">动态角色Id</param>
        IWfMatrixCandidate GetById(Guid id);
         */
    }
}
