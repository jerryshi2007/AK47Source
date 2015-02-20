#region
// -------------------------------------------------
// Assembly	��	DeluxeWorks.Library
// FileName	��	CachingPerformanceCounters.cs
// Remark	��	Cache���ܼ�������
// -------------------------------------------------
// VERSION  	AUTHOR		DATE			CONTENT
// 1.0		    ������	    20070430		����
// -------------------------------------------------
#endregion

using System;
using System.Text;
using System.Collections.Generic;
using MCS.Library.Core;

namespace MCS.Library.Caching
{
    /// <summary>
    /// ��Cache�йص����ܼ�������װ
    /// </summary>
    public sealed class CachingPerformanceCounters : PerformanceCountersWrapperBase
    {
        /// <summary>
        /// ���췽��
        /// </summary>
        /// <param name="instanceName">ʵ������</param>
        public CachingPerformanceCounters(string instanceName)
			: base(CachePerformanceCounterInstaller.Instance.cacheCounterInstaller, instanceName)
        {
        }

        /// <summary>
        /// Cache��ļ���
        /// </summary>
        public PerformanceCounterWrapper EntriesCounter
        {
            get
            {
				return base.GetCounter("Cache Entries");
            }
        }

        /// <summary>
        /// ���д���
        /// </summary>
        public PerformanceCounterWrapper HitsCounter
        {
            get
            {
				return base.GetCounter("Cache Hits");
            }
        }

        /// <summary>
        /// û�����еĴ���
        /// </summary>
        public PerformanceCounterWrapper MissesCounter
        {
            get
            {
				return base.GetCounter("Cache Misses");
            }
        }

        /// <summary>
        /// �������е����д���
        /// </summary>
        public PerformanceCounterWrapper HitRatioCounter
        {
            get
            {
				return base.GetCounter("Cache Hit Ratio");
            }
        }

        /// <summary>
        /// �������е��ܷ�����
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