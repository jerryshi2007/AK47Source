using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MCS.Library.Logging;

namespace MCS.Library.Caching
{
	/// <summary>
	/// 内存映射文件的监听器
	/// </summary>
	[LoggerDescription("MemoryMappedFileCache")]
	[LogSource("MemoryMappedFileCache")]
	public class MemoryMappedFileCacheMonitor : NotifierCacheMonitorBase
	{
		/// <summary>
		/// 监听器的实例
		/// </summary>
		public static readonly MemoryMappedFileCacheMonitor Instance = new MemoryMappedFileCacheMonitor();

		private MemoryMappedFileCacheMonitor()
		{
		}

		/// <summary>
		/// 得到数据获取器
		/// </summary>
		/// <returns></returns>
		protected override CacheNotifyFetcherBase GetDataFetcher()
		{
			return new MemoryMappedFileCacheNotifyFetcher();
		}

		/// <summary>
		/// 创建接收到消息的日志对象
		/// </summary>
		/// <param name="notifyData"></param>
		/// <returns></returns>
		protected override LogEntity CreateLogEntity(CacheNotifyData notifyData)
		{
			LogEntity logEntity = base.CreateLogEntity(notifyData);

			logEntity.EventID = 7004;
			logEntity.Title = "MemoryMappedFile接收Cache通知";

			return logEntity;
		}
	}
}
