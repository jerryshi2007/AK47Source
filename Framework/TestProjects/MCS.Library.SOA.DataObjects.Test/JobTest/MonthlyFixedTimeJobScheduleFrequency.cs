using System;
using System.Text;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace MCS.Library.SOA.DataObjects.Test.JobTest
{
	/// <summary>
	/// Summary description for MonthlyFixedTimeJobScheduleFrequency
	/// </summary>
	[TestClass]
	public class MonthlyFixedTimeJobScheduleFrequency
	{
		private static readonly TimeSpan _TimeOffset = new TimeSpan(0, 0, 30);

		public MonthlyFixedTimeJobScheduleFrequency()
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
		[Description("得到每月固定时间的Scope")]
		public void MonthlyFixdTimeJobScheduleFrequencyTest()
		{
			MonthlyJobScheduleFrequency monthlyFrequency = PrepareMonthlyJobSchedule(1, 5);

			DateTime now = DateTime.Now;
			DateTime startTime = new DateTime(now.Year, now.Month, 1);

			DateTime timePoint = startTime.AddDays(4) + new TimeSpan(10, 10, 10);

			TimeScope scope = monthlyFrequency.GetTimeScope(startTime, timePoint, _TimeOffset);

			Assert.IsNotNull(scope);

			Console.WriteLine(scope.BeginTime);
			Console.WriteLine(scope.EndTime);

			Assert.AreEqual(new TimeSpan(10, 10, 10) - _TimeOffset, scope.BeginTime);
			Assert.AreEqual(new TimeSpan(10, 10, 10) + _TimeOffset, scope.EndTime);
		}

		[TestMethod]
		[TestCategory("Schedule")]
		[Description("不匹配每月指定日的固定时间的Scope")]
		public void NotMatchDayInMonthlyFixdTimeJobScheduleFrequencyTest()
		{
			MonthlyJobScheduleFrequency monthlyFrequency = PrepareMonthlyJobSchedule(1, 5);

			DateTime now = DateTime.Now;
			DateTime startTime = new DateTime(now.Year, now.Month, 1);

			DateTime timePoint = startTime.AddDays(10) + new TimeSpan(10, 10, 10);

			TimeScope scope = monthlyFrequency.GetTimeScope(startTime, timePoint, _TimeOffset);

			Assert.IsNull(scope);
		}

		[TestMethod]
		[TestCategory("Schedule")]
		[Description("不匹配每月间隔月的固定时间的Scope")]
		public void NotMatchMonthInMonthlyFixdTimeJobScheduleFrequencyTest()
		{
			MonthlyJobScheduleFrequency monthlyFrequency = PrepareMonthlyJobSchedule(2, 5);

			DateTime now = DateTime.Now;
			DateTime startTime = new DateTime(now.Year, now.Month, 1);

			DateTime timePoint = startTime.AddMonths(1).AddDays(4) + new TimeSpan(10, 10, 10);

			TimeScope scope = monthlyFrequency.GetTimeScope(startTime, timePoint, _TimeOffset);

			Assert.IsNull(scope);
		}

		[TestMethod]
		[TestCategory("Schedule")]
		[Description("不匹配每月间隔月的固定时间的Scope")]
		public void MatchMonthInMonthlyFixdTimeJobScheduleFrequencyTest()
		{
			MonthlyJobScheduleFrequency monthlyFrequency = PrepareMonthlyJobSchedule(2, 5);

			DateTime now = DateTime.Now;
			DateTime startTime = new DateTime(now.Year, now.Month, 1);

			DateTime timePoint = startTime.AddMonths(2).AddDays(4) + new TimeSpan(10, 10, 10);

			TimeScope scope = monthlyFrequency.GetTimeScope(startTime, timePoint, _TimeOffset);

			Assert.IsNotNull(scope);

			Console.WriteLine(scope.BeginTime);
			Console.WriteLine(scope.EndTime);

			Assert.AreEqual(new TimeSpan(10, 10, 10) - _TimeOffset, scope.BeginTime);
			Assert.AreEqual(new TimeSpan(10, 10, 10) + _TimeOffset, scope.EndTime);
		}

		[TestMethod]
		[TestCategory("Schedule")]
		[Description("预测间隔两月，间隔5天，调度接下来的执行时间")]
		public void EstimateMonthlyFixdTimeJobScheduleFrequencyTest()
		{
			MonthlyJobScheduleFrequency monthlyFrequency = PrepareMonthlyJobSchedule(2, 5);

			List<DateTime> estimateTime = monthlyFrequency.EstimateExecuteTime(DateTime.Now, _TimeOffset, 20, TimeSpan.FromSeconds(10));

			estimateTime.ForEach(d => Console.WriteLine(d.ToString("yyyy-MM-dd HH:mm:ss")));
		}

		private static MonthlyJobScheduleFrequency PrepareMonthlyJobSchedule(int durationMonth, int day)
		{
			return new MonthlyJobScheduleFrequency(day, durationMonth, new FixedTimeFrequency(new TimeSpan(10, 10, 10)));
		}
	}
}
