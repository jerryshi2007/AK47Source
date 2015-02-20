using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MCS.Library.Principal
{
	/// <summary>
	/// 与Principal相关的扩展方法
	/// </summary>
	public static class PrincipalExtensions
	{
		/// <summary>
		/// 原始仿真时间
		/// </summary>
		/// <param name="dt"></param>
		/// <returns></returns>
		public static DateTime OriginalSimulateTime(this DateTime dt)
		{
			DateTime result = dt;

			if (DeluxePrincipal.IsAuthenticated)
			{
				DeluxeIdentity identity = DeluxeIdentity.Current;

				if (identity.Ticket != null)
				{
					object stObj = null;
					if (identity.Ticket.SignInInfo.Properties.TryGetValue("SimulateTime", out stObj))
					{
						DateTime st = DateTime.Parse(stObj.ToString());

						if (st != DateTime.MinValue)
							result = new DateTime(st.Year, st.Month, st.Day, result.Hour, result.Minute, result.Second, result.Millisecond, DateTimeKind.Local);
						else
							result = st;
					}
				}
			}

			return result;
		}

		/// <summary>
		/// 得到模拟的时间
		/// </summary>
		/// <param name="dt"></param>
		/// <returns></returns>
		public static DateTime SimulateTime(this DateTime dt)
		{
			DateTime result = dt;

			if (DeluxePrincipal.IsAuthenticated)
			{
				DeluxeIdentity identity = DeluxeIdentity.Current;

				if (identity.Ticket != null)
				{
					result = dt.OriginalSimulateTime();

					if (result == DateTime.MinValue)
						result = DateTime.Now;
				}
			}

			return result;
		}
	}
}
