using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace MCS.Library.SOA.DataObjects.Test.Schedules
{
	[TestClass]
	public class DailyPeriodicityScheduleTests
	{
		[TestMethod]
		[TestCategory("ScheduleSequence")]
		[Description("当天和第二天执行五分钟一次")]
		public void DailyPeriodicityScheduleTest1()
		{
			InvokeWebServiceJob job = ScheduleUtil.CreateDefaultJob();
			job.Schedules.Add(new JobSchedule("", "", new DateTime(2013, 7, 11),
				new DailyJobScheduleFrequency(1, new RecurringTimeFrequency(5, IntervalUnit.Minute, new TimeSpan(12, 0, 0), new TimeSpan(13, 0, 0)))));

			var checkPoints = new[]
			{ 
				new DateTime(2013, 7, 11, 9, 8, 29), new DateTime(2013, 7, 11, 11, 59, 29), new DateTime(2013, 7, 11, 12, 0, 29), new DateTime(2013, 7, 11, 12, 4, 29),
				new DateTime(2013, 7, 11, 12, 5, 29), new DateTime(2013, 7, 11, 12, 6, 29), new DateTime(2013, 7, 11, 12, 10, 29), new DateTime(2013, 7, 11, 13, 0, 29),
				new DateTime(2013, 7, 12, 9, 8, 29), new DateTime(2013, 7, 12, 11, 55, 29), new DateTime(2013, 7, 12, 12, 0, 29), new DateTime(2013, 7, 12, 12, 1, 29),
				new DateTime(2013, 7, 12, 12, 4, 29), new DateTime(2013, 7, 12, 12, 5, 29), new DateTime(2013, 7, 12, 12, 6, 29), new DateTime(2013, 7, 13, 9, 8, 29),
				new DateTime(2013, 7, 13, 11, 59, 29), new DateTime(2013, 7, 13, 12, 0, 29), new DateTime(2013, 7, 13, 12, 1, 29)
			};

			var asserts = new[]
			{
				false,			false,			true,			false,
				true,			false,			true,			false,			
				false,			false,			true ,			false,
				false,			true,			false ,			false,
				false,			true,			false ,
			};

			ScheduleUtil.StartTest(job, checkPoints, asserts);
		}

		[TestMethod]
		[TestCategory("ScheduleSequence")]
		[Description("当天和第三天执行五分钟一次")]
		public void DailyPeriodicityScheduleTest2()
		{
			InvokeWebServiceJob job = ScheduleUtil.CreateDefaultJob();
			job.Schedules.Add(new JobSchedule("", "", new DateTime(2013, 7, 11),
				new DailyJobScheduleFrequency(2, new RecurringTimeFrequency(5, IntervalUnit.Minute, new TimeSpan(12, 0, 0), new TimeSpan(13, 0, 0)))));

			var checkPoints = new[]
			{ 
				new DateTime(2013, 7, 11, 9, 8, 29), new DateTime(2013, 7, 11, 11, 59, 29), new DateTime(2013, 7, 11, 12, 0, 29), new DateTime(2013, 7, 11, 12, 4, 29),
				new DateTime(2013, 7, 11, 12, 5, 29), new DateTime(2013, 7, 11, 12, 6, 29), new DateTime(2013, 7, 11, 12, 10, 29),new DateTime(2013, 7, 11, 13, 0, 29),
				new DateTime(2013, 7, 12, 9, 8, 29), new DateTime(2013, 7, 12, 11, 55, 29), new DateTime(2013, 7, 12, 12, 0, 29), new DateTime(2013, 7, 12, 12, 1, 29),
				new DateTime(2013, 7, 12, 12, 4, 29), new DateTime(2013, 7, 12, 12, 5, 29), new DateTime(2013, 7, 12, 12, 6, 29), new DateTime(2013, 7, 13, 9, 8, 29),
				new DateTime(2013, 7, 13, 11, 59, 29), new DateTime(2013, 7, 13, 12, 0, 29), new DateTime(2013, 7, 13, 12, 1, 29)
			};

			var asserts = new[]
			{
				false,			false,			true,			false,
				true,			false,			true,			false,			
				false,			false,			false ,			false,
				false,			false,			false ,			false,
				false,			true,			false ,
			};

			ScheduleUtil.StartTest(job, checkPoints, asserts);
		}

		[TestMethod]
		[TestCategory("ScheduleSequence")]
		[Description("当天和第四天执行五分钟一次")]
		public void DailyPeriodicityScheduleTest3()
		{
			InvokeWebServiceJob job = ScheduleUtil.CreateDefaultJob();
			job.Schedules.Add(new JobSchedule("", "", new DateTime(2013, 7, 11),
				new DailyJobScheduleFrequency(3, new RecurringTimeFrequency(5, IntervalUnit.Minute, new TimeSpan(12, 0, 0), new TimeSpan(13, 0, 0)))));

			var checkPoints = new[]
			{ 
				new DateTime(2013, 7, 11, 9, 8, 29), new DateTime(2013, 7, 11, 11, 59, 29), new DateTime(2013, 7, 11, 12, 0, 29), new DateTime(2013, 7, 11, 12, 4, 29),
				new DateTime(2013, 7, 11, 12, 5, 29), new DateTime(2013, 7, 11, 12, 6, 29), new DateTime(2013, 7, 11, 12, 10, 29), new DateTime(2013, 7, 11, 13, 0, 29),
				new DateTime(2013, 7, 12, 9, 8, 29), new DateTime(2013, 7, 12, 11, 55, 29), new DateTime(2013, 7, 12, 12, 0, 29), new DateTime(2013, 7, 12, 12, 1, 29),
				new DateTime(2013, 7, 12, 12, 4, 29), new DateTime(2013, 7, 12, 12, 5, 29), new DateTime(2013, 7, 12, 12, 6, 29), new DateTime(2013, 7, 13, 9, 8, 29),
				new DateTime(2013, 7, 13, 11, 59, 29), new DateTime(2013, 7, 13, 12, 0, 29), new DateTime(2013, 7, 13, 12, 1, 29)
			};

			var asserts = new[]
			{
				false,			false,			true,			false,
				true,			false,			true,			false,			
				false,			false,			false ,			false,
				false,			false,			false ,			false,
				false,			false,			false ,
			};

			ScheduleUtil.StartTest(job, checkPoints, asserts);
		}

		[TestMethod]
		[TestCategory("ScheduleSequence")]
		[Description("各个调度点不同")]
		public void DailyPeriodicityScheduleTest4()
		{
			InvokeWebServiceJob job = ScheduleUtil.CreateDefaultJob();
			job.Schedules.Add(new JobSchedule("", "", new DateTime(2013, 7, 12),
				new DailyJobScheduleFrequency(1, new RecurringTimeFrequency(5, IntervalUnit.Minute, new TimeSpan(7, 0, 0), new TimeSpan(13, 0, 0)))));

			var checkPoints = new[]
			{ 
				new DateTime(2013, 7, 11, 9, 6, 29), new DateTime(2013, 7, 11, 11, 59, 29), new DateTime(2013, 7, 11, 12, 0, 29), new DateTime(2013, 7, 11, 12, 4, 29),
				new DateTime(2013, 7, 11, 12, 5, 29), new DateTime(2013, 7, 11, 12, 6, 29),new DateTime(2013, 7, 11, 12, 10, 29),new DateTime(2013, 7, 11, 13, 0, 29),
				new DateTime(2013, 7, 12, 6, 50, 29), new DateTime(2013, 7, 12, 7, 0, 29), new DateTime(2013, 7, 12, 7, 1, 29), new DateTime(2013, 7, 12, 7, 5, 29),
				new DateTime(2013, 7, 12, 12, 4, 29), new DateTime(2013, 7, 12, 12, 5, 29), new DateTime(2013, 7, 12, 12, 6, 29), new DateTime(2013, 7, 13, 6, 55, 29),
				new DateTime(2013, 7, 13, 7, 1, 29), new DateTime(2013, 7, 13, 7, 2, 29), new DateTime(2013, 7, 13, 12, 1, 29)
			};

			var asserts = new[]
			{
				false,			false,			false,			false,
				false,			false,			false,			false,			
				false,			true,			false,			true,
				true,			false,			false,			false,
				true,			false,			true,
			};

			ScheduleUtil.StartTest(job, checkPoints, asserts);
		}

		[TestMethod]
		[TestCategory("ScheduleSequence")]
		[Description("各个调度点不同，每五分钟执行")]
		public void DailyPeriodicityScheduleTest5()
		{
			InvokeWebServiceJob job = ScheduleUtil.CreateDefaultJob();
			job.Schedules.Add(new JobSchedule("", "", new DateTime(2013, 7, 12),
				new DailyJobScheduleFrequency(1, new RecurringTimeFrequency(5, IntervalUnit.Minute, new TimeSpan(12, 0, 0), new TimeSpan(13, 0, 0)))));

			var checkPoints = new[]
			{ 
				new DateTime(2013, 7, 11, 11, 0, 29), new DateTime(2013, 7, 11, 11, 59, 29), new DateTime(2013, 7, 11, 12, 0, 29), new DateTime(2013, 7, 11, 12, 4, 29),
				new DateTime(2013, 7, 11, 12, 5, 29), new DateTime(2013, 7, 11, 12, 6, 29), new DateTime(2013, 7, 11, 12, 10, 29), new DateTime(2013, 7, 11, 13, 0, 29),
				new DateTime(2013, 7, 12, 9, 8, 29), new DateTime(2013, 7, 12, 11, 55, 29), new DateTime(2013, 7, 12, 12, 0, 29), new DateTime(2013, 7, 12, 12, 1, 29),
				new DateTime(2013, 7, 12, 12, 4, 29), new DateTime(2013, 7, 12, 12, 5, 29), new DateTime(2013, 7, 12, 12, 6, 29), new DateTime(2013, 7, 13, 9, 8, 29),
				new DateTime(2013, 7, 13, 11, 59, 29), new DateTime(2013, 7, 13, 12, 0, 29), new DateTime(2013, 7, 13, 12, 1, 29)
			};

			var asserts = new[]
			{
				false,			false,			false,			false,
				false,			false,			false,			false,			
				false,			false,			true,			false,
				false,			true,			false,			false,
				false,			true,			false,
			};

			ScheduleUtil.StartTest(job, checkPoints, asserts);
		}
	}
}
