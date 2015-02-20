using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MCS.Library.Core;

namespace MCS.Library.SOA.DataObjects
{
	[Serializable]
	[XElementSerializable]
	public class DailyJobScheduleFrequency : JobScheduleFrequencyBase
	{
		public DailyJobScheduleFrequency()
		{
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="days">每隔几天</param>
		/// <param name="frequencyTime">时间频率</param>
		public DailyJobScheduleFrequency(int days, TimeFrequencyBase frequencyTime)
		{
			this.DurationDays = days;
			this.FrequencyTime = frequencyTime;
		}

		/// <summary>
		/// 每隔几天
		/// </summary>
		public int DurationDays
		{
			get;
			private set;
		}

		public override string Description
		{
			get
			{
				return string.Format("每{0}天，{1}", DurationDays, FrequencyTime.Description);
			}
		}

		protected override bool DateIsMatched(DateTime startTime, DateTime timePoint)
		{
			return (Math.Floor((timePoint - startTime).TotalDays) % this.DurationDays == 0);
		}

		/*
		protected override int GetDurationDays()
		{
			return this.DurationDays;
		}

		protected override DateTime PlusDurationDateTime(DateTime basicDateTime)
		{
			return basicDateTime.AddDays(this.DurationDays);
		}

		protected override DateTime ApplyStrategy(DateTime basicDateTime)
		{
			return basicDateTime;
		}
		*/
	}
}

