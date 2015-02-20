using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MCS.Library.Caching
{
	/// <summary>
	/// 通知类Cache依赖项的基类
	/// </summary>
	public abstract class NotifierCacheDependencyBase : DependencyBase
	{
		private bool changed = false;

		/// <summary>
		/// 是否改变
		/// </summary>
		public override bool HasChanged
		{
			get
			{
				return this.changed;
			}
		}

		/// <summary>
		/// 
		/// </summary>
		internal protected override void SetChanged()
		{
			this.changed = true;
		}

		/// <summary>
		/// 根据Cache项得到通知的Key
		/// </summary>
		/// <returns></returns>
		protected CacheNotifyKey GetNotifyKey()
		{
			CacheNotifyKey key = new CacheNotifyKey();
			KeyValuePair<object, object> kp = this.CacheItem.GetKeyValue();

			key.CacheQueueType = this.CacheItem.Queue.GetType();
			key.CacheKey = kp.Key;

			return key;
		}

		/// <summary>
		/// 当CacheItem加入到Cache队列中时
		/// </summary>
		protected internal override void CacheItemBinded()
		{
			CacheNotifyKey key = GetNotifyKey();
			NotifierCacheMonitorBase monitor = GetMonitor();

			lock (monitor.CacheItems)
			{
				monitor.CacheItems[key] = this;
			}

			monitor.EnsureMonitorNotifyThread();
		}

		/// <summary>
        /// 释放资源
		/// </summary>
		/// <param name="disposing"></param>
	    protected override void Dispose(bool disposing)
	    {
	        if (disposing)
	        {
                CacheNotifyKey key = GetNotifyKey();
                NotifierCacheMonitorBase monitor = GetMonitor();

                lock (monitor.CacheItems)
                {
                    if (monitor.CacheItems.ContainsKey(key))
                        monitor.CacheItems.Remove(key);
                }
	        }

            base.Dispose(disposing);
	    }

	    /// <summary>
		/// 得到监听器的实例
		/// </summary>
		/// <returns></returns>
		protected abstract NotifierCacheMonitorBase GetMonitor();
	}
}
