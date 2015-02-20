using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace MCS.Library.SOA.DataObjects.Test.Schedules
{
	[TestClass]
	public class MonthlyOnceScheduleTests
	{
		[TestMethod]
		[TestCategory("ScheduleSequence")]
		[Description("每月11日执行一次")]
		public void MonthlyOnceScheduleTest1()
		{
			InvokeWebServiceJob job = ScheduleUtil.CreateDefaultJob();
			job.Schedules.Add(new JobSchedule("", "", new DateTime(2013, 7, 11),
				new MonthlyJobScheduleFrequency(11, 1, new FixedTimeFrequency(new TimeSpan(12, 0, 0)))));

			var checkPoints = new[]
			{ 
				new DateTime(2013, 7, 11, 9, 8, 29), new DateTime(2013, 7, 11,11,59,29), new DateTime(2013, 7, 11,12,0,29), new DateTime(2013, 7, 12,12,0,29),
				new DateTime(2013, 8, 11, 12, 0, 29), new DateTime(2013,9,11,12,0,29), new DateTime(2013,9,12,12,0,29), new DateTime(2013,10,11,12,0,29),
			};

			var asserts = new[]
			{
				false,			false,			true,			false,
				true,			true,			false,			true,			
			};

			ScheduleUtil.StartTest(job, checkPoints, asserts);
		}

		[TestMethod]
		[TestCategory("ScheduleSequence")]
		[Description("每2月11日执行一次")]
		public void MonthlyOnceScheduleTest2()
		{
			InvokeWebServiceJob job = ScheduleUtil.CreateDefaultJob();
			job.Schedules.Add(new JobSchedule("", "", new DateTime(2013, 7, 11),
				new MonthlyJobScheduleFrequency(11, 2, new FixedTimeFrequency(new TimeSpan(12, 0, 0)))));

			var checkPoints = new[]
			{ 
				new DateTime(2013, 7, 11, 9, 8, 29), new DateTime(2013, 7, 11, 11, 59, 29), new DateTime(2013, 7, 11, 12, 0, 29), new DateTime(2013, 7, 12, 12, 0, 29),
				new DateTime(2013,8,11, 12, 0, 29), new DateTime(2013,9,11, 12, 0, 29), new DateTime(2013, 9, 12, 12, 0, 29), new DateTime(2013, 10, 11, 12, 0, 29),
			};

			var asserts = new[]
			{
				false,			false,			true,			false,
				false,			true,			false,			false,			
			};

			ScheduleUtil.StartTest(job, checkPoints, asserts);
		}

		[TestMethod]
		[TestCategory("ScheduleSequence")]
		[Description("每3月11日执行一次")]
		public void MonthlyOnceScheduleTest3()
		{
			InvokeWebServiceJob job = ScheduleUtil.CreateDefaultJob();
			job.Schedules.Add(new JobSchedule("", "", new DateTime(2013, 7, 11),
				new MonthlyJobScheduleFrequency(11, 3, new FixedTimeFrequency(new TimeSpan(12, 0, 0)))));

			var checkPoints = new[]
			{ 
				new DateTime(2013, 7, 11, 9, 8, 29), new DateTime(2013, 7, 11, 11, 59, 29), new DateTime(2013, 7, 11, 12, 0, 29), new DateTime(2013, 7, 12, 12, 0, 29),
				new DateTime(2013, 8, 11, 12, 0, 29), new DateTime(2013, 9, 11, 12, 0, 29), new DateTime(2013, 9, 12, 12, 0, 29), new DateTime(2013, 10, 11, 12, 0, 29),
			};

			var asserts = new[]
			{
				false,			false,			true,			false,
				false,			false,			false,			true,			
			};

			ScheduleUtil.StartTest(job, checkPoints, asserts);
		}

		[TestMethod]
		[TestCategory("ScheduleSequence")]
		[Description("每月11日7点执行一次，当天不执行")]
		public void MonthlyOnceScheduleTest4()
		{
			InvokeWebServiceJob job = ScheduleUtil.CreateDefaultJob();
			job.Schedules.Add(new JobSchedule("", "", new DateTime(2013, 7, 11),
				new MonthlyJobScheduleFrequency(11, 1, new FixedTimeFrequency(new TimeSpan(7, 0, 0)))));

			var checkPoints = new[]
			{ 
				new DateTime(2013, 7, 11, 9, 6, 29), new DateTime(2013, 7, 11, 11, 59, 29), new DateTime(2013, 7, 11, 12, 0, 29), new DateTime(2013, 7, 12, 7, 0, 29),
				new DateTime(2013, 8, 11, 7, 0, 29), new DateTime(2013, 9, 11, 12, 0, 29), new DateTime(2013, 9, 12, 12, 0, 29), new DateTime(2013, 10, 11, 7, 0, 29),
			};

			var asserts = new[]
			{
				false,			false,			false,			false,
				true,			false,			false,			true,			
			};

			ScheduleUtil.StartTest(job, checkPoints, asserts);
		}

		[TestMethod]
		[TestCategory("ScheduleSequence")]
		[Description("每月11日12点执行一次，当天不执行")]
		public void MonthlyOnceScheduleTest5()
		{
			InvokeWebServiceJob job = ScheduleUtil.CreateDefaultJob();
			job.Schedules.Add(new JobSchedule("", "", new DateTime(2013, 7, 12),
				new MonthlyJobScheduleFrequency(11, 1, new FixedTimeFrequency(new TimeSpan(12, 0, 0)))));

			var checkPoints = new[]
			{ 
				new DateTime(2013, 7, 11, 12, 0, 29), new DateTime(2013, 7, 12, 12, 0, 29),
				new DateTime(2013, 8, 11, 12, 0, 29), new DateTime(2013, 9, 11, 12, 0, 29), new DateTime(2013, 9, 12, 12, 0, 29), new DateTime(2013, 10, 11, 12, 0, 29),
			};

			var asserts = new[]
			{
				false,			false,
				true,			true,			false,			true,			
			};

			ScheduleUtil.StartTest(job, checkPoints, asserts);
		}
	}
}
