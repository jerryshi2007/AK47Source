using System;
using System.Text;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace MCS.Library.Data.DataObjects
{
	/// <summary>
	/// 
	/// </summary>
	/// <typeparam name="TKey"></typeparam>
	/// <typeparam name="TItem"></typeparam>
	/// <param name="data"></param>
	/// <returns></returns>
	public delegate TKey GetGroupKeyForItemDelegate<TKey, TItem>(TItem data);

	/// <summary>
	/// 
	/// </summary>
	/// <typeparam name="TGroupNode"></typeparam>
	/// <typeparam name="TKey"></typeparam>
	/// <typeparam name="TList"></typeparam>
	public abstract class GroupNodeCollection<TGroupNode, TKey, TList> : ReadOnlyCollection<TGroupNode>
		where TGroupNode : GroupNode<TKey, TList>, new()
		where TList : IList, new()
	{
		/// <summary>
		/// 
		/// </summary>
		protected GroupNodeCollection()
			: base(new List<TGroupNode>())
		{
		}

		private Dictionary<TKey, TGroupNode> innerDictionary = new Dictionary<TKey, TGroupNode>();

		/// <summary>
		/// 
		/// </summary>
		/// <param name="key"></param>
		/// <returns></returns>
		public TGroupNode this[TKey key]
		{
			get
			{
				return innerDictionary[key];
			}
		}
		
		/// <summary>
		/// 
		/// </summary>
		/// <param name="item"></param>
		/// <returns></returns>
		protected virtual TKey GetKeyForItem(TGroupNode item)
		{
			return item.GroupKey;
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="key"></param>
		/// <param name="data"></param>
		/// <returns></returns>
		protected virtual TGroupNode CreateGroupNode<TItem>(TKey key, TItem data)
		{
			TGroupNode groupItem = new TGroupNode();
			groupItem.GroupKey = key;

			return groupItem;
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="key"></param>
		/// <param name="node"></param>
		/// <returns></returns>
		protected virtual bool TryGetValue(TKey key, out TGroupNode node)
		{
			return innerDictionary.TryGetValue(key, out node);
		}

		/// <summary>
		/// 
		/// </summary>
		/// <typeparam name="TItem"></typeparam>
		/// <param name="enumerator"></param>
		/// <param name="getKeyDelegate"></param>
		protected virtual void FillData<TItem>(IEnumerable enumerator,
			GetGroupKeyForItemDelegate<TKey, TItem> getKeyDelegate)
		{
			this.Items.Clear();

			foreach (TItem data in enumerator)
			{
				if (getKeyDelegate != null)
				{
					TKey key = getKeyDelegate(data);
					TGroupNode groupItem;

					if (innerDictionary.ContainsKey(key))
						groupItem = this[key];
					else
					{
						groupItem = CreateGroupNode(key, data);

						innerDictionary.Add(key, groupItem);
						Items.Add(groupItem);
					}

					groupItem.Data.Add(data);
				}
			}
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="comparer"></param>
		public void SortGroups(IComparer<TGroupNode> comparer)
		{
			TGroupNode[] nodes = new TGroupNode[this.Count];

			Items.CopyTo(nodes, 0);

			Array.Sort(nodes, comparer);

			Items.Clear();

			for (int i = 0; i < nodes.Length; i++)
				Items.Add(nodes[i]);
		}
	}
}
