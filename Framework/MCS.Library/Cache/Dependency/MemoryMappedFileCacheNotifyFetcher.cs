using System;
using System.Collections.Generic;
using System.IO.MemoryMappedFiles;
using System.Linq;
using System.Text;
using System.Threading;
using System.IO;

namespace MCS.Library.Caching
{
	/// <summary>
	/// 内存映射文件的通知获取器
	/// </summary>
	public class MemoryMappedFileCacheNotifyFetcher : CacheNotifyFetcherBase
	{
		/// <summary>
		/// 最后一次Cache项的Ticks
		/// </summary>
		private long lastTicks = DateTime.Now.Ticks;
		private static readonly CacheNotifyData[] NullNotifyData = new CacheNotifyData[0];
		/// <summary>
		/// 获取数据
		/// </summary>
		/// <returns></returns>
		public override CacheNotifyData[] GetData()
		{
			CacheNotifyData[] result = MemoryMappedFileCacheNotifyFetcher.NullNotifyData;

			if (CacheNotifierSettings.GetConfig().EnableMmfNotifier)
			{
				result = CacheNotifyDataMap.ReadCacheNotifyData(ref this.lastTicks);

				UdpCacheNotifier.TotalCounters.MmfReceivedItemsCounter.IncrementBy(result.Length);
				UdpCacheNotifier.TotalCounters.MmfReceivedCountPerSecond.IncrementBy(result.Length);

				UdpCacheNotifier.AppInstanceCounters.MmfReceivedItemsCounter.IncrementBy(result.Length);
				UdpCacheNotifier.AppInstanceCounters.MmfReceivedCountPerSecond.IncrementBy(result.Length);
			}

			return result;
		}

		/// <summary>
		/// 轮询间隔
		/// </summary>
		/// <returns></returns>
		public override TimeSpan GetInterval()
		{
			return TimeSpan.FromMilliseconds(50);
		}
	}
}
