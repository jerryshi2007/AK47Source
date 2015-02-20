using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Diagnostics;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace MCS.Library.SOA.DataObjects.Test.Schedules
{
	internal static class ScheduleUtil
	{
		/// <summary>
		/// 缺省的最后执行时间2013年7月11日9:5
		/// </summary>
		public static readonly DateTime DefaultLastExecuteTime = new DateTime(2013, 7, 11, 9, 5, 0);
		public static readonly TimeSpan timeOffset = new TimeSpan(0, 0, 30);

		public static InvokeWebServiceJob CreateDefaultJob()
		{
			return new InvokeWebServiceJob() { LastExecuteTime = ScheduleUtil.DefaultLastExecuteTime };
		}

		public static void StartTest(InvokeWebServiceJob job, DateTime[] checkPoints, bool[] asserts)
		{
			Debug.Assert(checkPoints.Length == asserts.Length);

			int len = checkPoints.Length;

			for (int i = 0; i < len; i++)
			{
				bool result = false;
				foreach (JobSchedule schedule in job.Schedules)
				{
					Debug.Assert(job.LastExecuteTime <= checkPoints[i], "上次执行时间不应大于本次执行时间。");
					result = schedule.IsNextExecuteTime(job.LastExecuteTime.Value, checkPoints[i], timeOffset);

					if (result)
						break;
				}

				if (result != asserts[i] && Debugger.IsAttached)
					Debugger.Break();

				Assert.AreEqual(asserts[i], result, string.Format("i={0}:时间点{1}执行情况与预期不一致。", i.ToString(), checkPoints[i]));

				if (result)
				{
					job.LastExecuteTime = checkPoints[i];
				}
			}
		}

		internal static List<DayOfWeek> Weeks(int first)
		{
			List<DayOfWeek> weeks = new List<DayOfWeek>() { (DayOfWeek)first };

			return weeks;
		}

		internal static List<DayOfWeek> Weeks(int first, int second)
		{
			List<DayOfWeek> weeks = new List<DayOfWeek>() { (DayOfWeek)first, (DayOfWeek)second };

			return weeks;
		}

		internal static List<DayOfWeek> Weeks(int first, int second, int third)
		{
			List<DayOfWeek> weeks = new List<DayOfWeek>() { (DayOfWeek)first, (DayOfWeek)second, (DayOfWeek)third };

			return weeks;
		}

		internal static List<DayOfWeek> Weeks(int first, int second, int third, int forth)
		{
			List<DayOfWeek> weeks = new List<DayOfWeek>() { (DayOfWeek)first, (DayOfWeek)second, (DayOfWeek)third, (DayOfWeek)forth };

			return weeks;
		}

		internal static List<DayOfWeek> Weeks(int first, int second, int third, int forth, int fifth)
		{
			List<DayOfWeek> weeks = new List<DayOfWeek>() { (DayOfWeek)first, (DayOfWeek)second, (DayOfWeek)third, (DayOfWeek)forth, (DayOfWeek)fifth };

			return weeks;
		}
	}
}
