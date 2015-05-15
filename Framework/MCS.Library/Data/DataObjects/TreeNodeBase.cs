using System;
using System.Text;
using System.Collections.Generic;
using MCS.Library.Core;
using MCS.Library.Data.Mapping;

namespace MCS.Library.Data.DataObjects
{
    /// <summary>
    /// 树节点的基类
    /// </summary>
    /// <typeparam name="TNode">树节点的类型</typeparam>
    /// <typeparam name="TCollection">树节点的集合类型</typeparam>
    [Serializable]
    [XElementSerializable]
    public abstract class TreeNodeBase<TNode, TCollection>
        where TNode : TreeNodeBase<TNode, TCollection>
        where TCollection : TreeNodeBaseCollection<TNode, TCollection>, new()
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="parent"></param>
        /// <param name="node"></param>
        /// <param name="context"></param>
        /// <returns></returns>
        public delegate bool TraverseNodeHandler(TreeNodeBase<TNode, TCollection> parent, TreeNodeBase<TNode, TCollection> node, object context);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="parent"></param>
        /// <param name="context"></param>
        /// <returns></returns>
        public delegate bool BeforeTraverseNodeHandler(TreeNodeBase<TNode, TCollection> parent, object context);

        private TNode parent = null;
        private TNode nextSibling = null;
        private TNode previousSibing = null;
        private TCollection children = new TCollection();

        /// <summary>
        /// 父节点
        /// </summary>
        [NoMapping]
        public virtual TNode Parent
        {
            get
            {
                return this.parent;
            }
            internal set
            {
                this.parent = value;
            }
        }

        /// <summary>
        /// 下一个兄弟节点
        /// </summary>
        [NoMapping]
        public virtual TNode NextSibling
        {
            get
            {
                return this.nextSibling;
            }
            internal set
            {
                this.nextSibling = value;
            }
        }

        /// <summary>
        /// 前一个兄弟节点
        /// </summary>
        [NoMapping]
        public virtual TNode PreviousSibing
        {
            get
            {
                return this.previousSibing;
            }
            internal set
            {
                this.previousSibing = value;
            }
        }

        /// <summary>
        /// 子节点集合
        /// </summary>
        [NoMapping]
        public virtual TCollection Children
        {
            get
            {
                if (this.children.parent == null)
                    this.children.parent = (TNode)this;

                return this.children;
            }
        }

        /// <summary>
        /// 第一个子节点
        /// </summary>
        [NoMapping]
        public virtual TNode FirstChild
        {
            get
            {
                return this.Children.FirstNode;
            }
        }

        /// <summary>
        /// 最后一个子节点
        /// </summary>
        [NoMapping]
        public virtual TNode LastChild
        {
            get
            {
                return this.Children.LastNode;
            }
        }

        /// <summary>
        /// 按照深度优先遍历子节点
        /// </summary>
        /// <param name="handler"></param>
        public void TraverseChildren(TraverseNodeHandler handler)
        {
            TraverseChildren(handler, null, null);
        }

        /// <summary>
        /// 按照深度优先遍历子节点
        /// </summary>
        /// <param name="handler"></param>
        /// <param name="context"></param>
        public void TraverseChildren(TraverseNodeHandler handler, object context)
        {
            TraverseChildren(handler, null, context);
        }

        /// <summary>
        /// 按照深度优先遍历子节点
        /// </summary>
        /// /// <param name="beforeHandler"></param>
        /// <param name="handler"></param>
        /// <param name="context"></param>
        public void TraverseChildren(TraverseNodeHandler handler, BeforeTraverseNodeHandler beforeHandler, object context)
        {
            bool continued = true;

            if (beforeHandler != null)
                continued = beforeHandler(this, context);

            if (!continued)
                return;

            foreach (TNode node in children)
            {
                if (handler != null)
                    continued = handler(this, node, context);

                if (continued)
                    node.TraverseChildren(handler, beforeHandler, context);
                else
                    break;
            }
        }

        /// <summary>
        /// 生成图形化的树型图
        /// </summary>
        /// <returns></returns>
        public TreeGraph GenerateGraph()
        {
            TreeGraphLabelNode originalGraphNode = GenerateGraphLabelNode((TNode)this, 0, 0, this.CalculateMaxWidth() * 2);

            TraverseTreeGraphContext context = new TraverseTreeGraphContext();

            return new TreeGraph()
            {
                Root = GenerateGraphNodeWithConnectors(null, originalGraphNode, context),
                Width = context.Width,
                Height = context.Height
            };
        }

        /// <summary>
        /// 计算节点的最大深度
        /// </summary>
        /// <returns></returns>
        public int CalculateMaxLevel()
        {
            int result = 0;

            if (this is ITreeNodeSize)
                result = this.CalculateChildrenMaxLevelWithSize();
            else
                result = this.CalculateChildrenMaxLevel() + 1;

            return result;
        }

        private int CalculateChildrenMaxLevelWithSize()
        {
            ITreeNodeSize size = (ITreeNodeSize)this;

            if (size.MaxLevel == 0)
            {
                if (this.Children.Count == 0)
                {
                    size.MaxLevel = 1;
                }
                else
                {
                    foreach (TNode child in this.Children)
                    {
                        int childrenLevel = child.CalculateChildrenMaxLevelWithSize() + 1;

                        size.MaxLevel = Math.Max(size.MaxLevel, childrenLevel);
                    }
                }
            }

            return size.MaxLevel;
        }

        private int CalculateChildrenMaxLevel()
        {
            int result = 0;

            foreach (TNode child in this.Children)
            {
                int childrenLevel = child.CalculateChildrenMaxLevel() + 1;

                result = Math.Max(result, childrenLevel);
            }

            return result;
        }

        /// <summary>
        /// 计算节点的最大宽度
        /// </summary>
        /// <returns></returns>
        public int CalculateMaxWidth()
        {
            int result = 0;

            if (this is ITreeNodeSize)
                result = this.CalculateChildrenMaxWidthWithSize();
            else
                result = Math.Max(1, this.CalculateChildrenMaxWidth());

            return result;
        }

        private int CalculateChildrenMaxWidth()
        {
            int result = this.Children.Count;
            int childrenWidth = 0;

            foreach (TNode child in this.Children)
                childrenWidth += child.CalculateChildrenMaxWidth();

            return Math.Max(result, childrenWidth);
        }

        private int CalculateChildrenMaxWidthWithSize()
        {
            ITreeNodeSize size = (ITreeNodeSize)this;

            if (size.MaxWidth == 0)
            {
                if (this.Children.Count == 0)
                {
                    size.MaxWidth = 1;
                }
                else
                {
                    int childrenWidth = 0;

                    foreach (TNode child in this.Children)
                        childrenWidth += child.CalculateChildrenMaxWidthWithSize();

                    size.MaxWidth = Math.Max(size.MaxWidth, childrenWidth);
                }
            }

            return size.MaxWidth;
        }

        private TreeGraphLabelNode GenerateGraphLabelNode(TNode node, int startX, int startY, int width)
        {
            TreeGraphLabelNode label = new TreeGraphLabelNode(startX + width / 2, startY);

            label.CopyNameInfo(node as INamedTreeNode);

            int childStartX = startX;

            foreach (TNode child in node.Children)
            {
                int childWidth = child.CalculateMaxWidth() * 2;

                TreeGraphLabelNode childLabel = GenerateGraphLabelNode(child, childStartX, startY + 2, childWidth);

                childStartX += childWidth;

                label.Children.Add(childLabel);
            }

            return label;
        }

        private static TreeGraphLabelNode GenerateGraphNodeWithConnectors(TreeGraphNodeBase parentGraphNode, TreeGraphLabelNode originalGraphNode, TraverseTreeGraphContext context)
        {
            TreeGraphLabelNode resutLabelNode = new TreeGraphLabelNode() { Name = originalGraphNode.Name, Description = originalGraphNode.Description, X = originalGraphNode.X, Y = originalGraphNode.Y };

            context.Width = Math.Max(context.Width, resutLabelNode.X + 1);
            context.Height = Math.Max(context.Height, resutLabelNode.Y + 1);

            if (parentGraphNode != null)
                parentGraphNode.Children.Add(resutLabelNode);

            FillGraphConnectorNodes(originalGraphNode, resutLabelNode, context);

            foreach (TreeGraphNodeBase childConnectorNode in resutLabelNode.Children)
            {
                TreeGraphLabelNode matchedLabelNode = (TreeGraphLabelNode)originalGraphNode.Children.Find(n => n.X == childConnectorNode.X);

                if (matchedLabelNode != null)
                    GenerateGraphNodeWithConnectors(childConnectorNode, matchedLabelNode, context);
            }

            return resutLabelNode;
        }

        private static void FillGraphConnectorNodes(TreeGraphLabelNode originalGraphNode, TreeGraphLabelNode resutLabelNode, TraverseTreeGraphContext context)
        {
            if (originalGraphNode.Children.Count > 0)
            {
                int startX = originalGraphNode.Children[0].X;
                int endX = originalGraphNode.Children[originalGraphNode.Children.Count - 1].X;

                for (int i = startX; i <= endX; i++)
                {
                    TreeGraphConnectorNode connector = null;

                    TreeGraphNodeBase child = originalGraphNode.Children.Find(c => c.X == i);

                    if (child != null)
                    {
                        if (originalGraphNode.Children.Count == 1)
                        {
                            connector = new TreeGraphConnectorNode(TreeGraphNodeType.VLine, i, originalGraphNode.Y + 1);
                        }
                        else
                        {
                            if (child == originalGraphNode.Children[0])
                                connector = new TreeGraphConnectorNode(TreeGraphNodeType.LeftCorner, i, originalGraphNode.Y + 1);
                            else
                                if (child == originalGraphNode.Children[originalGraphNode.Children.Count - 1])
                                    connector = new TreeGraphConnectorNode(TreeGraphNodeType.RightCorner, i, originalGraphNode.Y + 1);
                                else
                                    if (child.X == originalGraphNode.X)
                                        connector = new TreeGraphConnectorNode(TreeGraphNodeType.Cross, i, originalGraphNode.Y + 1);
                                    else
                                        connector = new TreeGraphConnectorNode(TreeGraphNodeType.Tee, i, originalGraphNode.Y + 1);
                        }
                    }
                    else
                    {
                        if (i == originalGraphNode.X)
                            connector = new TreeGraphConnectorNode(TreeGraphNodeType.ReverseTee, i, originalGraphNode.Y + 1);
                        else
                            connector = new TreeGraphConnectorNode(TreeGraphNodeType.HLine, i, originalGraphNode.Y + 1);
                    }

                    context.Width = Math.Max(context.Width, connector.X + 1);
                    context.Height = Math.Max(context.Height, connector.Y + 1);

                    resutLabelNode.Children.Add(connector);
                }
            }
        }
    }
}
