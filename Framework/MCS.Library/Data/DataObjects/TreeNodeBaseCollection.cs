using System;
using System.Text;
using System.Collections;
using System.Collections.Generic;
using MCS.Library.Core;

namespace MCS.Library.Data.DataObjects
{
	/// <summary>
	/// 树节点的集合
	/// </summary>
	/// <typeparam name="TNode">集合中元素的类型</typeparam>
	/// <typeparam name="TCollection">集合的类型</typeparam>
    [Serializable]
    [XElementSerializable]
	public abstract class TreeNodeBaseCollection<TNode, TCollection> : DataObjectCollectionBase<TNode>
		where TNode : TreeNodeBase<TNode, TCollection>
		where TCollection : TreeNodeBaseCollection<TNode, TCollection>, new()
	{
		internal TNode parent = null;

		/// <summary>
		/// 构造方法
		/// </summary>
		public TreeNodeBaseCollection()
		{
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="node"></param>
        public TNode Add(TNode node)
		{
			InnerAdd(node);

            return node;
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="node"></param>
        public TNode Remove(TNode node)
		{
			ExceptionHelper.FalseThrow<ArgumentNullException>(node != null, "node");

			List.Remove(node);

            return node;
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="index"></param>
		/// <returns></returns>
		public TNode this[int index]
		{
			get
			{
				return (TNode)List[index];
			}
			set
			{
				List[index] = value;

				TNode prev = value.PreviousSibing;
				TNode next = value.NextSibling;

				value.PreviousSibing = prev;
				value.NextSibling = next;

				if (prev != null)
					prev.NextSibling = value;

				if (next != null)
					next.PreviousSibing = value;
			}
		}

		internal TNode FirstNode
		{
			get
			{
				TNode node = null;

				if (this.Count > 0)
					node = this[0];

				return node;
			}
		}

		internal TNode LastNode
		{
			get
			{
				TNode node = null;

				if (this.Count > 0)
					node = this[this.Count - 1];

				return node;
			}
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="node"></param>
		protected override void InnerAdd(TNode node)
		{
			base.InnerAdd(node);

			TNode lastNode = LastNode;

			if (lastNode != null)
			{
				node.PreviousSibing = lastNode;
				lastNode.NextSibling = node;
			}
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="index"></param>
		/// <param name="value"></param>
		protected override void OnInsert(int index, object value)
		{
			((TNode)value).Parent = this.parent;

			base.OnInsert(index, value);
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="index"></param>
		/// <param name="oldValue"></param>
		/// <param name="newValue"></param>
		protected override void OnSet(int index, object oldValue, object newValue)
		{
			((TNode)newValue).Parent = this.parent;
			base.OnSet(index, oldValue, newValue);
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="index"></param>
		/// <param name="value"></param>
		protected override void OnRemove(int index, object value)
		{
			TNode node = (TNode)value;
			TNode prev = node.PreviousSibing;
			TNode next = node.NextSibling;

			if (prev != null)
				prev.NextSibling = next;

			if (next != null)
				next.PreviousSibing = prev;

			base.OnRemove(index, value);
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="value"></param>
		protected override void OnValidate(object value)
		{
			ExceptionHelper.FalseThrow<ArgumentNullException>(value != null, "value");

			base.OnValidate(value);
		}
	}
}