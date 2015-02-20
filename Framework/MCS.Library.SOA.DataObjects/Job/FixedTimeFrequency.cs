using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MCS.Library.Core;

namespace MCS.Library.SOA.DataObjects
{
	[Serializable]
	[XElementSerializable]
	public class FixedTimeFrequency : TimeFrequencyBase
	{
		public FixedTimeFrequency()
		{
		}

		public FixedTimeFrequency(TimeSpan time)
		{
			this.OccurTime = time;
		}

		public TimeSpan OccurTime
		{
			get;
			private set;
		}

		public override string Description
		{
			get
			{
				return string.Format("在 {0:D2}:{1:D2}:{2:D2} 执行。", OccurTime.Hours, OccurTime.Minutes, OccurTime.Seconds);
			}
		}

		public override TimeSpan CalculateTime(TimeSpan lastExeTime)
		{
			return OccurTime;
		}

		public override TimeScope GetTimeScope(TimeSpan timePoint, TimeSpan timeOffset)
		{
			TimeScope result = null;

			if (timePoint >= this.OccurTime - timeOffset && timePoint < this.OccurTime + timeOffset)
				result = CreateTimeScope(timeOffset);

			return result;
		}

		/// <summary>
		/// 暂时不使用
		/// </summary>
		/// <param name="lastExeTime"></param>
		/// <param name="overday"></param>
		/// <returns></returns>
		public override TimeScope GetNextTimeScope(TimeSpan lastExeTime, TimeSpan timeOffset, out bool overday)
		{
			TimeScope result = GetTimeScope(lastExeTime, timeOffset);
			overday = false;

			if (result != null)
				overday = true;
			else
				result = CreateTimeScope(timeOffset);

			return result;
		}

		private TimeScope CreateTimeScope(TimeSpan timeOffset)
		{
			TimeScope result = new TimeScope();

			result.BeginTime = this.OccurTime - timeOffset;
			result.EndTime = this.OccurTime + timeOffset;

			return result;
		}
	}
}
