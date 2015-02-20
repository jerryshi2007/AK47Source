using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MCS.Library.Core;

namespace MCS.Library.SOA.DataObjects
{
	[Serializable]
	[XElementSerializable]
	public class MonthlyJobScheduleFrequency : JobScheduleFrequencyBase
	{
		public MonthlyJobScheduleFrequency()
		{
		}

		/// <summary>
		/// 构造方法，每隔几个月，第几天执行
		/// </summary>
		/// <param name="day"></param>
		/// <param name="durMonths"></param>
		/// <param name="timeFrequency"></param>
		public MonthlyJobScheduleFrequency(int day, int durMonths, TimeFrequencyBase timeFrequency)
		{
			this.Day = day;
			this.DurationMonths = durMonths;
			this.FrequencyTime = timeFrequency;
		}

		/// <summary>
		/// 每隔几个月
		/// </summary>
		public int DurationMonths
		{
			get;
			private set;
		}

		/// <summary>
		/// 第几天
		/// </summary>
		public int Day
		{
			get;
			private set;
		}

		public override string Description
		{
			get
			{
				return string.Format("每{0}个月的第{1}天，{2}", DurationMonths, Day, FrequencyTime.Description);
			}
		}

		protected override bool DateIsMatched(DateTime startTime, DateTime timePoint)
		{
			bool result = false;

			if ((GetYearMonthValue(timePoint) - GetYearMonthValue(startTime)) % this.DurationMonths == 0)
				result = (timePoint.Day == this.Day);

			return result;
		}

		private static int GetYearMonthValue(DateTime date)
		{
			return date.Year * 12 + date.Month;
		}

		/*
		protected override DateTime ApplyStrategy(DateTime basicDateTime)
		{
			if (basicDateTime.Day == this.Day)
			{
				return basicDateTime;
			}

			DateTime result = new DateTime(basicDateTime.Year, basicDateTime.Month, GetOccurDay(basicDateTime),
				basicDateTime.Hour, basicDateTime.Minute, basicDateTime.Second);

			if (result < DateTime.Now)
			{
				result = basicDateTime.AddMonths(this.DurationMonths);

				result = new DateTime(result.Year, result.Month, GetOccurDay(result),
					result.Hour, result.Minute, result.Second);
			}

			return result;
		}

		protected override int GetDurationDays()
		{
			return this.DurationMonths * 30;
		}

		protected override DateTime PlusDurationDateTime(DateTime basicDateTime)
		{
			return basicDateTime.AddMonths(this.DurationMonths);
		}

		private int GetOccurDay(DateTime basicDateTime)
		{
			int daysInMonth = DateTime.DaysInMonth(basicDateTime.Year, basicDateTime.Month);

			return this.Day > daysInMonth ? daysInMonth : this.Day;
		}
		*/
	}
}
