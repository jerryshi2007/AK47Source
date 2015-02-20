using System;
using System.Text;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using MCS.Library.Core;

namespace MCS.Library.SOA.DataObjects.Test.JobTest
{
	[TestClass]
	public class JobScheduleFrequencyTest
	{
		//[TestMethod]
		//[TestCategory("Schedule")]
		//public void DailyJobScheduleFrequencyTest()
		//{
		//    int dailyDuration = 5;
		//    var fixedTime = new TimeSpan(DateTime.Now.Hour - 1, 0, 0);
		//    var recurringBeginTime = new TimeSpan(10, 0, 0);
		//    var recurringEndTime = new TimeSpan(22, 0, 0);

		//    TimeFrequencyBase timeFrequency = new FixedTimeFrequency(fixedTime);
		//    JobScheduleFrequencyBase daily = new DailyJobScheduleFrequency(dailyDuration, timeFrequency);
		//    ShowFrequencyExecuteResult(daily, 8, 20);

		//    fixedTime = new TimeSpan(DateTime.Now.Hour + 1, 0, 0);
		//    timeFrequency = new FixedTimeFrequency(fixedTime);
		//    daily = new DailyJobScheduleFrequency(dailyDuration, timeFrequency);
		//    ShowFrequencyExecuteResult(daily, 8, 20);

		//    timeFrequency = new RecurringTimeFrequency(30, IntervalUnit.Minute, recurringBeginTime, recurringEndTime);
		//    daily = new DailyJobScheduleFrequency(dailyDuration, timeFrequency);
		//    ShowFrequencyExecuteResult(daily, 8, 23);

		//    SerializeTest(daily);
		//}

		//[TestMethod]
		//[TestCategory("Schedule")]
		//public void WeeklyJobScheduleFrequencyTest()
		//{
		//    int durationWeeks = 1;
		//    List<DayOfWeek> weekList = new List<DayOfWeek>() {DayOfWeek.Tuesday,DayOfWeek.Wednesday,
		//     DayOfWeek.Thursday,DayOfWeek.Friday,DayOfWeek.Saturday,DayOfWeek.Sunday };
		//    var fixedTime = new TimeSpan(DateTime.Now.Hour - 1, 0, 0);
		//    var recurringBeginTime = new TimeSpan(10, 0, 0);
		//    var recurringEndTime = new TimeSpan(22, 0, 0);

		//    TimeFrequencyBase timeFrequency = new FixedTimeFrequency(fixedTime);
		//    JobScheduleFrequencyBase weekly = new WeeklyJobScheduleFrequency(weekList, durationWeeks, timeFrequency);
		//    ShowFrequencyExecuteResult(weekly, 8, 23);

		//    fixedTime = new TimeSpan(DateTime.Now.Hour + 1, 0, 0);
		//    timeFrequency = new FixedTimeFrequency(fixedTime);
		//    weekly = new WeeklyJobScheduleFrequency(weekList, durationWeeks, timeFrequency);
		//    ShowFrequencyExecuteResult(weekly, 8, 20);

		//    timeFrequency = new RecurringTimeFrequency(1, IntervalUnit.Hour, fixedTime, new TimeSpan(22, 0, 0));
		//    weekly = new WeeklyJobScheduleFrequency(weekList, durationWeeks, timeFrequency);
		//    ShowFrequencyExecuteResult(weekly, 8, 23);

		//    SerializeTest(weekly);
		//}

		//[TestMethod]
		//[TestCategory("Job")]
		//public void MonthlyJobScheduleFrequencyTest()
		//{
		//    int occurDay = DateTime.Now.Day;
		//    int durationMonth = 1;
		//    var fixedTime = new TimeSpan(DateTime.Now.Hour - 1, 0, 0);
		//    var recurringBeginTime = new TimeSpan(10, 0, 0);
		//    var recurringEndTime = new TimeSpan(22, 0, 0);

		//    TimeFrequencyBase timeFrequency = new FixedTimeFrequency(fixedTime);
		//    JobScheduleFrequencyBase monthly = new MonthlyJobScheduleFrequency(occurDay, durationMonth, timeFrequency);
		//    ShowFrequencyExecuteResult(monthly, 8, 23);

		//    fixedTime = new TimeSpan(DateTime.Now.Hour + 1, 0, 0);
		//    timeFrequency = new FixedTimeFrequency(fixedTime);
		//    monthly = new MonthlyJobScheduleFrequency(occurDay, durationMonth, timeFrequency);
		//    ShowFrequencyExecuteResult(monthly, 8, 23);

		//    timeFrequency = new RecurringTimeFrequency(3, IntervalUnit.Hour, fixedTime, new TimeSpan(22, 0, 0));
		//    monthly = new MonthlyJobScheduleFrequency(occurDay, durationMonth, timeFrequency);
		//    ShowFrequencyExecuteResult(monthly, 8, 23);

		//    SerializeTest(monthly);
		//}

		//private void SerializeTest(JobScheduleFrequencyBase freqObj)
		//{
		//    Console.WriteLine("序列化前信息：{0}", freqObj.Description);

		//    XElementFormatter formatter = new XElementFormatter();
		//    var element = formatter.Serialize(freqObj);

		//    Console.WriteLine("序列化结果：{0}", element.ToString());

		//    var deseObj = formatter.Deserialize(element) as JobScheduleFrequencyBase;

		//    Console.WriteLine("反序列化信息：{0}", deseObj.Description);
		//}

		//private static void ShowFrequencyExecuteResult(JobScheduleFrequencyBase daily, int startHour, int endHour)
		//{
		//    Console.WriteLine(daily.Description);

		//    DateTime lastExecDate = new DateTime(2011, 1, 1);
		//    var result = daily.CalculateDate(lastExecDate);
		//    Console.WriteLine("上次执行时间{0}，下次执行时间{1}，下次执行是{2}，当前时间{3}", lastExecDate, result, result.DayOfWeek, DateTime.Now);

		//    Console.WriteLine("**********************");
		//    lastExecDate = new DateTime(DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day, startHour, 0, 0);
		//    for (int i = 0; i < 30; i++)
		//    {
		//        result = daily.CalculateDate(lastExecDate);
		//        Console.WriteLine("上次执行时间{0}，下次执行时间{1}，下次执行是{2}，当前时间{3}", lastExecDate, result, result.DayOfWeek, DateTime.Now);
		//        lastExecDate = result;
		//    }

		//    Console.WriteLine("**********************");
		//    lastExecDate = new DateTime(DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day, endHour, 0, 0);
		//    for (int i = 0; i < 30; i++)
		//    {
		//        result = daily.CalculateDate(lastExecDate);
		//        Console.WriteLine("上次执行时间{0}，下次执行时间{1}，下次执行是{2}，当前时间{3}", lastExecDate, result, result.DayOfWeek, DateTime.Now);
		//        lastExecDate = result;
		//    }

		//    Console.WriteLine();
		//}

		//[TestMethod]
		//public void OtherTest()
		//{
		//    var timeFre = new RecurringTimeFrequency(2, IntervalUnit.Hour, new TimeSpan(), new TimeSpan(23, 59, 59));
		//    var daily = new DailyJobScheduleFrequency(1, timeFre);
		//    var lastExecDate = CreateDateTime(DateTime.Now, new TimeSpan(3, 0, 0));
		//    var result = daily.CalculateDate(lastExecDate);
		//    Console.WriteLine(result);
		//}

		//private DateTime CreateDateTime(DateTime date, TimeSpan time)
		//{
		//    return new DateTime(date.Year, date.Month, date.Day, time.Hours, time.Minutes, time.Seconds);
		//}
	}
}
