using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MCS.Library.Caching
{
	/// <summary>
	/// Cache通知的Key
	/// </summary>
	public struct CacheNotifyKey
	{
		private Type cacheQueueType;
		private object cacheKey;

		/// <summary>
		/// 类型队列的类型
		/// </summary>
		public Type CacheQueueType
		{
			get
			{
				return this.cacheQueueType;
			}
			set
			{
				this.cacheQueueType = value;
			}
		}

		/// <summary>
		/// Cache通知的Key
		/// </summary>
		public object CacheKey
		{
			get
			{
				return this.cacheKey;
			}
			set
			{
				this.cacheKey = value;
			}
		}

		/// <summary>
		/// 重载哈希算法
		/// </summary>
		/// <returns></returns>
		public override int GetHashCode()
		{
			int result = 0;

			if (this.cacheKey == null && this.cacheQueueType == null)
				result = base.GetHashCode();
			else
			{
				if (this.cacheKey != null && this.cacheQueueType == null)
					result = this.cacheKey.ToString().GetHashCode();
				else
					if (this.cacheKey == null && this.cacheQueueType != null)
						result = this.cacheQueueType.FullName.GetHashCode();
					else
						result = (this.cacheKey.ToString() + this.cacheQueueType.FullName).GetHashCode();
			}

			return result;
		}
	}
}
