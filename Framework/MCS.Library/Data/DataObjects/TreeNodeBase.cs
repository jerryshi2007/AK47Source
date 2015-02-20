using System;
using System.Text;
using System.Collections.Generic;
using MCS.Library.Core;
using MCS.Library.Data.Mapping;

namespace MCS.Library.Data.DataObjects
{
	/// <summary>
	/// ���ڵ�Ļ���
	/// </summary>
	/// <typeparam name="TNode">���ڵ������</typeparam>
	/// <typeparam name="TCollection">���ڵ�ļ�������</typeparam>
    [Serializable]
    [XElementSerializable]
	public abstract class TreeNodeBase<TNode, TCollection>
		where TNode : TreeNodeBase<TNode, TCollection>
		where TCollection : TreeNodeBaseCollection<TNode, TCollection>, new()
	{
		/// <summary>
		/// 
		/// </summary>
		/// <param name="node"></param>
		/// <param name="context"></param>
		/// <returns></returns>
		public delegate bool TraverseNodeHandler(TreeNodeBase<TNode, TCollection> node, object context);

		/// <summary>
		/// 
		/// </summary>
		/// <param name="node"></param>
		/// <param name="context"></param>
		/// <returns></returns>
		public delegate bool BeforeTraverseNodeHandler(TreeNodeBase<TNode, TCollection> node, object context);

		private TNode parent = null;
		private TNode nextSibling = null;
		private TNode previousSibing = null;
		private TCollection children = new TCollection();

		/// <summary>
		/// ���ڵ�
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
		/// ��һ���ֵܽڵ�
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
		/// ǰһ���ֵܽڵ�
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
		/// �ӽڵ㼯��
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
		/// ��һ���ӽڵ�
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
		/// ���һ���ӽڵ�
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
		/// ����������ȱ����ӽڵ�
		/// </summary>
		/// <param name="handler"></param>
		public void TraverseChildren(TraverseNodeHandler handler)
		{
			TraverseChildren(handler, null, null);
		}

		/// <summary>
		/// ����������ȱ����ӽڵ�
		/// </summary>
		/// <param name="handler"></param>
		/// <param name="context"></param>
		public void TraverseChildren(TraverseNodeHandler handler, object context)
		{
			TraverseChildren(handler, null, context);
		}

		/// <summary>
		/// ����������ȱ����ӽڵ�
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
					continued = handler(node, context);

				if (continued)
					node.TraverseChildren(handler, beforeHandler, context);
				else
					break;
			}
		}
	}
}
