using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MCS.Library.Data.DataObjects
{
    /// <summary>
    /// 对象比较信息的接口
    /// </summary>
    public interface IObjectCompareInfo
    {
        /// <summary>
        /// 需要比较的字段，可以还是多个字段，由逗号或分号分隔
        /// </summary>
        string KeyFields
        {
            get;
            set;
        }

        /// <summary>
        /// 是否为List
        /// </summary>
        bool IsList
        {
            get;
            set;
        }
    }
}
