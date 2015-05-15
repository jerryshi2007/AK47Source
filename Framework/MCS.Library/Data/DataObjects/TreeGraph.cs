using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MCS.Library.Data.DataObjects
{
    /// <summary>
    /// 树型图
    /// </summary>
    [Serializable]
    public class TreeGraph
    {
        /// <summary>
        /// 根节点
        /// </summary>
        public TreeGraphLabelNode Root
        {
            get;
            set;
        }

        /// <summary>
        /// 宽度
        /// </summary>
        public int Width
        {
            get;
            set;
        }

        /// <summary>
        /// 高度
        /// </summary>
        public int Height
        {
            get;
            set;
        }
    }
}
