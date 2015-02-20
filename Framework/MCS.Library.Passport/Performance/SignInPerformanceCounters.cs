using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.Hosting;
using MCS.Library.Core;

namespace MCS.Library.Passport.Performance
{
	/// <summary>
	/// 认证所涉及到的性能监控指针
	/// </summary>
	public sealed class SignInPerformanceCounters : PerformanceCountersWrapperBase
	{
		private static SignInPerformanceCounters globalInstance = null;
		private static SignInPerformanceCounters appInstance = null;

		/// <summary>
		/// 
		/// </summary>
		public static SignInPerformanceCounters GlobalInstance
		{
			get
			{
				if (SignInPerformanceCounters.globalInstance == null)
				{
					lock (typeof(SignInPerformanceCounters))
					{
						if (SignInPerformanceCounters.globalInstance == null)
							SignInPerformanceCounters.globalInstance = new SignInPerformanceCounters("_Total_");
					}
				}

				return SignInPerformanceCounters.globalInstance;
			}
		}

		/// <summary>
		/// 
		/// </summary>
		public static SignInPerformanceCounters AppInstance
		{
			get
			{
				if (SignInPerformanceCounters.appInstance == null)
				{
					lock (typeof(SignInPerformanceCounters))
					{
						if (SignInPerformanceCounters.appInstance == null)
						{
							SignInPerformanceCounters.appInstance = new SignInPerformanceCounters(GetInstanceName());
						}
					}
				}

				return SignInPerformanceCounters.appInstance;
			}
		}

		/// <summary>
		/// 构造方法
		/// </summary>
		/// <param name="instanceName"></param>
		private SignInPerformanceCounters(string instanceName)
			: base(PassportPerformanceCounterInstaller.Instance.signInPerformanceCounter, instanceName)
		{
		}

		/// <summary>
		/// 总认证次数
		/// </summary>
		public PerformanceCounterWrapper SignInCount
		{
			get
			{
				return this.GetCounter("SignIn Count");
			}
		}

		/// <summary>
		/// 认证失败的次数
		/// </summary>
		public PerformanceCounterWrapper SignInFailCount
		{
			get
			{
				return this.GetCounter("SignIn Fail Count");
			}
		}

		/// <summary>
		/// 认证成功的次数
		/// </summary>
		public PerformanceCounterWrapper SignInSuccessCount
		{
			get
			{
				return this.GetCounter("SignIn Success Count");
			}
		}

		/// <summary>
		/// 平均认证时间
		/// </summary>
		public PerformanceCounterWrapper SignInAverageDuration
		{
			get
			{
				return this.GetCounter("SignIn Average Duration(ms)");
			}
		}

		/// <summary>
		/// 平均认证时间的Base
		/// </summary>
		public PerformanceCounterWrapper SignInAverageDurationBase
		{
			get
			{
				return this.GetCounter("SignIn Average Duration Base");
			}
		}

		/// <summary>
		/// 每秒认证的次数
		/// </summary>
		public PerformanceCounterWrapper SignInCountPerSecond
		{
			get
			{
				return this.GetCounter("SignIn Count Per Second");
			}
		}

		/// <summary>
		/// 执行认证动作
		/// </summary>
		/// <param name="action"></param>
		public static void DoAction(Action action)
		{
			if (action != null)
			{
				Stopwatch sw = new Stopwatch();
				sw.Start();

				try
				{
					SignInPerformanceCounters.GlobalInstance.SignInCount.Increment();
					SignInPerformanceCounters.AppInstance.SignInCount.Increment();

					action();

					SignInPerformanceCounters.GlobalInstance.SignInSuccessCount.Increment();
					SignInPerformanceCounters.AppInstance.SignInSuccessCount.Increment();
				}
				catch (System.Exception)
				{
					SignInPerformanceCounters.GlobalInstance.SignInFailCount.Increment();
					SignInPerformanceCounters.AppInstance.SignInFailCount.Increment();
					throw;
				}
				finally
				{
					sw.Stop();

					SignInPerformanceCounters.GlobalInstance.SignInAverageDuration.IncrementBy(sw.ElapsedMilliseconds / 1000);
					SignInPerformanceCounters.GlobalInstance.SignInAverageDurationBase.Increment();

					SignInPerformanceCounters.AppInstance.SignInAverageDuration.IncrementBy(sw.ElapsedMilliseconds / 1000);
					SignInPerformanceCounters.AppInstance.SignInAverageDurationBase.Increment();

					SignInPerformanceCounters.GlobalInstance.SignInCountPerSecond.Increment();
					SignInPerformanceCounters.AppInstance.SignInCountPerSecond.Increment();
				}
			}
		}
	}
}
