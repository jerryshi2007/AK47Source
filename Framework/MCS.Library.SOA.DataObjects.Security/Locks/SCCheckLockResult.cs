using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MCS.Library.SOA.DataObjects.Security.Locks
{
	/// <summary>
	/// 锁检查状态
	/// </summary>
	[Serializable]
	public class SCCheckLockResult
	{
		public bool Available
		{
			get
			{
				return this.LockStatus == SCCheckLockStatus.NotLocked || this.LockStatus == SCCheckLockStatus.LockExpired;
			}
		}

		public SCCheckLockStatus LockStatus
		{
			get;
			set;
		}

		private SCLock _Lock = null;

		public SCLock Lock
		{
			get
			{
				return this._Lock;
			}
			internal set
			{
				this._Lock = value;
			}
		}
	}
}
