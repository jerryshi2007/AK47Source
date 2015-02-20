using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MCS.Library.Core;

namespace MCS.Library.Services
{
	/// <summary>
	/// Web Service方法调用的Performance Counter
	/// </summary>
	public class WebMethodServerCounters : PerformanceCountersWrapperBase
	{
		/// <summary>
        /// 构造方法
        /// </summary>
        /// <param name="instanceName">实例名称</param>
		public WebMethodServerCounters(string instanceName)
			: base(WebMethodServerCountersInstaller.Instance.webMethodServerCounterInstaller, instanceName)
        {
        }

		/// <summary>
		/// 总请求数
		/// </summary>
		public PerformanceCounterWrapper RequestCount
		{
			get
			{
				return base.GetCounter("Request count");
			}
		}

		/// <summary>
		/// 请求失败的次数
		/// </summary>
		public PerformanceCounterWrapper RequestFailCount
		{
			get
			{
				return base.GetCounter("Request fail count");
			}
		}

		/// <summary>
		/// 请求成功的次数
		/// </summary>
		public PerformanceCounterWrapper RequestSuccessCount
		{
			get
			{
				return base.GetCounter("Request success count");
			}
		}

		/// <summary>
		/// 每秒调用次数
		/// </summary>
		public PerformanceCounterWrapper RequestsPerSecond
		{
			get
			{
				return base.GetCounter("Requests per second");
			}
		}

		/// <summary>
		/// 平均执行时间的基数
		/// </summary>
		public PerformanceCounterWrapper RequestAverageDurationBase
		{
			get
			{
				return base.GetCounter("Request average duration base");
			}
		}

		/// <summary>
		/// 平均执行时间的系数
		/// </summary>
		public PerformanceCounterWrapper RequestAverageDuration
		{
			get
			{
				return base.GetCounter("Request average duration(ms)");
			}
		}
	}
}
