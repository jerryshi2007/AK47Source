using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MCS.Library.Core;

namespace MCS.Library.SOA.DataObjects
{
	/// <summary>
	/// <remarks>本周不计算在内</remarks>
	/// </summary>
	[Serializable]
	[XElementSerializable]
	public class WeeklyJobScheduleFrequency : JobScheduleFrequencyBase
	{
		public WeeklyJobScheduleFrequency()
		{
		}

		/// <summary>
		/// 构造方法
		/// </summary>
		/// <param name="dayOfWeeks">每周几</param>
		/// <param name="durWeeks">每隔几周</param>
		/// <param name="timeFrequency">时间频率</param>
		public WeeklyJobScheduleFrequency(List<DayOfWeek> dayOfWeeks, int durWeeks, TimeFrequencyBase timeFrequency)
		{
			this._daysOfWeek = dayOfWeeks;
			this.DurationWeeks = durWeeks;
			this.FrequencyTime = timeFrequency;
		}

		private List<DayOfWeek> _daysOfWeek;

		/// <summary>
		/// 每周几
		/// </summary>
		public List<DayOfWeek> DaysOfWeek
		{
			get
			{
				if (_daysOfWeek == null)
					this._daysOfWeek = new List<DayOfWeek>();

				return _daysOfWeek;
			}
			private set
			{
				this._daysOfWeek = value;
			}
		}

		/// <summary>
		/// 每隔几周
		/// </summary>
		public int DurationWeeks
		{
			get;
			set;
		}

		public override string Description
		{
			get
			{
				StringBuilder strBuilder = new StringBuilder();

				if (DaysOfWeek.Count > 0)
				{
					strBuilder.Append("的");

					for (int i = 0; i < DaysOfWeek.Count; i++)
					{
						strBuilder.Append(GetCNDayOfWeek(DaysOfWeek[i]));
						strBuilder.Append(",");
					}

					strBuilder.Remove(strBuilder.Length - 1, 1);
				}

				return string.Format("每{0}周{1}，{2}", DurationWeeks, strBuilder.ToString(), FrequencyTime.Description);
			}
		}

		protected override bool DateIsMatched(DateTime startTime, DateTime timePoint)
		{
			bool result = false;

			DateTime normalizedStartTime = startTime.AddDays(-((int)startTime.DayOfWeek - (int)DayOfWeek.Sunday));

			normalizedStartTime = normalizedStartTime.Date;

			int weeks = (int)((timePoint - normalizedStartTime).TotalDays / 7);

			if (weeks % this.DurationWeeks == 0)
			{
				result = this.DaysOfWeek.Exists(weekDay => timePoint.DayOfWeek == weekDay);
			}

			return result;
		}

		/*
		protected override DateTime ApplyStrategy(DateTime basicDateTime)
		{
			if (this.DaysOfWeek.Count == 0 || this.DaysOfWeek.Count(p => p == basicDateTime.DayOfWeek) > 0)
			{
				return basicDateTime;
			}

			while (true)
			{
				basicDateTime = basicDateTime.AddDays(1d);
				if (this.DaysOfWeek.Count(p => p == basicDateTime.DayOfWeek) > 0)
				{
					var fixedTime = this.FrequencyTime as FixedTimeFrequency;

					if (fixedTime != null)
					{
						return CreateDateTime(basicDateTime, fixedTime.OccurTime);
					}
					else
					{
						var recurringTime = (RecurringTimeFrequency)this.FrequencyTime;
						return CreateDateTime(basicDateTime, recurringTime.StartTime);
					}
				}
			}
		}

		protected override int GetDurationDays()
		{
			return this.DurationWeeks * 7;
		}

		protected override DateTime PlusDurationDateTime(DateTime basicDateTime)
		{
			return basicDateTime.AddDays(GetDurationDays());
		}
		*/

		private static string GetCNDayOfWeek(DayOfWeek dayWeek)
		{
			string result = string.Empty;

			switch (dayWeek)
			{
				case DayOfWeek.Monday:
					result = "星期一";
					break;
				case DayOfWeek.Tuesday:
					result = "星期二";
					break;
				case DayOfWeek.Wednesday:
					result = "星期三";
					break;
				case DayOfWeek.Thursday:
					result = "星期四";
					break;
				case DayOfWeek.Friday:
					result = "星期五";
					break;
				case DayOfWeek.Saturday:
					result = "星期六";
					break;
				case DayOfWeek.Sunday:
					result = "星期日";
					break;
			}

			return result;
		}
	}
}
