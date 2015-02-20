using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MCS.Library.Core;

namespace MCS.Library.SOA.DataObjects
{
	public enum IntervalUnit
	{
		[EnumItemDescription("小时")]
		Hour = 0,
		[EnumItemDescription("分钟")]
		Minute = 1,
		[EnumItemDescription("秒")]
		Second = 2
	}

	[Serializable]
	[XElementSerializable]
	public class RecurringTimeFrequency : TimeFrequencyBase
	{
		public RecurringTimeFrequency()
		{
		}

		public RecurringTimeFrequency(int interval, IntervalUnit unit, TimeSpan startTime, TimeSpan endTime)
		{
			this.Interval = interval;
			this.Unit = unit;
			this.StartTime = startTime;
			this.EndTime = endTime;
		}

		public TimeSpan StartTime { get; private set; }
		public TimeSpan EndTime { get; private set; }
		public int Interval { get; private set; }
		public IntervalUnit Unit { get; private set; }

		public override string Description
		{
			get
			{
				return string.Format("{0:D2}:{1:D2}:{2:D2} 到 {3:D2}:{4:D2}:{5:D2}之间，每 {6}{7} 执行一次。",
					StartTime.Hours, StartTime.Minutes, StartTime.Seconds,
					EndTime.Hours, EndTime.Minutes, EndTime.Seconds,
					Interval, EnumItemDescriptionAttribute.GetDescription(Unit));
			}
		}

		public TimeSpan GetIntervalTimeSpan()
		{
			TimeSpan result = TimeSpan.Zero;

			switch (this.Unit)
			{
				case IntervalUnit.Hour:
					result = TimeSpan.FromHours(this.Interval);
					break;
				case IntervalUnit.Minute:
					result = TimeSpan.FromMinutes(this.Interval);
					break;
				case IntervalUnit.Second:
					result = TimeSpan.FromSeconds(this.Interval);
					break;
				default:
					throw new NotImplementedException(string.Format("不支持的间隔单位{0}", this.Unit.ToString()));
			}

			return result;
		}

		public override TimeSpan CalculateTime(TimeSpan lastExeTime)
		{
			TimeSpan result;
			switch (this.Unit)
			{
				case IntervalUnit.Hour:
					result = lastExeTime.Add(new TimeSpan(this.Interval, 0, 0));
					break;
				case IntervalUnit.Minute:
					result = lastExeTime.Add(new TimeSpan(0, this.Interval, 0));
					break;
				case IntervalUnit.Second:
					result = lastExeTime.Add(new TimeSpan(0, 0, this.Interval));
					break;
				default:
					throw new NotImplementedException(string.Format("不支持的间隔单位{0}", this.Unit.ToString()));
			}

			return result;
		}

		/// <summary>
		/// 得到某时间段属于的时间周期
		/// </summary>
		/// <param name="timePoint"></param>
		/// <returns></returns>
		public override TimeScope GetTimeScope(TimeSpan timePoint, TimeSpan timeOffset)
		{
			TimeScope result = null;

			if (timePoint >= this.StartTime && timePoint < this.EndTime)
			{
				result = new TimeScope();
				TimeSpan interval = GetIntervalTimeSpan();

				double beginPoint = Math.Floor((timePoint.TotalSeconds - this.StartTime.TotalSeconds) / interval.TotalSeconds) * interval.TotalSeconds;

				result.BeginTime = this.StartTime + TimeSpan.FromSeconds(beginPoint);
				result.EndTime = result.BeginTime + interval;
			}

			return result;
		}

		/// <summary>
		/// 暂时不使用
		/// </summary>
		/// <param name="lastExeTime"></param>
		/// <param name="timeOffset"></param>
		/// <param name="overday"></param>
		/// <returns></returns>
		public override TimeScope GetNextTimeScope(TimeSpan lastExeTime, TimeSpan timeOffset, out bool overday)
		{
			TimeScope execTimeScope = GetTimeScope(lastExeTime, timeOffset);
			TimeSpan nextTime;

			overday = false;

			if (execTimeScope != null)
			{
				nextTime = lastExeTime.Add(GetIntervalTimeSpan());

				if (nextTime > this.EndTime)
					overday = true;
				else
					nextTime = this.StartTime;
			}
			else
			{
				nextTime = this.StartTime;
			}

			return GetTimeScope(nextTime, timeOffset);
		}

		public override TimeSpan GetMinExecuteInterval()
		{
			return GetIntervalTimeSpan();
		}
	}
}
