using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using MCS.Library.Data;

namespace MCS.Library.SOA.DataObjects.Test.Schedules
{
	[TestClass]
	public class DailyOnceScheduleTests
	{
		private static readonly TimeSpan _TimeOffset = new TimeSpan(0, 0, 30);

		[TestMethod]
		[TestCategory("ScheduleSequence")]
		[Description("当天和第二天执行一次")]
		public void DailyOnceScheduleTest1()
		{
			InvokeWebServiceJob job = ScheduleUtil.CreateDefaultJob();
			job.Schedules.Add(new JobSchedule("", "", new DateTime(2013, 7, 11),
				new DailyJobScheduleFrequency(1, new FixedTimeFrequency(new TimeSpan(12, 0, 0)))));

			var checkPoints = new[]
			{ 
				new DateTime(2013, 7, 11, 9, 8, 29), new DateTime(2013, 7, 11, 11, 59, 29), new DateTime(2013, 7, 11, 12, 0, 29), new DateTime(2013, 7, 11, 12, 1, 29),
				new DateTime(2013, 7, 12, 9, 8, 29), new DateTime(2013, 7, 12, 11, 59, 29), new DateTime(2013, 7, 12, 12, 0, 29), new DateTime(2013, 7, 12, 12, 1, 29),
				new DateTime(2013, 7, 13, 9, 8, 29), new DateTime(2013, 7, 13, 11, 59, 29), new DateTime(2013, 7, 13, 12, 0, 29), new DateTime(2013, 7, 13, 12, 1, 29),
			};

			var asserts = new[]
			{
				false,			false,			true,			false,
				false,			false,			true,			false,			
				false,			false,			true ,			false,
			};

			ScheduleUtil.StartTest(job, checkPoints, asserts);
		}

		[TestMethod]
		[TestCategory("ScheduleSequence")]
		[Description("当天和第三天执行一次")]
		public void DailyOnceScheduleTest2()
		{
			InvokeWebServiceJob job = ScheduleUtil.CreateDefaultJob();
			job.Schedules.Add(new JobSchedule("", "", new DateTime(2013, 7, 11),
				new DailyJobScheduleFrequency(2, new FixedTimeFrequency(new TimeSpan(12, 0, 0)))));

			var checkPoints = new[]
			{ 
				new DateTime(2013, 7, 11, 9, 8, 29), new DateTime(2013, 7, 11, 11, 59, 29), new DateTime(2013, 7, 11, 12, 0, 29), new DateTime(2013, 7, 11, 12, 1, 29),
			    new DateTime(2013, 7, 12, 9, 8, 29), new DateTime(2013, 7, 12, 11, 59, 29), new DateTime(2013, 7, 12, 12, 0, 29), new DateTime(2013, 7, 12, 12, 1, 29),
			    new DateTime(2013, 7, 13, 9, 8, 29), new DateTime(2013, 7, 13, 11, 59, 29), new DateTime(2013, 7, 13, 12, 0, 29), new DateTime(2013, 7, 13, 12, 1, 29),
			};

			var asserts = new[]
			{
				false,			false,			true,			false,
				false,			false,			false,			false,			
				false,			false,			true,			false,
			};

			ScheduleUtil.StartTest(job, checkPoints, asserts);
		}

		[TestMethod]
		[TestCategory("ScheduleSequence")]
		[Description("当天和第四条执行一次")]
		public void DailyOnceScheduleTest3()
		{
			InvokeWebServiceJob job = ScheduleUtil.CreateDefaultJob();
			job.Schedules.Add(new JobSchedule("", "", new DateTime(2013, 7, 11),
				new DailyJobScheduleFrequency(3, new FixedTimeFrequency(new TimeSpan(12, 0, 0)))));

			var checkPoints = new[]
			{ 
				new DateTime(2013, 7, 11, 9, 8, 29), new DateTime(2013, 7, 11, 11, 59, 29), new DateTime(2013, 7, 11, 12, 0, 29), new DateTime(2013, 7, 11, 12, 1,  29),
				new DateTime(2013, 7, 12, 9, 8, 29), new DateTime(2013, 7, 12, 11, 59, 29), new DateTime(2013, 7, 12, 12, 0, 29), new DateTime(2013, 7, 12, 12, 1, 29),
				new DateTime(2013, 7, 13, 9, 8, 29), new DateTime(2013, 7, 13, 11, 59, 29), new DateTime(2013, 7, 13, 12, 0, 29), new DateTime(2013, 7, 13, 12, 1, 29),
			};

			var asserts = new[]
			{
				false,			false,			true,			false,
				false,			false,			false ,			false,			
				false,			false,			false ,			false,
			};

			ScheduleUtil.StartTest(job, checkPoints, asserts);
		}


		[TestMethod]
		[TestCategory("ScheduleSequence")]
		[Description("每天执行，但当天不执行")]
		public void DailyOnceScheduleTest4()
		{
			InvokeWebServiceJob job = ScheduleUtil.CreateDefaultJob();
			job.Schedules.Add(new JobSchedule("", "", new DateTime(2013, 7, 11),
				new DailyJobScheduleFrequency(1, new FixedTimeFrequency(new TimeSpan(7, 0, 0)))));

			var checkPoints = new[]
			{ 
				new DateTime (2013, 7, 11, 9, 8, 29), new DateTime(2013, 7, 11, 11, 59, 29), new DateTime(2013, 7, 11, 12, 0, 29), new DateTime(2013, 7, 11, 12, 1, 29),
				new DateTime(2013, 7, 12, 9, 8, 29), new DateTime(2013, 7, 12, 7, 0, 29),	new DateTime(2013, 7, 12, 7, 1, 29), new DateTime(2013, 7, 13, 7, 2, 29),
				new DateTime(2013, 7, 13, 6, 58, 29), new DateTime(2013, 7, 13, 7, 0, 29), new DateTime(2013, 7, 13, 7, 2, 29), new DateTime (2013, 7, 13, 12, 1, 29)
			};

			var asserts = new[]
			{
				false,			false,			false ,			false,
				false,			true ,			false ,			false ,			
				false,			true ,			false ,			false,
			};

			ScheduleUtil.StartTest(job, checkPoints, asserts);
		}

		[TestMethod]
		[TestCategory("ScheduleSequence")]
		[Description("每天执行，但当天不执行")]
		public void DailyOnceScheduleTest5()
		{
			InvokeWebServiceJob job = ScheduleUtil.CreateDefaultJob();
			job.Schedules.Add(new JobSchedule("", "", new DateTime(2013, 7, 12),
				new DailyJobScheduleFrequency(1, new FixedTimeFrequency(new TimeSpan(12, 0, 0)))));

			var checkPoints = new[]
			{ 
				new DateTime (2013, 7, 11, 11, 59, 29), new DateTime(2013, 7, 11, 12, 0, 29), new DateTime(2013, 7, 11, 12, 1, 29), new DateTime(2013, 7, 12, 12, 2, 29),
				new DateTime(2013, 7, 12, 11, 59, 29), new DateTime(2013, 7, 12, 12, 0, 29), new DateTime(2013, 7, 12, 12, 1, 29), new DateTime(2013, 7, 12, 12, 2, 29),
				new DateTime(2013, 7, 13, 9, 8, 29), new DateTime(2013, 7, 13, 11, 59, 29), new DateTime(2013, 7, 13, 12, 0, 29), new DateTime (2013, 7, 13, 12, 1, 29)
			};

			var asserts = new[]
			{
				false,			false ,			false ,			false,
				false,			true ,			false ,			false ,			
				false,			false ,			true ,			false,
			};

			ScheduleUtil.StartTest(job, checkPoints, asserts);
		}

		[TestMethod]
		[TestCategory("ScheduleSequence")]
		[Description("密集的调度")]
		public void IntensiveTest()
		{
			InvokeWebServiceJob job = ScheduleUtil.CreateDefaultJob();

			job.LastExecuteTime = new DateTime(2013, 7, 2);
			job.Schedules.Add(new JobSchedule("", "", new DateTime(2013, 7, 11),
				new DailyJobScheduleFrequency(1, new FixedTimeFrequency(new TimeSpan(6, 0, 0)))));

			var checkPoints = new[]
			{ 
				new DateTime (2013, 7, 11, 5, 58, 44), new DateTime(2013, 7, 11, 5, 59, 44), new DateTime(2013, 7, 11, 6, 00, 44),
				new DateTime (2013, 7, 12, 5, 58, 44), new DateTime(2013, 7, 12, 5, 59, 44), new DateTime(2013, 7, 12, 6, 00, 44),
				new DateTime (2013, 7, 13, 5, 58, 44), new DateTime(2013, 7, 13, 5, 59, 44), new DateTime(2013, 7, 13, 6, 00, 44),
				new DateTime (2013, 7, 14, 5, 58, 44), new DateTime(2013, 7, 14, 5, 59, 44), new DateTime(2013, 7, 14, 6, 00, 44),
			};

			var asserts = new[]
			{
				false,			true,			false,
				false,			true,			false,
				false,			true,			false,
				false,			true,			false,
			};

			ScheduleUtil.StartTest(job, checkPoints, asserts);
		}


		[TestMethod]
		[TestCategory("ScheduleSequence")]
		[Description("时间差检查")]
		public void TimeOffsetTest()
		{
			DateTime dt = (DateTime)DbHelper.RunSqlReturnScalar("select getdate()", "TestServer");
			DateTime now = DateTime.Now;
			Assert.IsTrue(Math.Round((dt - now).TotalSeconds) < 5);
		}
	}
}
