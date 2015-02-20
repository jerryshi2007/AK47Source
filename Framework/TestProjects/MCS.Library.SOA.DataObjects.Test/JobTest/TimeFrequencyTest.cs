using System;
using System.Text;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace MCS.Library.SOA.DataObjects.Test.JobTest
{
	[TestClass]
	public class TimeFrequencyTest
	{
		private static readonly TimeSpan _TimeOffset = new TimeSpan(0, 0, 30);

		[TestMethod]
		[TestCategory("Schedule")]
		public void FixedTimeFrequencyTest()
		{
			TimeSpan occurTime = new TimeSpan(10, 10, 10);
			FixedTimeFrequency frequency = new FixedTimeFrequency(occurTime);

			Console.WriteLine(frequency.Description);

			TimeScope scope = frequency.GetTimeScope(new TimeSpan(10, 10, 14), _TimeOffset);

			Assert.IsNotNull(scope);

			Console.WriteLine(scope.BeginTime);
			Console.WriteLine(scope.EndTime);

			Assert.AreEqual(occurTime - _TimeOffset, scope.BeginTime);
			Assert.AreEqual(occurTime + _TimeOffset, scope.EndTime);
		}

		[TestMethod]
		[TestCategory("Schedule")]
		public void BeforeMidnightFixedTimeFrequencyTest()
		{
			TimeSpan occurTime = new TimeSpan(23, 59, 58);
			FixedTimeFrequency frequency = new FixedTimeFrequency(occurTime);

			Console.WriteLine(frequency.Description);

			TimeScope scope = frequency.GetTimeScope(new TimeSpan(1, 0, 0, 3), _TimeOffset);

			Assert.IsNotNull(scope);

			Console.WriteLine(scope.BeginTime);
			Console.WriteLine(scope.EndTime);

			Assert.AreEqual(occurTime - _TimeOffset, scope.BeginTime);
			Assert.AreEqual(occurTime + _TimeOffset, scope.EndTime);
		}

		[TestMethod]
		[TestCategory("Schedule")]
		public void AfterMidnightFixedTimeFrequencyTest()
		{
			TimeSpan occurTime = new TimeSpan(0, 0, 0);
			FixedTimeFrequency frequency = new FixedTimeFrequency(occurTime);

			Console.WriteLine(frequency.Description);

			TimeScope scope = frequency.GetTimeScope(new TimeSpan(0, 0, 5), _TimeOffset);

			Assert.IsNotNull(scope);

			Console.WriteLine(scope.BeginTime);
			Console.WriteLine(scope.EndTime);

			Assert.AreEqual(occurTime - _TimeOffset, scope.BeginTime);
			Assert.AreEqual(occurTime + _TimeOffset, scope.EndTime);
		}

		[TestMethod]
		[TestCategory("Schedule")]
		[Description("10:00:00 到 22:00:00之间，每 10秒，测试时间在区间内")]
		public void InRecurringTimeFrequencyTest()
		{
			TimeSpan sTime = new TimeSpan(10, 0, 0);
			TimeSpan eTime = new TimeSpan(22, 0, 0);

			TimeFrequencyBase frequency = new RecurringTimeFrequency(10, IntervalUnit.Second, sTime, eTime);
			Console.WriteLine(frequency.Description);

			TimeSpan lastExecuteTime = new TimeSpan(10, 11, 11);
			TimeScope scope = frequency.GetTimeScope(lastExecuteTime, _TimeOffset);

			Assert.IsNotNull(scope);
			Console.WriteLine(scope.BeginTime);
			Console.WriteLine(scope.EndTime);

			Assert.AreEqual(new TimeSpan(10, 11, 10), scope.BeginTime);
			Assert.AreEqual(new TimeSpan(10, 11, 20), scope.EndTime);
		}

		[TestMethod]
		[TestCategory("Schedule")]
		[Description("10:00:00 到 22:00:00之间，每 10秒，测试时间在区间前")]
		public void BeforeRecurringTimeFrequencyTest()
		{
			TimeSpan sTime = new TimeSpan(10, 0, 0);
			TimeSpan eTime = new TimeSpan(22, 0, 0);

			TimeFrequencyBase frequency = new RecurringTimeFrequency(10, IntervalUnit.Second, sTime, eTime);
			Console.WriteLine(frequency.Description);

			TimeSpan lastExecuteTime = new TimeSpan(9, 59, 59);
			TimeScope scope = frequency.GetTimeScope(lastExecuteTime, _TimeOffset);

			Assert.IsNull(scope);
		}

		[TestMethod]
		[TestCategory("Schedule")]
		[Description("10:00:00 到 22:00:00之间，每 10秒，测试时间在区间后")]
		public void AfterRecurringTimeFrequencyTest()
		{
			TimeSpan sTime = new TimeSpan(10, 0, 0);
			TimeSpan eTime = new TimeSpan(22, 0, 0);

			TimeFrequencyBase frequency = new RecurringTimeFrequency(10, IntervalUnit.Second, sTime, eTime);
			Console.WriteLine(frequency.Description);

			TimeSpan lastExecuteTime = new TimeSpan(22, 0, 1);
			TimeScope scope = frequency.GetTimeScope(lastExecuteTime, _TimeOffset);

			Assert.IsNull(scope);
		}
	}
}
