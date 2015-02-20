using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CIIC.HSR.TSP.WF.Bizlet.Contract
{
    public interface IWfMatrixConditionConvertor
    {
        /// <summary>
        /// 转换为表达式
        /// </summary>
        /// <returns>转换为表达式</returns>
        string ToExpression();
    }
}
