using System;
using System.Text;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace MCS.Library.SOA.DataObjects.Test.JobTest
{
	/// <summary>
	/// Summary description for WeeklyFixedTimeJobScheduleFrequency
	/// </summary>
	[TestClass]
	public class WeeklyFixedTimeJobScheduleFrequency
	{
		private static readonly TimeSpan _TimeOffset = new TimeSpan(0, 0, 30);

		public WeeklyFixedTimeJobScheduleFrequency()
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
		[Description("得到每周固定时间的Scope")]
		public void WeeklyFixdTimeJobScheduleFrequencyTest()
		{
			WeeklyJobScheduleFrequency weeklyFrequency = PrepareWeeklyJobSchedule(1, DayOfWeek.Monday);

			DateTime now = DateTime.Now;
			DateTime startTime = now.AddDays(-((int)now.DayOfWeek - (int)DayOfWeek.Sunday)).Date;

			DateTime timePoint = startTime.AddDays(1) + new TimeSpan(10, 10, 10); ;

			TimeScope scope = weeklyFrequency.GetTimeScope(startTime, timePoint, _TimeOffset);

			Assert.IsNotNull(scope);

			Console.WriteLine(scope.BeginTime);
			Console.WriteLine(scope.EndTime);

			Assert.AreEqual(new TimeSpan(10, 10, 10) - _TimeOffset, scope.BeginTime);
			Assert.AreEqual(new TimeSpan(10, 10, 10) + _TimeOffset, scope.EndTime);
		}

		[TestMethod]
		[TestCategory("Schedule")]
		[Description("不匹配周几，每周固定时间的Scope")]
		public void NotMatchWeekDayWeeklyFixdTimeJobScheduleFrequencyTest()
		{
			WeeklyJobScheduleFrequency weeklyFrequency = PrepareWeeklyJobSchedule(1, DayOfWeek.Monday);

			DateTime now = DateTime.Now;
			DateTime startTime = now.AddDays(-((int)now.DayOfWeek - (int)DayOfWeek.Sunday)).Date;

			DateTime timePoint = startTime.AddDays(2) + new TimeSpan(10, 10, 10); ;

			TimeScope scope = weeklyFrequency.GetTimeScope(startTime, timePoint, _TimeOffset);

			Assert.IsNull(scope);
		}

		[TestMethod]
		[TestCategory("Schedule")]
		[Description("不匹配周间隔，每周固定时间的Scope")]
		public void NotMatchWeekIntervalWeeklyFixdTimeJobScheduleFrequencyTest()
		{
			WeeklyJobScheduleFrequency weeklyFrequency = PrepareWeeklyJobSchedule(2, DayOfWeek.Monday);

			DateTime now = DateTime.Now;
			DateTime startTime = now.AddDays(-((int)now.DayOfWeek - (int)DayOfWeek.Sunday)).Date;

			DateTime timePoint = startTime.AddDays(8) + new TimeSpan(10, 10, 10); ;

			TimeScope scope = weeklyFrequency.GetTimeScope(startTime, timePoint, _TimeOffset);

			Assert.IsNull(scope);
		}

		[TestMethod]
		[TestCategory("Schedule")]
		[Description("匹配周间隔，每周固定时间的Scope")]
		public void MatchWeekIntervalWeeklyFixdTimeJobScheduleFrequencyTest()
		{
			WeeklyJobScheduleFrequency weeklyFrequency = PrepareWeeklyJobSchedule(2, DayOfWeek.Monday);

			DateTime now = DateTime.Now;
			DateTime startTime = now.AddDays(-((int)now.DayOfWeek - (int)DayOfWeek.Sunday)).Date;

			DateTime timePoint = startTime.AddDays(15) + new TimeSpan(10, 10, 10); ;

			TimeScope scope = weeklyFrequency.GetTimeScope(startTime, timePoint, _TimeOffset);

			Assert.IsNotNull(scope);

			Console.WriteLine(scope.BeginTime);
			Console.WriteLine(scope.EndTime);

			Assert.AreEqual(new TimeSpan(10, 10, 10) - _TimeOffset, scope.BeginTime);
			Assert.AreEqual(new TimeSpan(10, 10, 10) + _TimeOffset, scope.EndTime);
		}

		[TestMethod]
		[TestCategory("Schedule")]
		[Description("预测间隔两月，间隔5天，调度接下来的执行时间")]
		public void EstimateWeekIntervalWeeklyFixdTimeJobScheduleFrequencyTest()
		{
			WeeklyJobScheduleFrequency weeklyFrequency = PrepareWeeklyJobSchedule(2, DayOfWeek.Monday);

			List<DateTime> estimateTime = weeklyFrequency.EstimateExecuteTime(DateTime.Now, _TimeOffset, 20, TimeSpan.FromSeconds(10));

			estimateTime.ForEach(d => Console.WriteLine(d.ToString("yyyy-MM-dd HH:mm:ss")));
		}


		[TestMethod]
		[TestCategory("Schedule")]
		[Description("带上次执行时间的，每周固定时间的Scope")]
		public void WithLastExeTimeMatchWeekIntervalWeeklyFixdTimeJobScheduleFrequencyTest()
		{
			List<DayOfWeek> daysList = new List<DayOfWeek>();

			daysList.Add(DayOfWeek.Monday);
			daysList.Add(DayOfWeek.Tuesday);
			daysList.Add(DayOfWeek.Wednesday);
			daysList.Add(DayOfWeek.Thursday);
			daysList.Add(DayOfWeek.Friday);

			FixedTimeFrequency frequency = new FixedTimeFrequency(new TimeSpan(6, 0, 0));

			WeeklyJobScheduleFrequency weeklyFrequency = new WeeklyJobScheduleFrequency(daysList, 1, frequency);

			DateTime startTime = new DateTime(2013, 5, 31);
			DateTime lastExeTime = new DateTime(2013, 7, 30, 5, 59, 27);

			DateTime checkPoint = new DateTime(2013, 7, 30, 6, 0, 27);
			bool result = weeklyFrequency.IsNextExecuteTime(startTime, lastExeTime, checkPoint, TimeSpan.FromSeconds(60 / 2 * 1.1));

			Assert.IsFalse(result);
		}

		private static WeeklyJobScheduleFrequency PrepareWeeklyJobSchedule(int durationWeek, params DayOfWeek[] days)
		{
			List<DayOfWeek> daysList = new List<DayOfWeek>(days);

			return new WeeklyJobScheduleFrequency(daysList, durationWeek, new FixedTimeFrequency(new TimeSpan(10, 10, 10)));
		}
	}
}
