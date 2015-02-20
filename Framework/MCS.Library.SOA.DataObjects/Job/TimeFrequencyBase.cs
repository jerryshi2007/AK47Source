using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MCS.Library.Core;

namespace MCS.Library.SOA.DataObjects
{
	[Serializable]
	[XElementSerializable]
	public abstract class TimeFrequencyBase
	{
		/// <summary>
		/// 
		/// </summary>
		/// <param name="basicTime"></param>
		/// <returns></returns>
		public abstract TimeSpan CalculateTime(TimeSpan lastExeTime);

		/// <summary>
		/// 根据上次执行时间和预计时间，判断预计时间是否是下次执行时间
		/// </summary>
		/// <param name="lastExeTime"></param>
		/// <param name="nextCheckPoint"></param>
		/// <returns></returns>
		public bool IsNextExecuteTime(TimeSpan lastExeTime, TimeSpan nextCheckPoint, TimeSpan timeOffset)
		{
			TimeScope lastScope = GetTimeScope(lastExeTime, timeOffset);
			TimeScope nextScope = GetTimeScope(nextCheckPoint, timeOffset);

			bool result = false;

			if (nextScope != null)
			{
				result = TimeScope.Compare(lastScope, nextScope) == false;

				if (result)
					result = (nextCheckPoint - GetMinExecuteInterval()) >= lastExeTime;
			}

			return result;
		}

		/// <summary>
		/// 得到下一个执行周期，如果跨天，则overday返回true(暂时不使用)
		/// </summary>
		/// <param name="lastExeTime"></param>
		/// <param name="timeOffset"></param>
		/// <param name="overday"></param>
		/// <returns></returns>
		public abstract TimeScope GetNextTimeScope(TimeSpan lastExeTime, TimeSpan timeOffset, out bool overday);

		/// <summary>
		/// 获取时间频率的文字描述
		/// </summary>
		public abstract string Description { get; }

		/// <summary>
		/// 根据时间点计算属于哪个时间区间，如果返回null，则表示没有找到时间区间
		/// </summary>
		/// <param name="ts"></param>
		/// <returns></returns>
		public abstract TimeScope GetTimeScope(TimeSpan timePoint, TimeSpan timeOffset);

		/// <summary>
		/// 得到最小执行间隔。避免即使在两个Scope中，也过于高密度的执行
		/// </summary>
		/// <returns></returns>
		public virtual TimeSpan GetMinExecuteInterval()
		{
			return TimeSpan.Zero;
		}
	}
}
