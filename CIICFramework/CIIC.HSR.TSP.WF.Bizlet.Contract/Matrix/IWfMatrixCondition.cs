using CIIC.HSR.TSP.DataAccess.Query;
using CIIC.HSR.TSP.WF.Bizlet.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CIIC.HSR.TSP.WF.Bizlet.Contract
{
    /// <summary>
    /// 
    /// </summary>
    public interface IWfMatrixCondition : IWfMatrixConditionConvertor
    {
        /* 沈峥注释
        /// <summary>
        /// 唯一标识
        /// </summary>
        Guid Id { get; set; }
        */

        /// <summary>
        /// 变量
        /// </summary>
        IWfMatrixParameterDefinition Parameter { get; set; }

        /// <summary>
        /// 值
        /// </summary>
        string Value { get; set; }

        /// <summary>
        /// 比较符
        /// </summary>
        ComparsionSign Sign { get; set; }

        /*沈峥注释
         * 
        /// <summary>
        /// 序号
        /// </summary>
        int Sort { get; set; }
         */
    }
}
