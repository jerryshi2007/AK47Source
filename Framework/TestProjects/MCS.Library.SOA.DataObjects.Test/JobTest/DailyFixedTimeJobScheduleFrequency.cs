using System;
using System.Text;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace MCS.Library.SOA.DataObjects.Test.JobTest
{
	/// <summary>
	/// Summary description for DailyFixedTimeJobScheduleFrequency
	/// </summary>
	[TestClass]
	public class DailyFixedTimeJobScheduleFrequency
	{
		private static readonly TimeSpan _TimeOffset = new TimeSpan(0, 0, 30);

		public DailyFixedTimeJobScheduleFrequency()
		{
		}

		private TestContext testContextInstance;

		/// <summary>
		///Gets or sets the test context which provides
		///information about and functionality for the current test run.
		///</summary>
		public TestContext TestContext
		{
			get
			{
				return testContextInstance;
			}
			set
			{
				testContextInstance = value;
			}
		}

		#region Additional test attributes
		//
		// You can use the following additional attributes as you write your tests:
		//
		// Use ClassInitialize to run code before running the first test in the class
		// [ClassInitialize()]
		// public static void MyClassInitialize(TestContext testContext) { }
		//
		// Use ClassCleanup to run code after all tests in a class have run
		// [ClassCleanup()]
		// public static void MyClassCleanup() { }
		//
		// Use TestInitialize to run code before running each test 
		// [TestInitialize()]
		// public void MyTestInitialize() { }
		//
		// Use TestCleanup to run code after each test has run
		// [TestCleanup()]
		// public void MyTestCleanup() { }
		//
		#endregion

		[TestMethod]
		[TestCategory("Schedule")]
		[Description("得到每天固定时间的Scope")]
		public void DailyFixdTimeJobScheduleFrequencyTest()
		{
			DailyJobScheduleFrequency dailyFrequency = PrepareDailyJobSchedule(1);

			DateTime startTime = DateTime.Now.Date;
			DateTime timePoint = startTime + new TimeSpan(10, 10, 10);

			TimeScope scope = dailyFrequency.GetTimeScope(startTime, timePoint, _TimeOffset);

			Assert.IsNotNull(scope);

			Console.WriteLine(scope.BeginTime);
			Console.WriteLine(scope.EndTime);

			Assert.AreEqual(new TimeSpan(10, 10, 10) - _TimeOffset, scope.BeginTime);
			Assert.AreEqual(new TimeSpan(10, 10, 10) + _TimeOffset, scope.EndTime);
		}

		[TestMethod]
		[TestCategory("Schedule")]
		[Description("间隔两天，获取每天处于间隔期内的Scope（应该为null）")]
		public void NotInIntervalDailyFixdTimeJobScheduleFrequencyTest()
		{
			DailyJobScheduleFrequency dailyFrequency = PrepareDailyJobSchedule(2);

			DateTime startTime = DateTime.Now.Date.AddDays(1);
			DateTime timePoint = startTime + new TimeSpan(10, 10, 10);

			TimeScope scope = dailyFrequency.GetTimeScope(startTime, timePoint, _TimeOffset);

			Assert.IsNotNull(scope);
		}

		[TestMethod]
		[TestCategory("Schedule")]
		[Description("间隔两天，获取每天处于执行期内的Scope")]
		public void InIntervalDailyFixdTimeJobScheduleFrequencyTest()
		{
			DailyJobScheduleFrequency dailyFrequency = PrepareDailyJobSchedule(2);

			DateTime startTime = DateTime.Now.Date.AddDays(2);
			DateTime timePoint = startTime + new TimeSpan(10, 10, 10);

			TimeScope scope = dailyFrequency.GetTimeScope(startTime, timePoint, _TimeOffset);

			Assert.IsNotNull(scope);

			Console.WriteLine(scope.BeginTime);
			Console.WriteLine(scope.EndTime);

			Assert.AreEqual(new TimeSpan(10, 10, 10) - _TimeOffset, scope.BeginTime);
			Assert.AreEqual(new TimeSpan(10, 10, 10) + _TimeOffset, scope.EndTime);
		}

		[TestMethod]
		[TestCategory("Schedule")]
		[Description("预测间隔两天固定时间调度接下来的执行时间")]
		public void EstimateDailyFixdTimeJobScheduleFrequencyTest()
		{
			DailyJobScheduleFrequency dailyFrequency = PrepareDailyJobSchedule(2);

			List<DateTime> estimateTime = dailyFrequency.EstimateExecuteTime(DateTime.Now, _TimeOffset, 20, TimeSpan.FromSeconds(10));

			estimateTime.ForEach(d => Console.WriteLine(d.ToString("yyyy-MM-dd HH:mm:ss")));
		}

		[TestMethod]
		[TestCategory("Schedule")]
		[Description("预测间隔两天，间隔10秒，调度接下来的执行时间")]
		public void EstimateDailyRecurringTimeJobScheduleFrequencyTest()
		{
			DailyJobScheduleFrequency dailyFrequency = PrepareDailyRecurringJobSchedule(2);

			List<DateTime> estimateTime = dailyFrequency.EstimateExecuteTime(DateTime.Now, _TimeOffset, 20, TimeSpan.FromSeconds(10));

			estimateTime.ForEach(d => Console.WriteLine(d.ToString("yyyy-MM-dd HH:mm:ss")));
		}

		private static DailyJobScheduleFrequency PrepareDailyJobSchedule(int intervalDays)
		{
			return new DailyJobScheduleFrequency(intervalDays, new FixedTimeFrequency(new TimeSpan(10, 10, 10)));
		}

		private static DailyJobScheduleFrequency PrepareDailyRecurringJobSchedule(int intervalDays)
		{
			TimeSpan sTime = new TimeSpan(10, 0, 0);
			TimeSpan eTime = new TimeSpan(22, 0, 0);

			return new DailyJobScheduleFrequency(intervalDays, new RecurringTimeFrequency(20, IntervalUnit.Second, sTime, eTime));
		}
	}
}
