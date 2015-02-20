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
	/// <typeparam name="TList"></typeparam>
	public abstract class GroupNode<TKey, TList> where TList : IList, new()
	{
		private TKey groupKey;
		private TList data;

		/// <summary>
		/// 
		/// </summary>
		public TKey GroupKey
		{
			get
			{
				return this.groupKey;
			}
			internal set
			{
				this.groupKey = value;
			}
		}

		/// <summary>
		/// 
		/// </summary>
		public TList Data
		{
			get
			{
				if (this.data == null)
					this.data = new TList();

				return this.data;
			}
			internal set
			{
				this.data = value;
			}
		}
	}
}
