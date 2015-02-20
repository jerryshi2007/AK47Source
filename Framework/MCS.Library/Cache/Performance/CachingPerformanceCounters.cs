#region
// -------------------------------------------------
// Assembly	：	DeluxeWorks.Library
// FileName	：	CachingPerformanceCounters.cs
// Remark	：	Cache性能计数器类
// -------------------------------------------------
// VERSION  	AUTHOR		DATE			CONTENT
// 1.0		    万振龙	    20070430		创建
// -------------------------------------------------
#endregion

using System;
using System.Text;
using System.Collections.Generic;
using MCS.Library.Core;

namespace MCS.Library.Caching
{
    /// <summary>
    /// 和Cache有关的性能计数器包装
    /// </summary>
    public sealed class CachingPerformanceCounters : PerformanceCountersWrapperBase
    {
        /// <summary>
        /// 构造方法
        /// </summary>
        /// <param name="instanceName">实例名称</param>
        public CachingPerformanceCounters(string instanceName)
			: base(CachePerformanceCounterInstaller.Instance.cacheCounterInstaller, instanceName)
        {
        }

        /// <summary>
        /// Cache项的计数
        /// </summary>
        public PerformanceCounterWrapper EntriesCounter
        {
            get
            {
				return base.GetCounter("Cache Entries");
            }
        }

        /// <summary>
        /// 命中次数
        /// </summary>
        public PerformanceCounterWrapper HitsCounter
        {
            get
            {
				return base.GetCounter("Cache Hits");
            }
        }

        /// <summary>
        /// 没有命中的次数
        /// </summary>
        public PerformanceCounterWrapper MissesCounter
        {
            get
            {
				return base.GetCounter("Cache Misses");
            }
        }

        /// <summary>
        /// 命令率中的命中次数
        /// </summary>
        public PerformanceCounterWrapper HitRatioCounter
        {
            get
            {
				return base.GetCounter("Cache Hit Ratio");
            }
        }

        /// <summary>
        /// 命中率中的总访问数
        /// </summary>
        public PerformanceCounterWrapper HitRatioBaseCounter
        {
            get
            {
				return base.GetCounter("Cache Hit Ratio Base");
            }
        }
    }
}