#region
// -------------------------------------------------
// Assembly	：	DeluxeWorks.Library
// FileName	：	CacheQueueBase.cs
// Remark	：	CacheQueue基类
// -------------------------------------------------
// VERSION  	AUTHOR		DATE			CONTENT
// 1.0		    万振龙	    20070430		创建
// -------------------------------------------------
#endregion

using System;
using System.Collections.Generic;
using System.Text;
using MCS.Library.Core;

namespace MCS.Library.Caching
{
	/// <summary>
	/// Cache队列的虚基类
	/// </summary>
	public abstract class CacheQueueBase
	{
		private CachingPerformanceCounters totalCounters;
		private CachingPerformanceCounters counters;

		/// <summary>
		/// Cache项的数量
		/// </summary>
		public abstract int Count
		{
			get;
		}

		/// <summary>
		/// 清除Cache队列
		/// </summary>
		public abstract void Clear();

		/// <summary>
		/// 是否都标记为更新
		/// </summary>
		public abstract void SetChanged();

		/// <summary>
		/// 虚方法，删除Cache项
		/// </summary>
		/// <param name="cacheItem">被删除的Cache项</param>
		internal protected abstract void RemoveItem(CacheItemBase cacheItem);

		/// <summary>
		/// 得到所有项的描述信息
		/// </summary>
		/// <returns></returns>
		public abstract CacheItemInfoCollection GetAllItemsInfo();

		/// <summary>
		/// 重载ToString。输出内部的项数
		/// </summary>
		/// <returns></returns>
		public override string ToString()
		{
			string result = base.ToString();

			result += string.Format("({0:#,##0})", this.Count);

			return result;
		}

		/// <summary>
		/// 构造方法，初始化性能指针
		/// </summary>
		protected CacheQueueBase()
		{
			this.InitPerformanceCounters(this.GetType().Name);
		}

		/// <summary>
		/// 构造方法，初始化性能指针
		/// </summary>
		protected CacheQueueBase(string instanceName)
		{
			this.InitPerformanceCounters(instanceName);
		}

		/// <summary>
		/// 初始化性能监视指针
		/// </summary>
		/// <param name="instanceName">本地性能监视器的指针</param>
		protected void InitPerformanceCounters(string instanceName)
		{
			if (this.totalCounters == null)
				this.totalCounters = new CachingPerformanceCounters("_Total_");

			if (this.counters == null)
			{
				instanceName.CheckStringIsNullOrEmpty("instanceName");
				this.counters = new CachingPerformanceCounters(instanceName);
			}
		}

		/// <summary>
		/// 所有Cache的性能指针
		/// </summary>
		protected CachingPerformanceCounters TotalCounters
		{
			get
			{
				return this.totalCounters;
			}
		}

		/// <summary>
		/// 性能指针
		/// </summary>
		protected CachingPerformanceCounters Counters
		{
			get
			{
				return this.counters;
			}
		}
	}
}
