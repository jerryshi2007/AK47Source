using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MCS.Library.Data.DataObjects
{
    /// <summary>
    /// 参照TypeCode枚举值
    /// </summary>
    public enum ColumnDataType
    {
        /// <summary>
        /// 
        /// </summary>
        DataObject = 1,

        /// <summary>
        /// 
        /// </summary>
        Boolean = 3,

        /// <summary>
        /// 
        /// </summary>
        Integer = 9,

        /// <summary>
        /// 
        /// </summary>
        Decimal = 15,

        /// <summary>
        /// 
        /// </summary>
        DateTime = 16,

        /// <summary>
        /// 
        /// </summary>
        String = 18,

        /// <summary>
        /// 
        /// </summary>
        Enum = 20
    }
}
