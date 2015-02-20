using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MCS.Library.Data.DataObjects;
using MCS.Library.Core;
using System.Diagnostics;

namespace MCS.Library.SOA.DataObjects
{
	[Serializable]
	[XElementSerializable]
	public abstract class JobScheduleFrequencyBase
	{
		public string ID
		{
			get;
			set;
		}

		public abstract string Description
		{
			get;
		}

		public TimeFrequencyBase FrequencyTime
		{
			get;
			protected set;
		}

		/// <summary>
		/// 预估后面多少次的执行时间
		/// </summary>
		/// <param name="startTime"></param>
		/// <param name="timeOffset"></param>
		/// <param name="count"></param>
		/// <param name="timeout">计算多久后会超时</param>
		/// <returns></returns>
		public List<DateTime> EstimateExecuteTime(DateTime startTime, TimeSpan timeOffset, int maxCount, TimeSpan timeout)
		{
			Stopwatch sw = new Stopwatch();

			List<DateTime> result = new List<DateTime>();

			DateTime lastExeTime = DateTime.Now;
			DateTime checkPoint = lastExeTime;

			int count = 0;

			sw.Start();

			while (count < maxCount && sw.Elapsed < timeout)
			{
				Debug.Write(checkPoint);
				if (this.IsNextExecuteTime(startTime, lastExeTime, checkPoint, timeOffset))
				{
					Debug.WriteLine("→OK");
					result.Add(checkPoint);
					lastExeTime = checkPoint;
					count++;
				}
				else
				{
					Debug.WriteLine("→Fail");
				}

				checkPoint = checkPoint.Add(GetEstimateSampleTime());
			}

			return result;
		}

		/*
		public DateTime LastModifyTime
		{
			get;
			set;
		}
		*/

		public TimeScope GetTimeScope(DateTime startTime, DateTime timePoint, TimeSpan timeOffset)
		{
			TimeScope result = null;

			if (DateIsMatched(startTime, timePoint))
			{
				if (FrequencyTime != null)
					result = FrequencyTime.GetTimeScope(timePoint.TimeOfDay, timeOffset);
			}

			return result;
		}

		/// <summary>
		/// 根据上次执行时间，判断检查点是否符合执行时间
		/// </summary>
		/// <param name="startTime"></param>
		/// <param name="lastExeTime"></param>
		/// <param name="nextCheckPoint"></param>
		/// <returns></returns>
		public bool IsNextExecuteTime(DateTime startTime, DateTime lastExeTime, DateTime nextCheckPoint, TimeSpan timeOffset)
		{
			bool result = false;

			if (lastExeTime.Date != nextCheckPoint.Date)
			{
				//上次执行时间和检查点不是同一天
				TimeScope scope = GetTimeScope(startTime, nextCheckPoint, timeOffset);

				result = scope != null;
			}
			else
			{
				//上次执行时间和检查点是同一天
				if (DateIsMatched(startTime, nextCheckPoint) && FrequencyTime != null)
					result = FrequencyTime.IsNextExecuteTime(lastExeTime.TimeOfDay, nextCheckPoint.TimeOfDay, timeOffset);
			}

			return result;
		}

		/// <summary>
		/// 日期是否匹配
		/// </summary>
		/// <param name="startTime"></param>
		/// <param name="timePoint"></param>
		/// <returns></returns>
		protected abstract bool DateIsMatched(DateTime startTime, DateTime timePoint);

		/// <summary>
		/// 得到预估时间时的采样周期
		/// </summary>
		/// <returns></returns>
		protected virtual TimeSpan GetEstimateSampleTime()
		{
			return TimeSpan.FromSeconds(20);
		}

		//internal DateTime StartDate
		//{
		//    get;
		//    set;
		//}

		/*
		/// <summary>
		/// 计算计划执行时间
		/// </summary>
		/// <param name="lastExecuteDate"></param>
		/// <returns></returns>
		public virtual DateTime CalculateDate(DateTime lastExecuteDate)
		{
			DateTime result;

			if (IsLastExeDateBeyondDuration(lastExecuteDate))
				result = CalculateDateWhenBeyondDuration();
			else
				result = CalculateDateWhenNormal(lastExecuteDate);

			return result;
		}
		*/

		//protected bool IsStartInToday()
		//{
		//    return this.StartDate.ToShortDateString() == DateTime.Now.ToShortDateString();
		//}

		/*
		/// <summary>
		/// 最后执行时间是否超过了Duration天数
		/// </summary>
		/// <param name="lastExeDate"></param>
		/// <returns></returns>
		protected virtual bool IsLastExeDateBeyondDuration(DateTime lastExeDate)
		{
			return (DateTime.Now - lastExeDate).Days > GetDurationDays();
		}

		/// <summary>
		/// 最后执行时间超过预定义时间周期时，计算时间的方法。
		/// </summary>
		/// <returns></returns>
		protected virtual DateTime CalculateDateWhenBeyondDuration()
		{
			(this.FrequencyTime != null).FalseThrow("时间执行频率不能为空");

			DateTime result;
			DateTime now = DateTime.Now;

			//是否是固定时间点
			if (this.FrequencyTime is FixedTimeFrequency)
			{
				FixedTimeFrequency frequency = (FixedTimeFrequency)this.FrequencyTime;
				DateTime tempDateTime = now;

				if (frequency.OccurTime < now.TimeOfDay)	//如果执行时间已经过去，则改为下一天
					tempDateTime = now.AddDays(1);

				result = CreateDateTime(tempDateTime, frequency.OccurTime);
			}
			else
			{
				if (this.FrequencyTime is RecurringTimeFrequency)
				{
					RecurringTimeFrequency frequency = (RecurringTimeFrequency)this.FrequencyTime;

					if (now.TimeOfDay < frequency.StartTime)			//尚未到达执行期间
					{
						result = CreateDateTime(now, frequency.StartTime);
					}
					else if (frequency.StartTime <= now.TimeOfDay &&	//正处于执行期间
						now.TimeOfDay <= frequency.EndTime)
					{
						result = now;
					}
					else   //已超出执行期间
					{
						result = CreateDateTime(now.AddDays(1), frequency.StartTime);
					}
				}
				else
					throw new NotSupportedException(string.Format("不支持的时间频率类型{0}", this.FrequencyTime.GetType().FullName));
			}

			return ApplyStrategy(result);
		}

		/// <summary>
		/// 正常的计算日期方法
		/// </summary>
		/// <param name="lastExecuteDate"></param>
		/// <returns></returns>
		protected virtual DateTime CalculateDateWhenNormal(DateTime lastExecuteDate)
		{
			DateTime now = DateTime.Now;

			FixedTimeFrequency fixedTimeFre = this.FrequencyTime as FixedTimeFrequency;

			if (fixedTimeFre != null)
			{
				//DateTime result = lastExecuteDate;

				//if (IsStartInToday() == false)
				DateTime result = PlusDurationDateTime(lastExecuteDate);

				result = CreateDateTime(result, fixedTimeFre.OccurTime);

				if (result < now)					//执行时间已过
					result = result.AddDays(1);

				return ApplyStrategy(result);
			}

			RecurringTimeFrequency recurringTimeFre = (RecurringTimeFrequency)this.FrequencyTime;
			DateTime tempDateTime = lastExecuteDate;
			TimeSpan tempTime = lastExecuteDate.TimeOfDay;
			bool isExecCurrentDay = false;

			if (lastExecuteDate.Date == DateTime.Now.Date)	//当天执行过
			{
				tempTime = recurringTimeFre.CalculateTime(tempTime);
				isExecCurrentDay = true;
			}
			else
			{
				tempDateTime = PlusDurationDateTime(lastExecuteDate);
				tempDateTime = CreateDateTime(tempDateTime, recurringTimeFre.StartTime);
			}

			if (now.Date < tempDateTime.Date)
			{
				tempDateTime = CreateDateTime(tempDateTime, recurringTimeFre.StartTime);

				return ApplyStrategy(tempDateTime);
			}

			//当天没运行过
			if (isExecCurrentDay == false)
			{
				return ApplyStrategy(tempDateTime);
			}

			tempTime = tempTime > DateTime.Now.TimeOfDay ? tempTime : DateTime.Now.TimeOfDay;

			if (tempTime < recurringTimeFre.StartTime)		//尚未到达执行开始时间
			{
				tempDateTime = CreateDateTime(tempDateTime, recurringTimeFre.StartTime);
			}
			else if (recurringTimeFre.StartTime <= tempTime && tempTime <= recurringTimeFre.EndTime) //正处于执行期间
			{
				tempDateTime = CreateDateTime(tempDateTime, tempTime);
			}
			else //超出执行期间
			{
				if (lastExecuteDate.Date == DateTime.Now.Date)
				{
					tempDateTime = PlusDurationDateTime(lastExecuteDate);
				}
				else
				{
					tempDateTime = tempDateTime.AddDays(1);
				}

				tempDateTime = CreateDateTime(tempDateTime, recurringTimeFre.StartTime);
			}

			return ApplyStrategy(tempDateTime);
		}
		*/

		/*
		/// <summary>
		/// 获取周期相应的天数
		/// </summary>
		/// <returns></returns>
		protected abstract int GetDurationDays();

		/// <summary>
		/// 获取加上执行周期后的时间
		/// </summary>
		/// <param name="basicDateTime"></param>
		/// <returns></returns>
		protected abstract DateTime PlusDurationDateTime(DateTime basicDateTime);

		/// <summary>
		/// 应用计划策略
		/// </summary>
		/// <param name="basicDateTime"></param>
		/// <returns></returns>
		protected abstract DateTime ApplyStrategy(DateTime basicDateTime);

		protected static DateTime CreateDateTime(DateTime date, TimeSpan time)
		{
			return new DateTime(date.Year, date.Month, date.Day, time.Hours, time.Minutes, time.Seconds, DateTimeKind.Local);
		}
		*/
	}
}
