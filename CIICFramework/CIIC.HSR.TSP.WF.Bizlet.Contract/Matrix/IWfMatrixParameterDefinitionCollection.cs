using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CIIC.HSR.TSP.WF.Bizlet.Contract
{
    /// <summary>
    /// 所有的变量定义
    /// </summary>
    public interface IWfMatrixParameterDefinitionCollection : IList<IWfMatrixParameterDefinition>
    {
        /// <summary>
        /// 获取可用的参数定义
        /// </summary>
        List<IWfMatrixParameterDefinition> GetEnabledDefinitions();

        /* 沈峥注释
        /// <summary>
        /// 根据Id获取参数定义
        /// </summary>
        /// <param name="id">Id</param>
        /// <returns>参数定义信息</returns>
        IWfMatrixParameterDefinition GetById(Guid id);

        /// <summary>
        /// 删除变量定义
        /// </summary>
        /// <param name="id">变量名</param>
        void DeleteDefinition(Guid id);
        */
    }
}
