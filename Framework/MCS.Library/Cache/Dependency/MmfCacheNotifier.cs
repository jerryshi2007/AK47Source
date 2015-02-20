using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using MCS.Library.Logging;
using MCS.Library.Core;
using System.Diagnostics;

namespace MCS.Library.Caching
{
    /// <summary>
    /// 基于内存映射文件发送Cache通知
    /// </summary>
    public class MmfCacheNotifier : ICacheNotifier
    {
        /// <summary>
        /// 内部构建的实体对象，单件模式的表现方式
        /// </summary>
        public static readonly MmfCacheNotifier Instance = new MmfCacheNotifier();

        private MmfCacheNotifier()
        {
        }

        /// <summary>
        /// 同步发送Cache通知
        /// </summary>
        /// <param name="notifyDataArray"></param>
        public void SendNotify(params CacheNotifyData[] notifyDataArray)
        {
            notifyDataArray.NullCheck("notifyDataArray");

            InnerSendNotify(notifyDataArray);
        }

        /// <summary>
        /// 异步发送Cache通知
        /// </summary>
        /// <param name="notifyDataArray"></param>
        public void SendNotifyAsync(params CacheNotifyData[] notifyDataArray)
        {
            notifyDataArray.NullCheck("notifyDataArray");

            ThreadPool.QueueUserWorkItem(new WaitCallback(SendNotifyThreadCallBack), notifyDataArray);
        }

        private void SendNotifyThreadCallBack(object context)
        {
            CacheNotifyData[] notifyDataArray = (CacheNotifyData[])context;

            InnerSendNotify(notifyDataArray);
        }

        private void InnerSendNotify(CacheNotifyData[] notifyDataArray)
        {
			if (CacheNotifierSettings.GetConfig().EnableMmfNotifier)
			{
				Logger logger = null;

				try
				{
					logger = LoggerFactory.Create("MmfCache");
				}
				catch (SystemSupportException)
				{
				}

				CacheNotifyDataMap.WriteCacheNotifyData(notifyDataArray);

				if (logger != null)
				{
					foreach (CacheNotifyData notifyData in notifyDataArray)
						logger.Write(CreateLogEntity(notifyData));
				}
			}
        }

        private static LogEntity CreateLogEntity(CacheNotifyData notifyData)
        {
            string message = string.Format("{0}",
                    notifyData.ToString()
                );

            LogEntity logEntity = new LogEntity(message);

            logEntity.EventID = 7005;
            logEntity.LogEventType = TraceEventType.Information;
            logEntity.Source = "MmfCache";
            logEntity.Title = "发送MmfCache";
            logEntity.Priority = LogPriority.Normal;

            return logEntity;
        }
    }
}
