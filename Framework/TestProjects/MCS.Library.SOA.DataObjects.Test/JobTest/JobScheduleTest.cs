using System;
using System.Text;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace MCS.Library.SOA.DataObjects.Test.JobTest
{
	[TestClass]
	public class JobScheduleTest
	{
		//[TestMethod]
		//[TestCategory("Schedule")]
		//[Description("每周二、五，每天1点到21点，每隔20秒执行一次")]
		//public void GetWeekDayScheduleTimeTest()
		//{
		//    string schID = Guid.NewGuid().ToString();
		//    string schName = "TestSchedule" + DateTime.Now.ToString();
		//    DateTime sTime = new DateTime(2011, 4, 1);

		//    TimeFrequencyBase timeFrequency = new RecurringTimeFrequency(20, IntervalUnit.Minute,
		//        new TimeSpan(1, 0, 0), new TimeSpan(21, 0, 0));

		//    List<DayOfWeek> daysOfWeek = new List<DayOfWeek>(){ 
		//        DayOfWeek.Tuesday, DayOfWeek.Friday};
		//    WeeklyJobScheduleFrequency schFqc = new WeeklyJobScheduleFrequency(daysOfWeek, 1, timeFrequency);
		//    JobSchedule schedule = new JobSchedule(schID, schName, sTime, schFqc);

		//    Console.WriteLine(schedule.Description);
		//    for (int i = 0; i < 30; i++)
		//    {
		//        sTime = schedule.GetScheduleTime(sTime);
		//        Console.WriteLine(sTime.ToString());
		//    }
		//}

		//[TestMethod]
		//[TestCategory("Schedule")]
		//[Description("间隔5天，每天1点到21点，每隔20秒执行一次")]
		//public void GetDayIntervalScheduleTimeTest()
		//{
		//    string schID = Guid.NewGuid().ToString();
		//    string schName = "TestSchedule" + DateTime.Now.ToString();
		//    DateTime sTime = new DateTime(2011, 4, 1);

		//    TimeFrequencyBase timeFrequency = new RecurringTimeFrequency(20, IntervalUnit.Minute,
		//        new TimeSpan(1, 0, 0), new TimeSpan(21, 0, 0));

		//    //间隔5天，每天隔20分钟
		//    JobScheduleFrequencyBase schFqc = new DailyJobScheduleFrequency(5, timeFrequency);
		//    JobSchedule schedule = new JobSchedule(schID, schName, sTime, schFqc);

		//    Console.WriteLine(schedule.Description);

		//    for (int i = 0; i < 300; i++)
		//    {
		//        sTime = schedule.GetScheduleTime(sTime);
		//        Console.WriteLine(sTime.ToString());
		//    }
		//}
	}
}
