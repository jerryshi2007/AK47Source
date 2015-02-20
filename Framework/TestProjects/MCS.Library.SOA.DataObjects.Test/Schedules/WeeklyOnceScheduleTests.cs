using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace MCS.Library.SOA.DataObjects.Test.Schedules
{
	[TestClass]
	public class WeeklyOnceScheduleTests
	{
		[TestMethod]
		[TestCategory("ScheduleSequence")]
		[Description("每周一三五执行一次，当天不执行")]
		public void WeeklyOnceScheduleTest1()
		{
			InvokeWebServiceJob job = ScheduleUtil.CreateDefaultJob();
			job.Schedules.Add(new JobSchedule("", "", new DateTime(2013, 7, 11),
				new WeeklyJobScheduleFrequency(ScheduleUtil.Weeks(1, 3, 5), 1, new FixedTimeFrequency(new TimeSpan(12, 0, 0)))));

			var checkPoints = new[]
			{ 
				new DateTime(2013, 7, 11, 11, 59, 29), new DateTime(2013, 7, 11, 12, 0, 29), new DateTime(2013, 7, 11, 12, 1, 29), new DateTime(2013, 7, 12, 11, 59, 29),
				new DateTime(2013, 7, 12, 12, 0, 29), new DateTime(2013, 7, 12, 12, 1, 29),new DateTime(2013, 7, 13, 12, 0, 29), new DateTime(2013, 7, 14, 12, 0, 29),
				new DateTime(2013, 7, 15, 12, 0, 29), new DateTime(2013, 7, 16, 12, 0, 29), new DateTime(2013, 7, 17, 12, 0, 29), new DateTime(2013, 7, 18, 12, 0, 29)
			};

			var asserts = new[]
			{
				false,			false,			false,			false,
				true,			false,			false,			false,			
				true,			false,			true ,			false,
			};

			ScheduleUtil.StartTest(job, checkPoints, asserts);
		}

		[TestMethod]
		[TestCategory("ScheduleSequence")]
		[Description("每周二四六执行一次，当天执行")]
		public void WeeklyOnceScheduleTest2()
		{
			InvokeWebServiceJob job = ScheduleUtil.CreateDefaultJob();
			job.Schedules.Add(new JobSchedule("", "", new DateTime(2013, 7, 11),
				new WeeklyJobScheduleFrequency(ScheduleUtil.Weeks(2, 4, 6), 1, new FixedTimeFrequency(new TimeSpan(12, 0, 0)))));

			var checkPoints = new[]
			{ 
				new DateTime(2013, 7, 11, 11, 59, 29), new DateTime(2013, 7, 11, 12, 0, 29), new DateTime(2013, 7, 11, 12, 1, 29), new DateTime(2013, 7, 12, 11, 59, 29),
				new DateTime(2013, 7, 12, 12, 0, 29), new DateTime(2013, 7, 12, 12, 1, 29), new DateTime(2013, 7, 13, 12, 0, 29), new DateTime(2013, 7, 14, 12, 0, 29),
				new DateTime(2013, 7, 15, 12, 0, 29), new DateTime(2013, 7, 16, 12, 0, 29), new DateTime(2013, 7, 17, 12, 0, 29), new DateTime(2013, 7, 18, 12, 0, 29)
			};

			var asserts = new[]
			{
				false,			true,			false,			false,
				false,			false,			true,			false,			
				false,			true,			false ,			true,
			};

			ScheduleUtil.StartTest(job, checkPoints, asserts);
		}

		[TestMethod]
		[TestCategory("ScheduleSequence")]
		[Description("每2周一三五执行一次")]
		public void WeeklyOnceScheduleTest3()
		{
			InvokeWebServiceJob job = ScheduleUtil.CreateDefaultJob();
			job.Schedules.Add(new JobSchedule("", "", new DateTime(2013, 7, 11),
				new WeeklyJobScheduleFrequency(ScheduleUtil.Weeks(1, 3, 5), 2, new FixedTimeFrequency(new TimeSpan(12, 0, 0)))));

			var checkPoints = new[]
			{ 
				new DateTime(2013, 7, 11, 11, 59, 29), new DateTime(2013, 7, 11, 12, 0, 29), new DateTime(2013, 7, 11, 12, 1, 29), new DateTime(2013, 7, 12, 11, 59, 29),
				new DateTime(2013, 7, 12, 12, 0, 29), new DateTime(2013, 7, 12, 12, 1, 29), new DateTime(2013, 7, 13, 12, 0, 29), new DateTime(2013, 7, 14, 12, 0, 29),
				new DateTime(2013, 7, 15, 12, 0, 29), new DateTime(2013, 7, 16, 12, 0, 29), new DateTime(2013, 7, 17, 12, 0, 29), new DateTime(2013, 7, 18, 12, 0, 29)
			};

			var asserts = new[]
			{
				false,			false,			false,			false,
				true,			false,			false,			false,			
				false,			false,			false ,			false,
			};

			ScheduleUtil.StartTest(job, checkPoints, asserts);
		}

		[TestMethod]
		[TestCategory("ScheduleSequence")]
		[Description("每周二四六执行一次，当天不执行")]
		public void WeeklyOnceScheduleTest4()
		{
			InvokeWebServiceJob job = ScheduleUtil.CreateDefaultJob();
			job.Schedules.Add(new JobSchedule("", "", new DateTime(2013, 7, 11),
				new WeeklyJobScheduleFrequency(ScheduleUtil.Weeks(2, 4, 6), 1, new FixedTimeFrequency(new TimeSpan(7, 0, 0)))));

			var checkPoints = new[]
			{ 
				new DateTime(2013, 7, 11, 11, 59, 29), new DateTime(2013, 7, 11, 12, 0, 29), new DateTime(2013, 7, 11, 12, 0, 29), new DateTime(2013,7, 12, 6, 59, 29),
				new DateTime(2013, 7, 12, 12, 0, 29), new DateTime(2013, 7, 13, 6, 59, 31),new DateTime(2013, 7, 13, 12, 0, 29), new DateTime(2013, 7, 14, 7, 0, 29),
				new DateTime(2013, 7, 15, 7, 0, 29), new DateTime(2013, 7, 16, 6, 59, 29), new DateTime(2013, 7, 17, 12, 0, 29), new DateTime(2013, 7,18, 12, 0, 29)
			};

			var asserts = new[]
			{
				false,			false,			false,			false,
				false,			true,			false,			false,			
				false,			false,			false ,			false,
			};

			ScheduleUtil.StartTest(job, checkPoints, asserts);
		}

		[TestMethod]
		[TestCategory("ScheduleSequence")]
		[Description("每周二四六执行一次，当天不执行")]
		public void WeeklyOnceScheduleTest5()
		{
			InvokeWebServiceJob job = ScheduleUtil.CreateDefaultJob();
			job.Schedules.Add(new JobSchedule("", "", new DateTime(2013, 7, 12),
				new WeeklyJobScheduleFrequency(ScheduleUtil.Weeks(2, 4, 6), 1, new FixedTimeFrequency(new TimeSpan(12, 0, 0)))));

			var checkPoints = new[]
			{ 
				new DateTime(2013, 7, 11, 11, 59, 29), new DateTime(2013, 7, 11, 12, 0, 29), new DateTime(2013, 7, 11, 12, 1, 29), new DateTime(2013, 7, 12, 11, 59, 29),
				new DateTime(2013, 7, 12, 12, 0, 29), new DateTime(2013, 7, 12, 12, 1, 29),new DateTime(2013, 7, 13, 12, 0, 29), new DateTime(2013, 7, 14, 12, 0, 29),
				new DateTime(2013, 7, 15, 12, 0, 29), new DateTime(2013, 7, 16, 12, 0, 29), new DateTime(2013, 7, 17, 12, 0, 29), new DateTime(2013, 7, 18, 12, 0, 29)
			};

			var asserts = new[]
			{
				false,			false,			false,			false,
				false,			false,			true,			false,			
				false,			true,			false ,			true,
			};

			ScheduleUtil.StartTest(job, checkPoints, asserts);
		}
	}
}
