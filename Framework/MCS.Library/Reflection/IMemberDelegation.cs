using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MCS.Library.Reflection
{
    /// <summary>
    /// 成员委托需要实现的接口
    /// </summary>
    public interface IMemberDelegation
    {
        /// <summary>
        /// 初始化委托信息
        /// </summary>
        void InitDelegations();
    }
}
