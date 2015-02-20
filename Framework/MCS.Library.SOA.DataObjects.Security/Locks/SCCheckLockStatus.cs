using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MCS.Library.Core;

namespace MCS.Library.SOA.DataObjects.Security.Locks
{
	/// <summary>
	/// 锁检查的状态
	/// </summary>
	public enum SCCheckLockStatus
	{
		[EnumItemDescription("没有上锁")]
		NotLocked,

		[EnumItemDescription("上锁但是已经过期")]
		LockExpired,

		[EnumItemDescription("已经上锁")]
		Locked
	}
}
