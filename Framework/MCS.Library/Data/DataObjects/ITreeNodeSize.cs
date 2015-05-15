using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MCS.Library.Data.DataObjects
{
    /// <summary>
    /// 带尺寸的树节点所需要实现的接口
    /// </summary>
    public interface ITreeNodeSize
    {
        /// <summary>
        /// 最大的宽度
        /// </summary>
        int MaxWidth
        {
            get;
            set;
        }

        /// <summary>
        /// 最大的深度
        /// </summary>
        int MaxLevel
        {
            get;
            set;
        }
    }
}
