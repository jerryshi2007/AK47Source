using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MCS.Library.Data.DataObjects
{
    /// <summary>
    /// 树型图的标签节点
    /// </summary>
    [Serializable]
    public class TreeGraphLabelNode : TreeGraphNodeBase, INamedTreeNode
    {
        /// <summary>
        /// 构造方法
        /// </summary>
        public TreeGraphLabelNode()
            : base(TreeGraphNodeType.Label)
        {
        }

        /// <summary>
        /// 构造方法
        /// </summary>
        /// <param name="name"></param>
        public TreeGraphLabelNode(string name)
            : base(TreeGraphNodeType.Label)
        {
            this.Name = name;
        }

        /// <summary>
        /// 构造方法
        /// </summary>
        /// <param name="name"></param>
        /// <param name="x"></param>
        /// <param name="y"></param>
        public TreeGraphLabelNode(string name, int x, int y)
            : base(TreeGraphNodeType.Label, x, y)
        {
            this.Name = name;
        }

        /// <summary>
        /// 构造方法
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        public TreeGraphLabelNode(int x, int y)
            : base(TreeGraphNodeType.Label, x, y)
        {
        }

        /// <summary>
        /// 节点名称
        /// </summary>
        public string Name
        {
            get;
            set;
        }

        /// <summary>
        /// 节点的描述
        /// </summary>
        public string Description
        {
            get;
            set;
        }

        /// <summary>
        /// 从带名称描述的节点复制信息
        /// </summary>
        /// <param name="namedNode"></param>
        public void CopyNameInfo(INamedTreeNode namedNode)
        {
            if (namedNode != null)
            {
                this.Name = namedNode.Name;
                this.Description = namedNode.Description;
            }
        }
    }
}
