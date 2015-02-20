using System;
using System.Text;
using System.Diagnostics;
using System.Collections.Generic;
using MCS.Library.Core;
using MCS.Library.Caching;

namespace MCS.Library.Core
{
	/// <summary>
	/// ����MonitorData��Helper��
	/// </summary>
	public static class PerformanceMonitorHelper
	{
		/// <summary>
		/// ��ʼִ�м���
		/// </summary>
		/// <param name="name"></param>
		/// <returns></returns>
		public static MonitorData StartMonitor(string name)
		{
			ExceptionHelper.CheckStringIsNullOrEmpty(name, "name");

			ExceptionHelper.TrueThrow(StopWatchContextCache.Instance.ContainsKey(name),
				"�Ѿ�����������Ϊ{0}��MonitorData", name);

			MonitorData md = new MonitorData();
			StopWatchContextCache.Instance.Add(name, md);

			md.Stopwatch.Start();

			return md;
		}

		/// <summary>
		/// �������Ƿ��Ѿ�����
		/// </summary>
		/// <param name="name"></param>
		/// <returns></returns>
		public static bool ExistsMonitor(string name)
		{
			return StopWatchContextCache.Instance.ContainsKey(name);
		}

		/// <summary>
		/// �Ƴ�������
		/// </summary>
		/// <param name="name"></param>
		public static void RemoveMonitor(string name)
		{
			MonitorData md = null;

			ExceptionHelper.FalseThrow(StopWatchContextCache.Instance.TryGetValue(name, out md),
				"�����ҵ�����Ϊ{0}��MonitorData");

			md.Stopwatch.Stop();
			StopWatchContextCache.Instance.Remove(name);
		}

		/// <summary>
		/// �õ�ָ�����Ƶļ������
		/// </summary>
		/// <param name="name"></param>
		/// <returns></returns>
		public static MonitorData GetMonitor(string name)
		{
			ExceptionHelper.CheckStringIsNullOrEmpty(name, "name");

			MonitorData md = null;

			ExceptionHelper.FalseThrow(StopWatchContextCache.Instance.TryGetValue(name, out md),
				"�����ҵ�����Ϊ{0}��MonitorData");

			return md;
		}

		/// <summary>
		/// �õ�Ĭ�ϵ�Monitor
		/// </summary>
		/// <returns></returns>
		public static MonitorData GetDefaultMonitor()
		{
			MonitorData md = null;

			if (StopWatchContextCache.Instance.TryGetValue(DefaultMonitorName, out md) == false)
				md = StartMonitor(DefaultMonitorName);

			return md;
		}

		/// <summary>
		/// Ĭ�ϵļ�����������
		/// </summary>
		public static string DefaultMonitorName
		{
			get
			{
				string result = string.Empty;

				if (StringTableContextCache.Instance.TryGetValue("DefaultMonitorName", out result) == false)
					result = "DefaultMonitor";

				return result;
			}
			set
			{
				StringTableContextCache.Instance["DefaultMonitorName"] = value; 
			}
		}
	}

	internal class StopWatchContextCache : ContextCacheQueueBase<string, MonitorData>
	{
		public static StopWatchContextCache Instance
		{
			get
			{
				return ContextCacheManager.GetInstance<StopWatchContextCache>();
			}
		}
	}
}
