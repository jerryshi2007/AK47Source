using System;
using System.Web;
using System.Text;
using System.Collections.Generic;
using MCS.Library.Core;

namespace MCS.Web.Library
{
	/// <summary>
	/// 
	/// </summary>
	public sealed class PMPerformanceCounters
	{
		private string instanceName = string.Empty;

		private PerformanceCounterWrapper pageAccessCount = null;
		private PerformanceCounterWrapper pageSuccessCount = null;
		private PerformanceCounterWrapper pageErrorCount = null;
		private PerformanceCounterWrapper pageSuccessRatio = null;
		private PerformanceCounterWrapper pageSuccessRatioBase = null;
		private PerformanceCounterWrapper pageAccessCurrentAverage = null;
		private PerformanceCounterWrapper pageAccessCurrentAverageBase = null;
		private PerformanceCounterWrapper pageAccessTotalAverage = null;
		private PerformanceCounterWrapper pageAccessTotalAverageBase = null;
		private PerformanceCounterWrapper pageAccessCountPerSecond = null;

		/// <summary>
		/// 
		/// </summary>
		public PerformanceCounterWrapper PageAccessCount
		{
			get { return pageAccessCount; }
			set { pageAccessCount = value; }
		}

		private static Dictionary<string, PMPerformanceCounters> counters = 
			new Dictionary<string, PMPerformanceCounters>(StringComparer.OrdinalIgnoreCase);

		/// <summary>
		/// 
		/// </summary>
		/// <param name="instName"></param>
		/// <returns></returns>
		public static PMPerformanceCounters GetCounters(string instName)
		{
			ExceptionHelper.CheckStringIsNullOrEmpty(instName, "instanceName");

			PMPerformanceCounters result = null;

			lock (PMPerformanceCounters.counters)
			{
				if (PMPerformanceCounters.counters.TryGetValue(instName, out result) == false)
				{
					result = new PMPerformanceCounters(instName);
					PMPerformanceCounters.counters.Add(instName, result);
				}
			}

			return result;
		}

		/// <summary>
		/// 
		/// </summary>
		public PerformanceCounterWrapper PageSuccessCount
		{
			get { return pageSuccessCount; }
		}

		/// <summary>
		/// 
		/// </summary>
		public PerformanceCounterWrapper PageErrorCount
		{
			get { return pageErrorCount; }
		}

		/// <summary>
		/// 
		/// </summary>
		public PerformanceCounterWrapper PageSuccessRatio
		{
			get { return pageSuccessRatio; }
		}

		/// <summary>
		/// 
		/// </summary>
		public PerformanceCounterWrapper PageSuccessRatioBase
		{
			get { return pageSuccessRatioBase; }
		}

		/// <summary>
		/// 
		/// </summary>
		public PerformanceCounterWrapper PageAccessCurrentAverage
		{
			get { return pageAccessCurrentAverage; }
		}

		/// <summary>
		/// 
		/// </summary>
		public PerformanceCounterWrapper PageAccessCurrentAverageBase
		{
			get { return pageAccessCurrentAverageBase; }
		}

		/// <summary>
		/// 
		/// </summary>
		public PerformanceCounterWrapper PageAccessTotalAverage
		{
			get { return pageAccessTotalAverage; }
		}

		/// <summary>
		/// 
		/// </summary>
		public PerformanceCounterWrapper PageAccessTotalAverageBase
		{
			get { return pageAccessTotalAverageBase; }
		}

		/// <summary>
		/// 
		/// </summary>
		public PerformanceCounterWrapper PageAccessCountPerSecond
		{
			get { return pageAccessCountPerSecond; }
		}

		private PMPerformanceCounters(string instName)
		{
			this.instanceName = instName;

			instName = instName.Replace('/', '_');
			InitCounters(instName);
		}

		private void InitCounters(string instanceName)
		{
			PerformanceCounterInitData data = new PerformanceCounterInitData(
				"MCSLibraryPageCounters", "Page access count", instanceName);

			this.pageAccessCount = new PerformanceCounterWrapper(data);

			data.CounterName = "Page access error count";
			this.pageErrorCount = new PerformanceCounterWrapper(data);

			data.CounterName = "Page access success count";
			this.pageSuccessCount = new PerformanceCounterWrapper(data);

			data.CounterName = "Page access success ratio";
			this.pageSuccessRatio = new PerformanceCounterWrapper(data);

			data.CounterName = "Page access success ratio base";
			this.pageSuccessRatioBase = new PerformanceCounterWrapper(data);

			data.CounterName = "Page access current average";
			this.pageAccessCurrentAverage = new PerformanceCounterWrapper(data);

			data.CounterName = "Page access current average base";
			this.pageAccessCurrentAverageBase = new PerformanceCounterWrapper(data);

			data.CounterName = "Page access total average(ms)";
			this.pageAccessTotalAverage = new PerformanceCounterWrapper(data);

			data.CounterName = "Page access total average base";
			this.pageAccessTotalAverageBase = new PerformanceCounterWrapper(data);

			data.CounterName = "Page access count per second";
			this.pageAccessCountPerSecond = new PerformanceCounterWrapper(data);
		}
	}
}
