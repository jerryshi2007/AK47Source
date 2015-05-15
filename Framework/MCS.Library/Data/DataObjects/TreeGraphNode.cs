using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MCS.Library.Data.DataObjects
{
    /// <summary>
    /// 树形图的节点基类
    /// </summary>
    [Serializable]
    public abstract class TreeGraphNodeBase : TreeNodeBase<TreeGraphNodeBase, TreeGraphNodeCollection>
    {
        /// <summary>
        /// 构造方法
        /// </summary>
        public TreeGraphNodeBase()
        {
        }

        /// <summary>
        /// 构造方法
        /// </summary>
        /// <param name="nodeType">节点类型</param>
        public TreeGraphNodeBase(TreeGraphNodeType nodeType)
        {
            this.NodeType = nodeType;
        }

        /// <summary>
        /// 构造方法
        /// </summary>
        /// <param name="nodeType"></param>
        /// <param name="x"></param>
        /// <param name="y"></param>
        public TreeGraphNodeBase(TreeGraphNodeType nodeType, int x, int y) :
            this(nodeType)
        {
            this.X = x;
            this.Y = y;
        }

        /// <summary>
        /// 节点的类型
        /// </summary>
        public TreeGraphNodeType NodeType
        {
            get;
            protected set;
        }

        /// <summary>
        /// 节点的横坐标，向右为正
        /// </summary>
        public int X
        {
            get;
            set;
        }

        /// <summary>
        /// 节点的纵坐标，向下为正
        /// </summary>
        public int Y
        {
            get;
            set;
        }

        /// <summary>
        /// 节点的标签
        /// </summary>
        public object Tag
        {
            get;
            set;
        }
    }

    /// <summary>
    /// 树形图节点的集合
    /// </summary>
    [Serializable]
    public class TreeGraphNodeCollection : TreeNodeBaseCollection<TreeGraphNodeBase, TreeGraphNodeCollection>
    {
    }
}
