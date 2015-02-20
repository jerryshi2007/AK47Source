#region
// -------------------------------------------------
// Assembly	：	DeluxeWorks.Library.Accredit
// FileName	：	IsAdminUserQueue.cs
// Remark	：		IsAdminUser接口实现上的数据缓存队列的实现
// -------------------------------------------------
// VERSION  	AUTHOR				DATE					CONTENT
// 1.0				ccic\yuanyong		2008121630		新创建
// -------------------------------------------------
#endregion

using System;
using System.Collections.Generic;
using System.Text;
using System.Data;

using MCS.Library.Caching;

namespace MCS.Library.Accredit.AppAdmin.Caching
{
	internal class IsAdminUserQueue : CacheQueue<string, bool>
	{
		//internal static object CacheQueueSync = new object();
		public static readonly IsAdminUserQueue Instance = CacheManager.GetInstance<IsAdminUserQueue>();
		private IsAdminUserQueue() { }
	}
}
