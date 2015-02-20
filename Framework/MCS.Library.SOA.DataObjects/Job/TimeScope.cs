using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MCS.Library.SOA.DataObjects
{
	/// <summary>
	/// 计算出来的时间区间
	/// </summary>
	public class TimeScope
	{
		/// <summary>
		/// 时间区间的起始时间
		/// </summary>
		public TimeSpan BeginTime
		{
			get;
			set;
		}

		/// <summary>
		/// 时间区间的结束时间
		/// </summary>
		public TimeSpan EndTime
		{
			get;
			set;
		}

		/// <summary>
		/// 比较两个Scope是否相等
		/// </summary>
		/// <param name="scope1"></param>
		/// <param name="scope2"></param>
		/// <returns></returns>
		public static bool Compare(TimeScope scope1, TimeScope scope2)
		{
			bool result = false;

			if (scope1 == null && scope2 == null)
			{
				result = true;
			}
			else
			{
				if (scope1 != null && scope2 != null)
					result = (scope1.BeginTime == scope2.BeginTime && scope1.EndTime == scope2.EndTime);
			}

			return result;
		}

		public bool IsInScope(TimeSpan lastExeTime)
		{
			return this.BeginTime <= lastExeTime && this.EndTime < lastExeTime;
		}
	}
}
