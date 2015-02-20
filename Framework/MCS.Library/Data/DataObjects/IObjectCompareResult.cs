using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MCS.Library.Data.DataObjects
{
    /// <summary>
    /// 对象比较的结果
    /// </summary>
    public interface IObjectCompareResult
    {
        /// <summary>
        /// 对象的类型名称
        /// </summary>
        string ObjectTypeName
        {
            get;
        }

        /// <summary>
        /// 对象的比较结果是否存在差异
        /// </summary>
        bool AreDifferent
        {
            get;
        }

        /// <summary>
        /// 参与比较的对象是否是可列举的（集合）
        /// </summary>
        bool AreEnumerable
        {
            get;
        }
    }
}
