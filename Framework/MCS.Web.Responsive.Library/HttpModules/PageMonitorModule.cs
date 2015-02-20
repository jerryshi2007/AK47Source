using System;
using System.Web;
using System.Text;
using System.IO;
using System.Diagnostics;
using System.Collections.Generic;
using MCS.Library.Logging;
using MCS.Library.Core;

namespace MCS.Web.Responsive.Library
{
	internal class PageMonitorModule : IHttpModule
	{
		private const string MonitorDataKey = "PageMonitorModuleMonitorDataKey";

		public void Dispose()
		{
		}

		public void Init(HttpApplication context)
		{
			context.BeginRequest += new EventHandler(context_BeginRequestExecute);
			context.EndRequest += new EventHandler(context_EndRequestExecute);
			context.Error += new EventHandler(context_Error);
		}

		private void context_Error(object sender, EventArgs e)
		{
			if (PerformanceMonitorHelper.ExistsMonitor(PageMonitorModule.MonitorDataKey))
			{
				MonitorData md = PerformanceMonitorHelper.GetMonitor(PageMonitorModule.MonitorDataKey);

				md.HasErrors = true;
			}
		}

		private void context_BeginRequestExecute(object sender, EventArgs e)
		{
			if (PageMonitorSettings.GetConfig().Enabled)
			{
				PageMonitorElement pme = PageMonitorSettings.GetConfig().Pages.GetMatchedElement();

				if (pme != null)
				{
					MonitorData md = PerformanceMonitorHelper.StartMonitor(PageMonitorModule.MonitorDataKey);

					md.EnableLogging = pme.EnableLogging;
					md.EnablePFCounter = pme.EnablePFCounter;
					md.MonitorName = pme.Name;

					if (string.IsNullOrEmpty(pme.CounterInstanceName))
						md.InstanceName = pme.Name;
					else
						md.InstanceName = pme.CounterInstanceName;

					PerformanceMonitorHelper.DefaultMonitorName = PageMonitorModule.MonitorDataKey;

					if (pme.EnableLogging)
						md.LogWriter.WriteLine("请求{0}的开始时间: {1:yyyy-MM-dd HH:mm:ss.fff}", md.MonitorName, DateTime.Now);
				}
			}
		}

		private void context_EndRequestExecute(object sender, EventArgs e)
		{
			if (PerformanceMonitorHelper.ExistsMonitor(PageMonitorModule.MonitorDataKey))
			{
				if (HttpContext.Current.Response.StatusCode != 302)
				{
					MonitorData md = PerformanceMonitorHelper.GetMonitor(PageMonitorModule.MonitorDataKey);

					md.Stopwatch.Stop();

					if (md.EnableLogging)
						md.LogWriter.WriteLine("请求{0}的结束时间: {1:yyyy-MM-dd HH:mm:ss.fff}，经过{2:#,##0}毫秒",
							md.MonitorName, DateTime.Now, md.Stopwatch.ElapsedMilliseconds);

					CommitLogging(md);
					SetCountersValue(md);

					PerformanceMonitorHelper.RemoveMonitor(PageMonitorModule.MonitorDataKey);
					PerformanceMonitorHelper.DefaultMonitorName = "DefaultMonitor";
				}
			}
		}

		private void SetCountersValue(MonitorData md)
		{
			PMPerformanceCounters counters = PMPerformanceCounters.GetCounters(md.InstanceName);

			counters.PageAccessCount.Increment();
			counters.PageAccessCountPerSecond.Increment();

			if (md.HasErrors)
				counters.PageErrorCount.Increment();
			else
			{
				counters.PageSuccessCount.Increment();
				counters.PageSuccessRatio.Increment();
			}

			counters.PageSuccessRatioBase.Increment();

			counters.PageAccessCurrentAverageBase.Increment();
			counters.PageAccessCurrentAverage.IncrementBy(md.Stopwatch.ElapsedTicks);
			counters.PageAccessTotalAverageBase.Increment();
			counters.PageAccessTotalAverage.IncrementBy(md.Stopwatch.ElapsedMilliseconds / 100);
		}

		private void CommitLogging(MonitorData md)
		{
			StringBuilder strB = ((StringWriter)md.LogWriter).GetStringBuilder();

			if (strB.Length > 0)
			{
				try
				{
					Logger logger = LoggerFactory.Create("PageMonitor");

					if (logger != null)
						logger.Write(strB.ToString(), LogPriority.Normal, 8005, TraceEventType.Information, "页面访问日志");
				}
				catch (System.Exception)
				{
				}
			}
		}
	}
}
