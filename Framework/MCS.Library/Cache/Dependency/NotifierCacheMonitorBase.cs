using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading;
using MCS.Library.Core;
using MCS.Library.Logging;

namespace MCS.Library.Caching
{
	/// <summary>
	/// Cache变化的监听器的积累
	/// </summary>
	[LoggerDescription("NotifyCache")]
	[LogSource("NotifyCache")]
	public abstract class NotifierCacheMonitorBase
	{
		private readonly CacheNotifyKeyToDependencies cacheItemEntries = new CacheNotifyKeyToDependencies();
		private Thread monitorNotifyThread = null;

		/// <summary>
		/// 通知的Key的依赖项之间的关系
		/// </summary>
		public CacheNotifyKeyToDependencies CacheItems
		{
			get
			{
				return this.cacheItemEntries;
			}
		}

		/// <summary>
		/// 确保监听线程被创建
		/// </summary>
		public void EnsureMonitorNotifyThread()
		{
			lock (this)
			{
				if (this.monitorNotifyThread == null)
				{
					Thread thread = new Thread(new ThreadStart(MonitorThreadMethod));
					thread.IsBackground = true;

					this.monitorNotifyThread = thread;

					thread.Start();
				}
			}
		}

		/// <summary>
		/// 处理Cache项的改变
		/// </summary>
		/// <param name="data"></param>
		protected void DoCacheChanged(CacheNotifyData data)
		{
			DependencyBase dependency;

			if (data.NotifyType == CacheNotifyType.Clear)
			{
				//清除所有UDP缓存相关的缓存队列
				CacheQueueBase needToClearQueue = null;

				lock (this.CacheItems)
				{
					foreach (KeyValuePair<CacheNotifyKey, DependencyBase> kp in this.CacheItems)
					{
						if (kp.Key.CacheQueueType == data.CacheQueueType)
						{
							needToClearQueue = kp.Value.CacheItem.Queue;
						}
					}
				}

				if (needToClearQueue != null)
					needToClearQueue.Clear();
			}
			else
			{
				CacheNotifyKey key = new CacheNotifyKey();

				key.CacheKey = data.CacheKey;
				key.CacheQueueType = data.CacheQueueType;

				lock (this.CacheItems)
				{
					if (this.CacheItems.TryGetValue(key, out dependency))
					{
						switch (data.NotifyType)
						{
							case CacheNotifyType.Invalid:
								dependency.SetChanged();
								this.CacheItems.Remove(key);
								break;
							case CacheNotifyType.Update:
								dependency.CacheItem.SetValue(data.CacheData);
								break;
						}
					}
				}
			}
		}

		/// <summary>
		/// 创建Logger。如果出现异常则有可能为空
		/// </summary>
		/// <returns></returns>
		protected virtual Logger CreateLogger()
		{
			Logger logger = null;

			try
			{
				string name = "NotifyCache";
				LoggerDescriptionAttribute loggerDesp = AttributeHelper.GetCustomAttribute<LoggerDescriptionAttribute>(this.GetType());

				if (loggerDesp != null)
					name = loggerDesp.Name;

				logger = LoggerFactory.Create(name);
			}
			catch (SystemSupportException)
			{
			}

			return logger;
		}

		/// <summary>
		/// 将CacheNotifyData写入日志
		/// </summary>
		/// <param name="logger"></param>
		/// <param name="notifyData"></param>
		protected virtual void WriteCacheNotifyDataToLog(Logger logger, CacheNotifyData notifyData)
		{
			if (logger != null)
				logger.Write(CreateLogEntity(notifyData));
		}

		/// <summary>
		/// 创建Log项
		/// </summary>
		/// <param name="notifyData"></param>
		/// <returns></returns>
		protected virtual LogEntity CreateLogEntity(CacheNotifyData notifyData)
		{
			LogEntity logEntity = new LogEntity(notifyData.ToString());

			logEntity.EventID = 7002;
			logEntity.LogEventType = TraceEventType.Information;

			logEntity.Source = GetLogSource();
			logEntity.Title = "接收Cache";
			logEntity.Priority = LogPriority.Normal;

			return logEntity;
		}

		/// <summary>
		/// 获得CacheNotifyData读取器
		/// </summary>
		/// <returns></returns>
		protected abstract CacheNotifyFetcherBase GetDataFetcher();

		/// <summary>
		/// 监听线程方法
		/// </summary>
		private void MonitorThreadMethod()
		{
			try
			{
				using (CacheNotifyFetcherBase fetcher = this.GetDataFetcher())
				{
					fetcher.Init();
					Logger logger = CreateLogger();

					while (true)
					{
						try
						{
							CacheNotifyData[] dataArray = fetcher.GetData();

							foreach (CacheNotifyData data in dataArray)
							{
								DoCacheChanged(data);

								WriteCacheNotifyDataToLog(logger, data);
							}

							Thread.Sleep(fetcher.GetInterval());
						}
						catch (ThreadAbortException)
						{
						}
						catch (System.Exception ex)
						{
							Trace.WriteLine(EnvironmentHelper.GetEnvironmentInfo());
							Trace.WriteLine(ex.ToString());

							Thread.Sleep(fetcher.GetInterval());
						}
					}
				}
			}
			catch (ThreadAbortException)
			{
			}
			catch (System.Exception ex)
			{
				ex.WriteToEventLog(GetLogSource(), EventLogEntryType.Error, 40001);
			}
		}

		private string GetLogSource()
		{
			string source = "NotifyCache";

			LogSourceAttribute logSource = AttributeHelper.GetCustomAttribute<LogSourceAttribute>(this.GetType());

			if (logSource != null)
				source = logSource.Name;

			return source;
		}
	}
}
