using MCS.Library.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MCS.Library.Expression
{
    /// <summary>
    /// 字典项计算的上下文对象
    /// </summary>
    public class ExpressionDictionaryCalculatorContext
    {
        /// <summary>
        /// 构造方法
        /// </summary>
        /// <param name="dictionaryInfo"></param>
        /// <param name="callerContext"></param>
        internal ExpressionDictionaryCalculatorContext(ExpressionDictionary dictionaryInfo, object callerContext)
        {
            dictionaryInfo.NullCheck("dictionaryInfo");

            this.DictionaryInfo = dictionaryInfo;
            this.CallerContext = callerContext;
        }

        /// <summary>
        /// 对应的配置元素
        /// </summary>
        public ExpressionDictionary DictionaryInfo
        {
            get;
            private set;
        }

        /// <summary>
        /// 表达式计算的调用上下文
        /// </summary>
        public object CallerContext
        {
            get;
            private set;
        }
    }
}
