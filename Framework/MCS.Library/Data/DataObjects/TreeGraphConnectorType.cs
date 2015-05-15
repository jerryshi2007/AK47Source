using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MCS.Library.Data.DataObjects
{
    /// <summary>
    /// 连接器类型
    /// </summary>
    public enum TreeGraphNodeType
    {
        /// <summary>
        /// 标签
        /// </summary>
        Label,

        /// <summary>
        /// 左边角
        /// </summary>
        LeftCorner,

        /// <summary>
        /// 右边角
        /// </summary>
        RightCorner,

        /// <summary>
        /// 十字
        /// </summary>
        Cross,

        /// <summary>
        /// T型，竖线向下
        /// </summary>
        Tee,

        /// <summary>
        /// 反T型，竖线向上
        /// </summary>
        ReverseTee,

        /// <summary>
        /// 横线
        /// </summary>
        HLine,

        /// <summary>
        /// 竖线
        /// </summary>
        VLine
    }
}
