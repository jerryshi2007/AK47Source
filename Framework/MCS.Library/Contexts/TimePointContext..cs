using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MCS.Library.Caching;
using System.Web;
using MCS.Library.Configuration;

namespace MCS.Library.Core
{
	/// <summary>
	/// 时间点上下文
	/// </summary>
	public class TimePointContext
	{
		private bool _UseCurrentTime = true;
		private DateTime _SimulatedTime = DateTime.MinValue;

		/// <summary>
		/// 是否使用当前时间
		/// </summary>
		public bool UseCurrentTime
		{
			get
			{
				return this._UseCurrentTime;
			}
			set
			{
				this._UseCurrentTime = value;
			}
		}

		/// <summary>
		/// 获取仿真后的时间
		/// </summary>
		public DateTime SimulatedTime
		{
			get
			{
				DateTime result = DateTime.Now;

				if (UseCurrentTime == false && this._SimulatedTime != DateTime.MinValue)
					result = this._SimulatedTime;

				return this._SimulatedTime;
			}
			set
			{
				this._SimulatedTime = value;
			}
		}

		/// <summary>
		/// 获取时间点上下文对象
		/// </summary>
		public static TimePointContext Current
		{
			get
			{
				return (TimePointContext)ObjectContextCache.Instance.GetOrAddNewValue("TimePointContext", (cache, key) =>
				{
					TimePointContext context = new TimePointContext();

					cache.Add(key, context);

					return context;
				});
			}
		}

		/// <summary>
		/// 清除时间点模拟的上下文
		/// </summary>
		public static void Clear()
		{
			ObjectContextCache.Instance.Remove("TimePointContext");
		}

		/// <summary>
		/// 得到当前TimePointContext的值
		/// </summary>
		/// <returns></returns>
		public static TimePointContext GetCurrentState()
		{
			TimePointContext result = new TimePointContext();

			result._UseCurrentTime = TimePointContext.Current._UseCurrentTime;
			result._SimulatedTime = TimePointContext.Current._SimulatedTime;

			return result;
		}

		/// <summary>
		/// 使用state的值恢复Current的值
		/// </summary>
		/// <param name="state"></param>
		public static void RestoreCurrentState(TimePointContext state)
		{
			if (state != null)
			{
				TimePointContext.Current._UseCurrentTime = state._UseCurrentTime;
				TimePointContext.Current._SimulatedTime = state._SimulatedTime;
			}
		}

		/// <summary>
		/// 从Cookie中加载Context，Cookie的Key为TimePointSimulationSettings.GetConfig().CookieKey定义
		/// </summary>
		/// <returns></returns>
		public static TimePointContext LoadFromCookie()
		{
			(EnvironmentHelper.Mode == InstanceMode.Web).FalseThrow("当前应用不能获取到HttpContext");

			HttpRequest request = HttpContext.Current.Request;

			HttpCookie cookie = request.Cookies[TimePointSimulationSettings.GetConfig().CookieKey];

			if (cookie != null)
			{
				TimePointContext.Current.UseCurrentTime = cookie.Values.GetValue("UseCurrentTime", true);
				TimePointContext.Current.SimulatedTime = cookie.Values.GetValue("SimulatedTime", DateTime.MinValue);
			}

			return TimePointContext.Current;
		}

		/// <summary>
		/// 将Context保存到Cookie，Cookie的Key为TimePointSimulationSettings.GetConfig().CookieKey定义
		/// </summary>
		public void SaveToCookie()
		{
			(EnvironmentHelper.Mode == InstanceMode.Web).FalseThrow("当前应用不能获取到HttpContext");

			HttpResponse response = HttpContext.Current.Response;
			HttpRequest request = HttpContext.Current.Request;

			HttpCookie cookie = new HttpCookie(TimePointSimulationSettings.GetConfig().CookieKey);

			cookie.Values.Add("UseCurrentTime", this.UseCurrentTime.ToString());
			cookie.Values.Add("SimulatedTime", this.SimulatedTime.ToString("yyyy-MM-dd HH:mm:ss.fff"));

			cookie.Expires = DateTime.MaxValue;

			response.Cookies.Add(cookie);
		}
	}
}
