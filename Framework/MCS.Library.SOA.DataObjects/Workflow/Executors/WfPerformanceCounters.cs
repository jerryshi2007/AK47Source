using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web;
using MCS.Library.Core;

namespace MCS.Library.SOA.DataObjects.Workflow
{
	public class WfPerformanceCounters
	{
		private PerformanceCounterWrapper totalWFCount = null;
		private PerformanceCounterWrapper failWFCount = null;
		private PerformanceCounterWrapper successWFCount = null;
		private PerformanceCounterWrapper averageWFBase;
		private PerformanceCounterWrapper averageWFDuration = null;
		private PerformanceCounterWrapper averageWithTxWFDuration = null;
		private PerformanceCounterWrapper averageWithTxWFBase = null;
		private PerformanceCounterWrapper moveToCountPerSecond = null;

		private static WfPerformanceCounters globalInstance = null;
		private static WfPerformanceCounters appInstance = null;

		public static WfPerformanceCounters GlobalInstance
		{
			get
			{
				if (WfPerformanceCounters.globalInstance == null)
				{
					lock (typeof(WfPerformanceCounters))
					{
						if (WfPerformanceCounters.globalInstance == null)
							WfPerformanceCounters.globalInstance = new WfPerformanceCounters("_Total_");
					}
				}

				return WfPerformanceCounters.globalInstance;
			}
		}

		public static WfPerformanceCounters AppInstance
		{
			get
			{
				if (WfPerformanceCounters.appInstance == null)
				{
					lock (typeof(WfPerformanceCounters))
					{
						if (WfPerformanceCounters.appInstance == null)
							WfPerformanceCounters.appInstance = new WfPerformanceCounters();
					}
				}

				return WfPerformanceCounters.appInstance;
			}
		}

		private WfPerformanceCounters(string instanceName)
		{
			InitCounters(instanceName);
		}

		private WfPerformanceCounters()
		{
			string instanceName = string.Empty;

			if (EnvironmentHelper.Mode == InstanceMode.Web)
				instanceName = HttpContext.Current.Request.ApplicationPath;
			else
				instanceName = AppDomain.CurrentDomain.FriendlyName;

			instanceName = instanceName.Replace('/', '_');

			InitCounters(instanceName);
		}

		public PerformanceCounterWrapper AverageWFBase
		{
			get { return averageWFBase; }
		}

		public PerformanceCounterWrapper TotalWFCount
		{
			get { return totalWFCount; }
		}

		public PerformanceCounterWrapper FailWFCount
		{
			get { return failWFCount; }
		}

		public PerformanceCounterWrapper SuccessWFCount
		{
			get { return successWFCount; }
		}

		public PerformanceCounterWrapper AverageWFDuration
		{
			get { return averageWFDuration; }
		}

		public PerformanceCounterWrapper AverageWithTxWFBase
		{
			get { return averageWithTxWFBase; }
		}

		public PerformanceCounterWrapper AverageWithTxWFDuration
		{
			get { return averageWithTxWFDuration; }
		}

		public PerformanceCounterWrapper MoveToCountPerSecond
		{
			get { return this.moveToCountPerSecond; }
		}

		private void InitCounters(string instanceName)
		{
			PerformanceCounterInitData data = new PerformanceCounterInitData(
				"MCSWorkflow", "WF Count", instanceName);
			this.totalWFCount = new PerformanceCounterWrapper(data);

			data.CounterName = "WF fail count";
			this.failWFCount = new PerformanceCounterWrapper(data);

			data.CounterName = "WF success count";
			this.successWFCount = new PerformanceCounterWrapper(data);

			data.CounterName = "WF Average Duration(ms)";
			this.averageWFDuration = new PerformanceCounterWrapper(data);

			data.CounterName = "WF Average Duration Base";
			this.averageWFBase = new PerformanceCounterWrapper(data);

			data.CounterName = "WF with Tx Average duration(ms)";
			this.averageWithTxWFDuration = new PerformanceCounterWrapper(data);

			data.CounterName = "WF with Tx Average duration base";
			this.averageWithTxWFBase = new PerformanceCounterWrapper(data);

			data.CounterName = "WF Move to count per second";
			this.moveToCountPerSecond = new PerformanceCounterWrapper(data);
		}
	}
}
