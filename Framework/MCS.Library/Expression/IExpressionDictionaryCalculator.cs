using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MCS.Library.Expression
{
    /// <summary>
    /// 表达式字典需要计算需要实现的接口
    /// </summary>
    public interface IExpressionDictionaryCalculator
    {
        /// <summary>
        /// 计算并返回字典项的值
        /// </summary>
        /// <param name="dictionaryName">字典的名称</param>
        /// <param name="key">字典的Key</param>
        /// <param name="context">计算上下文</param>
        /// <returns></returns>
        object Calculate(string dictionaryName, string key, ExpressionDictionaryCalculatorContext context);
    }
}
