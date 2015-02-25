using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MCS.Library.Core;
using MCS.Library.SOA.DataObjects.Security.Locks;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Threading;

namespace MCS.Library.SOA.DataObjects.Security.Test.Locks
{
	[TestClass]
	public class LockTest
	{
		[TestMethod]
		[TestCategory("Locks")]
		[Description("增加一个新的锁")]
		public void AddNewLockTest()
		{
			SCLock lockData = new SCLock();
			lockData.LockID = UuidHelper.NewUuidString();

			SCCheckLockResult result = SCLockAdapter.Instance.AddLock(lockData);

			Assert.IsTrue(result.Available);
			Assert.IsTrue(result.Lock.LockTime > DateTime.MinValue);
		}

		[TestMethod]
		[TestCategory("Locks")]
		[Description("试图增加一个在有效期内的已经存在的锁")]
		public void AddExistedNewLockTest()
		{
			SCLock lockData = new SCLock();
			lockData.LockID = UuidHelper.NewUuidString();

			SCCheckLockResult result = SCLockAdapter.Instance.AddLock(lockData);

			//重复加锁
			result = SCLockAdapter.Instance.AddLock(lockData);

			Assert.IsFalse(result.Available);
			Assert.AreEqual(SCCheckLockStatus.Locked, result.LockStatus);
		}

		[TestMethod]
		[TestCategory("Locks")]
		[Description("增加一个在有效期之外的已经存在的锁")]
		public void AddExistedExpiredNewLockTest()
		{
			SCLock lockData = new SCLock();
			lockData.EffectiveTime = TimeSpan.FromSeconds(1);
			lockData.LockID = UuidHelper.NewUuidString();

			SCCheckLockResult result = SCLockAdapter.Instance.AddLock(lockData);

			Thread.Sleep(1100);

			//重复加锁
			result = SCLockAdapter.Instance.AddLock(lockData);

			Assert.IsTrue(result.Available);
			Assert.AreEqual(SCCheckLockStatus.LockExpired, result.LockStatus);
			Assert.IsTrue(result.Lock.LockTime > DateTime.MinValue);
		}

		[TestMethod]
		[TestCategory("Locks")]
		[Description("试图延长一个在有效期内的已经存在的锁")]
		public void ExtendExistedLockTest()
		{
			SCLock lockData = new SCLock();
			lockData.LockID = UuidHelper.NewUuidString();

			SCCheckLockResult result = SCLockAdapter.Instance.AddLock(lockData);
			
			Thread.Sleep(500);

			//延长
			SCCheckLockResult extendedResult = SCLockAdapter.Instance.ExtendLockTime(lockData);

			Assert.IsTrue(extendedResult.Lock.LockTime > result.Lock.LockTime);

			Console.WriteLine("Original time: {0:yyyy-MM-dd HH:mm:ss.ffff}, extended time: {1:yyyy-MM-dd HH:mm:ss.ffff}",
				result.Lock.LockTime, extendedResult.Lock.LockTime);
		}

		[TestMethod]
		[TestCategory("Locks")]
		[Description("试图延长一个在过期的已经存在的锁")]
		public void ExtendExistedExpiredLockTest()
		{
			SCLock lockData = new SCLock();
			lockData.LockID = UuidHelper.NewUuidString();
			lockData.EffectiveTime = TimeSpan.FromSeconds(1);

			SCCheckLockResult result = SCLockAdapter.Instance.AddLock(lockData);

			Thread.Sleep(1100);

			//延长
			SCCheckLockResult extendedResult = SCLockAdapter.Instance.ExtendLockTime(lockData);

			Assert.IsTrue(extendedResult.Lock.LockTime > result.Lock.LockTime);

			Console.WriteLine("Original time: {0:yyyy-MM-dd HH:mm:ss.ffff}, extended time: {1:yyyy-MM-dd HH:mm:ss.ffff}",
				result.Lock.LockTime, extendedResult.Lock.LockTime);
		}
	}
}
