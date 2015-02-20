using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MCS.Library.Caching
{
	/// <summary>
	/// Cache队列信息
	/// </summary>
	[Serializable]
	public class CacheQueueInfo
	{
		/// <summary>
		/// Cache队列的名称
		/// </summary>
		public string QueueTypeName
		{
			get;
			internal set;
		}

		/// <summary>
		/// Cache队列的全名
		/// </summary>
		public string QueueTypeFullName
		{
			get;
			internal set;
		}

		/// <summary>
		/// Cache队列中的项数
		/// </summary>
		public int Count
		{
			get;
			internal set;
		}
	}

	/// <summary>
	/// Cache队列信息的集合
	/// </summary>
	[Serializable]
	public class CacheQueueInfoCollection : CollectionBase
	{
		/// <summary>
		/// 增加一项
		/// </summary>
		/// <param name="info"></param>
		internal void Add(CacheQueueInfo info)
		{
			List.Add(info);
		}

		/// <summary>
		/// 取得第n项信息
		/// </summary>
		/// <param name="index"></param>
		/// <returns></returns>
		public CacheQueueInfo this[int index]
		{
			get
			{
				return (CacheQueueInfo)List[index];
			}
		}
	}
}
