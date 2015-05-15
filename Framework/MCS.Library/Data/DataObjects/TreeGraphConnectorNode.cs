using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MCS.Library.Data.DataObjects
{
    /// <summary>
    /// 连线节点
    /// </summary>
    [Serializable]
    public class TreeGraphConnectorNode : TreeGraphNodeBase
    {
        /// <summary>
        /// 构造方法
        /// </summary>
        /// <param name="nodeType"></param>
        public TreeGraphConnectorNode(TreeGraphNodeType nodeType)
            : base(nodeType)
        {
        }

        /// <summary>
        /// 构造方法
        /// </summary>
        /// <param name="nodeType"></param>
        /// <param name="x"></param>
        /// <param name="y"></param>
        public TreeGraphConnectorNode(TreeGraphNodeType nodeType, int x, int y)
            : base(nodeType, x, y)
        {
        }
    }
}
