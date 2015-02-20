using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;
using System.Threading;
using MCS.Library.Core;
using MCS.Library.Logging;

namespace MCS.Library.Caching
{
	/// <summary>
	/// UdpCache变化的监听器
	/// </summary>
	[LoggerDescription("UdpCache")]
	[LogSource("UdpCache")]
	public class UdpNotifierCacheMonitor : NotifierCacheMonitorBase
	{
		/// <summary>
		/// 监听器的实例
		/// </summary>
		public static readonly UdpNotifierCacheMonitor Instance = new UdpNotifierCacheMonitor();

		private UdpNotifierCacheMonitor()
		{
		}

		/// <summary>
		/// 获取CacheNotifyData读取器
		/// </summary>
		/// <returns></returns>
		protected override CacheNotifyFetcherBase GetDataFetcher()
		{
			return new UdpCacheNotifyFetcher();
		}

		/// <summary>
		/// 创建接收到消息的日志对象
		/// </summary>
		/// <param name="notifyData"></param>
		/// <returns></returns>
		protected override LogEntity CreateLogEntity(CacheNotifyData notifyData)
		{
			LogEntity logEntity = base.CreateLogEntity(notifyData);

			logEntity.EventID = 7002;
			logEntity.Title = "Udp接收Cache通知";

			return logEntity;
		}
	}
}
