using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MCS.Library.SOA.Contracts
{
    /// <summary>
    /// 退件条件
    /// </summary>
    public class ReturnCondition
    {
        /// <summary>
        /// 表达式
        /// </summary>
        public string Exception { get; set; }
        /// <summary>
        /// Activity Key
        /// </summary>
        public string ActKey { get; set; }
    }

    public class ReturnConfig
    {
        public List<ReturnCondition> Conditions { get; set; }
    }
}
