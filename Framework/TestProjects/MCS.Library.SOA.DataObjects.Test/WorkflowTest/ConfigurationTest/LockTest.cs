using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using MCS.Library.OGUPermission;

namespace MCS.Library.SOA.DataObjects.Test.WorkflowTest
{
	[TestClass]
	public class Lock
	{
		[TestMethod]
		[Description("设置锁")]
		[TestCategory(ProcessTestHelper.Lock)]
		public void LockSettingsSaveTest()
		{
			string lockId = "LockID1";
			IUser user = (IUser)OguObjectSettings.GetConfig().Objects["requestor"].Object;

			MCS.Library.SOA.DataObjects.Lock _lock = GetInstanceOfLock(lockId, user);

			LockAdapter.SetLock(_lock);

			CheckLockResult result = LockAdapter.CheckLock(lockId, user.ID);

			Assert.AreEqual(user.ID, result.PersonID);
			Assert.AreEqual(lockId, result.CurrentLock.LockID);

			LockAdapter.Unlock(lockId, user.ID);
		}

		[TestMethod]
		[Description("解锁并重新设置锁")]
		[TestCategory(ProcessTestHelper.Lock)]
		public void LockSettingsUnLockTest()
		{
			string lockId = "LockID2";
			IUser user = (IUser)OguObjectSettings.GetConfig().Objects["approver1"].Object;

			MCS.Library.SOA.DataObjects.Lock _lock = GetInstanceOfLock(lockId, user);

			LockAdapter.SetLock(_lock);
			CheckLockResult result = LockAdapter.CheckLock(lockId, user.ID);
			Assert.IsNotNull(result.CurrentLock);

			LockAdapter.Unlock(lockId, user.ID);

			CheckLockResult checkResult = LockAdapter.CheckLock(lockId, user.ID);
			Assert.IsNull(checkResult.CurrentLock);

			IUser user2 = (IUser)OguObjectSettings.GetConfig().Objects["approver2"].Object;
			result.CurrentLock.PersonID = user2.ID;
			LockAdapter.SetLock(result.CurrentLock);

			CheckLockResult reResult = LockAdapter.CheckLock(lockId, user2.ID);
			Assert.IsNotNull(reResult.CurrentLock);

			LockAdapter.Unlock(lockId, user2.ID);
		}

		[TestMethod]
		[Description("强制加锁")]
		[TestCategory(ProcessTestHelper.Lock)]
		public void LockSettingsFoceLockTest()
		{
			string lockId = "LockID3";
			IUser user = (IUser)OguObjectSettings.GetConfig().Objects["approver1"].Object;

			MCS.Library.SOA.DataObjects.Lock _lock = GetInstanceOfLock(lockId, user);
			_lock.LockType = LockType.AdminLock;
			LockAdapter.SetLock(_lock);

			CheckLockResult result = LockAdapter.CheckLock(_lock.LockID, user.ID);

			IUser user2 = (IUser)OguObjectSettings.GetConfig().Objects["approver2"].Object;
			result.CurrentLock.LockType = LockType.ActivityLock;
			result.CurrentLock.PersonID = user2.ID;
			LockAdapter.SetLock(result.CurrentLock, true);

			LockAdapter.Unlock(result.CurrentLock.LockID, user.ID);
		}

		private static MCS.Library.SOA.DataObjects.Lock GetInstanceOfLock(string lockId, IUser user)
		{
			MCS.Library.SOA.DataObjects.Lock _lock = new DataObjects.Lock();

			_lock.LockID = lockId;
			_lock.LockTime = DateTime.Now;
			_lock.LockType = LockType.ActivityLock;
			double db = 0.0;
			_lock.EffectiveTime = TimeSpan.FromMinutes(db);
			_lock.PersonID = user.ID;

			return _lock;
		}

	}
}
