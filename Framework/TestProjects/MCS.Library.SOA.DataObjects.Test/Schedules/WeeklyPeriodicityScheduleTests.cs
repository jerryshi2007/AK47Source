using System;
using System.Text;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace MCS.Library.SOA.DataObjects.Test.Schedules
{
	/// <summary>
	/// WeeklyPeriodicityScheduleTests 的摘要说明
	/// </summary>
	[TestClass]
	public class WeeklyPeriodicityScheduleTests
	{
		[TestMethod]
		[TestCategory("ScheduleSequence")]
		[Description("每周一三五执行，当天不执行")]
		public void WeeklyPeriodicityScheduleTest1()
		{
			InvokeWebServiceJob job = ScheduleUtil.CreateDefaultJob();
			job.Schedules.Add(new JobSchedule("", "", new DateTime(2013, 7, 11),
				new WeeklyJobScheduleFrequency(ScheduleUtil.Weeks(1, 3, 5), 1, new RecurringTimeFrequency(5, IntervalUnit.Minute, new TimeSpan(12, 0, 0), new TimeSpan(13, 0, 0)))));

			var checkPoints = new[]
			{ 
				new DateTime(2013, 7, 11, 11, 59, 29), new DateTime(2013, 7, 11, 12,0, 29), new DateTime(2013, 7, 11, 12, 5, 29), new DateTime(2013, 7, 11, 12, 9, 29),
				new DateTime(2013, 7, 12, 11, 59, 29), new DateTime(2013, 7, 12, 12, 0, 29), new DateTime(2013, 7, 12, 12, 1, 29),
				new DateTime(2013, 7, 13, 12, 0, 29), new DateTime(2013, 7, 14, 12, 0, 29), new DateTime(2013, 7, 15, 12, 0, 29), new DateTime(2013, 7, 16, 12, 0, 29),
				new DateTime(2013, 7, 16, 12, 5, 29), new DateTime(2013, 7, 17, 12, 0, 29), new DateTime(2013, 7, 18, 12, 0, 29)
			};

			var asserts = new[]
			{
				false,			false,			false,			false,			
				false,			true,			false,			false,
				false,			true,			false,			false,			
				true,			false,	
			};

			ScheduleUtil.StartTest(job, checkPoints, asserts);
		}

		[TestMethod]
		[TestCategory("ScheduleSequence")]
		[Description("每2周二四六执行，当天执行")]
		public void WeeklyPeriodicityScheduleTest2()
		{
			InvokeWebServiceJob job = ScheduleUtil.CreateDefaultJob();
			job.Schedules.Add(new JobSchedule("", "", new DateTime(2013, 7, 11),
				new WeeklyJobScheduleFrequency(ScheduleUtil.Weeks(2, 4, 6), 1, new RecurringTimeFrequency(5, IntervalUnit.Minute, new TimeSpan(12, 0, 0), new TimeSpan(13, 0, 0)))));

			var checkPoints = new[]
			{ 
				new DateTime(2013, 7, 11, 11, 59, 29), new DateTime(2013, 7, 11, 12, 0, 29), new DateTime(2013, 7, 11, 12, 5, 29), new DateTime(2013, 7, 11, 12, 9, 29),
				new DateTime(2013, 7, 12, 11, 59, 29), new DateTime(2013, 7, 12, 12, 0, 29), new DateTime(2013, 7, 12, 12, 1, 29),
				new DateTime(2013, 7, 13, 12, 0, 29), new DateTime(2013, 7, 14, 12, 0, 29), new DateTime(2013, 7, 15, 12, 0, 29), new DateTime(2013, 7, 16, 12, 0, 29),
				new DateTime(2013, 7, 16, 12, 5, 29), new DateTime(2013, 7, 17, 12, 0, 29), new DateTime(2013, 7, 18, 12, 0, 29)
			};

			var asserts = new[]
			{
				false,			true,			true,			false,			
				false,			false,			false,			
				true,			false,			false,			true,			
				true,			false,			true
			};

			ScheduleUtil.StartTest(job, checkPoints, asserts);
		}

		[TestMethod]
		[TestCategory("ScheduleSequence")]
		[Description("每3周一三五执行，当天不执行")]
		public void WeeklyPeriodicityScheduleTest3()
		{
			InvokeWebServiceJob job = ScheduleUtil.CreateDefaultJob();
			job.Schedules.Add(new JobSchedule("", "", new DateTime(2013, 7, 11),
				new WeeklyJobScheduleFrequency(ScheduleUtil.Weeks(1, 3, 5), 2, new RecurringTimeFrequency(5, IntervalUnit.Minute, new TimeSpan(12, 0, 0), new TimeSpan(13, 0, 0)))));

			var checkPoints = new[]
			{ 
				new DateTime(2013, 7, 11, 11, 59, 29), new DateTime(2013, 7, 11, 12, 0, 29), new DateTime(2013, 7, 11, 12, 5, 29), new DateTime(2013, 7, 11, 12, 9, 29),
				new DateTime(2013, 7, 12, 11, 59, 29), new DateTime(2013, 7, 12, 12, 0, 29), new DateTime(2013, 7, 12, 12, 1, 29),
				new DateTime(2013, 7, 13, 12, 0, 29), new DateTime(2013, 7, 14, 12, 0, 29), new DateTime(2013, 7, 15, 12, 0, 29), new DateTime(2013, 7, 16, 12, 0, 29),
				new DateTime(2013, 7, 16, 12, 5, 29), new DateTime(2013, 7, 17, 12, 0, 29), new DateTime(2013, 7, 18, 12, 0, 29)
			};

			var asserts = new[]
			{
				false,			false,			false,			false,			
				false,			true,			false,			
				false,			false,			false,			false,			
				false,			false,			false
			};

			ScheduleUtil.StartTest(job, checkPoints, asserts);
		}

		[TestMethod]
		[TestCategory("ScheduleSequence")]
		[Description("每周二四六执行，当天不执行")]
		public void WeeklyPeriodicityScheduleTest4()
		{
			InvokeWebServiceJob job = ScheduleUtil.CreateDefaultJob();
			job.Schedules.Add(new JobSchedule("", "", new DateTime(2013, 7, 11),
				new WeeklyJobScheduleFrequency(ScheduleUtil.Weeks(2, 4, 6), 1, new RecurringTimeFrequency(5, IntervalUnit.Minute, new TimeSpan(7, 0, 0), new TimeSpan(8, 0, 0)))));

			var checkPoints = new[]
			{ 
				new DateTime(2013, 7, 11, 11, 59, 29), new DateTime(2013, 7, 11, 12, 0, 29), new DateTime(2013, 7, 11, 12, 5, 29), new DateTime(2013, 7, 11, 12, 9, 29),
				new DateTime(2013, 7, 12, 6, 59, 29), new DateTime(2013, 7, 12, 7, 0, 29), new DateTime(2013, 7, 12, 8, 0, 29),
				new DateTime(2013, 7, 13, 6, 59, 31), new DateTime(2013, 7, 14, 6, 5, 29), new DateTime(2013, 7, 15, 6, 5, 29), new DateTime(2013, 7, 16, 6, 10, 29),
				new DateTime(2013, 7, 16, 6, 15, 29), new DateTime(2013, 7, 17, 12, 0, 29), new DateTime(2013, 7, 18, 6, 12, 29)
			};

			var asserts = new[]
			{
				false,			false,			false,			false,			
				false,			false,			false,			
				false,			false,			false,			false,			
				false,			false,			false
			};

			ScheduleUtil.StartTest(job, checkPoints, asserts);
		}

		[TestMethod]
		[TestCategory("ScheduleSequence")]
		[Description("每周二四六执行，当天不执行")]
		public void WeeklyPeriodicityScheduleTest5()
		{
			InvokeWebServiceJob job = ScheduleUtil.CreateDefaultJob();
			job.Schedules.Add(new JobSchedule("", "", new DateTime(2013, 7, 12),
				new WeeklyJobScheduleFrequency(ScheduleUtil.Weeks(2, 4, 6), 1, new RecurringTimeFrequency(5, IntervalUnit.Minute, new TimeSpan(12, 0, 0), new TimeSpan(13, 0, 0)))));

			var checkPoints = new[]
			{ 
				new DateTime(2013, 7, 11, 11, 59, 29), new DateTime(2013, 7, 11, 12, 0, 29), new DateTime(2013, 7, 11, 12, 5, 29), new DateTime(2013, 7, 11, 12, 9, 29),
				new DateTime(2013, 7, 12, 11, 59, 29), new DateTime(2013, 7, 12, 12, 0, 29), new DateTime(2013, 7, 12, 12, 1, 29),
				new DateTime(2013, 7, 13, 12, 0, 29), new DateTime(2013, 7, 14, 12, 0, 29), new DateTime(2013, 7, 15, 12, 0, 29), new DateTime(2013, 7, 16, 12, 0, 29),
				new DateTime(2013, 7, 16, 12, 5, 29), new DateTime(2013, 7, 17, 12, 0, 29), new DateTime(2013, 7, 18, 12, 0, 29)
			};

			var asserts = new[]
			{
				false,			false,			false,			false,			
				false,			false,			false,			
				true,			false,			false,			true,			
				true,			false,			true
			};

			ScheduleUtil.StartTest(job, checkPoints, asserts);
		}
	}
}
