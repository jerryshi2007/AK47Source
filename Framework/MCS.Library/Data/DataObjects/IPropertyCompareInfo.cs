using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MCS.Library.Data.DataObjects
{
    /// <summary>
    /// 对象属性的比较信息
    /// </summary>
    public interface IPropertyCompareInfo
    {
        /// <summary>
        /// 是否需要进行比较
        /// </summary>
        bool RequireCompare
        {
            get;
            set;
        }

        /// <summary>
        /// 展示修改信息时的排序
        /// </summary>
        int SortID
        {
            get;
            set;
        }

        /// <summary>
        /// 对该属性的文字描述
        /// </summary>
        string Description
        {
            get;
            set;
        }
    }
}
