using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MCS.Library.Core;

namespace MCS.Library.Caching
{
    /// <summary>
    /// Udp通知的相关性能指针
    /// </summary>
    public sealed class CacheNotifierPerformanceCounters : PerformanceCountersWrapperBase
    {
        /// <summary>
        /// 构造方法
        /// </summary>
        /// <param name="instanceName">实例名称</param>
        public CacheNotifierPerformanceCounters(string instanceName)
            : base(CachePerformanceCounterInstaller.Instance.cacheNotifierCounterInstaller, instanceName)
        {
        }

        /// <summary>
        /// Udp通知已经发送到次数
        /// </summary>
        public PerformanceCounterWrapper UdpSentItemsCounter
        {
            get
            {
                return base.GetCounter("Udp Sent Items");
            }
        }

        /// <summary>
        /// Udp通知已经接收到的次数
        /// </summary>
        public PerformanceCounterWrapper UdpReceivedItemsCounter
        {
            get
            {
                return base.GetCounter("Udp Received Items");
            }
        }

        /// <summary>
        /// Mmf的当前指针位置
        /// </summary>
        public PerformanceCounterWrapper MmfCurrentPointer
        {
            get
            {
                return base.GetCounter("Mmf Current Pointer");
            }
        }

        /// <summary>
        /// MemoryeMappedFile已经发送到次数
        /// </summary>
        public PerformanceCounterWrapper MmfSentItemsCounter
        {
            get
            {
                return base.GetCounter("Mmf Sent Items");
            }
        }

        /// <summary>
        /// Mmf通知已经接收到的次数
        /// </summary>
        public PerformanceCounterWrapper MmfReceivedItemsCounter
        {
            get
            {
                return base.GetCounter("Mmf Received Items");
            }
        }

        /// <summary>
        /// 从Udp转发到Mmf的Cache通知的个数
        /// </summary>
        public PerformanceCounterWrapper ForwardedUdpToMmfItems
        {
            get
            {
                return base.GetCounter("Forwarded Udp To Mmf Items");
            }
        }

        /// <summary>
        /// 每秒Udp通知的发送次数
        /// </summary>
        public PerformanceCounterWrapper UdpSentCountPerSecond
        {
            get
            {
                return base.GetCounter("Udp Sent Count Per Second");
            }
        }

        /// <summary>
        /// 每秒Udp通知的接收次数
        /// </summary>
        public PerformanceCounterWrapper UdpReceivedCountPerSecond
        {
            get
            {
                return base.GetCounter("Udp Received Count Per Second");
            }
        }

        /// <summary>
        /// 每秒Mmf通知的发送次数
        /// </summary>
        public PerformanceCounterWrapper MmfSentCountPerSecond
        {
            get
            {
                return base.GetCounter("Mmf Sent Count Per Second");
            }
        }

        /// <summary>
        /// 每秒Mmf通知的接收次数
        /// </summary>
        public PerformanceCounterWrapper MmfReceivedCountPerSecond
        {
            get
            {
                return base.GetCounter("Mmf Received Count Per Second");
            }
        }

        /// <summary>
        /// 每秒从Udp转发到Mmf通知的次数
        /// </summary>
        public PerformanceCounterWrapper ForwardUdpToMmfCountPerSecond
        {
            get
            {
                return base.GetCounter("Forward Udp To Mmf Count Per Second");
            }
        }
    }
}
