using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MCS.Library.Core;

namespace MCS.Library.SOA.DataObjects.Security.Locks
{
	/// <summary>
	/// 锁类型定义
	/// </summary>
	public enum SCLockType
	{
		None = 0,

		[EnumItemDescription("数据操作锁")]
		DataOperation = 2,

		[EnumItemDescription("同步操作锁")]
		Synchronize = 3
	}
}
