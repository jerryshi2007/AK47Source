using System;
using System.Collections.Generic;
using System.Text;
using System.Configuration;

namespace MCS.Library.Services
{
	public class WorkScheduleConfigureElement : ConfigurationElement
	{
		[ConfigurationProperty("checkPoints", IsRequired = true)]
		public string CheckPoints
		{
			get
			{
				return (string)this["checkPoints"];
			}
		}

		[ConfigurationProperty("timeScope", DefaultValue = "00:00:10")]
		public TimeSpan TimeScope
		{
			get
			{
				return (TimeSpan)this["timeScope"];
			}
		}

		[ConfigurationProperty("enabled", IsRequired = false, DefaultValue = false)]
		public bool Enabled
		{
			get
			{
				return (bool)this["enabled"];
			}
		}

		/// <summary>
		/// 判断是否已到任务执行点
		/// </summary>
		/// <returns></returns>
		public bool IsCheckPointNeared()
		{
			if (this.GetNearestDailyTimes(this.CheckPoints, this.TimeScope) != DateTime.MinValue)
				return true;
			else
				return false;
		}

		/// <summary>
		/// 获取指定的系列时间点中，在当前时间一定范围前后的，最早的那个时间点
		/// </summary>
		/// <param name="strTimes">指定的时间点串（以","或者";"分隔）</param>
		/// <param name="timeScope">时间范围</param>
		/// <returns>符合条件的时间点，如果没有则返回DateTime.MinValue</returns>
		private DateTime GetNearestDailyTimes(string strTimes, TimeSpan timeScope)
		{
			DateTime result = DateTime.MinValue;

			if (string.IsNullOrEmpty(strTimes) == false)
			{
				if (string.Compare(strTimes, "now", true) != 0)
				{
					string[] timeArray = strTimes.Split(',', ';');

					DateTime dtCurrent = DateTime.Now;

					for (int i = 0; i < timeArray.Length; i++)
					{
						DateTime dtDefined = GetDateTimeFromTimeString(timeArray[i]);

						if (dtDefined > dtCurrent.AddSeconds(-timeScope.TotalSeconds) && dtDefined <= dtCurrent.AddSeconds(timeScope.TotalSeconds)) //在当前时间的前n分钟(相当于每个检查点范围内当前时间以内)
							if (dtDefined > result) //取一个离当前时间最接近的时间点
								result = dtDefined;
					}
				}
				else
					result = DateTime.Now;
			}

			return result;
		}

		/// <summary>
		/// 把字符串时间strTime对应转换为DateTime时间对象
		/// </summary>
		/// <param name="?"></param>
		/// <returns></returns>
		private DateTime GetDateTimeFromTimeString(string strTime)
		{
			try
			{
				strTime = strTime.Trim();

				int nYear = DateTime.Now.Year;
				int nMonth = DateTime.Now.Month;
				int nDay = DateTime.Now.Day;

				string[] timePart = strTime.Split(':');

				int nHour = GetTimePart(timePart, 0);
				int nMin = GetTimePart(timePart, 1);
				int nSecond = GetTimePart(timePart, 2);

				return new DateTime(nYear, nMonth, nDay, nHour, nMin, nSecond);
			}
			catch (Exception ex)
			{
				throw new FormatException(string.Format("时间格式错误\"{0}\"", strTime), ex);
			}
		}

		/// <summary>
		/// 获取timePart字符串数据中的第nIndex项数据（时、分、秒）
		/// </summary>
		/// <param name="timePart"></param>
		/// <param name="nIndex"></param>
		/// <returns></returns>
		private int GetTimePart(string[] timePart, int nIndex)
		{
			int nTime = 0;

			if (nIndex < timePart.Length)
				nTime = int.Parse(timePart[nIndex]);

			return nTime;
		}
	}
}
