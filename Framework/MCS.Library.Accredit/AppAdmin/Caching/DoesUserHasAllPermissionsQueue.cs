#region
// -------------------------------------------------
// Assembly	：	DeluxeWorks.Library.Accredit
// FileName	：	DoesUserHasAllPermissionsQueue.cs
// Remark	：		DoesUserHasAllPermissions接口实现上的数据缓存队列的实现
// -------------------------------------------------
// VERSION  	AUTHOR				DATE					CONTENT
// 1.0				ccic\yuanyong		2008121630		新创建
// -------------------------------------------------
#endregion

using System;
using System.Collections.Generic;
using System.Text;
using MCS.Library.Caching;

namespace MCS.Library.Accredit.AppAdmin.Caching
{
	internal class DoesUserHasAllPermissionsQueue : CacheQueue<string, bool>
	{
		//internal static object CacheQueueSync = new object();
		public static readonly DoesUserHasAllPermissionsQueue Instance = CacheManager.GetInstance<DoesUserHasAllPermissionsQueue>();
		private DoesUserHasAllPermissionsQueue() { }
	}
}
